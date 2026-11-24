using TaskDoc.Contracts.Users;

namespace TaskDoc.Contracts.Auth;

/// <summary>
/// Sign-in for both local and AD users. The API looks the user up by name and checks the
/// password against the database or Active Directory depending on the user's auth type.
/// </summary>
public sealed record LoginRequest(string UserName, string Password);

public sealed record RefreshRequest(string RefreshToken);

public sealed record LogoutRequest(string RefreshToken);

/// <summary>
/// Issued on login and refresh. Send <paramref name="AccessToken"/> as a Bearer token.
/// Each refresh token works once: refreshing returns a new one and revokes the old one.
/// </summary>
public sealed record TokenResponse(
    string AccessToken,
    DateTimeOffset AccessTokenExpiresAt,
    string RefreshToken,
    DateTimeOffset RefreshTokenExpiresAt,
    UserDto User);
