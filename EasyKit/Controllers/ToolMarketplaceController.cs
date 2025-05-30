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
        // Use the same detection logic as the rest of EasyKit
        var exePath = _processService.FindExecutablePath(tool.Command);
        if (string.IsNullOrEmpty(exePath) || !File.Exists(exePath))
            return false;
        var (output, error, exitCode) = _processService.RunProcessWithOutput(exePath, tool.VersionArgs);
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