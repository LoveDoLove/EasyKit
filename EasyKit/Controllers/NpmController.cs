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

public class NpmController
{
    private const string NPM = "npm";
    private const string NODE = "node";
    private const string NCU = "ncu";
    private readonly ConfirmationHelper _confirmationHelper;
    private readonly ConsoleService _console;
    private readonly NotificationView _notificationView;
    private readonly CmdService _processService;
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
        _processService = new CmdService();
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
        if (!EnsureNpmInstalled()) return;

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
        // Use streaming for version check (quick, but user sees output)
        _processService.RunProcessWithStreaming(NPM, "--version", Environment.CurrentDirectory);
        return true; // Assume success for MVP
    }

    private void InstallPackages()
    {
        _console.WriteInfo("Installing npm packages...");
        if (EnsureNpmInstalled())
        {
            _processService.RunProcessInNewCmdWindow(NPM, "install", Environment.CurrentDirectory);
            _console.WriteInfo("Started 'npm install' in a new command window.");
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

        _processService.RunProcessInNewCmdWindow(NCU, "--version", Environment.CurrentDirectory);
        _console.WriteInfo("Started 'ncu --version' in a new command window.");
        _processService.RunProcessInNewCmdWindow(NPM, "install -g npm-check-updates", Environment.CurrentDirectory);
        _console.WriteInfo("Started 'npm install -g npm-check-updates' in a new command window.");
        _processService.RunProcessInNewCmdWindow(NCU, "-i", Environment.CurrentDirectory);
        _console.WriteInfo("Started 'ncu -i' in a new command window.");
        Console.ReadLine();
    }

    private void BuildProduction()
    {
        if (EnsureNpmInstalled())
        {
            _processService.RunProcessInNewCmdWindow(NPM, "run build", Environment.CurrentDirectory);
            _console.WriteInfo("Started 'npm run build' in a new command window.");
        }

        Console.ReadLine();
    }

    private void BuildDevelopment()
    {
        if (EnsureNpmInstalled())
        {
            _processService.RunProcessInNewCmdWindow(NPM, "run dev", Environment.CurrentDirectory);
            _console.WriteInfo("Started 'npm run dev' in a new command window.");
        }

        Console.ReadLine();
    }

    private void SecurityAudit()
    {
        if (EnsureNpmInstalled())
        {
            _processService.RunProcessWithStreaming(NPM, "audit", Environment.CurrentDirectory);
            _console.WriteInfo("Ran 'npm audit' with streaming output.");
        }

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

        var script = _prompt.Prompt("Enter npm script name (e.g. 'start'): ");
        _processService.RunProcessInNewCmdWindow(NPM, $"run {script}", Environment.CurrentDirectory);
        _console.WriteInfo($"Started 'npm run {script}' in a new command window.");
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
        if (_prompt.ConfirmYesNo("Are you sure you want to reset the npm cache?", false))
        {
            if (EnsureNpmInstalled())
            {
                _processService.RunProcessWithStreaming(NPM, "cache clean --force", Environment.CurrentDirectory);
                _console.WriteInfo("Ran 'npm cache clean --force' with streaming output.");
            }
            else
            {
                _console.WriteError("npm is not installed.");
            }
        }
        else
        {
            _console.WriteInfo("Reset cache canceled.");
        }

        Console.ReadLine();
    }
}