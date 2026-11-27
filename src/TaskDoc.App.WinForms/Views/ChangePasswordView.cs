using TaskDoc.Client.ViewModels;

namespace TaskDoc.WinFormsApp.Views;

public partial class ChangePasswordView : ViewBase
{
    public ChangePasswordView(ChangePasswordViewModel vm)
    {
        InitializeComponent();
        Ui.FadeWhenDisabled(ChangeButton);

        Bind.Text(CurrentPasswordBox, vm, nameof(vm.CurrentPassword), () => vm.CurrentPassword, v => vm.CurrentPassword = v);
        Bind.Text(NewPasswordBox, vm, nameof(vm.NewPassword), () => vm.NewPassword, v => vm.NewPassword = v);
        Bind.Text(ConfirmPasswordBox, vm, nameof(vm.ConfirmPassword), () => vm.ConfirmPassword, v => vm.ConfirmPassword = v);
        Bind.Label(ErrorBar, vm, nameof(vm.ErrorMessage), () => vm.ErrorMessage);
        Bind.Visible(ErrorBar, vm, nameof(vm.HasError), () => vm.HasError);
        Bind.Command(ChangeButton, vm.ChangeCommand, busyText: "変更中…");
        Bind.EnterExecutes(vm.ChangeCommand, CurrentPasswordBox, NewPasswordBox, ConfirmPasswordBox);
    }

    public override Control InitialFocus => CurrentPasswordBox;
}
