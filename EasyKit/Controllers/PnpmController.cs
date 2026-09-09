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
using CommonUtilities.Utilities.System;
using EasyKit.Helpers.Console;
using EasyKit.Models;
using EasyKit.Services;
using EasyKit.UI.ConsoleUI;

namespace EasyKit.Controllers;

public class PnpmController
{
    private const string PNPM = "pnpm";
    private const string NODE = "node";
    private const string PNPM_CLI = "pnpm";
    private readonly ConfirmationHelper _confirmationHelper;
    private readonly ConsoleService _console;
    private readonly NotificationView _notificationView;
    private readonly CmdService _processService;
    private readonly SecureProcessRunner _processRunner;
    private readonly PromptView _prompt;
    private readonly Software _software;

    public PnpmController(
        Software software,
        ConsoleService console,
        ConfirmationHelper confirmationHelper,
        PromptView prompt,
        NotificationView notificationView)
    {
        _software = software;
        _console = console;
        _confirmationHelper = confirmationHelper;
        _prompt = prompt;
        _notificationView = notificationView;
        _processService = new CmdService();
        _processRunner = new SecureProcessRunner();
    }

    /// <summary>
    ///     Safely runs a pnpm command with proper argument handling.
    /// </summary>
    private (string output, string error, int exitCode) RunPnpmCommand(IEnumerable<string> arguments, string? workingDirectory = null)
    {
        return _processRunner.RunProcess(PNPM, arguments, workingDirectory ?? Environment.CurrentDirectory);
    }

    /// <summary>
    ///     Safely runs a pnpm command with streaming output.
    /// </summary>
    private int RunPnpmCommandStreaming(IEnumerable<string> arguments, string? workingDirectory = null,
        Action<string>? onOutput = null, Action<string>? onError = null)
    {
        return _processRunner.RunProcessStreaming(PNPM, arguments, workingDirectory ?? Environment.CurrentDirectory, onOutput, onError);
    }

    public void ShowMenu()
    {
        // Get user settings
        int menuWidth = 100;
        var menuWidthObj = _console.Config.Get("menu_width", 100);
        if (menuWidthObj is int mw)
            menuWidth = mw;
        var colorScheme = MenuTheme.ColorScheme.Teal;

        // Check if pnpm is installed first
        if (!EnsurePnpmInstalled()) return;

        // User-friendly, logical order for pnpm menu
        var menuView = new MenuView();
        var menu = menuView.CreateMenu("pnpm Tools", width: menuWidth);
        menu.AddOption("1", "Install packages (pnpm install)", () => InstallPackages())
            .AddOption("2", "Add dependency (pnpm add)", () => AddPackage())
            .AddOption("3", "Add dev dependency (pnpm add --save-dev)", () => AddDevPackage())
            .AddOption("4", "Remove dependency (pnpm remove)", () => RemovePackage())
            .AddOption("5", "Update packages (pnpm outdated + upgrade)", () => UpdatePackages())
            .AddOption("6", "Show package.json info", () => ShowPackageInfo())
            .AddOption("7", "Build for production (pnpm run build)", () => BuildProduction())
            .AddOption("8", "Start development server (pnpm run dev)", () => BuildDevelopment())
            .AddOption("9", "Run custom pnpm script", () => RunCustomScript())
            .AddOption("10", "Execute command (pnpm exec)", () => ExecCommand())
            .AddOption("11", "Run one-off command (pnpm dlx)", () => DlxCmd())
            .AddOption("12", "Security audit (pnpm audit)", () => SecurityAudit())
            .AddOption("13", "Reset pnpm cache", () => ResetCache())
            .AddOption("14", "Run diagnostics", () => RunDiagnostics())
            .AddOption("0", "Back to main menu", () =>
            {
                /* Return to main menu */
            });

        menu.WithColors(colorScheme.border, colorScheme.highlight, colorScheme.title, colorScheme.text,
                colorScheme.help)
            .WithHelpText("Select an option or press 0 to return to the main menu")
            .WithDoubleBorder()
            .Show();
    }

    private void OpenPnpmWebsite()
    {
        _console.WriteInfo(
            "Node.js and pnpm are required. Opening the official Node.js download page in your default browser...");
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://nodejs.org/en/download/",
                UseShellExecute = true
            });
            _console.WriteInfo(
                "Browser opened. Please download and install Node.js (includes npm). After installation, install pnpm via 'npm install -g pnpm'. Then restart EasyKit.");
        }
        catch (Exception ex)
        {
            LoggerUtilities.Error($"Error opening browser: {ex.Message}");
            _console.WriteError("Failed to open browser. Please visit https://nodejs.org/en/download/ manually.");
        }

        Console.ReadLine();
    }

    private bool EnsurePnpmInstalled()
    {
        var (output, error, exitCode) = RunPnpmCommand(["--version"]);
        if (exitCode != 0 || string.IsNullOrWhiteSpace(output))
        {
            _console.WriteError("pnpm is not installed or not accessible via PATH.");
            _console.WriteInfo("To install pnpm, run: npm install -g pnpm");
            _console.WriteInfo("Or visit: https://pnpm.io/installation");
            _console.WriteInfo("Press Enter to continue...");
            Console.ReadLine();
            return false;
        }
        _console.WriteSuccess($"✓ pnpm is installed. Version: {output.Trim()}");
        return true;
    }

    private void InstallPackages()
    {
        _console.WriteInfo("Installing pnpm packages...");
        if (EnsurePnpmInstalled())
        {
            _processRunner.RunProcessInNewWindow(PNPM, ["install"]);
            _console.WriteInfo("Started 'pnpm install' in a new command window.");
        }

        Console.ReadLine();
    }

    private void UpdatePackages()
    {
        _console.WriteInfo("Updating pnpm packages...");
        if (!EnsurePnpmInstalled())
        {
            Console.ReadLine();
            return;
        }

        // Show outdated packages first
        RunPnpmCommandStreaming(["outdated"]);
        _console.WriteInfo("Above is the list of outdated packages.");

        // Ask user if they want to upgrade
        if (_confirmationHelper.ConfirmAction("Do you want to upgrade packages to latest versions?", false))
        {
            _processRunner.RunProcessInNewWindow(PNPM, ["upgrade"]);
            _console.WriteInfo("Started 'pnpm upgrade' in a new command window.");
        }
        else
        {
            _console.WriteInfo("Update skipped.");
        }

        Console.ReadLine();
    }

    private void BuildProduction()
    {
        if (EnsurePnpmInstalled())
        {
            _processRunner.RunProcessInNewWindow(PNPM, ["run", "build"]);
            _console.WriteInfo("Started 'pnpm run build' in a new command window.");
        }

        Console.ReadLine();
    }

    private void BuildDevelopment()
    {
        if (EnsurePnpmInstalled())
        {
            _processRunner.RunProcessInNewWindow(PNPM, ["run", "dev"]);
            _console.WriteInfo("Started 'pnpm run dev' in a new command window.");
        }

        Console.ReadLine();
    }

    private void SecurityAudit()
    {
        if (EnsurePnpmInstalled())
        {
            RunPnpmCommandStreaming(["audit"]);
            _console.WriteInfo("Ran 'pnpm audit' with streaming output.");
        }

        Console.ReadLine();
    }

    private void RunCustomScript()
    {
        if (!EnsurePnpmInstalled())
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

        var script = _prompt.Prompt("Enter pnpm script name (e.g. 'start'): ");
        if (string.IsNullOrWhiteSpace(script))
        {
            _console.WriteError("Script name cannot be empty.");
            Console.ReadLine();
            return;
        }

        // Validate script name - only allow alphanumeric, hyphens, underscores, colons
        if (!System.Text.RegularExpressions.Regex.IsMatch(script, @"^[a-zA-Z0-9_\-:]+$"))
        {
            _console.WriteError("Invalid script name. Only alphanumeric characters, hyphens, underscores, and colons are allowed.");
            Console.ReadLine();
            return;
        }

        _processRunner.RunProcessInNewWindow(PNPM, ["run", script]);
        _console.WriteInfo($"Started 'pnpm run {script}' in a new command window.");
        Console.ReadLine();
    }

    private void AddPackage()
    {
        if (!EnsurePnpmInstalled())
        {
            Console.ReadLine();
            return;
        }

        var package = _prompt.Prompt("Enter package name (e.g. 'react'): ");
        if (string.IsNullOrWhiteSpace(package))
        {
            _console.WriteError("Package name cannot be empty.");
            Console.ReadLine();
            return;
        }

        // Validate package name
        if (!System.Text.RegularExpressions.Regex.IsMatch(package, @"^[a-zA-Z0-9@/_\-\.]+$"))
        {
            _console.WriteError("Invalid package name. Only alphanumeric characters, @, /, _, -, and . are allowed.");
            Console.ReadLine();
            return;
        }

        _console.WriteInfo($"Adding dependency '{package}'...");
        _processRunner.RunProcessInNewWindow(PNPM, ["add", package]);
        _console.WriteSuccess($"✓ Added '{package}' successfully!");
        Console.ReadLine();
    }

    private void AddDevPackage()
    {
        if (!EnsurePnpmInstalled())
        {
            Console.ReadLine();
            return;
        }

        var package = _prompt.Prompt("Enter dev package name (e.g. 'jest'): ");
        if (string.IsNullOrWhiteSpace(package))
        {
            _console.WriteError("Package name cannot be empty.");
            Console.ReadLine();
            return;
        }

        // Validate package name
        if (!System.Text.RegularExpressions.Regex.IsMatch(package, @"^[a-zA-Z0-9@/_\-\.]+$"))
        {
            _console.WriteError("Invalid package name. Only alphanumeric characters, @, /, _, -, and . are allowed.");
            Console.ReadLine();
            return;
        }

        _console.WriteInfo($"Adding dev dependency '{package}'...");
        _processRunner.RunProcessInNewWindow(PNPM, ["add", "--save-dev", package]);
        _console.WriteSuccess($"✓ Added dev dependency '{package}' successfully!");
        Console.ReadLine();
    }

    private void RemovePackage()
    {
        if (!EnsurePnpmInstalled())
        {
            Console.ReadLine();
            return;
        }

        var package = _prompt.Prompt("Enter package name to remove (e.g. 'react'): ");
        if (string.IsNullOrWhiteSpace(package))
        {
            _console.WriteError("Package name cannot be empty.");
            Console.ReadLine();
            return;
        }

        // Validate package name
        if (!System.Text.RegularExpressions.Regex.IsMatch(package, @"^[a-zA-Z0-9@/_\-\.]+$"))
        {
            _console.WriteError("Invalid package name. Only alphanumeric characters, @, /, _, -, and . are allowed.");
            Console.ReadLine();
            return;
        }

        if (_confirmationHelper.ConfirmAction($"Are you sure you want to remove '{package}'?", false))
        {
            _console.WriteInfo($"Removing dependency '{package}'...");
            _processRunner.RunProcessInNewWindow(PNPM, ["remove", package]);
            _console.WriteSuccess($"✓ Removed '{package}' successfully!");
        }
        else
        {
            _console.WriteInfo("Cancelled.");
        }

        Console.ReadLine();
    }

    private void ExecCommand()
    {
        if (!EnsurePnpmInstalled())
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

        var command = _prompt.Prompt("Enter command to execute (e.g. 'tsc --noEmit'): ");
        if (string.IsNullOrWhiteSpace(command))
        {
            _console.WriteError("Command cannot be empty.");
            Console.ReadLine();
            return;
        }

        // Basic validation - reject shell metacharacters
        if (command.Any(c => new[] { ';', '|', '&', '$', '`', '>', '<' }.Contains(c)))
        {
            _console.WriteError("Invalid command. Shell metacharacters are not allowed.");
            Console.ReadLine();
            return;
        }

        _console.WriteInfo($"Executing: pnpm exec {command}");
        _processRunner.RunProcessInNewWindow(PNPM, ["exec", "--", command]);
        Console.ReadLine();
    }

    private void DlxCmd()
    {
        if (!EnsurePnpmInstalled())
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

        var package = _prompt.Prompt("Enter package to run (e.g. 'tsx'): ");
        if (string.IsNullOrWhiteSpace(package))
        {
            _console.WriteError("Package name cannot be empty.");
            Console.ReadLine();
            return;
        }

        // Validate package name
        if (!System.Text.RegularExpressions.Regex.IsMatch(package, @"^[a-zA-Z0-9@/_\-\.]+$"))
        {
            _console.WriteError("Invalid package name. Only alphanumeric characters, @, /, _, -, and . are allowed.");
            Console.ReadLine();
            return;
        }

        _console.WriteInfo($"Running: pnpm dlx {package}");
        _processRunner.RunProcessInNewWindow(PNPM, ["dlx", package]);
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
            LoggerUtilities.Error($"Error reading package.json: {ex.Message}");
            _console.WriteError("Invalid package.json file");
        }

        Console.ReadLine();
    }

    private void ResetCache()
    {
        if (_confirmationHelper.ConfirmAction("Are you sure you want to reset the pnpm cache?", false))
        {
            if (EnsurePnpmInstalled())
            {
                RunPnpmCommandStreaming(["store", "prune"]);
                _console.WriteInfo("Ran 'pnpm store prune' to clean the pnpm store cache.");
            }
            else
            {
                _console.WriteError("pnpm is not installed.");
            }
        }
        else
        {
            _console.WriteInfo("Reset cache canceled.");
        }

        Console.ReadLine();
    }

    private void RunDiagnostics()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("===== PNPM Configuration Diagnostics =====\n");

        // Step 1: Check if pnpm is accessible
        sb.AppendLine("Step 1: Checking if pnpm is accessible in PATH");
        var process = RunPnpmCommand(["--version"]);
        if (process.exitCode == 0 && !string.IsNullOrWhiteSpace(process.output))
        {
            sb.AppendLine($"[OK] pnpm is accessible. Version: {process.output.Trim()}");
        }
        else
        {
            sb.AppendLine("[ERROR] pnpm is not accessible via PATH.");
            sb.AppendLine("Please install pnpm from https://pnpm.io/installation");
            sb.AppendLine("You can install it via: npm install -g pnpm");
            sb.AppendLine("\n===== End of PNPM Configuration Diagnostics =====");
            Console.WriteLine(sb.ToString());
            Console.ReadLine();
            return;
        }

        // Step 2: Check Node.js
        sb.AppendLine("\nStep 2: Checking Node.js version");
        var nodeProcess = _processRunner.RunProcess(NODE, ["--version"], Environment.CurrentDirectory);
        if (nodeProcess.exitCode == 0 && !string.IsNullOrWhiteSpace(nodeProcess.output))
        {
            sb.AppendLine($"[OK] Node.js is accessible. Version: {nodeProcess.output.Trim()}");
        }
        else
        {
            sb.AppendLine("[WARNING] Node.js is not accessible via PATH.");
            sb.AppendLine("pnpm requires Node.js 18+ (Node.js 20+ recommended).");
            sb.AppendLine("Please install Node.js from https://nodejs.org/");
        }

        // Step 3: Check package.json
        sb.AppendLine("\nStep 3: Checking package.json");
        if (File.Exists("package.json"))
        {
            sb.AppendLine("[OK] package.json found in current directory.");
            try
            {
                var json = File.ReadAllText("package.json");
                var doc = System.Text.Json.JsonDocument.Parse(json);
                sb.AppendLine("Package info:");
                if (doc.RootElement.TryGetProperty("name", out var nameProp))
                    sb.AppendLine($"  Name: {nameProp.GetString()}");
                if (doc.RootElement.TryGetProperty("version", out var versionProp))
                    sb.AppendLine($"  Version: {versionProp.GetString()}");
                if (doc.RootElement.TryGetProperty("scripts", out var scriptsProp) &&
                    scriptsProp.ValueKind == System.Text.Json.JsonValueKind.Object)
                {
                    sb.AppendLine("  Available scripts:");
                    foreach (var script in scriptsProp.EnumerateObject())
                        sb.AppendLine($"    - {script.Name}");
                }
            }
            catch (Exception ex)
            {
                sb.AppendLine($"[ERROR] Invalid package.json: {ex.Message}");
            }
        }
        else
        {
            sb.AppendLine("[WARNING] No package.json found in current directory.");
            sb.AppendLine("Create one with: pnpm init");
        }

        // Step 4: Check node_modules presence
        sb.AppendLine("\nStep 4: Checking node_modules");
        if (Directory.Exists("node_modules"))
        {
            var packageCount = Directory.GetFiles("node_modules", "package.json",
                System.IO.SearchOption.AllDirectories).Length;
            sb.AppendLine($"[OK] node_modules exists with approximately {packageCount} packages.");
        }
        else
        {
            sb.AppendLine("[INFO] No node_modules found. Run 'pnpm install' to install dependencies.");
        }

        // Step 5: pnpm store info
        sb.AppendLine("\nStep 5: pnpm store info");
        var storeProcess = RunPnpmCommand(["store", "path"]);
        if (storeProcess.exitCode == 0 && !string.IsNullOrWhiteSpace(storeProcess.output))
        {
            sb.AppendLine($"[OK] pnpm store path: {storeProcess.output.Trim()}");
        }
        else
        {
            sb.AppendLine("[INFO] Could not retrieve pnpm store path.");
        }

        // Step 6: pnpm config
        sb.AppendLine("\nStep 6: pnpm configuration");
        var configProcess = RunPnpmCommand(["config", "list"]);
        if (configProcess.exitCode == 0 && !string.IsNullOrWhiteSpace(configProcess.output))
        {
            sb.AppendLine("Current pnpm config:");
            foreach (var line in configProcess.output.Split('\n'))
                sb.AppendLine($"  {line}");
        }
        else
        {
            sb.AppendLine("[INFO] Could not retrieve pnpm config.");
        }

        sb.AppendLine("\n===== End of PNPM Configuration Diagnostics =====");
        Console.WriteLine(sb.ToString());
        Console.ReadLine();
    }
}
