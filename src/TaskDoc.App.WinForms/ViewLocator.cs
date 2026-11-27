using TaskDoc.Client.ViewModels;
using TaskDoc.WinFormsApp.Views;

namespace TaskDoc.WinFormsApp;

/// <summary>
/// Maps each page / dialog view model to its view (App.xaml's DataTemplates in the WPF app).
/// Every page / dialog view model from TaskDoc.Client needs a line here; ViewMappingTests checks this file.
/// </summary>
public static class ViewLocator
{
    public static ViewBase Create(ViewModelBase viewModel) => viewModel switch
    {
        LoginViewModel vm => new LoginView(vm),
        HomeViewModel vm => new HomeView(vm),
        DashboardViewModel vm => new DashboardView(vm),
        UsersViewModel vm => new UsersView(vm),
        UserEditorViewModel vm => new UserEditorView(vm),
        ChangePasswordViewModel vm => new ChangePasswordView(vm),
        _ => throw new InvalidOperationException($"No view registered for {viewModel.GetType().Name}"),
    };
}
