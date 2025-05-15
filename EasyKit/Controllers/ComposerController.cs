using System.Diagnostics;
using System.Text.Json;

namespace EasyKit.Controllers;

public class ComposerController
{
    private readonly ConfirmationService _confirmation = new();
    private readonly ConsoleService _console;
    private readonly LoggerService _logger;
    private readonly PromptView _prompt = new();
    private readonly Software _software;

    public ComposerController(Software software, LoggerService logger, ConsoleService console)
    {
        _software = software;
        _logger = logger;
        _console = console;
    }

    public void ShowMenu()
    {
        // Get user settings
        int menuWidth = 50;
        string colorScheme = "dark";

        // Try to get user preferences from config if available
        var menuWidthObj = _console.Config.Get("menu_width", 50);
        if (menuWidthObj is int mw)
            menuWidth = mw;

        var colorSchemeObj = _console.Config.Get("color_scheme", "dark");
        if (colorSchemeObj != null)
            colorScheme = colorSchemeObj.ToString() ?? "dark";

        // Apply the appropriate color scheme based on user settings
        var (border, highlight, title, text, help) = colorScheme.ToLower() == "light"
            ? MenuTheme.ColorScheme.Light
            : MenuTheme.ColorScheme.Dark;

        // Create and configure the menu
        var menuView = new MenuView();
        menuView.CreateMenu("Composer Tools", width: menuWidth)
            .AddOption("1", "Install packages (composer install)", () => InstallPackages())
            .AddOption("2", "Update packages (composer update)", () => UpdatePackages())
            .AddOption("3", "Regenerate autoload files (composer dump-autoload)", () => RegenerateAutoload())
            .AddOption("4", "Require a new package", () => RequirePackage())
            .AddOption("5", "Create new project", () => CreateProject())
            .AddOption("6", "Validate composer.json", () => ValidateJson())
            .AddOption("7", "Clear Composer cache", () => ClearCache())
            .AddOption("8", "Show composer.json info", () => ShowPackageInfo())
            .AddOption("0", "Back to main menu", () =>
            {
                /* Return to main menu */
            })
            .WithColors(border, highlight, title, text, help)
            .WithHelpText("Select an option or press 0 to return to the main menu")
            .Show();
    }

    private string? FindComposerCommand()
    {
        if (File.Exists("composer.phar"))
            return "php composer.phar";
        if (File.Exists("composer.bat"))
            return "composer.bat";
        if (File.Exists("composer.exe"))
            return "composer.exe";
        return "composer";
    }

    private bool RunComposerCommand(string args, bool showOutput = true)
    {
        var composerCmd = FindComposerCommand();
        if (composerCmd == null)
        {
            _console.WriteError("Composer executable not found. Please ensure Composer is installed and in your PATH.");
            return false;
        }

        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = composerCmd.Split(' ')[0],
                Arguments = string.Join(' ', composerCmd.Split(' ').Skip(1)) +
                            (string.IsNullOrWhiteSpace(args) ? "" : " " + args),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            var process = Process.Start(psi);
            if (process == null)
            {
                _console.WriteError("Failed to start composer process.");
                return false;
            }

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();
            if (showOutput)
            {
                if (!string.IsNullOrWhiteSpace(output)) _console.WriteInfo(output);
                if (!string.IsNullOrWhiteSpace(error)) _console.WriteError(error);
            }

            return process.ExitCode == 0;
        }
        catch (Exception ex)
        {
            _logger.Error($"Error running composer command: {ex.Message}");
            if (showOutput) _console.WriteError(ex.Message);
            return false;
        }
    }

    private void InstallPackages()
    {
        _console.WriteInfo("Installing Composer packages...");
        if (RunComposerCommand("install"))
            _console.WriteSuccess("✓ Packages installed successfully!");
        else
            _console.WriteError("✗ Failed to install packages.");
        Console.ReadLine();
    }

    private void UpdatePackages()
    {
        _console.WriteInfo("Updating Composer packages...");
        if (RunComposerCommand("update"))
            _console.WriteSuccess("✓ Packages updated successfully!");
        else
            _console.WriteError("✗ Failed to update packages.");
        Console.ReadLine();
    }

    private void RegenerateAutoload()
    {
        _console.WriteInfo("Regenerating autoload files...");
        if (RunComposerCommand("dump-autoload"))
            _console.WriteSuccess("✓ Autoload files regenerated!");
        else
            _console.WriteError("✗ Failed to regenerate autoload files.");
        Console.ReadLine();
    }

    private void RequirePackage()
    {
        var package = _prompt.Prompt("Enter package name (e.g. 'vendor/package'): ");
        var dev = _confirmation.ConfirmAction("Is this a development dependency?", false);
        var args = dev ? $"require --dev {package}" : $"require {package}";
        _console.WriteInfo($"Installing {package}...");
        if (RunComposerCommand(args))
            _console.WriteSuccess($"✓ Package {package} installed successfully!");
        else
            _console.WriteError($"✗ Failed to install {package}.");
        Console.ReadLine();
    }

    private void CreateProject()
    {
        var package = _prompt.Prompt("Enter project package (e.g. 'laravel/laravel'): ");
        var directory = _prompt.Prompt("Enter project directory name: ");
        _console.WriteInfo($"Creating new project from {package}...");
        if (RunComposerCommand($"create-project {package} {directory}"))
            _console.WriteSuccess($"✓ Project created successfully in {directory}!");
        else
            _console.WriteError("✗ Failed to create project.");
        Console.ReadLine();
    }

    private void ValidateJson()
    {
        if (!File.Exists("composer.json"))
        {
            _console.WriteInfo("No composer.json found in current directory");
            Console.ReadLine();
            return;
        }

        _console.WriteInfo("Validating composer.json...");
        if (RunComposerCommand("validate"))
            _console.WriteSuccess("✓ composer.json is valid!");
        else
            _console.WriteError("✗ composer.json validation failed.");
        Console.ReadLine();
    }

    private void ClearCache()
    {
        if (_confirmation.ConfirmAction("Are you sure you want to clear Composer cache?", false))
        {
            _console.WriteInfo("Clearing Composer cache...");
            if (RunComposerCommand("clear-cache"))
                _console.WriteSuccess("✓ Cache cleared successfully!");
            else
                _console.WriteError("✗ Failed to clear cache.");
        }
        else
        {
            _console.WriteInfo("Cancelled.");
        }

        Console.ReadLine();
    }

    private void ShowPackageInfo()
    {
        if (!File.Exists("composer.json"))
        {
            _console.WriteInfo("No composer.json found in current directory");
            Console.ReadLine();
            return;
        }

        try
        {
            var json = File.ReadAllText("composer.json");
            var doc = JsonDocument.Parse(json);
            _console.WriteInfo(JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented = true }));
        }
        catch (Exception ex)
        {
            _logger.Error($"Error reading composer.json: {ex.Message}");
            _console.WriteError("Invalid composer.json file");
        }

        Console.ReadLine();
    }
}