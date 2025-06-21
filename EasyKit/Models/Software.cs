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
using System.Reflection;
using System.Runtime.InteropServices;

namespace EasyKit.Models;

/// <summary>
///     Provides OS platform detection and software presence checks.
/// </summary>
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

    /// <summary>
    ///     The detected OS platform ("windows", "linux", "osx", or "unknown").
    /// </summary>
    public string OSPlatform { get; }

    /// <summary>
    ///     Checks if a named software is available on the system.
    /// </summary>
    /// <param name="softwareName">The software to check (e.g., "git", "choco").</param>
    /// <returns>True if found, false otherwise.</returns>
    public bool CheckSoftware(string softwareName)
    {
        // Dynamically find a private instance method named "Check_softwareName" (e.g., "Check_git")
        var method = GetType().GetMethod($"Check_{softwareName}", BindingFlags.NonPublic | BindingFlags.Instance);
        if (method != null)
            // If found, invoke it and cast the result to bool
            return (bool)method.Invoke(this, null)!; // Null-forgiving operator used as we expect bool return
        Console.WriteLine($"No check method found for {softwareName}. Please implement 'Check_{softwareName}()'.");
        return false;
    }

    /// <summary>
    ///     Checks for Chocolatey package manager on Windows systems.
    ///     Uses the 'where' command to locate 'choco.exe'.
    /// </summary>
    /// <returns>True if Chocolatey is found, false otherwise.</returns>
    private bool Check_choco()
    {
        if (OSPlatform != "windows")
        {
            Console.WriteLine("Chocolatey check is only applicable on Windows.");
            return false;
        }

        try
        {
            // Use 'where' command on Windows to find the executable
            var process = Process.Start(new ProcessStartInfo
            {
                FileName = "where", // Command to locate files in PATH
                Arguments = "choco.exe",
                RedirectStandardOutput = true, // Capture output to check ExitCode
                UseShellExecute = false,
                CreateNoWindow = true
            });
            if (process == null) return false; // Process failed to start
            process.WaitForExit();
            return process.ExitCode == 0; // ExitCode 0 usually means success (found)
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error checking for Chocolatey: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    ///     Checks for Git version control system on the current platform.
    ///     Uses 'where' on Windows and 'which' on Linux/macOS to locate the 'git' executable.
    /// </summary>
    /// <returns>True if Git is found, false otherwise.</returns>
    private bool Check_git()
    {
        try
        {
            // Determine the command based on the OS platform
            string command = OSPlatform == "windows" ? "where" : "which";
            var process = Process.Start(new ProcessStartInfo
            {
                FileName = command, // 'where' for Windows, 'which' for Linux/macOS
                Arguments = "git",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            });
            if (process == null) return false; // Process failed to start
            process.WaitForExit();
            return process.ExitCode == 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error checking for Git: {ex.Message}");
            return false;
        }
    }

    // To add checks for other software:
    // 1. Create a private method named `Check_softwarename()` (e.g., `Check_node()`).
    // 2. Implement the logic to detect the software, typically by running a command like 'softwarename --version'
    //    or using 'where'/'which' to find its executable.
    // 3. Return true if found, false otherwise.
    // 4. Call `softwareInstance.CheckSoftware("softwarename")` to use it.
}