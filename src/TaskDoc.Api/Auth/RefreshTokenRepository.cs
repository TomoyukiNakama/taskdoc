using Dapper;
using TaskDoc.Api.Data;

namespace TaskDoc.Api.Auth;

public sealed class RefreshTokenRecord
{
    public long Id { get; init; }
    public long UserId { get; init; }
    public DateTimeOffset ExpiresAt { get; init; }
    public DateTimeOffset? RevokedAt { get; init; }
}

public sealed class RefreshTokenRepository(IDbConnectionFactory connections)
{
    public async Task InsertAsync(long userId, string tokenHash, DateTimeOffset now, DateTimeOffset expiresAt, CancellationToken ct)
    {
        await using var db = await connections.OpenAsync(ct);
        await db.ExecuteAsync(new CommandDefinition("""
            INSERT INTO RefreshTokens (UserId, TokenHash, CreatedAt, ExpiresAt)
            VALUES (@userId, @tokenHash, @now, @expiresAt)
            """,
            new { userId, tokenHash, now, expiresAt }, cancellationToken: ct));
    }

    public async Task<RefreshTokenRecord?> FindByHashAsync(string tokenHash, CancellationToken ct)
    {
        await using var db = await connections.OpenAsync(ct);
        return await db.QuerySingleOrDefaultAsync<RefreshTokenRecord>(new CommandDefinition(
            "SELECT Id, UserId, ExpiresAt, RevokedAt FROM RefreshTokens WHERE TokenHash = @tokenHash",
            new { tokenHash }, cancellationToken: ct));
    }

    /// <summary>
    /// Atomically revokes <paramref name="oldId"/> and stores its replacement.
    /// Returns false if the old token was already revoked (a concurrent or replayed refresh).
    /// </summary>
    public async Task<bool> RotateAsync(
        long oldId, long userId, string newTokenHash, DateTimeOffset now, DateTimeOffset expiresAt, CancellationToken ct)
    {
        await using var db = await connections.OpenAsync(ct);
        await using var tx = await db.BeginTransactionAsync(ct);

        var newId = await db.ExecuteScalarAsync<long>(new CommandDefinition("""
            INSERT INTO RefreshTokens (UserId, TokenHash, CreatedAt, ExpiresAt)
            VALUES (@userId, @newTokenHash, @now, @expiresAt)
            RETURNING Id
            """,
            new { userId, newTokenHash, now, expiresAt }, tx, cancellationToken: ct));

        var revoked = await db.ExecuteAsync(new CommandDefinition(
            "UPDATE RefreshTokens SET RevokedAt = @now, ReplacedById = @newId WHERE Id = @oldId AND RevokedAt IS NULL",
            new { oldId, newId, now }, tx, cancellationToken: ct));

        if (revoked == 0)
        {
            await tx.RollbackAsync(ct);
            return false;
        }

        await tx.CommitAsync(ct);
        return true;
    }

    public async Task RevokeAsync(long id, DateTimeOffset now, CancellationToken ct)
    {
        await using var db = await connections.OpenAsync(ct);
        await db.ExecuteAsync(new CommandDefinition(
            "UPDATE RefreshTokens SET RevokedAt = @now WHERE Id = @id AND RevokedAt IS NULL",
            new { id, now }, cancellationToken: ct));
    }

    /// <summary>Signs the user out everywhere (password change, deactivation, token replay).</summary>
    public async Task RevokeAllForUserAsync(long userId, DateTimeOffset now, CancellationToken ct)
    {
        await using var db = await connections.OpenAsync(ct);
        await db.ExecuteAsync(new CommandDefinition(
            "UPDATE RefreshTokens SET RevokedAt = @now WHERE UserId = @userId AND RevokedAt IS NULL",
            new { userId, now }, cancellationToken: ct));
    }
}
