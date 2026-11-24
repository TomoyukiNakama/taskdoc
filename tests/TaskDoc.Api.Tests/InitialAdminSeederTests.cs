using System.Net;

namespace TaskDoc.Api.Tests;

/// <summary>The initial admin is created on startup, so each test needs its own factory (and database).</summary>
public sealed class InitialAdminSeederTests : IAsyncLifetime
{
    private readonly TaskDocApiFactory _api = new();

    public ValueTask InitializeAsync() => ValueTask.CompletedTask;

    public ValueTask DisposeAsync() => _api.DisposeAsync();

    [Fact]
    public async Task ConfiguredPassword_InitialAdminCanSignIn()
    {
        _api.Settings["Auth:InitialAdmin:Password"] = "initial-pass-123";

        var tokens = await _api.LoginOkAsync("admin", "initial-pass-123");

        Assert.True(tokens.User.IsAdmin);
    }

    [Fact]
    public async Task ConfiguredUserNameAndPassword_AreBothUsed()
    {
        _api.Settings["Auth:InitialAdmin:UserName"] = "root";
        _api.Settings["Auth:InitialAdmin:Password"] = "initial-pass-123";

        await _api.LoginOkAsync("root", "initial-pass-123");
        var admin = await _api.LoginAsync("admin", "initial-pass-123");

        Assert.Equal(HttpStatusCode.Unauthorized, admin.StatusCode);
    }

    [Fact]
    public async Task NoConfiguredPassword_UsesRandomPassword()
    {
        var response = await _api.LoginAsync("admin", "initial-pass-123");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public void ConfiguredPasswordTooShort_StartupFails()
    {
        _api.Settings["Auth:InitialAdmin:Password"] = "short";

        var error = Assert.ThrowsAny<Exception>(() => _api.CreateClient());

        Assert.Contains("Auth:InitialAdmin:Password", error.ToString());
    }
}
