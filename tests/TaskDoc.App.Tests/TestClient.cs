using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using TaskDoc.App.Services;
using TaskDoc.Contracts.Auth;
using TaskDoc.Contracts.Users;

namespace TaskDoc.App.Tests;

/// <summary>
/// The real client DI container (Refit clients, AuthSession, AuthHeaderHandler) with the network
/// replaced by <see cref="StubApi"/> and the clock by a <see cref="FakeTimeProvider"/>.
/// </summary>
public sealed class TestClient : IDisposable
{
    public static readonly DateTimeOffset Start = new(2026, 4, 1, 9, 0, 0, TimeSpan.Zero);

    private readonly ServiceProvider _services;

    public TestClient()
    {
        var services = new ServiceCollection().AddTaskDocClient(new ClientOptions { HttpMessageHandlerFactory = () => Api });
        services.AddSingleton<TimeProvider>(Clock);
        _services = services.BuildServiceProvider();
    }

    public FakeTimeProvider Clock { get; } = new(Start);

    public StubApi Api { get; } = new();

    public T Get<T>() where T : notnull => _services.GetRequiredService<T>();

    public static TokenResponse Tokens(
        string suffix, DateTimeOffset issuedAt, bool isAdmin = false, UserAuthType authType = UserAuthType.Local) => new(
        AccessToken: $"access-{suffix}",
        AccessTokenExpiresAt: issuedAt.AddMinutes(15),
        RefreshToken: $"refresh-{suffix}",
        RefreshTokenExpiresAt: issuedAt.AddDays(14),
        User: new UserDto(1, "taro", "山田 太郎", null, authType, isAdmin, IsActive: true, issuedAt, issuedAt));

    public void Dispose() => _services.Dispose();
}

/// <summary>Answers requests by "METHOD /path" and records what was sent.</summary>
public sealed class StubApi : HttpMessageHandler
{
    private readonly Dictionary<string, Func<HttpRequestMessage, HttpResponseMessage>> _routes = new();

    public List<(string Route, string? Authorization)> Requests { get; } = [];

    /// <summary>JSON bodies of requests that had one, in order.</summary>
    public List<(string Route, string Body)> Bodies { get; } = [];

    public void On(string route, Func<HttpRequestMessage, HttpResponseMessage> respond) => _routes[route] = respond;

    public void OnJson<T>(string route, T body) => On(route, _ => Json(HttpStatusCode.OK, body));

    public static HttpResponseMessage Json<T>(HttpStatusCode status, T body) =>
        new(status) { Content = JsonContent.Create(body) };

    public static HttpResponseMessage Problem(HttpStatusCode status, string detail) =>
        new(status) { Content = JsonContent.Create(new { title = status.ToString(), status = (int)status, detail }) };

    public int Count(string route) => Requests.Count(r => r.Route == route);

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var route = $"{request.Method} {request.RequestUri!.AbsolutePath}";
        Requests.Add((route, request.Headers.Authorization?.ToString()));
        if (request.Content is not null)
        {
            Bodies.Add((route, await request.Content.ReadAsStringAsync(cancellationToken)));
        }
        var response = _routes.TryGetValue(route, out var respond)
            ? respond(request)
            : new HttpResponseMessage(HttpStatusCode.NotFound);
        response.RequestMessage = request; // a real HttpClientHandler always sets this; Refit relies on it
        return response;
    }
}
