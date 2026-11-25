using System.Text.RegularExpressions;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using TaskDoc.Client.ViewModels;

namespace TaskDoc.App.Tests;

/// <summary>
/// Every page / dialog view model in TaskDoc.Client needs a view in both apps. A missing mapping does not fail the
/// build: Avalonia shows a "No view registered" placeholder, WPF shows the type name. The WPF app cannot run here
/// (Windows only), so its App.xaml is checked as text.
/// </summary>
public sealed partial class ViewMappingTests
{
    /// <summary>View models that are not shown through a view mapping (MainViewModel is the window's DataContext).</summary>
    private static readonly Type[] NotMapped = [typeof(MainViewModel)];

    public static TheoryData<string> ViewModels() =>
        new(typeof(ViewModelBase).Assembly.GetTypes()
            .Where(t => t.IsSubclassOf(typeof(ViewModelBase)) && !t.IsAbstract && !NotMapped.Contains(t))
            .Select(t => t.Name)
            .Order());

    [AvaloniaTheory]
    [MemberData(nameof(ViewModels))]
    public void AvaloniaViewLocator_HasAView(string viewModel)
    {
        var view = new ViewLocator().Build(CreateUninitialized(viewModel));

        Assert.False(view is TextBlock { Text: { } text } && text.StartsWith("No view registered"),
            $"ViewLocator has no view for {viewModel}");
    }

    [Theory]
    [MemberData(nameof(ViewModels))]
    public void WpfApp_HasADataTemplate(string viewModel)
    {
        var appXaml = File.ReadAllText(Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "../../../../../src/TaskDoc.App.Wpf/App.xaml")));
        var mapped = WpfDataTemplate().Matches(appXaml).Select(m => m.Groups[1].Value);

        Assert.Contains(viewModel, mapped);
    }

    /// <summary>The view locator only looks at the type, so constructor arguments are not needed.</summary>
    private static object CreateUninitialized(string viewModel) =>
        System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject(
            typeof(ViewModelBase).Assembly.GetTypes().Single(t => t.Name == viewModel));

    [GeneratedRegex(@"<DataTemplate DataType=""\{x:Type vm:(\w+)\}"">")]
    private static partial Regex WpfDataTemplate();
}
