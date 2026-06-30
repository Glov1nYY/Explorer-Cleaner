namespace ExplorerCleaner.Forms;

public partial class ProgressForm : Form
{
    private Label _statusLabel = null!;
    private ProgressBar _progressBar = null!;

    public ProgressForm()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.Text = "正在清理...";
        this.Size = new Size(360, 120);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.ControlBox = false;
        this.BackColor = Color.White;
        this.ShowInTaskbar = false;

        _statusLabel = new Label
        {
            Text = "正在准备...",
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Microsoft YaHei UI", 10F),
            ForeColor = Color.FromArgb(30, 41, 59),
            Location = new Point(20, 20),
            Size = new Size(304, 24)
        };

        _progressBar = new ProgressBar
        {
            Style = ProgressBarStyle.Marquee,
            MarqueeAnimationSpeed = 30,
            Location = new Point(20, 55),
            Size = new Size(304, 20)
        };

        this.Controls.AddRange([_statusLabel, _progressBar]);
    }

    public void UpdateStatus(string message)
    {
        if (this.InvokeRequired)
        {
            this.Invoke(() => _statusLabel.Text = message);
        }
        else
        {
            _statusLabel.Text = message;
        }
    }

    public void MarkComplete(string message, int delayMs = 1500)
    {
        if (this.InvokeRequired)
        {
            this.Invoke(() => MarkComplete(message, delayMs));
            return;
        }

        _progressBar.Style = ProgressBarStyle.Blocks;
        _progressBar.Value = 100;
        _statusLabel.Text = message;
        _statusLabel.ForeColor = Color.FromArgb(34, 197, 94);

        var timer = new System.Windows.Forms.Timer { Interval = delayMs };
        timer.Tick += (_, _) =>
        {
            timer.Stop();
            this.Close();
        };
        timer.Start();
    }
}
