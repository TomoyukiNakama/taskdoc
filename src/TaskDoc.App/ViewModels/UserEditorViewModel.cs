using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaskDoc.App.Services;
using TaskDoc.Contracts.Users;

namespace TaskDoc.App.ViewModels;

/// <summary>
/// Register a new user or edit an existing one. Final validation happens in the API; its messages are shown here.
/// </summary>
public partial class UserEditorViewModel : ViewModelBase
{
    private readonly ITaskDocApi _api;
    private readonly UserDto? _existing;
    private readonly Func<UserDto, Task> _onSaved;
    private readonly Action _onClosed;

    /// <param name="onSaved">
    /// Awaited after a successful save (the list reloads here), so the save button stays busy until the
    /// screen shows the result.
    /// </param>
    /// <param name="onClosed">Called when the panel is cancelled.</param>
    public UserEditorViewModel(
        ITaskDocApi api, UserDto? existing, long? currentUserId, Func<UserDto, Task> onSaved, Action onClosed)
    {
        _api = api;
        _existing = existing;
        _onSaved = onSaved;
        _onClosed = onClosed;
        IsEditingSelf = existing is not null && existing.Id == currentUserId;

        UserName = existing?.UserName ?? "";
        DisplayName = existing?.DisplayName ?? "";
        Email = existing?.Email ?? "";
        IsLocal = existing?.AuthType != UserAuthType.Ad;
        IsAdmin = existing?.IsAdmin ?? false;
        IsActive = existing?.IsActive ?? true;
    }

    public bool IsNew => _existing is null;

    public string Title => IsNew ? "ユーザーを登録" : "ユーザーを編集";

    /// <summary>
    /// Admins cannot remove their own admin rights or deactivate themselves here, so they cannot lock themselves out.
    /// </summary>
    public bool IsEditingSelf { get; }

    public bool CanChangeAdminAndStatus => !IsEditingSelf;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    public partial string UserName { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    public partial string DisplayName { get; set; }

    [ObservableProperty]
    public partial string Email { get; set; }

    /// <summary>System user (password in TaskDoc) when true, AD user when false. Fixed after registration.</summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsAd), nameof(ShowInitialPassword))]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    public partial bool IsLocal { get; set; }

    public bool IsAd
    {
        get => !IsLocal;
        set => IsLocal = !value;
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    public partial string Password { get; set; } = "";

    public bool ShowInitialPassword => IsNew && IsLocal;

    /// <summary>Admins can reset the password of an existing system user.</summary>
    public bool ShowPasswordReset => !IsNew && _existing!.AuthType == UserAuthType.Local;

    [ObservableProperty]
    public partial bool IsAdmin { get; set; }

    [ObservableProperty]
    public partial bool IsActive { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ResetPasswordCommand))]
    public partial string NewPassword { get; set; } = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    public partial string? ErrorMessage { get; set; }

    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasResetMessage))]
    public partial string? ResetMessage { get; set; }

    public bool HasResetMessage => !string.IsNullOrEmpty(ResetMessage);

    private bool CanSave() =>
        !string.IsNullOrWhiteSpace(UserName)
        && !string.IsNullOrWhiteSpace(DisplayName)
        && (!ShowInitialPassword || !string.IsNullOrEmpty(Password));

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task SaveAsync(CancellationToken ct)
    {
        ErrorMessage = null;
        var email = string.IsNullOrWhiteSpace(Email) ? null : Email.Trim();
        try
        {
            var saved = IsNew
                ? await _api.CreateUserAsync(new CreateUserRequest(
                    UserName.Trim(), DisplayName.Trim(), email,
                    IsLocal ? UserAuthType.Local : UserAuthType.Ad,
                    IsLocal ? Password : null,
                    IsAdmin), ct)
                : await _api.UpdateUserAsync(_existing!.Id, new UpdateUserRequest(
                    DisplayName.Trim(), email, IsAdmin, IsActive), ct);

            Password = "";
            await _onSaved(saved);
        }
        catch (Exception ex) when (ApiErrors.ToMessage(ex) is { } message)
        {
            ErrorMessage = message;
        }
    }

    private bool CanResetPassword() => ShowPasswordReset && !string.IsNullOrEmpty(NewPassword);

    [RelayCommand(CanExecute = nameof(CanResetPassword))]
    private async Task ResetPasswordAsync(CancellationToken ct)
    {
        ErrorMessage = null;
        ResetMessage = null;
        try
        {
            await _api.SetUserPasswordAsync(_existing!.Id, new SetPasswordRequest(NewPassword), ct);
            NewPassword = "";
            ResetMessage = "パスワードを再設定しました。このユーザーはすべての端末でログアウトされます。";
        }
        catch (Exception ex) when (ApiErrors.ToMessage(ex) is { } message)
        {
            ErrorMessage = message;
        }
    }

    [RelayCommand]
    private void Cancel() => _onClosed();
}
