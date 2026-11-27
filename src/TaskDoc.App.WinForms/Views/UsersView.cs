using System.Collections.Specialized;
using System.ComponentModel;
using TaskDoc.Client.ViewModels;

namespace TaskDoc.WinFormsApp.Views;

/// <summary>User management (system administrators): list, register, edit.</summary>
public partial class UsersView : ViewBase
{
    private readonly UsersViewModel _vm;
    private readonly BindingList<UserRow> _rows = [];

    /// <summary>True while the grid is being updated from the view model (its selection events are ignored then).</summary>
    private bool _syncing;

    public UsersView(UsersViewModel vm)
    {
        _vm = vm;
        InitializeComponent();
        Ui.FadeWhenDisabled(RefreshButton, EditButton, NewUserButton);

        // Not shown in the designer's property grid, so set here.
        UserList.AutoGenerateColumns = false;
        UserList.DataSource = _rows;
        UserList.CellFormatting += OnCellFormatting;

        Bind.Label(ErrorBar, vm, nameof(vm.ErrorMessage), () => vm.ErrorMessage);
        Bind.Watch(vm, () => ErrorBar.Visible = errorSpacer.Visible = vm.HasError, nameof(vm.HasError));
        Bind.Command(RefreshButton, vm.LoadCommand);
        Bind.Command(EditButton, vm.EditCommand);
        Bind.Command(NewUserButton, vm.NewUserCommand);

        // Rows follow vm.Users; the selection is kept in sync both ways.
        NotifyCollectionChangedEventHandler usersChanged = (_, _) => SyncRows();
        Bind.On(usersChanged, h => vm.Users.CollectionChanged += h, h => vm.Users.CollectionChanged -= h);
        SyncRows();
        Bind.Watch(vm, SyncSelection, nameof(vm.SelectedUser));
        Bind.On<EventHandler>((_, _) =>
        {
            if (!_syncing)
            {
                vm.SelectedUser = UserList.CurrentRow?.DataBoundItem as UserRow;
            }
        }, h => UserList.SelectionChanged += h, h => UserList.SelectionChanged -= h);
        Bind.On<DataGridViewCellEventHandler>((_, e) =>
        {
            if (e.RowIndex >= 0) // not the header
            {
                Binder.Execute(vm.EditCommand);
            }
        }, h => UserList.CellDoubleClick += h, h => UserList.CellDoubleClick -= h);
    }

    private void SyncRows()
    {
        _syncing = true;
        try
        {
            _rows.RaiseListChangedEvents = false;
            _rows.Clear();
            foreach (var user in _vm.Users)
            {
                _rows.Add(user);
            }

            _rows.RaiseListChangedEvents = true;
            _rows.ResetBindings();
        }
        finally
        {
            _syncing = false;
        }

        SyncSelection();
    }

    private void SyncSelection()
    {
        _syncing = true;
        try
        {
            var index = _vm.SelectedUser is { } selected ? _rows.IndexOf(selected) : -1;
            if (index < 0)
            {
                UserList.ClearSelection();
                UserList.CurrentCell = null;
            }
            else
            {
                UserList.CurrentCell = UserList.Rows[index].Cells[0];
                UserList.Rows[index].Selected = true;
            }
        }
        finally
        {
            _syncing = false;
        }
    }

    private void OnCellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || UserList.Rows[e.RowIndex].DataBoundItem is not UserRow row)
        {
            return;
        }

        if (e.ColumnIndex == isAdminColumn.Index)
        {
            e.Value = row.IsAdmin ? "管理者" : "";
            e.FormattingApplied = true;
        }

        if (!row.IsActive)
        {
            // Inactive users are drawn faded.
            e.CellStyle!.ForeColor = SystemColors.GrayText;
            e.CellStyle.SelectionForeColor = SystemColors.GrayText;
        }
    }
}
