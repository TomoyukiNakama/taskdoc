using System.Net;
using System.Net.Http.Json;
using TaskDoc.Contracts.Auth;
using TaskDoc.Contracts.Users;

namespace TaskDoc.Api.Tests;

/// <summary>Each test gets its own factory (and database) so lockout and clock changes do not leak.</summary>
public sealed class AuthEndpointTests : IAsyncLifetime
{
    private readonly TaskDocApiFactory _api = new();

    public ValueTask InitializeAsync() => ValueTask.CompletedTask;

    public ValueTask DisposeAsync() => _api.DisposeAsync();

    [Fact]
    public async Task Login_LocalUser_ReturnsTokensAndUser()
    {
        await _api.AddUserAsync("taro");

        var tokens = await _api.LoginOkAsync("taro");

        Assert.False(string.IsNullOrEmpty(tokens.AccessToken));
        Assert.False(string.IsNullOrEmpty(tokens.RefreshToken));
        Assert.Equal("taro", tokens.User.UserName);
        Assert.Equal(UserAuthType.Local, tokens.User.AuthType);
        Assert.Equal(_api.Clock.GetUtcNow().AddMinutes(15), tokens.AccessTokenExpiresAt);
    }

    [Theory]
    [InlineData("TARO")]
    [InlineData(@"CORP\taro")]
    [InlineData("taro@corp.example.com")]
    public async Task Login_AcceptsUserNameVariants(string input)
    {
        await _api.AddUserAsync("taro");

        var response = await _api.LoginAsync(input, "password123");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Login_WrongPassword_Returns401()
    {
        await _api.AddUserAsync("taro");

        var response = await _api.LoginAsync("taro", "wrong-password");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_UnregisteredUser_Returns401()
    {
        var response = await _api.LoginAsync("nobody", "password123");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_AdUser_ChecksPasswordAgainstAd()
    {
        await _api.AddUserAsync("hanako", password: null, authType: UserAuthType.Ad);
        _api.Ad.Passwords["hanako"] = "ad-secret";

        Assert.Equal(HttpStatusCode.OK, (await _api.LoginAsync("hanako", "ad-secret")).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await _api.LoginAsync("hanako", "wrong")).StatusCode);
    }

    [Fact]
    public async Task Login_AdAccountNotRegistered_Returns401EvenIfAdAcceptsIt()
    {
        _api.Ad.Passwords["jiro"] = "ad-secret";

        var response = await _api.LoginAsync("jiro", "ad-secret");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_AdUnavailable_Returns503()
    {
        await _api.AddUserAsync("hanako", password: null, authType: UserAuthType.Ad);
        _api.Ad.Unavailable = true;

        var response = await _api.LoginAsync("hanako", "ad-secret");

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    [Fact]
    public async Task Login_LocksOutAfterRepeatedFailures_AndUnlocksAfterDuration()
    {
        await _api.AddUserAsync("taro");
        for (var i = 0; i < 5; i++)
        {
            await _api.LoginAsync("taro", "wrong-password");
        }

        // Locked: even the right password is rejected.
        Assert.Equal(HttpStatusCode.Unauthorized, (await _api.LoginAsync("taro", "password123")).StatusCode);

        _api.Clock.Advance(TimeSpan.FromMinutes(16));
        Assert.Equal(HttpStatusCode.OK, (await _api.LoginAsync("taro", "password123")).StatusCode);
    }

    [Fact]
    public async Task Login_InactiveUser_Returns401()
    {
        var adminId = await _api.AddUserAsync("admin1", isAdmin: true);
        var userId = await _api.AddUserAsync("taro");
        var admin = _api.CreateClient((await _api.LoginOkAsync("admin1")).AccessToken);
        await admin.PutAsJsonAsync($"/api/users/{userId}", new UpdateUserRequest("taro さん", null, IsAdmin: false, IsActive: false));

        var response = await _api.LoginAsync("taro", "password123");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Me_RequiresAccessToken()
    {
        await _api.AddUserAsync("taro");

        Assert.Equal(HttpStatusCode.Unauthorized, (await _api.CreateClient().GetAsync("/api/auth/me")).StatusCode);

        var tokens = await _api.LoginOkAsync("taro");
        var me = await _api.CreateClient(tokens.AccessToken).GetFromJsonAsync<UserDto>("/api/auth/me");
        Assert.Equal("taro", me!.UserName);
    }

    [Fact]
    public async Task AccessToken_ExpiresAfter15Minutes()
    {
        await _api.AddUserAsync("taro");
        var client = _api.CreateClient((await _api.LoginOkAsync("taro")).AccessToken);

        _api.Clock.Advance(TimeSpan.FromMinutes(16));

        Assert.Equal(HttpStatusCode.Unauthorized, (await client.GetAsync("/api/auth/me")).StatusCode);
    }

    [Fact]
    public async Task Refresh_RotatesToken_AndOldTokenCannotBeReused()
    {
        await _api.AddUserAsync("taro");
        var first = await _api.LoginOkAsync("taro");
        var client = _api.CreateClient();

        var refreshed = await client.PostAsJsonAsync("/api/auth/refresh", new RefreshRequest(first.RefreshToken));
        Assert.Equal(HttpStatusCode.OK, refreshed.StatusCode);
        var second = (await refreshed.Content.ReadFromJsonAsync<TokenResponse>())!;
        Assert.NotEqual(first.RefreshToken, second.RefreshToken);

        // Replaying the old token is treated as theft: it fails and also kills the new session.
        var replay = await client.PostAsJsonAsync("/api/auth/refresh", new RefreshRequest(first.RefreshToken));
        Assert.Equal(HttpStatusCode.Unauthorized, replay.StatusCode);
        var afterReplay = await client.PostAsJsonAsync("/api/auth/refresh", new RefreshRequest(second.RefreshToken));
        Assert.Equal(HttpStatusCode.Unauthorized, afterReplay.StatusCode);
    }

    [Fact]
    public async Task Refresh_FailsAfterRefreshTokenExpires()
    {
        await _api.AddUserAsync("taro");
        var tokens = await _api.LoginOkAsync("taro");

        _api.Clock.Advance(TimeSpan.FromDays(15));

        var response = await _api.CreateClient().PostAsJsonAsync("/api/auth/refresh", new RefreshRequest(tokens.RefreshToken));
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Logout_RevokesRefreshToken()
    {
        await _api.AddUserAsync("taro");
        var tokens = await _api.LoginOkAsync("taro");
        var client = _api.CreateClient();

        Assert.Equal(HttpStatusCode.NoContent,
            (await client.PostAsJsonAsync("/api/auth/logout", new LogoutRequest(tokens.RefreshToken))).StatusCode);

        var response = await client.PostAsJsonAsync("/api/auth/refresh", new RefreshRequest(tokens.RefreshToken));
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ChangeOwnPassword_RequiresCurrentPassword_AndSignsOutOtherSessions()
    {
        await _api.AddUserAsync("taro");
        var otherSession = await _api.LoginOkAsync("taro");
        var current = await _api.LoginOkAsync("taro");
        var client = _api.CreateClient(current.AccessToken);

        var wrong = await client.PutAsJsonAsync("/api/auth/me/password", new ChangePasswordRequest("nope", "new-password-1"));
        Assert.Equal(HttpStatusCode.BadRequest, wrong.StatusCode);

        var ok = await client.PutAsJsonAsync("/api/auth/me/password", new ChangePasswordRequest("password123", "new-password-1"));
        Assert.Equal(HttpStatusCode.OK, ok.StatusCode);

        Assert.Equal(HttpStatusCode.Unauthorized,
            (await _api.CreateClient().PostAsJsonAsync("/api/auth/refresh", new RefreshRequest(otherSession.RefreshToken))).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await _api.LoginAsync("taro", "new-password-1")).StatusCode);
    }
}
