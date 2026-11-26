using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaskDoc.Client.Services;

namespace TaskDoc.Client.ViewModels;

/// <summary>Sign-in for both AD users and users registered in the system (the API tells them apart).</summary>
public partial class LoginViewModel(AuthSession session) : ViewModelBase
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private string _userName = "";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private string _password = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    private string? _errorMessage;

    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    public event EventHandler? SignedIn;

    private bool CanLogin() => !string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrEmpty(Password);

    [RelayCommand(CanExecute = nameof(CanLogin))]
    private async Task LoginAsync(CancellationToken ct)
    {
        ErrorMessage = null;
        var error = await session.SignInAsync(UserName.Trim(), Password, ct);
        if (error is not null)
        {
            ErrorMessage = error;
            Password = "";
            return;
        }

        Password = "";
        SignedIn?.Invoke(this, EventArgs.Empty);
    }
}
