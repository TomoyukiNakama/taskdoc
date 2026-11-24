using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaskDoc.App.Services;
using TaskDoc.Contracts.Users;

namespace TaskDoc.App.ViewModels;

/// <summary>
/// Signed-in shell: sidebar navigation and the current section.
/// Sections appear only to users who may use them (user management: admins; password change: system users).
/// </summary>
public partial class HomeViewModel : ViewModelBase
{
    public const string DashboardRoute = "dashboard";
    public const string UsersRoute = "users";
    public const string ChangePasswordRoute = "password";

    private readonly AuthSession _session;
    private readonly Func<DashboardViewModel> _createDashboard;
    private readonly Func<UsersViewModel> _createUsers;
    private readonly Func<ChangePasswordViewModel> _createChangePassword;

    public HomeViewModel(
        AuthSession session,
        Func<DashboardViewModel> createDashboard,
        Func<UsersViewModel> createUsers,
        Func<ChangePasswordViewModel> createChangePassword)
    {
        _session = session;
        _createDashboard = createDashboard;
        _createUsers = createUsers;
        _createChangePassword = createChangePassword;
        CurrentSection = createDashboard();
        CurrentRoute = DashboardRoute;
    }

    public string DisplayName => _session.CurrentUser?.DisplayName ?? "";

    public string UserName => _session.CurrentUser?.UserName ?? "";

    public string Initial => DisplayName.Length > 0 ? DisplayName[..1] : "?";

    public bool IsAdmin => _session.CurrentUser?.IsAdmin == true;

    /// <summary>AD users change their password in Active Directory, not here.</summary>
    public bool CanChangePassword => _session.CurrentUser?.AuthType == UserAuthType.Local;

    [ObservableProperty]
    public partial ViewModelBase CurrentSection { get; set; }

    /// <summary>Route of the current section; the sidebar highlights the matching item.</summary>
    [ObservableProperty]
    public partial string CurrentRoute { get; set; }

    /// <summary>Sidebar shows labels when expanded, icons only when collapsed.</summary>
    [ObservableProperty]
    public partial bool IsSidebarExpanded { get; set; } = true;

    public event EventHandler? SignedOut;

    [RelayCommand]
    private void ToggleSidebar() => IsSidebarExpanded = !IsSidebarExpanded;

    [RelayCommand]
    private void ShowDashboard()
    {
        if (CurrentSection is not DashboardViewModel)
        {
            Navigate(_createDashboard(), DashboardRoute);
        }
    }

    [RelayCommand]
    private void ShowUsers()
    {
        if (IsAdmin && CurrentSection is not UsersViewModel)
        {
            var users = _createUsers();
            Navigate(users, UsersRoute);
            users.LoadCommand.Execute(null);
        }
    }

    [RelayCommand]
    private void ShowChangePassword()
    {
        if (CanChangePassword && CurrentSection is not ChangePasswordViewModel)
        {
            Navigate(_createChangePassword(), ChangePasswordRoute);
        }
    }

    [RelayCommand]
    private async Task SignOutAsync()
    {
        await _session.SignOutAsync();
        SignedOut?.Invoke(this, EventArgs.Empty);
    }

    private void Navigate(ViewModelBase section, string route)
    {
        CurrentSection = section;
        CurrentRoute = route;
    }
}
