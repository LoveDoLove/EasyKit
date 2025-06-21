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

using CommonUtilities.Helpers.Processes;

namespace EasyKit.Services;

/// <summary>
///     Minimal ProcessService for running external commands using system PATH.
/// </summary>
public class ProcessService
{
    /// <summary>
    ///     Runs a process and captures its output, error, and exit code.
    ///     Now delegates to ProcessExecutionHelper.RunProcessWithCmd.
    /// </summary>
    /// <param name="command">The command to run (e.g., "npm", "php").</param>
    /// <param name="args">Arguments to pass to the command.</param>
    /// <param name="workingDirectory">Optional working directory.</param>
    /// <returns>Tuple of (output, error, exitCode) - always empty for MVP.</returns>
    public (string output, string error, int exitCode) RunProcess(string command, string args,
        string? workingDirectory = null)
    {
        return ProcessExecutionHelper.RunProcessWithCmd(command, args, workingDirectory);
    }

    /// <summary>
    ///     Runs a process and streams its output and error in real time.
    ///     Now delegates to ProcessExecutionHelper.RunProcessWithCmdStreaming.
    /// </summary>
    public int RunProcessWithStreaming(string command, string args, string? workingDirectory = null,
        Action<string>? onOutput = null, Action<string>? onError = null)
    {
        return ProcessExecutionHelper.RunProcessWithCmdStreaming(command, args, workingDirectory, onOutput, onError);
    }

    /// <summary>
    ///     Runs a process in a new cmd.exe window (best for interactive or environment-sensitive commands on Windows).
    ///     Now delegates to ProcessExecutionHelper.RunProcessInNewCmdWindow.
    /// </summary>
    public void RunProcessInNewCmdWindow(string command, string args, string? workingDirectory = null)
    {
        ProcessExecutionHelper.RunProcessInNewCmdWindow(command, args, workingDirectory);
    }
}