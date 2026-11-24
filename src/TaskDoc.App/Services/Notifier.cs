using ShadUI;

namespace TaskDoc.App.Services;

/// <summary>
/// Short-lived notifications ("registered", "updated") shown as ShadUI toasts in the corner of the screen.
/// The ToastHost in MainView displays them on every platform.
/// </summary>
public sealed class Notifier(ToastManager toasts)
{
    /// <summary>The most recent message, for tests and diagnostics.</summary>
    public string? LastMessage { get; private set; }

    public void Success(string message)
    {
        LastMessage = message;
        toasts.CreateToast(message).WithDelay(4).OnBottomRight().DismissOnClick().ShowSuccess();
    }
}
