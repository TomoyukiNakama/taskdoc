using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaskDoc.App.Services;

namespace TaskDoc.App.ViewModels;

/// <summary>Sign-in for both AD users and users registered in the system (the API tells them apart).</summary>
public partial class LoginViewModel(AuthSession session) : ViewModelBase
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    public partial string UserName { get; set; } = "";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    public partial string Password { get; set; } = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    public partial string? ErrorMessage { get; set; }

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
