using System.ComponentModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace TaskDoc.WinFormsApp;

/// <summary>
/// Connects a view's controls to view model properties and commands (the role XAML bindings play in the other apps).
/// Each view owns one and disposes it with the view, which removes every handler it added, so a view model that
/// outlives its view (a save still running after its dialog closed) no longer touches disposed controls.
/// </summary>
/// <remarks>
/// View models raise PropertyChanged on the UI thread: their awaits resume on the WinForms synchronization context,
/// and AuthSession.SessionExpired is marshalled through <see cref="Services.WinFormsUiDispatcher"/>.
/// </remarks>
public sealed class Binder : IDisposable
{
    private readonly List<Action> _cleanup = [];

    /// <summary>Runs <paramref name="update"/> now and whenever one of <paramref name="properties"/> changes.</summary>
    public void Watch(INotifyPropertyChanged source, Action update, params string[] properties)
    {
        PropertyChangedEventHandler handler = (_, e) =>
        {
            if (string.IsNullOrEmpty(e.PropertyName) || properties.Contains(e.PropertyName))
            {
                update();
            }
        };
        source.PropertyChanged += handler;
        _cleanup.Add(() => source.PropertyChanged -= handler);
        update();
    }

    /// <summary>Two-way text: the view model follows typing, the box follows the view model (e.g. cleared after save).</summary>
    public void Text(TextBox box, INotifyPropertyChanged source, string property, Func<string> get, Action<string> set)
    {
        Watch(source, () =>
        {
            var value = get();
            if (box.Text != value)
            {
                box.Text = value;
            }
        }, property);
        On<EventHandler>((_, _) => set(box.Text), h => box.TextChanged += h, h => box.TextChanged -= h);
    }

    /// <summary>Two-way check state.</summary>
    public void Checked(CheckBox box, INotifyPropertyChanged source, string property, Func<bool> get, Action<bool> set)
    {
        Watch(source, () => box.Checked = get(), property);
        On<EventHandler>((_, _) => set(box.Checked), h => box.CheckedChanged += h, h => box.CheckedChanged -= h);
    }

    /// <summary>A pair of radio buttons for one bool (first = true).</summary>
    public void Choice(RadioButton whenTrue, RadioButton whenFalse, INotifyPropertyChanged source, string property,
        Func<bool> get, Action<bool> set)
    {
        Watch(source, () =>
        {
            var value = get();
            whenTrue.Checked = value;
            whenFalse.Checked = !value;
        }, property);
        EventHandler changed = (sender, _) =>
        {
            if (sender is RadioButton { Checked: true } radio)
            {
                set(radio == whenTrue);
            }
        };
        On(changed, h => whenTrue.CheckedChanged += h, h => whenTrue.CheckedChanged -= h);
        On(changed, h => whenFalse.CheckedChanged += h, h => whenFalse.CheckedChanged -= h);
    }

    public void Label(Control control, INotifyPropertyChanged source, string property, Func<string?> get) =>
        Watch(source, () => control.Text = get() ?? "", property);

    public void Visible(Control control, INotifyPropertyChanged source, string property, Func<bool> get) =>
        Watch(source, () => control.Visible = get(), property);

    public void Enabled(Control control, INotifyPropertyChanged source, string property, Func<bool> get) =>
        Watch(source, () => control.Enabled = get(), property);

    /// <summary>
    /// Click runs the command; the button is disabled while it cannot run (an async command cannot run while it is
    /// already running, so a second click is ignored). With <paramref name="busyText"/>, the caption changes while
    /// the command runs.
    /// </summary>
    public void Command(ButtonBase button, ICommand command, string? busyText = null)
    {
        void Refresh() => button.Enabled = command.CanExecute(null);

        EventHandler canExecuteChanged = (_, _) => Refresh();
        command.CanExecuteChanged += canExecuteChanged;
        _cleanup.Add(() => command.CanExecuteChanged -= canExecuteChanged);
        Refresh();

        On<EventHandler>((_, _) => Execute(command), h => button.Click += h, h => button.Click -= h);

        if (busyText is not null && command is IAsyncRelayCommand async)
        {
            var idleText = button.Text;
            Watch(async, () => button.Text = async.IsRunning ? busyText : idleText, nameof(async.IsRunning));
        }
    }

    /// <summary>Enter in any of <paramref name="controls"/> runs <paramref name="command"/> (like a form's AcceptButton).</summary>
    public void EnterExecutes(ICommand command, params Control[] controls)
    {
        foreach (var control in controls)
        {
            KeyEventHandler handler = (_, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true; // no beep from single-line text boxes
                    Execute(command);
                }
            };
            control.KeyDown += handler;
            _cleanup.Add(() => control.KeyDown -= handler);
        }
    }

    public static void Execute(ICommand command)
    {
        if (command.CanExecute(null))
        {
            command.Execute(null);
        }
    }

    /// <summary>Adds an event handler that <see cref="Dispose"/> removes.</summary>
    public void On<THandler>(THandler handler, Action<THandler> add, Action<THandler> remove)
        where THandler : Delegate
    {
        add(handler);
        _cleanup.Add(() => remove(handler));
    }

    public void Dispose()
    {
        foreach (var undo in _cleanup)
        {
            undo();
        }

        _cleanup.Clear();
    }
}
