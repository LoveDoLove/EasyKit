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

using EasyKit.Helpers.Console;
using EasyKit.Models;
using EasyKit.Services;
using EasyKit.UI.ConsoleUI;

namespace EasyKit.Controllers;

/// <summary>
///     Controller for uv Python package/project/tool management.
/// </summary>
public class UvController
{
    private const string UV = "uv";
    private readonly ConfirmationHelper _confirmation;
    private readonly ConsoleService _console;
    private readonly NotificationView _notificationView;
    private readonly SecureProcessRunner _processRunner;
    private readonly PromptView _prompt;
    private readonly ProjectDetector _projectDetector;

    public UvController(
        ConsoleService console,
        ConfirmationHelper confirmation,
        PromptView prompt,
        NotificationView notificationView,
        ProjectDetector? projectDetector = null)
    {
        _console = console;
        _confirmation = confirmation;
        _prompt = prompt;
        _notificationView = notificationView;
        _processRunner = new SecureProcessRunner();
        _projectDetector = projectDetector ?? new ProjectDetector();
    }

    public void ShowMenu()
    {
        if (!EnsureUvInstalled()) return;

        var menuView = new MenuView();
        var menu = menuView.CreateMenu("uv Tools", width: 100);
        
        menu.AddOption("1", "Initialize project (uv init)", () => InitProject())
            .AddOption("2", "Add dependency (uv add)", () => AddDependency())
            .AddOption("3", "Add dev dependency (uv add --dev)", () => AddDevDependency())
            .AddOption("4", "Remove dependency (uv remove)", () => RemoveDependency())
            .AddOption("5", "Sync dependencies (uv sync)", () => SyncDependencies())
            .AddOption("6", "Lock dependencies (uv lock)", () => LockDependencies())
            .AddOption("7", "Run script (uv run)", () => RunScript())
            .AddOption("8", "Install tool (uv tool install)", () => InstallTool())
            .AddOption("9", "List tools (uv tool list)", () => ListTools())
            .AddOption("10", "Upgrade tools (uv tool upgrade)", () => UpgradeTools())
            .AddOption("11", "Run with uvx (uvx)", () => RunUvx())
            .AddOption("12", "Install Python (uv python install)", () => InstallPython())
            .AddOption("13", "List Python versions (uv python list)", () => ListPythonVersions())
            .AddOption("14", "Pin Python version (uv python pin)", () => PinPythonVersion())
            .AddOption("15", "Create venv (uv venv)", () => CreateVenv())
            .AddOption("16", "pip install (uv pip install)", () => PipInstall())
            .AddOption("17", "pip uninstall (uv pip uninstall)", () => PipUninstall())
            .AddOption("18", "Run diagnostics", () => RunDiagnostics())
            .AddOption("0", "Back to main menu", () => { /* Return */ });

        menu.WithColors(
            ConsoleColor.DarkGreen,
            ConsoleColor.Green,
            ConsoleColor.Yellow,
            ConsoleColor.White,
            ConsoleColor.DarkGray)
            .WithHelpText("Select an option or press 0 to return to the main menu")
            .WithDoubleBorder()
            .Show();
    }

    private bool EnsureUvInstalled()
    {
        var (output, error, exitCode) = _processRunner.RunProcess(UV, ["--version"]);
        if (exitCode != 0 || string.IsNullOrWhiteSpace(output))
        {
            _console.WriteError("uv is not installed or not accessible via PATH.");
            _console.WriteInfo("To install uv, visit: https://astral.sh/uv");
            _console.WriteInfo("Press Enter to continue...");
            Console.ReadLine();
            return false;
        }
        _console.WriteSuccess($"✓ uv is installed. Version: {output.Trim()}");
        return true;
    }

    private void InitProject()
    {
        _console.WriteInfo("Initializing new Python project...");
        var name = _prompt.Prompt("Project name (default: my-project): ") ?? "my-project";
        if (string.IsNullOrWhiteSpace(name)) name = "my-project";
        
        var (output, error, exitCode) = _processRunner.RunProcess(UV, ["init", name]);
        if (exitCode == 0)
            _console.WriteSuccess($"✓ Project '{name}' initialized successfully!");
        else
            _console.WriteError($"Failed to initialize project: {error}");
        
        WaitForUser();
    }

    private void AddDependency()
    {
        var package = _prompt.Prompt("Package name (e.g. 'requests'): ");
        if (string.IsNullOrWhiteSpace(package))
        {
            _console.WriteError("Package name cannot be empty.");
            WaitForUser();
            return;
        }

        _console.WriteInfo($"Adding dependency '{package}'...");
        var (output, error, exitCode) = _processRunner.RunProcess(UV, ["add", package]);
        if (exitCode == 0)
            _console.WriteSuccess($"✓ Added '{package}' successfully!");
        else
            _console.WriteError($"Failed to add package: {error}");
        
        WaitForUser();
    }

    private void AddDevDependency()
    {
        var package = _prompt.Prompt("Dev package name (e.g. 'pytest'): ");
        if (string.IsNullOrWhiteSpace(package))
        {
            _console.WriteError("Package name cannot be empty.");
            WaitForUser();
            return;
        }

        _console.WriteInfo($"Adding dev dependency '{package}'...");
        var (output, error, exitCode) = _processRunner.RunProcess(UV, ["add", "--dev", package]);
        if (exitCode == 0)
            _console.WriteSuccess($"✓ Added dev dependency '{package}' successfully!");
        else
            _console.WriteError($"Failed to add dev package: {error}");
        
        WaitForUser();
    }

    private void RemoveDependency()
    {
        var package = _prompt.Prompt("Package name to remove: ");
        if (string.IsNullOrWhiteSpace(package))
        {
            _console.WriteError("Package name cannot be empty.");
            WaitForUser();
            return;
        }

        _console.WriteInfo($"Removing dependency '{package}'...");
        var (output, error, exitCode) = _processRunner.RunProcess(UV, ["remove", package]);
        if (exitCode == 0)
            _console.WriteSuccess($"✓ Removed '{package}' successfully!");
        else
            _console.WriteError($"Failed to remove package: {error}");
        
        WaitForUser();
    }

    private void SyncDependencies()
    {
        _console.WriteInfo("Syncing dependencies...");
        var exitCode = _processRunner.RunProcessStreaming(
            UV, 
            ["sync"],
            onOutput: line => _console.WriteInfo(line),
            onError: line => _console.WriteError(line));
        
        if (exitCode == 0)
            _console.WriteSuccess("✓ Dependencies synced successfully!");
        else
            _console.WriteError("Failed to sync dependencies.");
        
        WaitForUser();
    }

    private void LockDependencies()
    {
        _console.WriteInfo("Generating lockfile...");
        var (output, error, exitCode) = _processRunner.RunProcess(UV, ["lock"]);
        if (exitCode == 0)
            _console.WriteSuccess("✓ Lockfile generated successfully!");
        else
            _console.WriteError($"Failed to generate lockfile: {error}");
        
        WaitForUser();
    }

    private void RunScript()
    {
        var script = _prompt.Prompt("Script name or command (e.g. 'pytest' or 'python -m pytest'): ");
        if (string.IsNullOrWhiteSpace(script))
        {
            _console.WriteError("Script name cannot be empty.");
            WaitForUser();
            return;
        }

        _console.WriteInfo($"Running script: {script}...");
        var exitCode = _processRunner.RunProcessStreaming(
            UV,
            ["run", script],
            onOutput: line => _console.WriteInfo(line),
            onError: line => _console.WriteError(line));
        
        if (exitCode == 0)
            _console.WriteSuccess("✓ Script completed successfully!");
        else
            _console.WriteError("Script failed.");
        
        WaitForUser();
    }

    private void InstallTool()
    {
        var tool = _prompt.Prompt("Tool name to install (e.g. 'ruff', 'black'): ");
        if (string.IsNullOrWhiteSpace(tool))
        {
            _console.WriteError("Tool name cannot be empty.");
            WaitForUser();
            return;
        }

        _console.WriteInfo($"Installing tool '{tool}'...");
        var (output, error, exitCode) = _processRunner.RunProcess(UV, ["tool", "install", tool]);
        if (exitCode == 0)
            _console.WriteSuccess($"✓ Tool '{tool}' installed successfully!");
        else
            _console.WriteError($"Failed to install tool: {error}");
        
        WaitForUser();
    }

    private void ListTools()
    {
        _console.WriteInfo("Installed tools:");
        var exitCode = _processRunner.RunProcessStreaming(
            UV,
            ["tool", "list"],
            onOutput: line => _console.WriteInfo(line));
    }

    private void UpgradeTools()
    {
        if (!_confirmation.ConfirmAction("Are you sure you want to upgrade all tools?", false))
        {
            _console.WriteInfo("Upgrade cancelled.");
            WaitForUser();
            return;
        }

        _console.WriteInfo("Upgrading tools...");
        var (output, error, exitCode) = _processRunner.RunProcess(UV, ["tool", "upgrade"]);
        if (exitCode == 0)
            _console.WriteSuccess("✓ Tools upgraded successfully!");
        else
            _console.WriteError($"Failed to upgrade tools: {error}");
        
        WaitForUser();
    }

    private void RunUvx()
    {
        var tool = _prompt.Prompt("Tool to run with uvx (e.g. 'ruff check'): ");
        if (string.IsNullOrWhiteSpace(tool))
        {
            _console.WriteError("Command cannot be empty.");
            WaitForUser();
            return;
        }

        _console.WriteInfo($"Running '{tool}' with uvx...");
        var exitCode = _processRunner.RunProcessStreaming(
            UV,
            ["uvx", tool],
            onOutput: line => _console.WriteInfo(line),
            onError: line => _console.WriteError(line));
        
        if (exitCode == 0)
            _console.WriteSuccess("✓ Command completed successfully!");
        else
            _console.WriteError("Command failed.");
        
        WaitForUser();
    }

    private void InstallPython()
    {
        var version = _prompt.Prompt("Python version to install (e.g. '3.12'): ");
        if (string.IsNullOrWhiteSpace(version))
        {
            _console.WriteError("Version cannot be empty.");
            WaitForUser();
            return;
        }

        _console.WriteInfo($"Installing Python {version}...");
        var (output, error, exitCode) = _processRunner.RunProcess(UV, ["python", "install", version]);
        if (exitCode == 0)
            _console.WriteSuccess($"✓ Python {version} installed successfully!");
        else
            _console.WriteError($"Failed to install Python: {error}");
        
        WaitForUser();
    }

    private void ListPythonVersions()
    {
        _console.WriteInfo("Installed Python versions:");
        var exitCode = _processRunner.RunProcessStreaming(
            UV,
            ["python", "list"],
            onOutput: line => _console.WriteInfo(line));
    }

    private void PinPythonVersion()
    {
        var version = _prompt.Prompt("Python version to pin (e.g. '3.12'): ");
        if (string.IsNullOrWhiteSpace(version))
        {
            _console.WriteError("Version cannot be empty.");
            WaitForUser();
            return;
        }

        _console.WriteInfo($"Pinning Python version to {version}...");
        var (output, error, exitCode) = _processRunner.RunProcess(UV, ["python", "pin", version]);
        if (exitCode == 0)
            _console.WriteSuccess($"✓ Python version pinned to {version}!");
        else
            _console.WriteError($"Failed to pin version: {error}");
        
        WaitForUser();
    }

    private void CreateVenv()
    {
        _console.WriteInfo("Creating virtual environment...");
        var (output, error, exitCode) = _processRunner.RunProcess(UV, ["venv"]);
        if (exitCode == 0)
            _console.WriteSuccess("✓ Virtual environment created successfully!");
        else
            _console.WriteError($"Failed to create venv: {error}");
        
        WaitForUser();
    }

    private void PipInstall()
    {
        var package = _prompt.Prompt("Package name to install: ");
        if (string.IsNullOrWhiteSpace(package))
        {
            _console.WriteError("Package name cannot be empty.");
            WaitForUser();
            return;
        }

        _console.WriteInfo($"Installing '{package}' via pip...");
        var (output, error, exitCode) = _processRunner.RunProcess(UV, ["pip", "install", package]);
        if (exitCode == 0)
            _console.WriteSuccess($"✓ Installed '{package}' successfully!");
        else
            _console.WriteError($"Failed to install package: {error}");
        
        WaitForUser();
    }

    private void PipUninstall()
    {
        var package = _prompt.Prompt("Package name to uninstall: ");
        if (string.IsNullOrWhiteSpace(package))
        {
            _console.WriteError("Package name cannot be empty.");
            WaitForUser();
            return;
        }

        _console.WriteInfo($"Uninstalling '{package}' via pip...");
        var (output, error, exitCode) = _processRunner.RunProcess(UV, ["pip", "uninstall", package]);
        if (exitCode == 0)
            _console.WriteSuccess($"✓ Uninstalled '{package}' successfully!");
        else
            _console.WriteError($"Failed to uninstall package: {error}");
        
        WaitForUser();
    }

    private void RunDiagnostics()
    {
        _console.WriteInfo("===== UV Configuration Diagnostics =====\n");

        // Check uv
        var (uvOutput, uvError, uvExit) = _processRunner.RunProcess(UV, ["--version"]);
        if (uvExit == 0 && !string.IsNullOrWhiteSpace(uvOutput))
        {
            _console.WriteSuccess($"✓ uv is installed. Version: {uvOutput.Trim()}");
        }
        else
        {
            _console.WriteError("✗ uv is not installed or not accessible via PATH.");
            _console.WriteInfo("To install uv, visit: https://astral.sh/uv");
            WaitForUser();
            return;
        }

        // Check Python
        var (pyOutput, pyError, pyExit) = _processRunner.RunProcess("python", ["--version"]);
        if (pyExit == 0 && !string.IsNullOrWhiteSpace(pyOutput))
        {
            _console.WriteSuccess($"✓ Python is accessible. Version: {pyOutput.Trim()}");
        }
        else
        {
            _console.WriteError("✗ Python is not accessible via PATH.");
        }

        // Check pyproject.toml
        _console.WriteInfo("\nChecking project files...");
        if (File.Exists("pyproject.toml"))
        {
            _console.WriteSuccess("✓ pyproject.toml found");
        }
        else
        {
            _console.WriteInfo("  No pyproject.toml found");
        }

        if (File.Exists("uv.lock"))
        {
            _console.WriteSuccess("✓ uv.lock found");
        }
        else
        {
            _console.WriteInfo("  No uv.lock found");
        }

        if (Directory.Exists(".venv"))
        {
            _console.WriteSuccess("✓ .venv directory found");
        }
        else
        {
            _console.WriteInfo("  No .venv directory found");
        }

        _console.WriteInfo("\n===== End of UV Configuration Diagnostics =====");
        WaitForUser();
    }

    private void WaitForUser()
    {
        _console.WriteInfo("\nPress Enter to continue...");
        Console.ReadLine();
    }
}
