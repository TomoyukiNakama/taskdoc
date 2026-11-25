using ShadUI;
using TaskDoc.Client.ViewModels;

namespace TaskDoc.App.Services;

/// <summary>
/// DataContext of MainView: the shared app shell plus the ShadUI managers its DialogHost and ToastHost bind to
/// (those are Avalonia-specific, so they are not part of <see cref="MainViewModel"/>).
/// </summary>
public sealed class MainViewContext(MainViewModel shell, DialogManager dialogManager, ToastManager toastManager)
{
    public MainViewModel Shell { get; } = shell;

    public DialogManager DialogManager { get; } = dialogManager;

    public ToastManager ToastManager { get; } = toastManager;
}
