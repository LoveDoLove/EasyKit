using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security.Principal;

namespace EasyKit.Services;

public static class AdminService
{
    /// <summary>
    ///     Checks if the application is running with administrator/root privileges based on the platform
    /// </summary>
    /// <returns>True if running as administrator/root, false otherwise</returns>
    public static bool IsRunningAsAdmin()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return IsRunningAsWindowsAdmin();

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return IsRunningAsLinuxRoot();
        // Default to false for unsupported platforms
        return false;
    }

    /// <summary>
    ///     Checks if the application is running with Windows administrator privileges
    /// </summary>
    [SupportedOSPlatform("windows")]
    private static bool IsRunningAsWindowsAdmin()
    {
        using WindowsIdentity identity = WindowsIdentity.GetCurrent();
        WindowsPrincipal principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }

    /// <summary>
    ///     Checks if the application is running with Linux root privileges
    /// </summary>
    [SupportedOSPlatform("linux")]
    private static bool IsRunningAsLinuxRoot()
    {
        // On Linux, the root user has UID 0
        return Environment.UserName == "root" || GetEffectiveUserId() == 0;
    }

    /// <summary>
    ///     Gets the effective user ID on Linux systems
    /// </summary>
    [DllImport("libc", SetLastError = true)]
    [SupportedOSPlatform("linux")]
    private static extern uint geteuid();

    [SupportedOSPlatform("linux")]
    private static uint GetEffectiveUserId()
    {
        try
        {
            return geteuid();
        }
        catch
        {
            // If the call fails, assume not root
            return uint.MaxValue;
        }
    }

    /// <summary>
    ///     Restarts the application with administrator/root privileges based on the platform
    /// </summary>
    /// <returns>True if the restart was requested, false if there was an error</returns>
    public static bool RestartAsAdmin()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return RestartAsWindowsAdmin();

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return RestartAsLinuxRoot();
        // Default to false for unsupported platforms
        return false;
    }

    /// <summary>
    ///     Restarts the application with Windows administrator privileges
    /// </summary>
    [SupportedOSPlatform("windows")]
    private static bool RestartAsWindowsAdmin()
    {
        try
        {
            // Get the path to the current executable
            string exePath = Process.GetCurrentProcess().MainModule?.FileName ?? string.Empty;
            if (string.IsNullOrEmpty(exePath)) return false;

            // Create process start info
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                UseShellExecute = true,
                WorkingDirectory = Environment.CurrentDirectory,
                FileName = exePath,
                Verb = "runas" // This is what requests elevation
            };

            // Start the new process with admin rights
            Process.Start(startInfo);

            // Exit the current (non-elevated) process
            Environment.Exit(0);
            return true;
        }
        catch (Exception)
        {
            // User cancelled the UAC dialog or an error occurred
            return false;
        }
    }

    /// <summary>
    ///     Restarts the application with Linux root privileges using sudo
    /// </summary>
    [SupportedOSPlatform("linux")]
    private static bool RestartAsLinuxRoot()
    {
        try
        {
            // Get the path to the current executable
            string exePath = Process.GetCurrentProcess().MainModule?.FileName ?? string.Empty;
            if (string.IsNullOrEmpty(exePath)) return false;

            // Create process start info for using sudo
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                UseShellExecute = false,
                WorkingDirectory = Environment.CurrentDirectory,
                FileName = "sudo",
                Arguments = $"\"{exePath}\"",
                CreateNoWindow = false
            };

            // Start the new process with root privileges
            Process.Start(startInfo);

            // Exit the current (non-root) process
            Environment.Exit(0);
            return true;
        }
        catch (Exception)
        {
            // User cancelled the sudo prompt or an error occurred
            return false;
        }
    }
}