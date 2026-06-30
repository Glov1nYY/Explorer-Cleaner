using ExplorerCleaner.Models;

namespace ExplorerCleaner.Services.Cleaners;

public interface ICleaner
{
    string Name { get; }
    bool RequiresExplorerRestart { get; }
    Task<CleanResult> CleanAsync();
}
