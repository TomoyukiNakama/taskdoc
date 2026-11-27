using Microsoft.Extensions.DependencyInjection;
using TaskDoc.Client.Services;
using TaskDoc.Client.ViewModels;

namespace TaskDoc.WinFormsApp.Services;

/// <summary>
/// Dialogs as modal forms (DialogForm) owned by MainForm, holding the view model's view (ViewLocator).
/// </summary>
public sealed class WinFormsDialogService : IDialogService
{
    private readonly Dictionary<ViewModelBase, Form> _open = [];
    private Form? _owner;

    /// <summary>Called by MainForm once it exists.</summary>
    public void SetOwner(Form owner) => _owner = owner;

    public void Show<TDialog>(TDialog dialog, Action onClosed) where TDialog : ViewModelBase
    {
        var owner = _owner ?? throw new InvalidOperationException("MainForm has not registered itself yet.");
        var form = new DialogForm(ViewLocator.Create(dialog));
        _open[dialog] = form;

        // Show returns at once (as in the other apps); the modal loop starts after the current event finishes.
        owner.BeginInvoke(() =>
        {
            try
            {
                form.ShowDialog(owner);
            }
            finally
            {
                _open.Remove(dialog);
                form.Dispose(); // also disposes the view, which unhooks it from the view model
                onClosed();
            }
        });
    }

    public void Close<TDialog>(TDialog dialog) where TDialog : ViewModelBase
    {
        if (_open.TryGetValue(dialog, out var form))
        {
            form.Close();
        }
    }
}

/// <summary>Notifications in MainForm's toast bar.</summary>
public sealed class WinFormsNotificationPresenter : INotificationPresenter
{
    private Action<string>? _show;

    /// <summary>Called by MainForm with the method that displays a toast.</summary>
    public void SetPresenter(Action<string> show) => _show = show;

    public void ShowSuccess(string message) => _show?.Invoke(message);
}

/// <summary>Posts to the WinForms synchronization context installed by Program.Main.</summary>
public sealed class WinFormsUiDispatcher : IUiDispatcher
{
    private readonly SynchronizationContext _context = SynchronizationContext.Current
        ?? throw new InvalidOperationException("Create WinFormsUiDispatcher on the UI thread.");

    public void Post(Action action) => _context.Post(_ => action(), null);
}

public static class WinFormsServiceCollectionExtensions
{
    /// <summary>The shared client (TaskDoc.Client) plus its Windows Forms implementations and the main form.</summary>
    public static IServiceCollection AddTaskDocWinFormsApp(this IServiceCollection services, ClientOptions options)
    {
        services.AddTaskDocClient(options);

        services.AddSingleton<WinFormsDialogService>();
        services.AddSingleton<IDialogService>(sp => sp.GetRequiredService<WinFormsDialogService>());
        services.AddSingleton<WinFormsNotificationPresenter>();
        services.AddSingleton<INotificationPresenter>(sp => sp.GetRequiredService<WinFormsNotificationPresenter>());
        services.AddSingleton<IUiDispatcher, WinFormsUiDispatcher>();
        services.AddSingleton<MainForm>();

        return services;
    }
}
