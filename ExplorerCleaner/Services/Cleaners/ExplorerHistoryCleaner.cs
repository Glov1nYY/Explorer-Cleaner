using ExplorerCleaner.Models;
using Microsoft.Win32;

namespace ExplorerCleaner.Services.Cleaners;

public class ExplorerHistoryCleaner : ICleaner
{
    public string Name => "资源管理器历史";
    public bool RequiresExplorerRestart => false;

    public Task<CleanResult> CleanAsync()
    {
        try
        {
            int deleted = 0;

            using var typedPaths = Registry.CurrentUser.OpenSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\Explorer\TypedPaths", writable: true);
            if (typedPaths != null)
            {
                var names = typedPaths.GetValueNames();
                foreach (var name in names)
                {
                    typedPaths.DeleteValue(name);
                    deleted++;
                }
            }

            using var runMru = Registry.CurrentUser.OpenSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\Explorer\RunMRU", writable: true);
            if (runMru != null)
            {
                var names = runMru.GetValueNames();
                foreach (var name in names)
                {
                    runMru.DeleteValue(name);
                    deleted++;
                }
            }

            return Task.FromResult(new CleanResult
            {
                CleanerName = Name,
                Status = deleted > 0 ? CleanResultStatus.Success : CleanResultStatus.Skipped,
                Message = deleted > 0 ? $"清理了 {deleted} 条历史记录" : "无需清理"
            });
        }
        catch (Exception ex)
        {
            return Task.FromResult(new CleanResult
            {
                CleanerName = Name,
                Status = CleanResultStatus.Failed,
                Message = ex.Message
            });
        }
    }
}
