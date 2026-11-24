using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TaskDoc.App.ViewModels;
using TaskDoc.App.Views;
using TaskDoc.Contracts.Users;

namespace TaskDoc.App.Tests;

/// <summary>Helpers for driving the real views on the headless platform.</summary>
public static class Ui
{
    public static (Window Window, MainViewModel Shell) ShowApp(TestClient client)
    {
        var shell = client.Get<MainViewModel>();
        var window = new Window { Content = new MainView { DataContext = shell }, Width = 1000, Height = 700 };
        window.Show();
        Dispatcher.UIThread.RunJobs();
        return (window, shell);
    }

    /// <summary>
    /// The control with this name. Different views may reuse a name (e.g. "ErrorText" in the user list and in
    /// the edit panel over it); then the one that is actually visible is returned.
    /// </summary>
    public static T Find<T>(Window window, string name) where T : Control
    {
        Dispatcher.UIThread.RunJobs();
        var matches = window.GetVisualDescendants().OfType<T>().Where(c => c.Name == name).ToList();
        return matches.Count == 1 ? matches[0] : matches.Single(c => c.IsEffectivelyVisible);
    }

    public static bool Exists(Window window, string name)
    {
        Dispatcher.UIThread.RunJobs();
        return window.GetVisualDescendants().OfType<Control>().Any(c => c.Name == name);
    }

    /// <summary>Clicks a button through its command and waits for an async command to finish.</summary>
    public static async Task ClickAsync(Window window, string buttonName)
    {
        var button = Find<Button>(window, buttonName);
        Assert.True(button.IsEffectivelyEnabled, $"{buttonName} is disabled");
        button.Command!.Execute(button.CommandParameter);
        await WaitUntilAsync(() => button.Command is not CommunityToolkit.Mvvm.Input.IAsyncRelayCommand { IsRunning: true });
    }

    public static async Task WaitUntilAsync(Func<bool> condition)
    {
        for (var i = 0; i < 200 && !condition(); i++)
        {
            await Task.Delay(5);
            Dispatcher.UIThread.RunJobs();
        }

        Dispatcher.UIThread.RunJobs();
    }

    public static void Type(Window window, string textBoxName, string text)
    {
        Find<TextBox>(window, textBoxName).Text = text;
        Dispatcher.UIThread.RunJobs();
    }

    /// <summary>Signs in through the login screen as the given kind of user and returns the home shell.</summary>
    public static async Task<(Window Window, HomeViewModel Home)> SignInAsync(
        TestClient client, bool isAdmin = false, UserAuthType authType = UserAuthType.Local)
    {
        client.Api.OnJson("POST /api/auth/login", TestClient.Tokens("1", TestClient.Start, isAdmin, authType));
        var (window, shell) = ShowApp(client);
        Type(window, "UserNameBox", "taro");
        Type(window, "PasswordBox", "password123");
        await ClickAsync(window, "LoginButton");
        return (window, Assert.IsType<HomeViewModel>(shell.CurrentPage));
    }
}
