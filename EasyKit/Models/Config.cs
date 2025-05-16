using System.Text.Json;

namespace EasyKit.Models;

public class Config
{
    private const string AppName = "EasyKit";
    private const string AppAuthor = "LoveDoLove";
    private const string AppVersion = "4.0.2";
    private readonly string _configFilePath;

    public Config()
    {
        _configFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppName,
            "config.json");
        // Initialize Settings before LoadConfig to avoid null reference issues
        Settings = new Dictionary<string, object>();
        LoadConfig();
    }

    public Dictionary<string, object> Settings { get; private set; }

    private void LoadConfig()
    {
        // Default configuration
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
            {
                "context_menu_registry_paths", new[]
                {
                    @"Software\Classes\*\shell\EasyKit",
                    @"Software\Classes\Directory\shell\EasyKit",
                    @"Software\Classes\Directory\Background\shell\EasyKit"
                }
            },
            { "context_menu_scope", "user" } // "user" for HKCU, "system" for HKCR
        };

        // Load from config file if exists
        if (File.Exists(_configFilePath))
            try
            {
                var json = File.ReadAllText(_configFilePath);
                var userConfig = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
                if (userConfig != null)
                    foreach (var kv in userConfig)
                        Settings[kv.Key] = kv.Value;
                // --- Version auto-update logic ---
                if (!Settings.TryGetValue("version", out var userVersion) || userVersion == null || userVersion.ToString() != AppVersion)
                {
                    Settings["version"] = AppVersion;
                    SaveConfig();
                }
                // --- End version auto-update logic ---
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading config file: {ex.Message}");
            }
        else
            SaveConfig();
    }

    public void SaveConfig()
    {
        string? dirPath = Path.GetDirectoryName(_configFilePath);
        if (!string.IsNullOrEmpty(dirPath)) Directory.CreateDirectory(dirPath);
        var json = JsonSerializer.Serialize(Settings, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_configFilePath, json);
    }

    public object? Get(string key, object? defaultValue = null)
    {
        return Settings.ContainsKey(key) ? Settings[key] : defaultValue;
    }

    /// <summary>
    ///     Sets a configuration value and saves the configuration to disk.
    /// </summary>
    /// <param name="key">The configuration key to set</param>
    /// <param name="value">The value to store</param>
    public void Set(string key, object value)
    {
        Settings[key] = value;
        SaveConfig();
    }

    private string GetLogPath()
    {
        var logDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppName,
            "logs");
        Directory.CreateDirectory(logDir);
        return Path.Combine(logDir, "easykit.log");
    }

    /// <summary>
    ///     Resets all settings to their default values and saves the config.
    /// </summary>
    public void ResetToDefaults()
    {
        // Default configuration
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
            {
                "context_menu_registry_paths", new[]
                {
                    @"Software\Classes\*\shell\EasyKit",
                    @"Software\Classes\Directory\shell\EasyKit",
                    @"Software\Classes\Directory\Background\shell\EasyKit"
                }
            },
            { "context_menu_scope", "user" } // "user" for HKCU, "system" for HKCR
        };
        SaveConfig();
    }
}