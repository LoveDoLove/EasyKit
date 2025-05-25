using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;
using CommonUtilities.Config;
using CommonUtilities.Utilities;

namespace EasyKit.Services;

/// <summary>
///     Service for handling process execution operations across the application.
/// </summary>
public class ProcessService
{
    private readonly Config? _config;
    private readonly ConsoleService _console;

    /// <summary>
    ///     ProcessService constructor using the new Config class.
    /// </summary>
    public ProcessService(ConsoleService console, Config? config = null)
    {
        _console = console;
        _config = config;
    }

    /// <summary>
    ///     Runs a process with the specified parameters.
    /// </summary>
    /// <param name="file">The executable file to run.</param>
    /// <param name="args">The arguments to pass to the executable.</param>
    /// <param name="showOutput">Whether to show the output in the console.</param>
    /// <param name="workingDirectory">
    ///     The working directory for the process. Defaults to the current directory if not
    ///     specified.
    /// </param>
    /// <returns>True if the process exited with code 0, otherwise false.</returns>
    public bool RunProcess(string file, string args, bool showOutput = true, string? workingDirectory = null)
    {
        try
        {
            // Try to get explicit path for common tools if direct call fails
            string executablePath = file;
            if (file.Equals("npm", StringComparison.OrdinalIgnoreCase) ||
                file.Equals("node", StringComparison.OrdinalIgnoreCase) ||
                file.Equals("ncu", StringComparison.OrdinalIgnoreCase) ||
                file.Equals("php", StringComparison.OrdinalIgnoreCase) ||
                file.Equals("composer", StringComparison.OrdinalIgnoreCase))
            {
                var explicitPath = FindExecutablePath(file);
                if (!string.IsNullOrEmpty(explicitPath))
                {
                    executablePath = explicitPath;
                    LoggerUtilities.Info($"Using explicit path for {file}: {explicitPath}");

                    // Special handling for composer.phar files - run them using PHP
                    if (file.Equals("composer", StringComparison.OrdinalIgnoreCase) &&
                        explicitPath.EndsWith(".phar", StringComparison.OrdinalIgnoreCase))
                    {
                        // Find PHP path
                        string phpPath = "php";
                        var phpExplicitPath = FindExecutablePath("php");
                        if (!string.IsNullOrEmpty(phpExplicitPath)) phpPath = phpExplicitPath;

                        return RunProcess(phpPath, $"{explicitPath} {args}", showOutput, workingDirectory);
                    }
                }
            }

            var process = new Process();
            process.StartInfo.FileName = executablePath;
            process.StartInfo.Arguments = args;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            // On Windows, load user profile to ensure PATH is available
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
#pragma warning disable CA1416 // Validate platform compatibility
                process.StartInfo.LoadUserProfile = true;
#pragma warning restore CA1416 // Validate platform compatibility
            }

            if (!string.IsNullOrEmpty(workingDirectory))
                process.StartInfo.WorkingDirectory = workingDirectory;
            else
                process.StartInfo.WorkingDirectory = Environment.CurrentDirectory;

            // Show diagnostic info if requested
            if (showOutput)
            {
                _console.WriteInfo($"Running: {executablePath} {args}");
                _console.WriteInfo($"Working directory: {process.StartInfo.WorkingDirectory}");
            }

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (showOutput)
            {
                if (!string.IsNullOrWhiteSpace(output)) _console.WriteInfo(output);
                if (!string.IsNullOrWhiteSpace(error)) _console.WriteError(error);
            }

            if (process.ExitCode == 0) return true;

            if (showOutput && (file.Equals("npm", StringComparison.OrdinalIgnoreCase) ||
                               file.Equals("node", StringComparison.OrdinalIgnoreCase) ||
                               file.Equals("php", StringComparison.OrdinalIgnoreCase) ||
                               file.Equals("composer", StringComparison.OrdinalIgnoreCase)))
            {
                _console.WriteError($"\nThe {file} command failed with exit code {process.ExitCode}");

                if (file.Equals("npm", StringComparison.OrdinalIgnoreCase) ||
                    file.Equals("node", StringComparison.OrdinalIgnoreCase))
                {
                    _console.WriteInfo("If you're experiencing npm-related issues, try the following:");
                    _console.WriteInfo("1. Run the 'npm diagnostics' option from the NPM Tools menu");
                    _console.WriteInfo("2. Make sure Node.js is properly installed and in your PATH");
                    _console.WriteInfo("3. Try using the 'Configure npm path' option to explicitly set the npm path");
                }
                else if (file.Equals("php", StringComparison.OrdinalIgnoreCase) ||
                         file.Equals("composer", StringComparison.OrdinalIgnoreCase))
                {
                    _console.WriteInfo("If you're experiencing PHP/Composer issues, try the following:");
                    _console.WriteInfo("1. Make sure PHP is properly installed and in your PATH");
                    _console.WriteInfo("2. Verify that Composer is installed correctly");
                }
            }

            return false;
        }
        catch (Win32Exception ex) when (ex.NativeErrorCode == 2) // File not found
        {
            LoggerUtilities.Error($"Command not found: {file}. {ex.Message}");
            if (showOutput)
            {
                _console.WriteError("[ERROR] Unable to execute the selected option:");
                _console.WriteError(
                    $"`{file}` is not found in the current environment. Ensure {file} is properly installed and added to your system's PATH.");

                // Special handling for npm on Windows
                if (file.Equals("npm", StringComparison.OrdinalIgnoreCase) &&
                    Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    // Try to find npm.cmd in common locations
                    string[] npmLocations =
                    {
                        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "nodejs",
                            "npm.cmd"),
                        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "nodejs",
                            "npm.cmd"),
                        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "npm",
                            "npm.cmd"),
                        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Roaming",
                            "npm", "npm.cmd")
                    };

                    foreach (string npmPath in npmLocations)
                        if (File.Exists(npmPath))
                        {
                            _console.WriteInfo($"\nFound npm.cmd at: {npmPath}");
                            _console.WriteInfo(
                                "You can use the 'Configure npm path' option to set this path explicitly.");
                            break;
                        }
                }

                // Display diagnostic information to help troubleshoot
                DisplayPathDiagnostics(file);
            }

            return false;
        }
        catch (Exception ex)
        {
            LoggerUtilities.Error($"Error running process: {ex.Message}");
            if (showOutput) _console.WriteError($"Error: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    ///     Attempts to find the explicit path for a given executable name.
    /// </summary>
    /// <param name="executableName">The name of the executable to find.</param>
    /// <returns>The explicit path if found, otherwise null.</returns>
    public string? FindExecutablePath(string executableName)
    {
        // If no config is available, return null
        if (_config == null) return null;

        // Check for explicitly configured paths in settings
        var configPath = _config.Get($"{executableName.ToLower()}_path", "");
        if (configPath != null && !string.IsNullOrWhiteSpace(configPath.ToString()) &&
            File.Exists(configPath.ToString())) return configPath.ToString();

        // For npm and node, try to find them directly in PATH first
        if (executableName.Equals("npm", StringComparison.OrdinalIgnoreCase) ||
            executableName.Equals("node", StringComparison.OrdinalIgnoreCase))
        {
            var pathExecutable = FindExecutableInPath(executableName);
            if (!string.IsNullOrEmpty(pathExecutable)) return pathExecutable;
        }

        // Common paths for executables depending on OS
        string[] searchPaths = GetSearchPathsForExecutable(executableName);

        foreach (var path in searchPaths)
            if (File.Exists(path))
                return path;

        return null;
    }

    /// <summary>
    ///     Gets an array of potential paths where an executable might be found based on OS.
    /// </summary>
    /// <param name="executableName">The name of the executable to find.</param>
    /// <returns>An array of potential file paths.</returns>
    private string[] GetSearchPathsForExecutable(string executableName)
    {
        string extension = Environment.OSVersion.Platform == PlatformID.Win32NT ? ".exe" : "";
        var paths = new List<string>();

        if (Environment.OSVersion.Platform == PlatformID.Win32NT) // Windows
        {
            // Add Windows-specific paths
            string programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            string programFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            switch (executableName.ToLower())
            {
                case "npm":
                case "node":
                    paths.Add(Path.Combine(programFiles, "nodejs", $"{executableName}{extension}"));
                    paths.Add(Path.Combine(programFilesX86, "nodejs", $"{executableName}{extension}"));
                    paths.Add(Path.Combine(appData, "npm", $"{executableName}{extension}"));
                    paths.Add(Path.Combine(appData, "nvm", "current", $"{executableName}{extension}"));

                    // Add .cmd versions for npm on Windows
                    if (executableName.ToLower() == "npm")
                    {
                        paths.Add(Path.Combine(programFiles, "nodejs", "npm.cmd"));
                        paths.Add(Path.Combine(programFilesX86, "nodejs", "npm.cmd"));
                        paths.Add(Path.Combine(appData, "npm", "npm.cmd"));
                        paths.Add(Path.Combine(appData, "nvm", "current", "npm.cmd"));
                        paths.Add(Path.Combine(appData, "Roaming", "npm", "npm.cmd"));
                    }

                    break;
                case "ncu":
                    paths.Add(Path.Combine(appData, "npm", "ncu.cmd"));
                    paths.Add(Path.Combine(appData, "npm", "ncu"));
                    break;
                case "php":
                    paths.Add(Path.Combine(programFiles, "PHP", $"{executableName}{extension}"));
                    paths.Add(Path.Combine(programFilesX86, "PHP", $"{executableName}{extension}"));
                    paths.Add(Path.Combine("C:", "PHP", $"{executableName}{extension}"));
                    paths.Add(Path.Combine("C:", "xampp", "php", $"{executableName}{extension}"));
                    paths.Add(Path.Combine("C:", "laragon", "bin", "php", "php-8.2.0", $"{executableName}{extension}"));
                    paths.Add(Path.Combine("C:", "laragon", "bin", "php", "php-8.1.0", $"{executableName}{extension}"));
                    paths.Add(Path.Combine("C:", "laragon", "bin", "php", "php-8.0.0", $"{executableName}{extension}"));
                    paths.Add(Path.Combine("C:", "laragon", "bin", "php", "php-7.4.0", $"{executableName}{extension}"));
                    paths.Add(Path.Combine("C:", "wamp64", "bin", "php", "php8.2.0", $"{executableName}{extension}"));
                    paths.Add(Path.Combine("C:", "wamp64", "bin", "php", "php8.1.0", $"{executableName}{extension}"));
                    paths.Add(Path.Combine("C:", "wamp64", "bin", "php", "php8.0.0", $"{executableName}{extension}"));
                    paths.Add(Path.Combine("C:", "wamp64", "bin", "php", "php7.4.0", $"{executableName}{extension}"));
                    break;
                case "composer":
                    paths.Add(Path.Combine(appData, "Composer", "composer.phar"));
                    paths.Add(Path.Combine("C:", "xampp", "php", "composer.phar"));
                    paths.Add(Path.Combine("C:", "ProgramData", "ComposerSetup", "bin", "composer.phar"));
                    paths.Add(Path.Combine("C:", "ProgramData", "ComposerSetup", "bin", "composer.bat"));
                    paths.Add(Path.Combine("C:", "ProgramData", "ComposerSetup", "bin", "composer"));
                    paths.Add(Path.Combine("C:", "composer", "composer.phar"));
                    paths.Add(Path.Combine("C:", "composer", "composer.bat"));
                    paths.Add(Path.Combine("C:", "laragon", "bin", "composer", "composer.phar"));
                    paths.Add(Path.Combine("C:", "laragon", "bin", "composer", "composer.bat"));
                    paths.Add(Path.Combine("C:", "wamp64", "bin", "composer", "composer.phar"));
                    paths.Add(Path.Combine("C:", "wamp64", "bin", "composer", "composer.bat"));
                    break;
            }
        }
        else // Unix-like systems
        {
            string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            switch (executableName.ToLower())
            {
                case "npm":
                case "node":
                    paths.Add(Path.Combine("/usr", "local", "bin", executableName));
                    paths.Add(Path.Combine("/usr", "bin", executableName));
                    paths.Add(Path.Combine(home, ".nvm", "current", "bin", executableName));
                    break;
                case "ncu":
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
                    paths.Add(Path.Combine(home, ".composer", "vendor", "bin", executableName));
                    break;
            }
        }

        return paths.ToArray();
    }

    /// <summary>
    ///     Runs a process and captures its output, error, and exit code.
    /// </summary>
    /// <param name="file">The executable file to run.</param>
    /// <param name="args">The arguments to pass to the executable.</param>
    /// <param name="workingDirectory">
    ///     The working directory for the process. Defaults to the current directory if not
    ///     specified.
    /// </param>
    /// <returns>A tuple containing the process output, error, and exit code.</returns>
    public (string output, string error, int exitCode) RunProcessWithOutput(string file, string args,
        string? workingDirectory = null)
    {
        try
        {
            // Try to get explicit path for common tools if direct call fails
            string executablePath = file;
            if (file.Equals("npm", StringComparison.OrdinalIgnoreCase) ||
                file.Equals("node", StringComparison.OrdinalIgnoreCase) ||
                file.Equals("ncu", StringComparison.OrdinalIgnoreCase) ||
                file.Equals("php", StringComparison.OrdinalIgnoreCase) ||
                file.Equals("composer", StringComparison.OrdinalIgnoreCase) ||
                file.Equals("git", StringComparison.OrdinalIgnoreCase))
            {
                var explicitPath = FindExecutablePath(file);
                if (!string.IsNullOrEmpty(explicitPath))
                {
                    executablePath = explicitPath;
                    LoggerUtilities.Info($"Using explicit path for {file}: {explicitPath}");
                }

                // Special handling for npm which might need .cmd extension on Windows
                if (file.Equals("npm", StringComparison.OrdinalIgnoreCase) &&
                    Environment.OSVersion.Platform == PlatformID.Win32NT &&
                    !executablePath.EndsWith(".cmd", StringComparison.OrdinalIgnoreCase))
                {
                    string npmCmd = executablePath + ".cmd";
                    if (File.Exists(npmCmd))
                    {
                        executablePath = npmCmd;
                        LoggerUtilities.Info($"Using npm.cmd instead of npm: {npmCmd}");
                    }
                }

                // Special handling for composer.phar files
                if (file.Equals("composer", StringComparison.OrdinalIgnoreCase) &&
                    executablePath.EndsWith(".phar", StringComparison.OrdinalIgnoreCase))
                {
                    LoggerUtilities.Info($"Found Composer phar file at {executablePath}, using PHP to execute it");
                    return RunProcessWithOutput("php", executablePath + " " + args, workingDirectory);
                }
            }

            var process = new Process();
            process.StartInfo.FileName = executablePath;
            process.StartInfo.Arguments = args;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            // On Windows, load user profile to ensure PATH is available
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
#pragma warning disable CA1416 // Validate platform compatibility
                process.StartInfo.LoadUserProfile = true;
#pragma warning restore CA1416 // Validate platform compatibility
            }

            if (!string.IsNullOrEmpty(workingDirectory))
                process.StartInfo.WorkingDirectory = workingDirectory;
            else
                process.StartInfo.WorkingDirectory = Environment.CurrentDirectory;

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            return (output, error, process.ExitCode);
        }
        catch (Win32Exception ex) when (ex.NativeErrorCode == 2) // File not found
        {
            LoggerUtilities.Error($"Command not found: {file}. {ex.Message}");
            return ("", $"Command not found: {file}. Ensure it is properly installed and in your PATH.", 1);
        }
        catch (Exception ex)
        {
            LoggerUtilities.Error($"Error running process: {ex.Message}");
            return ("", ex.Message, 1);
        }
    }

    /// <summary>
    ///     Displays diagnostic information to help troubleshoot command not found errors.
    /// </summary>
    /// <param name="command">The command that was not found.</param>
    private void DisplayPathDiagnostics(string command)
    {
        try
        {
            _console.WriteInfo("\nDiagnostic Information:");

            // Show current working directory
            _console.WriteInfo($"Current directory: {Environment.CurrentDirectory}");

            // Show PATH environment variable
            string path = Environment.GetEnvironmentVariable("PATH") ?? "";
            _console.WriteInfo("PATH contains the following directories:");
            foreach (var dir in path.Split(Path.PathSeparator))
                if (!string.IsNullOrWhiteSpace(dir))
                {
                    _console.WriteInfo($"  - {dir}");

                    // Check if the command exists in this PATH directory
                    string cmdPath = Path.Combine(dir, command);
                    string cmdPathWithExt = Path.Combine(dir, command + ".exe");
                    if (File.Exists(cmdPath))
                        _console.WriteInfo($"    ✓ {command} found in this location!");
                    else if (File.Exists(cmdPathWithExt))
                        _console.WriteInfo($"    ✓ {command}.exe found in this location!");
                }

            // Provide installation guidance based on the command
            if (command.Equals("npm", StringComparison.OrdinalIgnoreCase) ||
                command.Equals("node", StringComparison.OrdinalIgnoreCase))
            {
                _console.WriteInfo("\nTo install Node.js/npm:");
                _console.WriteInfo("- Windows: Download installer from https://nodejs.org/");
                _console.WriteInfo("- macOS: Use Homebrew `brew install node` or download from https://nodejs.org/");
                _console.WriteInfo("- Linux: Use your package manager or NVM (https://github.com/nvm-sh/nvm)");

                // On Windows, npm is usually a .cmd file, so try to detect that
                if (Environment.OSVersion.Platform == PlatformID.Win32NT &&
                    command.Equals("npm", StringComparison.OrdinalIgnoreCase))
                {
                    string pathEnv = Environment.GetEnvironmentVariable("PATH") ?? "";
                    bool npmCmdFound = false;
                    bool npmFound = false;

                    foreach (var dir in pathEnv.Split(Path.PathSeparator))
                    {
                        if (string.IsNullOrWhiteSpace(dir) || !Directory.Exists(dir))
                            continue;

                        if (File.Exists(Path.Combine(dir, "npm.cmd")))
                        {
                            npmCmdFound = true;
                            _console.WriteInfo($"\nFound npm.cmd in: {dir}");
                        }

                        if (File.Exists(Path.Combine(dir, "npm")))
                        {
                            npmFound = true;
                            _console.WriteInfo($"\nFound npm in: {dir}");
                        }
                    }

                    if (npmCmdFound || npmFound)
                    {
                        _console.WriteInfo("\nIt appears npm is installed but not accessible. Try these steps:");
                        _console.WriteInfo("1. Close and reopen your command prompt to refresh environment variables");
                        _console.WriteInfo(
                            "2. Use the 'Configure npm path' option in this tool to explicitly set the path to npm.cmd");
                        _console.WriteInfo(
                            "3. Check that the Node.js installation directory is in your PATH environment variable");
                    }
                }
            }
            else if (command.Equals("php", StringComparison.OrdinalIgnoreCase))
            {
                _console.WriteInfo("\nTo install PHP:");
                _console.WriteInfo("- Windows: Download from https://windows.php.net/download/ or use XAMPP");
                _console.WriteInfo("- macOS: Use Homebrew `brew install php`");
                _console.WriteInfo("- Linux: Use your package manager (e.g., `apt install php`)");
            }
            else if (command.Equals("composer", StringComparison.OrdinalIgnoreCase))
            {
                _console.WriteInfo("\nTo install Composer:");
                _console.WriteInfo("- All platforms: See instructions at https://getcomposer.org/download/");
            }
        }
        catch (Exception ex)
        {
            LoggerUtilities.Error($"Error displaying PATH diagnostics: {ex.Message}");
        }
    }

    /// <summary>
    ///     Searches the PATH environment variable for a specific executable.
    /// </summary>
    /// <param name="executableName">The name of the executable to find.</param>
    /// <returns>The path to the executable if found, otherwise null.</returns>
    private string? FindExecutableInPath(string executableName)
    {
        try
        {
            string extensions = Environment.OSVersion.Platform == PlatformID.Win32NT
                ? ".exe;.cmd;.bat;"
                : string.Empty;

            string path = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;

            foreach (string directory in path.Split(Path.PathSeparator))
            {
                if (string.IsNullOrWhiteSpace(directory))
                    continue;

                if (!Directory.Exists(directory))
                    continue;

                // Check for the executable with various extensions
                if (string.IsNullOrEmpty(extensions))
                {
                    string filePath = Path.Combine(directory, executableName);
                    if (File.Exists(filePath))
                        return filePath;
                }
                else
                {
                    foreach (string ext in extensions.Split(';'))
                    {
                        if (string.IsNullOrEmpty(ext))
                            continue;

                        // Check with the extension
                        string filePathWithExt = Path.Combine(directory, executableName + ext);
                        if (File.Exists(filePathWithExt))
                            return filePathWithExt;

                        // Also check without the extension for npm/node
                        if (executableName.Equals("npm", StringComparison.OrdinalIgnoreCase) ||
                            executableName.Equals("node", StringComparison.OrdinalIgnoreCase))
                        {
                            string filePathNoExt = Path.Combine(directory, executableName);
                            if (File.Exists(filePathNoExt))
                                return filePathNoExt;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            LoggerUtilities.Error($"Error searching PATH for {executableName}: {ex.Message}");
        }

        return null;
    }

    /// <summary>
    ///     Gets information about the installed PHP version
    /// </summary>
    /// <returns>A tuple containing (phpVersion, phpPath, isCompatible)</returns>
    public (string version, string path, bool isCompatible) GetPhpVersionInfo()
    {
        string phpPath = "php";
        string version = "Unknown";
        bool isCompatible = false;

        // Try to find the PHP executable path
        var explicitPath = FindExecutablePath("php");
        if (!string.IsNullOrEmpty(explicitPath))
        {
            phpPath = explicitPath;
            LoggerUtilities.Info($"Using explicit path for PHP: {explicitPath}");
        }

        // Run the PHP --version command
        var (output, error, exitCode) = RunProcessWithOutput(phpPath, "--version");

        if (exitCode == 0 && !string.IsNullOrWhiteSpace(output))
        {
            // Parse the PHP version from the output
            // Example output: "PHP 8.1.0 (cli) (built: Nov 23 2021 10:40:40) (NTS)"
            var versionMatch = Regex.Match(output, @"PHP\s+(\d+\.\d+\.\d+)");
            if (versionMatch.Success)
            {
                version = versionMatch.Groups[1].Value;

                // Check if the version is compatible with Laravel/Composer
                var versionParts = version.Split('.');
                if (versionParts.Length >= 2 &&
                    int.TryParse(versionParts[0], out int major) &&
                    int.TryParse(versionParts[1], out int minor))
                    // Laravel 10+ requires PHP 8.1+
                    // Laravel 9 requires PHP 8.0+
                    // Laravel 8 requires PHP 7.3+
                    isCompatible = major > 7 || (major == 7 && minor >= 3);
            }
        }

        return (version, phpPath, isCompatible);
    }

    /// <summary>
    ///     Checks if required PHP extensions for Laravel/Composer are installed
    /// </summary>
    /// <param name="phpPath">Path to the PHP executable</param>
    /// <returns>A tuple containing (missingExtensions, isCompatible)</returns>
    public (List<string> missingExtensions, bool isCompatible) CheckPhpExtensions(string phpPath = "php")
    {
        var requiredExtensions = new List<string>
        {
            "BCMath",
            "Ctype",
            "Fileinfo",
            "JSON",
            "Mbstring",
            "OpenSSL",
            "PDO",
            "Tokenizer",
            "XML",
            "cURL"
        };

        var missingExtensions = new List<string>();
        bool isCompatible = true;

        // Run PHP -m to get loaded extensions
        var (output, _, exitCode) = RunProcessWithOutput(phpPath, "-m");

        if (exitCode == 0 && !string.IsNullOrWhiteSpace(output))
        {
            var loadedExtensions = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(ext => ext.Trim().ToLower())
                .ToList();

            foreach (var ext in requiredExtensions)
                if (!loadedExtensions.Contains(ext.ToLower()))
                    missingExtensions.Add(ext);

            // Check if any critical extensions are missing
            isCompatible = !missingExtensions.Any(ext =>
                ext == "JSON" || ext == "PDO" || ext == "OpenSSL" || ext == "Mbstring");
        }
        else
        {
            // If we can't check extensions, assume not compatible
            isCompatible = false;
            missingExtensions = requiredExtensions;
        }

        return (missingExtensions, isCompatible);
    }

    /// <summary>
    ///     Checks if Composer is installed and returns version information
    /// </summary>
    /// <returns>A tuple containing (composerVersion, composerPath, isGlobal)</returns>
    public (string version, string path, bool isGlobal) GetComposerInfo()
    {
        string version = "Unknown";
        string composerPath = "composer";
        bool isGlobal = false; // Try to find the Composer executable path
        var explicitPath = FindExecutablePath("composer");
        if (!string.IsNullOrEmpty(explicitPath))
        {
            // If it's a .phar file, we need to run it with PHP
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

            LoggerUtilities.Info($"Using explicit path for Composer: {composerPath}");
        }

        // Check if composer.phar exists in the current directory
        if (File.Exists("composer.phar"))
        {
            composerPath = "php composer.phar";
            isGlobal = false;
        }

        // Run the Composer --version command
        var (output, _, exitCode) = composerPath.StartsWith("php ")
            ? RunProcessWithOutput("php", composerPath.Substring(4) + " --version")
            : RunProcessWithOutput(composerPath, "--version");

        if (exitCode == 0 && !string.IsNullOrWhiteSpace(output))
        {
            // Parse the Composer version from the output
            // Example output: "Composer version 2.3.5 2022-04-13 16:43:00"
            var versionMatch = Regex.Match(output, @"Composer version (\d+\.\d+\.\d+)");
            if (versionMatch.Success) version = versionMatch.Groups[1].Value;
        }

        return (version, composerPath, isGlobal);
    }

    /// <summary>
    ///     Gets information about the installed Laravel version
    /// </summary>
    /// <param name="workingDirectory">The Laravel project directory</param>
    /// <returns>A tuple containing (laravelVersion, isCompatible)</returns>
    public (string version, bool isCompatible) GetLaravelVersionInfo(string? workingDirectory = null)
    {
        string version = "Unknown";
        bool isCompatible = false;

        string directory = workingDirectory ?? Environment.CurrentDirectory;

        // Check if artisan file exists
        if (!File.Exists(Path.Combine(directory, "artisan"))) return (version, isCompatible);

        // Run Laravel version command
        var (output, _, exitCode) = RunProcessWithOutput("php", "artisan --version", directory);

        if (exitCode == 0 && !string.IsNullOrWhiteSpace(output))
        {
            // Parse the Laravel version (e.g., "Laravel Framework 8.83.27")
            var versionMatch = Regex.Match(output, @"Laravel Framework\s+(\d+\.\d+\.?\d*)");
            if (versionMatch.Success)
            {
                version = versionMatch.Groups[1].Value;

                // Check major version for compatibility
                if (version.Contains('.'))
                {
                    string majorVersionStr = version.Split('.')[0];
                    if (int.TryParse(majorVersionStr, out int majorVersion))
                        // Laravel versions 6+ are supported in this context
                        isCompatible = majorVersion >= 6;
                }
            }
        }

        return (version, isCompatible);
    }

    /// <summary>
    ///     Sets environment-specific PHP configuration options for better performance
    /// </summary>
    /// <param name="options">Dictionary of PHP options to set</param>
    /// <returns>A string with PHP -d options that can be prepended to commands</returns>
    public string GetPhpConfigOptions(Dictionary<string, string>? options = null)
    {
        var configOptions = options ?? new Dictionary<string, string>
        {
            ["memory_limit"] = "-1",
            ["max_execution_time"] = "0"
        };

        // Build the string of -d options
        var optionsStr = string.Join(" ", configOptions.Select(o => $"-d {o.Key}={o.Value}"));

        return optionsStr;
    }

    /// <summary>
    ///     Checks if PHP has the required memory settings for Composer operations
    /// </summary>
    /// <param name="phpPath">Path to PHP executable</param>
    /// <returns>A tuple with (hasEnoughMemory, currentLimit, recommendedLimit)</returns>
    public (bool hasEnoughMemory, string currentLimit, string recommendedLimit) CheckPhpMemoryLimit(
        string phpPath = "php")
    {
        const string recommendedLimit = "-1"; // No limit is best for Composer
        string currentLimit = "Unknown";
        bool hasEnoughMemory = false;

        // Run PHP to check memory_limit setting
        var (output, _, exitCode) = RunProcessWithOutput(phpPath, "-r \"echo ini_get('memory_limit');\"");

        if (exitCode == 0 && !string.IsNullOrWhiteSpace(output))
        {
            currentLimit = output.Trim();

            // Parse the memory limit
            if (currentLimit == "-1")
                // No limit, which is ideal
                hasEnoughMemory = true;
            else if (currentLimit.EndsWith("M", StringComparison.OrdinalIgnoreCase) &&
                     int.TryParse(currentLimit.TrimEnd('M', 'm'), out int mbLimit))
                // Check if at least 1.5GB is available (Composer recommendation)
                hasEnoughMemory = mbLimit >= 1536;
            else if (currentLimit.EndsWith("G", StringComparison.OrdinalIgnoreCase) &&
                     int.TryParse(currentLimit.TrimEnd('G', 'g'), out int gbLimit))
                // Convert GB to MB for comparison
                hasEnoughMemory = gbLimit >= 1.5;
        }

        return (hasEnoughMemory, currentLimit, recommendedLimit);
    }

    /// <summary>
    ///     Checks if a specific Composer package is installed
    /// </summary>
    /// <param name="packageName">Name of the package to check</param>
    /// <param name="workingDirectory">Directory containing composer.json</param>
    /// <returns>True if the package is installed</returns>
    public bool IsComposerPackageInstalled(string packageName, string? workingDirectory = null)
    {
        string directory = workingDirectory ?? Environment.CurrentDirectory;

        // Check if composer.json exists
        string composerJsonPath = Path.Combine(directory, "composer.json");
        if (!File.Exists(composerJsonPath)) return false;

        try
        {
            // Read composer.json
            string json = File.ReadAllText(composerJsonPath);

            // Parse the JSON
            using var jsonDoc = JsonDocument.Parse(json);
            var root = jsonDoc.RootElement;

            // Check require and require-dev sections
            bool CheckSection(string sectionName)
            {
                if (root.TryGetProperty(sectionName, out var section) && section.ValueKind == JsonValueKind.Object)
                    foreach (var property in section.EnumerateObject())
                        if (property.Name.Equals(packageName, StringComparison.OrdinalIgnoreCase))
                            return true;

                return false;
            }

            return CheckSection("require") || CheckSection("require-dev");
        }
        catch (Exception ex)
        {
            LoggerUtilities.Error($"Error checking for Composer package {packageName}: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    ///     Detects which PHP environment variables are set and provides recommended values
    /// </summary>
    /// <returns>Dictionary of environment variables with recommendations</returns>
    public Dictionary<string, (string? currentValue, string recommendedValue, bool needsUpdate)>
        GetPhpEnvironmentRecommendations()
    {
        var recommendations = new Dictionary<string, (string? currentValue, string recommendedValue, bool needsUpdate)>
        {
            // Common PHP environment variables and their recommended values for development
            ["PHP_MEMORY_LIMIT"] = (null, "-1", false),
            ["PHP_MAX_EXECUTION_TIME"] = (null, "0", false),
            ["PHP_UPLOAD_MAX_FILESIZE"] = (null, "128M", false),
            ["PHP_POST_MAX_SIZE"] = (null, "128M", false),
            ["PHP_DISPLAY_ERRORS"] = (null, "On", false),
            ["COMPOSER_MEMORY_LIMIT"] = (null, "-1", false),
            ["COMPOSER_PROCESS_TIMEOUT"] = (null, "0", false),
            ["COMPOSER_NO_INTERACTION"] = (null, "1", false)
        };

        // Get current values from environment
        foreach (var key in recommendations.Keys.ToList())
        {
            string? currentValue = Environment.GetEnvironmentVariable(key);
            string recommendedValue = recommendations[key].recommendedValue;

            // Check if update is needed
            bool needsUpdate = string.IsNullOrEmpty(currentValue) || currentValue != recommendedValue;

            recommendations[key] = (currentValue, recommendedValue, needsUpdate);
        }

        return recommendations;
    }

    /// <summary>
    ///     Sets recommended PHP environment variables for the current process
    /// </summary>
    /// <returns>Number of variables that were updated</returns>
    public int SetRecommendedPhpEnvironmentVariables()
    {
        var recommendations = GetPhpEnvironmentRecommendations();
        int updatedCount = 0;

        foreach (var (key, (current, recommended, needsUpdate)) in recommendations)
            if (needsUpdate)
            {
                Environment.SetEnvironmentVariable(key, recommended);
                LoggerUtilities.Info($"Set environment variable {key}={recommended}");
                updatedCount++;
            }

        return updatedCount;
    }
}