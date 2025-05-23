using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace CommonUtilities.Models;

/// <summary>
/// Provides OS platform detection and software presence checks.
/// </summary>
public class Software
{
    /// <summary>
    /// The detected OS platform ("windows", "linux", "osx", or "unknown").
    /// </summary>
    public string OSPlatform { get; }

    public Software()
    {
        if (RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
            OSPlatform = "windows";
        else if (RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux))
            OSPlatform = "linux";
        else if (RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX))
            OSPlatform = "osx";
        else
            OSPlatform = "unknown";
    }

    /// <summary>
    /// Checks if a named software is available on the system.
    /// </summary>
    /// <param name="softwareName">The software to check (e.g., "git", "choco").</param>
    /// <returns>True if found, false otherwise.</returns>
    public bool CheckSoftware(string softwareName)
    {
        var method = GetType().GetMethod($"Check_{softwareName}", BindingFlags.NonPublic | BindingFlags.Instance);
        if (method != null)
            return (bool)method.Invoke(this, null);
        Console.WriteLine($"No check method found for {softwareName}");
        return false;
    }

    /// <summary>
    /// Checks for Chocolatey on Windows.
    /// </summary>
    private bool Check_choco()
    {
        if (OSPlatform != "windows")
        {
            Console.WriteLine("Chocolatey is only available on Windows");
            return false;
        }

        try
        {
            var process = Process.Start(new ProcessStartInfo
            {
                FileName = "where",
                Arguments = "choco.exe",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            });
            process.WaitForExit();
            return process.ExitCode == 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error checking Chocolatey: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Checks for Git on the current platform.
    /// </summary>
    private bool Check_git()
    {
        try
        {
            var process = Process.Start(new ProcessStartInfo
            {
                FileName = OSPlatform == "windows" ? "where" : "which",
                Arguments = "git",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            });
            process.WaitForExit();
            return process.ExitCode == 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error checking Git: {ex.Message}");
            return false;
        }
    }

    // Extend with more software checks as needed.
}