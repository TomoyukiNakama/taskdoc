namespace TaskDoc.Client.Services;

/// <summary>
/// Short-lived notifications ("registered", "updated"). Each app displays them in its own way
/// (<see cref="INotificationPresenter"/>: ShadUI toast in the Avalonia app, WPF-UI snackbar in the WPF app).
/// </summary>
public sealed class Notifier(INotificationPresenter presenter)
{
    /// <summary>The most recent message, for tests and diagnostics.</summary>
    public string? LastMessage { get; private set; }

    public void Success(string message)
    {
        LastMessage = message;
        presenter.ShowSuccess(message);
    }
}
