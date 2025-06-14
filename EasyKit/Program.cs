using System.Reflection;
using System.Runtime.InteropServices;
using CommonUtilities.Models.Share;
using CommonUtilities.Services.Admin;
using CommonUtilities.Services.ContextMenuManager;
using CommonUtilities.Services.Shared;
using CommonUtilities.UI.ConsoleUI;
using CommonUtilities.Utilities.System;
using EasyKit.Controllers;
using EasyKit.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EasyKit;

internal class Program
{
    // Updated for new Config.cs API: requires appName and appVersion
    private static readonly Config Config =
        new("EasyKit", Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0");

    private static readonly IServiceProvider ServiceProvider = ConfigureServices();
    private static readonly Software Software = new();
    private static readonly ConsoleService ConsoleService = new(Config);
    private static readonly ConfirmationService ConfirmationService = new(Config);
    private static readonly MenuView MenuView = new();
    private static readonly PromptView PromptView = new();
    private static readonly NotificationView NotificationView = new();

    private static readonly NpmController NpmController =
        new(Software, ConsoleService, ConfirmationService, PromptView, NotificationView);

    private static readonly LaravelController LaravelController =
        new(Software, ConsoleService, ConfirmationService, PromptView, NotificationView);

    private static readonly ComposerController ComposerController =
        new(Software, ConsoleService, ConfirmationService, PromptView, NotificationView);

    private static readonly GitController GitController =
        new(Software, ConsoleService, ConfirmationService, PromptView, NotificationView);

    private static readonly SettingsController SettingsController = new(Config, ConsoleService, PromptView);

    private static readonly ShortcutManagerController ShortcutManagerController =
        new(Config, ConsoleService, PromptView, ServiceProvider.GetRequiredService<IContextMenuManager>());

    private static readonly ToolMarketplaceController ToolMarketplaceController = new(
        new ProcessService(ConsoleService, Config),
        ConsoleService);

    // Helper to select the best executable for a tool on Windows
    private static string? SelectBestExecutable(string tool, List<string> candidates)
    {
        if (candidates == null || candidates.Count == 0)
            return null;

        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            switch (tool.ToLower())
            {
                case "npm":
                    // Prefer npm.cmd, then npm.exe, then npm
                    var npmCmd =
                        candidates.FirstOrDefault(p => p.EndsWith("npm.cmd", StringComparison.OrdinalIgnoreCase));
                    if (npmCmd != null) return npmCmd;
                    var npmExe =
                        candidates.FirstOrDefault(p => p.EndsWith("npm.exe", StringComparison.OrdinalIgnoreCase));
                    if (npmExe != null) return npmExe;
                    break;
                case "node":
                    // Prefer node.exe
                    var nodeExe =
                        candidates.FirstOrDefault(p => p.EndsWith("node.exe", StringComparison.OrdinalIgnoreCase));
                    if (nodeExe != null) return nodeExe;
                    break;
                case "php":
                    // Prefer php.exe
                    var phpExe =
                        candidates.FirstOrDefault(p => p.EndsWith("php.exe", StringComparison.OrdinalIgnoreCase));
                    if (phpExe != null) return phpExe;
                    break;
                case "composer":
                    // Prefer composer.bat, then composer.phar, then composer.exe
                    var composerBat = candidates.FirstOrDefault(p =>
                        p.EndsWith("composer.bat", StringComparison.OrdinalIgnoreCase));
                    if (composerBat != null) return composerBat;
                    var composerPhar = candidates.FirstOrDefault(p =>
                        p.EndsWith("composer.phar", StringComparison.OrdinalIgnoreCase));
                    if (composerPhar != null) return composerPhar;
                    var composerExe = candidates.FirstOrDefault(p =>
                        p.EndsWith("composer.exe", StringComparison.OrdinalIgnoreCase));
                    if (composerExe != null) return composerExe;
                    break;
                case "git":
                    // Prefer git.exe
                    var gitExe =
                        candidates.FirstOrDefault(p => p.EndsWith("git.exe", StringComparison.OrdinalIgnoreCase));
                    if (gitExe != null) return gitExe;
                    break;
            }

        // Fallback: return the first candidate
        return candidates[0];
    }

    private static void AutoDetectAndSaveToolPaths()
    {
        var processService = new ProcessService(ConsoleService, Config);
        var tools = new[] { "npm", "node", "php", "composer", "git" };
        foreach (var tool in tools)
        {
            // Gather all possible candidates
            var candidates = new List<string>();
            // 1. PATH search
            var pathExecutable = processService.FindExecutablePath(tool);
            if (!string.IsNullOrEmpty(pathExecutable))
                candidates.Add(pathExecutable);
            // 2. All known search paths
            var searchPaths = typeof(ProcessService)
                .GetMethod("GetSearchPathsForExecutable", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.Invoke(processService, new object[] { tool }) as string[];
            if (searchPaths != null)
                foreach (var path in searchPaths)
                    if (!string.IsNullOrEmpty(path) && File.Exists(path) &&
                        !candidates.Contains(path, StringComparer.OrdinalIgnoreCase))
                        candidates.Add(path);

            // Select the best candidate
            var best = SelectBestExecutable(tool, candidates);
            if (!string.IsNullOrEmpty(best))
            {
                if (candidates.Count > 1 && !string.Equals(best, candidates[0], StringComparison.OrdinalIgnoreCase))
                    LoggerUtilities.Warning(
                        $"Auto-detected {tool}: Found multiple candidates, selected '{best}' as the best match.");
                Config.Set($"{tool}_path", best);
                LoggerUtilities.Info($"Auto-detected {tool} path: {best} and saved to config.");
            }
        }
    }

    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        // Add logging
        services.AddLogging(configure =>
        {
            configure.AddConsole();
            // Add other logging providers if needed
        });

        // Register IContextMenuManager
        services.AddScoped<IContextMenuManager>(sp =>
        {
            var osPlatform = OSPlatform.Create("Unknown");
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                osPlatform = OSPlatform.Windows;
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) osPlatform = OSPlatform.Linux;
            // Add other OS checks like OSPlatform.OSX if needed

            if (osPlatform == OSPlatform.Windows)
                return new WindowsContextMenuManager(@"Software\\EasyKit\\ContextMenuEntries");

            if (osPlatform == OSPlatform.Linux) return new LinuxContextMenuManager();
            // Potentially add MacOS support or a default/null implementation
            var logger = sp.GetRequiredService<ILogger<Program>>(); // Fallback logger for the warning
            logger.LogWarning(
                $"Context menu management is not supported on this OS: {RuntimeInformation.OSDescription}. Attempting to use it will result in an exception.");
            // Return a NullContextMenuManager or throw if strict support is required.
            // For now, let's throw as per the initial requirement.
            throw new PlatformNotSupportedException(
                $"Context menu management is not supported on this OS: {RuntimeInformation.OSDescription}.");
        });

        return services.BuildServiceProvider();
    }

    private static void Main(string[] args)
    {
        try
        {
            // Initialize logging using CommonUtilities
            LoggerUtilities.StartLog("EasyKit");
            LoggerUtilities.Info("EasyKit application started");

            AutoDetectAndSaveToolPaths();

            // If launched from context menu, always use absolute path argument
            string? originalArg = args.Length > 0 ? args[0] : null;

            // Check if the application is running as administrator
            if (!AdminService.IsRunningAsAdmin())
            {
                if (ConfirmationService.ConfirmAdminElevation())
                {
                    Console.WriteLine("Restarting with administrator privileges...");
                    // Relaunch with original argument if present
                    if (AdminService.RestartAsAdmin(originalArg))
                        return;

                    Console.WriteLine();
                    NotificationView.Show("Failed to restart with admin rights. Some features may be limited.",
                        NotificationView.NotificationType.Warning, requireKeyPress: true);
                }
                else
                {
                    NotificationView.Show("Continuing without admin rights. Some features may be limited.",
                        NotificationView.NotificationType.Warning, requireKeyPress: true);
                }
            }

            // If an argument is provided, open the folder or file directly
            if (!string.IsNullOrEmpty(originalArg))
            {
                try
                {
                    // Handle the file or directory passed from context menu
                    if (Directory.Exists(originalArg))
                    {
                        // Change current directory to the selected directory
                        Environment.CurrentDirectory = originalArg;
                        Console.WriteLine($"Directory set to: {originalArg}");
                        NotificationView.Show($"Working directory set to: {originalArg}",
                            NotificationView.NotificationType.Success, requireKeyPress: false);

                        // After setting directory, continue to the main menu
                        MainMenu();
                    }
                    else if (File.Exists(originalArg))
                    {
                        // Handle file operations based on file type
                        string extension = Path.GetExtension(originalArg).ToLower();
                        string directory = Path.GetDirectoryName(originalArg) ?? Environment.CurrentDirectory;

                        // Set current directory to the file's directory
                        Environment.CurrentDirectory = directory;
                        Console.WriteLine($"File selected: {originalArg}");
                        NotificationView.Show($"File selected: {Path.GetFileName(originalArg)}",
                            NotificationView.NotificationType.Success, requireKeyPress: false);

                        // After handling file, continue to the main menu
                        MainMenu();
                    }
                    else
                    {
                        Console.WriteLine($"Path not found: {originalArg}");
                        NotificationView.Show($"Path not found: {originalArg}",
                            NotificationView.NotificationType.Warning, requireKeyPress: true);
                        // Continue to main menu if path not found
                        MainMenu();
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error opening path: {originalArg}");
                    Console.WriteLine(ex.ToString());
                    Console.ResetColor();
                    NotificationView.Show("Failed to open the selected path. See console for details.",
                        NotificationView.NotificationType.Error, requireKeyPress: true);
                    // Continue to main menu even after error
                    MainMenu();
                }

                return;
            }

            MainMenu();
        }
        catch (Exception ex)
        {
            LoggerUtilities.Fatal(ex, "A fatal error occurred");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("A fatal error occurred:");
            Console.WriteLine(ex.ToString());
            Console.ResetColor();
            NotificationView.Show("A fatal error occurred. See console for details.",
                NotificationView.NotificationType.Error, requireKeyPress: true);
        }
        finally
        {
            // Clean up logging
            LoggerUtilities.Info("EasyKit application shutting down");
            LoggerUtilities.StopLog();
        }
    }

    public static void MainMenu()
    {
        while (true)
        {
            Console.Clear();
            string version = Config.Get("version", "1.0")?.ToString() ?? "1.0";

            // Use MenuView from CommonUtilities to show the main menu
            MenuView.ShowMenu("EasyKit Main Menu v" + version, new[]
            {
                "0. Exit",
                "1. NPM Tools",
                "2. Laravel Tools",
                "3. Composer Tools",
                "4. Git Tools",
                "5. Settings",
                "6. Shortcut Manager"
            });

            Console.WriteLine("[T] Tool Marketplace");
            Console.WriteLine("[Q] Quit");
            var key = Console.ReadKey(true).Key;
            switch (key)
            {
                case ConsoleKey.T:
                    ToolMarketplaceController.ShowMarketplace();
                    break;
                case ConsoleKey.Q:
                case ConsoleKey.D0:
                    if (ExitProgram()) return;
                    break;
                case ConsoleKey.D1:
                    NpmController.ShowMenu();
                    break;
                case ConsoleKey.D2:
                    LaravelController.ShowMenu();
                    break;
                case ConsoleKey.D3:
                    ComposerController.ShowMenu();
                    break;
                case ConsoleKey.D4:
                    GitController.ShowMenu();
                    break;
                case ConsoleKey.D5:
                    SettingsController.ShowMenu();
                    break;
                case ConsoleKey.D6:
                    ShortcutManagerMenu();
                    break;
            }
        }
    }

    /// <summary>
    ///     Displays and handles the Shortcut Manager menu.
    ///     This menu allows users to manage predefined application shortcuts.
    /// </summary>
    private static void ShortcutManagerMenu()
    {
        while (true)
        {
            Console.Clear();
            // Get current statuses
            bool isOpenWithEasyKitEnabled = Config.Get("open_with_easykit", false) is bool bOpen && bOpen;

            var menuItems = new List<string>
            {
                "0. Back",
                $"1. Open with EasyKit       [Status: {(isOpenWithEasyKitEnabled ? "Enabled" : "Disabled")}] [Manage...]"
            };

            MenuView.ShowMenu("Shortcut Manager", menuItems.ToArray());
            var key = Console.ReadKey(true).Key;
            switch (key)
            {
                case ConsoleKey.D0:
                    return;
                case ConsoleKey.D1: // Manage Open with EasyKit
                    ShortcutManagerController.ManageContextMenuAsync().GetAwaiter().GetResult();
                    break;
            }
        }
    }

    private static bool ExitProgram()
    {
        // Check if confirm_exit is enabled
        var confirmExitObj = Config.Get("confirm_exit", true);
        bool confirmExit = confirmExitObj is bool b && b;

        if (confirmExit)
        {
            var promptView = new PromptView();
            bool confirm = promptView.ConfirmYesNo("Are you sure you want to exit?", false);
            if (!confirm) return false;
        }

        // Clear the screen for a clean exit
        Console.Clear();

        // Get the console width for centered text
        int width = Console.WindowWidth;

        // Display a nice goodbye message
        Console.ForegroundColor = ConsoleColor.Cyan;
        string goodbye = "Thank you for using EasyKit!";
        string byLine = "A toolkit for web developers";

        // Center the messages
        int goodbyePadding = (width - goodbye.Length) / 2;
        int byLinePadding = (width - byLine.Length) / 2;

        // Add some vertical spacing
        for (int i = 0; i < Console.WindowHeight / 3; i++)
            Console.WriteLine();

        // Write centered messages
        Console.WriteLine(new string(' ', goodbyePadding) + goodbye);
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine(new string(' ', byLinePadding) + byLine);

        // Reset colors
        Console.ResetColor();

        // Add a small delay for better UX
        Thread.Sleep(1000);

        return true;
    }
}