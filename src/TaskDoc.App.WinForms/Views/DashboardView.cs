using TaskDoc.Client.ViewModels;

namespace TaskDoc.WinFormsApp.Views;

/// <summary>Placeholder until the task and document screens exist.</summary>
public partial class DashboardView : ViewBase
{
    public DashboardView(DashboardViewModel vm)
    {
        InitializeComponent();
        Ui.FadeWhenDisabled(CheckConnectionButton);

        Bind.Label(ConnectionStatusText, vm, nameof(vm.ConnectionStatus), () => vm.ConnectionStatus);
        Bind.Command(CheckConnectionButton, vm.CheckConnectionCommand, busyText: "確認中…");
    }
}
