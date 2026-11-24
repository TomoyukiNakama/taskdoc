using Android.App;
using Android.Content.PM;
using Avalonia;
using Avalonia.Android;

namespace TaskDoc.App.Android;

[Activity(
    Label = "TaskDoc.App.Android",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity
{
}
