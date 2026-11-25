using Avalonia.Controls;
using Avalonia.Controls.Templates;
using TaskDoc.Client.ViewModels;
using TaskDoc.App.Views;

namespace TaskDoc.App;

/// <summary>
/// Maps each page view model to its view. Explicit (no reflection) so it keeps working when the
/// Browser/WASM and mobile builds trim unused code. Add a line here for every new page.
/// </summary>
public class ViewLocator : IDataTemplate
{
    public Control? Build(object? param) => param switch
    {
        LoginViewModel => new LoginView(),
        HomeViewModel => new HomeView(),
        DashboardViewModel => new DashboardView(),
        UsersViewModel => new UsersView(),
        UserEditorViewModel => new UserEditorView(),
        ChangePasswordViewModel => new ChangePasswordView(),
        null => null,
        _ => new TextBlock { Text = $"No view registered for {param.GetType().Name}" },
    };

    public bool Match(object? data) => data is ViewModelBase;
}
