#nullable disable

namespace TaskDoc.WinFormsApp.Views;

partial class UserEditorView
{
    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        formLayout = new TableLayoutPanel();
        TitleLabel = new Label();
        fixedNote = new Label();
        authTypeLabel = new Label();
        authTypeRow = new FlowLayoutPanel();
        LocalRadio = new RadioButton();
        AdRadio = new RadioButton();
        userNameLabel = new Label();
        UserNameBox = new TextBox();
        userNameHint = new Label();
        displayNameLabel = new Label();
        DisplayNameBox = new TextBox();
        emailLabel = new Label();
        EmailBox = new TextBox();
        passwordLabel = new Label();
        PasswordBox = new TextBox();
        passwordHint = new Label();
        flagsRow = new FlowLayoutPanel();
        IsAdminCheck = new CheckBox();
        IsActiveCheck = new CheckBox();
        selfHint = new Label();
        ErrorBar = new Label();
        buttonsRow = new FlowLayoutPanel();
        CancelButton = new Button();
        SaveButton = new Button();
        resetSection = new TableLayoutPanel();
        resetDivider = new Panel();
        resetTitle = new Label();
        resetHint = new Label();
        resetRow = new Panel();
        NewPasswordBox = new TextBox();
        resetGap = new Panel();
        ResetPasswordButton = new Button();
        ResetBar = new Label();
        formLayout.SuspendLayout();
        authTypeRow.SuspendLayout();
        flagsRow.SuspendLayout();
        buttonsRow.SuspendLayout();
        resetSection.SuspendLayout();
        resetRow.SuspendLayout();
        SuspendLayout();
        //
        // formLayout
        //
        formLayout.AutoSize = true;
        formLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        formLayout.ColumnCount = 1;
        formLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 440F));
        formLayout.Controls.Add(TitleLabel, 0, 0);
        formLayout.Controls.Add(fixedNote, 0, 1);
        formLayout.Controls.Add(authTypeLabel, 0, 2);
        formLayout.Controls.Add(authTypeRow, 0, 3);
        formLayout.Controls.Add(userNameLabel, 0, 4);
        formLayout.Controls.Add(UserNameBox, 0, 5);
        formLayout.Controls.Add(userNameHint, 0, 6);
        formLayout.Controls.Add(displayNameLabel, 0, 7);
        formLayout.Controls.Add(DisplayNameBox, 0, 8);
        formLayout.Controls.Add(emailLabel, 0, 9);
        formLayout.Controls.Add(EmailBox, 0, 10);
        formLayout.Controls.Add(passwordLabel, 0, 11);
        formLayout.Controls.Add(PasswordBox, 0, 12);
        formLayout.Controls.Add(passwordHint, 0, 13);
        formLayout.Controls.Add(flagsRow, 0, 14);
        formLayout.Controls.Add(selfHint, 0, 15);
        formLayout.Controls.Add(ErrorBar, 0, 16);
        formLayout.Controls.Add(buttonsRow, 0, 17);
        formLayout.Controls.Add(resetSection, 0, 18);
        formLayout.Location = new Point(24, 20);
        formLayout.Name = "formLayout";
        formLayout.RowCount = 19;
        formLayout.RowStyles.Add(new RowStyle());
        formLayout.RowStyles.Add(new RowStyle());
        formLayout.RowStyles.Add(new RowStyle());
        formLayout.RowStyles.Add(new RowStyle());
        formLayout.RowStyles.Add(new RowStyle());
        formLayout.RowStyles.Add(new RowStyle());
        formLayout.RowStyles.Add(new RowStyle());
        formLayout.RowStyles.Add(new RowStyle());
        formLayout.RowStyles.Add(new RowStyle());
        formLayout.RowStyles.Add(new RowStyle());
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
        // TitleLabel
        //
        TitleLabel.AutoSize = true;
        TitleLabel.Font = new Font("Yu Gothic UI", 14F, FontStyle.Bold);
        TitleLabel.Margin = new Padding(0);
        TitleLabel.Name = "TitleLabel";
        TitleLabel.Text = "ユーザーを登録";
        //
        // fixedNote
        //
        fixedNote.AutoSize = true;
        fixedNote.ForeColor = Color.FromArgb(96, 96, 96);
        fixedNote.Margin = new Padding(0, 4, 0, 0);
        fixedNote.Name = "fixedNote";
        fixedNote.Text = "種類は登録後に変更できません。";
        //
        // authTypeLabel
        //
        authTypeLabel.AutoSize = true;
        authTypeLabel.Font = new Font("Yu Gothic UI", 9.75F, FontStyle.Bold);
        authTypeLabel.Margin = new Padding(0, 12, 0, 4);
        authTypeLabel.Name = "authTypeLabel";
        authTypeLabel.Text = "種類";
        //
        // authTypeRow
        //
        authTypeRow.AutoSize = true;
        authTypeRow.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        authTypeRow.Controls.Add(LocalRadio);
        authTypeRow.Controls.Add(AdRadio);
        authTypeRow.Margin = new Padding(0);
        authTypeRow.Name = "authTypeRow";
        authTypeRow.TabIndex = 0;
        authTypeRow.WrapContents = false;
        //
        // LocalRadio
        //
        LocalRadio.AutoSize = true;
        LocalRadio.Margin = new Padding(0, 0, 20, 0);
        LocalRadio.Name = "LocalRadio";
        LocalRadio.TabIndex = 0;
        LocalRadio.Text = "システムユーザー";
        //
        // AdRadio
        //
        AdRadio.AutoSize = true;
        AdRadio.Margin = new Padding(0);
        AdRadio.Name = "AdRadio";
        AdRadio.TabIndex = 1;
        AdRadio.Text = "AD ユーザー";
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
        UserNameBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        UserNameBox.BorderStyle = BorderStyle.FixedSingle;
        UserNameBox.Font = new Font("Yu Gothic UI", 10.75F);
        UserNameBox.Margin = new Padding(0);
        UserNameBox.Name = "UserNameBox";
        UserNameBox.TabIndex = 1;
        //
        // userNameHint
        //
        userNameHint.AutoSize = true;
        userNameHint.Font = new Font("Yu Gothic UI", 8.75F);
        userNameHint.ForeColor = Color.FromArgb(96, 96, 96);
        userNameHint.Margin = new Padding(0, 2, 0, 0);
        userNameHint.MaximumSize = new Size(440, 0);
        userNameHint.Name = "userNameHint";
        userNameHint.Text = "半角英数字と . _ -（AD ユーザーは AD のアカウント名）";
        //
        // displayNameLabel
        //
        displayNameLabel.AutoSize = true;
        displayNameLabel.Font = new Font("Yu Gothic UI", 9.75F, FontStyle.Bold);
        displayNameLabel.Margin = new Padding(0, 12, 0, 4);
        displayNameLabel.Name = "displayNameLabel";
        displayNameLabel.Text = "表示名";
        //
        // DisplayNameBox
        //
        DisplayNameBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        DisplayNameBox.BorderStyle = BorderStyle.FixedSingle;
        DisplayNameBox.Font = new Font("Yu Gothic UI", 10.75F);
        DisplayNameBox.Margin = new Padding(0);
        DisplayNameBox.Name = "DisplayNameBox";
        DisplayNameBox.PlaceholderText = "例: 山田 太郎";
        DisplayNameBox.TabIndex = 2;
        //
        // emailLabel
        //
        emailLabel.AutoSize = true;
        emailLabel.Font = new Font("Yu Gothic UI", 9.75F, FontStyle.Bold);
        emailLabel.Margin = new Padding(0, 12, 0, 4);
        emailLabel.Name = "emailLabel";
        emailLabel.Text = "メールアドレス（任意）";
        //
        // EmailBox
        //
        EmailBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        EmailBox.BorderStyle = BorderStyle.FixedSingle;
        EmailBox.Font = new Font("Yu Gothic UI", 10.75F);
        EmailBox.Margin = new Padding(0);
        EmailBox.Name = "EmailBox";
        EmailBox.TabIndex = 3;
        //
        // passwordLabel
        //
        passwordLabel.AutoSize = true;
        passwordLabel.Font = new Font("Yu Gothic UI", 9.75F, FontStyle.Bold);
        passwordLabel.Margin = new Padding(0, 12, 0, 4);
        passwordLabel.Name = "passwordLabel";
        passwordLabel.Text = "初期パスワード";
        //
        // PasswordBox
        //
        PasswordBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        PasswordBox.BorderStyle = BorderStyle.FixedSingle;
        PasswordBox.Font = new Font("Yu Gothic UI", 10.75F);
        PasswordBox.Margin = new Padding(0);
        PasswordBox.Name = "PasswordBox";
        PasswordBox.TabIndex = 4;
        PasswordBox.UseSystemPasswordChar = true;
        //
        // passwordHint
        //
        passwordHint.AutoSize = true;
        passwordHint.Font = new Font("Yu Gothic UI", 8.75F);
        passwordHint.ForeColor = Color.FromArgb(96, 96, 96);
        passwordHint.Margin = new Padding(0, 2, 0, 0);
        passwordHint.Name = "passwordHint";
        passwordHint.Text = "8 文字以上";
        //
        // flagsRow
        //
        flagsRow.AutoSize = true;
        flagsRow.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        flagsRow.Controls.Add(IsAdminCheck);
        flagsRow.Controls.Add(IsActiveCheck);
        flagsRow.Margin = new Padding(0, 12, 0, 0);
        flagsRow.Name = "flagsRow";
        flagsRow.TabIndex = 5;
        flagsRow.WrapContents = false;
        //
        // IsAdminCheck
        //
        IsAdminCheck.AutoSize = true;
        IsAdminCheck.Margin = new Padding(0, 0, 24, 0);
        IsAdminCheck.Name = "IsAdminCheck";
        IsAdminCheck.TabIndex = 0;
        IsAdminCheck.Text = "システム管理者";
        //
        // IsActiveCheck
        //
        IsActiveCheck.AutoSize = true;
        IsActiveCheck.Margin = new Padding(0);
        IsActiveCheck.Name = "IsActiveCheck";
        IsActiveCheck.TabIndex = 1;
        IsActiveCheck.Text = "有効（ログインできる）";
        //
        // selfHint
        //
        selfHint.AutoSize = true;
        selfHint.Font = new Font("Yu Gothic UI", 8.75F);
        selfHint.ForeColor = Color.FromArgb(96, 96, 96);
        selfHint.Margin = new Padding(0, 2, 0, 0);
        selfHint.MaximumSize = new Size(440, 0);
        selfHint.Name = "selfHint";
        selfHint.Text = "自分自身の管理者権限と有効/無効は、ほかの管理者が変更してください。";
        //
        // ErrorBar
        //
        ErrorBar.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        ErrorBar.AutoSize = true;
        ErrorBar.BackColor = Color.FromArgb(253, 231, 233);
        ErrorBar.ForeColor = Color.FromArgb(196, 43, 28);
        ErrorBar.Margin = new Padding(0, 12, 0, 0);
        ErrorBar.MaximumSize = new Size(440, 0);
        ErrorBar.Name = "ErrorBar";
        ErrorBar.Padding = new Padding(10, 8, 10, 8);
        ErrorBar.Text = "（エラーメッセージ）";
        //
        // buttonsRow
        //
        buttonsRow.Anchor = AnchorStyles.Right;
        buttonsRow.AutoSize = true;
        buttonsRow.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        buttonsRow.Controls.Add(CancelButton);
        buttonsRow.Controls.Add(SaveButton);
        buttonsRow.Margin = new Padding(0, 16, 0, 0);
        buttonsRow.Name = "buttonsRow";
        buttonsRow.TabIndex = 6;
        buttonsRow.WrapContents = false;
        //
        // CancelButton
        //
        CancelButton.AutoSize = true;
        CancelButton.BackColor = Color.White;
        CancelButton.Cursor = Cursors.Hand;
        CancelButton.FlatAppearance.BorderColor = Color.FromArgb(220, 220, 220);
        CancelButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(245, 245, 245);
        CancelButton.FlatStyle = FlatStyle.Flat;
        CancelButton.Margin = new Padding(8, 0, 0, 0);
        CancelButton.MinimumSize = new Size(88, 34);
        CancelButton.Name = "CancelButton";
        CancelButton.Padding = new Padding(10, 2, 10, 2);
        CancelButton.TabIndex = 0;
        CancelButton.Text = "キャンセル";
        CancelButton.UseVisualStyleBackColor = false;
        //
        // SaveButton
        //
        SaveButton.AutoSize = true;
        SaveButton.BackColor = Color.FromArgb(0, 103, 192);
        SaveButton.Cursor = Cursors.Hand;
        SaveButton.FlatAppearance.BorderColor = Color.FromArgb(0, 103, 192);
        SaveButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 90, 168);
        SaveButton.FlatStyle = FlatStyle.Flat;
        SaveButton.ForeColor = Color.White;
        SaveButton.Margin = new Padding(8, 0, 0, 0);
        SaveButton.MinimumSize = new Size(88, 34);
        SaveButton.Name = "SaveButton";
        SaveButton.Padding = new Padding(10, 2, 10, 2);
        SaveButton.TabIndex = 1;
        SaveButton.Text = "保存";
        SaveButton.UseVisualStyleBackColor = false;
        //
        // resetSection
        //
        resetSection.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        resetSection.AutoSize = true;
        resetSection.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        resetSection.ColumnCount = 1;
        resetSection.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 440F));
        resetSection.Controls.Add(resetDivider, 0, 0);
        resetSection.Controls.Add(resetTitle, 0, 1);
        resetSection.Controls.Add(resetHint, 0, 2);
        resetSection.Controls.Add(resetRow, 0, 3);
        resetSection.Controls.Add(ResetBar, 0, 4);
        resetSection.Margin = new Padding(0);
        resetSection.Name = "resetSection";
        resetSection.RowCount = 5;
        resetSection.RowStyles.Add(new RowStyle());
        resetSection.RowStyles.Add(new RowStyle());
        resetSection.RowStyles.Add(new RowStyle());
        resetSection.RowStyles.Add(new RowStyle());
        resetSection.RowStyles.Add(new RowStyle());
        resetSection.TabIndex = 7;
        //
        // resetDivider
        //
        resetDivider.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        resetDivider.BackColor = Color.FromArgb(220, 220, 220);
        resetDivider.Margin = new Padding(0, 16, 0, 12);
        resetDivider.Name = "resetDivider";
        resetDivider.Size = new Size(440, 1);
        resetDivider.TabIndex = 0;
        //
        // resetTitle
        //
        resetTitle.AutoSize = true;
        resetTitle.Font = new Font("Yu Gothic UI", 9.75F, FontStyle.Bold);
        resetTitle.Margin = new Padding(0);
        resetTitle.Name = "resetTitle";
        resetTitle.Text = "パスワードの再設定";
        //
        // resetHint
        //
        resetHint.AutoSize = true;
        resetHint.Font = new Font("Yu Gothic UI", 8.75F);
        resetHint.ForeColor = Color.FromArgb(96, 96, 96);
        resetHint.Margin = new Padding(0, 2, 0, 0);
        resetHint.MaximumSize = new Size(440, 0);
        resetHint.Name = "resetHint";
        resetHint.Text = "新しいパスワードを設定すると、このユーザーはすべての端末でログアウトされ、ロックも解除されます。";
        //
        // resetRow
        //
        resetRow.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        resetRow.Controls.Add(NewPasswordBox);
        resetRow.Controls.Add(resetGap);
        resetRow.Controls.Add(ResetPasswordButton);
        resetRow.Margin = new Padding(0, 8, 0, 0);
        resetRow.Name = "resetRow";
        resetRow.Size = new Size(440, 34);
        resetRow.TabIndex = 1;
        //
        // NewPasswordBox
        //
        NewPasswordBox.BorderStyle = BorderStyle.FixedSingle;
        NewPasswordBox.Dock = DockStyle.Fill;
        NewPasswordBox.Font = new Font("Yu Gothic UI", 10.75F);
        NewPasswordBox.Name = "NewPasswordBox";
        NewPasswordBox.PlaceholderText = "新しいパスワード（8 文字以上）";
        NewPasswordBox.TabIndex = 0;
        NewPasswordBox.UseSystemPasswordChar = true;
        //
        // resetGap
        //
        resetGap.Dock = DockStyle.Right;
        resetGap.Name = "resetGap";
        resetGap.Size = new Size(8, 34);
        resetGap.TabIndex = 2;
        //
        // ResetPasswordButton
        //
        ResetPasswordButton.BackColor = Color.FromArgb(196, 43, 28);
        ResetPasswordButton.Cursor = Cursors.Hand;
        ResetPasswordButton.Dock = DockStyle.Right;
        ResetPasswordButton.FlatAppearance.BorderColor = Color.FromArgb(196, 43, 28);
        ResetPasswordButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(170, 36, 24);
        ResetPasswordButton.FlatStyle = FlatStyle.Flat;
        ResetPasswordButton.ForeColor = Color.White;
        ResetPasswordButton.Name = "ResetPasswordButton";
        ResetPasswordButton.Size = new Size(88, 34);
        ResetPasswordButton.TabIndex = 1;
        ResetPasswordButton.Text = "再設定";
        ResetPasswordButton.UseVisualStyleBackColor = false;
        //
        // ResetBar
        //
        ResetBar.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        ResetBar.AutoSize = true;
        ResetBar.BackColor = Color.FromArgb(223, 246, 221);
        ResetBar.ForeColor = Color.FromArgb(15, 123, 15);
        ResetBar.Margin = new Padding(0, 8, 0, 0);
        ResetBar.MaximumSize = new Size(440, 0);
        ResetBar.Name = "ResetBar";
        ResetBar.Padding = new Padding(10, 8, 10, 8);
        ResetBar.Text = "（完了メッセージ）";
        //
        // UserEditorView
        //
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        AutoSize = true;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;
        BackColor = Color.White;
        Controls.Add(formLayout);
        ForeColor = Color.FromArgb(27, 27, 27);
        Name = "UserEditorView";
        Padding = new Padding(24, 20, 24, 20);
        formLayout.ResumeLayout(false);
        formLayout.PerformLayout();
        authTypeRow.ResumeLayout(false);
        authTypeRow.PerformLayout();
        flagsRow.ResumeLayout(false);
        flagsRow.PerformLayout();
        buttonsRow.ResumeLayout(false);
        buttonsRow.PerformLayout();
        resetSection.ResumeLayout(false);
        resetSection.PerformLayout();
        resetRow.ResumeLayout(false);
        resetRow.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private TableLayoutPanel formLayout;
    private Label TitleLabel;
    private Label fixedNote;
    private Label authTypeLabel;
    private FlowLayoutPanel authTypeRow;
    private RadioButton LocalRadio;
    private RadioButton AdRadio;
    private Label userNameLabel;
    private TextBox UserNameBox;
    private Label userNameHint;
    private Label displayNameLabel;
    private TextBox DisplayNameBox;
    private Label emailLabel;
    private TextBox EmailBox;
    private Label passwordLabel;
    private TextBox PasswordBox;
    private Label passwordHint;
    private FlowLayoutPanel flagsRow;
    private CheckBox IsAdminCheck;
    private CheckBox IsActiveCheck;
    private Label selfHint;
    private Label ErrorBar;
    private FlowLayoutPanel buttonsRow;
    private Button CancelButton;
    private Button SaveButton;
    private TableLayoutPanel resetSection;
    private Panel resetDivider;
    private Label resetTitle;
    private Label resetHint;
    private Panel resetRow;
    private TextBox NewPasswordBox;
    private Panel resetGap;
    private Button ResetPasswordButton;
    private Label ResetBar;
}
