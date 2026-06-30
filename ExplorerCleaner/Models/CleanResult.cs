namespace ExplorerCleaner.Models;

public enum CleanResultStatus
{
    Success,
    Skipped,
    Failed
}

public class CleanResult
{
    public string CleanerName { get; init; } = "";
    public CleanResultStatus Status { get; init; }
    public string? Message { get; init; }
}
