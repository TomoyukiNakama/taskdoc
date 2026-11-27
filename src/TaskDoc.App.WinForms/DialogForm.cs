using TaskDoc.WinFormsApp.Views;

namespace TaskDoc.WinFormsApp;

/// <summary>
/// The window around a dialog view (WinFormsDialogService). It has no buttons of its own: the views have theirs.
/// Esc and the title bar's close button close it.
/// </summary>
public partial class DialogForm : Form
{
    private readonly ViewBase _view;

    public DialogForm(ViewBase view)
    {
        InitializeComponent();
        _view = view;
        Controls.Add(view);
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        _view.InitialFocus?.Select();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.KeyCode == Keys.Escape)
        {
            e.Handled = true;
            Close();
        }
    }
}
