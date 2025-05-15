using System.Diagnostics;

namespace EasyKit.Controllers;

public class LaravelController
{
    private readonly ConfirmationService _confirmation = new();
    private readonly ConsoleService _console;
    private readonly LoggerService _logger;
    private readonly ProcessService _processService;
    private readonly Software _software;

    public LaravelController(Software software, LoggerService logger, ConsoleService console)
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
        string colorSchemeStr = "dark";

        // Try to get user preferences from config
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

        // Display current directory
        string currentDirectory = Environment.CurrentDirectory;

        // Create and configure the menu with a Laravel-specific theme
        var menuView = new MenuView();
        menuView.CreateMenu("Laravel Toolkit", width: menuWidth)
            .WithSubtitle($"Current Directory: {currentDirectory}")
            .AddOption("1", "Quick Setup (env, install, key, cache)", () => QuickSetup())
            .AddOption("2", "Install Composer Packages", () => InstallPackages())
            .AddOption("3", "Update Composer Packages", () => UpdatePackages())
            .AddOption("4", "Regenerate Autoload Files", () => RegenerateAutoload())
            .AddOption("5", "Build for Production", () => BuildProduction())
            .AddOption("6", "Start Development Server", () => RunDevServer())
            .AddOption("7", "Create Storage Link", () => CreateStorageLink())
            .AddOption("8", "Run Database Seeding (migrate:fresh --seed)", () => RunDatabaseSeeding())
            .AddOption("9", "Test Database Connection", () => TestDatabase())
            .AddOption("10", "Check PHP Version", () => CheckPhpVersion())
            .AddOption("11", "Check Laravel Configuration", () => CheckConfiguration())
            .AddOption("12", "Reset All Laravel Cache", () => ResetCache())
            .AddOption("13", "View Route List", () => ViewRouteList())
            .AddOption("0", "Back to Main Menu", () =>
            {
                /* Return to main menu */
            })
            .WithColors(ConsoleColor.DarkRed, ConsoleColor.Red, ConsoleColor.White)
            .WithHelpText("Select an option or press 0 to return to the main menu")
            .WithDoubleBorder()
            .Show();
    }

    private bool EnsurePhpInstalled()
    {
        var (phpVersion, phpPath, isCompatible) = _processService.GetPhpVersionInfo();

        if (phpVersion == "Unknown" || !isCompatible)
        {
            _console.WriteError("PHP is not properly installed or not compatible with Laravel.");
            _console.WriteInfo($"Found PHP version: {phpVersion}");
            _console.WriteInfo("Laravel requires PHP 7.3+ (PHP 8.0+ recommended for newer Laravel versions).");
            _console.WriteInfo("Please install PHP from https://www.php.net/ or https://windows.php.net/download/");
            Console.ReadLine();
            return false;
        }

        // Check PHP extensions required for Laravel
        var (missingExtensions, extensionsCompatible) = _processService.CheckPhpExtensions(phpPath);
        if (missingExtensions.Count > 0)
        {
            _console.WriteInfo("Some PHP extensions required by Laravel might be missing:");
            foreach (var ext in missingExtensions) _console.WriteInfo($"  - {ext}");

            if (!extensionsCompatible)
            {
                _console.WriteError("Critical PHP extensions are missing. Laravel might not work correctly.");
                _console.WriteInfo("Please enable these extensions in your php.ini file.");
                Console.ReadLine();
                return false;
            }
        }

        _console.WriteInfo($"Using PHP {phpVersion} from {phpPath}");
        return true;
    }

    private string? FindComposerCommand()
    {
        // Get detailed Composer information
        var (composerVersion, composerPath, isGlobal) = _processService.GetComposerInfo();

        if (!string.IsNullOrEmpty(composerPath) && composerPath != "composer")
        {
            _console.WriteInfo($"Using Composer {composerVersion} from {composerPath}");
            return composerPath;
        }

        // Fallback to checking local files
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
        if (!EnsurePhpInstalled()) return false;

        var composerCmd = FindComposerCommand();
        if (composerCmd == null)
        {
            _console.WriteError("Composer executable not found. Please ensure Composer is installed and in your PATH.");
            return false;
        }

        try
        {
            // If the command is "php composer.phar", we need to handle it differently
            if (composerCmd.StartsWith("php "))
            {
                string phpArgs = composerCmd.Substring(4) + " " + args;
                // Add memory limit for large projects
                phpArgs = "-d memory_limit=-1 " + phpArgs;
                return _processService.RunProcess("php", phpArgs, showOutput, Environment.CurrentDirectory);
            }

            // For global composer installation
            return _processService.RunProcess(composerCmd, args, showOutput, Environment.CurrentDirectory);
        }
        catch (Exception ex)
        {
            _logger.Error($"Error running Composer command: {ex.Message}");
            if (showOutput) _console.WriteError($"Error: {ex.Message}");
            return false;
        }
    }

    private bool RunArtisanCommand(string args, bool showOutput = true)
    {
        if (!EnsurePhpInstalled()) return false;

        if (!File.Exists("artisan"))
        {
            _console.WriteError(
                "This doesn't appear to be a Laravel project directory. Make sure you're in the root directory of a Laravel project.");
            return false;
        }

        try
        {
            // Get Laravel version info
            var (laravelVersion, isCompatible) = _processService.GetLaravelVersionInfo();

            if (showOutput && laravelVersion != "Unknown")
            {
                _console.WriteInfo($"Laravel version: {laravelVersion}");

                if (!isCompatible)
                    _console.WriteInfo("Warning: Your Laravel version might not be fully supported by EasyKit.");
            }

            // Add PHP config options for better performance
            string phpOptions = _processService.GetPhpConfigOptions(new Dictionary<string, string>
            {
                ["memory_limit"] = "-1",
                ["max_execution_time"] = "0",
                ["display_errors"] = "On"
            });

            // Set environment variables for optimal Laravel performance
            _processService.SetRecommendedPhpEnvironmentVariables();

            // Run Artisan command with config options
            bool result = _processService.RunProcess("php", $"{phpOptions} artisan {args}", showOutput);

            // Check for common error patterns in output even when exit code is 0
            if (!result && showOutput)
            {
                HandleLaravelError(args);

                // Provide suggestions based on common issues
                if (args.Contains("cache:status") && result == false)
                {
                    _console.WriteInfo("Note: 'cache:status' command may not be available in all Laravel versions.");
                    _console.WriteInfo("Try using 'php artisan list cache' to see available cache commands.");
                }
                else if (args.Contains("--compact") && result == false)
                {
                    _console.WriteInfo("Note: The '--compact' option may not be available in your Laravel version.");
                    _console.WriteInfo("Try using 'php artisan route:list' without additional options.");
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.Error($"Error running Artisan command: {ex.Message}");
            if (showOutput) _console.WriteError($"Error: {ex.Message}");
            return false;
        }
    }

    private void QuickSetup()
    {
        if (!File.Exists("artisan"))
        {
            _console.WriteError("This doesn't appear to be a Laravel project directory.");
            Console.ReadLine();
            return;
        }

        if (!File.Exists(".env"))
        {
            if (File.Exists(".env.example"))
            {
                File.Copy(".env.example", ".env");
                _console.WriteSuccess("✓ Created .env file");
            }
            else
            {
                _console.WriteError("✗ .env.example file not found");
                Console.ReadLine();
                return;
            }
        }

        if (RunComposerCommand("install", false))
        {
            _console.WriteSuccess("✓ Installed Composer dependencies");
        }
        else
        {
            _console.WriteError("✗ Failed to install dependencies");
            Console.ReadLine();
            return;
        }

        if (RunArtisanCommand("key:generate", false))
            _console.WriteSuccess("✓ Generated application key");
        else
            _console.WriteError("✗ Failed to generate key");
        RunArtisanCommand("config:clear", false);
        RunArtisanCommand("cache:clear", false);
        _console.WriteSuccess("✓ Cleared configuration and cache");
        _console.WriteSuccess("\nSetup completed successfully!");
        Console.ReadLine();
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

    private void BuildProduction()
    {
        _console.WriteInfo("Building for production...");

        // Check Laravel version to determine appropriate optimization commands
        bool isLaravel11Plus = false;

        var versionProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "php",
                Arguments = "artisan --version",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        try
        {
            versionProcess.Start();
            string versionOutput = versionProcess.StandardOutput.ReadToEnd();
            versionProcess.WaitForExit();

            // Check if Laravel 11+
            if (versionOutput.Contains("Laravel Framework") &&
                (versionOutput.Contains("11.") || versionOutput.Contains("12.")))
                isLaravel11Plus = true;
        }
        catch
        {
            // If version check fails, default to backward compatible commands
            isLaravel11Plus = false;
        }

        // Define base steps that work across versions
        var steps = new List<(string message, string command)>
        {
            ("Installing production dependencies...", "install --no-dev")
        };

        // Add version-specific optimization steps
        if (isLaravel11Plus)
        {
            // Laravel 11+ optimization steps
            steps.Add(("Optimizing application...", "artisan optimize"));
        }
        else
        {
            // Legacy optimization steps for Laravel 10 and below
            steps.Add(("Optimizing configuration...", "artisan config:cache"));
            steps.Add(("Optimizing routes...", "artisan route:cache"));
            steps.Add(("Optimizing views...", "artisan view:cache"));
        }

        // Execute all steps
        foreach (var (msg, cmd) in steps)
        {
            _console.WriteInfo(msg);
            bool success = cmd.StartsWith("artisan ")
                ? RunArtisanCommand(cmd.Substring(8), false)
                : RunComposerCommand(cmd, false);
            if (!success)
            {
                _console.WriteError($"✗ {msg} failed");
                Console.ReadLine();
                return;
            }
        }

        _console.WriteSuccess("✓ Production build completed!");
        Console.ReadLine();
    }

    private void RunDevServer()
    {
        _console.WriteInfo("Starting development server...");
        if (!EnsurePhpInstalled())
        {
            Console.ReadLine();
            return;
        }

        if (!File.Exists("artisan"))
        {
            _console.WriteError("This doesn't appear to be a Laravel project directory.");
            Console.ReadLine();
            return;
        }

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/c start cmd /k \"php artisan serve\"",
                UseShellExecute = true
            });
            _console.WriteInfo(
                "✓ Development server started in a new terminal window. Press Ctrl+C in that window to stop the server.");
        }
        catch (Exception ex)
        {
            _logger.Error($"Error starting dev server: {ex.Message}");
            _console.WriteError("✗ Failed to start development server.");
        }

        Console.ReadLine();
    }

    private void CreateStorageLink()
    {
        _console.WriteInfo("Creating storage symbolic link...");
        if (RunArtisanCommand("storage:link"))
            _console.WriteSuccess("✓ Storage link created!");
        else
            _console.WriteError("✗ Failed to create storage link.");
        Console.ReadLine();
    }

    private void RunDatabaseSeeding()
    {
        if (_confirmation.ConfirmAction("This will refresh your database. Are you sure?", false))
        {
            _console.WriteInfo("Running database seeding...");
            if (RunArtisanCommand("migrate:fresh --seed"))
                _console.WriteSuccess("✓ Database seeded successfully!");
            else
                _console.WriteError("✗ Failed to seed database.");
        }
        else
        {
            _console.WriteInfo("Cancelled.");
        }

        Console.ReadLine();
    }

    private void TestDatabase()
    {
        _console.WriteInfo("Testing database connection...");
        // Try db:show first (Laravel 9+)
        bool success = RunArtisanCommand("db:show", false);

        // If db:show fails, fall back to older methods
        if (!success)
        {
            // For older Laravel versions
            success = RunArtisanCommand("db:monitor", false);

            // Last resort - simple connection test
            if (!success)
            {
                _console.WriteInfo("Trying alternative connection test...");
                success = RunArtisanCommand("migrate:status");
            }
        }

        if (success)
            _console.WriteSuccess("✓ Database connection successful!");
        else
            _console.WriteError("✗ Database connection failed.");

        Console.ReadLine();
    }

    private void CheckPhpVersion()
    {
        _console.WriteInfo("Checking PHP version...");
        _processService.RunProcess("php", "--version");
        Console.ReadLine();
    }

    private void CheckConfiguration()
    {
        _console.WriteInfo("Checking Laravel configuration...");
        var checks = new[]
        {
            ("PHP Version", "php --version"),
            ("Laravel Version", "php artisan --version"),
            ("Environment", "php artisan env")
        };

        // Handle special cases for commands that might not be available in all Laravel versions
        foreach (var (name, cmd) in checks)
        {
            _console.WriteInfo($"\n{name}:");
            _processService.RunProcess(cmd.Split(' ')[0], string.Join(' ', cmd.Split(' ').Skip(1)));
        }

        // Handle cache status with error handling
        _console.WriteInfo("\nCache Status:");
        bool cacheStatusSuccess = RunArtisanCommand("cache:status");
        if (!cacheStatusSuccess)
        {
            _console.WriteInfo("Available cache commands:");
            RunArtisanCommand("list cache");
        }

        // Handle route list with error handling
        _console.WriteInfo("\nRoute List:");
        bool routeListSuccess = RunArtisanCommand("route:list");
        if (!routeListSuccess) _console.WriteInfo("Please use \"php artisan route:list\" without unsupported options.");

        Console.ReadLine();
    }

    private void ResetCache()
    {
        if (_confirmation.ConfirmAction("Are you sure you want to reset all cache?", false))
        {
            var commands = new[]
            {
                "config:clear",
                "cache:clear",
                "view:clear",
                "route:clear",
                "event:clear",
                "optimize:clear"
            };
            foreach (var cmd in commands)
                RunArtisanCommand(cmd, false);
            _console.WriteSuccess("✓ All cache cleared successfully!");
        }
        else
        {
            _console.WriteInfo("Cancelled.");
        }

        Console.ReadLine();
    }

    private void ViewRouteList()
    {
        _console.WriteInfo("Retrieving route list...");

        // Try with pagination first for better readability
        bool success = RunArtisanCommand("route:list");

        if (!success)
        {
            // If that fails, try without any options
            _console.WriteInfo("Trying alternative route listing...");
            RunArtisanCommand("route:list");
        }

        Console.ReadLine();
    }

    /// <summary>
    ///     Provides helpful diagnostics for common Laravel errors
    /// </summary>
    /// <param name="command">The Artisan command that failed</param>
    private void HandleLaravelError(string command)
    {
        _console.WriteInfo("\nDiagnosing Laravel error...");

        // Common Laravel error patterns and solutions
        if (command.StartsWith("migrate") || command.Contains("db:"))
        {
            // Database-related errors
            _console.WriteInfo(
                "This might be a database connection issue. Check your .env file for correct database settings.");
            _console.WriteInfo("Make sure your database server is running and accessible.");
            _console.WriteInfo("Common solutions:");
            _console.WriteInfo(
                "1. Verify DB_HOST, DB_PORT, DB_DATABASE, DB_USERNAME, and DB_PASSWORD in your .env file");
            _console.WriteInfo("2. Ensure your database server is running");
            _console.WriteInfo("3. Run 'php artisan config:clear' to clear cached configuration");
        }
        else if (command.StartsWith("key:generate"))
        {
            // Key generation errors
            _console.WriteInfo(
                "Error generating application key. Check your .env file and ensure it exists and is writable.");
            _console.WriteInfo("If your .env file doesn't exist, copy .env.example to .env first.");
        }
        else if (command.Contains("storage:link"))
        {
            // Storage link errors
            _console.WriteInfo("Error creating storage link. Possible solutions:");
            _console.WriteInfo("1. Check if you have sufficient permissions to create symbolic links");
            _console.WriteInfo("2. On Windows, make sure you're running as Administrator");
            _console.WriteInfo(
                "3. Try creating the link manually with 'mklink /D public\\storage storage\\app\\public'");
        }
        else if (command.StartsWith("cache:") || command.StartsWith("config:") || command.StartsWith("route:"))
        {
            // Cache-related errors
            _console.WriteInfo("Error clearing cache. Possible solutions:");
            _console.WriteInfo("1. Check if storage/framework directories exist and are writable");
            _console.WriteInfo("2. Try manually deleting cache files in storage/framework/cache");
            _console.WriteInfo("3. Ensure you have appropriate file system permissions");
        }
        else
        {
            // General Laravel/Artisan errors
            _console.WriteInfo("Common troubleshooting steps:");
            _console.WriteInfo("1. Check if all required PHP extensions are enabled");
            _console.WriteInfo("2. Run 'composer dump-autoload' to regenerate class mappings");
            _console.WriteInfo("3. Ensure you have proper permissions for storage and bootstrap/cache directories");
            _console.WriteInfo("4. Check Laravel logs in storage/logs for more details");
        }
    }
}