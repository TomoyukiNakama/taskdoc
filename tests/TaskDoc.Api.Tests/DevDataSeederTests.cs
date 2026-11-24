using System.Net;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using TaskDoc.Api.Data;
using TaskDoc.Api.Users;
using TaskDoc.Contracts.Users;

namespace TaskDoc.Api.Tests;

public sealed class DevDataSeederTests : IAsyncLifetime
{
    private readonly TaskDocApiFactory _api = new() { EnvironmentName = "Development" };

    public async ValueTask InitializeAsync()
    {
        // Starting the host runs migrations and InitialAdminSeeder, which creates a random-password "admin".
        // `seed-dev` itself runs before that seeder; emulate that fresh state by removing all users.
        _ = _api.CreateClient();
        await using var db = await _api.Services.GetRequiredService<IDbConnectionFactory>().OpenAsync();
        await db.ExecuteAsync("DELETE FROM Users");
    }

    public ValueTask DisposeAsync() => _api.DisposeAsync();

    [Fact]
    public async Task CreatesSampleUsersThatCanSignInAsDocumented()
    {
        var result = await DevDataSeeder.SeedAsync(_api.Services);

        Assert.Equal(["admin", "yamada", "sato", "suzuki"], result.Added);
        Assert.Empty(result.Skipped);
        Assert.Equal(HttpStatusCode.OK, (await _api.LoginAsync("admin", "Admin-pass-1")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await _api.LoginAsync("yamada", "User-pass-1")).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await _api.LoginAsync("suzuki", "User-pass-1")).StatusCode); // inactive

        var users = _api.Services.GetRequiredService<UserRepository>();
        Assert.True((await users.FindByUserNameAsync("admin", CancellationToken.None))!.IsAdmin);
        Assert.Equal(UserAuthType.Ad, (await users.FindByUserNameAsync("sato", CancellationToken.None))!.AuthType);
    }

    [Fact]
    public async Task RunningTwice_ChangesNothingAndReportsSkipped()
    {
        await DevDataSeeder.SeedAsync(_api.Services);

        var second = await DevDataSeeder.SeedAsync(_api.Services);

        Assert.Empty(second.Added);
        Assert.Equal(["admin", "yamada", "sato", "suzuki"], second.Skipped);
        Assert.Equal(4, (await _api.Services.GetRequiredService<UserRepository>().ListAsync(CancellationToken.None)).Count);
    }

    [Fact]
    public async Task ExistingUserIsNotOverwritten()
    {
        await _api.AddUserAsync("admin", password: "someone-elses-pass", isAdmin: true);

        var result = await DevDataSeeder.SeedAsync(_api.Services);

        Assert.Contains("admin", result.Skipped);
        Assert.Equal(HttpStatusCode.OK, (await _api.LoginAsync("admin", "someone-elses-pass")).StatusCode);
    }

    [Fact]
    public async Task RefusesOutsideDevelopment()
    {
        await using var testing = new TaskDocApiFactory(); // "Testing" environment
        _ = testing.CreateClient();

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => DevDataSeeder.SeedAsync(testing.Services));
        Assert.Contains("Development", ex.Message);
    }
}
