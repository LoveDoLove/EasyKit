using System.Text.Json;
using EasyKit.Views;
using EasyKit.Controllers;

namespace EasyKit;

internal class Program
{
    private static readonly Config Config = new();
    private static readonly Software Software = new();
    private static readonly LoggerService Logger = new();
    private static readonly ConsoleService ConsoleService = new(Config);
    private static readonly ConfirmationService ConfirmationService = new(ConsoleService, Config);
    private static readonly MenuView MenuView = new();
    private static readonly PromptView PromptView = new();
    private static readonly NotificationView NotificationView = new();
    private static readonly NpmController NpmController = new(Software, Logger, ConsoleService, ConfirmationService, PromptView, NotificationView);
    private static readonly LaravelController LaravelController = new(Software, Logger, ConsoleService, ConfirmationService, PromptView, NotificationView);
    private static readonly ComposerController ComposerController = new(Software, Logger, ConsoleService, ConfirmationService, PromptView, NotificationView);
    private static readonly GitController GitController = new(Software, Logger, ConsoleService, ConfirmationService, PromptView, NotificationView);
    private static readonly SettingsController SettingsController = new(Config, Logger, ConsoleService, PromptView);
    private static readonly ShortcutManagerController ShortcutManagerController = new(Config, Logger, ConsoleService, PromptView);
    private static readonly ToolMarketplaceController ToolMarketplaceController = new(
        new Services.ProcessService(Logger, ConsoleService, Config),
        ConsoleService);

    // Helper to select the best executable for a tool on Windows
    private static string? SelectBestExecutable(string tool, List<string> candidates)
    {
        if (candidates == null || candidates.Count == 0)
            return null;

        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
        {
            switch (tool.ToLower())
            {
                case "npm":
                    // Prefer npm.cmd, then npm.exe, then npm
                    var npmCmd = candidates.FirstOrDefault(p => p.EndsWith("npm.cmd", StringComparison.OrdinalIgnoreCase));
                    if (npmCmd != null) return npmCmd;
                    var npmExe = candidates.FirstOrDefault(p => p.EndsWith("npm.exe", StringComparison.OrdinalIgnoreCase));
                    if (npmExe != null) return npmExe;
                    break;
                case "node":
                    // Prefer node.exe
                    var nodeExe = candidates.FirstOrDefault(p => p.EndsWith("node.exe", StringComparison.OrdinalIgnoreCase));
                    if (nodeExe != null) return nodeExe;
                    break;
                case "php":
                    // Prefer php.exe
                    var phpExe = candidates.FirstOrDefault(p => p.EndsWith("php.exe", StringComparison.OrdinalIgnoreCase));
                    if (phpExe != null) return phpExe;
                    break;
                case "composer":
                    // Prefer composer.bat, then composer.phar, then composer.exe
                    var composerBat = candidates.FirstOrDefault(p => p.EndsWith("composer.bat", StringComparison.OrdinalIgnoreCase));
                    if (composerBat != null) return composerBat;
                    var composerPhar = candidates.FirstOrDefault(p => p.EndsWith("composer.phar", StringComparison.OrdinalIgnoreCase));
                    if (composerPhar != null) return composerPhar;
                    var composerExe = candidates.FirstOrDefault(p => p.EndsWith("composer.exe", StringComparison.OrdinalIgnoreCase));
                    if (composerExe != null) return composerExe;
                    break;
                case "git":
                    // Prefer git.exe
                    var gitExe = candidates.FirstOrDefault(p => p.EndsWith("git.exe", StringComparison.OrdinalIgnoreCase));
                    if (gitExe != null) return gitExe;
                    break;
            }
        }
        // Fallback: return the first candidate
        return candidates[0];
    }

    private static void AutoDetectAndSaveToolPaths()
    {
        var processService = new ProcessService(Logger, ConsoleService, Config);
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
                .GetMethod("GetSearchPathsForExecutable", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.Invoke(processService, new object[] { tool }) as string[];
            if (searchPaths != null)
            {
                foreach (var path in searchPaths)
                {
                    if (!string.IsNullOrEmpty(path) && System.IO.File.Exists(path) && !candidates.Contains(path, StringComparer.OrdinalIgnoreCase))
                        candidates.Add(path);
                }
            }
            // Select the best candidate
            var best = SelectBestExecutable(tool, candidates);
            if (!string.IsNullOrEmpty(best))
            {
                if (candidates.Count > 1 && !string.Equals(best, candidates[0], StringComparison.OrdinalIgnoreCase))
                {
                    Logger.Warning($"Auto-detected {tool}: Found multiple candidates, selected '{best}' as the best match.");
                }
                Config.Set($"{tool}_path", best);
                Logger.Info($"Auto-detected {tool} path: {best} and saved to config.");
            }
        }
    }

    private static void Main(string[] args)
    {
        try
        {
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
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("A fatal error occurred:");
            Console.WriteLine(ex.ToString());
            Console.ResetColor();
            NotificationView.Show("A fatal error occurred. See console for details.",
                NotificationView.NotificationType.Error, requireKeyPress: true);
        }
    }

    public static void MainMenu()
    {
        while (true)
        {
            Console.Clear();
            string version = "1.0"; // Or fetch dynamically if available
            MenuView.ShowMainMenu(version);
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
                default:
                    // Ignore other keys
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