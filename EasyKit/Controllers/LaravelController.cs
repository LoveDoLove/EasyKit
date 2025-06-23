// MIT License
// 
// Copyright (c) 2025 LoveDoLove
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System.Diagnostics;
using EasyKit.Helpers.Console;
using EasyKit.Models;
using EasyKit.Services;
using EasyKit.UI.ConsoleUI;

namespace EasyKit.Controllers;

public class LaravelController
{
    private readonly ConfirmationHelper _confirmation;
    private readonly ConsoleService _console;
    private readonly NotificationView _notificationView;
    private readonly CmdService _processService;
    private readonly PromptView _prompt;
    private readonly Software _software;

    public LaravelController(
        Software software,
        ConsoleService console,
        ConfirmationHelper confirmation,
        PromptView prompt,
        NotificationView notificationView)
    {
        _software = software;
        _console = console;
        _confirmation = confirmation;
        _prompt = prompt;
        _notificationView = notificationView;
        _processService = new CmdService();
    }

    public void RunPhpDiagnostics()
    {
        _console.WriteInfo("===== PHP Configuration Diagnostics =====");
        _console.WriteInfo(
            "This will check your PHP installation and environment for Laravel/Composer compatibility.\n");

        // Step 1: Check if PHP is accessible in PATH
        _console.WriteInfo("Step 1: Checking if PHP is accessible in PATH");
        var process = _processService.RunProcess("php", "--version", Environment.CurrentDirectory);
        if (process.exitCode == 0 && !string.IsNullOrWhiteSpace(process.output))
        {
            _console.WriteSuccess($"\u2713 PHP is accessible. Version: {process.output.Split('\n')[0].Trim()}");
        }
        else
        {
            _console.WriteError("\u2717 PHP is not accessible via PATH.");
            _console.WriteInfo(
                "Please install PHP from https://windows.php.net/download/ and ensure it is in your PATH.");
            _console.WriteInfo("You can also use the Tool Marketplace in EasyKit to open the download page.");
            _console.WriteInfo("\n===== End of PHP Configuration Diagnostics =====");
            Console.ReadLine();
            return;
        }

        // Step 2: Recommendations
        _console.WriteInfo("\nStep 2: Recommendations");
        _console.WriteInfo("- If you encounter issues, ensure PHP is installed and in your PATH.");
        _console.WriteInfo("- Enable all required extensions in your php.ini file.");
        _console.WriteInfo("- Set memory_limit to -1 for Composer-heavy operations.");
        _console.WriteInfo("- Restart your terminal or EasyKit after making changes.");
        _console.WriteInfo(
            "- Use the Tool Marketplace in EasyKit to check installation status or open the download page.");

        _console.WriteInfo("\n===== End of PHP Configuration Diagnostics =====");
        Console.ReadLine();
    }

    private bool EnsurePhpInstalled()
    {
        var process = _processService.RunProcess("php", "--version", Environment.CurrentDirectory);
        if (process.exitCode != 0)
        {
            _console.WriteError("PHP is not properly installed or not compatible with Laravel.");
            _console.WriteInfo("Laravel requires PHP 7.3+ (PHP 8.0+ recommended for newer Laravel versions).");
            _console.WriteInfo("Please install PHP from https://www.php.net/ or https://windows.php.net/download/");
            Console.ReadLine();
            return false;
        }

        _console.WriteInfo($"Using PHP: {process.output.Split('\n')[0].Trim()}");
        return true;
    }

    private bool RunComposerCommand(string args, bool showOutput = true)
    {
        // Use a new cmd window for install/update, streaming for others
        if (args.StartsWith("install") || args.StartsWith("update") || args.Contains("--no-dev"))
        {
            _processService.RunProcessInNewCmdWindow("composer", args, Environment.CurrentDirectory);
            // Can't get exit code, so assume success for UX
            return true;
        }

        if (showOutput)
        {
            int exitCode = _processService.RunProcessWithStreaming("composer", args, Environment.CurrentDirectory);
            return exitCode == 0;
        }
        else
        {
            var (output, error, exitCode) = _processService.RunProcess("composer", args, Environment.CurrentDirectory);
            return exitCode == 0;
        }
    }

    private bool RunArtisanCommand(string args, bool showOutput = true)
    {
        if (showOutput)
        {
            int exitCode =
                _processService.RunProcessWithStreaming("php", $"artisan {args}", Environment.CurrentDirectory);
            return exitCode == 0;
        }
        else
        {
            var (output, error, exitCode) =
                _processService.RunProcess("php", $"artisan {args}", Environment.CurrentDirectory);
            return exitCode == 0;
        }
    }

    public void ShowMenu()
    {
        // Get user settings
        int menuWidth = 100;

        // Try to get user preferences from config
        var menuWidthObj = _console.Config.Get("menu_width", 100);
        if (menuWidthObj is int mw)
            menuWidth = mw;

        // Display current directory
        string currentDirectory = Environment.CurrentDirectory;

        // Create and configure the menu with a Laravel-specific theme
        var menuView = new MenuView();
        menuView.CreateMenu("Laravel Toolkit", width: menuWidth)
            .AddOption("1", "Run PHP diagnostics", () => RunPhpDiagnostics())
            .AddOption("2", "Quick Setup (env, install, key, cache)", () => QuickSetup())
            .AddOption("3", "Check PHP Version", () => CheckPhpVersion())
            .AddOption("4", "Check Laravel Configuration", () => CheckConfiguration())
            .AddOption("5", "Install Composer Packages", () => InstallPackages())
            .AddOption("6", "Update Composer Packages", () => UpdatePackages())
            .AddOption("7", "Regenerate Autoload Files", () => RegenerateAutoload())
            .AddOption("8", "Build for Production", () => BuildProduction())
            .AddOption("9", "Start Development Server", () => RunDevServer())
            .AddOption("10", "View Route List", () => ViewRouteList())
            .AddOption("11", "Test Database Connection", () => TestDatabase())
            .AddOption("12", "Run Database Seeding (migrate:fresh --seed)", () => RunDatabaseSeeding())
            .AddOption("13", "Create Storage Link", () => CreateStorageLink())
            .AddOption("14", "Reset All Laravel Cache", () => ResetCache())
            .AddOption("0", "Back to Main Menu", () =>
            {
                /* Return to main menu */
            })
            .WithColors(ConsoleColor.DarkRed, ConsoleColor.Red, ConsoleColor.White)
            .WithHelpText("Select an option or press 0 to return to the main menu")
            .WithDoubleBorder()
            .Show();
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

        // Use new cmd window for composer install
        _processService.RunProcessInNewCmdWindow("composer", "install", Environment.CurrentDirectory);
        _console.WriteSuccess("✓ Installed Composer dependencies (see new window for details)");

        if (RunArtisanCommand("key:generate"))
            _console.WriteSuccess("✓ Generated application key");
        else
            _console.WriteError("✗ Failed to generate key");
        RunArtisanCommand("config:clear");
        RunArtisanCommand("cache:clear");
        _console.WriteSuccess("✓ Cleared configuration and cache");
        _console.WriteSuccess("\nSetup completed successfully!");
        Console.ReadLine();
    }

    private void InstallPackages()
    {
        _console.WriteInfo("Installing Composer packages...");
        _processService.RunProcessInNewCmdWindow("composer", "install", Environment.CurrentDirectory);
        _console.WriteSuccess("✓ Packages install command started in new window!");
        Console.ReadLine();
    }

    private void UpdatePackages()
    {
        _console.WriteInfo("Updating Composer packages...");
        _processService.RunProcessInNewCmdWindow("composer", "update", Environment.CurrentDirectory);
        _console.WriteSuccess("✓ Packages update command started in new window!");
        Console.ReadLine();
    }

    private void RegenerateAutoload()
    {
        _console.WriteInfo("Regenerating autoload files...");
        RunComposerCommand("dump-autoload");
        _console.WriteSuccess("✓ Autoload files regenerated!");
        Console.ReadLine();
    }

    private void BuildProduction()
    {
        _console.WriteInfo("Building for production...");
        bool isLaravel11Plus = false;
        var phpPath = "php";
        var versionProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = phpPath,
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
            if (versionOutput.Contains("Laravel Framework") &&
                (versionOutput.Contains("11.") || versionOutput.Contains("12.")))
                isLaravel11Plus = true;
        }
        catch
        {
            isLaravel11Plus = false;
        }

        var steps = new List<(string message, string command)>
        {
            ("Installing production dependencies...", "install --no-dev")
        };
        if (isLaravel11Plus)
        {
            steps.Add(("Optimizing application...", "artisan optimize"));
        }
        else
        {
            steps.Add(("Optimizing configuration...", "artisan config:cache"));
            steps.Add(("Optimizing routes...", "artisan route:cache"));
            steps.Add(("Optimizing views...", "artisan view:cache"));
        }

        foreach (var (msg, cmd) in steps)
        {
            _console.WriteInfo(msg);
            if (cmd.StartsWith("artisan "))
                RunArtisanCommand(cmd.Substring(8));
            else
                _processService.RunProcessInNewCmdWindow("composer", cmd, Environment.CurrentDirectory);
        }

        _console.WriteSuccess("✓ Production build steps started (see new window for composer install)");
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

        // Use new cmd window for dev server
        _processService.RunProcessInNewCmdWindow("php", "artisan serve", Environment.CurrentDirectory);
        _console.WriteInfo(
            "✓ Development server started in a new terminal window. Press Ctrl+C in that window to stop the server.");
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
        foreach (var (name, cmd) in checks)
        {
            _console.WriteInfo($"\n{name}:");
            var parts = cmd.Split(' ', 2);
            _processService.RunProcess(parts[0], parts.Length > 1 ? parts[1] : "");
        }

        _console.WriteInfo("\nCache Status:");
        bool cacheStatusSuccess = RunArtisanCommand("cache:status");
        if (!cacheStatusSuccess)
        {
            _console.WriteInfo("Available cache commands:");
            RunArtisanCommand("list cache");
        }

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