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

namespace EasyKit.Services;

/// <summary>
///     Minimal ProcessService for running external commands using system PATH.
/// </summary>
public class ProcessService
{
    /// <summary>
    ///     Runs a process and captures its output, error, and exit code.
    ///     [MVP] Now always opens a new cmd.exe window for execution (no output capture).
    /// </summary>
    /// <param name="command">The command to run (e.g., "npm", "php").</param>
    /// <param name="args">Arguments to pass to the command.</param>
    /// <param name="workingDirectory">Optional working directory.</param>
    /// <returns>Tuple of (output, error, exitCode) - always empty for MVP.</returns>
    public (string output, string error, int exitCode) RunProcess(string command, string args,
        string? workingDirectory = null)
    {
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c \"{command} {args}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = workingDirectory ?? Environment.CurrentDirectory,
                CreateNoWindow = true
            };
            using var process = new Process { StartInfo = psi };
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();
            int exitCode = process.ExitCode;
            // Show what it does in main window
            Console.WriteLine($"[ProcessService] Ran: {command} {args}");
            if (!string.IsNullOrWhiteSpace(output))
                Console.WriteLine(output);
            if (!string.IsNullOrWhiteSpace(error))
                Console.Error.WriteLine(error);
            return (output, error, exitCode);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[ProcessService] Error: {ex.Message}");
            return ("", ex.Message, -1);
        }
    }

    /// <summary>
    ///     Runs a process and streams its output and error in real time.
    ///     [MVP] Now always opens a new cmd.exe window for execution (no streaming).
    /// </summary>
    public int RunProcessWithStreaming(string command, string args, string? workingDirectory = null,
        Action<string>? onOutput = null, Action<string>? onError = null)
    {
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c \"{command} {args}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = workingDirectory ?? Environment.CurrentDirectory,
                CreateNoWindow = true
            };
            using var process = new Process { StartInfo = psi };
            process.OutputDataReceived += (s, e) =>
            {
                if (e.Data != null)
                {
                    onOutput?.Invoke(e.Data);
                    Console.WriteLine(e.Data);
                }
            };
            process.ErrorDataReceived += (s, e) =>
            {
                if (e.Data != null)
                {
                    onError?.Invoke(e.Data);
                    Console.Error.WriteLine(e.Data);
                }
            };
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            Console.WriteLine($"[ProcessService] Streaming: {command} {args}");
            process.WaitForExit();
            return process.ExitCode;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[ProcessService] Streaming Error: {ex.Message}");
            return -1;
        }
    }

    /// <summary>
    ///     Runs a process in a new cmd.exe window (best for interactive or environment-sensitive commands on Windows).
    /// </summary>
    [Obsolete("This method is deprecated. Use RunProcess instead.")]
    public void RunProcessInNewCmdWindow(string command, string args, string? workingDirectory = null)
    {
        // Deprecated: No longer opens a new window
        RunProcess(command, args, workingDirectory);
    }
}