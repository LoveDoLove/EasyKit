using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace CommonUtilities.Helpers;

/// <summary>
/// Provides static helpers for process execution, diagnostics, and environment checks.
/// </summary>
public static class ProcessHelper
{
    /// <summary>
    /// Runs a process asynchronously and returns output, error, and exit code.
    /// </summary>
    public static async Task<(string output, string error, int exitCode)> RunProcessAsync(
        string fileName,
        string arguments,
        string? workingDirectory = null,
        int timeoutMilliseconds = 300000,
        CancellationToken cancellationToken = default)
    {
        using var process = new Process();
        process.StartInfo = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        if (!string.IsNullOrEmpty(workingDirectory))
            process.StartInfo.WorkingDirectory = workingDirectory;

        var outputBuilder = new StringBuilder();
        var errorBuilder = new StringBuilder();

        process.OutputDataReceived += (_, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
                outputBuilder.AppendLine(e.Data);
        };
        process.ErrorDataReceived += (_, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
                errorBuilder.AppendLine(e.Data);
        };

        try
        {
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            var processTask = process.WaitForExitAsync(cancellationToken);
            var timeoutTask = Task.Delay(timeoutMilliseconds, cancellationToken);

            if (await Task.WhenAny(processTask, timeoutTask) == timeoutTask)
                if (!process.HasExited)
                {
                    try { process.Kill(true); } catch { }
                    throw new TimeoutException($"Process timed out: {fileName} {arguments}");
                }

            return (outputBuilder.ToString(), errorBuilder.ToString(), process.ExitCode);
        }
        catch (OperationCanceledException)
        {
            if (!process.HasExited)
                try { process.Kill(true); } catch { }
            throw;
        }
        catch (Win32Exception ex)
        {
            throw new InvalidOperationException($"Command not found: {fileName}. {ex.Message}", ex);
        }
        catch (Exception)
        {
            if (!process.HasExited)
                try { process.Kill(true); } catch { }
            throw;
        }
    }

    /// <summary>
    /// Runs a process synchronously and returns output, error, and exit code.
    /// </summary>
    public static (string output, string error, int exitCode) RunProcess(
        string fileName,
        string arguments,
        string? workingDirectory = null)
    {
        var process = new Process();
        process.StartInfo = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        if (!string.IsNullOrEmpty(workingDirectory))
            process.StartInfo.WorkingDirectory = workingDirectory;

        process.Start();
        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();
        process.WaitForExit();

        return (output, error, process.ExitCode);
    }

    /// <summary>
    /// Finds the explicit path for a given executable name, searching common locations.
    /// </summary>
    public static string? FindExecutablePath(string executableName)
    {
        string[] searchPaths = GetSearchPathsForExecutable(executableName);
        foreach (var path in searchPaths)
            if (File.Exists(path))
                return path;
        return null;
    }

    /// <summary>
    /// Gets an array of potential paths where an executable might be found based on OS.
    /// </summary>
    private static string[] GetSearchPathsForExecutable(string executableName)
    {
        string extension = Environment.OSVersion.Platform == PlatformID.Win32NT ? ".exe" : "";
        var paths = new List<string>();

        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
        {
            string programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            string programFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            switch (executableName.ToLower())
            {
                case "npm":
                case "node":
                    paths.Add(Path.Combine(programFiles, "nodejs", $"{executableName}{extension}"));
                    paths.Add(Path.Combine(programFilesX86, "nodejs", $"{executableName}{extension}"));
                    paths.Add(Path.Combine(appData, "npm", $"{executableName}{extension}"));
                    break;
                case "php":
                    paths.Add(Path.Combine(programFiles, "PHP", $"{executableName}{extension}"));
                    paths.Add(Path.Combine(programFilesX86, "PHP", $"{executableName}{extension}"));
                    break;
                case "composer":
                    paths.Add(Path.Combine(appData, "Composer", "composer.phar"));
                    break;
            }
        }
        else
        {
            string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            switch (executableName.ToLower())
            {
                case "npm":
                case "node":
                    paths.Add(Path.Combine("/usr", "local", "bin", executableName));
                    paths.Add(Path.Combine("/usr", "bin", executableName));
                    break;
                case "php":
                    paths.Add(Path.Combine("/usr", "local", "bin", executableName));
                    paths.Add(Path.Combine("/usr", "bin", executableName));
                    break;
                case "composer":
                    paths.Add(Path.Combine("/usr", "local", "bin", executableName));
                    paths.Add(Path.Combine("/usr", "bin", executableName));
                    break;
            }
        }
        return paths.ToArray();
    }

    /// <summary>
    /// Gets PHP version info (version, path, isCompatible).
    /// </summary>
    public static (string version, string path, bool isCompatible) GetPhpVersionInfo()
    {
        string phpPath = "php";
        string version = "Unknown";
        bool isCompatible = false;

        var explicitPath = FindExecutablePath("php");
        if (!string.IsNullOrEmpty(explicitPath))
            phpPath = explicitPath;

        var (output, _, exitCode) = RunProcess(phpPath, "--version");
        if (exitCode == 0 && !string.IsNullOrWhiteSpace(output))
        {
            var versionMatch = Regex.Match(output, @"PHP\s+(\d+\.\d+\.\d+)");
            if (versionMatch.Success)
            {
                version = versionMatch.Groups[1].Value;
                var versionParts = version.Split('.');
                if (versionParts.Length >= 2 &&
                    int.TryParse(versionParts[0], out int major) &&
                    int.TryParse(versionParts[1], out int minor))
                    isCompatible = major > 7 || (major == 7 && minor >= 3);
            }
        }
        return (version, phpPath, isCompatible);
    }

    /// <summary>
    /// Checks if required PHP extensions are installed.
    /// </summary>
    public static (List<string> missingExtensions, bool isCompatible) CheckPhpExtensions(string phpPath = "php")
    {
        var requiredExtensions = new List<string>
        {
            "BCMath", "Ctype", "Fileinfo", "JSON", "Mbstring", "OpenSSL", "PDO", "Tokenizer", "XML", "cURL"
        };
        var missingExtensions = new List<string>();
        bool isCompatible = true;

        var (output, _, exitCode) = RunProcess(phpPath, "-m");
        if (exitCode == 0 && !string.IsNullOrWhiteSpace(output))
        {
            var loadedExtensions = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(ext => ext.Trim().ToLower()).ToList();
            foreach (var ext in requiredExtensions)
                if (!loadedExtensions.Contains(ext.ToLower()))
                    missingExtensions.Add(ext);
            isCompatible = !missingExtensions.Any(ext =>
                ext == "JSON" || ext == "PDO" || ext == "OpenSSL" || ext == "Mbstring");
        }
        else
        {
            isCompatible = false;
            missingExtensions = requiredExtensions;
        }
        return (missingExtensions, isCompatible);
    }

    /// <summary>
    /// Gets Composer version info (version, path, isGlobal).
    /// </summary>
    public static (string version, string path, bool isGlobal) GetComposerInfo()
    {
        string version = "Unknown";
        string composerPath = "composer";
        bool isGlobal = false;
        var explicitPath = FindExecutablePath("composer");
        if (!string.IsNullOrEmpty(explicitPath))
        {
            if (explicitPath.EndsWith(".phar", StringComparison.OrdinalIgnoreCase))
            {
                composerPath = "php " + explicitPath;
                isGlobal = false;
            }
            else
            {
                composerPath = explicitPath;
                isGlobal = true;
            }
        }
        if (File.Exists("composer.phar"))
        {
            composerPath = "php composer.phar";
            isGlobal = false;
        }
        var (output, _, exitCode) = composerPath.StartsWith("php ")
            ? RunProcess("php", composerPath.Substring(4) + " --version")
            : RunProcess(composerPath, "--version");
        if (exitCode == 0 && !string.IsNullOrWhiteSpace(output))
        {
            var versionMatch = Regex.Match(output, @"Composer version (\d+\.\d+\.\d+)");
            if (versionMatch.Success) version = versionMatch.Groups[1].Value;
        }
        return (version, composerPath, isGlobal);
    }

    /// <summary>
    /// Gets Laravel version info (version, isCompatible).
    /// </summary>
    public static (string version, bool isCompatible) GetLaravelVersionInfo(string? workingDirectory = null)
    {
        string version = "Unknown";
        bool isCompatible = false;
        string directory = workingDirectory ?? Environment.CurrentDirectory;
        if (!File.Exists(Path.Combine(directory, "artisan"))) return (version, isCompatible);
        var (output, _, exitCode) = RunProcess("php", "artisan --version", directory);
        if (exitCode == 0 && !string.IsNullOrWhiteSpace(output))
        {
            var versionMatch = Regex.Match(output, @"Laravel Framework\s+(\d+\.\d+\.?\d*)");
            if (versionMatch.Success)
            {
                version = versionMatch.Groups[1].Value;
                if (version.Contains('.'))
                {
                    string majorVersionStr = version.Split('.')[0];
                    if (int.TryParse(majorVersionStr, out int majorVersion))
                        isCompatible = majorVersion >= 6;
                }
            }
        }
        return (version, isCompatible);
    }
}