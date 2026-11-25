using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using TaskDoc.Client.Services;
using TaskDoc.Client.ViewModels;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace TaskDoc.WpfApp.Services;

/// <summary>
/// Dialogs as WPF-UI ContentDialogs inside MainWindow's ContentDialogHost. The dialog's content is the view model;
/// App.xaml's DataTemplates pick the view. Footer buttons are hidden because the views have their own.
/// </summary>
public sealed class WpfDialogService : IDialogService
{
    private readonly Dictionary<ViewModelBase, ContentDialog> _open = [];
    private ContentDialogHost? _host;

    /// <summary>Called by MainWindow once its ContentDialogHost exists.</summary>
    public void SetHost(ContentDialogHost host) => _host = host;

    public void Show<TDialog>(TDialog dialog, Action onClosed) where TDialog : ViewModelBase
    {
        var host = _host ?? throw new InvalidOperationException("MainWindow has not registered its ContentDialogHost yet.");
        var contentDialog = new ContentDialog(host)
        {
            Content = dialog,
            IsFooterVisible = false,
            DialogMaxWidth = 560,
            // The default margin (35) caps the dialog's height; a smaller one lets the user editor fit the
            // default window height (720) without scrolling.
            DialogMargin = new Thickness(12),
        };
        _open[dialog] = contentDialog;
        _ = ShowAsync(dialog, contentDialog, onClosed);
    }

    public void Close<TDialog>(TDialog dialog) where TDialog : ViewModelBase
    {
        if (_open.TryGetValue(dialog, out var contentDialog))
        {
            contentDialog.Hide(ContentDialogResult.None);
        }
    }

    private async Task ShowAsync(ViewModelBase dialog, ContentDialog contentDialog, Action onClosed)
    {
        try
        {
            await contentDialog.ShowAsync();
        }
        finally
        {
            _open.Remove(dialog);
            onClosed();
        }
    }
}

/// <summary>Notifications as a WPF-UI snackbar at the bottom of MainWindow.</summary>
public sealed class WpfSnackbarPresenter(ISnackbarService snackbar) : INotificationPresenter
{
    public void ShowSuccess(string message) =>
        snackbar.Show("完了", message, ControlAppearance.Success,
            new SymbolIcon { Symbol = SymbolRegular.CheckmarkCircle24 }, TimeSpan.FromSeconds(4));
}

public sealed class WpfUiDispatcher : IUiDispatcher
{
    public void Post(Action action) => Application.Current.Dispatcher.BeginInvoke(action);
}

public static class WpfServiceCollectionExtensions
{
    /// <summary>The shared client (TaskDoc.Client) plus its WPF / WPF-UI implementations and the main window.</summary>
    public static IServiceCollection AddTaskDocWpfApp(this IServiceCollection services, ClientOptions options)
    {
        services.AddTaskDocClient(options);

        services.AddSingleton<WpfDialogService>();
        services.AddSingleton<IDialogService>(sp => sp.GetRequiredService<WpfDialogService>());
        services.AddSingleton<ISnackbarService, SnackbarService>();
        services.AddSingleton<INotificationPresenter, WpfSnackbarPresenter>();
        services.AddSingleton<IUiDispatcher, WpfUiDispatcher>();
        services.AddSingleton<MainWindow>();

        return services;
    }
}
