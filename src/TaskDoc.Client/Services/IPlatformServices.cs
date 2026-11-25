using TaskDoc.Client.ViewModels;

namespace TaskDoc.Client.Services;

// What the view models need from the UI framework. TaskDoc.App implements these with Avalonia + ShadUI,
// TaskDoc.App.Wpf with WPF + WPF-UI.

/// <summary>Modal dialogs whose content is a view model (each app maps the view model to its view).</summary>
public interface IDialogService
{
    /// <summary>
    /// Shows <paramref name="dialog"/> as a modal dialog. It is not dismissed by clicking outside it, so typed
    /// input is not lost by accident; the dialog closes itself through <see cref="Close{TDialog}"/>.
    /// </summary>
    /// <param name="onClosed">Called once after the dialog has closed, however it was closed.</param>
    void Show<TDialog>(TDialog dialog, Action onClosed) where TDialog : ViewModelBase;

    void Close<TDialog>(TDialog dialog) where TDialog : ViewModelBase;
}

/// <summary>Displays short-lived notifications (toast / snackbar). Used through <see cref="Notifier"/>.</summary>
public interface INotificationPresenter
{
    void ShowSuccess(string message);
}

/// <summary>Runs work on the UI thread.</summary>
public interface IUiDispatcher
{
    /// <summary>Queues <paramref name="action"/> to run on the UI thread (also when called from it).</summary>
    void Post(Action action);
}
