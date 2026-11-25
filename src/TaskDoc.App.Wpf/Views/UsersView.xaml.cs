using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TaskDoc.Client.ViewModels;

namespace TaskDoc.WpfApp.Views;

public partial class UsersView : UserControl
{
    public UsersView()
    {
        InitializeComponent();
    }

    /// <summary>Double-click a row to edit it (ignores double-clicks on the header or scroll bar).</summary>
    private void OnUserDoubleClick(object sender, MouseButtonEventArgs e)
    {
        for (var element = e.OriginalSource as DependencyObject; element is not null; element = VisualTreeHelper.GetParent(element))
        {
            if (element is DataGridRow)
            {
                if (DataContext is UsersViewModel vm && vm.EditCommand.CanExecute(null))
                {
                    vm.EditCommand.Execute(null);
                }

                return;
            }
        }
    }
}
