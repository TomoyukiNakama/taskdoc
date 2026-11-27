#nullable disable

namespace TaskDoc.WinFormsApp.Views;

partial class DashboardView
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
        cardLayout = new TableLayoutPanel();
        cardTitleLabel = new Label();
        cardSubtitleLabel = new Label();
        ConnectionStatusText = new Label();
        CheckConnectionButton = new Button();
        pageLayout.SuspendLayout();
        card.SuspendLayout();
        cardLayout.SuspendLayout();
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
        titleLabel.Text = "ホーム";
        //
        // subtitleLabel
        //
        subtitleLabel.AutoSize = true;
        subtitleLabel.ForeColor = Color.FromArgb(96, 96, 96);
        subtitleLabel.Margin = new Padding(0, 4, 0, 0);
        subtitleLabel.Name = "subtitleLabel";
        subtitleLabel.Text = "タスクとドキュメントの画面はこれから作成します。";
        //
        // card
        //
        card.AutoSize = true;
        card.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        card.BackColor = Color.White;
        card.BorderStyle = BorderStyle.FixedSingle;
        card.Controls.Add(cardLayout);
        card.Margin = new Padding(0, 24, 0, 0);
        card.Name = "card";
        card.Padding = new Padding(20);
        card.TabIndex = 0;
        //
        // cardLayout
        //
        cardLayout.AutoSize = true;
        cardLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        cardLayout.ColumnCount = 1;
        cardLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 360F));
        cardLayout.Controls.Add(cardTitleLabel, 0, 0);
        cardLayout.Controls.Add(cardSubtitleLabel, 0, 1);
        cardLayout.Controls.Add(ConnectionStatusText, 0, 2);
        cardLayout.Controls.Add(CheckConnectionButton, 0, 3);
        cardLayout.Location = new Point(20, 20);
        cardLayout.Name = "cardLayout";
        cardLayout.RowCount = 4;
        cardLayout.RowStyles.Add(new RowStyle());
        cardLayout.RowStyles.Add(new RowStyle());
        cardLayout.RowStyles.Add(new RowStyle());
        cardLayout.RowStyles.Add(new RowStyle());
        cardLayout.TabIndex = 0;
        //
        // cardTitleLabel
        //
        cardTitleLabel.AutoSize = true;
        cardTitleLabel.Font = new Font("Yu Gothic UI", 12F, FontStyle.Bold);
        cardTitleLabel.Margin = new Padding(0);
        cardTitleLabel.Name = "cardTitleLabel";
        cardTitleLabel.Text = "API 接続";
        //
        // cardSubtitleLabel
        //
        cardSubtitleLabel.AutoSize = true;
        cardSubtitleLabel.ForeColor = Color.FromArgb(96, 96, 96);
        cardSubtitleLabel.Margin = new Padding(0, 4, 0, 12);
        cardSubtitleLabel.MaximumSize = new Size(360, 0);
        cardSubtitleLabel.Name = "cardSubtitleLabel";
        cardSubtitleLabel.Text = "サーバーとデータベースに接続できるか確認します。";
        //
        // ConnectionStatusText
        //
        ConnectionStatusText.AutoSize = true;
        ConnectionStatusText.Margin = new Padding(0);
        ConnectionStatusText.MaximumSize = new Size(360, 0);
        ConnectionStatusText.Name = "ConnectionStatusText";
        ConnectionStatusText.Text = "未確認";
        //
        // CheckConnectionButton
        //
        CheckConnectionButton.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        CheckConnectionButton.BackColor = Color.White;
        CheckConnectionButton.Cursor = Cursors.Hand;
        CheckConnectionButton.FlatAppearance.BorderColor = Color.FromArgb(220, 220, 220);
        CheckConnectionButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(245, 245, 245);
        CheckConnectionButton.FlatStyle = FlatStyle.Flat;
        CheckConnectionButton.Margin = new Padding(0, 12, 0, 0);
        CheckConnectionButton.Name = "CheckConnectionButton";
        CheckConnectionButton.Size = new Size(360, 34);
        CheckConnectionButton.TabIndex = 0;
        CheckConnectionButton.Text = "接続を確認";
        CheckConnectionButton.UseVisualStyleBackColor = false;
        //
        // DashboardView
        //
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        BackColor = Color.FromArgb(243, 243, 243);
        Controls.Add(pageLayout);
        ForeColor = Color.FromArgb(27, 27, 27);
        Name = "DashboardView";
        Size = new Size(800, 600);
        cardLayout.ResumeLayout(false);
        cardLayout.PerformLayout();
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
    private TableLayoutPanel cardLayout;
    private Label cardTitleLabel;
    private Label cardSubtitleLabel;
    private Label ConnectionStatusText;
    private Button CheckConnectionButton;
}
