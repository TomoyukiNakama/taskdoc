using Microsoft.Extensions.Options;
using TaskDoc.Api.Users;
using TaskDoc.Contracts.Auth;
using TaskDoc.Contracts.Users;

namespace TaskDoc.Api.Auth;

public enum LoginFailure
{
    /// <summary>Unknown user, wrong password, inactive or locked account. Deliberately not distinguished to the caller.</summary>
    InvalidCredentials,

    /// <summary>Active Directory could not be reached.</summary>
    DirectoryUnavailable,
}

public sealed record LoginResult(TokenResponse? Tokens, LoginFailure? Failure)
{
    public static LoginResult Ok(TokenResponse tokens) => new(tokens, null);
    public static LoginResult Fail(LoginFailure failure) => new(null, failure);
}

public sealed class AuthService(
    UserRepository users,
    RefreshTokenRepository refreshTokens,
    PasswordHasher passwordHasher,
    IAdAuthenticator adAuthenticator,
    TokenService tokenService,
    IOptions<AuthOptions> options,
    TimeProvider clock,
    ILogger<AuthService> logger)
{
    public async Task<LoginResult> LoginAsync(LoginRequest request, CancellationToken ct)
    {
        var userName = NormalizeUserName(request.UserName);
        var user = userName.Length == 0 ? null : await users.FindByUserNameAsync(userName, ct);

        if (user is null)
        {
            passwordHasher.VerifyDummy(request.Password ?? "");
            logger.LogInformation("Login rejected: user {UserName} is not registered.", userName);
            return LoginResult.Fail(LoginFailure.InvalidCredentials);
        }

        var now = clock.GetUtcNow();
        if (!user.IsActive)
        {
            logger.LogInformation("Login rejected: user {UserName} is inactive.", user.UserName);
            return LoginResult.Fail(LoginFailure.InvalidCredentials);
        }

        if (user.LockoutEndAt is { } lockoutEnd && lockoutEnd > now)
        {
            logger.LogInformation("Login rejected: user {UserName} is locked out until {LockoutEnd}.", user.UserName, lockoutEnd);
            return LoginResult.Fail(LoginFailure.InvalidCredentials);
        }

        var passwordOk = user.AuthType switch
        {
            UserAuthType.Local => user.PasswordHash is not null && passwordHasher.Verify(request.Password ?? "", user.PasswordHash),
            UserAuthType.Ad => await CheckAdPasswordAsync(user, request.Password ?? "", ct),
            _ => false,
        };

        if (passwordOk is null)
        {
            return LoginResult.Fail(LoginFailure.DirectoryUnavailable);
        }

        if (passwordOk == false)
        {
            await RecordFailureAsync(user, now, ct);
            return LoginResult.Fail(LoginFailure.InvalidCredentials);
        }

        await users.RecordSuccessfulLoginAsync(user.Id, now, ct);
        logger.LogInformation("User {UserName} signed in ({AuthType}).", user.UserName, user.AuthType);

        var signedIn = await users.FindByIdAsync(user.Id, ct) ?? user;
        return LoginResult.Ok(await StartSessionAsync(signedIn, ct));
    }

    /// <summary>Issues tokens for a new session (login, or re-issue after the user changed their password).</summary>
    public async Task<TokenResponse> StartSessionAsync(UserRecord user, CancellationToken ct) =>
        await IssueTokensAsync(user, refreshTokenToRotate: null, ct)
        ?? throw new InvalidOperationException("Issuing tokens for a new session cannot conflict.");

    /// <summary>Returns null when the refresh token is unknown, expired, revoked, or its user can no longer sign in.</summary>
    public async Task<TokenResponse?> RefreshAsync(RefreshRequest request, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(request.RefreshToken))
        {
            return null;
        }

        var stored = await refreshTokens.FindByHashAsync(TokenService.HashRefreshToken(request.RefreshToken), ct);
        if (stored is null)
        {
            return null;
        }

        var now = clock.GetUtcNow();
        if (stored.RevokedAt is not null)
        {
            // A revoked token came back: either it was stolen or replayed. End every session of this user.
            logger.LogWarning("Revoked refresh token reused for user {UserId}; revoking all sessions.", stored.UserId);
            await refreshTokens.RevokeAllForUserAsync(stored.UserId, now, ct);
            return null;
        }

        if (stored.ExpiresAt <= now)
        {
            return null;
        }

        var user = await users.FindByIdAsync(stored.UserId, ct);
        if (user is null || !user.IsActive)
        {
            await refreshTokens.RevokeAllForUserAsync(stored.UserId, now, ct);
            return null;
        }

        return await IssueTokensAsync(user, stored, ct);
    }

    public async Task LogoutAsync(LogoutRequest request, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(request.RefreshToken))
        {
            return;
        }

        var stored = await refreshTokens.FindByHashAsync(TokenService.HashRefreshToken(request.RefreshToken), ct);
        if (stored is not null)
        {
            await refreshTokens.RevokeAsync(stored.Id, clock.GetUtcNow(), ct);
        }
    }

    /// <summary>Accepts "user", "DOMAIN\user" and "user@domain" and returns "user".</summary>
    public static string NormalizeUserName(string? input)
    {
        var name = (input ?? "").Trim();
        var backslash = name.LastIndexOf('\\');
        if (backslash >= 0)
        {
            name = name[(backslash + 1)..];
        }

        var at = name.IndexOf('@');
        if (at >= 0)
        {
            name = name[..at];
        }

        return name;
    }

    /// <returns>true / false for the password check, or null when AD is unavailable.</returns>
    private async Task<bool?> CheckAdPasswordAsync(UserRecord user, string password, CancellationToken ct)
    {
        var result = await adAuthenticator.AuthenticateAsync(user.UserName, password, ct);
        return result switch
        {
            AdAuthResult.Success => true,
            AdAuthResult.InvalidCredentials => false,
            _ => null,
        };
    }

    private async Task RecordFailureAsync(UserRecord user, DateTimeOffset now, CancellationToken ct)
    {
        var lockout = options.Value.Lockout;
        var failures = user.FailedLoginCount + 1;
        DateTimeOffset? lockoutEnd = null;

        if (failures >= lockout.MaxFailedAttempts)
        {
            lockoutEnd = now + lockout.Duration;
            failures = 0;
            logger.LogWarning("User {UserName} locked out until {LockoutEnd} after repeated failed sign-ins.", user.UserName, lockoutEnd);
        }
        else
        {
            logger.LogInformation("Login rejected: wrong password for {UserName} ({Failures} consecutive).", user.UserName, failures);
        }

        await users.RecordFailedLoginAsync(user.Id, failures, lockoutEnd, ct);
    }

    /// <returns>null only when rotating and the old refresh token was revoked concurrently.</returns>
    private async Task<TokenResponse?> IssueTokensAsync(UserRecord user, RefreshTokenRecord? refreshTokenToRotate, CancellationToken ct)
    {
        var now = clock.GetUtcNow();
        var (accessToken, accessExpiresAt) = tokenService.CreateAccessToken(user);
        var (refreshToken, refreshHash) = TokenService.CreateRefreshToken();
        var refreshExpiresAt = now + options.Value.Jwt.RefreshTokenLifetime;

        if (refreshTokenToRotate is null)
        {
            await refreshTokens.InsertAsync(user.Id, refreshHash, now, refreshExpiresAt, ct);
        }
        else if (!await refreshTokens.RotateAsync(refreshTokenToRotate.Id, user.Id, refreshHash, now, refreshExpiresAt, ct))
        {
            return null;
        }

        return new TokenResponse(accessToken, accessExpiresAt, refreshToken, refreshExpiresAt, user.ToDto());
    }
}
