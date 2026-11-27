#nullable disable

namespace TaskDoc.WinFormsApp.Views;

partial class UsersView
{
    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        DataGridViewCellStyle headerStyle = new DataGridViewCellStyle();
        DataGridViewCellStyle cellStyle = new DataGridViewCellStyle();
        DataGridViewCellStyle userNameStyle = new DataGridViewCellStyle();
        page = new Panel();
        UserList = new DataGridView();
        userNameColumn = new DataGridViewTextBoxColumn();
        displayNameColumn = new DataGridViewTextBoxColumn();
        authTypeColumn = new DataGridViewTextBoxColumn();
        isAdminColumn = new DataGridViewTextBoxColumn();
        statusColumn = new DataGridViewTextBoxColumn();
        lastLoginColumn = new DataGridViewTextBoxColumn();
        errorSpacer = new Panel();
        ErrorBar = new Label();
        header = new Panel();
        titles = new FlowLayoutPanel();
        titleLabel = new Label();
        subtitleLabel = new Label();
        actions = new FlowLayoutPanel();
        RefreshButton = new Button();
        EditButton = new Button();
        NewUserButton = new Button();
        page.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)UserList).BeginInit();
        header.SuspendLayout();
        titles.SuspendLayout();
        actions.SuspendLayout();
        SuspendLayout();
        //
        // page
        //
        page.Controls.Add(UserList);
        page.Controls.Add(errorSpacer);
        page.Controls.Add(ErrorBar);
        page.Controls.Add(header);
        page.Dock = DockStyle.Fill;
        page.Location = new Point(0, 0);
        page.Name = "page";
        page.Padding = new Padding(32, 24, 32, 24);
        page.Size = new Size(860, 600);
        page.TabIndex = 0;
        //
        // UserList
        //
        UserList.AllowUserToAddRows = false;
        UserList.AllowUserToDeleteRows = false;
        UserList.AllowUserToResizeRows = false;
        UserList.BackgroundColor = Color.White;
        UserList.BorderStyle = BorderStyle.FixedSingle;
        UserList.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        UserList.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
        headerStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
        headerStyle.BackColor = Color.White;
        headerStyle.ForeColor = Color.FromArgb(96, 96, 96);
        headerStyle.Padding = new Padding(6, 0, 0, 0);
        headerStyle.SelectionBackColor = Color.White;
        headerStyle.SelectionForeColor = Color.FromArgb(96, 96, 96);
        UserList.ColumnHeadersDefaultCellStyle = headerStyle;
        UserList.ColumnHeadersHeight = 36;
        UserList.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        UserList.Columns.AddRange(new DataGridViewColumn[] { userNameColumn, displayNameColumn, authTypeColumn, isAdminColumn, statusColumn, lastLoginColumn });
        cellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
        cellStyle.BackColor = Color.White;
        cellStyle.ForeColor = Color.FromArgb(27, 27, 27);
        cellStyle.Padding = new Padding(6, 0, 0, 0);
        cellStyle.SelectionBackColor = Color.FromArgb(225, 236, 248);
        cellStyle.SelectionForeColor = Color.FromArgb(27, 27, 27);
        UserList.DefaultCellStyle = cellStyle;
        UserList.Dock = DockStyle.Fill;
        UserList.EnableHeadersVisualStyles = false;
        UserList.GridColor = Color.FromArgb(235, 235, 235);
        UserList.MultiSelect = false;
        UserList.Location = new Point(32, 144);
        UserList.Name = "UserList";
        UserList.Size = new Size(796, 432);
        UserList.ReadOnly = true;
        UserList.RowHeadersVisible = false;
        UserList.RowTemplate.Height = 36;
        UserList.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        UserList.StandardTab = true;
        UserList.TabIndex = 3;
        //
        // userNameColumn
        //
        userNameColumn.DataPropertyName = "UserName";
        userNameStyle.Font = new Font("Yu Gothic UI", 9.75F, FontStyle.Bold);
        userNameColumn.DefaultCellStyle = userNameStyle;
        userNameColumn.HeaderText = "ユーザー名";
        userNameColumn.MinimumWidth = 60;
        userNameColumn.Name = "userNameColumn";
        userNameColumn.ReadOnly = true;
        userNameColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
        userNameColumn.Width = 160;
        //
        // displayNameColumn
        //
        displayNameColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        displayNameColumn.DataPropertyName = "DisplayName";
        displayNameColumn.HeaderText = "表示名";
        displayNameColumn.MinimumWidth = 60;
        displayNameColumn.Name = "displayNameColumn";
        displayNameColumn.ReadOnly = true;
        displayNameColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
        //
        // authTypeColumn
        //
        authTypeColumn.DataPropertyName = "AuthTypeLabel";
        authTypeColumn.HeaderText = "種類";
        authTypeColumn.MinimumWidth = 60;
        authTypeColumn.Name = "authTypeColumn";
        authTypeColumn.ReadOnly = true;
        authTypeColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
        authTypeColumn.Width = 90;
        //
        // isAdminColumn
        //
        isAdminColumn.DataPropertyName = "IsAdmin";
        isAdminColumn.HeaderText = "権限";
        isAdminColumn.MinimumWidth = 60;
        isAdminColumn.Name = "isAdminColumn";
        isAdminColumn.ReadOnly = true;
        isAdminColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
        isAdminColumn.Width = 90;
        //
        // statusColumn
        //
        statusColumn.DataPropertyName = "StatusLabel";
        statusColumn.HeaderText = "状態";
        statusColumn.MinimumWidth = 60;
        statusColumn.Name = "statusColumn";
        statusColumn.ReadOnly = true;
        statusColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
        statusColumn.Width = 70;
        //
        // lastLoginColumn
        //
        lastLoginColumn.DataPropertyName = "LastLoginLabel";
        lastLoginColumn.HeaderText = "最終ログイン";
        lastLoginColumn.MinimumWidth = 60;
        lastLoginColumn.Name = "lastLoginColumn";
        lastLoginColumn.ReadOnly = true;
        lastLoginColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
        lastLoginColumn.Width = 150;
        //
        // errorSpacer
        //
        errorSpacer.Dock = DockStyle.Top;
        errorSpacer.Location = new Point(32, 132);
        errorSpacer.Name = "errorSpacer";
        errorSpacer.Size = new Size(796, 12);
        errorSpacer.TabIndex = 2;
        //
        // ErrorBar
        //
        ErrorBar.BackColor = Color.FromArgb(253, 231, 233);
        ErrorBar.Dock = DockStyle.Top;
        ErrorBar.ForeColor = Color.FromArgb(196, 43, 28);
        ErrorBar.Location = new Point(32, 96);
        ErrorBar.Name = "ErrorBar";
        ErrorBar.Padding = new Padding(10, 0, 10, 0);
        ErrorBar.Size = new Size(796, 36);
        ErrorBar.Text = "（エラーメッセージ）";
        ErrorBar.TextAlign = ContentAlignment.MiddleLeft;
        //
        // header
        //
        header.Controls.Add(titles);
        header.Controls.Add(actions);
        header.Dock = DockStyle.Top;
        header.Location = new Point(32, 24);
        header.Name = "header";
        header.Size = new Size(796, 72);
        header.TabIndex = 0;
        //
        // titles
        //
        titles.Controls.Add(titleLabel);
        titles.Controls.Add(subtitleLabel);
        titles.Dock = DockStyle.Fill;
        titles.FlowDirection = FlowDirection.TopDown;
        titles.Location = new Point(0, 0);
        titles.Name = "titles";
        titles.Size = new Size(456, 72);
        titles.TabIndex = 1;
        titles.WrapContents = false;
        //
        // titleLabel
        //
        titleLabel.AutoSize = true;
        titleLabel.Font = new Font("Yu Gothic UI", 18F, FontStyle.Bold);
        titleLabel.Margin = new Padding(0);
        titleLabel.Name = "titleLabel";
        titleLabel.Text = "ユーザー管理";
        //
        // subtitleLabel
        //
        subtitleLabel.AutoSize = true;
        subtitleLabel.ForeColor = Color.FromArgb(96, 96, 96);
        subtitleLabel.Margin = new Padding(0, 4, 0, 0);
        subtitleLabel.Name = "subtitleLabel";
        subtitleLabel.Text = "ログインできるユーザーを管理します。行をダブルクリックしても編集できます。";
        //
        // actions
        //
        actions.AutoSize = true;
        actions.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        actions.Controls.Add(RefreshButton);
        actions.Controls.Add(EditButton);
        actions.Controls.Add(NewUserButton);
        actions.Dock = DockStyle.Right;
        actions.Location = new Point(456, 0);
        actions.Name = "actions";
        actions.Size = new Size(340, 72);
        actions.TabIndex = 0;
        actions.WrapContents = false;
        //
        // RefreshButton
        //
        RefreshButton.AutoSize = true;
        RefreshButton.BackColor = Color.White;
        RefreshButton.Cursor = Cursors.Hand;
        RefreshButton.FlatAppearance.BorderColor = Color.FromArgb(220, 220, 220);
        RefreshButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(245, 245, 245);
        RefreshButton.FlatStyle = FlatStyle.Flat;
        RefreshButton.Margin = new Padding(8, 0, 0, 0);
        RefreshButton.MinimumSize = new Size(88, 34);
        RefreshButton.Name = "RefreshButton";
        RefreshButton.Padding = new Padding(10, 2, 10, 2);
        RefreshButton.TabIndex = 0;
        RefreshButton.Text = "⟳  更新";
        RefreshButton.UseVisualStyleBackColor = false;
        //
        // EditButton
        //
        EditButton.AutoSize = true;
        EditButton.BackColor = Color.White;
        EditButton.Cursor = Cursors.Hand;
        EditButton.FlatAppearance.BorderColor = Color.FromArgb(220, 220, 220);
        EditButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(245, 245, 245);
        EditButton.FlatStyle = FlatStyle.Flat;
        EditButton.Margin = new Padding(8, 0, 0, 0);
        EditButton.MinimumSize = new Size(88, 34);
        EditButton.Name = "EditButton";
        EditButton.Padding = new Padding(10, 2, 10, 2);
        EditButton.TabIndex = 1;
        EditButton.Text = "編集";
        EditButton.UseVisualStyleBackColor = false;
        //
        // NewUserButton
        //
        NewUserButton.AutoSize = true;
        NewUserButton.BackColor = Color.FromArgb(0, 103, 192);
        NewUserButton.Cursor = Cursors.Hand;
        NewUserButton.FlatAppearance.BorderColor = Color.FromArgb(0, 103, 192);
        NewUserButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 90, 168);
        NewUserButton.FlatStyle = FlatStyle.Flat;
        NewUserButton.ForeColor = Color.White;
        NewUserButton.Margin = new Padding(8, 0, 0, 0);
        NewUserButton.MinimumSize = new Size(88, 34);
        NewUserButton.Name = "NewUserButton";
        NewUserButton.Padding = new Padding(10, 2, 10, 2);
        NewUserButton.TabIndex = 2;
        NewUserButton.Text = "＋  ユーザーを登録";
        NewUserButton.UseVisualStyleBackColor = false;
        //
        // UsersView
        //
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        BackColor = Color.FromArgb(243, 243, 243);
        Controls.Add(page);
        ForeColor = Color.FromArgb(27, 27, 27);
        Name = "UsersView";
        Size = new Size(860, 600);
        page.ResumeLayout(false);
        page.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)UserList).EndInit();
        header.ResumeLayout(false);
        header.PerformLayout();
        titles.ResumeLayout(false);
        titles.PerformLayout();
        actions.ResumeLayout(false);
        actions.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private Panel page;
    private DataGridView UserList;
    private DataGridViewTextBoxColumn userNameColumn;
    private DataGridViewTextBoxColumn displayNameColumn;
    private DataGridViewTextBoxColumn authTypeColumn;
    private DataGridViewTextBoxColumn isAdminColumn;
    private DataGridViewTextBoxColumn statusColumn;
    private DataGridViewTextBoxColumn lastLoginColumn;
    private Panel errorSpacer;
    private Label ErrorBar;
    private Panel header;
    private FlowLayoutPanel titles;
    private Label titleLabel;
    private Label subtitleLabel;
    private FlowLayoutPanel actions;
    private Button RefreshButton;
    private Button EditButton;
    private Button NewUserButton;
}
