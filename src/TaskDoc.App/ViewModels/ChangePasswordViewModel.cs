using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaskDoc.App.Services;
using TaskDoc.Contracts.Users;

namespace TaskDoc.App.ViewModels;

/// <summary>A system user changes their own password. Other devices are signed out; this one stays signed in.</summary>
public partial class ChangePasswordViewModel(ITaskDocApi api, AuthSession session, Notifier notifier) : ViewModelBase
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ChangeCommand))]
    public partial string CurrentPassword { get; set; } = "";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ChangeCommand))]
    public partial string NewPassword { get; set; } = "";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ChangeCommand))]
    public partial string ConfirmPassword { get; set; } = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    public partial string? ErrorMessage { get; set; }

    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    private bool CanChange() =>
        !string.IsNullOrEmpty(CurrentPassword) && !string.IsNullOrEmpty(NewPassword) && !string.IsNullOrEmpty(ConfirmPassword);

    [RelayCommand(CanExecute = nameof(CanChange))]
    private async Task ChangeAsync(CancellationToken ct)
    {
        ErrorMessage = null;

        if (NewPassword != ConfirmPassword)
        {
            ErrorMessage = "新しいパスワードと確認用のパスワードが一致しません。";
            return;
        }

        try
        {
            var tokens = await api.ChangeOwnPasswordAsync(new ChangePasswordRequest(CurrentPassword, NewPassword), ct);
            session.ReplaceSession(tokens);
            CurrentPassword = NewPassword = ConfirmPassword = "";
            notifier.Success("パスワードを変更しました。ほかの端末ではログアウトされます。");
        }
        catch (Exception ex) when (ApiErrors.ToMessage(ex) is { } message)
        {
            ErrorMessage = message;
        }
    }
}
