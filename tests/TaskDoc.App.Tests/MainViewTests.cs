using System.Net;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using TaskDoc.Client.Services;
using TaskDoc.Client.ViewModels;
using TaskDoc.Contracts.Users;

namespace TaskDoc.App.Tests;

/// <summary>Login screen and the signed-in header, driven through the real views.</summary>
public sealed class MainViewTests : IDisposable
{
    private readonly TestClient _client = new();

    public void Dispose() => _client.Dispose();

    [AvaloniaFact]
    public void StartsOnLoginPage_WithLoginDisabledUntilBothFieldsAreFilled()
    {
        var (window, shell) = Ui.ShowApp(_client);

        Assert.IsType<LoginViewModel>(shell.CurrentPage);
        Assert.False(Ui.Find<Button>(window, "LoginButton").IsEffectivelyEnabled);

        Ui.Type(window, "UserNameBox", "taro");
        Assert.False(Ui.Find<Button>(window, "LoginButton").IsEffectivelyEnabled);
    }

    [AvaloniaFact]
    public async Task SuccessfulLogin_ShowsHomeWithUserName()
    {
        var (window, _) = await Ui.SignInAsync(_client);

        Assert.Equal("山田 太郎", Ui.Find<TextBlock>(window, "UserText").Text);
    }

    [AvaloniaFact]
    public async Task FailedLogin_ShowsErrorAndClearsPassword()
    {
        _client.Api.On("POST /api/auth/login",
            _ => StubApi.Problem(HttpStatusCode.Unauthorized, "ユーザー名またはパスワードが正しくありません。"));
        var (window, shell) = Ui.ShowApp(_client);
        Ui.Type(window, "UserNameBox", "taro");
        Ui.Type(window, "PasswordBox", "wrong-password");

        await Ui.ClickAsync(window, "LoginButton");

        Assert.IsType<LoginViewModel>(shell.CurrentPage);
        var error = Ui.Find<TextBlock>(window, "ErrorText");
        Assert.True(error.IsEffectivelyVisible);
        Assert.Equal("ユーザー名またはパスワードが正しくありません。", error.Text);
        Assert.Equal("", Ui.Find<TextBox>(window, "PasswordBox").Text);
    }

    [AvaloniaFact]
    public async Task ExpiredSession_ReturnsToLoginWithMessage()
    {
        _client.Api.On("POST /api/auth/refresh", _ => StubApi.Problem(HttpStatusCode.Unauthorized, "expired"));
        var (_, _) = await Ui.SignInAsync(_client);
        var shell = _client.Get<MainViewModel>();

        _client.Clock.Advance(TimeSpan.FromMinutes(20));
        await _client.Get<AuthSession>().GetAccessTokenAsync(TestContext.Current.CancellationToken);
        Dispatcher.UIThread.RunJobs();

        var login = Assert.IsType<LoginViewModel>(shell.CurrentPage);
        Assert.Equal("セッションの有効期限が切れました。再度ログインしてください。", login.ErrorMessage);
    }

    [AvaloniaTheory]
    [InlineData(false, UserAuthType.Local, false, true)]
    [InlineData(true, UserAuthType.Local, true, true)]
    [InlineData(false, UserAuthType.Ad, false, false)]
    public async Task Navigation_ShowsOnlySectionsTheUserMayUse(
        bool isAdmin, UserAuthType authType, bool expectUsers, bool expectChangePassword)
    {
        var (window, _) = await Ui.SignInAsync(_client, isAdmin, authType);

        Assert.Equal(expectUsers, Ui.Find<Button>(window, "UsersNav").IsVisible);
        Assert.Equal(expectChangePassword, Ui.Find<Button>(window, "ChangePasswordNav").IsVisible);
    }

    [AvaloniaFact]
    public async Task SignOut_ReturnsToLogin()
    {
        _client.Api.On("POST /api/auth/logout", _ => new HttpResponseMessage(HttpStatusCode.NoContent));
        var (window, _) = await Ui.SignInAsync(_client);

        await Ui.ClickAsync(window, "SignOutButton");

        Assert.IsType<LoginViewModel>(_client.Get<MainViewModel>().CurrentPage);
    }
}
