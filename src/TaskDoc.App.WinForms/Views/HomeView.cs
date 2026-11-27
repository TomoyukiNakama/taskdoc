using TaskDoc.Client.ViewModels;

namespace TaskDoc.WinFormsApp.Views;

/// <summary>Signed-in shell: sidebar (labels when expanded, icons only when collapsed) and the current section.</summary>
public partial class HomeView : ViewBase
{
    private const int ExpandedWidth = 230;
    private const int CollapsedWidth = 60;

    /// <summary>Sidebar buttons with their icon; the label is the button's AccessibleName (set in the designer).</summary>
    private readonly (Button Button, string Icon)[] _navItems;

    /// <summary>Whether the sidebar currently has its expanded width (the designer draws it expanded).</summary>
    private bool _shownExpanded = true;

    private ViewBase? _section;

    public HomeView(HomeViewModel vm)
    {
        InitializeComponent();
        _navItems = [(DashboardNav, "🏠"), (UsersNav, "👥"), (ChangePasswordNav, "🔑"), (SignOutButton, "⇥")];

        // Sections appear only to users who may use them.
        UsersNav.Visible = vm.IsAdmin;
        ChangePasswordNav.Visible = vm.CanChangePassword;

        Bind.Watch(vm, () =>
        {
            var expanded = vm.IsSidebarExpanded;
            if (expanded != _shownExpanded)
            {
                // Scale from the current width rather than setting a fixed one: this also runs before the view is
                // scaled to the screen's DPI (from the constructor) and after it.
                sidebar.Width = expanded
                    ? sidebar.Width * ExpandedWidth / CollapsedWidth
                    : sidebar.Width * CollapsedWidth / ExpandedWidth;
                _shownExpanded = expanded;
            }

            appNameLabel.Visible = expanded;
            adminHeading.Visible = expanded && vm.IsAdmin;
            accountHeading.Visible = expanded && vm.CanChangePassword;
            foreach (var (button, icon) in _navItems)
            {
                button.Text = expanded ? $"{icon}   {button.AccessibleName}" : icon;
                button.TextAlign = expanded ? ContentAlignment.MiddleLeft : ContentAlignment.MiddleCenter;
                button.Width = sidebar.ClientSize.Width - sidebar.Padding.Horizontal;
            }

            UserText.Text = expanded ? $"{vm.DisplayName}{(vm.IsAdmin ? "（管理者）" : "")}\n{vm.UserName}" : vm.Initial;
            UserText.TextAlign = expanded ? ContentAlignment.MiddleLeft : ContentAlignment.MiddleCenter;
        }, nameof(vm.IsSidebarExpanded));

        Bind.Watch(vm, () =>
        {
            Highlight(DashboardNav, vm.CurrentRoute == HomeViewModel.DashboardRoute);
            Highlight(UsersNav, vm.CurrentRoute == HomeViewModel.UsersRoute);
            Highlight(ChangePasswordNav, vm.CurrentRoute == HomeViewModel.ChangePasswordRoute);
        }, nameof(vm.CurrentRoute));

        Bind.Watch(vm, () => ShowSection(vm.CurrentSection), nameof(vm.CurrentSection));

        Bind.Command(SidebarToggle, vm.ToggleSidebarCommand);
        Bind.Command(DashboardNav, vm.ShowDashboardCommand);
        Bind.Command(UsersNav, vm.ShowUsersCommand);
        Bind.Command(ChangePasswordNav, vm.ShowChangePasswordCommand);
        Bind.Command(SignOutButton, vm.SignOutCommand);
    }

    private static void Highlight(Button button, bool selected)
    {
        button.BackColor = selected ? Ui.Selected : Ui.Surface;
        button.Font = new Font(button.Font, selected ? FontStyle.Bold : FontStyle.Regular);
    }

    /// <summary>The old section is disposed (which unhooks it from its view model); the new one fills the area.</summary>
    private void ShowSection(ViewModelBase section)
    {
        var old = _section;
        _section = ViewLocator.Create(section);
        _section.Dock = DockStyle.Fill;
        content.Controls.Add(_section);
        if (old is not null)
        {
            content.Controls.Remove(old);
            old.Dispose();
        }
    }
}
