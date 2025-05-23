using System.Text.Json;

namespace CommonUtilities.Models;

/// <summary>
/// Application configuration model with load/save and default management.
/// </summary>
public class Config
{
    private const string AppName = "EasyKit";
    private const string AppAuthor = "LoveDoLove";
    private const string AppVersion = "4.0.2";
    private readonly string _configFilePath;

    public Config()
    {
        _configFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppName, "config.json");
        Settings = new Dictionary<string, object>();
        LoadConfig();
    }

    /// <summary>
    /// In-memory settings dictionary.
    /// </summary>
    public Dictionary<string, object> Settings { get; private set; }

    /// <summary>
    /// Loads configuration from disk or initializes defaults.
    /// </summary>
    private void LoadConfig()
    {
        Settings = new Dictionary<string, object>
        {
            { "color_scheme", "dark" },
            { "enable_logging", true },
            { "log_path", GetLogPath() },
            { "show_tips", true },
            { "confirm_exit", true },
            { "confirm_destructive_actions", true },
            { "menu_width", 50 },
            { "version", AppVersion },
            { "context_menu_name", "EasyKit" },
            { "context_menu_registry_paths", new[]
                {
                    @"Software\Classes\*\shell\EasyKit",
                    @"Software\Classes\Directory\shell\EasyKit",
                    @"Software\Classes\Directory\Background\shell\EasyKit"
                }
            },
            { "context_menu_scope", "user" }
        };

        if (File.Exists(_configFilePath))
        {
            try
            {
                var json = File.ReadAllText(_configFilePath);
                var userConfig = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
                if (userConfig != null)
                {
                    foreach (var kv in userConfig)
                        Settings[kv.Key] = kv.Value;
                }
                if (!Settings.TryGetValue("version", out var userVersion) || userVersion == null || userVersion.ToString() != AppVersion)
                {
                    Settings["version"] = AppVersion;
                    SaveConfig();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading config file: {ex.Message}");
            }
        }
        else
        {
            SaveConfig();
        }
    }

    /// <summary>
    /// Saves configuration to disk.
    /// </summary>
    public void SaveConfig()
    {
        string? dirPath = Path.GetDirectoryName(_configFilePath);
        if (!string.IsNullOrEmpty(dirPath)) Directory.CreateDirectory(dirPath);
        var json = JsonSerializer.Serialize(Settings, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_configFilePath, json);
    }

    /// <summary>
    /// Gets a configuration value by key.
    /// </summary>
    public object? Get(string key, object? defaultValue = null)
    {
        return Settings.ContainsKey(key) ? Settings[key] : defaultValue;
    }

    /// <summary>
    /// Sets a configuration value and saves to disk.
    /// </summary>
    public void Set(string key, object value)
    {
        Settings[key] = value;
        SaveConfig();
    }

    private string GetLogPath()
    {
        var logDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppName, "logs");
        Directory.CreateDirectory(logDir);
        return Path.Combine(logDir, "easykit.log");
    }

    /// <summary>
    /// Resets all settings to their default values and saves the config.
    /// </summary>
    public void ResetToDefaults()
    {
        Settings = new Dictionary<string, object>
        {
            { "color_scheme", "dark" },
            { "enable_logging", true },
            { "log_path", GetLogPath() },
            { "show_tips", true },
            { "confirm_exit", true },
            { "confirm_destructive_actions", true },
            { "menu_width", 50 },
            { "version", AppVersion },
            { "context_menu_name", "EasyKit" },
            { "context_menu_registry_paths", new[]
                {
                    @"Software\Classes\*\shell\EasyKit",
                    @"Software\Classes\Directory\shell\EasyKit",
                    @"Software\Classes\Directory\Background\shell\EasyKit"
                }
            },
            { "context_menu_scope", "user" }
        };
        SaveConfig();
    }
}