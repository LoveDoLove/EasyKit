using System.Diagnostics;
using System.Text.Json;
using EasyKit.Services;

namespace EasyKit.Controllers;

public class ComposerController
{
    private readonly ConfirmationService _confirmation = new();
    private readonly ConsoleService _console;
    private readonly LoggerService _logger;
    private readonly PromptView _prompt = new();
    private readonly Software _software;
    private readonly ProcessService _processService;

    public ComposerController(Software software, LoggerService logger, ConsoleService console)
    {
        _software = software;
        _logger = logger;
        _console = console;
        _processService = new ProcessService(logger, console, console.Config);
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
            .AddOption("9", "Run diagnostics", () => RunDiagnostics())
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
        // First, check if we have a local composer.phar
        if (File.Exists("composer.phar"))
            return "php composer.phar";

        // Next, check if composer.bat exists locally
        if (File.Exists("composer.bat"))
            return "composer.bat";

        // Check if composer.exe exists locally
        if (File.Exists("composer.exe"))
            return "composer.exe";

        // Use our enhanced method to get detailed Composer information
        var (version, path, isGlobal) = _processService.GetComposerInfo();
        if (!string.IsNullOrEmpty(path) && !path.Equals("composer", StringComparison.OrdinalIgnoreCase))
        {
            _logger.Info($"Found Composer {version} at {path} (Global: {isGlobal})");
            return path;
        }

        // Fallback to just "composer" and let the system try to find it
        return "composer";
    }

    private bool RunComposerCommand(string args, bool showOutput = true)
    {
        // First, ensure PHP is installed and compatible
        var (phpVersion, phpPath, phpCompatible) = _processService.GetPhpVersionInfo();

        if (phpVersion == "Unknown" || !phpCompatible)
        {
            _console.WriteError($"PHP is not properly installed or not in PATH. Found version: {phpVersion}");
            _console.WriteInfo("Laravel and Composer require PHP 7.3+ (PHP 8.0+ recommended).");
            _console.WriteInfo("Please install PHP from https://www.php.net/ or https://windows.php.net/download/");
            return false;
        }

        // Check PHP memory limit
        var (hasEnoughMemory, currentLimit, recommendedLimit) = _processService.CheckPhpMemoryLimit(phpPath);
        if (!hasEnoughMemory && showOutput)
        {
            _console.WriteInfo($"PHP memory_limit is currently set to {currentLimit}.");
            _console.WriteInfo($"Composer operations may require more memory. Recommended setting: {recommendedLimit}");
            _console.WriteInfo("Setting optimal environment variables for Composer...");

            // Set recommended environment variables
            _processService.SetRecommendedPhpEnvironmentVariables();
        }

        // Check PHP extensions
        var (missingExtensions, extensionsCompatible) = _processService.CheckPhpExtensions(phpPath);
        if (missingExtensions.Count > 0 && showOutput)
        {
            _console.WriteInfo("Some PHP extensions required by Laravel/Composer might be missing:");
            foreach (var ext in missingExtensions)
            {
                _console.WriteInfo($"  - {ext}");
            }

            if (!extensionsCompatible)
            {
                _console.WriteError("Critical PHP extensions are missing. Composer might not work correctly.");
                _console.WriteInfo("Please enable these extensions in your php.ini file.");
            }
        }

        // Find the composer command
        var composerCmd = FindComposerCommand();
        if (composerCmd == null)
        {
            _console.WriteError("Composer executable not found. Please ensure Composer is installed and in your PATH.");
            _console.WriteInfo("You can install Composer by following instructions at https://getcomposer.org/download/");
            return false;
        }

        // Show diagnostics about the Composer command if requested
        if (showOutput)
        {
            _console.WriteInfo($"Using PHP {phpVersion} from {phpPath}");
            _console.WriteInfo($"Using Composer command: {composerCmd}");
        }
        try
        {
            // Capture the output to parse for common errors
            string output = "";
            string error = "";
            int exitCode = 0;
            bool result = false;

            // If the command is "php composer.phar", we need to handle it differently
            if (composerCmd.StartsWith("php "))
            {
                string phpArgs = composerCmd.Substring(4) + " " + args;
                // Add memory limit for large projects
                phpArgs = "-d memory_limit=-1 " + phpArgs;

                // Run the command with output capture for error handling
                (output, error, exitCode) = _processService.RunProcessWithOutput("php", phpArgs, Environment.CurrentDirectory);

                // Display output if requested
                if (showOutput)
                {
                    if (!string.IsNullOrEmpty(output)) _console.WriteInfo(output);
                    if (!string.IsNullOrEmpty(error)) _console.WriteError(error);
                }

                result = exitCode == 0;
            }
            else
            {
                // For global composer command, run it directly
                (output, error, exitCode) = _processService.RunProcessWithOutput(composerCmd, args, Environment.CurrentDirectory);

                // Display output if requested
                if (showOutput)
                {
                    if (!string.IsNullOrEmpty(output)) _console.WriteInfo(output);
                    if (!string.IsNullOrEmpty(error)) _console.WriteError(error);
                }

                result = exitCode == 0;
            }            // Handle common Composer errors
            if (!result && showOutput)
            {
                string combinedOutput = output + "\n" + error;
                HandleComposerError(combinedOutput);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.Error($"Error running Composer command: {ex.Message}");
            if (showOutput) _console.WriteError($"Error: {ex.Message}");
            HandleComposerError(ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Handles common Composer errors and provides helpful information to the user
    /// </summary>
    /// <param name="errorMessage">The error message from Composer</param>
    private void HandleComposerError(string errorMessage)
    {
        // Common Composer error patterns and solutions
        Dictionary<string, string> errorPatterns = new()
        {
            // Memory limit errors
            {"allowed memory size of", "Composer is running out of memory. Try setting COMPOSER_MEMORY_LIMIT=-1 environment variable."},
            
            // Timeout errors
            {"operation timed out", "Network operation timed out. Check your internet connection or try increasing COMPOSER_PROCESS_TIMEOUT."},
            
            // Package not found errors
            {"could not find a matching version", "The requested package version doesn't exist. Check the package name and version constraint."},
            
            // PHP version errors
            {"requires php", "Your PHP version is not compatible with the package requirements."},
            
            // Extension errors
            {"requires ext-", "You're missing a required PHP extension. Check your php.ini configuration."},
            
            // JSON errors
            {"json parse error", "There's a syntax error in your composer.json file."},
            
            // Authentication errors
            {"authentication required", "Authentication failed. Check your Composer credentials."},
            
            // Permission errors
            {"permission denied", "Permission error. Try running as administrator or check folder permissions."}
        };

        foreach (var (pattern, solution) in errorPatterns)
        {
            if (errorMessage.ToLower().Contains(pattern.ToLower()))
            {
                _console.WriteInfo($"\nPossible solution: {solution}");

                // For memory limit issues, provide more detailed help
                if (pattern == "allowed memory size of")
                {
                    var (hasEnoughMemory, currentLimit, recommendedLimit) = _processService.CheckPhpMemoryLimit();
                    _console.WriteInfo($"Current PHP memory_limit: {currentLimit}");
                    _console.WriteInfo($"Recommended setting: {recommendedLimit}");
                    _console.WriteInfo("You can set this temporarily with: php -d memory_limit=-1 composer.phar [command]");
                    _console.WriteInfo("Or permanently in your php.ini file.");
                }

                // For extension issues, check which extensions are missing
                if (pattern == "requires ext-")
                {
                    var (missingExtensions, _) = _processService.CheckPhpExtensions();
                    if (missingExtensions.Count > 0)
                    {
                        _console.WriteInfo("Missing PHP extensions detected:");
                        foreach (var ext in missingExtensions)
                        {
                            _console.WriteInfo($"  - {ext}");
                        }
                    }
                }

                break; // Only show the first matching solution
            }
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

    private void RunDiagnostics()
    {
        _console.WriteInfo("Running Composer diagnostics...\n");

        // Check PHP version and configuration
        var (phpVersion, phpPath, phpCompatible) = _processService.GetPhpVersionInfo();
        _console.WriteInfo($"PHP Version: {phpVersion} (Compatible: {(phpCompatible ? "Yes" : "No")})");
        _console.WriteInfo($"PHP Path: {phpPath}");

        // Check PHP memory limit
        var (hasEnoughMemory, currentLimit, recommendedLimit) = _processService.CheckPhpMemoryLimit(phpPath);
        _console.WriteInfo($"PHP Memory Limit: {currentLimit} (Recommended: {recommendedLimit}, Sufficient: {(hasEnoughMemory ? "Yes" : "No")})");

        // Check PHP extensions
        var (missingExtensions, extensionsCompatible) = _processService.CheckPhpExtensions(phpPath);
        _console.WriteInfo($"PHP Extensions Status: {(extensionsCompatible ? "All critical extensions present" : "Missing critical extensions")}");
        if (missingExtensions.Count > 0)
        {
            _console.WriteInfo("Missing Extensions:");
            foreach (var ext in missingExtensions)
            {
                _console.WriteInfo($"  - {ext}");
            }
        }

        // Check Composer installation
        var (composerVersion, composerPath, isGlobal) = _processService.GetComposerInfo();
        _console.WriteInfo($"Composer Version: {composerVersion}");
        _console.WriteInfo($"Composer Path: {composerPath}");
        _console.WriteInfo($"Composer Installation Type: {(isGlobal ? "Global" : "Local/Project")}");

        // Check environment variables
        var envRecommendations = _processService.GetPhpEnvironmentRecommendations();
        _console.WriteInfo("\nEnvironment Variables Status:");
        foreach (var (key, (currentValue, recommendedValue, needsUpdate)) in envRecommendations)
        {
            string status = needsUpdate ? "Not Optimal" : "OK";
            _console.WriteInfo($"  - {key}: {(currentValue ?? "Not Set")} (Recommended: {recommendedValue}, Status: {status})");
        }

        // Check composer.json if it exists
        if (File.Exists("composer.json"))
        {
            try
            {
                _console.WriteInfo("\nProject Analysis:");
                var json = File.ReadAllText("composer.json");
                using var jsonDoc = System.Text.Json.JsonDocument.Parse(json);
                var root = jsonDoc.RootElement;

                // Check PHP version requirement
                if (root.TryGetProperty("require", out var requirements) && requirements.TryGetProperty("php", out var phpRequirement))
                {
                    _console.WriteInfo($"  - Required PHP Version: {phpRequirement}");
                    // Check if current PHP version meets requirements
                    bool isPhpCompatible = phpRequirement.ToString().Contains(phpVersion.Split('.')[0]);
                    _console.WriteInfo($"  - PHP Version Compatibility: {(isPhpCompatible ? "Compatible" : "Might not be compatible")}");
                }

                // Get package count
                if (root.TryGetProperty("require", out var reqSection))
                {
                    int packageCount = reqSection.EnumerateObject().Count();
                    if (reqSection.TryGetProperty("php", out _)) packageCount--; // Exclude PHP itself
                    _console.WriteInfo($"  - Production Dependencies: {packageCount}");
                }

                if (root.TryGetProperty("require-dev", out var reqDevSection))
                {
                    int devPackageCount = reqDevSection.EnumerateObject().Count();
                    _console.WriteInfo($"  - Development Dependencies: {devPackageCount}");
                }

                // Run composer validate silently
                var (output, error, exitCode) = _processService.RunProcessWithOutput(
                    FindComposerCommand() ?? "composer", "validate --no-check-publish", Environment.CurrentDirectory);
                _console.WriteInfo($"  - composer.json Validity: {(exitCode == 0 ? "Valid" : "Invalid")}");
                if (exitCode != 0)
                {
                    _console.WriteInfo($"  - Validation Error: {error}");
                }
            }
            catch (Exception ex)
            {
                _console.WriteError($"Error analyzing composer.json: {ex.Message}");
            }
        }

        _console.WriteInfo("\nDiagnostic complete!");
        Console.ReadLine();
    }
}