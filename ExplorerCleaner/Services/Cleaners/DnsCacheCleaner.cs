using System.Diagnostics;
using ExplorerCleaner.Models;

namespace ExplorerCleaner.Services.Cleaners;

public class DnsCacheCleaner : ICleaner
{
    public string Name => "DNS 缓存";
    public bool RequiresExplorerRestart => false;

    public async Task<CleanResult> CleanAsync()
    {
        try
        {
            var psi = new ProcessStartInfo("ipconfig", "/flushdns")
            {
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            if (process == null)
            {
                return new CleanResult
                {
                    CleanerName = Name,
                    Status = CleanResultStatus.Failed,
                    Message = "无法启动 ipconfig"
                };
            }

            await process.WaitForExitAsync();

            return new CleanResult
            {
                CleanerName = Name,
                Status = process.ExitCode == 0 ? CleanResultStatus.Success : CleanResultStatus.Failed,
                Message = process.ExitCode == 0 ? "DNS 缓存已刷新" : "刷新失败"
            };
        }
        catch (Exception ex)
        {
            return new CleanResult
            {
                CleanerName = Name,
                Status = CleanResultStatus.Failed,
                Message = ex.Message
            };
        }
    }
}
