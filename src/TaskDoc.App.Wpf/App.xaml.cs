using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using TaskDoc.WpfApp.Services;
using TaskDoc.Client.Services;

namespace TaskDoc.WpfApp;

public partial class App : Application
{
    private ServiceProvider? _services;

    /// <summary>
    /// Client settings. The API address can be changed with the environment variable TASKDOC_API_URL
    /// (default: http://localhost:5227, the API's "http" launch profile).
    /// </summary>
    public static ClientOptions Options { get; } = new();

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        if (Environment.GetEnvironmentVariable("TASKDOC_API_URL") is { Length: > 0 } apiUrl
            && Uri.TryCreate(apiUrl, UriKind.Absolute, out var apiUri))
        {
            Options.ApiBaseAddress = apiUri;
        }

        _services = new ServiceCollection()
            .AddTaskDocWpfApp(Options)
            .BuildServiceProvider();

        MainWindow = _services.GetRequiredService<MainWindow>();
        MainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _services?.Dispose();
        base.OnExit(e);
    }
}
