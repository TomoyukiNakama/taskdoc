using System.Net;
using Refit;
using TaskDoc.Contracts.Auth;
using TaskDoc.Contracts.Users;

namespace TaskDoc.App.Services;

/// <summary>
/// The signed-in user's tokens. Refreshes the access token shortly before it expires and signs the
/// user out when refreshing is no longer possible.
/// Tokens are kept in memory only, so each app start begins at the login screen.
/// </summary>
public sealed class AuthSession(IAuthApi authApi, TimeProvider clock)
{
    /// <summary>Refresh this long before the access token expires, to absorb clock and network delays.</summary>
    private static readonly TimeSpan RefreshMargin = TimeSpan.FromMinutes(1);

    private readonly SemaphoreSlim _refreshLock = new(1, 1);
    private TokenResponse? _tokens;

    public UserDto? CurrentUser => _tokens?.User;

    public bool IsSignedIn => _tokens is not null;

    /// <summary>Raised when the session ends without the user asking (expired or revoked).</summary>
    public event EventHandler? SessionExpired;

    /// <returns>null on success, otherwise a message to show on the login screen.</returns>
    public async Task<string?> SignInAsync(string userName, string password, CancellationToken ct = default)
    {
        try
        {
            _tokens = await authApi.LoginAsync(new LoginRequest(userName, password), ct);
            return null;
        }
        catch (ApiException ex)
        {
            return ApiErrors.FromProblemJson(ex.Content) ?? ex.StatusCode switch
            {
                HttpStatusCode.Unauthorized => "ユーザー名またはパスワードが正しくありません。",
                _ => $"ログインできませんでした（{(int)ex.StatusCode}）。",
            };
        }
        catch (ApiRequestException)
        {
            // Refit 17: the request could not be sent (server down, DNS, TLS...).
            return ApiErrors.ConnectionFailed;
        }
    }

    /// <summary>
    /// Switches to a session the API issued outside of login (after the user changed their password,
    /// the API signs out every session and returns a new one for this client).
    /// </summary>
    public void ReplaceSession(TokenResponse tokens) => _tokens = tokens;

    public async Task SignOutAsync()
    {
        var tokens = Interlocked.Exchange(ref _tokens, null);
        if (tokens is null)
        {
            return;
        }

        try
        {
            await authApi.LogoutAsync(new LogoutRequest(tokens.RefreshToken));
        }
        catch (ApiExceptionBase)
        {
            // The local session is gone either way; the server-side token simply expires.
        }
    }

    /// <summary>A valid access token, refreshed first if it is about to expire. Null when signed out.</summary>
    public async Task<string?> GetAccessTokenAsync(CancellationToken ct = default)
    {
        var tokens = _tokens;
        if (tokens is null)
        {
            return null;
        }

        if (tokens.AccessTokenExpiresAt - RefreshMargin > clock.GetUtcNow())
        {
            return tokens.AccessToken;
        }

        return await RefreshAsync(tokens, ct);
    }

    /// <summary>
    /// Called after the API rejected <paramref name="rejectedAccessToken"/> (for example, after a server restart
    /// changed the development signing key). Returns a new token, or null if the session has ended.
    /// </summary>
    public async Task<string?> RenewAfterRejectionAsync(string rejectedAccessToken, CancellationToken ct = default)
    {
        var tokens = _tokens;
        if (tokens is null)
        {
            return null;
        }

        // Another request may already have refreshed while this one was in flight.
        return tokens.AccessToken != rejectedAccessToken ? tokens.AccessToken : await RefreshAsync(tokens, ct);
    }

    private async Task<string?> RefreshAsync(TokenResponse expiring, CancellationToken ct)
    {
        // One refresh at a time: the server revokes every session if the same refresh token is used twice.
        await _refreshLock.WaitAsync(ct);
        try
        {
            var current = _tokens;
            if (current is null)
            {
                return null;
            }

            if (!ReferenceEquals(current, expiring))
            {
                return current.AccessToken; // refreshed by another caller while we waited
            }

            try
            {
                _tokens = await authApi.RefreshAsync(new RefreshRequest(current.RefreshToken), ct);
                return _tokens.AccessToken;
            }
            catch (ApiException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
            {
                _tokens = null;
                SessionExpired?.Invoke(this, EventArgs.Empty);
                return null;
            }
        }
        finally
        {
            _refreshLock.Release();
        }
    }
}
