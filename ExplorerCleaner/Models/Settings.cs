using System.Text.Json;

namespace ExplorerCleaner.Models;

public class Settings
{
    private static readonly string AppDataPath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ExplorerCleaner");
    private static readonly string SettingsFilePath = Path.Combine(AppDataPath, "settings.json");

    public List<string> EnabledCleaners { get; set; } = new()
    {
        "QuickAccess",
        "ThumbnailCache",
        "IconCache",
        "ExplorerHistory"
    };

    public int AutoCleanIntervalMinutes { get; set; } = 30;
    public bool AutoStart { get; set; } = false;

    public static Settings Load()
    {
        try
        {
            if (File.Exists(SettingsFilePath))
            {
                var json = File.ReadAllText(SettingsFilePath);
                return JsonSerializer.Deserialize<Settings>(json) ?? new Settings();
            }
        }
        catch { }
        return new Settings();
    }

    public void Save()
    {
        try
        {
            Directory.CreateDirectory(AppDataPath);
            var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(SettingsFilePath, json);
        }
        catch { }
    }
}
