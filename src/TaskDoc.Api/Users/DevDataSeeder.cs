using Microsoft.Data.Sqlite;
using TaskDoc.Api.Auth;
using TaskDoc.Api.Data;
using TaskDoc.Contracts.Users;

namespace TaskDoc.Api.Users;

/// <summary>
/// Development-only test data: users with known passwords, so a fresh machine can sign in right away.
/// Run with <c>dotnet run --project src/TaskDoc.Api --launch-profile http -- seed-dev [--reset]</c>
/// (or scripts/setup-dev-db.sh). Refuses to run outside the Development environment, because it creates an
/// administrator whose password is published in the README.
/// </summary>
public static class DevDataSeeder
{
    public const string Command = "seed-dev";
    public const string ResetOption = "--reset";

    /// <summary>The users created by <see cref="SeedAsync"/>. Passwords are documented in README.md.</summary>
    public static IReadOnlyList<DevUser> Users { get; } =
    [
        new("admin", "管理者", UserAuthType.Local, "Admin-pass-1", IsAdmin: true, IsActive: true),
        new("yamada", "山田 太郎", UserAuthType.Local, "User-pass-1", IsAdmin: false, IsActive: true),
        new("sato", "佐藤 花子", UserAuthType.Ad, Password: null, IsAdmin: false, IsActive: true),
        new("suzuki", "鈴木 次郎", UserAuthType.Local, "User-pass-1", IsAdmin: false, IsActive: false),
    ];

    public sealed record DevUser(
        string UserName, string DisplayName, UserAuthType AuthType, string? Password, bool IsAdmin, bool IsActive);

    public static bool IsRequested(string[] args) => args.Contains(Command, StringComparer.OrdinalIgnoreCase);

    /// <summary>Deletes the SQLite database file (and its -wal / -shm files) so it is recreated from scratch.</summary>
    public static void DeleteDatabase(string connectionString)
    {
        var dataSource = new SqliteConnectionStringBuilder(connectionString).DataSource;
        if (string.IsNullOrEmpty(dataSource) || dataSource == ":memory:")
        {
            return;
        }

        SqliteConnection.ClearAllPools();
        var path = Path.GetFullPath(dataSource);
        foreach (var file in new[] { path, path + "-wal", path + "-shm" })
        {
            try
            {
                File.Delete(file);
            }
            catch (IOException ex)
            {
                // Windows does not allow deleting a file another process has open (typically a running API).
                throw new InvalidOperationException(
                    $"データベースファイルを削除できません（使用中の可能性があります）: {file}{Environment.NewLine}" +
                    "API を止めてから、もう一度実行してください。", ex);
            }
        }
    }

    /// <param name="Added">Sample users created now.</param>
    /// <param name="Skipped">
    /// Sample user names that already existed and were left unchanged — their passwords may differ from the
    /// documented ones (e.g. an "admin" created by InitialAdminSeeder with a random password).
    /// </param>
    public sealed record SeedResult(IReadOnlyList<string> Added, IReadOnlyList<string> Skipped);

    /// <summary>
    /// Adds the sample users that do not exist yet. Existing users with the same name are left unchanged,
    /// so running it twice is harmless.
    /// </summary>
    public static async Task<SeedResult> SeedAsync(IServiceProvider services, CancellationToken ct = default)
    {
        var environment = services.GetRequiredService<IHostEnvironment>();
        if (!environment.IsDevelopment())
        {
            throw new InvalidOperationException(
                $"'{Command}' creates users with published passwords and only runs in the Development environment " +
                $"(current: {environment.EnvironmentName}).");
        }

        var users = services.GetRequiredService<UserRepository>();
        var hasher = services.GetRequiredService<PasswordHasher>();
        var now = services.GetRequiredService<TimeProvider>().GetUtcNow();
        var added = new List<string>();
        var skipped = new List<string>();

        foreach (var user in Users)
        {
            if (await users.FindByUserNameAsync(user.UserName, ct) is not null)
            {
                skipped.Add(user.UserName);
                continue;
            }

            var hash = user.Password is null ? null : hasher.Hash(user.Password);
            var id = await users.InsertAsync(
                user.UserName, user.DisplayName, null, user.AuthType, hash, user.IsAdmin, now, ct);
            if (!user.IsActive)
            {
                await users.UpdateProfileAsync(id, user.DisplayName, null, user.IsAdmin, isActive: false, now, ct);
            }

            added.Add(user.UserName);
        }

        return new SeedResult(added, skipped);
    }
}
