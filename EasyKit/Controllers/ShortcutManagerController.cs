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
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    ToggleOpenWithEasyKitWindows();
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    ToggleOpenWithEasyKitLinux();
                else if (choice == "0") break;
        }
    }

    [SupportedOSPlatform("windows")]
    private void ToggleOpenWithEasyKitWindows()
    {
        bool openWithEasyKit = _config.Get("open_with_easykit", false) is bool b && b;
        if (!openWithEasyKit)
        {
            RegisterOpenWithEasyKit();
            _console.WriteInfo("'Open with EasyKit' option added.");
            _config.Settings["open_with_easykit"] = true;
            _config.SaveConfig();
        }
        else
        {
            UnregisterOpenWithEasyKit();
            _console.WriteInfo("'Open with EasyKit' option removed.");
            _config.Settings["open_with_easykit"] = false;
            _config.SaveConfig();
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
                ?
                [
                    @"*\shell\EasyKit",
                    @"Directory\shell\EasyKit",
                    @"Directory\Background\shell\EasyKit"
                ]
                :
                [
                    @"Software\Classes\*\shell\EasyKit",
                    @"Software\Classes\Directory\shell\EasyKit",
                    @"Software\Classes\Directory\Background\shell\EasyKit"
                ];

            foreach (var registryPath in registryPaths)
                try
                {
                    using var key = root.CreateSubKey(registryPath);
                    key.SetValue("", "EasyKit", RegistryValueKind.String);
                    using var commandKey = key.CreateSubKey("command");
                    string arg = GetContextMenuArgument(registryPath);
                    string command = $"\"{exePath}\" \"{arg}\"";
                    commandKey.SetValue("", command, RegistryValueKind.String);
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

    private static string GetContextMenuArgument(string registryPath)
    {
        return registryPath.Contains("Directory\\Background") ? "%V" : "%1";
    }

    [SupportedOSPlatform("windows")]
    private void UnregisterOpenWithEasyKit()
    {
        try
        {
            var scope = _config.Get("context_menu_scope", "user")?.ToString() ?? "user";
            var root = scope == "system" ? Registry.ClassesRoot : Registry.CurrentUser;
            string[] delRegistryPaths = scope == "system"
                ?
                [
                    @"*\shell\EasyKit",
                    @"Directory\shell\EasyKit",
                    @"Directory\Background\shell\EasyKit"
                ]
                :
                [
                    @"Software\Classes\*\shell\EasyKit",
                    @"Software\Classes\Directory\shell\EasyKit",
                    @"Software\Classes\Directory\Background\shell\EasyKit"
                ];
            foreach (var delRegistryPath in delRegistryPaths)
                try
                {
                    root.DeleteSubKeyTree(delRegistryPath, false);
                }
                catch
                {
                    // ignored
                }
        }
        catch (Exception ex)
        {
            _console.WriteInfo("Failed to remove context menu: " + ex.Message);
        }
    }

    [SupportedOSPlatform("linux")]
    private void ToggleOpenWithEasyKitLinux()
    {
        bool openWithEasyKit = _config.Get("open_with_easykit", false) is bool b && b;
        if (!openWithEasyKit)
        {
            RegisterOpenWithEasyKitLinux();
            _console.WriteInfo("'Open with EasyKit' option added.");
            _config.Settings["open_with_easykit"] = true;
            _config.SaveConfig();
        }
        else
        {
            UnregisterOpenWithEasyKitLinux();
            _console.WriteInfo("'Open with EasyKit' option removed.");
            _config.Settings["open_with_easykit"] = false;
            _config.SaveConfig();
        }
    }

    [SupportedOSPlatform("linux")]
    private void RegisterOpenWithEasyKitLinux()
    {
        try
        {
            string exePath = Process.GetCurrentProcess().MainModule?.FileName ?? "EasyKit.exe";
            string homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string appDir = Path.Combine(homeDir, ".local", "share", "applications");
            string fileActionPath = Path.Combine(appDir, "easykit-file.desktop");
            string dirActionPath = Path.Combine(appDir, "easykit-directory.desktop");

            // Create applications directory if it doesn't exist
            if (!Directory.Exists(appDir)) Directory.CreateDirectory(appDir);

            // Desktop file for regular files
            string fileContent = $"""
                                  [Desktop Entry]
                                  Type=Application
                                  Name=Open with EasyKit
                                  Icon={exePath}
                                  Exec={exePath} %f
                                  NoDisplay=true
                                  MimeType=*/*;
                                  """;
            File.WriteAllText(fileActionPath, fileContent);

            // Desktop file for directories
            string dirContent = $"""
                                 [Desktop Entry]
                                 Type=Application
                                 Name=Open with EasyKit
                                 Icon={exePath}
                                 Exec={exePath} %f
                                 NoDisplay=true
                                 MimeType=inode/directory;
                                 """;
            File.WriteAllText(dirActionPath, dirContent);

            // Update the database of desktop entries
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "update-desktop-database",
                    Arguments = $"{appDir}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            try
            {
                process.Start();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to update desktop database: {ex.Message}");
            }

            _logger.Info("Linux context menu entries created successfully");
        }
        catch (Exception ex)
        {
            _console.WriteInfo($"Failed to add context menu entry: {ex.Message}");
            _logger.Error($"Failed to add Linux context menu entry: {ex.Message}");
        }
    }

    [SupportedOSPlatform("linux")]
    private void UnregisterOpenWithEasyKitLinux()
    {
        try
        {
            string homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string appDir = Path.Combine(homeDir, ".local", "share", "applications");
            string fileActionPath = Path.Combine(appDir, "easykit-file.desktop");
            string dirActionPath = Path.Combine(appDir, "easykit-directory.desktop");

            // Remove desktop entry files if they exist
            if (File.Exists(fileActionPath)) File.Delete(fileActionPath);

            if (File.Exists(dirActionPath)) File.Delete(dirActionPath);

            // Update the database of desktop entries
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "update-desktop-database",
                    Arguments = $"{appDir}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            try
            {
                process.Start();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to update desktop database: {ex.Message}");
            }

            _logger.Info("Linux context menu entries removed successfully");
        }
        catch (Exception ex)
        {
            _console.WriteInfo($"Failed to remove context menu entry: {ex.Message}");
            _logger.Error($"Failed to remove Linux context menu entry: {ex.Message}");
        }
    }
}