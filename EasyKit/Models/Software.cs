using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace EasyKit.Models;

public class Software
{
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

    public string OSPlatform { get; }

    public bool CheckSoftware(string softwareName)
    {
        var method = GetType().GetMethod($"Check_{softwareName}", BindingFlags.NonPublic | BindingFlags.Instance);
        if (method != null)
            return (bool)method.Invoke(this, null);
        Console.WriteLine($"No check method found for {softwareName}");
        return false;
    }

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
    // TODO: Add more software checks as needed
}