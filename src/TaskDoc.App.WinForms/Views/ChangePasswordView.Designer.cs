#nullable disable

namespace TaskDoc.WinFormsApp.Views;

partial class ChangePasswordView
{
    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        pageLayout = new FlowLayoutPanel();
        titleLabel = new Label();
        subtitleLabel = new Label();
        card = new Panel();
        formLayout = new TableLayoutPanel();
        currentPasswordLabel = new Label();
        CurrentPasswordBox = new TextBox();
        newPasswordLabel = new Label();
        NewPasswordBox = new TextBox();
        newPasswordHint = new Label();
        confirmPasswordLabel = new Label();
        ConfirmPasswordBox = new TextBox();
        ErrorBar = new Label();
        ChangeButton = new Button();
        pageLayout.SuspendLayout();
        card.SuspendLayout();
        formLayout.SuspendLayout();
        SuspendLayout();
        //
        // pageLayout
        //
        pageLayout.AutoScroll = true;
        pageLayout.Controls.Add(titleLabel);
        pageLayout.Controls.Add(subtitleLabel);
        pageLayout.Controls.Add(card);
        pageLayout.Dock = DockStyle.Fill;
        pageLayout.FlowDirection = FlowDirection.TopDown;
        pageLayout.Location = new Point(0, 0);
        pageLayout.Name = "pageLayout";
        pageLayout.Padding = new Padding(32, 24, 32, 24);
        pageLayout.Size = new Size(800, 600);
        pageLayout.TabIndex = 0;
        pageLayout.WrapContents = false;
        //
        // titleLabel
        //
        titleLabel.AutoSize = true;
        titleLabel.Font = new Font("Yu Gothic UI", 18F, FontStyle.Bold);
        titleLabel.Margin = new Padding(0);
        titleLabel.Name = "titleLabel";
        titleLabel.Text = "パスワード変更";
        //
        // subtitleLabel
        //
        subtitleLabel.AutoSize = true;
        subtitleLabel.ForeColor = Color.FromArgb(96, 96, 96);
        subtitleLabel.Margin = new Padding(0, 4, 0, 0);
        subtitleLabel.Name = "subtitleLabel";
        subtitleLabel.Text = "変更すると、この端末以外ではログアウトされます。";
        //
        // card
        //
        card.AutoSize = true;
        card.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        card.BackColor = Color.White;
        card.BorderStyle = BorderStyle.FixedSingle;
        card.Controls.Add(formLayout);
        card.Margin = new Padding(0, 24, 0, 0);
        card.Name = "card";
        card.Padding = new Padding(20);
        card.TabIndex = 0;
        //
        // formLayout
        //
        formLayout.AutoSize = true;
        formLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        formLayout.ColumnCount = 1;
        formLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 360F));
        formLayout.Controls.Add(currentPasswordLabel, 0, 0);
        formLayout.Controls.Add(CurrentPasswordBox, 0, 1);
        formLayout.Controls.Add(newPasswordLabel, 0, 2);
        formLayout.Controls.Add(NewPasswordBox, 0, 3);
        formLayout.Controls.Add(newPasswordHint, 0, 4);
        formLayout.Controls.Add(confirmPasswordLabel, 0, 5);
        formLayout.Controls.Add(ConfirmPasswordBox, 0, 6);
        formLayout.Controls.Add(ErrorBar, 0, 7);
        formLayout.Controls.Add(ChangeButton, 0, 8);
        formLayout.Location = new Point(20, 20);
        formLayout.Name = "formLayout";
        formLayout.RowCount = 9;
        formLayout.RowStyles.Add(new RowStyle());
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
        // currentPasswordLabel
        //
        currentPasswordLabel.AutoSize = true;
        currentPasswordLabel.Font = new Font("Yu Gothic UI", 9.75F, FontStyle.Bold);
        currentPasswordLabel.Margin = new Padding(0, 0, 0, 4);
        currentPasswordLabel.Name = "currentPasswordLabel";
        currentPasswordLabel.Text = "現在のパスワード";
        //
        // CurrentPasswordBox
        //
        CurrentPasswordBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        CurrentPasswordBox.BorderStyle = BorderStyle.FixedSingle;
        CurrentPasswordBox.Font = new Font("Yu Gothic UI", 10.75F);
        CurrentPasswordBox.Margin = new Padding(0);
        CurrentPasswordBox.Name = "CurrentPasswordBox";
        CurrentPasswordBox.TabIndex = 0;
        CurrentPasswordBox.UseSystemPasswordChar = true;
        //
        // newPasswordLabel
        //
        newPasswordLabel.AutoSize = true;
        newPasswordLabel.Font = new Font("Yu Gothic UI", 9.75F, FontStyle.Bold);
        newPasswordLabel.Margin = new Padding(0, 12, 0, 4);
        newPasswordLabel.Name = "newPasswordLabel";
        newPasswordLabel.Text = "新しいパスワード";
        //
        // NewPasswordBox
        //
        NewPasswordBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        NewPasswordBox.BorderStyle = BorderStyle.FixedSingle;
        NewPasswordBox.Font = new Font("Yu Gothic UI", 10.75F);
        NewPasswordBox.Margin = new Padding(0);
        NewPasswordBox.Name = "NewPasswordBox";
        NewPasswordBox.TabIndex = 1;
        NewPasswordBox.UseSystemPasswordChar = true;
        //
        // newPasswordHint
        //
        newPasswordHint.AutoSize = true;
        newPasswordHint.Font = new Font("Yu Gothic UI", 8.75F);
        newPasswordHint.ForeColor = Color.FromArgb(96, 96, 96);
        newPasswordHint.Margin = new Padding(0, 2, 0, 0);
        newPasswordHint.Name = "newPasswordHint";
        newPasswordHint.Text = "8 文字以上";
        //
        // confirmPasswordLabel
        //
        confirmPasswordLabel.AutoSize = true;
        confirmPasswordLabel.Font = new Font("Yu Gothic UI", 9.75F, FontStyle.Bold);
        confirmPasswordLabel.Margin = new Padding(0, 12, 0, 4);
        confirmPasswordLabel.Name = "confirmPasswordLabel";
        confirmPasswordLabel.Text = "新しいパスワード（確認）";
        //
        // ConfirmPasswordBox
        //
        ConfirmPasswordBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        ConfirmPasswordBox.BorderStyle = BorderStyle.FixedSingle;
        ConfirmPasswordBox.Font = new Font("Yu Gothic UI", 10.75F);
        ConfirmPasswordBox.Margin = new Padding(0);
        ConfirmPasswordBox.Name = "ConfirmPasswordBox";
        ConfirmPasswordBox.TabIndex = 2;
        ConfirmPasswordBox.UseSystemPasswordChar = true;
        //
        // ErrorBar
        //
        ErrorBar.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        ErrorBar.AutoSize = true;
        ErrorBar.BackColor = Color.FromArgb(253, 231, 233);
        ErrorBar.ForeColor = Color.FromArgb(196, 43, 28);
        ErrorBar.Margin = new Padding(0, 12, 0, 0);
        ErrorBar.MaximumSize = new Size(360, 0);
        ErrorBar.Name = "ErrorBar";
        ErrorBar.Padding = new Padding(10, 8, 10, 8);
        ErrorBar.Text = "（エラーメッセージ）";
        //
        // ChangeButton
        //
        ChangeButton.Anchor = AnchorStyles.Right;
        ChangeButton.AutoSize = true;
        ChangeButton.BackColor = Color.FromArgb(0, 103, 192);
        ChangeButton.Cursor = Cursors.Hand;
        ChangeButton.FlatAppearance.BorderColor = Color.FromArgb(0, 103, 192);
        ChangeButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 90, 168);
        ChangeButton.FlatStyle = FlatStyle.Flat;
        ChangeButton.ForeColor = Color.White;
        ChangeButton.Margin = new Padding(0, 20, 0, 0);
        ChangeButton.MinimumSize = new Size(88, 34);
        ChangeButton.Name = "ChangeButton";
        ChangeButton.Padding = new Padding(10, 2, 10, 2);
        ChangeButton.TabIndex = 3;
        ChangeButton.Text = "パスワードを変更";
        ChangeButton.UseVisualStyleBackColor = false;
        //
        // ChangePasswordView
        //
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        BackColor = Color.FromArgb(243, 243, 243);
        Controls.Add(pageLayout);
        ForeColor = Color.FromArgb(27, 27, 27);
        Name = "ChangePasswordView";
        Size = new Size(800, 600);
        formLayout.ResumeLayout(false);
        formLayout.PerformLayout();
        card.ResumeLayout(false);
        card.PerformLayout();
        pageLayout.ResumeLayout(false);
        pageLayout.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private FlowLayoutPanel pageLayout;
    private Label titleLabel;
    private Label subtitleLabel;
    private Panel card;
    private TableLayoutPanel formLayout;
    private Label currentPasswordLabel;
    private TextBox CurrentPasswordBox;
    private Label newPasswordLabel;
    private TextBox NewPasswordBox;
    private Label newPasswordHint;
    private Label confirmPasswordLabel;
    private TextBox ConfirmPasswordBox;
    private Label ErrorBar;
    private Button ChangeButton;
}
