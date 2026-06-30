using System.Diagnostics;
using ExplorerCleaner.Models;
using ExplorerCleaner.Services.Cleaners;

namespace ExplorerCleaner.Services;

public class CleanerService
{
    private readonly Dictionary<string, ICleaner> _allCleaners = new()
    {
        ["QuickAccess"] = new QuickAccessCleaner(),
        ["ThumbnailCache"] = new ThumbnailCacheCleaner(),
        ["IconCache"] = new IconCacheCleaner(),
        ["ExplorerHistory"] = new ExplorerHistoryCleaner(),
        ["DnsCache"] = new DnsCacheCleaner(),
        ["TempFiles"] = new TempFilesCleaner()
    };

    public IEnumerable<string> AllCleanerKeys => _allCleaners.Keys;
    public string GetCleanerName(string key) => _allCleaners.TryGetValue(key, out var c) ? c.Name : key;

    public async Task<List<CleanResult>> RunAsync(List<string> enabledKeys, IProgress<string>? progress = null)
    {
        var results = new List<CleanResult>();
        var keysToRun = enabledKeys.Where(k => _allCleaners.ContainsKey(k)).ToList();

        bool needsRestart = keysToRun.Any(k =>
            _allCleaners.TryGetValue(k, out var c) && c.RequiresExplorerRestart);

        if (needsRestart)
        {
            progress?.Report("正在停止资源管理器...");
            KillExplorer();
            await Task.Delay(500);
        }

        foreach (var key in keysToRun)
        {
            if (!_allCleaners.TryGetValue(key, out var cleaner)) continue;

            progress?.Report($"正在清理：{cleaner.Name}");
            var result = await cleaner.CleanAsync();
            results.Add(result);
        }

        if (needsRestart)
        {
            progress?.Report("正在重启资源管理器...");
            StartExplorer();
        }

        return results;
    }

    private static void KillExplorer()
    {
        try
        {
            foreach (var proc in Process.GetProcessesByName("explorer"))
            {
                proc.Kill();
            }
        }
        catch { }
    }

    private static void StartExplorer()
    {
        try
        {
            Process.Start("explorer.exe");
        }
        catch { }
    }
}
