using System.Diagnostics;
using System.Text.Json;

namespace EasyKit.Controllers;

public class NpmController
{
    private readonly ConsoleService _console;
    private readonly LoggerService _logger;
    private readonly Software _software;

    public NpmController(Software software, LoggerService logger, ConsoleService console)
    {
        _software = software;
        _logger = logger;
        _console = console;
    }

    public void ShowMenu()
    {
        // Get user settings
        int menuWidth = 50;
        string colorSchemeStr = "dark";

        // Try to get user preferences from config if available
        var menuWidthObj = _console.Config.Get("menu_width", 50);
        if (menuWidthObj is int mw)
            menuWidth = mw;

        var colorSchemeObj = _console.Config.Get("color_scheme", "dark");
        if (colorSchemeObj != null)
            colorSchemeStr = colorSchemeObj.ToString() ?? "dark";
        // Apply the appropriate color scheme based on user settings
        var colorScheme = MenuTheme.ColorScheme.Dark;
        if (colorSchemeStr.ToLower() == "light")
            colorScheme = MenuTheme.ColorScheme.Light;
        // Use a custom theme for NPM - purple with double border
        else
            colorScheme = MenuTheme.ColorScheme.Purple;

        // Create and configure the menu
        var menuView = new MenuView();
        menuView.CreateMenu("NPM Tools", width: menuWidth)
            .AddOption("1", "Install packages (npm install)", () => InstallPackages())
            .AddOption("2", "Update packages (npm-check-updates)", () => UpdatePackages())
            .AddOption("3", "Build for production (npm run build)", () => BuildProduction())
            .AddOption("4", "Start development server (npm run dev)", () => BuildDevelopment())
            .AddOption("5", "Security audit (npm audit)", () => SecurityAudit())
            .AddOption("6", "Run custom npm script", () => RunCustomScript())
            .AddOption("7", "Show package.json info", () => ShowPackageInfo())
            .AddOption("8", "Reset npm cache", () => ResetCache())
            .AddOption("0", "Back to main menu", () =>
            {
                /* Return to main menu */
            })
            .WithColors(colorScheme.border, colorScheme.highlight, colorScheme.title, colorScheme.text,
                colorScheme.help)
            .WithHelpText("Select an option or press 0 to return to the main menu")
            .WithDoubleBorder() // Using a double border for NPM tools for a distinct look
            .Show();
    }

    private bool EnsureNpmInstalled()
    {
        var result = RunProcess("npm", "--version", false);
        if (!result)
        {
            _console.WriteError("Node.js/NPM is not installed. Please install Node.js from https://nodejs.org/");
            Console.ReadLine();
        }

        return result;
    }

    private bool RunProcess(string file, string args, bool showOutput = true)
    {
        try
        {
            var process = new Process();
            process.StartInfo.FileName = file;
            process.StartInfo.Arguments = args;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
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
            _logger.Error($"Error running process: {ex.Message}");
            if (showOutput) _console.WriteError(ex.Message);
            return false;
        }
    }

    private void InstallPackages()
    {
        _console.WriteInfo("Installing npm packages...");
        if (EnsureNpmInstalled())
        {
            if (RunProcess("npm", "install --no-fund --loglevel=error"))
                _console.WriteInfo("✓ Packages installed successfully!");
            else
                _console.WriteError("✗ Failed to install packages.");
        }

        Console.ReadLine();
    }

    private void UpdatePackages()
    {
        _console.WriteInfo("Updating npm packages (npm-check-updates)...");
        if (!EnsureNpmInstalled())
        {
            Console.ReadLine();
            return;
        }

        // Check if ncu is installed
        if (!RunProcess("ncu", "--version", false))
        {
            _console.WriteInfo("Installing npm-check-updates globally...");
            if (!RunProcess("npm", "install -g npm-check-updates"))
            {
                _console.WriteError("Failed to install npm-check-updates");
                Console.ReadLine();
                return;
            }
        }

        if (RunProcess("ncu", "-u"))
        {
            _console.WriteInfo("✓ package.json updated!");
            RunProcess("npm", "install --no-fund --loglevel=error");
        }
        else
        {
            _console.WriteError("✗ Failed to update packages.");
        }

        Console.ReadLine();
    }

    private void BuildProduction()
    {
        _console.WriteInfo("Building for production (npm run build)...");
        if (EnsureNpmInstalled())
        {
            if (RunProcess("npm", "run build"))
                _console.WriteInfo("✓ Production build completed!");
            else
                _console.WriteError("✗ Build failed.");
        }

        Console.ReadLine();
    }

    private void BuildDevelopment()
    {
        _console.WriteInfo("Starting development server (npm run dev)...");
        if (!EnsureNpmInstalled())
        {
            Console.ReadLine();
            return;
        }

        try
        {
            // Open a new cmd window and run npm run dev
            Process.Start(new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/c start cmd /k \"npm run dev\"",
                UseShellExecute = true
            });
            _console.WriteInfo("✓ Development server started in a new terminal window.");
        }
        catch (Exception ex)
        {
            _logger.Error($"Error starting dev server: {ex.Message}");
            _console.WriteError("✗ Failed to start development server.");
        }

        Console.ReadLine();
    }

    private void SecurityAudit()
    {
        _console.WriteInfo("Running npm security audit...");
        if (EnsureNpmInstalled())
            RunProcess("npm", "audit");
        Console.ReadLine();
    }

    private void RunCustomScript()
    {
        if (!EnsureNpmInstalled())
        {
            Console.ReadLine();
            return;
        }

        if (!File.Exists("package.json"))
        {
            _console.WriteError("No package.json found in current directory");
            Console.ReadLine();
            return;
        }

        try
        {
            var json = File.ReadAllText("package.json");
            var packageObj = JsonDocument.Parse(json);
            if (!packageObj.RootElement.TryGetProperty("scripts", out var scriptsProp))
            {
                _console.WriteError("No scripts found in package.json");
                Console.ReadLine();
                return;
            }

            var scripts = scriptsProp.EnumerateObject().Select(p => p.Name).ToList();
            if (scripts.Count == 0)
            {
                _console.WriteError("No scripts found in package.json");
                Console.ReadLine();
                return;
            }

            _console.WriteInfo("Available scripts:");
            foreach (var script in scripts)
                _console.WriteInfo($"- {script}");

            var promptView = new PromptView();
            var scriptName = promptView.PromptWithAutocomplete("Enter script name to run: ", scripts);

            if (string.IsNullOrWhiteSpace(scriptName))
            {
                _console.WriteInfo("Operation cancelled.");
                Console.ReadLine();
                return;
            }

            if (!scripts.Contains(scriptName))
            {
                _console.WriteError("Invalid script name.");
                Console.ReadLine();
                return;
            }

            _console.WriteInfo($"Running npm run {scriptName}...");
            if (RunProcess("npm", $"run {scriptName}"))
                _console.WriteInfo($"✓ Script {scriptName} completed!");
            else
                _console.WriteError($"✗ Script {scriptName} failed.");
        }
        catch (Exception ex)
        {
            _logger.Error($"Error running custom script: {ex.Message}");
            _console.WriteError("Error reading package.json");
        }

        Console.ReadLine();
    }

    private void ShowPackageInfo()
    {
        if (!File.Exists("package.json"))
        {
            _console.WriteError("No package.json found in current directory");
            Console.ReadLine();
            return;
        }

        try
        {
            var json = File.ReadAllText("package.json");
            _console.WriteInfo(json);
        }
        catch (Exception ex)
        {
            _logger.Error($"Error reading package.json: {ex.Message}");
            _console.WriteError("Invalid package.json file");
        }

        Console.ReadLine();
    }

    private void ResetCache()
    {
        var promptView = new PromptView();
        if (promptView.ConfirmYesNo("Are you sure you want to reset the npm cache?", false))
        {
            if (EnsureNpmInstalled() && RunProcess("npm", "cache clean --force"))
                _console.WriteInfo("✓ Cache reset successfully!");
            else
                _console.WriteError("✗ Failed to reset cache.");
        }
        else
        {
            _console.WriteInfo("Cache reset cancelled.");
        }

        Console.ReadLine();
    }
}