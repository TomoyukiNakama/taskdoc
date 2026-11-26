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

        // Refit 16: client implementations and JSON metadata are source-generated at compile time
        // (no reflection, so trimming / WASM are safe). New DTOs must be added to TaskDocJsonContext.
        // The serializer is set explicitly: Refit's default options add a camelCase enum converter, which would
        // send "local" instead of "Local" and override the [JsonConverter] on the enum types.
        RefitSettings Settings(IServiceProvider _) => new(new SystemTextJsonContentSerializer(TaskDocJsonContext.Default.Options))
        {
            HttpMessageHandlerFactory = options.HttpMessageHandlerFactory,
        };

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
