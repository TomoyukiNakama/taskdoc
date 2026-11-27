namespace TaskDoc.WinFormsApp.Views;

/// <summary>
/// A page, section or dialog body for one view model. Disposing it unhooks it from the view model.
/// Not abstract: the Windows Forms designer creates an instance of the base class to show a view.
/// </summary>
public class ViewBase : UserControl
{
    protected Binder Bind { get; } = new();

    /// <summary>The control to focus when the view appears.</summary>
    public virtual Control? InitialFocus => null;

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        InitialFocus?.Select();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Bind.Dispose();
        }

        base.Dispose(disposing);
    }
}
