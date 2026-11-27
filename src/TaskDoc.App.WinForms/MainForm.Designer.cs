#nullable disable

namespace TaskDoc.WinFormsApp;

partial class MainForm
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
        content = new Panel();
        toast = new Label();
        toastTimer = new System.Windows.Forms.Timer(components);
        SuspendLayout();
        //
        // content
        //
        content.Dock = DockStyle.Fill;
        content.Location = new Point(0, 0);
        content.Name = "content";
        content.Size = new Size(1100, 680);
        content.TabIndex = 0;
        //
        // toast
        //
        toast.BackColor = Color.FromArgb(223, 246, 221);
        toast.Cursor = Cursors.Hand;
        toast.Dock = DockStyle.Bottom;
        toast.ForeColor = Color.FromArgb(15, 123, 15);
        toast.Location = new Point(0, 680);
        toast.Name = "toast";
        toast.Padding = new Padding(16, 0, 16, 0);
        toast.Size = new Size(1100, 40);
        toast.Text = "✓  （通知）";
        toast.TextAlign = ContentAlignment.MiddleLeft;
        toast.Visible = false;
        toast.Click += OnToastClick;
        //
        // toastTimer
        //
        toastTimer.Interval = 4000;
        toastTimer.Tick += OnToastTimerTick;
        //
        // MainForm
        //
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        BackColor = Color.FromArgb(243, 243, 243);
        ClientSize = new Size(1100, 720);
        Controls.Add(content);
        Controls.Add(toast);
        ForeColor = Color.FromArgb(27, 27, 27);
        MinimumSize = new Size(640, 500);
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "TaskDoc";
        ResumeLayout(false);
    }

    #endregion

    private Panel content;
    private Label toast;
    private System.Windows.Forms.Timer toastTimer;
}
