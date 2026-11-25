using Microsoft.Extensions.DependencyInjection;
using Refit;
using TaskDoc.Client.ViewModels;
using TaskDoc.Contracts;

namespace TaskDoc.Client.Services;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// API clients, the sign-in session and every view model. The app must also register
    /// <see cref="IDialogService"/>, <see cref="INotificationPresenter"/> and <see cref="IUiDispatcher"/>.
    /// </summary>
    public static IServiceCollection AddTaskDocClient(this IServiceCollection services, ClientOptions options)
    {
        services.AddSingleton(options);
        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<AuthSession>();
        services.AddTransient<AuthHeaderHandler>();
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
