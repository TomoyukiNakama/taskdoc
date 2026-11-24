using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using TaskDoc.Api.Auth;
using TaskDoc.Api.Users;
using TaskDoc.Contracts.Auth;
using TaskDoc.Contracts.Users;

namespace TaskDoc.Api.Tests;

/// <summary>
/// Hosts the API in-process against a throwaway SQLite file, so tests run the same SQL as production.
/// Time, AD and password hashing cost are replaced with test doubles.
/// </summary>
public sealed class TaskDocApiFactory : WebApplicationFactory<Program>
{
    /// <summary>
    /// Hosting environment. A property rather than a constructor argument because xUnit class fixtures
    /// must have a single public parameterless constructor. Set it before the first CreateClient().
    /// </summary>
    public string EnvironmentName { get; init; } = "Testing";

    private readonly string _databasePath =
        Path.Combine(Path.GetTempPath(), $"taskdoc-test-{Guid.NewGuid():N}.db");

    public FakeTimeProvider Clock { get; } = new(new DateTimeOffset(2026, 4, 1, 9, 0, 0, TimeSpan.Zero));

    public FakeAdAuthenticator Ad { get; } = new();

    /// <summary>Extra configuration values. Set them before the first request or service access.</summary>
    public Dictionary<string, string?> Settings { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(EnvironmentName);
        builder.UseSetting("ConnectionStrings:Default", $"Data Source={_databasePath}");
        builder.UseSetting("Auth:Jwt:SigningKey", "test-signing-key-test-signing-key-0123456789");
        builder.UseSetting("Logging:LogLevel:Default", "Error");
        foreach (var (key, value) in Settings)
        {
            builder.UseSetting(key, value);
        }

        builder.ConfigureTestServices(services =>
        {
            services.AddSingleton<TimeProvider>(Clock);
            services.AddSingleton<IAdAuthenticator>(Ad);
            services.AddSingleton(new PasswordHasher(iterations: 1_000));
        });
    }

    /// <summary>Registers a user directly in the database (bypassing the admin API).</summary>
    public async Task<long> AddUserAsync(
        string userName, string? password = "password123", bool isAdmin = false, UserAuthType authType = UserAuthType.Local)
    {
        var hash = authType == UserAuthType.Local ? Services.GetRequiredService<PasswordHasher>().Hash(password!) : null;
        return await Services.GetRequiredService<UserRepository>().InsertAsync(
            userName, $"{userName} さん", null, authType, hash, isAdmin, Clock.GetUtcNow(), CancellationToken.None);
    }

    public async Task<HttpResponseMessage> LoginAsync(string userName, string password) =>
        await CreateClient().PostAsJsonAsync("/api/auth/login", new LoginRequest(userName, password));

    public async Task<TokenResponse> LoginOkAsync(string userName, string password = "password123")
    {
        var response = await LoginAsync(userName, password);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<TokenResponse>())!;
    }

    /// <summary>An HttpClient that sends the given access token.</summary>
    public HttpClient CreateClient(string accessToken)
    {
        var client = CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        return client;
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        SqliteConnection.ClearAllPools();
        try
        {
            File.Delete(_databasePath);
        }
        catch (IOException)
        {
            // Best effort: on Windows the file can still be briefly locked. It lives in the temp folder anyway,
            // and failing here would turn a passing test into a cleanup error.
        }
    }
}

public sealed class FakeAdAuthenticator : IAdAuthenticator
{
    public Dictionary<string, string> Passwords { get; } = new(StringComparer.OrdinalIgnoreCase);

    public bool Unavailable { get; set; }

    public Task<AdAuthResult> AuthenticateAsync(string accountName, string password, CancellationToken cancellationToken) =>
        Task.FromResult(
            Unavailable ? AdAuthResult.Unavailable
            : Passwords.TryGetValue(accountName, out var expected) && expected == password ? AdAuthResult.Success
            : AdAuthResult.InvalidCredentials);
}
