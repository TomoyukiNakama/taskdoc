using TaskDoc.Contracts.Users;

namespace TaskDoc.Api.Users;

/// <summary>A row of the Users table.</summary>
public sealed class UserRecord
{
    public long Id { get; init; }
    public string UserName { get; init; } = "";
    public string DisplayName { get; init; } = "";
    public string? Email { get; init; }
    public UserAuthType AuthType { get; init; }
    public string? PasswordHash { get; init; }
    public bool IsAdmin { get; init; }
    public bool IsActive { get; init; }
    public int FailedLoginCount { get; init; }
    public DateTimeOffset? LockoutEndAt { get; init; }
    public DateTimeOffset? LastLoginAt { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset UpdatedAt { get; init; }

    public UserDto ToDto() =>
        new(Id, UserName, DisplayName, Email, AuthType, IsAdmin, IsActive, CreatedAt, LastLoginAt);
}
