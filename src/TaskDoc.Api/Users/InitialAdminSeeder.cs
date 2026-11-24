using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using TaskDoc.Api.Auth;
using TaskDoc.Contracts.Users;

namespace TaskDoc.Api.Users;

/// <summary>
/// Only registered users can sign in, so a fresh database needs one admin to register everyone else.
/// When the Users table is empty, creates a local admin with the configured password (Auth:InitialAdmin:Password),
/// or with a random password that is written to the log once.
/// </summary>
public static class InitialAdminSeeder
{
    public static async Task SeedAsync(IServiceProvider services, CancellationToken ct = default)
    {
        var users = services.GetRequiredService<UserRepository>();
        if (await users.CountAsync(ct) > 0)
        {
            return;
        }

        var options = services.GetRequiredService<IOptions<AuthOptions>>().Value.InitialAdmin;
        var userName = options.UserName;
        var configured = !string.IsNullOrEmpty(options.Password);
        if (configured && PasswordHasher.ValidateStrength(options.Password) is { } error)
        {
            // Fail rather than create an admin with a weak password.
            throw new InvalidOperationException($"Auth:InitialAdmin:Password is invalid: {error}");
        }

        var password = configured
            ? options.Password!
            : RandomNumberGenerator.GetString("ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789", 16);
        var hash = services.GetRequiredService<PasswordHasher>().Hash(password);
        var now = services.GetRequiredService<TimeProvider>().GetUtcNow();

        await users.InsertAsync(userName, "管理者", null, UserAuthType.Local, hash, isAdmin: true, now, ct);

        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(InitialAdminSeeder));
        if (configured)
        {
            logger.LogWarning(
                "Created initial admin '{UserName}' with the configured password (Auth:InitialAdmin:Password).",
                userName);
        }
        else
        {
            logger.LogWarning(
                "Created initial admin '{UserName}' with password '{Password}'. Sign in and change this password now; it is shown only once.",
                userName, password);
        }
    }
}
