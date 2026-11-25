using Avalonia.Controls;
using Avalonia.Input;
using TaskDoc.Client.ViewModels;

namespace TaskDoc.App.Views;

public partial class UsersView : UserControl
{
    public UsersView()
    {
        InitializeComponent();
    }

    /// <summary>Double-click (or double-tap) a row to edit it.</summary>
    private void OnUserDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (DataContext is UsersViewModel vm && vm.EditCommand.CanExecute(null))
        {
            vm.EditCommand.Execute(null);
        }
    }
}
