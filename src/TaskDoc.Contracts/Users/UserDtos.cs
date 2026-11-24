namespace TaskDoc.Contracts.Users;

/// <summary>A registered user as returned by the API.</summary>
public sealed record UserDto(
    long Id,
    string UserName,
    string DisplayName,
    string? Email,
    UserAuthType AuthType,
    bool IsAdmin,
    bool IsActive,
    DateTimeOffset CreatedAt,
    DateTimeOffset? LastLoginAt);

/// <summary>
/// Registers a user. <paramref name="Password"/> is required for <see cref="UserAuthType.Local"/>
/// and must be null for <see cref="UserAuthType.Ad"/> (AD users sign in with their AD password).
/// For AD users, <paramref name="UserName"/> is the account name (sAMAccountName).
/// </summary>
public sealed record CreateUserRequest(
    string UserName,
    string DisplayName,
    string? Email,
    UserAuthType AuthType,
    string? Password,
    bool IsAdmin);

/// <summary>Updates a user's profile and status. The user name and auth type cannot change.</summary>
public sealed record UpdateUserRequest(
    string DisplayName,
    string? Email,
    bool IsAdmin,
    bool IsActive);

/// <summary>Admin sets a new password for a local user.</summary>
public sealed record SetPasswordRequest(string NewPassword);

/// <summary>A local user changes their own password.</summary>
public sealed record ChangePasswordRequest(string CurrentPassword, string NewPassword);
