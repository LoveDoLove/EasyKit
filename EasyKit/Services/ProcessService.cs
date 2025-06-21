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
    /// </summary>
    /// <param name="command">The command to run (e.g., "npm", "php").</param>
    /// <param name="args">Arguments to pass to the command.</param>
    /// <param name="workingDirectory">Optional working directory.</param>
    /// <returns>Tuple of (output, error, exitCode).</returns>
    public (string output, string error, int exitCode) RunProcess(string command, string args, string? workingDirectory = null)
    {
        try
        {
            using (var process = new Process())
            {
                process.StartInfo.FileName = command;
                process.StartInfo.Arguments = args;
                process.StartInfo.WorkingDirectory = workingDirectory ?? Environment.CurrentDirectory;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;

                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();
                return (output, error, process.ExitCode);
            }
        }
        catch (Exception ex)
        {
            return ("", ex.Message, -1);
        }
    }

    /// <summary>
    ///     Runs a process and streams its output and error in real time.
    /// </summary>
    /// <param name="command">The command to run.</param>
    /// <param name="args">Arguments to pass to the command.</param>
    /// <param name="workingDirectory">Optional working directory.</param>
    /// <param name="onOutput">Callback for each output line.</param>
    /// <param name="onError">Callback for each error line.</param>
    /// <returns>Exit code of the process.</returns>
    public int RunProcessWithStreaming(string command, string args, string? workingDirectory = null, Action<string>? onOutput = null, Action<string>? onError = null)
    {
        try
        {
            using (var process = new Process())
            {
                process.StartInfo.FileName = command;
                process.StartInfo.Arguments = args;
                process.StartInfo.WorkingDirectory = workingDirectory ?? Environment.CurrentDirectory;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;

                if (onOutput != null)
                    process.OutputDataReceived += (s, e) => { if (e.Data != null) onOutput(e.Data); };
                if (onError != null)
                    process.ErrorDataReceived += (s, e) => { if (e.Data != null) onError(e.Data); };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
                return process.ExitCode;
            }
        }
        catch (Exception ex)
        {
            onError?.Invoke(ex.Message);
            return -1;
        }
    }
}