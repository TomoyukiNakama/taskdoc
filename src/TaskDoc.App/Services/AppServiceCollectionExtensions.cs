using Microsoft.Extensions.DependencyInjection;
using ShadUI;
using TaskDoc.App.Views;
using TaskDoc.Client.Services;
using TaskDoc.Client.ViewModels;

namespace TaskDoc.App.Services;

public static class AppServiceCollectionExtensions
{
    /// <summary>The shared client (TaskDoc.Client) plus its Avalonia / ShadUI implementations.</summary>
    public static IServiceCollection AddTaskDocAvaloniaApp(this IServiceCollection services, ClientOptions options)
    {
        services.AddTaskDocClient(options);

        // ShadUI dialogs and toasts. Their hosts (DialogHost / ToastHost) sit in MainView.
        // Each custom dialog maps its view model to a view here.
        services.AddSingleton(_ => new DialogManager().Register<UserEditorView, UserEditorViewModel>());
        services.AddSingleton<ToastManager>();
        services.AddSingleton<IDialogService, ShadUiDialogService>();
        services.AddSingleton<INotificationPresenter, ShadUiToastPresenter>();
        services.AddSingleton<IUiDispatcher, AvaloniaUiDispatcher>();
        services.AddSingleton<MainViewContext>();

        return services;
    }
}
