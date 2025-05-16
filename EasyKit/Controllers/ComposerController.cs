using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace EasyKit.Controllers;

public class ComposerController
{
    private readonly ConfirmationService _confirmation;
    private readonly ConsoleService _console;
    private readonly LoggerService _logger;
    private readonly NotificationView _notificationView;
    private readonly ProcessService _processService;
    private readonly PromptView _prompt;
    private readonly Software _software;

    public ComposerController(
        Software software,
        LoggerService logger,
        ConsoleService console,
        ConfirmationService confirmation,
        PromptView prompt,
        NotificationView notificationView)
    {
        _software = software;
        _logger = logger;
        _console = console;
        _confirmation = confirmation;
        _prompt = prompt;
        _notificationView = notificationView;
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

    // Helper to get the detected composer path
    private string GetComposerPath()
    {
        return _processService.FindExecutablePath("composer") ?? "composer";
    }

    // Helper to get the detected php path
    private string GetPhpPath()
    {
        return _processService.FindExecutablePath("php") ?? "php";
    }

    private string? FindComposerCommand()
    {
        // First, check if we have a local composer.phar
        if (File.Exists("composer.phar"))
            return $"{GetPhpPath()} composer.phar";

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
        return GetComposerPath();
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
            _processService.SetRecommendedPhpEnvironmentVariables();
        }

        // Check PHP extensions
        var (missingExtensions, extensionsCompatible) = _processService.CheckPhpExtensions(phpPath);
        if (missingExtensions.Count > 0 && showOutput)
        {
            _console.WriteError("Some PHP extensions required by Laravel/Composer might be missing:");
            foreach (var ext in missingExtensions) _console.WriteError($"  - {ext}");
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
            _console.WriteInfo(
                "You can install Composer by following instructions at https://getcomposer.org/download/");
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
            string output = "";
            string error = "";
            int exitCode = 0;
            bool result = false;

            if (composerCmd.StartsWith("php "))
            {
                string phpArgs = composerCmd.Substring(4) + " " + args;
                phpArgs = "-d memory_limit=-1 " + phpArgs;
                (output, error, exitCode) =
                    _processService.RunProcessWithOutput(GetPhpPath(), phpArgs, Environment.CurrentDirectory);
                if (showOutput)
                {
                    if (!string.IsNullOrEmpty(output)) _console.WriteInfo(output);
                    if (!string.IsNullOrEmpty(error)) _console.WriteError(error);
                }

                result = exitCode == 0;
            }
            else
            {
                (output, error, exitCode) =
                    _processService.RunProcessWithOutput(composerCmd, args, Environment.CurrentDirectory);
                if (showOutput)
                {
                    if (!string.IsNullOrEmpty(output)) _console.WriteInfo(output);
                    if (!string.IsNullOrEmpty(error)) _console.WriteError(error);
                }

                result = exitCode == 0;
            }

            if (!result && showOutput && !string.IsNullOrEmpty(error)) HandleComposerError(error);
            return result;
        }
        catch (Exception ex)
        {
            if (showOutput)
                _console.WriteError($"Exception: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    ///     Handles common Composer errors and provides helpful information to the user
    /// </summary>
    /// <param name="errorMessage">The error message from Composer</param>
    private void HandleComposerError(string errorMessage)
    {
        // Check for the specific invalid application error for .phar files
        if (errorMessage.Contains("not a valid application for this OS platform") &&
            errorMessage.Contains("composer.phar"))
        {
            Console.WriteLine("[ERROR] Composer PHAR file cannot be executed directly on Windows.");
            Console.WriteLine("This could be fixed by either:");
            Console.WriteLine("1. Running composer.phar with PHP: php composer.phar [command]");
            Console.WriteLine("2. Using Composer installer for Windows to get composer.bat");
            Console.WriteLine("3. Downloading Composer from https://getcomposer.org/download/");

            // Check if we have PHP available
            var (phpVersion, phpPath, _) = _processService.GetPhpVersionInfo();
            if (phpVersion != "Unknown" && File.Exists(phpPath))
            {
                Console.WriteLine("\nAttempting to run with PHP instead...");
                var match = Regex.Match(errorMessage, @"'([^']*composer\\.phar)'");
                if (match.Success)
                {
                    string composerPharPath = match.Groups[1].Value;
                    Console.WriteLine($"Using PHP to execute: {composerPharPath}");
                    _console.Config.Set("composer_path", $"php {composerPharPath}");
                }
            }

            return;
        }

        Dictionary<string, string> errorPatterns = new()
        {
            {
                "allowed memory size of",
                "Composer is running out of memory. Try setting COMPOSER_MEMORY_LIMIT=-1 environment variable."
            },
            {
                "operation timed out",
                "Network operation timed out. Check your internet connection or try increasing COMPOSER_PROCESS_TIMEOUT."
            },
            {
                "could not find a matching version",
                "The requested package version doesn't exist. Check the package name and version constraint."
            },
            { "requires php", "Your PHP version is not compatible with the package requirements." },
            { "requires ext-", "You're missing a required PHP extension. Check your php.ini configuration." },
            { "json parse error", "There's a syntax error in your composer.json file." },
            { "authentication required", "Authentication failed. Check your Composer credentials." },
            { "permission denied", "Permission error. Try running as administrator or check folder permissions." }
        };

        foreach (var (pattern, solution) in errorPatterns)
            if (errorMessage.ToLower().Contains(pattern.ToLower()))
            {
                Console.WriteLine($"\nPossible solution: {solution}");
                if (pattern == "allowed memory size of")
                {
                    var (hasEnoughMemory, currentLimit, recommendedLimit) = _processService.CheckPhpMemoryLimit();
                    Console.WriteLine($"Current PHP memory_limit: {currentLimit}");
                    Console.WriteLine($"Recommended setting: {recommendedLimit}");
                    Console.WriteLine(
                        "You can set this temporarily with: php -d memory_limit=-1 composer.phar [command]");
                    Console.WriteLine("Or permanently in your php.ini file.");
                }

                if (pattern == "requires ext-")
                {
                    var (missingExtensions, _) = _processService.CheckPhpExtensions();
                    if (missingExtensions.Count > 0)
                    {
                        Console.WriteLine("Missing PHP extensions detected:");
                        foreach (var ext in missingExtensions) Console.WriteLine($"  - {ext}");
                    }
                }

                break;
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
            _console.WriteError("No composer.json found in current directory");
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
            _console.WriteError("No composer.json found in current directory");
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
        var sb = new StringBuilder();
        sb.AppendLine("===== COMPOSER Configuration Diagnostics =====\n");

        // Step 1: Check if Composer is accessible
        sb.AppendLine("Step 1: Checking if Composer is accessible in PATH or via detected path");
        var (composerVersion, composerPath, isGlobal) = _processService.GetComposerInfo();
        var (composerOut, composerErr, composerExit) =
            _processService.RunProcessWithOutput(composerPath, "--version", Environment.CurrentDirectory);
        if (composerExit == 0 && !string.IsNullOrWhiteSpace(composerOut))
        {
            sb.AppendLine($"[OK] Composer is accessible. Version: {composerOut.Trim()}");
            sb.AppendLine($"Detected Composer path: {composerPath}");
            sb.AppendLine($"Installation Type: {(isGlobal ? "Global" : "Local/Project")}");
        }
        else
        {
            sb.AppendLine("[ERROR] Composer is not accessible via PATH or detected path.");
            sb.AppendLine(
                "Please install Composer from https://getcomposer.org/download/ and ensure it is in your PATH.");
            sb.AppendLine("You can also use the Tool Marketplace in EasyKit to open the download page.");
            sb.AppendLine("\n===== End of COMPOSER Configuration Diagnostics =====");
            Console.WriteLine(sb.ToString());
            Console.ReadLine();
            return;
        }

        // Step 2: Check PHP version and compatibility
        sb.AppendLine("\nStep 2: Checking PHP version and compatibility");
        var (phpVersion, phpPath, phpCompatible) = _processService.GetPhpVersionInfo();
        sb.AppendLine($"PHP Version: {phpVersion} (Compatible: {(phpCompatible ? "Yes" : "No")})");
        sb.AppendLine($"PHP Path: {phpPath}");
        if (!phpCompatible)
            sb.AppendLine("[ERROR] PHP version may not be compatible with Composer. Composer requires PHP 7.3+.");
        else
            sb.AppendLine("[OK] PHP version is compatible.");

        // Step 3: Check PHP extensions
        sb.AppendLine("\nStep 3: Checking required PHP extensions");
        var (missingExtensions, extensionsCompatible) = _processService.CheckPhpExtensions(phpPath);
        if (extensionsCompatible)
            sb.AppendLine("[OK] All critical PHP extensions are present.");
        else
            sb.AppendLine("[ERROR] Missing critical PHP extensions required for Composer.");
        if (missingExtensions.Count > 0)
        {
            sb.AppendLine("Missing extensions:");
            foreach (var ext in missingExtensions) sb.AppendLine($"  - {ext}");
        }

        // Step 4: Check PHP memory limit
        sb.AppendLine("\nStep 4: Checking PHP memory limit");
        var (hasEnoughMemory, currentLimit, recommendedLimit) = _processService.CheckPhpMemoryLimit(phpPath);
        sb.AppendLine($"Current memory_limit: {currentLimit} (Recommended: {recommendedLimit})");
        if (hasEnoughMemory)
            sb.AppendLine("[OK] PHP memory limit is sufficient for Composer operations.");
        else
            sb.AppendLine(
                "[WARNING] PHP memory limit may be too low for Composer. Set memory_limit to -1 or at least 1536M.");

        // Step 5: Show environment variable recommendations
        sb.AppendLine("\nStep 5: Environment variable recommendations");
        var envRecommendations = _processService.GetPhpEnvironmentRecommendations();
        foreach (var (key, (currentValue, recommendedValue, needsUpdate)) in envRecommendations)
        {
            string status = needsUpdate ? "Not Optimal" : "OK";
            sb.AppendLine(
                $"  - {key}: {currentValue ?? "Not Set"} (Recommended: {recommendedValue}, Status: {status})");
        }

        // Step 6: Validate composer.json
        sb.AppendLine("\nStep 6: Validating composer.json");
        if (File.Exists("composer.json"))
        {
            var (output, error, exitCode) = _processService.RunProcessWithOutput(
                FindComposerCommand() ?? GetComposerPath(), "validate --no-check-publish",
                Environment.CurrentDirectory);
            sb.AppendLine($"composer.json Validity: {(exitCode == 0 ? "Valid" : "Invalid")}");
            if (exitCode != 0) sb.AppendLine($"Validation Error: {error}");
        }
        else
        {
            sb.AppendLine("No composer.json found in current directory.");
        }

        // Step 7: Project analysis (dependencies)
        sb.AppendLine("\nStep 7: Project analysis (dependencies)");
        if (File.Exists("composer.json"))
            try
            {
                var json = File.ReadAllText("composer.json");
                using var jsonDoc = JsonDocument.Parse(json);
                var root = jsonDoc.RootElement;
                if (root.TryGetProperty("require", out var reqSection))
                {
                    int packageCount = reqSection.EnumerateObject().Count();
                    if (reqSection.TryGetProperty("php", out _)) packageCount--; // Exclude PHP itself
                    sb.AppendLine($"Production Dependencies: {packageCount}");
                }

                if (root.TryGetProperty("require-dev", out var reqDevSection))
                {
                    int devPackageCount = reqDevSection.EnumerateObject().Count();
                    sb.AppendLine($"Development Dependencies: {devPackageCount}");
                }

                if (root.TryGetProperty("require", out var requirements) &&
                    requirements.TryGetProperty("php", out var phpRequirement))
                {
                    sb.AppendLine($"Required PHP Version: {phpRequirement}");
                    bool isPhpCompatible = phpRequirement.ToString().Contains(phpVersion.Split('.')[0]);
                    sb.AppendLine(
                        $"PHP Version Compatibility: {(isPhpCompatible ? "Compatible" : "Might not be compatible")}");
                }
            }
            catch (Exception ex)
            {
                sb.AppendLine($"Error analyzing composer.json: {ex.Message}");
            }

        // Step 8: Recommendations
        sb.AppendLine("\nStep 8: Recommendations");
        sb.AppendLine("- If you encounter issues, ensure Composer and PHP are installed and in your PATH.");
        sb.AppendLine("- Enable all required extensions in your php.ini file.");
        sb.AppendLine("- Set memory_limit to -1 for Composer-heavy operations.");
        sb.AppendLine("- Restart your terminal or EasyKit after making changes.");
        sb.AppendLine("- Use the Tool Marketplace in EasyKit to check installation status or open the download page.");

        sb.AppendLine("\n===== End of COMPOSER Configuration Diagnostics =====");
        Console.WriteLine(sb.ToString());
        Console.ReadLine();
    }
}