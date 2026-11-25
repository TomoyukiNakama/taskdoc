using TaskDoc.WpfApp.Services;
using TaskDoc.Client.ViewModels;
using Wpf.Ui;
using Wpf.Ui.Appearance;

namespace TaskDoc.WpfApp;

public partial class MainWindow : Wpf.Ui.Controls.FluentWindow
{
    public MainWindow(MainViewModel shell, WpfDialogService dialogs, ISnackbarService snackbar)
    {
        InitializeComponent();
        DataContext = shell;
        dialogs.SetHost(DialogHost);
        snackbar.SetSnackbarPresenter(SnackbarPresenter);

        // Follow the Windows light / dark setting, now and when it changes.
        Loaded += (_, _) => SystemThemeWatcher.Watch(this);
    }
}
