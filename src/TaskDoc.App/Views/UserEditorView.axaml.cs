using Avalonia.Controls;
using Avalonia.Interactivity;

namespace TaskDoc.App.Views;

public partial class UserEditorView : UserControl
{
    public UserEditorView()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        // New user: start at the user name. Existing user: the name is fixed, so start at the display name.
        (UserNameBox.IsReadOnly ? DisplayNameBox : UserNameBox).Focus();
    }
}
