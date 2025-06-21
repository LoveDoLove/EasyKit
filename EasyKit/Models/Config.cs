// MIT License
// 
// Copyright (c) 2025 LoveDoLove
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System.Text.Json;

namespace EasyKit.Models;

/// <summary>
///     Application configuration model with load/save and default management.
/// </summary>
public class Config
{
    private readonly string _appName;
    private readonly string _appVersion;
    private readonly string _configFilePath;

    /// <summary>
    ///     Initializes a new instance of the Config class.
    /// </summary>
    /// <param name="appName">Application name for config folder.</param>
    /// <param name="appVersion">Application version for config management.</param>
    /// <param name="configFilePath">Optional: Full path to config file. If null, uses default location.</param>
    public Config(string appName, string appVersion, string? configFilePath = null)
    {
        _appName = appName;
        _appVersion = appVersion;
        _configFilePath = configFilePath ??
                          Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), appName,
                              "config.json");
        Settings = new Dictionary<string, object>();
        LoadConfig();
    }

    /// <summary>
    ///     In-memory settings dictionary.
    /// </summary>
    public Dictionary<string, object> Settings { get; private set; }

    /// <summary>
    ///     Loads configuration from disk or initializes defaults.
    /// </summary>
    private void LoadConfig()
    {
        Settings = new Dictionary<string, object>
        {
            { "enable_logging", true },
            { "log_path", GetLogPath() },
            { "show_tips", true },
            { "menu_width", 100 },
            { "version", _appVersion }
        };

        if (File.Exists(_configFilePath))
            try
            {
                var json = File.ReadAllText(_configFilePath);
                var userConfig = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

                // If userConfig is successfully deserialized, merge it with default settings.
                // User's settings will override defaults.
                if (userConfig != null)
                    foreach (var kv in userConfig)
                        Settings[kv.Key] = kv.Value;

                // Check if the version in the loaded config matches the current app version.
                // If not, or if version is missing, update it and save the config.
                // This helps in managing config changes across app updates.
                if (!Settings.TryGetValue("version", out var userVersionObj) ||
                    userVersionObj == null ||
                    userVersionObj.ToString() != _appVersion)
                {
                    Settings["version"] = _appVersion; // Update to current application version
                    SaveConfig(); // Save immediately to reflect version update
                }
            }
            catch (JsonException ex) // Catch specific JSON parsing errors
            {
                // Log or handle JSON format errors, potentially reset to defaults or notify user.
                Console.WriteLine(
                    $"Error deserializing config file '{_configFilePath}': {ex.Message}. Using default settings.");
                // Optionally, could reset to defaults here if config is corrupted:
                // ResetToDefaults(); // This would overwrite potentially recoverable user settings.
            }
            catch (Exception ex) // Catch other general IO errors or unexpected issues
            {
                // Log or handle other errors during file reading or processing.
                Console.WriteLine(
                    $"Error loading config file '{_configFilePath}': {ex.Message}. Using default settings.");
            }
        else
            // Config file does not exist, so save the default configuration.
            SaveConfig();
    }

    /// <summary>
    ///     Saves the current configuration settings to the JSON file on disk.
    ///     Ensures the directory exists before writing.
    /// </summary>
    public void SaveConfig()
    {
        try
        {
            string? dirPath = Path.GetDirectoryName(_configFilePath);
            if (!string.IsNullOrEmpty(dirPath))
                Directory.CreateDirectory(dirPath); // Ensure the configuration directory exists
            var json = JsonSerializer.Serialize(Settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_configFilePath, json);
        }
        catch (Exception ex)
        {
            // Log or handle errors during file saving.
            Console.WriteLine($"Error saving config file '{_configFilePath}': {ex.Message}");
            // Depending on the application, might want to throw or notify the user.
        }
    }

    /// <summary>
    ///     Gets a configuration value by its key.
    /// </summary>
    /// <param name="key">The key of the configuration value to retrieve.</param>
    /// <param name="defaultValue">The default value to return if the key is not found. Defaults to null.</param>
    /// <returns>The configuration value if the key exists; otherwise, the <paramref name="defaultValue" />.</returns>
    public object? Get(string key, object? defaultValue = null)
    {
        return Settings.TryGetValue(key, out var value) ? value : defaultValue;
    }

    /// <summary>
    ///     Sets a configuration value for the specified key and immediately saves the configuration to disk.
    /// </summary>
    /// <param name="key">The key of the configuration value to set.</param>
    /// <param name="value">The value to set for the configuration key.</param>
    public void Set(string key, object value)
    {
        Settings[key] = value;
        SaveConfig();
    }

    /// <summary>
    ///     Generates and returns the default path for the application's log file.
    ///     It ensures the log directory exists.
    ///     The path is typically in the ApplicationData folder, under a subfolder named after the application, then 'logs'.
    ///     Example: C:\Users\[User]\AppData\Roaming\[AppName]\logs\[appname].log
    /// </summary>
    /// <returns>The full path to the log file.</returns>
    private string GetLogPath()
    {
        // Construct the log directory path within the user's ApplicationData folder.
        var logDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), _appName,
            "logs");
        // Ensure the log directory exists.
        Directory.CreateDirectory(logDir);
        // Construct the log file name using the application name (lowercase).
        return Path.Combine(logDir, $"{_appName.ToLowerInvariant()}.log");
    }

    /// <summary>
    ///     Resets all configuration settings to their predefined default values and saves the configuration to disk.
    /// </summary>
    public void ResetToDefaults()
    {
        Settings = new Dictionary<string, object>
        {
            { "enable_logging", true },
            { "log_path", GetLogPath() },
            { "show_tips", true },
            { "menu_width", 100 },
            { "version", _appVersion }
        };
        SaveConfig();
    }
}