using Refit;
using TaskDoc.Contracts.Auth;
using TaskDoc.Contracts.Health;
using TaskDoc.Contracts.Users;

namespace TaskDoc.Client.Services;

/// <summary>
/// Refit client for TaskDoc.Api. Requests go through <see cref="AuthHeaderHandler"/>, which adds the
/// signed-in user's access token. Add endpoints here as the API grows (and new DTOs to TaskDocJsonContext).
/// </summary>
public interface ITaskDocApi
{
    [Get("/api/health")]
    Task<HealthResponse> GetHealthAsync(CancellationToken cancellationToken = default);

    [Get("/api/auth/me")]
    Task<UserDto> GetMeAsync(CancellationToken cancellationToken = default);

    /// <summary>Returns a fresh session; all other sessions of the user are signed out.</summary>
    [Put("/api/auth/me/password")]
    Task<TokenResponse> ChangeOwnPasswordAsync([Body] ChangePasswordRequest request, CancellationToken cancellationToken = default);

    // --- User management (system administrators only) ---

    [Get("/api/users")]
    Task<UserDto[]> GetUsersAsync(CancellationToken cancellationToken = default);

    [Post("/api/users")]
    Task<UserDto> CreateUserAsync([Body] CreateUserRequest request, CancellationToken cancellationToken = default);

    [Put("/api/users/{id}")]
    Task<UserDto> UpdateUserAsync(long id, [Body] UpdateUserRequest request, CancellationToken cancellationToken = default);

    [Put("/api/users/{id}/password")]
    Task SetUserPasswordAsync(long id, [Body] SetPasswordRequest request, CancellationToken cancellationToken = default);
}
