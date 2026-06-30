using ExplorerCleaner.Models;

namespace ExplorerCleaner.Services.Cleaners;

public class QuickAccessCleaner : ICleaner
{
    public string Name => "快速访问历史";
    public bool RequiresExplorerRestart => false;

    public Task<CleanResult> CleanAsync()
    {
        try
        {
            var recent = Environment.GetFolderPath(Environment.SpecialFolder.Recent);
            var autoDest = Path.Combine(recent, "AutomaticDestinations");
            var customDest = Path.Combine(recent, "CustomDestinations");

            int deleted = 0;
            if (Directory.Exists(autoDest))
            {
                foreach (var f in Directory.GetFiles(autoDest)) { File.Delete(f); deleted++; }
            }
            if (Directory.Exists(customDest))
            {
                foreach (var f in Directory.GetFiles(customDest)) { File.Delete(f); deleted++; }
            }

            return Task.FromResult(new CleanResult
            {
                CleanerName = Name,
                Status = deleted > 0 ? CleanResultStatus.Success : CleanResultStatus.Skipped,
                Message = deleted > 0 ? $"清理了 {deleted} 个文件" : "无需清理"
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
