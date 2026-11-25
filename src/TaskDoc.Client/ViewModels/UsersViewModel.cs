using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaskDoc.Client.Services;
using TaskDoc.Contracts.Users;

namespace TaskDoc.Client.ViewModels;

/// <summary>User management (system administrators): list, register, edit, reset passwords.</summary>
public partial class UsersViewModel(ITaskDocApi api, AuthSession session, IDialogService dialogs, Notifier notifier)
    : ViewModelBase
{
    public ObservableCollection<UserRow> Users { get; } = [];

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(EditCommand))]
    public partial UserRow? SelectedUser { get; set; }

    /// <summary>The register/edit dialog's view model while the dialog is open, otherwise null.</summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsEditorOpen))]
    public partial UserEditorViewModel? Editor { get; set; }

    public bool IsEditorOpen => Editor is not null;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    public partial string? ErrorMessage { get; set; }

    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    [RelayCommand]
    private async Task LoadAsync(CancellationToken ct) => await ReloadAsync(selectId: SelectedUser?.Id, ct);

    [RelayCommand]
    private void NewUser() => OpenEditor(existing: null);

    private bool CanEdit() => SelectedUser is not null;

    [RelayCommand(CanExecute = nameof(CanEdit))]
    private void Edit() => OpenEditor(SelectedUser!.User);

    private void OpenEditor(UserDto? existing)
    {
        UserEditorViewModel? editor = null;
        editor = new UserEditorViewModel(
            api,
            existing,
            currentUserId: session.CurrentUser?.Id,
            onSaved: async saved =>
            {
                dialogs.Close(editor!);
                await ReloadAsync(saved.Id, CancellationToken.None);
                notifier.Success(existing is null
                    ? $"{saved.DisplayName} さんを登録しました。"
                    : $"{saved.DisplayName} さんの情報を更新しました。");
            },
            onClosed: () => dialogs.Close(editor!));

        Editor = editor;
        dialogs.Show(editor, onClosed: () => Editor = null);
    }

    private async Task ReloadAsync(long? selectId, CancellationToken ct)
    {
        ErrorMessage = null;
        try
        {
            var users = await api.GetUsersAsync(ct);
            Users.Clear();
            foreach (var user in users)
            {
                Users.Add(new UserRow(user));
            }

            SelectedUser = Users.FirstOrDefault(u => u.Id == selectId);
        }
        catch (Exception ex) when (ApiErrors.ToMessage(ex) is { } message)
        {
            ErrorMessage = message;
        }
    }
}
