using TaskDoc.Client.ViewModels;

namespace TaskDoc.WinFormsApp.Views;

public partial class LoginView : ViewBase
{
    public LoginView(LoginViewModel vm)
    {
        InitializeComponent();
        Ui.FadeWhenDisabled(LoginButton);

        Bind.Text(UserNameBox, vm, nameof(vm.UserName), () => vm.UserName, v => vm.UserName = v);
        Bind.Text(PasswordBox, vm, nameof(vm.Password), () => vm.Password, v => vm.Password = v);
        Bind.Label(ErrorBar, vm, nameof(vm.ErrorMessage), () => vm.ErrorMessage);
        Bind.Visible(ErrorBar, vm, nameof(vm.HasError), () => vm.HasError);
        Bind.Command(LoginButton, vm.LoginCommand, busyText: "ログイン中…");
        Bind.EnterExecutes(vm.LoginCommand, UserNameBox, PasswordBox);
    }

    public override Control InitialFocus => UserNameBox;
}
