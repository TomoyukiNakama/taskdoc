using Dapper;
using TaskDoc.Api.Data;
using TaskDoc.Contracts.Users;

namespace TaskDoc.Api.Users;

public sealed class UserRepository(IDbConnectionFactory connections)
{
    private const string Columns = """
        Id, UserName, DisplayName, Email, AuthType, PasswordHash, IsAdmin, IsActive,
        FailedLoginCount, LockoutEndAt, LastLoginAt, CreatedAt, UpdatedAt
        """;

    public async Task<UserRecord?> FindByIdAsync(long id, CancellationToken ct)
    {
        await using var db = await connections.OpenAsync(ct);
        return await db.QuerySingleOrDefaultAsync<UserRecord>(new CommandDefinition(
            $"SELECT {Columns} FROM Users WHERE Id = @id", new { id }, cancellationToken: ct));
    }

    /// <summary>Case-insensitive (the column is COLLATE NOCASE).</summary>
    public async Task<UserRecord?> FindByUserNameAsync(string userName, CancellationToken ct)
    {
        await using var db = await connections.OpenAsync(ct);
        return await db.QuerySingleOrDefaultAsync<UserRecord>(new CommandDefinition(
            $"SELECT {Columns} FROM Users WHERE UserName = @userName", new { userName }, cancellationToken: ct));
    }

    public async Task<IReadOnlyList<UserRecord>> ListAsync(CancellationToken ct)
    {
        await using var db = await connections.OpenAsync(ct);
        var rows = await db.QueryAsync<UserRecord>(new CommandDefinition(
            $"SELECT {Columns} FROM Users ORDER BY UserName", cancellationToken: ct));
        return rows.AsList();
    }

    public async Task<long> CountAsync(CancellationToken ct)
    {
        await using var db = await connections.OpenAsync(ct);
        return await db.ExecuteScalarAsync<long>(new CommandDefinition("SELECT COUNT(*) FROM Users", cancellationToken: ct));
    }

    public async Task<long> CountActiveAdminsAsync(CancellationToken ct)
    {
        await using var db = await connections.OpenAsync(ct);
        return await db.ExecuteScalarAsync<long>(new CommandDefinition(
            "SELECT COUNT(*) FROM Users WHERE IsAdmin = 1 AND IsActive = 1", cancellationToken: ct));
    }

    public async Task<long> InsertAsync(
        string userName, string displayName, string? email, UserAuthType authType,
        string? passwordHash, bool isAdmin, DateTimeOffset now, CancellationToken ct)
    {
        await using var db = await connections.OpenAsync(ct);
        return await db.ExecuteScalarAsync<long>(new CommandDefinition("""
            INSERT INTO Users (UserName, DisplayName, Email, AuthType, PasswordHash, IsAdmin, IsActive, CreatedAt, UpdatedAt)
            VALUES (@userName, @displayName, @email, @authType, @passwordHash, @isAdmin, 1, @now, @now)
            RETURNING Id
            """,
            new { userName, displayName, email, authType = authType.ToString(), passwordHash, isAdmin, now },
            cancellationToken: ct));
    }

    public async Task UpdateProfileAsync(
        long id, string displayName, string? email, bool isAdmin, bool isActive, DateTimeOffset now, CancellationToken ct)
    {
        await using var db = await connections.OpenAsync(ct);
        await db.ExecuteAsync(new CommandDefinition("""
            UPDATE Users
            SET DisplayName = @displayName, Email = @email, IsAdmin = @isAdmin, IsActive = @isActive, UpdatedAt = @now
            WHERE Id = @id
            """,
            new { id, displayName, email, isAdmin, isActive, now }, cancellationToken: ct));
    }

    /// <summary>Also clears any lockout, so an admin password reset unlocks the account.</summary>
    public async Task SetPasswordHashAsync(long id, string passwordHash, DateTimeOffset now, CancellationToken ct)
    {
        await using var db = await connections.OpenAsync(ct);
        await db.ExecuteAsync(new CommandDefinition("""
            UPDATE Users
            SET PasswordHash = @passwordHash, FailedLoginCount = 0, LockoutEndAt = NULL, UpdatedAt = @now
            WHERE Id = @id AND AuthType = 'Local'
            """,
            new { id, passwordHash, now }, cancellationToken: ct));
    }

    public async Task RecordFailedLoginAsync(long id, int failedLoginCount, DateTimeOffset? lockoutEndAt, CancellationToken ct)
    {
        await using var db = await connections.OpenAsync(ct);
        await db.ExecuteAsync(new CommandDefinition(
            "UPDATE Users SET FailedLoginCount = @failedLoginCount, LockoutEndAt = @lockoutEndAt WHERE Id = @id",
            new { id, failedLoginCount, lockoutEndAt }, cancellationToken: ct));
    }

    public async Task RecordSuccessfulLoginAsync(long id, DateTimeOffset now, CancellationToken ct)
    {
        await using var db = await connections.OpenAsync(ct);
        await db.ExecuteAsync(new CommandDefinition(
            "UPDATE Users SET FailedLoginCount = 0, LockoutEndAt = NULL, LastLoginAt = @now WHERE Id = @id",
            new { id, now }, cancellationToken: ct));
    }
}
