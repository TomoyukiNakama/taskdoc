using Avalonia;
using Avalonia.Media;

namespace TaskDoc.App;

public static class AppBuilderExtensions
{
    /// <summary>
    /// Fonts for every platform head. Call this instead of WithInterFont() alone.
    /// <para>
    /// ShadUI expects Inter, and some of its parts (e.g. the window title) ask for the default font at Medium
    /// weight. Without an explicit default, Avalonia uses whatever the OS reports, which can be a font without
    /// that weight (on one Linux machine it was the serif "C059"), and startup then fails with
    /// "Could not create glyphTypeface ... $Default ... Weight: Medium". Making Inter the default avoids that.
    /// Characters Inter lacks (Japanese) still fall back to system fonts such as Noto Sans CJK.
    /// </para>
    /// </summary>
    public static AppBuilder UseTaskDocFonts(this AppBuilder builder) => builder
        .WithInterFont()
        .With(new FontManagerOptions { DefaultFamilyName = "fonts:Inter#Inter" });
}
