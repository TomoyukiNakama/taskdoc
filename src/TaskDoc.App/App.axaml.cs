using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using TaskDoc.App.Services;
using TaskDoc.App.ViewModels;
using TaskDoc.App.Views;

namespace TaskDoc.App;

public partial class App : Application
{
    /// <summary>
    /// Client settings. Platform heads adjust this before calling Start* (for example, the Android
    /// emulator reaches the host machine at 10.0.2.2 instead of localhost).
    /// </summary>
    public static ClientOptions Options { get; } = new();

    public IServiceProvider Services { get; private set; } = null!;

    /// <summary>
    /// Attach Avalonia developer tools (F12) in Debug builds. Tests turn this off: the headless runner creates
    /// a new App per test, and developer tools can be attached only once per process.
    /// </summary>
    public static bool EnableDeveloperTools { get; set; } = true;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
#if DEBUG
        if (EnableDeveloperTools)
        {
            this.AttachDeveloperTools();
        }
#endif
    }

    public override void OnFrameworkInitializationCompleted()
    {
        Services = new ServiceCollection()
            .AddTaskDocClient(Options)
            .BuildServiceProvider();

        // Check IActivityApplicationLifetime (Android) before ISingleViewApplicationLifetime (iOS / Browser).
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = Services.GetRequiredService<MainViewModel>()
            };
        }
        else if (ApplicationLifetime is IActivityApplicationLifetime activity)
        {
            activity.MainViewFactory = () => new MainView
            {
                DataContext = Services.GetRequiredService<MainViewModel>()
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleView)
        {
            singleView.MainView = new MainView
            {
                DataContext = Services.GetRequiredService<MainViewModel>()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
