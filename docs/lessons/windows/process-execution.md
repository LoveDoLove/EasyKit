# Windows Process Execution Patterns

**Date:** 2025-09-09  
**Type:** Lesson  
**Status:** Current

## Problem

Executing external commands on Windows requires handling multiple executable extensions and PATH resolution.

## Root Cause

Windows commands can be:
- `.exe` files (e.g., `git.exe`)
- `.cmd` files (e.g., `npm.cmd`)
- `.bat` files (e.g., `composer.bat`)
- `.ps1` files (e.g., `dotnet-script.ps1`)

Simply calling `Process.Start("git")` may fail if the executable isn't in PATH or has a non-standard extension.

## Incorrect Approach

```csharp
// FAILS: Can't find command without extension
Process.Start("git", "status");

// FAILS: String concatenation is vulnerable
Process.Start("cmd", $"/c git commit -m \"{message}\"");
```

## Correct Approach

```csharp
public class SecureProcessRunner
{
    private static string ResolveCommand(string command)
    {
        // If already a full path, use as-is
        if (command.Contains(Path.DirectorySeparatorChar))
            return command;
        
        // Try exact match
        if (File.Exists(command))
            return command;
        
        // Search PATH for executable
        var pathEnv = Environment.GetEnvironmentVariable("PATH") ?? "";
        var pathDirs = pathEnv.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries);
        var extensions = new[] { ".exe", ".cmd", ".bat", ".ps1" };
        
        foreach (var dir in pathDirs)
        {
            if (!Directory.Exists(dir)) continue;
            
            foreach (var ext in extensions)
            {
                var candidate = Path.Combine(dir, command + ext);
                if (File.Exists(candidate))
                    return candidate;
            }
        }
        
        return command; // Fall back to original
    }
}
```

## Key Principles

1. **Always resolve command paths** before execution
2. **Use ArgumentList** instead of string concatenation
3. **Set UseShellExecute = false** for security
4. **Handle timeouts** for long-running commands
5. **Support cancellation** for interactive operations

## Why It Matters

Without proper path resolution, commands fail even when installed. Without argument arrays, users are vulnerable to injection attacks.

## Evidence

- `SecureProcessRunner.cs` - Implementation
- Tests: 16 tests for process execution
- Controllers: All migrated to use SecureProcessRunner
