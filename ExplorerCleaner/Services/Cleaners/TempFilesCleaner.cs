using ExplorerCleaner.Models;

namespace ExplorerCleaner.Services.Cleaners;

public class TempFilesCleaner : ICleaner
{
    public string Name => "临时文件";
    public bool RequiresExplorerRestart => false;

    public Task<CleanResult> CleanAsync()
    {
        try
        {
            int deletedFiles = 0;
            int deletedDirs = 0;

            var tempPaths = new[]
            {
                Path.GetTempPath(),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Temp")
            };

            foreach (var tempPath in tempPaths)
            {
                if (!Directory.Exists(tempPath)) continue;

                try
                {
                    foreach (var f in Directory.GetFiles(tempPath))
                    {
                        try { File.Delete(f); deletedFiles++; }
                        catch { /* 文件被占用则跳过 */ }
                    }
                    foreach (var d in Directory.GetDirectories(tempPath))
                    {
                        try { Directory.Delete(d, recursive: true); deletedDirs++; }
                        catch { /* 目录被占用则跳过 */ }
                    }
                }
                catch { }
            }

            return Task.FromResult(new CleanResult
            {
                CleanerName = Name,
                Status = (deletedFiles + deletedDirs) > 0 ? CleanResultStatus.Success : CleanResultStatus.Skipped,
                Message = (deletedFiles + deletedDirs) > 0 ? $"清理了 {deletedFiles} 个文件, {deletedDirs} 个文件夹" : "无需清理"
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
