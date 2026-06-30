using ExplorerCleaner.Models;

namespace ExplorerCleaner.Services.Cleaners;

public class ThumbnailCacheCleaner : ICleaner
{
    public string Name => "缩略图缓存";
    public bool RequiresExplorerRestart => true;

    public Task<CleanResult> CleanAsync()
    {
        try
        {
            var explorerDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                @"Microsoft\Windows\Explorer");

            int deleted = 0;
            if (Directory.Exists(explorerDir))
            {
                foreach (var f in Directory.GetFiles(explorerDir, "thumbcache_*.db"))
                {
                    File.Delete(f);
                    deleted++;
                }
            }

            return Task.FromResult(new CleanResult
            {
                CleanerName = Name,
                Status = deleted > 0 ? CleanResultStatus.Success : CleanResultStatus.Skipped,
                Message = deleted > 0 ? $"清理了 {deleted} 个缓存文件" : "无需清理"
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
