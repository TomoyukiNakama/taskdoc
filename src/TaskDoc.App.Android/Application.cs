using Android.App;
using Android.Runtime;
using Avalonia;
using Avalonia.Android;

namespace TaskDoc.App.Android
{
    [Application]
    public class Application : AvaloniaAndroidApplication<App>
    {
        protected Application(nint javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
        {
            // The Android emulator reaches the development machine's localhost via 10.0.2.2.
            App.Options.ApiBaseAddress = new System.Uri("http://10.0.2.2:5227");

            return base.CustomizeAppBuilder(builder)
            .UseTaskDocFonts();
        }
    }
}
