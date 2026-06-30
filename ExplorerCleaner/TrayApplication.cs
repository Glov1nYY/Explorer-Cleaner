using ExplorerCleaner.Forms;
using ExplorerCleaner.Models;
using ExplorerCleaner.Services;

namespace ExplorerCleaner;

public class TrayApplication : ApplicationContext
{
    private readonly NotifyIcon _trayIcon;
    private readonly Settings _settings;
    private readonly CleanerService _cleanerService;
    private readonly StartupManager _startupManager;
    private System.Timers.Timer? _autoTimer;
    private SettingsForm? _settingsForm;

    public TrayApplication()
    {
        _settings = Settings.Load();
        _cleanerService = new CleanerService();
        _startupManager = new StartupManager();

        _trayIcon = new NotifyIcon
        {
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath)!,
            Text = "Explorer Cleaner",
            Visible = true,
            ContextMenuStrip = BuildContextMenu()
        };
        _trayIcon.DoubleClick += async (_, _) => await RunCleanAsync();

        SetupAutoTimer();
    }

    private ContextMenuStrip BuildContextMenu()
    {
        var menu = new ContextMenuStrip();

        var cleanItem = new ToolStripMenuItem("立即清理");
        cleanItem.Click += async (_, _) => await RunCleanAsync();

        var settingsItem = new ToolStripMenuItem("设置...");
        settingsItem.Click += (_, _) => ShowSettings();

        var exitItem = new ToolStripMenuItem("退出");
        exitItem.Click += (_, _) => Exit();

        menu.Items.AddRange(new ToolStripItem[] { cleanItem, settingsItem, new ToolStripSeparator(), exitItem });
        return menu;
    }

    private void SetupAutoTimer()
    {
        if (_settings.AutoCleanIntervalMinutes <= 0) return;

        _autoTimer = new System.Timers.Timer(_settings.AutoCleanIntervalMinutes * 60 * 1000);
        _autoTimer.AutoReset = true;
        _autoTimer.Elapsed += async (_, _) => await RunCleanAsync();
        _autoTimer.Start();
    }

    private async Task RunCleanAsync()
    {
        var enabled = _settings.EnabledCleaners;
        if (enabled.Count == 0) return;

        var results = await _cleanerService.RunAsync(enabled);

        int success = results.Count(r => r.Status == CleanResultStatus.Success);
        int failed = results.Count(r => r.Status == CleanResultStatus.Failed);

        _trayIcon.ShowBalloonTip(3000, "Explorer Cleaner",
            $"清理完成：成功 {success} 项" + (failed > 0 ? $", 失败 {failed} 项" : ""),
            failed > 0 ? ToolTipIcon.Warning : ToolTipIcon.Info);
    }

    private void ShowSettings()
    {
        if (_settingsForm == null || _settingsForm.IsDisposed)
        {
            _settingsForm = new SettingsForm(_settings, _cleanerService, _startupManager);
            _settingsForm.FormClosed += (_, _) =>
            {
                _settingsForm = null;
                RestartAutoTimer();
            };
        }
        _settingsForm.Show();
        _settingsForm.Activate();
    }

    private void RestartAutoTimer()
    {
        _autoTimer?.Stop();
        _autoTimer?.Dispose();
        _autoTimer = null;
        SetupAutoTimer();
    }

    private void Exit()
    {
        _trayIcon.Visible = false;
        _trayIcon.Dispose();
        _autoTimer?.Stop();
        _autoTimer?.Dispose();
        Application.Exit();
    }
}
