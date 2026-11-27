#nullable disable

namespace TaskDoc.WinFormsApp.Views;

partial class HomeView
{
    /// <summary>Required designer variable.</summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>Clean up any resources being used.</summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        sidebar = new Panel();
        nav = new FlowLayoutPanel();
        DashboardNav = new Button();
        adminHeading = new Label();
        UsersNav = new Button();
        accountHeading = new Label();
        ChangePasswordNav = new Button();
        footer = new Panel();
        SignOutButton = new Button();
        UserText = new Label();
        footerDivider = new Panel();
        header = new Panel();
        appNameLabel = new Label();
        SidebarToggle = new Button();
        sidebarDivider = new Panel();
        content = new Panel();
        toolTip = new ToolTip(components);
        sidebar.SuspendLayout();
        nav.SuspendLayout();
        footer.SuspendLayout();
        header.SuspendLayout();
        SuspendLayout();
        //
        // sidebar
        //
        sidebar.BackColor = Color.White;
        sidebar.Controls.Add(nav);
        sidebar.Controls.Add(footer);
        sidebar.Controls.Add(header);
        sidebar.Dock = DockStyle.Left;
        sidebar.Location = new Point(0, 0);
        sidebar.Name = "sidebar";
        sidebar.Padding = new Padding(8);
        sidebar.Size = new Size(230, 600);
        sidebar.TabIndex = 0;
        //
        // nav
        //
        nav.Controls.Add(DashboardNav);
        nav.Controls.Add(adminHeading);
        nav.Controls.Add(UsersNav);
        nav.Controls.Add(accountHeading);
        nav.Controls.Add(ChangePasswordNav);
        nav.Dock = DockStyle.Fill;
        nav.FlowDirection = FlowDirection.TopDown;
        nav.Location = new Point(8, 52);
        nav.Name = "nav";
        nav.Padding = new Padding(0, 4, 0, 0);
        nav.Size = new Size(214, 451);
        nav.TabIndex = 1;
        nav.WrapContents = false;
        //
        // DashboardNav
        //
        DashboardNav.AccessibleName = "ホーム";
        DashboardNav.BackColor = Color.White;
        DashboardNav.Cursor = Cursors.Hand;
        DashboardNav.FlatAppearance.BorderSize = 0;
        DashboardNav.FlatAppearance.MouseOverBackColor = Color.FromArgb(240, 240, 240);
        DashboardNav.FlatStyle = FlatStyle.Flat;
        DashboardNav.Margin = new Padding(0, 2, 0, 0);
        DashboardNav.Name = "DashboardNav";
        DashboardNav.Size = new Size(214, 40);
        DashboardNav.TabIndex = 0;
        DashboardNav.Text = "🏠   ホーム";
        DashboardNav.TextAlign = ContentAlignment.MiddleLeft;
        toolTip.SetToolTip(DashboardNav, "ホーム");
        DashboardNav.UseVisualStyleBackColor = false;
        //
        // adminHeading
        //
        adminHeading.AutoSize = true;
        adminHeading.Font = new Font("Yu Gothic UI", 8.75F);
        adminHeading.ForeColor = Color.FromArgb(96, 96, 96);
        adminHeading.Margin = new Padding(8, 14, 0, 4);
        adminHeading.Name = "adminHeading";
        adminHeading.Text = "管理";
        //
        // UsersNav
        //
        UsersNav.AccessibleName = "ユーザー管理";
        UsersNav.BackColor = Color.White;
        UsersNav.Cursor = Cursors.Hand;
        UsersNav.FlatAppearance.BorderSize = 0;
        UsersNav.FlatAppearance.MouseOverBackColor = Color.FromArgb(240, 240, 240);
        UsersNav.FlatStyle = FlatStyle.Flat;
        UsersNav.Margin = new Padding(0, 2, 0, 0);
        UsersNav.Name = "UsersNav";
        UsersNav.Size = new Size(214, 40);
        UsersNav.TabIndex = 1;
        UsersNav.Text = "👥   ユーザー管理";
        UsersNav.TextAlign = ContentAlignment.MiddleLeft;
        toolTip.SetToolTip(UsersNav, "ユーザー管理");
        UsersNav.UseVisualStyleBackColor = false;
        //
        // accountHeading
        //
        accountHeading.AutoSize = true;
        accountHeading.Font = new Font("Yu Gothic UI", 8.75F);
        accountHeading.ForeColor = Color.FromArgb(96, 96, 96);
        accountHeading.Margin = new Padding(8, 14, 0, 4);
        accountHeading.Name = "accountHeading";
        accountHeading.Text = "アカウント";
        //
        // ChangePasswordNav
        //
        ChangePasswordNav.AccessibleName = "パスワード変更";
        ChangePasswordNav.BackColor = Color.White;
        ChangePasswordNav.Cursor = Cursors.Hand;
        ChangePasswordNav.FlatAppearance.BorderSize = 0;
        ChangePasswordNav.FlatAppearance.MouseOverBackColor = Color.FromArgb(240, 240, 240);
        ChangePasswordNav.FlatStyle = FlatStyle.Flat;
        ChangePasswordNav.Margin = new Padding(0, 2, 0, 0);
        ChangePasswordNav.Name = "ChangePasswordNav";
        ChangePasswordNav.Size = new Size(214, 40);
        ChangePasswordNav.TabIndex = 2;
        ChangePasswordNav.Text = "🔑   パスワード変更";
        ChangePasswordNav.TextAlign = ContentAlignment.MiddleLeft;
        toolTip.SetToolTip(ChangePasswordNav, "パスワード変更");
        ChangePasswordNav.UseVisualStyleBackColor = false;
        //
        // footer
        //
        footer.Controls.Add(SignOutButton);
        footer.Controls.Add(UserText);
        footer.Controls.Add(footerDivider);
        footer.Dock = DockStyle.Bottom;
        footer.Location = new Point(8, 503);
        footer.Name = "footer";
        footer.Size = new Size(214, 89);
        footer.TabIndex = 2;
        //
        // SignOutButton
        //
        SignOutButton.AccessibleName = "ログアウト";
        SignOutButton.BackColor = Color.White;
        SignOutButton.Cursor = Cursors.Hand;
        SignOutButton.Dock = DockStyle.Top;
        SignOutButton.FlatAppearance.BorderSize = 0;
        SignOutButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(240, 240, 240);
        SignOutButton.FlatStyle = FlatStyle.Flat;
        SignOutButton.Location = new Point(0, 49);
        SignOutButton.Name = "SignOutButton";
        SignOutButton.Size = new Size(214, 40);
        SignOutButton.TabIndex = 0;
        SignOutButton.Text = "⇥   ログアウト";
        SignOutButton.TextAlign = ContentAlignment.MiddleLeft;
        toolTip.SetToolTip(SignOutButton, "ログアウト");
        SignOutButton.UseVisualStyleBackColor = false;
        //
        // UserText
        //
        UserText.AutoEllipsis = true;
        UserText.Dock = DockStyle.Top;
        UserText.Location = new Point(0, 1);
        UserText.Name = "UserText";
        UserText.Padding = new Padding(4, 0, 0, 0);
        UserText.Size = new Size(214, 48);
        UserText.Text = "表示名（管理者）\r\nユーザー名";
        UserText.TextAlign = ContentAlignment.MiddleLeft;
        //
        // footerDivider
        //
        footerDivider.BackColor = Color.FromArgb(220, 220, 220);
        footerDivider.Dock = DockStyle.Top;
        footerDivider.Location = new Point(0, 0);
        footerDivider.Name = "footerDivider";
        footerDivider.Size = new Size(214, 1);
        footerDivider.TabIndex = 1;
        //
        // header
        //
        header.Controls.Add(appNameLabel);
        header.Controls.Add(SidebarToggle);
        header.Dock = DockStyle.Top;
        header.Location = new Point(8, 8);
        header.Name = "header";
        header.Size = new Size(214, 44);
        header.TabIndex = 0;
        //
        // appNameLabel
        //
        appNameLabel.Dock = DockStyle.Fill;
        appNameLabel.Font = new Font("Yu Gothic UI", 13F, FontStyle.Bold);
        appNameLabel.Location = new Point(44, 0);
        appNameLabel.Name = "appNameLabel";
        appNameLabel.Padding = new Padding(6, 0, 0, 0);
        appNameLabel.Size = new Size(170, 44);
        appNameLabel.Text = "TaskDoc";
        appNameLabel.TextAlign = ContentAlignment.MiddleLeft;
        //
        // SidebarToggle
        //
        SidebarToggle.AccessibleName = "サイドバーの開閉";
        SidebarToggle.BackColor = Color.White;
        SidebarToggle.Cursor = Cursors.Hand;
        SidebarToggle.Dock = DockStyle.Left;
        SidebarToggle.FlatAppearance.BorderSize = 0;
        SidebarToggle.FlatAppearance.MouseOverBackColor = Color.FromArgb(240, 240, 240);
        SidebarToggle.FlatStyle = FlatStyle.Flat;
        SidebarToggle.Location = new Point(0, 0);
        SidebarToggle.Name = "SidebarToggle";
        SidebarToggle.Size = new Size(44, 44);
        SidebarToggle.TabIndex = 0;
        SidebarToggle.Text = "☰";
        toolTip.SetToolTip(SidebarToggle, "サイドバーの開閉");
        SidebarToggle.UseVisualStyleBackColor = false;
        //
        // sidebarDivider
        //
        sidebarDivider.BackColor = Color.FromArgb(220, 220, 220);
        sidebarDivider.Dock = DockStyle.Left;
        sidebarDivider.Location = new Point(230, 0);
        sidebarDivider.Name = "sidebarDivider";
        sidebarDivider.Size = new Size(1, 600);
        sidebarDivider.TabIndex = 1;
        //
        // content
        //
        content.Dock = DockStyle.Fill;
        content.Location = new Point(231, 0);
        content.Name = "content";
        content.Size = new Size(769, 600);
        content.TabIndex = 2;
        //
        // HomeView
        //
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        BackColor = Color.FromArgb(243, 243, 243);
        Controls.Add(content);
        Controls.Add(sidebarDivider);
        Controls.Add(sidebar);
        ForeColor = Color.FromArgb(27, 27, 27);
        Name = "HomeView";
        Size = new Size(1000, 600);
        sidebar.ResumeLayout(false);
        nav.ResumeLayout(false);
        nav.PerformLayout();
        footer.ResumeLayout(false);
        header.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Panel sidebar;
    private FlowLayoutPanel nav;
    private Button DashboardNav;
    private Label adminHeading;
    private Button UsersNav;
    private Label accountHeading;
    private Button ChangePasswordNav;
    private Panel footer;
    private Button SignOutButton;
    private Label UserText;
    private Panel footerDivider;
    private Panel header;
    private Label appNameLabel;
    private Button SidebarToggle;
    private Panel sidebarDivider;
    private Panel content;
    private ToolTip toolTip;
}
