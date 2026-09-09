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
///     EasyKit Doctor - Comprehensive diagnostic tool for development environment.
/// </summary>
public class DoctorController
{
    private readonly ConsoleService _console;
    private readonly SecureProcessRunner _processRunner;
    private readonly ProjectDetector _projectDetector;
    private readonly List<ToolStatus> _tools = new();

    public DoctorController(ConsoleService console, ProjectDetector? projectDetector = null)
    {
        _console = console;
        _processRunner = new SecureProcessRunner();
        _projectDetector = projectDetector ?? new ProjectDetector();
    }

    public void Run()
    {
        _console.WriteInfo("===== EasyKit Doctor =====\n");
        _console.WriteInfo("Checking development tools and environment...\n");

        // Check tools
        CheckTool("Git", "git", "--version");
        CheckTool("Node.js", "node", "--version");
        CheckTool("npm", "npm", "--version");
        CheckTool("pnpm", "pnpm", "--version");
        CheckTool("Corepack", "corepack", "--version");
        CheckTool("Python", "python", "--version");
        CheckTool("uv", "uv", "--version");
        CheckTool("PHP", "php", "--version");
        CheckTool("Composer", "composer", "--version");
        CheckTool(".NET SDK", "dotnet", "--version");
        CheckTool("Docker", "docker", "--version");

        // Check PATH
        _console.WriteInfo("\n--- PATH Check ---");
        var path = Environment.GetEnvironmentVariable("PATH");
        if (!string.IsNullOrEmpty(path))
        {
            var paths = path.Split(Path.PathSeparator);
            _console.WriteInfo($"PATH contains {paths.Length} entries:");
            foreach (var p in paths.Take(10))
                _console.WriteInfo($"  {p}");
            if (paths.Length > 10)
                _console.WriteInfo($"  ... and {paths.Length - 10} more");
        }

        // Check project type
        _console.WriteInfo("\n--- Project Detection ---");
        var detection = _projectDetector.Detect(Environment.CurrentDirectory);
        if (detection.Types.Count == 0)
            _console.WriteInfo("No specific project type detected in current directory.");
        else
        {
            _console.WriteSuccess($"Detected project types: {string.Join(", ", detection.Types)}");
            _console.WriteInfo($"Primary package manager: {_projectDetector.GetPrimaryPackageManager(Environment.CurrentDirectory)}");
        }

        // Summary
        _console.WriteInfo("\n--- Summary ---");
        var installed = _tools.Count(t => t.Status == ToolStatusType.Installed);
        var missing = _tools.Count(t => t.Status == ToolStatusType.NotInstalled);
        var outdated = _tools.Count(t => t.Status == ToolStatusType.Outdated);
        var broken = _tools.Count(t => t.Status == ToolStatusType.Broken);

        _console.WriteSuccess($"Installed: {installed}");
        if (outdated > 0)
            _console.WriteInfo($"Outdated: {outdated}");
        if (broken > 0)
            _console.WriteError($"Broken: {broken}");
        if (missing > 0)
            _console.WriteInfo($"Missing: {missing}");

        // Fix suggestions
        if (missing > 0)
        {
            _console.WriteInfo("\n--- Fix Suggestions ---");
            foreach (var tool in _tools.Where(t => t.Status == ToolStatusType.NotInstalled))
            {
                _console.WriteInfo($"To install {tool.Name}:");
                switch (tool.Name)
                {
                    case "Git":
                        _console.WriteInfo("  Visit https://git-scm.com/downloads");
                        break;
                    case "Node.js":
                        _console.WriteInfo("  Visit https://nodejs.org/");
                        break;
                    case "pnpm":
                        _console.WriteInfo("  Run: npm install -g pnpm");
                        break;
                    case "Python":
                        _console.WriteInfo("  Visit https://www.python.org/downloads/");
                        break;
                    case "uv":
                        _console.WriteInfo("  Visit https://astral.sh/uv");
                        break;
                    case "PHP":
                        _console.WriteInfo("  Visit https://windows.php.net/download/");
                        break;
                    case "Composer":
                        _console.WriteInfo("  Visit https://getcomposer.org/download/");
                        break;
                    case ".NET SDK":
                        _console.WriteInfo("  Visit https://dotnet.microsoft.com/download");
                        break;
                    case "Docker":
                        _console.WriteInfo("  Visit https://www.docker.com/products/docker-desktop");
                        break;
                }
            }
        }

        _console.WriteInfo("\n===== End of EasyKit Doctor =====");
        Console.ReadLine();
    }

    private void CheckTool(string name, string command, string versionArg)
    {
        ToolStatus tool;
        try
        {
            var (output, error, exitCode) = _processRunner.RunProcess(command, [versionArg]);

            tool = new ToolStatus
            {
                Name = name,
                Command = command,
                VersionArgs = versionArg,
                Status = exitCode == 0 && !string.IsNullOrWhiteSpace(output)
                    ? ToolStatusType.Installed
                    : ToolStatusType.NotInstalled,
                Version = exitCode == 0 ? output.Trim() : null,
                Error = exitCode != 0 ? error : null
            };
        }
        catch (Exception ex) when (ex is System.ComponentModel.Win32Exception || ex is System.IO.FileNotFoundException)
        {
            tool = new ToolStatus
            {
                Name = name,
                Command = command,
                VersionArgs = versionArg,
                Status = ToolStatusType.NotInstalled,
                Error = ex.Message
            };
        }

        _tools.Add(tool);

        if (tool.Status == ToolStatusType.Installed)
        {
            _console.WriteSuccess($"✓ {name}: {tool.Version}");
        }
        else
        {
            _console.WriteError($"✗ {name}: Not found or not accessible");
        }
    }
}

/// <summary>
///     Status of a detected tool.
/// </summary>
public record ToolStatus
{
    public string Name { get; init; } = "";
    public string Command { get; init; } = "";
    public string VersionArgs { get; init; } = "";
    public ToolStatusType Status { get; init; }
    public string? Version { get; init; }
    public string? Error { get; init; }
}

/// <summary>
///     Tool installation status.
/// </summary>
public enum ToolStatusType
{
    Installed,
    NotInstalled,
    Outdated,
    Broken,
    Unknown
}
