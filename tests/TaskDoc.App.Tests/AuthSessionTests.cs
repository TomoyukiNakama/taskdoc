using System.Net;
using TaskDoc.App.Services;
using TaskDoc.Contracts.Health;
using TaskDoc.Contracts.Users;

namespace TaskDoc.App.Tests;

public sealed class AuthSessionTests : IDisposable
{
    private readonly TestClient _client = new();

    public void Dispose() => _client.Dispose();

    private AuthSession Session => _client.Get<AuthSession>();

    private ITaskDocApi Api => _client.Get<ITaskDocApi>();

    private async Task SignInAsync()
    {
        _client.Api.OnJson("POST /api/auth/login", TestClient.Tokens("1", TestClient.Start));
        Assert.Null(await Session.SignInAsync("taro", "password123", TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task ApiRequests_CarryAccessToken()
    {
        await SignInAsync();
        _client.Api.OnJson("GET /api/auth/me", TestClient.Tokens("1", TestClient.Start).User);

        await Api.GetMeAsync(TestContext.Current.CancellationToken);

        Assert.Contains(("GET /api/auth/me", "Bearer access-1"), _client.Api.Requests);
    }

    [Fact]
    public async Task SignIn_Unauthorized_ReturnsServerMessage()
    {
        _client.Api.On("POST /api/auth/login",
            _ => StubApi.Problem(HttpStatusCode.Unauthorized, "ユーザー名またはパスワードが正しくありません。"));

        var error = await Session.SignInAsync("taro", "wrong", TestContext.Current.CancellationToken);

        Assert.Equal("ユーザー名またはパスワードが正しくありません。", error);
        Assert.False(Session.IsSignedIn);
    }

    [Fact]
    public async Task ExpiringAccessToken_IsRefreshedBeforeTheRequest()
    {
        await SignInAsync();
        _client.Clock.Advance(TimeSpan.FromMinutes(14.5)); // inside the 1-minute refresh margin
        _client.Api.OnJson("POST /api/auth/refresh", TestClient.Tokens("2", _client.Clock.GetUtcNow()));
        _client.Api.OnJson("GET /api/health", new HealthResponse("ok", "3", TestClient.Start));

        await Api.GetHealthAsync(TestContext.Current.CancellationToken);

        Assert.Equal(1, _client.Api.Count("POST /api/auth/refresh"));
        Assert.Contains(("GET /api/health", "Bearer access-2"), _client.Api.Requests);
    }

    [Fact]
    public async Task RejectedAccessToken_IsRenewedAndRequestRetriedOnce()
    {
        await SignInAsync();
        _client.Api.OnJson("POST /api/auth/refresh", TestClient.Tokens("2", TestClient.Start));
        _client.Api.On("GET /api/auth/me", request =>
            request.Headers.Authorization?.Parameter == "access-2"
                ? StubApi.Json(HttpStatusCode.OK, TestClient.Tokens("2", TestClient.Start).User)
                : new HttpResponseMessage(HttpStatusCode.Unauthorized));

        var me = await Api.GetMeAsync(TestContext.Current.CancellationToken);

        Assert.Equal("taro", me.UserName);
        Assert.Equal(2, _client.Api.Count("GET /api/auth/me"));
    }

    [Fact]
    public async Task ConcurrentRequests_ShareOneRefresh()
    {
        await SignInAsync();
        _client.Clock.Advance(TimeSpan.FromMinutes(20));
        _client.Api.OnJson("POST /api/auth/refresh", TestClient.Tokens("2", _client.Clock.GetUtcNow()));
        _client.Api.OnJson("GET /api/health", new HealthResponse("ok", "3", TestClient.Start));

        await Task.WhenAll(Enumerable.Range(0, 5).Select(_ => Api.GetHealthAsync(TestContext.Current.CancellationToken)));

        // A second refresh with the same token would make the server revoke the whole session.
        Assert.Equal(1, _client.Api.Count("POST /api/auth/refresh"));
    }

    [Fact]
    public async Task RefreshRejected_EndsSessionAndRaisesSessionExpired()
    {
        await SignInAsync();
        _client.Clock.Advance(TimeSpan.FromMinutes(20));
        _client.Api.On("POST /api/auth/refresh", _ => StubApi.Problem(HttpStatusCode.Unauthorized, "expired"));
        _client.Api.OnJson("GET /api/health", new HealthResponse("ok", "3", TestClient.Start));
        var expired = false;
        Session.SessionExpired += (_, _) => expired = true;

        await Api.GetHealthAsync(TestContext.Current.CancellationToken);

        Assert.True(expired);
        Assert.False(Session.IsSignedIn);
    }

    [Fact]
    public async Task SignOut_RevokesRefreshTokenOnServer()
    {
        await SignInAsync();
        _client.Api.On("POST /api/auth/logout", _ => new HttpResponseMessage(HttpStatusCode.NoContent));

        await Session.SignOutAsync();

        Assert.Equal(1, _client.Api.Count("POST /api/auth/logout"));
        Assert.False(Session.IsSignedIn);
        Assert.Null(await Session.GetAccessTokenAsync(TestContext.Current.CancellationToken));
    }
}
