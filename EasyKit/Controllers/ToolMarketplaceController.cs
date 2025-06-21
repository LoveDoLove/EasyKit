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
using EasyKit.Services;

namespace EasyKit.Controllers;

public class ToolMarketplaceController
{
    private readonly ConsoleService _console;

    private readonly List<ToolInfo> _essentialTools = new()
    {
        new ToolInfo("Node.js", "node", "https://nodejs.org/", "--version"),
        new ToolInfo("npm", "npm", "https://nodejs.org/", "--version"),
        new ToolInfo("PHP", "php", "https://www.php.net/downloads", "--version"),
        new ToolInfo("Composer", "composer", "https://getcomposer.org/download/", "--version"),
        new ToolInfo("Git", "git", "https://git-scm.com/downloads", "--version")
    };

    private readonly ProcessService _processService;

    public ToolMarketplaceController(ProcessService processService, ConsoleService console)
    {
        _processService = processService;
        _console = console;
    }

    public void ShowMarketplace()
    {
        _console.WriteInfo("\n=== Project Tool Marketplace ===\n");
        var installed = new List<ToolInfo>();
        var missing = new List<ToolInfo>();

        foreach (var tool in _essentialTools)
            if (IsToolInstalled(tool))
                installed.Add(tool);
            else
                missing.Add(tool);

        _console.WriteInfo("Installed tools:");
        foreach (var tool in installed)
            _console.WriteSuccess($"  ✓ {tool.Name}");

        _console.WriteInfo("\nMissing tools:");
        foreach (var tool in missing)
            _console.WriteError($"  ✗ {tool.Name}");

        if (missing.Count > 0)
        {
            _console.WriteInfo("\nSelect a tool to install (number), or press Enter to exit:");
            for (int i = 0; i < missing.Count; i++)
                _console.WriteInfo($"  [{i + 1}] {missing[i].Name} - {missing[i].DownloadUrl}");

            var input = Console.ReadLine();
            if (int.TryParse(input, out int choice) && choice > 0 && choice <= missing.Count)
                InstallTool(missing[choice - 1]);
        }
        else
        {
            _console.WriteSuccess("\nAll essential tools are installed!");
        }
    }

    private bool IsToolInstalled(ToolInfo tool)
    {
        // Use the new minimal process service: just try to run the tool
        var (output, error, exitCode) = _processService.RunProcess(tool.Command, tool.VersionArgs);
        return exitCode == 0 && !string.IsNullOrWhiteSpace(output);
    }

    private void InstallTool(ToolInfo tool)
    {
        _console.WriteInfo($"\nTo install {tool.Name}, visit: {tool.DownloadUrl}");
        try
        {
            Process.Start(new ProcessStartInfo { FileName = tool.DownloadUrl, UseShellExecute = true });
            _console.WriteInfo($"[INFO] Opened download page for {tool.Name}.");
        }
        catch (Exception ex)
        {
            _console.WriteError($"[ERROR] Failed to open browser: {ex.Message}");
        }
    }

    private record ToolInfo(string Name, string Command, string DownloadUrl, string VersionArgs);
}