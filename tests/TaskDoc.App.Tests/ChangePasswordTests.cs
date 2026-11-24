using System.Net;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using TaskDoc.App.ViewModels;
using TaskDoc.Contracts.Users;

namespace TaskDoc.App.Tests;

public sealed class ChangePasswordTests : IDisposable
{
    private readonly TestClient _client = new();

    public void Dispose() => _client.Dispose();

    private async Task<Window> OpenAsync()
    {
        var (window, _) = await Ui.SignInAsync(_client);
        await Ui.ClickAsync(window, "ChangePasswordNav");
        Ui.Type(window, "CurrentPasswordBox", "password123");
        return window;
    }

    [AvaloniaFact]
    public async Task MismatchedConfirmation_IsRejectedWithoutCallingApi()
    {
        var window = await OpenAsync();
        Ui.Type(window, "NewPasswordBox", "new-password-1");
        Ui.Type(window, "ConfirmPasswordBox", "new-password-2");

        await Ui.ClickAsync(window, "ChangeButton");

        Assert.Equal("新しいパスワードと確認用のパスワードが一致しません。", Ui.Find<TextBlock>(window, "ErrorText").Text);
        Assert.Equal(0, _client.Api.Count("PUT /api/auth/me/password"));
    }

    [AvaloniaFact]
    public async Task Success_SwitchesToTheNewSession()
    {
        _client.Api.OnJson("PUT /api/auth/me/password", TestClient.Tokens("2", TestClient.Start));
        _client.Api.OnJson("GET /api/auth/me", TestClient.Tokens("2", TestClient.Start).User);
        var window = await OpenAsync();
        Ui.Type(window, "NewPasswordBox", "new-password-1");
        Ui.Type(window, "ConfirmPasswordBox", "new-password-1");

        await Ui.ClickAsync(window, "ChangeButton");

        Assert.Equal("パスワードを変更しました。ほかの端末ではログアウトされます。", _client.Get<Services.Notifier>().LastMessage);
        Assert.Equal("", Ui.Find<TextBox>(window, "NewPasswordBox").Text);

        // Later requests use the token from the password-change response (the old session was revoked server-side).
        await _client.Get<Services.ITaskDocApi>().GetMeAsync(TestContext.Current.CancellationToken);
        Assert.Equal("Bearer access-2", _client.Api.Requests.Last(r => r.Route == "GET /api/auth/me").Authorization);
    }

    [AvaloniaFact]
    public async Task WrongCurrentPassword_ShowsApiMessage()
    {
        _client.Api.On("PUT /api/auth/me/password", _ => StubApi.Json(HttpStatusCode.BadRequest, new
        {
            status = 400,
            errors = new Dictionary<string, string[]> { ["CurrentPassword"] = ["現在のパスワードが正しくありません。"] },
        }));
        var window = await OpenAsync();
        Ui.Type(window, "NewPasswordBox", "new-password-1");
        Ui.Type(window, "ConfirmPasswordBox", "new-password-1");

        await Ui.ClickAsync(window, "ChangeButton");

        Assert.Equal("現在のパスワードが正しくありません。", Ui.Find<TextBlock>(window, "ErrorText").Text);
    }
}
