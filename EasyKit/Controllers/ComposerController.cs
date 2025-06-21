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

using System.Text;
using System.Text.Json;
using CommonUtilities.Utilities.System;
using EasyKit.Helpers.Console;
using EasyKit.Models;
using EasyKit.Services;
using EasyKit.UI.ConsoleUI;

namespace EasyKit.Controllers;

public class ComposerController
{
    private readonly ConfirmationHelper _confirmation;
    private readonly ConsoleService _console;
    private readonly NotificationView _notificationView;
    private readonly ProcessService _processService;
    private readonly PromptView _prompt;
    private readonly Software _software;

    public ComposerController(
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
        _processService = new ProcessService();
    }

    public void ShowMenu()
    {
        // Get user settings
        int menuWidth = 100;

        // Try to get user preferences from config if available
        var menuWidthObj = _console.Config.Get("menu_width", 100);
        if (menuWidthObj is int mw)
            menuWidth = mw;

        // Create and configure the menu
        var menuView = new MenuView();
        menuView.CreateMenu("Composer Tools", width: menuWidth)
            .AddOption("1", "Create new project", () => CreateProject())
            .AddOption("2", "Install packages (composer install)", () => InstallPackages())
            .AddOption("3", "Update packages (composer update)", () => UpdatePackages())
            .AddOption("4", "Require a new package", () => RequirePackage())
            .AddOption("5", "Regenerate autoload files (composer dump-autoload)", () => RegenerateAutoload())
            .AddOption("6", "Validate composer.json", () => ValidateJson())
            .AddOption("7", "Show composer.json info", () => ShowPackageInfo())
            .AddOption("8", "Clear Composer cache", () => ClearCache())
            .AddOption("9", "Run diagnostics", () => RunDiagnostics())
            .AddOption("0", "Back to main menu", () =>
            {
                /* Return to main menu */
            })
            .WithColors(MenuTheme.ColorScheme.Dark.border, MenuTheme.ColorScheme.Dark.highlight,
                MenuTheme.ColorScheme.Dark.title, MenuTheme.ColorScheme.Dark.text, MenuTheme.ColorScheme.Dark.help)
            .WithHelpText("Select an option or press 0 to return to the main menu")
            .Show();
    }

    private void InstallPackages()
    {
        _console.WriteInfo("Installing Composer packages...");
        _processService.RunProcessInNewCmdWindow("composer", "install", Environment.CurrentDirectory);
        _console.WriteSuccess("✓ Composer install launched in new window!");
        Console.ReadLine();
    }

    private void UpdatePackages()
    {
        _console.WriteInfo("Updating Composer packages...");
        _processService.RunProcessInNewCmdWindow("composer", "update", Environment.CurrentDirectory);
        _console.WriteSuccess("✓ Composer update launched in new window!");
        Console.ReadLine();
    }

    private void RequirePackage()
    {
        var package = _prompt.Prompt("Enter package name (e.g. 'vendor/package'): ");
        var dev = _confirmation.ConfirmAction("Is this a development dependency?", false);
        var args = dev ? $"require --dev {package}" : $"require {package}";
        _console.WriteInfo($"Installing {package}...");
        _processService.RunProcessInNewCmdWindow("composer", args, Environment.CurrentDirectory);
        _console.WriteSuccess("✓ Composer require launched in new window!");
        Console.ReadLine();
    }

    private void CreateProject()
    {
        var package = _prompt.Prompt("Enter project package (e.g. 'laravel/laravel'): ");
        var directory = _prompt.Prompt("Enter project directory name: ");
        _console.WriteInfo($"Creating new project from {package}...");
        _processService.RunProcessInNewCmdWindow("composer", $"create-project {package} {directory}",
            Environment.CurrentDirectory);
        _console.WriteSuccess("✓ Composer create-project launched in new window!");
        Console.ReadLine();
    }

    private void RegenerateAutoload()
    {
        _console.WriteInfo("Regenerating autoload files...");
        _processService.RunProcessWithStreaming("composer", "dump-autoload", Environment.CurrentDirectory);
        _console.WriteSuccess("✓ Autoload files regenerated!");
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
        _processService.RunProcess("composer", "validate", Environment.CurrentDirectory);
        _console.WriteSuccess("✓ composer.json is valid!");
        Console.ReadLine();
    }

    private void ClearCache()
    {
        if (_confirmation.ConfirmAction("Are you sure you want to clear Composer cache?", false))
        {
            _console.WriteInfo("Clearing Composer cache...");
            _processService.RunProcessWithStreaming("composer", "clear-cache", Environment.CurrentDirectory);
            _console.WriteSuccess("✓ Cache cleared successfully!");
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
            LoggerUtilities.Error($"Error reading composer.json: {ex.Message}");
            _console.WriteError("Invalid composer.json file");
        }

        Console.ReadLine();
    }

    private void RunDiagnostics()
    {
        var sb = new StringBuilder();
        sb.AppendLine("===== COMPOSER Configuration Diagnostics =====\n");
        // Step 1: Check if Composer is accessible
        sb.AppendLine("Step 1: Checking if Composer is accessible in PATH");
        var process = _processService.RunProcess("composer", "--version", Environment.CurrentDirectory);
        if (process.exitCode == 0 && !string.IsNullOrWhiteSpace(process.output))
        {
            sb.AppendLine($"[OK] Composer is accessible. Version: {process.output.Trim()}");
        }
        else
        {
            sb.AppendLine("[ERROR] Composer is not accessible via PATH.");
            sb.AppendLine(
                "Please install Composer from https://getcomposer.org/download/ and ensure it is in your PATH.");
            sb.AppendLine("You can also use the Tool Marketplace in EasyKit to open the download page.");
            sb.AppendLine("\n===== End of COMPOSER Configuration Diagnostics =====");
            Console.WriteLine(sb.ToString());
            Console.ReadLine();
            return;
        }

        // Step 2: Check PHP version (optional, minimal)
        sb.AppendLine("\nStep 2: Checking PHP version");
        var phpProcess = _processService.RunProcess("php", "--version", Environment.CurrentDirectory);
        if (phpProcess.exitCode == 0 && !string.IsNullOrWhiteSpace(phpProcess.output))
            sb.AppendLine($"[OK] PHP is accessible. Version: {phpProcess.output.Split('\n')[0].Trim()}");
        else
            sb.AppendLine("[ERROR] PHP is not accessible via PATH.");
        // Step 3: Validate composer.json
        sb.AppendLine("\nStep 3: Validating composer.json");
        if (File.Exists("composer.json"))
        {
            var valProcess =
                _processService.RunProcess("composer", "validate --no-check-publish", Environment.CurrentDirectory);
            sb.AppendLine($"composer.json Validity: {(valProcess.exitCode == 0 ? "Valid" : "Invalid")}");
            if (valProcess.exitCode != 0) sb.AppendLine($"Validation Error: {valProcess.error}");
        }
        else
        {
            sb.AppendLine("No composer.json found in current directory.");
        }

        sb.AppendLine("\n===== End of COMPOSER Configuration Diagnostics =====");
        Console.WriteLine(sb.ToString());
        Console.ReadLine();
    }
}