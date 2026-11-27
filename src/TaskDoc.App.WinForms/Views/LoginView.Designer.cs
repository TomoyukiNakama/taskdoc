#nullable disable

namespace TaskDoc.WinFormsApp.Views;

partial class LoginView
{
    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        centerLayout = new TableLayoutPanel();
        card = new Panel();
        formLayout = new TableLayoutPanel();
        titleLabel = new Label();
        subtitleLabel = new Label();
        userNameLabel = new Label();
        UserNameBox = new TextBox();
        passwordLabel = new Label();
        PasswordBox = new TextBox();
        ErrorBar = new Label();
        LoginButton = new Button();
        centerLayout.SuspendLayout();
        card.SuspendLayout();
        formLayout.SuspendLayout();
        SuspendLayout();
        //
        // centerLayout
        //
        centerLayout.AutoScroll = true;
        centerLayout.ColumnCount = 3;
        centerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        centerLayout.ColumnStyles.Add(new ColumnStyle());
        centerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        centerLayout.Controls.Add(card, 1, 1);
        centerLayout.Dock = DockStyle.Fill;
        centerLayout.Location = new Point(0, 0);
        centerLayout.Name = "centerLayout";
        centerLayout.RowCount = 3;
        centerLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        centerLayout.RowStyles.Add(new RowStyle());
        centerLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        centerLayout.Size = new Size(800, 600);
        centerLayout.TabIndex = 0;
        //
        // card
        //
        card.Anchor = AnchorStyles.None;
        card.AutoSize = true;
        card.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        card.BackColor = Color.White;
        card.BorderStyle = BorderStyle.FixedSingle;
        card.Controls.Add(formLayout);
        card.Name = "card";
        card.Padding = new Padding(28);
        card.TabIndex = 0;
        //
        // formLayout
        //
        formLayout.AutoSize = true;
        formLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        formLayout.ColumnCount = 1;
        formLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 340F));
        formLayout.Controls.Add(titleLabel, 0, 0);
        formLayout.Controls.Add(subtitleLabel, 0, 1);
        formLayout.Controls.Add(userNameLabel, 0, 2);
        formLayout.Controls.Add(UserNameBox, 0, 3);
        formLayout.Controls.Add(passwordLabel, 0, 4);
        formLayout.Controls.Add(PasswordBox, 0, 5);
        formLayout.Controls.Add(ErrorBar, 0, 6);
        formLayout.Controls.Add(LoginButton, 0, 7);
        formLayout.Location = new Point(28, 28);
        formLayout.Name = "formLayout";
        formLayout.RowCount = 8;
        formLayout.RowStyles.Add(new RowStyle());
        formLayout.RowStyles.Add(new RowStyle());
        formLayout.RowStyles.Add(new RowStyle());
        formLayout.RowStyles.Add(new RowStyle());
        formLayout.RowStyles.Add(new RowStyle());
        formLayout.RowStyles.Add(new RowStyle());
        formLayout.RowStyles.Add(new RowStyle());
        formLayout.RowStyles.Add(new RowStyle());
        formLayout.TabIndex = 0;
        //
        // titleLabel
        //
        titleLabel.AutoSize = true;
        titleLabel.Font = new Font("Yu Gothic UI", 14F, FontStyle.Bold);
        titleLabel.ForeColor = Color.FromArgb(27, 27, 27);
        titleLabel.Margin = new Padding(0);
        titleLabel.Name = "titleLabel";
        titleLabel.Text = "TaskDoc にログイン";
        //
        // subtitleLabel
        //
        subtitleLabel.AutoSize = true;
        subtitleLabel.ForeColor = Color.FromArgb(96, 96, 96);
        subtitleLabel.Margin = new Padding(0, 6, 0, 8);
        subtitleLabel.MaximumSize = new Size(340, 0);
        subtitleLabel.Name = "subtitleLabel";
        subtitleLabel.Text = "AD アカウント、またはシステムに登録されたユーザーでログインしてください。";
        //
        // userNameLabel
        //
        userNameLabel.AutoSize = true;
        userNameLabel.Font = new Font("Yu Gothic UI", 9.75F, FontStyle.Bold);
        userNameLabel.Margin = new Padding(0, 12, 0, 4);
        userNameLabel.Name = "userNameLabel";
        userNameLabel.Text = "ユーザー名";
        //
        // UserNameBox
        //
        UserNameBox.AccessibleName = "ユーザー名";
        UserNameBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        UserNameBox.BorderStyle = BorderStyle.FixedSingle;
        UserNameBox.Font = new Font("Yu Gothic UI", 10.75F);
        UserNameBox.Margin = new Padding(0);
        UserNameBox.Name = "UserNameBox";
        UserNameBox.PlaceholderText = "例: yamada";
        UserNameBox.TabIndex = 0;
        //
        // passwordLabel
        //
        passwordLabel.AutoSize = true;
        passwordLabel.Font = new Font("Yu Gothic UI", 9.75F, FontStyle.Bold);
        passwordLabel.Margin = new Padding(0, 12, 0, 4);
        passwordLabel.Name = "passwordLabel";
        passwordLabel.Text = "パスワード";
        //
        // PasswordBox
        //
        PasswordBox.AccessibleName = "パスワード";
        PasswordBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        PasswordBox.BorderStyle = BorderStyle.FixedSingle;
        PasswordBox.Font = new Font("Yu Gothic UI", 10.75F);
        PasswordBox.Margin = new Padding(0);
        PasswordBox.Name = "PasswordBox";
        PasswordBox.TabIndex = 1;
        PasswordBox.UseSystemPasswordChar = true;
        //
        // ErrorBar
        //
        ErrorBar.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        ErrorBar.AutoSize = true;
        ErrorBar.BackColor = Color.FromArgb(253, 231, 233);
        ErrorBar.ForeColor = Color.FromArgb(196, 43, 28);
        ErrorBar.Margin = new Padding(0, 12, 0, 0);
        ErrorBar.MaximumSize = new Size(340, 0);
        ErrorBar.Name = "ErrorBar";
        ErrorBar.Padding = new Padding(10, 8, 10, 8);
        ErrorBar.Text = "（エラーメッセージ）";
        //
        // LoginButton
        //
        LoginButton.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        LoginButton.BackColor = Color.FromArgb(0, 103, 192);
        LoginButton.Cursor = Cursors.Hand;
        LoginButton.FlatAppearance.BorderColor = Color.FromArgb(0, 103, 192);
        LoginButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 90, 168);
        LoginButton.FlatStyle = FlatStyle.Flat;
        LoginButton.ForeColor = Color.White;
        LoginButton.Margin = new Padding(0, 20, 0, 0);
        LoginButton.Name = "LoginButton";
        LoginButton.Size = new Size(340, 34);
        LoginButton.TabIndex = 2;
        LoginButton.Text = "ログイン";
        LoginButton.UseVisualStyleBackColor = false;
        //
        // LoginView
        //
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        BackColor = Color.FromArgb(243, 243, 243);
        Controls.Add(centerLayout);
        ForeColor = Color.FromArgb(27, 27, 27);
        Name = "LoginView";
        Size = new Size(800, 600);
        formLayout.ResumeLayout(false);
        formLayout.PerformLayout();
        card.ResumeLayout(false);
        card.PerformLayout();
        centerLayout.ResumeLayout(false);
        centerLayout.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private TableLayoutPanel centerLayout;
    private Panel card;
    private TableLayoutPanel formLayout;
    private Label titleLabel;
    private Label subtitleLabel;
    private Label userNameLabel;
    private TextBox UserNameBox;
    private Label passwordLabel;
    private TextBox PasswordBox;
    private Label ErrorBar;
    private Button LoginButton;
}
