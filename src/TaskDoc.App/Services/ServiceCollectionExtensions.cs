using Microsoft.Extensions.DependencyInjection;
using Refit;
using ShadUI;
using TaskDoc.App.ViewModels;
using TaskDoc.App.Views;
using TaskDoc.Contracts;

namespace TaskDoc.App.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTaskDocClient(this IServiceCollection services, ClientOptions options)
    {
        services.AddSingleton(options);
        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<AuthSession>();
        services.AddTransient<AuthHeaderHandler>();

        // ShadUI dialogs and toasts. Their hosts (DialogHost / ToastHost) sit in MainView.
        // Each custom dialog maps its view model to a view here.
        services.AddSingleton(_ => new DialogManager().Register<UserEditorView, UserEditorViewModel>());
        services.AddSingleton<ToastManager>();
        services.AddSingleton<Notifier>();

        // Refit 17: client implementations and JSON metadata are source-generated at compile time
        // (no reflection, so trimming / WASM are safe). New DTOs must be added to TaskDocJsonContext.
        RefitSettings Settings(IServiceProvider _) => new() { HttpMessageHandlerFactory = options.HttpMessageHandlerFactory };

        services.AddRefitGeneratedClient<IAuthApi>(TaskDocJsonContext.Default, Settings)
            .ConfigureHttpClient(client => client.BaseAddress = options.ApiBaseAddress);
        services.AddRefitGeneratedClient<ITaskDocApi>(TaskDocJsonContext.Default, Settings)
            .ConfigureHttpClient(client => client.BaseAddress = options.ApiBaseAddress)
            .AddHttpMessageHandler<AuthHeaderHandler>();

        // Pages and sections. Each is created fresh when navigated to (via Func<T> factories).
        services.AddSingleton<MainViewModel>();
        AddPage<LoginViewModel>(services);
        AddPage<HomeViewModel>(services);
        AddPage<DashboardViewModel>(services);
        AddPage<UsersViewModel>(services);
        AddPage<ChangePasswordViewModel>(services);

        return services;
    }

    private static void AddPage<T>(IServiceCollection services) where T : class
    {
        services.AddTransient<T>();
        services.AddSingleton<Func<T>>(sp => sp.GetRequiredService<T>);
    }
}
