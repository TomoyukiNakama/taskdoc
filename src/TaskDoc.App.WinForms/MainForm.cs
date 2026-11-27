using TaskDoc.Client.ViewModels;
using TaskDoc.WinFormsApp.Services;
using TaskDoc.WinFormsApp.Views;

namespace TaskDoc.WinFormsApp;

/// <summary>The window: the current page (login or home) and a toast bar for notifications.</summary>
public partial class MainForm : Form
{
    private readonly Binder _bind = new();
    private ViewBase? _page;

    public MainForm(MainViewModel shell, WinFormsDialogService dialogs, WinFormsNotificationPresenter notifications)
    {
        InitializeComponent();
        Disposed += (_, _) => _bind.Dispose();

        dialogs.SetOwner(this);
        notifications.SetPresenter(ShowToast);
        _bind.Watch(shell, () => ShowPage(shell.CurrentPage), nameof(shell.CurrentPage));
    }

    /// <summary>The old page is disposed (which unhooks it from its view model); the new one fills the window.</summary>
    private void ShowPage(ViewModelBase page)
    {
        var old = _page;
        _page = ViewLocator.Create(page);
        _page.Dock = DockStyle.Fill;
        content.Controls.Add(_page);
        if (old is not null)
        {
            content.Controls.Remove(old);
            old.Dispose();
        }

        _page.InitialFocus?.Select();
    }

    private void ShowToast(string message)
    {
        toast.Text = "✓  " + message;
        toast.Visible = true;
        toastTimer.Stop();
        toastTimer.Start();
    }

    private void HideToast()
    {
        toastTimer.Stop();
        toast.Visible = false;
    }

    private void OnToastClick(object? sender, EventArgs e) => HideToast();

    private void OnToastTimerTick(object? sender, EventArgs e) => HideToast();
}
