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
using System.Text.Json;
using CommonUtilities.Utilities.System;
using EasyKit.Helpers.Console;
using EasyKit.Models;
using EasyKit.Services;
using EasyKit.UI.ConsoleUI;

namespace EasyKit.Controllers;

public class NpmController
{
    private readonly ConfirmationHelper _confirmationHelper;
    private readonly ConsoleService _console;
    private readonly NotificationView _notificationView;
    private readonly ProcessService _processService;
    private readonly PromptView _prompt;
    private readonly Software _software;

    public NpmController(
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
        _processService = new ProcessService(console, console.Config);
    }

    // Helper to get the detected npm path
    private string GetNpmPath()
    {
        return _processService.FindExecutablePath("npm") ?? "npm";
    }

    // Helper to get the detected node path
    private string GetNodePath()
    {
        return _processService.FindExecutablePath("node") ?? "node";
    }

    // Helper to get the detected ncu path
    private string GetNcuPath()
    {
        return _processService.FindExecutablePath("ncu") ?? "ncu";
    }

    public void ShowMenu()
    {
        // Get user settings
        int menuWidth = 100;
        var menuWidthObj = _console.Config.Get("menu_width", 100);
        if (menuWidthObj is int mw)
            menuWidth = mw;
        var colorScheme = MenuTheme.ColorScheme.Purple;

        // Check if npm is installed first
        bool npmInstalled = IsNpmInstalled();

        // If not installed, immediately open website and return
        if (!npmInstalled)
        {
            OpenNodejsWebsite();
            return;
        }

        // User-friendly, logical order for NPM menu
        var menuView = new MenuView();
        var menu = menuView.CreateMenu("NPM Tools", width: menuWidth);
        menu.AddOption("1", "Install packages (npm install)", () => InstallPackages())
            .AddOption("2", "Update packages (npm-check-updates)", () => UpdatePackages())
            .AddOption("3", "Show package.json info", () => ShowPackageInfo())
            .AddOption("4", "Build for production (npm run build)", () => BuildProduction())
            .AddOption("5", "Start development server (npm run dev)", () => BuildDevelopment())
            .AddOption("6", "Run custom npm script", () => RunCustomScript())
            .AddOption("7", "Security audit (npm audit)", () => SecurityAudit())
            .AddOption("8", "Reset npm cache", () => ResetCache())
            .AddOption("9", "Configure npm path", () => ConfigureNpmPath())
            .AddOption("D", "Run npm diagnostics", () => RunNpmDiagnostics())
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

    private bool IsNpmInstalled()
    {
        // Simplified: Just check if npm is available in PATH
        return _processService.RunProcess(GetNpmPath(), "--version", false, Environment.CurrentDirectory);
    }

    private void OpenNodejsWebsite()
    {
        _console.WriteInfo(
            "Node.js and npm are required. Opening the official Node.js download page in your default browser...");
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://nodejs.org/en/download/",
                UseShellExecute = true
            });
            _console.WriteInfo(
                "Browser opened. Please download and install Node.js (includes npm). After installation, restart EasyKit.");
        }
        catch (Exception ex)
        {
            LoggerUtilities.Error($"Error opening browser: {ex.Message}");
            _console.WriteError("Failed to open browser. Please visit https://nodejs.org/en/download/ manually.");
        }

        Console.ReadLine();
    }

    private bool EnsureNpmInstalled()
    {
        if (IsNpmInstalled())
            return true;
        OpenNodejsWebsite();
        return false;
    }

    private void InstallPackages()
    {
        _console.WriteInfo("Installing npm packages...");
        if (EnsureNpmInstalled())
        {
            if (_processService.RunProcess(GetNpmPath(), "install --no-fund --loglevel=error", true,
                    Environment.CurrentDirectory))
                _console.WriteSuccess("✓ Packages installed successfully!");
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
        if (!_processService.RunProcess(GetNcuPath(), "--version", false))
        {
            _console.WriteInfo("Installing npm-check-updates globally...");
            if (!_processService.RunProcess(GetNpmPath(), "install -g npm-check-updates", true,
                    Environment.CurrentDirectory))
            {
                _console.WriteError("Failed to install npm-check-updates");
                Console.ReadLine();
                return;
            }
        }

        if (_processService.RunProcess(GetNcuPath(), "-u", true, Environment.CurrentDirectory))
        {
            _console.WriteInfo("✓ package.json updated!");
            _processService.RunProcess(GetNpmPath(), "install --no-fund --loglevel=error", true,
                Environment.CurrentDirectory);
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
            if (_processService.RunProcess(GetNpmPath(), "run build", true, Environment.CurrentDirectory))
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
            // Open a new cmd window and run npm run dev using the detected npm path
            var npmPath = GetNpmPath();
            var npmDir = Path.GetDirectoryName(npmPath);
            var npmCmd = npmPath.EndsWith(".cmd", StringComparison.OrdinalIgnoreCase) ? npmPath : npmPath + ".cmd";
            if (!File.Exists(npmCmd)) npmCmd = npmPath; // fallback if .cmd doesn't exist
            var quotedNpm = npmCmd.Contains(" ") ? $"\"{npmCmd}\"" : npmCmd;
            Process.Start(new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c start cmd /k \"{quotedNpm} run dev\"",
                UseShellExecute = true,
                WorkingDirectory = Environment.CurrentDirectory
            });
            _console.WriteInfo("✓ Development server started in a new terminal window.");
        }
        catch (Exception ex)
        {
            LoggerUtilities.Error($"Error starting dev server: {ex.Message}");
            _console.WriteError("✗ Failed to start development server.");
        }

        Console.ReadLine();
    }

    private void SecurityAudit()
    {
        _console.WriteInfo("Running npm security audit...");
        if (EnsureNpmInstalled())
            _processService.RunProcess(GetNpmPath(), "audit", true, Environment.CurrentDirectory);
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
            var scriptName = _prompt.PromptWithAutocomplete("Enter script name to run: ", scripts);
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
            if (_processService.RunProcess(GetNpmPath(), $"run {scriptName}", true, Environment.CurrentDirectory))
                _console.WriteInfo($"✓ Script {scriptName} completed!");
            else
                _console.WriteError($"✗ Script {scriptName} failed.");
        }
        catch (Exception ex)
        {
            LoggerUtilities.Error($"Error running custom script: {ex.Message}");
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
            LoggerUtilities.Error($"Error reading package.json: {ex.Message}");
            _console.WriteError("Invalid package.json file");
        }

        Console.ReadLine();
    }

    private void ResetCache()
    {
        var promptView = new PromptView();
        if (promptView.ConfirmYesNo("Are you sure you want to reset the npm cache?", false))
        {
            if (EnsureNpmInstalled() &&
                _processService.RunProcess(GetNpmPath(), "cache clean --force", true, Environment.CurrentDirectory))
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

    private void ConfigureNpmPath()
    {
        _console.WriteInfo("Configure Node.js/NPM Path");
        _console.WriteInfo(
            "This will help you specify the exact location of npm on your system."); // First, try to auto-detect npm installation
        _console.WriteInfo("\nSearching for npm installation...");
        var npmPath = _processService.FindExecutablePath("npm");

        if (!string.IsNullOrEmpty(npmPath))
        {
            _console.WriteInfo($"Found npm at: {npmPath}");
            var useDetected = _prompt.ConfirmYesNo("Use this detected npm location?");

            if (useDetected)
            {
                // Save the detected path to config
                _console.Config.Set("npm_path", npmPath);
                _console.WriteSuccess("✓ npm path configured successfully!");
                return;
            }
        }
        else
        {
            _console.WriteInfo("Could not automatically detect npm installation.");
        }

        // Let user browse for the npm executable
        _console.WriteInfo("\nPlease enter the full path to the npm executable:");
        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            _console.WriteInfo("Example: C:\\Program Files\\nodejs\\npm.cmd");
        else
            _console.WriteInfo("Example: /usr/local/bin/npm");

        string? customPath = _prompt.Prompt("Enter npm path (or leave empty to cancel): ");

        if (string.IsNullOrWhiteSpace(customPath))
        {
            _console.WriteInfo("Operation cancelled.");
            return;
        }

        // Validate the path
        if (!File.Exists(customPath))
        {
            _console.WriteError("The specified file does not exist.");
            return;
        }

        // Test the specified path
        _console.WriteInfo("Testing the specified npm path...");
        var testProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = customPath,
                Arguments = "--version",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };

        try
        {
            testProcess.Start();
            string output = testProcess.StandardOutput.ReadToEnd();
            testProcess.WaitForExit();

            if (testProcess.ExitCode == 0 && !string.IsNullOrWhiteSpace(output))
            {
                _console.WriteSuccess($"✓ npm test successful! Version: {output.Trim()}");
                _console.Config.Set("npm_path", customPath);
                _console.WriteSuccess("✓ npm path saved to configuration!");
            }
            else
            {
                _console.WriteError("✗ The specified path does not appear to be a valid npm executable.");
            }
        }
        catch (Exception ex)
        {
            _console.WriteError($"✗ Error testing npm path: {ex.Message}");
        }
    }

    private void RunNpmDiagnostics()
    {
        _console.WriteInfo("===== NPM Configuration Diagnostics =====");
        _console.WriteInfo(
            "This will check your Node.js and npm installation and provide troubleshooting information.\n");
        // Step 1: Check if npm is in PATH
        _console.WriteInfo("Step 1: Checking if npm is accessible in PATH");
        bool npmInPath = _processService.RunProcess(GetNpmPath(), "--version", true, Environment.CurrentDirectory);
        if (npmInPath)
            _console.WriteSuccess("✓ npm is correctly configured in your PATH environment variable.");
        else
            _console.WriteError("✗ npm is not accessible via PATH environment variable.");

        // Step 2: Check if Node.js is installed
        _console.WriteInfo("\nStep 2: Checking Node.js installation");
        bool nodeInPath = _processService.RunProcess(GetNodePath(), "--version", true, Environment.CurrentDirectory);
        if (nodeInPath)
            _console.WriteSuccess("✓ Node.js is correctly configured in your PATH environment variable.");
        else
            _console.WriteError("✗ Node.js is not accessible via PATH environment variable.");

        // Step 3: Check for explicit configuration in EasyKit settings
        _console.WriteInfo("\nStep 3: Checking EasyKit configuration");
        var npmPathSetting = _console.Config.Get("npm_path", "");

        if (npmPathSetting != null && !string.IsNullOrWhiteSpace(npmPathSetting.ToString()))
        {
            string npmPathStr = npmPathSetting.ToString()!;
            _console.WriteInfo($"npm path is explicitly configured in EasyKit settings: {npmPathStr}");

            if (File.Exists(npmPathStr))
            {
                _console.WriteSuccess("✓ Configured npm path exists.");

                // Test the configured npm path
                var testProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = npmPathStr,
                        Arguments = "--version",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    }
                };

                try
                {
                    testProcess.Start();
                    string output = testProcess.StandardOutput.ReadToEnd();
                    testProcess.WaitForExit();

                    if (testProcess.ExitCode == 0 && !string.IsNullOrWhiteSpace(output))
                        _console.WriteSuccess($"✓ Configured npm is working. Version: {output.Trim()}");
                    else
                        _console.WriteError(
                            "✗ Configured npm path exists but does not appear to be working correctly.");
                }
                catch (Exception ex)
                {
                    _console.WriteError($"✗ Error testing configured npm path: {ex.Message}");
                }
            }
            else
            {
                _console.WriteError($"✗ Configured npm path does not exist: {npmPathStr}");
            }
        }
        else
        {
            _console.WriteInfo("No explicit npm path is configured in EasyKit settings.");
        } // Step 4: Search for npm in common locations

        _console.WriteInfo("\nStep 4: Searching for npm in common installation locations");
        var npmPath = _processService.FindExecutablePath("npm");

        if (!string.IsNullOrEmpty(npmPath))
        {
            _console.WriteSuccess($"✓ Found npm at: {npmPath}");

            if (npmPathSetting == null || string.IsNullOrWhiteSpace(npmPathSetting.ToString()) ||
                !npmPathSetting.ToString()!.Equals(npmPath, StringComparison.OrdinalIgnoreCase))
            {
                var configureFoundPath =
                    _prompt.ConfirmYesNo("Would you like to configure EasyKit to use this npm path?");

                if (configureFoundPath)
                {
                    _console.Config.Set("npm_path", npmPath);
                    _console.WriteSuccess("✓ npm path configured in EasyKit settings.");
                }
            }
        }
        else
        {
            _console.WriteError("✗ Could not find npm in common installation locations.");
        }

        // Step 5: Check PATH environment variable
        _console.WriteInfo("\nStep 5: Checking PATH environment variable");
        string? pathEnv = Environment.GetEnvironmentVariable("PATH");

        if (string.IsNullOrEmpty(pathEnv))
        {
            _console.WriteError("✗ PATH environment variable is empty or not accessible.");
        }
        else
        {
            string[] paths = pathEnv.Split(Path.PathSeparator);
            _console.WriteInfo($"PATH contains {paths.Length} directories.");

            // Look for directories likely to contain Node.js/npm
            bool foundNodePath = false;
            foreach (string path in paths)
            {
                if (string.IsNullOrWhiteSpace(path)) continue;

                if (path.Contains("node", StringComparison.OrdinalIgnoreCase) ||
                    path.Contains("npm", StringComparison.OrdinalIgnoreCase))
                {
                    if (Directory.Exists(path))
                    {
                        _console.WriteSuccess($"✓ Found potential Node.js path in PATH: {path}");
                        foundNodePath = true;

                        // Check if npm/node executables exist in this directory
                        string[] executableNames =
                        {
                            "npm", "npm.cmd", "npm.exe",
                            "node", "node.exe"
                        };

                        foreach (string exec in executableNames)
                        {
                            string fullPath = Path.Combine(path, exec);
                            if (File.Exists(fullPath)) _console.WriteSuccess($"  ✓ Found {exec} at {fullPath}");
                        }
                    }
                    else
                    {
                        _console.WriteError($"✗ PATH contains non-existent Node.js directory: {path}");
                    }
                }
            }

            if (!foundNodePath) _console.WriteError("✗ No Node.js/npm directories found in PATH.");
        }

        // Step 6: Provide recommendations
        _console.WriteInfo("\nStep 6: Recommendations");

        if (!npmInPath && !nodeInPath)
        {
            _console.WriteInfo("Based on the diagnostics, it appears that Node.js/npm is:");

            if (!string.IsNullOrEmpty(npmPath))
            {
                _console.WriteInfo("✓ Installed on your system but not in your PATH environment variable.");
                _console.WriteInfo("To fix this issue, you can:");
                _console.WriteInfo("1. Use the 'Configure npm path' option to explicitly set the path in EasyKit.");
                _console.WriteInfo("2. Add the following directory to your system PATH environment variable:");
                _console.WriteInfo($"   {Path.GetDirectoryName(npmPath)}");

                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    var openEnvVars =
                        _prompt.ConfirmYesNo("Would you like to open Environment Variables settings now?");

                    if (openEnvVars)
                        try
                        {
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = "rundll32.exe",
                                Arguments = "sysdm.cpl,EditEnvironmentVariables",
                                UseShellExecute = true
                            });
                            _console.WriteInfo("Environment Variables dialog opened.");
                        }
                        catch (Exception ex)
                        {
                            LoggerUtilities.Error($"Error opening Environment Variables: {ex.Message}");
                            _console.WriteError("Failed to open Environment Variables dialog.");
                        }
                }
            }
            else
            {
                _console.WriteInfo("✗ Not installed on your system or installed in a non-standard location.");
                _console.WriteInfo("To fix this issue, you can:");
                _console.WriteInfo("1. Install Node.js from https://nodejs.org/");
                _console.WriteInfo("2. Ensure the installer adds Node.js to your PATH.");
                _console.WriteInfo(
                    "3. Alternatively, download and install Node.js, then use 'Configure npm path' to set the path manually.");

                var openDownload = _prompt.ConfirmYesNo("Would you like to open the Node.js download page now?");

                if (openDownload)
                {
                    OpenNodejsWebsite();
                    return; // Skip the final ReadLine since OpenNodejsWebsite already has one
                }
            }
        }
        else if (npmInPath && nodeInPath)
        {
            _console.WriteSuccess("✓ Node.js and npm are properly installed and configured on your system.");
            _console.WriteInfo("If you're still experiencing issues with npm commands in EasyKit, try the following:");
            _console.WriteInfo("1. Restart EasyKit to ensure it picks up the latest environment variables.");
            _console.WriteInfo("2. If the problem persists, use 'Configure npm path' to explicitly set the npm path.");
            _console.WriteInfo("3. Check if your antivirus or security software might be blocking npm execution.");
        }
        else if (nodeInPath && !npmInPath)
        {
            _console.WriteInfo("✓ Node.js is properly installed, but npm is not accessible.");
            _console.WriteInfo("This is unusual since npm is included with Node.js. To fix this issue:");
            _console.WriteInfo("1. Try reinstalling Node.js from https://nodejs.org/");
            _console.WriteInfo("2. Ensure the installer adds Node.js to your PATH.");
            _console.WriteInfo(
                "3. You can also try running 'npm -v' in a new command prompt to see if the issue persists.");
        }

        _console.WriteInfo("\n===== End of NPM Configuration Diagnostics =====");
        Console.ReadLine();
    }
}