using System.Diagnostics;

namespace EasyKit.Controllers;

public class LaravelController
{
    private readonly ConfirmationService _confirmation = new();
    private readonly ConsoleService _console;
    private readonly LoggerService _logger;
    private readonly Software _software;

    public LaravelController(Software software, LoggerService logger, ConsoleService console)
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
        var result = RunProcess("php", "--version", false);
        if (!result)
        {
            _console.WriteError("PHP is not installed or not in PATH. Please install PHP from https://www.php.net/");
            Console.ReadLine();
        }

        return result;
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
        if (!EnsurePhpInstalled()) return false;
        var composerCmd = FindComposerCommand();
        if (composerCmd == null)
        {
            _console.WriteError("Composer executable not found. Please ensure Composer is installed and in your PATH.");
            return false;
        }

        return RunProcess(composerCmd, args, showOutput);
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
            // Add timeout handling for long-running commands
            var result = RunProcess("php", $"artisan {args}", showOutput);

            // Check for common error patterns in output even when exit code is 0
            if (!result && showOutput)
            {
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

    private bool RunProcess(string file, string args, bool showOutput = true)
    {
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = file.Split(' ')[0],
                Arguments = string.Join(' ', file.Split(' ').Skip(1)) +
                            (string.IsNullOrWhiteSpace(args) ? "" : " " + args),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            var process = Process.Start(psi);
            if (process == null)
            {
                _console.WriteError($"Failed to start process: {file}");
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
            _logger.Error($"Error running process: {ex.Message}");
            if (showOutput) _console.WriteError(ex.Message);
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
            {
                isLaravel11Plus = true;
            }
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
                success = RunArtisanCommand("migrate:status", true);
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
        RunProcess("php", "--version");
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
            RunProcess(cmd.Split(' ')[0], string.Join(' ', cmd.Split(' ').Skip(1)));
        }

        // Handle cache status with error handling
        _console.WriteInfo("\nCache Status:");
        bool cacheStatusSuccess = RunArtisanCommand("cache:status", true);
        if (!cacheStatusSuccess)
        {
            _console.WriteInfo("Available cache commands:");
            RunArtisanCommand("list cache", true);
        }

        // Handle route list with error handling
        _console.WriteInfo("\nRoute List:");
        bool routeListSuccess = RunArtisanCommand("route:list", true);
        if (!routeListSuccess)
        {
            _console.WriteInfo("Please use \"php artisan route:list\" without unsupported options.");
        }

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
        bool success = RunArtisanCommand("route:list", true);

        if (!success)
        {
            // If that fails, try without any options
            _console.WriteInfo("Trying alternative route listing...");
            RunArtisanCommand("route:list", true);
        }

        Console.ReadLine();
    }
}