using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Microsoft.Win32;

namespace EasyKit.Controllers;

internal class ShortcutManagerController
{
    private readonly Config _config;
    private readonly ConsoleService _console;
    private readonly LoggerService _logger;
    private readonly PromptView _prompt = new();

    public ShortcutManagerController(Config config, LoggerService logger, ConsoleService console)
    {
        _config = config;
        _logger = logger;
        _console = console;
    }

    public void ShowMenu()
    {
        int menuWidth = 50;
        string colorSchemeStr = "dark";

        var menuWidthObj = _config.Get("menu_width", 50);
        if (menuWidthObj is int mw)
            menuWidth = mw;

        var colorSchemeObj = _config.Get("color_scheme", "dark");
        if (colorSchemeObj != null)
            colorSchemeStr = colorSchemeObj.ToString() ?? "dark";

        var colorScheme = MenuTheme.ColorScheme.Dark;
        if (colorSchemeStr.ToLower() == "light")
            colorScheme = MenuTheme.ColorScheme.Light;

        var menuView = new MenuView();
        menuView.CreateMenu("Shortcut Manager", width: menuWidth)
            .AddOption("1", "View Shortcuts", () => _console.WriteInfo("View Shortcuts - Not Implemented"))
            .AddOption("2", "Manage Context Menu", ManageContextMenu)
            .AddOption("0", "Back", () => { })
            .WithColors(colorScheme.border, colorScheme.highlight, colorScheme.title, colorScheme.text,
                colorScheme.help)
            .WithHelpText("Manage your keyboard shortcuts and context menu. Press 0 to return.")
            .WithSubtitle("Shortcut Manager")
            .Show();
    }

    private void ManageContextMenu()
    {
        // Only show context menu management options on Windows
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            _console.WriteInfo("\nContext menu management is only available on Windows.");
            _console.WriteInfo("Press any key to return...");
            Console.ReadKey(true);
            return;
        }

        while (true)
        {
            bool openWithEasyKit = _config.Get("open_with_easykit", false) is bool b && b;
            _console.WriteInfo("\nContext Menu:");
            _console.WriteInfo($"  1. {(openWithEasyKit ? "Remove" : "Add")} 'Open with EasyKit' option");
            _console.WriteInfo("  0. Back");

            var choice = _prompt.Prompt("Select an option: ");
            if (choice == "1")
                ToggleOpenWithEasyKit();
            else if (choice == "0") break;
        }
    }

    private void ToggleOpenWithEasyKit()
    {
        bool openWithEasyKit = _config.Get("open_with_easykit", false) is bool b && b;
        if (!openWithEasyKit)
        {
            RegisterOpenWithEasyKit();
            _console.WriteInfo("'Open with EasyKit' option added.");
        }
        else
        {
            UnregisterOpenWithEasyKit();
            _console.WriteInfo("'Open with EasyKit' option removed.");
        }

        _config.Settings["open_with_easykit"] = !openWithEasyKit;
        _config.SaveConfig();
    }

    [SupportedOSPlatform("windows")]
    private void RegisterOpenWithEasyKit()
    {
        try
        {
            string exePath = Process.GetCurrentProcess().MainModule?.FileName ?? "EasyKit.exe";
            var scope = _config.Get("context_menu_scope", "user")?.ToString() ?? "user";
            var root = scope == "system" ? Registry.ClassesRoot : Registry.CurrentUser;
            string[] registryPaths = scope == "system"
                ? new[]
                {
                    @"*\shell\EasyKit",
                    @"Directory\shell\EasyKit",
                    @"Directory\Background\shell\EasyKit"
                }
                : new[]
                {
                    @"Software\Classes\*\shell\EasyKit",
                    @"Software\Classes\Directory\shell\EasyKit",
                    @"Software\Classes\Directory\Background\shell\EasyKit"
                };

            foreach (var registryPath in registryPaths)
                try
                {
                    using (var key = root.CreateSubKey(registryPath))
                    {
                        if (key == null)
                        {
                            _console.WriteInfo(
                                $"Failed to create registry key: {registryPath} in {(scope == "system" ? "HKCR" : "HKCU")}");
                            continue;
                        }

                        key.SetValue("", "EasyKit", RegistryValueKind.String);
                        using (var commandKey = key.CreateSubKey("command"))
                        {
                            if (commandKey != null)
                            {
                                string arg = GetContextMenuArgument(registryPath);
                                string command = $"\"{exePath}\" \"{arg}\"";
                                commandKey.SetValue("", command, RegistryValueKind.String);
                                // Don't add IsolatedCommand as it can cause issues with elevation
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _console.WriteInfo(
                        $"Exception creating registry key: {registryPath} in {(scope == "system" ? "HKCR" : "HKCU")}: {ex.Message}");
                }
        }
        catch (Exception ex)
        {
            _console.WriteInfo("Failed to add context menu: " + ex.Message);
        }
    }

    /// <summary>
    ///     Returns the correct argument for the context menu command based on the registry path.
    /// </summary>
    private static string GetContextMenuArgument(string registryPath)
    {
        if (registryPath.Contains("Directory\\Background"))
            return "%V";
        return "%1";
    }

    [SupportedOSPlatform("windows")]
    private void UnregisterOpenWithEasyKit()
    {
        try
        {
            var scope = _config.Get("context_menu_scope", "user")?.ToString() ?? "user";
            var root = scope == "system" ? Registry.ClassesRoot : Registry.CurrentUser;
            string[] delRegistryPaths = scope == "system"
                ? new[]
                {
                    @"*\shell\EasyKit",
                    @"Directory\shell\EasyKit",
                    @"Directory\Background\shell\EasyKit"
                }
                : new[]
                {
                    @"Software\Classes\*\shell\EasyKit",
                    @"Software\Classes\Directory\shell\EasyKit",
                    @"Software\Classes\Directory\Background\shell\EasyKit"
                };
            foreach (var delRegistryPath in delRegistryPaths)
                try
                {
                    root.DeleteSubKeyTree(delRegistryPath, false);
                }
                catch
                {
                    /* Ignore if not present */
                }
        }
        catch (Exception ex)
        {
            _console.WriteInfo("Failed to remove context menu: " + ex.Message);
        }
    }

    // Add a shortcut to the directory background context menu
    [SupportedOSPlatform("windows")]
    private void AddBackgroundContextMenuShortcut(string shortcutName, string command)
    {
        try
        {
            string keyPath = $@"Software\Classes\Directory\Background\shell\{shortcutName}";
            using (var key = Registry.CurrentUser.CreateSubKey(keyPath))
            {
                if (key != null)
                {
                    // Set the menu label
                    key.SetValue(null, shortcutName, RegistryValueKind.String);
                    // Optional: set an icon
                    // key.SetValue("Icon", "C:\\Path\\To\\EasyKit.ico", RegistryValueKind.String);
                    using (var commandKey = key.CreateSubKey("command"))
                    {
                        if (commandKey != null)
                            // The command must be quoted and include "%V" for the directory path
                            commandKey.SetValue(null, $"\"{command}\" \"%V\"", RegistryValueKind.String);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _console.WriteInfo("Failed to add background context menu shortcut: " + ex.Message);
        }
    }

    // Remove a shortcut from the directory background context menu
    [SupportedOSPlatform("windows")]
    private void RemoveBackgroundContextMenuShortcut(string shortcutName)
    {
        try
        {
            string keyPath = $@"Software\Classes\Directory\Background\shell\{shortcutName}";
            Registry.CurrentUser.DeleteSubKeyTree(keyPath, false);
        }
        catch (Exception ex)
        {
            _console.WriteInfo("Failed to remove background context menu shortcut: " + ex.Message);
        }
    }
}