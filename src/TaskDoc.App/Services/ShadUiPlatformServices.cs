using Avalonia.Threading;
using ShadUI;
using TaskDoc.Client.Services;
using TaskDoc.Client.ViewModels;

namespace TaskDoc.App.Services;

/// <summary>Dialogs as ShadUI dialogs. Their host (DialogHost) sits in MainView, so they work on every platform.</summary>
public sealed class ShadUiDialogService(DialogManager dialogs) : IDialogService
{
    public void Show<TDialog>(TDialog dialog, Action onClosed) where TDialog : ViewModelBase =>
        dialogs.CreateDialog(dialog)
            .WithMinWidth(420)
            .WithMaxWidth(560)
            .WithSuccessCallback(onClosed)
            .WithCancelCallback(onClosed)
            .Show();

    public void Close<TDialog>(TDialog dialog) where TDialog : ViewModelBase => dialogs.Close(dialog);
}

/// <summary>Notifications as ShadUI toasts in the corner of the screen (ToastHost in MainView).</summary>
public sealed class ShadUiToastPresenter(ToastManager toasts) : INotificationPresenter
{
    public void ShowSuccess(string message) =>
        toasts.CreateToast(message).WithDelay(4).OnBottomRight().DismissOnClick().ShowSuccess();
}

public sealed class AvaloniaUiDispatcher : IUiDispatcher
{
    public void Post(Action action) => Dispatcher.UIThread.Post(action);
}
