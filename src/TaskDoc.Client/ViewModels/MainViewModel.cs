using CommunityToolkit.Mvvm.ComponentModel;
using TaskDoc.Client.Services;

namespace TaskDoc.Client.ViewModels;

/// <summary>
/// App shell: shows the login page until the user signs in, then the home page.
/// Returns to the login page on sign-out or when the session expires.
/// </summary>
public partial class MainViewModel : ViewModelBase
{
    private readonly Func<LoginViewModel> _createLogin;
    private readonly Func<HomeViewModel> _createHome;

    [ObservableProperty]
    public partial ViewModelBase CurrentPage { get; set; }

    public MainViewModel(
        AuthSession session,
        IUiDispatcher ui,
        Func<LoginViewModel> createLogin,
        Func<HomeViewModel> createHome)
    {
        _createLogin = createLogin;
        _createHome = createHome;
        CurrentPage = CreateLogin(message: null);

        // Raised from whichever thread performed the failed refresh.
        session.SessionExpired += (_, _) => ui.Post(() =>
            CurrentPage = CreateLogin("セッションの有効期限が切れました。再度ログインしてください。"));
    }

    private LoginViewModel CreateLogin(string? message)
    {
        var login = _createLogin();
        login.ErrorMessage = message;
        login.SignedIn += (_, _) => ShowHome();
        return login;
    }

    private void ShowHome()
    {
        var home = _createHome();
        home.SignedOut += (_, _) => CurrentPage = CreateLogin(message: null);
        CurrentPage = home;
    }
}
