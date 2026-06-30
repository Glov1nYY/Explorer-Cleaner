using ExplorerCleaner.Models;
using ExplorerCleaner.Services;

namespace ExplorerCleaner.Forms;

public partial class SettingsForm : Form
{
    private readonly Settings _settings;
    private readonly CleanerService _cleanerService;
    private readonly StartupManager _startupManager;

    private readonly Dictionary<string, CheckBox> _checkBoxes = new();
    private ComboBox _intervalCombo = null!;
    private CheckBox _autoStartCheck = null!;
    private Button _cleanNowBtn = null!;
    private Button _saveBtn = null!;

    public SettingsForm(Settings settings, CleanerService cleanerService, StartupManager startupManager)
    {
        _settings = settings;
        _cleanerService = cleanerService;
        _startupManager = startupManager;
        InitializeComponent();
        LoadSettings();
    }

    private void InitializeComponent()
    {
        this.Text = "Explorer Cleaner";
        this.ClientSize = new Size(420, 540);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.BackColor = Color.White;
        this.Font = new Font("Microsoft YaHei UI", 9F);

        var titleLabel = new Label
        {
            Text = "Explorer Cleaner",
            Font = new Font("Microsoft YaHei UI", 14F, FontStyle.Bold),
            ForeColor = Color.FromArgb(30, 41, 59),
            Location = new Point(30, 20),
            AutoSize = true
        };

        var subtitleLabel = new Label
        {
            Text = "选择需要清理的项目",
            Font = new Font("Microsoft YaHei UI", 9F),
            ForeColor = Color.FromArgb(100, 116, 139),
            Location = new Point(30, 52),
            AutoSize = true
        };

        // 清理项分组面板
        var groupPanel = new Panel
        {
            Location = new Point(30, 82),
            Size = new Size(344, 260),
            BackColor = Color.FromArgb(248, 250, 252),
            Padding = new Padding(16)
        };

        int y = 16;
        foreach (var key in _cleanerService.AllCleanerKeys)
        {
            var cb = new CheckBox
            {
                Text = _cleanerService.GetCleanerName(key),
                Location = new Point(16, y),
                AutoSize = true,
                Font = new Font("Microsoft YaHei UI", 10F),
                ForeColor = Color.FromArgb(30, 41, 59),
                Tag = key
            };
            _checkBoxes[key] = cb;
            groupPanel.Controls.Add(cb);
            y += 36;
        }

        // 间隔标签
        var intervalLabel = new Label
        {
            Text = "自动清理间隔",
            Font = new Font("Microsoft YaHei UI", 9F),
            ForeColor = Color.FromArgb(100, 116, 139),
            Location = new Point(30, 368),
            AutoSize = true
        };

        _intervalCombo = new ComboBox
        {
            Location = new Point(150, 364),
            Size = new Size(120, 28),
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font = new Font("Microsoft YaHei UI", 9F)
        };
        _intervalCombo.Items.AddRange(new object[] { "15 分钟", "30 分钟", "1 小时", "2 小时", "从不" });

        _autoStartCheck = new CheckBox
        {
            Text = "开机自启动",
            Location = new Point(30, 410),
            AutoSize = true,
            Font = new Font("Microsoft YaHei UI", 10F),
            ForeColor = Color.FromArgb(30, 41, 59)
        };

        _cleanNowBtn = new Button
        {
            Text = "立即清理",
            Location = new Point(140, 454),
            Size = new Size(110, 32),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(37, 99, 235),
            ForeColor = Color.White,
            Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        _cleanNowBtn.FlatAppearance.BorderSize = 0;
        _cleanNowBtn.Click += async (_, _) => await RunCleanAsync();

        _saveBtn = new Button
        {
            Text = "保存",
            Location = new Point(264, 454),
            Size = new Size(110, 32),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(241, 245, 249),
            ForeColor = Color.FromArgb(30, 41, 59),
            Font = new Font("Microsoft YaHei UI", 9F),
            Cursor = Cursors.Hand
        };
        _saveBtn.FlatAppearance.BorderSize = 0;
        _saveBtn.Click += (_, _) => SaveSettings();

        this.Controls.AddRange(new Control[]
        {
            titleLabel, subtitleLabel, groupPanel,
            intervalLabel, _intervalCombo, _autoStartCheck,
            _cleanNowBtn, _saveBtn
        });
    }

    private void LoadSettings()
    {
        foreach (var key in _settings.EnabledCleaners)
        {
            if (_checkBoxes.TryGetValue(key, out var cb))
                cb.Checked = true;
        }

        _intervalCombo.SelectedIndex = _settings.AutoCleanIntervalMinutes switch
        {
            15 => 0,
            30 => 1,
            60 => 2,
            120 => 3,
            _ => 4
        };

        _autoStartCheck.Checked = _startupManager.IsEnabled;
    }

    private void SaveSettings()
    {
        _settings.EnabledCleaners = _checkBoxes
            .Where(kv => kv.Value.Checked)
            .Select(kv => kv.Key)
            .ToList();

        _settings.AutoCleanIntervalMinutes = _intervalCombo.SelectedIndex switch
        {
            0 => 15,
            1 => 30,
            2 => 60,
            3 => 120,
            _ => 0
        };

        _settings.AutoStart = _autoStartCheck.Checked;
        _settings.Save();

        _startupManager.SetEnabled(_autoStartCheck.Checked);

        MessageBox.Show("设置已保存", "Explorer Cleaner",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private async Task RunCleanAsync()
    {
        var enabled = _checkBoxes
            .Where(kv => kv.Value.Checked)
            .Select(kv => kv.Key)
            .ToList();

        if (enabled.Count == 0)
        {
            MessageBox.Show("请至少选择一项清理内容", "提示",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var progressForm = new ProgressForm();
        var progress = new Progress<string>(msg => progressForm.UpdateStatus(msg));
        progressForm.Show(this);

        var results = await _cleanerService.RunAsync(enabled, progress);

        int success = results.Count(r => r.Status == CleanResultStatus.Success);
        int failed = results.Count(r => r.Status == CleanResultStatus.Failed);
        int skipped = results.Count(r => r.Status == CleanResultStatus.Skipped);

        var summary = $"成功 {success} 项";
        if (failed > 0) summary += $", 失败 {failed} 项";
        if (skipped > 0) summary += $", 跳过 {skipped} 项";

        progressForm.MarkComplete($"清理完成 - {summary}");
    }
}
