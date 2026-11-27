using TaskDoc.Client.ViewModels;

namespace TaskDoc.WinFormsApp.Views;

/// <summary>Register / edit a user. Shown in a modal dialog (WinFormsDialogService).</summary>
public partial class UserEditorView : ViewBase
{
    private readonly UserEditorViewModel _vm;

    public UserEditorView(UserEditorViewModel vm)
    {
        _vm = vm;
        InitializeComponent();
        Ui.FadeWhenDisabled(CancelButton, SaveButton, ResetPasswordButton);

        // Parts that depend on the user being edited (fixed while the dialog is open).
        TitleLabel.Text = vm.Title;
        fixedNote.Visible = vm.IsNew;
        authTypeRow.Enabled = vm.IsNew;
        UserNameBox.ReadOnly = !vm.IsNew;
        userNameHint.Visible = vm.IsNew;
        IsActiveCheck.Visible = !vm.IsNew;
        IsAdminCheck.Enabled = IsActiveCheck.Enabled = vm.CanChangeAdminAndStatus;
        selfHint.Visible = vm.IsEditingSelf;
        resetSection.Visible = vm.ShowPasswordReset;

        Bind.Choice(LocalRadio, AdRadio, vm, nameof(vm.IsLocal), () => vm.IsLocal, v => vm.IsLocal = v);
        Bind.Text(UserNameBox, vm, nameof(vm.UserName), () => vm.UserName, v => vm.UserName = v);
        Bind.Text(DisplayNameBox, vm, nameof(vm.DisplayName), () => vm.DisplayName, v => vm.DisplayName = v);
        Bind.Text(EmailBox, vm, nameof(vm.Email), () => vm.Email, v => vm.Email = v);
        Bind.Text(PasswordBox, vm, nameof(vm.Password), () => vm.Password, v => vm.Password = v);
        Bind.Watch(vm, () => passwordLabel.Visible = PasswordBox.Visible = passwordHint.Visible = vm.ShowInitialPassword,
            nameof(vm.ShowInitialPassword));
        Bind.Checked(IsAdminCheck, vm, nameof(vm.IsAdmin), () => vm.IsAdmin, v => vm.IsAdmin = v);
        Bind.Checked(IsActiveCheck, vm, nameof(vm.IsActive), () => vm.IsActive, v => vm.IsActive = v);
        Bind.Label(ErrorBar, vm, nameof(vm.ErrorMessage), () => vm.ErrorMessage);
        Bind.Visible(ErrorBar, vm, nameof(vm.HasError), () => vm.HasError);
        Bind.Command(CancelButton, vm.CancelCommand);
        Bind.Command(SaveButton, vm.SaveCommand, busyText: "保存中…");
        Bind.EnterExecutes(vm.SaveCommand, UserNameBox, DisplayNameBox, EmailBox, PasswordBox);

        Bind.Text(NewPasswordBox, vm, nameof(vm.NewPassword), () => vm.NewPassword, v => vm.NewPassword = v);
        Bind.Command(ResetPasswordButton, vm.ResetPasswordCommand, busyText: "再設定中…");
        Bind.EnterExecutes(vm.ResetPasswordCommand, NewPasswordBox);
        Bind.Label(ResetBar, vm, nameof(vm.ResetMessage), () => vm.ResetMessage);
        Bind.Visible(ResetBar, vm, nameof(vm.HasResetMessage), () => vm.HasResetMessage);
    }

    public override Control InitialFocus => _vm.IsNew ? UserNameBox : DisplayNameBox;
}
