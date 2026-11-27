using Microsoft.Extensions.DependencyInjection;
using TaskDoc.Client.Services;
using TaskDoc.WinFormsApp.Services;

namespace TaskDoc.WinFormsApp;

internal static class Program
{
    /// <summary>
    /// The API address can be changed with the environment variable TASKDOC_API_URL
    /// (default: http://localhost:5227, the API's "http" launch profile).
    /// </summary>
    [STAThread]
    private static void Main()
    {
        // High DPI, visual styles and the default font (see the csproj).
        ApplicationConfiguration.Initialize();

        // View models are created before any control exists; install the UI context now so that
        // WinFormsUiDispatcher can capture it and awaits in view models resume on the UI thread.
        SynchronizationContext.SetSynchronizationContext(new WindowsFormsSynchronizationContext());

        var options = new ClientOptions();
        if (Environment.GetEnvironmentVariable("TASKDOC_API_URL") is { Length: > 0 } apiUrl
            && Uri.TryCreate(apiUrl, UriKind.Absolute, out var apiUri))
        {
            options.ApiBaseAddress = apiUri;
        }

        using var services = new ServiceCollection()
            .AddTaskDocWinFormsApp(options)
            .BuildServiceProvider();

        Application.Run(services.GetRequiredService<MainForm>());
    }
}
