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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;

namespace EasyKit.Services;

/// <summary>
///     Secure process execution service that prevents shell injection.
///     Uses structured arguments instead of string concatenation.
/// </summary>
public class SecureProcessRunner
{
    private readonly HashSet<string> _allowedCommands = new(StringComparer.OrdinalIgnoreCase)
    {
        "git", "npm", "pnpm", "corepack", "composer", "php", "laravel",
        "uv", "python", "pip", "dotnet", "docker", "node",
        "where", "which", "choco", "nuget", "echo", "type", "dir", "copy"
    };

    private readonly HashSet<char> _dangerousChars = new() { '`', '|', '&', '$', '(', ')', '{', '}', '[', ']', '!', '#', '%', '^', '<', '>', ';', '+', '=' };

    /// <summary>
    ///     Runs a process securely with structured arguments.
    /// </summary>
    public (string output, string error, int exitCode) RunProcess(
        string command,
        IEnumerable<string> arguments,
        string? workingDirectory = null,
        CancellationToken cancellationToken = default,
        TimeSpan? timeout = null)
    {
        ValidateCommand(command);
        ValidateArguments(arguments);

        var resolvedCommand = ResolveCommand(command);

        var psi = new ProcessStartInfo
        {
            FileName = resolvedCommand,
            Arguments = BuildSafeArguments(arguments),
            WorkingDirectory = workingDirectory ?? Environment.CurrentDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(psi);
        if (process == null)
            throw new InvalidOperationException($"Failed to start process: {command}");

        var outputTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
        var errorTask = process.StandardError.ReadToEndAsync(cancellationToken);
        var exitTask = process.WaitForExitAsync(cancellationToken);

        var timeoutTask = Task.Delay(timeout ?? TimeSpan.FromMinutes(5), cancellationToken);

        var completed = Task.WhenAny(exitTask, timeoutTask);

        if (completed.Result == timeoutTask)
        {
            try { process.Kill(true); } catch { /* Ignore kill errors */ }
            return (outputTask.Result ?? "", "[TIMEOUT] Process timed out", -1);
        }

        Task.WaitAll(outputTask, errorTask, exitTask);

        return (outputTask.Result ?? "", errorTask.Result ?? "", process.ExitCode);
    }

    /// <summary>
    ///     Runs a process and streams output in real-time.
    /// </summary>
    public int RunProcessStreaming(
        string command,
        IEnumerable<string> arguments,
        string? workingDirectory = null,
        Action<string>? onOutput = null,
        Action<string>? onError = null,
        CancellationToken cancellationToken = default,
        TimeSpan? timeout = null)
    {
        ValidateCommand(command);
        ValidateArguments(arguments);

        var resolvedCommand = ResolveCommand(command);

        var psi = new ProcessStartInfo
        {
            FileName = resolvedCommand,
            Arguments = BuildSafeArguments(arguments),
            WorkingDirectory = workingDirectory ?? Environment.CurrentDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(psi);
        if (process == null)
            throw new InvalidOperationException($"Failed to start process: {command}");

        var outputTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
        var errorTask = process.StandardError.ReadToEndAsync(cancellationToken);
        var exitTask = process.WaitForExitAsync(cancellationToken);

        var timeoutTask = Task.Delay(timeout ?? TimeSpan.FromMinutes(5), cancellationToken);

        var completed = Task.WhenAny(exitTask, timeoutTask);

        if (completed.Result == timeoutTask)
        {
            try { process.Kill(true); } catch { /* Ignore kill errors */ }
            onError?.Invoke("[TIMEOUT] Process timed out");
            return -1;
        }

        Task.WaitAll(outputTask, errorTask, exitTask);

        if (onOutput != null && !string.IsNullOrEmpty(outputTask.Result))
            foreach (var line in outputTask.Result!.Split('\n'))
                onOutput(line);

        if (onError != null && !string.IsNullOrEmpty(errorTask.Result))
            foreach (var line in errorTask.Result!.Split('\n'))
                onError(line);

        return process.ExitCode;
    }

    /// <summary>
    ///     Runs a process in a new cmd window (for interactive commands).
    /// </summary>
    public void RunProcessInNewWindow(
        string command,
        IEnumerable<string> arguments,
        string? workingDirectory = null)
    {
        ValidateCommand(command);
        ValidateArguments(arguments);

        var psi = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = $"/K \"{command} {BuildSafeArguments(arguments)}\"",
            WorkingDirectory = workingDirectory ?? Environment.CurrentDirectory,
            UseShellExecute = true
        };

        Process.Start(psi);
    }

    private void ValidateCommand(string command)
    {
        var fileName = Path.GetFileNameWithoutExtension(command);
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("Invalid command name", nameof(command));

        if (!_allowedCommands.Contains(fileName))
            throw new SecurityException($"Command '{command}' is not in the allowed commands list");

        if (command.Any(c => _dangerousChars.Contains(c)))
            throw new SecurityException($"Invalid characters in command: {command}");
    }

    private void ValidateArguments(IEnumerable<string> arguments)
    {
        foreach (var arg in arguments)
        {
            if (arg == null) continue;

            // Check for shell metacharacters
            if (arg.Any(c => _dangerousChars.Contains(c)))
                throw new SecurityException($"Dangerous character(s) found in argument: {arg}");

            // Check for path traversal
            if (arg.Contains("..") || arg.StartsWith("/") || arg.StartsWith("-"))
                // Allow negative numbers and flags starting with -
                if (!arg.StartsWith("-") || arg.Length == 1)
                    throw new SecurityException($"Suspicious argument: {arg}");
        }
    }

    private string BuildSafeArguments(IEnumerable<string> arguments)
    {
        var sb = new StringBuilder();
        bool first = true;
        foreach (var arg in arguments)
        {
            if (string.IsNullOrWhiteSpace(arg)) continue;

            if (!first) sb.Append(' ');
            first = false;

            // Escape quotes and special characters
            var escaped = arg.Replace("\"", "\\\"").Replace("`", "\\`");
            sb.Append(escaped);
        }
        return sb.ToString();
    }

    /// <summary>
    ///     Resolves a command name to its full path on Windows.
    ///     Handles .exe, .cmd, .bat, .ps1 extensions and searches PATH.
    /// </summary>
    private static string ResolveCommand(string command)
    {
        // If it's already a full path or contains a path separator, use as-is
        if (command.Contains(Path.DirectorySeparatorChar) || command.Contains(Path.AltDirectorySeparatorChar))
            return command;

        // Try exact match first (for cases where the command is already a full path)
        if (File.Exists(command))
            return command;

        // Get PATH directories
        var pathEnv = Environment.GetEnvironmentVariable("PATH") ?? "";
        var pathDirs = pathEnv.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries);

        // Common Windows executable extensions
        var extensions = new[] { ".exe", ".cmd", ".bat", ".ps1" };

        foreach (var dir in pathDirs)
        {
            if (string.IsNullOrWhiteSpace(dir)) continue;

            // Check if directory exists
            if (!Directory.Exists(dir)) continue;

            // Try each extension
            foreach (var ext in extensions)
            {
                var candidate = Path.Combine(dir, command + ext);
                if (File.Exists(candidate))
                    return candidate;
            }
        }

        // Fall back to the original command name
        return command;
    }
}
