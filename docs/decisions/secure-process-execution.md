# Secure Process Execution

**Date:** 2025-09-09  
**Status:** Current  
**Related:** [pnpm First Strategy](pnpm-first.md)

## Context

EasyKit executes external commands (git, npm, pnpm, composer, etc.) on behalf of users. This creates a security risk if user input is not properly sanitized.

## Problem

Previous implementations used string concatenation for command arguments:
```csharp
// VULNERABLE - string concatenation
string args = $"commit -m \"{userInput}\"";
Process.Start("git", args);
```

This allows command injection attacks:
- User enters: `"feature; rm -rf /"`
- Resulting command: `git commit -m "feature; rm -rf /"`
- Attacker can execute arbitrary commands

## Decision

**All command execution must use `SecureProcessRunner` with argument arrays.**

## Implementation

### SecureProcessRunner Design

```csharp
public class SecureProcessRunner
{
    private readonly HashSet<string> _allowedCommands = new()
    {
        "git", "npm", "pnpm", "corepack", "composer", "php", "laravel",
        "uv", "python", "pip", "dotnet", "docker", "node",
        "where", "which", "choco", "nuget", "echo", "type", "dir", "copy"
    };

    private readonly HashSet<char> _dangerousChars = new()
    { '`', '|', '&', '$', '(', ')', '{', '}', '[', ']', '!', '#', '%', '^', '<', '>', ';', '+', '=' };
}
```

### Usage Pattern

```csharp
// SAFE - using SecureProcessRunner
private readonly SecureProcessRunner _processRunner;

var result = _processRunner.RunProcess(
    "git",
    new[] { "commit", "-m", userMessage },
    workingDirectory
);
```

## Security Layers

1. **Command Whitelist**: Only allowed commands can execute
2. **Dangerous Character Detection**: Blocks `|`, `&`, `$`, `` ` ``, `;`, etc.
3. **Argument Arrays**: Uses `ArgumentList` instead of string concatenation
4. **Path Traversal Prevention**: Blocks `..` in arguments
5. **Windows Executable Resolution**: Handles `.exe`, `.cmd`, `.bat`, `.ps1`

## Migration

All controllers were migrated from `CmdService` to `SecureProcessRunner`:
- `GitController.cs` - 9 injection points fixed
- `NpmController.cs` - Input validation added
- `ComposerController.cs` - Input validation added
- `PnpmController.cs` - Input validation + new commands
- `LaravelController.cs` - Artisan injection fixed

## Evidence

- Commit: `dd4ca70` - Security fixes
- Tests: 48 unit tests covering security scenarios
- Audit Report: `reports/comprehensive-audit-report.md`
