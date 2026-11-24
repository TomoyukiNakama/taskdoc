using System.Net;
using System.Net.Http.Json;
using TaskDoc.Contracts.Users;

namespace TaskDoc.Api.Tests;

public sealed class UserEndpointTests : IAsyncLifetime
{
    private readonly TaskDocApiFactory _api = new();
    private HttpClient _admin = null!;
    private long _adminId;

    public async ValueTask InitializeAsync()
    {
        _adminId = await _api.AddUserAsync("admin1", isAdmin: true);
        _admin = _api.CreateClient((await _api.LoginOkAsync("admin1")).AccessToken);
    }

    public ValueTask DisposeAsync() => _api.DisposeAsync();

    [Fact]
    public async Task NonAdmin_IsForbidden()
    {
        await _api.AddUserAsync("taro");
        var user = _api.CreateClient((await _api.LoginOkAsync("taro")).AccessToken);

        Assert.Equal(HttpStatusCode.Forbidden, (await user.GetAsync("/api/users")).StatusCode);
    }

    [Fact]
    public async Task Create_LocalUser_CanThenSignIn()
    {
        var response = await _admin.PostAsJsonAsync("/api/users",
            new CreateUserRequest("taro", "山田 太郎", "taro@example.com", UserAuthType.Local, "password123", IsAdmin: false));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = (await response.Content.ReadFromJsonAsync<UserDto>())!;
        Assert.Equal("山田 太郎", created.DisplayName);
        Assert.True(created.IsActive);
        Assert.Equal(HttpStatusCode.OK, (await _api.LoginAsync("taro", "password123")).StatusCode);
    }

    [Fact]
    public async Task Create_AdUser_WithoutPassword()
    {
        var response = await _admin.PostAsJsonAsync("/api/users",
            new CreateUserRequest("hanako", "佐藤 花子", null, UserAuthType.Ad, Password: null, IsAdmin: false));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(UserAuthType.Ad, (await response.Content.ReadFromJsonAsync<UserDto>())!.AuthType);
    }

    [Fact]
    public async Task Create_RejectsInvalidInput()
    {
        var shortPassword = await _admin.PostAsJsonAsync("/api/users",
            new CreateUserRequest("taro", "太郎", null, UserAuthType.Local, "short", false));
        var badName = await _admin.PostAsJsonAsync("/api/users",
            new CreateUserRequest("taro yamada", "太郎", null, UserAuthType.Local, "password123", false));
        var adWithPassword = await _admin.PostAsJsonAsync("/api/users",
            new CreateUserRequest("hanako", "花子", null, UserAuthType.Ad, "password123", false));

        Assert.Equal(HttpStatusCode.BadRequest, shortPassword.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, badName.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, adWithPassword.StatusCode);
    }

    [Fact]
    public async Task Create_DuplicateUserName_Returns409_CaseInsensitive()
    {
        await _api.AddUserAsync("taro");

        var response = await _admin.PostAsJsonAsync("/api/users",
            new CreateUserRequest("TARO", "太郎", null, UserAuthType.Local, "password123", false));

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Update_CannotRemoveLastActiveAdmin()
    {
        // Startup seeding created the initial "admin"; deactivating it leaves admin1 as the only active admin.
        var seeded = (await _admin.GetFromJsonAsync<UserDto[]>("/api/users"))!.Single(u => u.UserName == "admin");
        var deactivateSeeded = await _admin.PutAsJsonAsync($"/api/users/{seeded.Id}",
            new UpdateUserRequest(seeded.DisplayName, null, IsAdmin: true, IsActive: false));
        Assert.Equal(HttpStatusCode.OK, deactivateSeeded.StatusCode);

        var response = await _admin.PutAsJsonAsync($"/api/users/{_adminId}",
            new UpdateUserRequest("管理者", null, IsAdmin: false, IsActive: true));

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Deactivate_RevokesRefreshTokens()
    {
        var userId = await _api.AddUserAsync("taro");
        var tokens = await _api.LoginOkAsync("taro");

        await _admin.PutAsJsonAsync($"/api/users/{userId}", new UpdateUserRequest("太郎", null, false, IsActive: false));

        var refresh = await _api.CreateClient().PostAsJsonAsync("/api/auth/refresh",
            new Contracts.Auth.RefreshRequest(tokens.RefreshToken));
        Assert.Equal(HttpStatusCode.Unauthorized, refresh.StatusCode);
    }

    [Fact]
    public async Task SetPassword_ResetsAndUnlocksLocalUser()
    {
        var userId = await _api.AddUserAsync("taro");
        for (var i = 0; i < 5; i++)
        {
            await _api.LoginAsync("taro", "wrong-password");
        }

        var response = await _admin.PutAsJsonAsync($"/api/users/{userId}/password", new SetPasswordRequest("reset-pass-1"));

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await _api.LoginAsync("taro", "reset-pass-1")).StatusCode);
    }

    [Fact]
    public async Task SetPassword_RejectedForAdUser()
    {
        var userId = await _api.AddUserAsync("hanako", password: null, authType: UserAuthType.Ad);

        var response = await _admin.PutAsJsonAsync($"/api/users/{userId}/password", new SetPasswordRequest("reset-pass-1"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
