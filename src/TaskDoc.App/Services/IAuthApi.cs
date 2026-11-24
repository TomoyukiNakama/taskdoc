using Refit;
using TaskDoc.Contracts.Auth;

namespace TaskDoc.App.Services;

/// <summary>Sign-in endpoints. This client sends no access token (it is how tokens are obtained).</summary>
public interface IAuthApi
{
    [Post("/api/auth/login")]
    Task<TokenResponse> LoginAsync([Body] LoginRequest request, CancellationToken cancellationToken = default);

    [Post("/api/auth/refresh")]
    Task<TokenResponse> RefreshAsync([Body] RefreshRequest request, CancellationToken cancellationToken = default);

    [Post("/api/auth/logout")]
    Task LogoutAsync([Body] LogoutRequest request, CancellationToken cancellationToken = default);
}
