using Avalonia;
using Avalonia.Headless;

[assembly: AvaloniaTestApplication(typeof(TaskDoc.App.Tests.TestAppBuilder))]

namespace TaskDoc.App.Tests;

/// <summary>Runs the real <see cref="App"/> (styles and resources) on the headless platform.</summary>
public static class TestAppBuilder
{
    public static AppBuilder BuildAvaloniaApp()
    {
        // The headless runner creates an App per test; developer tools can only be attached once per process.
        App.EnableDeveloperTools = false;
        return AppBuilder.Configure<App>().UseHeadless(new AvaloniaHeadlessPlatformOptions());
    }
}
