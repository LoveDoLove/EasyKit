using System.Text.Json;

namespace EasyKit;

internal class Program
{
    private static readonly Config Config = new();
    private static readonly Software Software = new();
    private static readonly LoggerService Logger = new();
    private static readonly ConsoleService ConsoleService = new(Config);
    private static readonly ConfirmationService ConfirmationService = new(ConsoleService, Config);
    private static readonly MenuView MenuView = new();
    private static readonly NpmController NpmController = new(Software, Logger, ConsoleService);
    private static readonly LaravelController LaravelController = new(Software, Logger, ConsoleService);
    private static readonly ComposerController ComposerController = new(Software, Logger, ConsoleService);
    private static readonly GitController GitController = new(Software, Logger, ConsoleService);
    private static readonly SettingsController SettingsController = new(Config, Logger, ConsoleService);
    private static readonly ShortcutManagerController ShortcutManagerController = new(Config, Logger, ConsoleService);

    private static void Main(string[] args)
    {
        // Check if the application is running as administrator
        if (!AdminService.IsRunningAsAdmin())
        {
            if (ConfirmationService.ConfirmAdminElevation())
            {
                Console.WriteLine("Restarting with administrator privileges...");
                if (AdminService.RestartAsAdmin())
                    // The application will exit in the RestartAsAdmin method
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

        MainMenu();
    }

    public static void MainMenu()
    {
        while (true)
        {
            int menuWidth = 50;
            var menuWidthObj = Config.Get("menu_width", 50);
            if (menuWidthObj is int mw)
                menuWidth = mw;
            else if (menuWidthObj is JsonElement je && je.ValueKind == JsonValueKind.Number &&
                     je.TryGetInt32(out int jeInt)) menuWidth = jeInt;

            var versionObj = Config.Get("version", "3.2.1");
            string version = versionObj?.ToString() ?? "3.2.1";

            string colorSchemeStr = "dark";
            var colorSchemeObj = Config.Get("color_scheme", "dark");
            if (colorSchemeObj != null)
                colorSchemeStr =
                    colorSchemeObj.ToString() ?? "dark"; // Apply the appropriate color scheme based on user settings
            var colorScheme = MenuTheme.ColorScheme.Dark;
            if (colorSchemeStr.ToLower() == "light")
                colorScheme = MenuTheme.ColorScheme.Light;

            // Create and configure the main menu using the MenuBuilder
            MenuView.CreateMenu("EasyKit Main Menu", version, menuWidth)
                .AddOption("1", "NPM Tools", () => NpmController.ShowMenu())
                .AddOption("2", "Laravel Tools", () => LaravelController.ShowMenu())
                .AddOption("3", "Composer Tools", () => ComposerController.ShowMenu())
                .AddOption("4", "Git Tools", () => GitController.ShowMenu())
                .AddOption("5", "Settings", () => SettingsController.ShowMenu())
                .AddOption("6", "Shortcut Manager", () => ShortcutManagerController.ShowMenu())
                .AddOption("0", "Exit", () =>
                {
                    if (ExitProgram()) Environment.Exit(0);
                })
                // Add keyboard shortcuts for common operations
                .AddShortcut(ConsoleKey.N, "Quick access to NPM Tools", () => NpmController.ShowMenu())
                .AddShortcut(ConsoleKey.L, "Quick access to Laravel Tools", () => LaravelController.ShowMenu())
                .AddShortcut(ConsoleKey.C, "Quick access to Composer Tools", () => ComposerController.ShowMenu())
                .AddShortcut(ConsoleKey.G, "Quick access to Git Tools", () => GitController.ShowMenu())
                .AddShortcut(ConsoleKey.S, "Quick access to Settings", () => SettingsController.ShowMenu())
                .AddShortcut(ConsoleKey.M, "Quick access to Shortcut Manager",
                    () => ShortcutManagerController.ShowMenu())
                .WithColors(colorScheme.border, colorScheme.highlight, colorScheme.title, colorScheme.text,
                    colorScheme.help)
                .WithHelpText("Use number keys to select an option or arrow keys to navigate. Press F1 for help.")
                .WithSubtitle("A toolkit for web developers")
                .WithCenterVertically(true)
                .Show();
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