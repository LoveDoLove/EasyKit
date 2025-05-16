using System.Text.Json;
using System.Text.RegularExpressions;
using EasyKit.Views;

namespace EasyKit.Controllers;

public class ComposerController
{
    private readonly ConfirmationService _confirmation;
    private readonly ConsoleService _console;
    private readonly LoggerService _logger;
    private readonly ProcessService _processService;
    private readonly PromptView _prompt;
    private readonly Software _software;
    private readonly NotificationView _notificationView;

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
    private string GetComposerPath() => _processService.FindExecutablePath("composer") ?? "composer";
    // Helper to get the detected php path
    private string GetPhpPath() => _processService.FindExecutablePath("php") ?? "php";

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
            NotificationView.Show($"PHP is not properly installed or not in PATH. Found version: {phpVersion}", NotificationView.NotificationType.Error);
            NotificationView.Show("Laravel and Composer require PHP 7.3+ (PHP 8.0+ recommended).", NotificationView.NotificationType.Info);
            NotificationView.Show("Please install PHP from https://www.php.net/ or https://windows.php.net/download/", NotificationView.NotificationType.Info);
            return false;
        }

        // Check PHP memory limit
        var (hasEnoughMemory, currentLimit, recommendedLimit) = _processService.CheckPhpMemoryLimit(phpPath);
        if (!hasEnoughMemory && showOutput)
        {
            NotificationView.Show($"PHP memory_limit is currently set to {currentLimit}.", NotificationView.NotificationType.Info);
            NotificationView.Show($"Composer operations may require more memory. Recommended setting: {recommendedLimit}", NotificationView.NotificationType.Info);
            NotificationView.Show("Setting optimal environment variables for Composer...", NotificationView.NotificationType.Info);

            // Set recommended environment variables
            _processService.SetRecommendedPhpEnvironmentVariables();
        }

        // Check PHP extensions
        var (missingExtensions, extensionsCompatible) = _processService.CheckPhpExtensions(phpPath);
        if (missingExtensions.Count > 0 && showOutput)
        {
            NotificationView.Show("Some PHP extensions required by Laravel/Composer might be missing:", NotificationView.NotificationType.Warning);
            foreach (var ext in missingExtensions) NotificationView.Show($"  - {ext}", NotificationView.NotificationType.Warning);

            if (!extensionsCompatible)
            {
                NotificationView.Show("Critical PHP extensions are missing. Composer might not work correctly.", NotificationView.NotificationType.Error);
                NotificationView.Show("Please enable these extensions in your php.ini file.", NotificationView.NotificationType.Info);
            }
        }

        // Find the composer command
        var composerCmd = FindComposerCommand();
        if (composerCmd == null)
        {
            NotificationView.Show("Composer executable not found. Please ensure Composer is installed and in your PATH.", NotificationView.NotificationType.Error);
            NotificationView.Show("You can install Composer by following instructions at https://getcomposer.org/download/", NotificationView.NotificationType.Info);
            return false;
        }

        // Show diagnostics about the Composer command if requested
        if (showOutput)
        {
            NotificationView.Show($"Using PHP {phpVersion} from {phpPath}", NotificationView.NotificationType.Info);
            NotificationView.Show($"Using Composer command: {composerCmd}", NotificationView.NotificationType.Info);
        }

        try
        {
            string output = "";
            string error = "";
            int exitCode = 0;
            bool result = false;

            // If the command is "php composer.phar" or any path to a .phar file prefixed with php, 
            // we need to handle it differently
            if (composerCmd.StartsWith("php "))
            {
                string phpArgs = composerCmd.Substring(4) + " " + args;
                // Add memory limit for large projects
                phpArgs = "-d memory_limit=-1 " + phpArgs;

                // Run the command with output capture for error handling
                (output, error, exitCode) =
                    _processService.RunProcessWithOutput(GetPhpPath(), phpArgs, Environment.CurrentDirectory);

                // Display output if requested
                if (showOutput)
                {
                    if (!string.IsNullOrEmpty(output)) NotificationView.Show(output, NotificationView.NotificationType.Info);
                    if (!string.IsNullOrEmpty(error)) NotificationView.Show(error, NotificationView.NotificationType.Error);
                }

                result = exitCode == 0;
            }
            else
            {
                // For global composer command, run it directly
                (output, error, exitCode) =
                    _processService.RunProcessWithOutput(composerCmd, args, Environment.CurrentDirectory);

                // Display output if requested
                if (showOutput)
                {
                    if (!string.IsNullOrEmpty(output)) NotificationView.Show(output, NotificationView.NotificationType.Info);
                    if (!string.IsNullOrEmpty(error)) NotificationView.Show(error, NotificationView.NotificationType.Error);
                }

                result = exitCode == 0;
            }

            // Handle common Composer errors
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
            if (showOutput) NotificationView.Show($"Error: {ex.Message}", NotificationView.NotificationType.Error);
            HandleComposerError(ex.Message);
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
            NotificationView.Show("Error: Composer PHAR file cannot be executed directly on Windows.", NotificationView.NotificationType.Error);
            NotificationView.Show("This could be fixed by either:", NotificationView.NotificationType.Info);
            NotificationView.Show("1. Running composer.phar with PHP: php composer.phar [command]", NotificationView.NotificationType.Info);
            NotificationView.Show("2. Using Composer installer for Windows to get composer.bat", NotificationView.NotificationType.Info);
            NotificationView.Show("3. Downloading Composer from https://getcomposer.org/download/", NotificationView.NotificationType.Info);

            // Check if we have PHP available
            var (phpVersion, phpPath, _) = _processService.GetPhpVersionInfo();
            if (phpVersion != "Unknown" && File.Exists(phpPath))
            {
                NotificationView.Show("\nAttempting to run with PHP instead...", NotificationView.NotificationType.Info);

                // Extract the composer.phar path from the error message
                var match = Regex.Match(errorMessage, @"'([^']*composer\.phar)'");
                if (match.Success)
                {
                    string composerPharPath = match.Groups[1].Value;
                    NotificationView.Show($"Using PHP to execute: {composerPharPath}", NotificationView.NotificationType.Info);
                    // Store the corrected path for future use
                    _console.Config.Set("composer_path", $"php {composerPharPath}");
                }
            }

            return;
        }

        // Common Composer error patterns and solutions
        Dictionary<string, string> errorPatterns = new()
        {
            // Memory limit errors
            {
                "allowed memory size of",
                "Composer is running out of memory. Try setting COMPOSER_MEMORY_LIMIT=-1 environment variable."
            },

            // Timeout errors
            {
                "operation timed out",
                "Network operation timed out. Check your internet connection or try increasing COMPOSER_PROCESS_TIMEOUT."
            },

            // Package not found errors
            {
                "could not find a matching version",
                "The requested package version doesn't exist. Check the package name and version constraint."
            },

            // PHP version errors
            { "requires php", "Your PHP version is not compatible with the package requirements." },

            // Extension errors
            { "requires ext-", "You're missing a required PHP extension. Check your php.ini configuration." },

            // JSON errors
            { "json parse error", "There's a syntax error in your composer.json file." },

            // Authentication errors
            { "authentication required", "Authentication failed. Check your Composer credentials." },

            // Permission errors
            { "permission denied", "Permission error. Try running as administrator or check folder permissions." }
        };

        foreach (var (pattern, solution) in errorPatterns)
            if (errorMessage.ToLower().Contains(pattern.ToLower()))
            {
                NotificationView.Show($"\nPossible solution: {solution}", NotificationView.NotificationType.Info);

                // For memory limit issues, provide more detailed help
                if (pattern == "allowed memory size of")
                {
                    var (hasEnoughMemory, currentLimit, recommendedLimit) = _processService.CheckPhpMemoryLimit();
                    NotificationView.Show($"Current PHP memory_limit: {currentLimit}", NotificationView.NotificationType.Info);
                    NotificationView.Show($"Recommended setting: {recommendedLimit}", NotificationView.NotificationType.Info);
                    NotificationView.Show("You can set this temporarily with: php -d memory_limit=-1 composer.phar [command]", NotificationView.NotificationType.Info);
                    NotificationView.Show("Or permanently in your php.ini file.", NotificationView.NotificationType.Info);
                }

                // For extension issues, check which extensions are missing
                if (pattern == "requires ext-")
                {
                    var (missingExtensions, _) = _processService.CheckPhpExtensions();
                    if (missingExtensions.Count > 0)
                    {
                        NotificationView.Show("Missing PHP extensions detected:", NotificationView.NotificationType.Warning);
                        foreach (var ext in missingExtensions) NotificationView.Show($"  - {ext}", NotificationView.NotificationType.Warning);
                    }
                }

                break; // Only show the first matching solution
            }
    }

    private void InstallPackages()
    {
        NotificationView.Show("Installing Composer packages...", NotificationView.NotificationType.Info);
        if (RunComposerCommand("install"))
            NotificationView.Show("✓ Packages installed successfully!", NotificationView.NotificationType.Success);
        else
            NotificationView.Show("✗ Failed to install packages.", NotificationView.NotificationType.Error);
        Console.ReadLine();
    }

    private void UpdatePackages()
    {
        NotificationView.Show("Updating Composer packages...", NotificationView.NotificationType.Info);
        if (RunComposerCommand("update"))
            NotificationView.Show("✓ Packages updated successfully!", NotificationView.NotificationType.Success);
        else
            NotificationView.Show("✗ Failed to update packages.", NotificationView.NotificationType.Error);
        Console.ReadLine();
    }

    private void RegenerateAutoload()
    {
        NotificationView.Show("Regenerating autoload files...", NotificationView.NotificationType.Info);
        if (RunComposerCommand("dump-autoload"))
            NotificationView.Show("✓ Autoload files regenerated!", NotificationView.NotificationType.Success);
        else
            NotificationView.Show("✗ Failed to regenerate autoload files.", NotificationView.NotificationType.Error);
        Console.ReadLine();
    }

    private void RequirePackage()
    {
        var package = _prompt.Prompt("Enter package name (e.g. 'vendor/package'): ");
        var dev = _confirmation.ConfirmAction("Is this a development dependency?", false);
        var args = dev ? $"require --dev {package}" : $"require {package}";
        NotificationView.Show($"Installing {package}...", NotificationView.NotificationType.Info);
        if (RunComposerCommand(args))
            NotificationView.Show($"✓ Package {package} installed successfully!", NotificationView.NotificationType.Success);
        else
            NotificationView.Show($"✗ Failed to install {package}.", NotificationView.NotificationType.Error);
        Console.ReadLine();
    }

    private void CreateProject()
    {
        var package = _prompt.Prompt("Enter project package (e.g. 'laravel/laravel'): ");
        var directory = _prompt.Prompt("Enter project directory name: ");
        NotificationView.Show($"Creating new project from {package}...", NotificationView.NotificationType.Info);
        if (RunComposerCommand($"create-project {package} {directory}"))
            NotificationView.Show($"✓ Project created successfully in {directory}!", NotificationView.NotificationType.Success);
        else
            NotificationView.Show("✗ Failed to create project.", NotificationView.NotificationType.Error);
        Console.ReadLine();
    }

    private void ValidateJson()
    {
        if (!File.Exists("composer.json"))
        {
            NotificationView.Show("No composer.json found in current directory", NotificationView.NotificationType.Info);
            Console.ReadLine();
            return;
        }

        NotificationView.Show("Validating composer.json...", NotificationView.NotificationType.Info);
        if (RunComposerCommand("validate"))
            NotificationView.Show("✓ composer.json is valid!", NotificationView.NotificationType.Success);
        else
            NotificationView.Show("✗ composer.json validation failed.", NotificationView.NotificationType.Error);
        Console.ReadLine();
    }

    private void ClearCache()
    {
        if (_confirmation.ConfirmAction("Are you sure you want to clear Composer cache?", false))
        {
            NotificationView.Show("Clearing Composer cache...", NotificationView.NotificationType.Info);
            if (RunComposerCommand("clear-cache"))
                NotificationView.Show("✓ Cache cleared successfully!", NotificationView.NotificationType.Success);
            else
                NotificationView.Show("✗ Failed to clear cache.", NotificationView.NotificationType.Error);
        }
        else
        {
            NotificationView.Show("Cancelled.", NotificationView.NotificationType.Info);
        }

        Console.ReadLine();
    }

    private void ShowPackageInfo()
    {
        if (!File.Exists("composer.json"))
        {
            NotificationView.Show("No composer.json found in current directory", NotificationView.NotificationType.Info);
            Console.ReadLine();
            return;
        }

        try
        {
            var json = File.ReadAllText("composer.json");
            var doc = JsonDocument.Parse(json);
            NotificationView.Show(JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented = true }), NotificationView.NotificationType.Info);
        }
        catch (Exception ex)
        {
            _logger.Error($"Error reading composer.json: {ex.Message}");
            NotificationView.Show("Invalid composer.json file", NotificationView.NotificationType.Error);
        }

        Console.ReadLine();
    }

    private void RunDiagnostics()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("===== COMPOSER Configuration Diagnostics =====\n");

        // Step 1: Check if Composer is accessible
        sb.AppendLine("Step 1: Checking if Composer is accessible in PATH or via detected path");
        var (composerVersion, composerPath, isGlobal) = _processService.GetComposerInfo();
        var (composerOut, composerErr, composerExit) = _processService.RunProcessWithOutput(composerPath, "--version", Environment.CurrentDirectory);
        if (composerExit == 0 && !string.IsNullOrWhiteSpace(composerOut))
        {
            sb.AppendLine($"[OK] Composer is accessible. Version: {composerOut.Trim()}");
            sb.AppendLine($"Detected Composer path: {composerPath}");
            sb.AppendLine($"Installation Type: {(isGlobal ? "Global" : "Local/Project")}");
        }
        else
        {
            sb.AppendLine("[ERROR] Composer is not accessible via PATH or detected path.");
            sb.AppendLine("Please install Composer from https://getcomposer.org/download/ and ensure it is in your PATH.");
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
            sb.AppendLine("[WARNING] PHP memory limit may be too low for Composer. Set memory_limit to -1 or at least 1536M.");

        // Step 5: Show environment variable recommendations
        sb.AppendLine("\nStep 5: Environment variable recommendations");
        var envRecommendations = _processService.GetPhpEnvironmentRecommendations();
        foreach (var (key, (currentValue, recommendedValue, needsUpdate)) in envRecommendations)
        {
            string status = needsUpdate ? "Not Optimal" : "OK";
            sb.AppendLine($"  - {key}: {currentValue ?? "Not Set"} (Recommended: {recommendedValue}, Status: {status})");
        }

        // Step 6: Validate composer.json
        sb.AppendLine("\nStep 6: Validating composer.json");
        if (File.Exists("composer.json"))
        {
            var (output, error, exitCode) = _processService.RunProcessWithOutput(
                FindComposerCommand() ?? GetComposerPath(), "validate --no-check-publish", Environment.CurrentDirectory);
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
        {
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
                if (root.TryGetProperty("require", out var requirements) && requirements.TryGetProperty("php", out var phpRequirement))
                {
                    sb.AppendLine($"Required PHP Version: {phpRequirement}");
                    bool isPhpCompatible = phpRequirement.ToString().Contains(phpVersion.Split('.')[0]);
                    sb.AppendLine($"PHP Version Compatibility: {(isPhpCompatible ? "Compatible" : "Might not be compatible")}");
                }
            }
            catch (Exception ex)
            {
                sb.AppendLine($"Error analyzing composer.json: {ex.Message}");
            }
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