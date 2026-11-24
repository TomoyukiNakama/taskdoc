using System.Text.RegularExpressions;
using Avalonia;
using Avalonia.Headless.XUnit;
using Avalonia.Styling;

namespace TaskDoc.App.Tests;

/// <summary>
/// A DynamicResource with a missing key fails silently (the property just stays unset), so check that every
/// key the app's XAML references exists in both themes (they come from ShadUI's ShadTheme or App.axaml).
/// </summary>
public sealed partial class AppResourceKeysTests
{
    private static IReadOnlyList<(string File, string Key)> UsedKeys()
    {
        var appDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../../src/TaskDoc.App"));
        return Directory.EnumerateFiles(appDir, "*.axaml", SearchOption.AllDirectories)
            .Where(f => !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}"))
            .SelectMany(file => ResourceReference().Matches(File.ReadAllText(file))
                .Select(m => (File: Path.GetFileName(file), Key: m.Groups[1].Value)))
            .Distinct()
            .ToList();
    }

    [AvaloniaFact]
    public void EveryResourceKeyUsedInXaml_ResolvesInLightAndDark()
    {
        var used = UsedKeys();
        Assert.True(used.Count > 10, "expected the views to reference theme resources");

        var missing = (
            from u in used
            from variant in new[] { ThemeVariant.Light, ThemeVariant.Dark }
            where !(Application.Current!.TryGetResource(u.Key, variant, out var value) && value is not null)
            select $"{u.File}: {u.Key} ({variant})").ToList();

        Assert.Empty(missing);
    }

    [GeneratedRegex(@"\{(?:DynamicResource|StaticResource) (\w+)\}")]
    private static partial Regex ResourceReference();
}
