using System.Net;
using System.Text.Json;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using TaskDoc.Client.ViewModels;
using TaskDoc.Contracts.Users;

namespace TaskDoc.App.Tests;

/// <summary>User management screen, signed in as an admin, against the stub API.</summary>
public sealed class UsersViewTests : IDisposable
{
    private static readonly DateTimeOffset T0 = TestClient.Start;

    private readonly TestClient _client = new();
    private readonly List<UserDto> _users =
    [
        new(1, "taro", "山田 太郎", null, UserAuthType.Local, IsAdmin: true, IsActive: true, T0, T0),
        new(2, "hanako", "佐藤 花子", "hanako@example.com", UserAuthType.Ad, IsAdmin: false, IsActive: true, T0, null),
        new(3, "jiro", "鈴木 次郎", null, UserAuthType.Local, IsAdmin: false, IsActive: false, T0, null),
    ];

    public UsersViewTests()
    {
        _client.Api.On("GET /api/users", _ => StubApi.Json(HttpStatusCode.OK, _users.ToArray()));
    }

    public void Dispose() => _client.Dispose();

    private async Task<(Window Window, UsersViewModel Users)> OpenUsersAsync()
    {
        var (window, home) = await Ui.SignInAsync(_client, isAdmin: true);
        await Ui.ClickAsync(window, "UsersNav");
        var users = Assert.IsType<UsersViewModel>(home.CurrentSection);
        await Ui.WaitUntilAsync(() => users.Users.Count > 0);
        return (window, users);
    }

    [AvaloniaFact]
    public async Task ShowsAllUsersWithLabels()
    {
        var (window, users) = await OpenUsersAsync();

        Assert.Equal(["taro", "hanako", "jiro"], users.Users.Select(u => u.UserName));
        Assert.Equal(3, Ui.Find<ListBox>(window, "UserList").ItemCount);
        var hanako = users.Users[1];
        Assert.Equal("AD", hanako.AuthTypeLabel);
        Assert.Equal("—", hanako.LastLoginLabel);
        Assert.Equal("無効", users.Users[2].StatusLabel);
    }

    [AvaloniaFact]
    public async Task EditIsDisabledUntilAUserIsSelected()
    {
        var (window, users) = await OpenUsersAsync();
        Assert.False(Ui.Find<Button>(window, "EditButton").IsEffectivelyEnabled);

        users.SelectedUser = users.Users[1];

        Assert.True(Ui.Find<Button>(window, "EditButton").IsEffectivelyEnabled);
    }

    [AvaloniaFact]
    public async Task RegisterLocalUser_SendsRequestAndRefreshesList()
    {
        var created = new UserDto(4, "saburo", "田中 三郎", null, UserAuthType.Local, false, true, T0, null);
        _client.Api.On("POST /api/users", _ =>
        {
            _users.Add(created);
            return StubApi.Json(HttpStatusCode.Created, created);
        });
        var (window, users) = await OpenUsersAsync();

        await Ui.ClickAsync(window, "NewUserButton");
        Ui.Type(window, "UserNameBox", "saburo");
        Ui.Type(window, "DisplayNameBox", "田中 三郎");
        Ui.Type(window, "PasswordBox", "password123");
        await Ui.ClickAsync(window, "SaveButton");

        var body = JsonDocument.Parse(_client.Api.Bodies.Single(b => b.Route == "POST /api/users").Body).RootElement;
        Assert.Equal("saburo", body.GetProperty("userName").GetString());
        Assert.Equal("Local", body.GetProperty("authType").GetString());
        Assert.Equal("password123", body.GetProperty("password").GetString());

        // No extra waiting: when the save command completes, the list must already be reloaded.
        Assert.Equal(4, users.Users.Count);
        Assert.False(users.IsEditorOpen);
        Assert.Equal("田中 三郎 さんを登録しました。", _client.Get<Client.Services.Notifier>().LastMessage);
        Assert.Equal(4, users.SelectedUser?.Id);
    }

    [AvaloniaFact]
    public async Task RegisterAdUser_HidesPasswordAndSendsNone()
    {
        var created = new UserDto(5, "shiro", "高橋 四郎", null, UserAuthType.Ad, false, true, T0, null);
        _client.Api.OnJson("POST /api/users", created);
        var (window, _) = await OpenUsersAsync();

        await Ui.ClickAsync(window, "NewUserButton");
        Ui.Find<RadioButton>(window, "AdRadio").IsChecked = true;
        Assert.False(Ui.Find<TextBox>(window, "PasswordBox").IsEffectivelyVisible);
        Ui.Type(window, "UserNameBox", "shiro");
        Ui.Type(window, "DisplayNameBox", "高橋 四郎");
        await Ui.ClickAsync(window, "SaveButton");

        var body = JsonDocument.Parse(_client.Api.Bodies.Single(b => b.Route == "POST /api/users").Body).RootElement;
        Assert.Equal("Ad", body.GetProperty("authType").GetString());
        Assert.Equal(JsonValueKind.Null, body.GetProperty("password").ValueKind);
    }

    [AvaloniaFact]
    public async Task RegisterRejectedByApi_ShowsValidationMessagesAndKeepsPanelOpen()
    {
        _client.Api.On("POST /api/users", _ => StubApi.Json(HttpStatusCode.BadRequest, new
        {
            title = "One or more validation errors occurred.",
            status = 400,
            errors = new Dictionary<string, string[]>
            {
                ["UserName"] = ["ユーザー名は半角英数字と . _ - で 1〜64 文字にしてください。"],
                ["Password"] = ["パスワードは8文字以上にしてください。"],
            },
        }));
        var (window, users) = await OpenUsersAsync();

        await Ui.ClickAsync(window, "NewUserButton");
        Ui.Type(window, "UserNameBox", "bad name");
        Ui.Type(window, "DisplayNameBox", "誰か");
        Ui.Type(window, "PasswordBox", "short");
        await Ui.ClickAsync(window, "SaveButton");

        Assert.True(users.IsEditorOpen);
        var error = Ui.Find<TextBlock>(window, "ErrorText");
        Assert.Contains("ユーザー名は半角英数字", error.Text);
        Assert.Contains("パスワードは8文字以上", error.Text);
    }

    [AvaloniaFact]
    public async Task EditUser_SendsUpdateWithStatus()
    {
        _client.Api.On("PUT /api/users/3", _ =>
        {
            _users[2] = _users[2] with { IsActive = true };
            return StubApi.Json(HttpStatusCode.OK, _users[2]);
        });
        var (window, users) = await OpenUsersAsync();
        users.SelectedUser = users.Users[2];

        await Ui.ClickAsync(window, "EditButton");
        Assert.True(Ui.Find<TextBox>(window, "UserNameBox").IsReadOnly);
        Ui.Find<CheckBox>(window, "IsActiveCheck").IsChecked = true;
        await Ui.ClickAsync(window, "SaveButton");

        var body = JsonDocument.Parse(_client.Api.Bodies.Single(b => b.Route == "PUT /api/users/3").Body).RootElement;
        Assert.True(body.GetProperty("isActive").GetBoolean());
        Assert.Equal("鈴木 次郎 さんの情報を更新しました。", _client.Get<Client.Services.Notifier>().LastMessage);
        Assert.Equal("有効", users.Users.Single(u => u.Id == 3).StatusLabel);
        Assert.Equal(3, users.SelectedUser?.Id);
    }

    [AvaloniaFact]
    public async Task EditingSelf_CannotChangeOwnAdminOrStatus()
    {
        var (window, users) = await OpenUsersAsync();
        users.SelectedUser = users.Users[0]; // id 1 = the signed-in user

        await Ui.ClickAsync(window, "EditButton");

        Assert.False(Ui.Find<CheckBox>(window, "IsAdminCheck").IsEffectivelyEnabled);
        Assert.False(Ui.Find<CheckBox>(window, "IsActiveCheck").IsEffectivelyEnabled);
    }

    [AvaloniaFact]
    public async Task PasswordReset_OnlyForSystemUsers()
    {
        _client.Api.On("PUT /api/users/3/password", _ => new HttpResponseMessage(HttpStatusCode.NoContent));
        var (window, users) = await OpenUsersAsync();

        users.SelectedUser = users.Users[1]; // AD user
        await Ui.ClickAsync(window, "EditButton");
        Assert.False(Ui.Find<Button>(window, "ResetPasswordButton").IsEffectivelyVisible);
        users.Editor!.CancelCommand.Execute(null);

        users.SelectedUser = users.Users[2]; // system user
        await Ui.ClickAsync(window, "EditButton");
        Ui.Type(window, "NewPasswordBox", "reset-pass-1");
        await Ui.ClickAsync(window, "ResetPasswordButton");

        Assert.Equal(1, _client.Api.Count("PUT /api/users/3/password"));
        Assert.True(users.Editor!.HasResetMessage);
    }

    [AvaloniaFact]
    public async Task Cancel_ClosesPanelWithoutSaving()
    {
        var (window, users) = await OpenUsersAsync();

        await Ui.ClickAsync(window, "NewUserButton");
        Assert.True(users.IsEditorOpen);
        users.Editor!.CancelCommand.Execute(null);

        Assert.False(users.IsEditorOpen);
        Assert.Equal(0, _client.Api.Count("POST /api/users"));
    }
}
