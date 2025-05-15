using System.Diagnostics;

namespace EasyKit.Services;

/// <summary>
/// Service for handling process execution operations across the application.
/// </summary>
public class ProcessService
{
    private readonly ConsoleService _console;
    private readonly LoggerService _logger;
    private readonly Config? _config;

    public ProcessService(LoggerService logger, ConsoleService console, Config? config = null)
    {
        _logger = logger;
        _console = console;
        _config = config;
    }

    /// <summary>
    /// Runs a process with the specified parameters.
    /// </summary>
    /// <param name="file">The executable file to run.</param>
    /// <param name="args">The arguments to pass to the executable.</param>
    /// <param name="showOutput">Whether to show the output in the console.</param>
    /// <param name="workingDirectory">The working directory for the process. Defaults to the current directory if not specified.</param>
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
                    _logger.Info($"Using explicit path for {file}: {explicitPath}");
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

            if (process.ExitCode == 0)
            {
                return true;
            }
            else
            {
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
        }
        catch (System.ComponentModel.Win32Exception ex) when (ex.NativeErrorCode == 2) // File not found
        {
            _logger.Error($"Command not found: {file}. {ex.Message}");
            if (showOutput)
            {
                _console.WriteError($"[ERROR] Unable to execute the selected option:");
                _console.WriteError($"`{file}` is not found in the current environment. Ensure {file} is properly installed and added to your system's PATH.");
                // Display diagnostic information to help troubleshoot
                DisplayPathDiagnostics(file);
            }
            return false;
        }
        catch (Exception ex)
        {
            _logger.Error($"Error running process: {ex.Message}");
            if (showOutput) _console.WriteError($"Error: {ex.Message}");
            return false;
        }
    }    /// <summary>
    /// Attempts to find the explicit path for a given executable name.
    /// </summary>
    /// <param name="executableName">The name of the executable to find.</param>
    /// <returns>The explicit path if found, otherwise null.</returns>
    public string? FindExecutablePath(string executableName)
    {
        // If no config is available, return null
        if (_config == null) return null;

        // Check for explicitly configured paths in settings
        var configPath = _config.Get($"{executableName.ToLower()}_path", "");
        if (configPath != null && !string.IsNullOrWhiteSpace(configPath.ToString()) && File.Exists(configPath.ToString()))
        {
            return configPath.ToString();
        }

        // Common paths for executables depending on OS
        string[] searchPaths = GetSearchPathsForExecutable(executableName);
        
        foreach (var path in searchPaths)
        {
            if (File.Exists(path))
            {
                return path;
            }
        }

        return null;
    }

    /// <summary>
    /// Gets an array of potential paths where an executable might be found based on OS.
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
                    break;
                case "composer":
                    paths.Add(Path.Combine(appData, "Composer", "composer.phar"));
                    paths.Add(Path.Combine("C:", "xampp", "php", "composer.phar"));
                    paths.Add(Path.Combine("C:", "ProgramData", "ComposerSetup", "bin", "composer.phar"));
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
    }    /// <summary>
    /// Runs a process and captures its output, error, and exit code.
    /// </summary>
    /// <param name="file">The executable file to run.</param>
    /// <param name="args">The arguments to pass to the executable.</param>
    /// <param name="workingDirectory">The working directory for the process. Defaults to the current directory if not specified.</param>
    /// <returns>A tuple containing the process output, error, and exit code.</returns>
    public (string output, string error, int exitCode) RunProcessWithOutput(string file, string args, string? workingDirectory = null)
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
                    _logger.Info($"Using explicit path for {file}: {explicitPath}");
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
        catch (System.ComponentModel.Win32Exception ex) when (ex.NativeErrorCode == 2) // File not found
        {
            _logger.Error($"Command not found: {file}. {ex.Message}");
            return ("", $"Command not found: {file}. Ensure it is properly installed and in your PATH.", 1);
        }
        catch (Exception ex)
        {
            _logger.Error($"Error running process: {ex.Message}");
            return ("", ex.Message, 1);
        }
    }

    /// <summary>
    /// Displays diagnostic information to help troubleshoot command not found errors.
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
            _console.WriteInfo($"PATH contains the following directories:");
            foreach (var dir in path.Split(Path.PathSeparator))
            {
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
            }
            
            // Provide installation guidance based on the command
            if (command.Equals("npm", StringComparison.OrdinalIgnoreCase) || 
                command.Equals("node", StringComparison.OrdinalIgnoreCase))
            {
                _console.WriteInfo("\nTo install Node.js/npm:");
                _console.WriteInfo("- Windows: Download installer from https://nodejs.org/");
                _console.WriteInfo("- macOS: Use Homebrew `brew install node` or download from https://nodejs.org/");
                _console.WriteInfo("- Linux: Use your package manager or NVM (https://github.com/nvm-sh/nvm)");
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
            _logger.Error($"Error displaying PATH diagnostics: {ex.Message}");
        }
    }
}
