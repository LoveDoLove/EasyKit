# Controller Implementation Patterns

**Date:** 2025-09-09  
**Type:** Lesson  
**Status:** Current

## Problem

EasyKit has multiple controllers (Git, npm, pnpm, Composer, Laravel, uv, Doctor, Settings, Tool Marketplace) that need consistent implementation patterns.

## Pattern

### 1. Constructor Injection

```csharp
public class GitController
{
    private readonly SecureProcessRunner _processRunner;
    private readonly IConsole _console;
    
    public GitController(SecureProcessRunner processRunner, IConsole console)
    {
        _processRunner = processRunner;
        _console = console;
    }
}
```

### 2. Helper Methods

Extract common operations into private helpers:

```csharp
private (string output, string error, int exitCode) RunGitCommand(
    IEnumerable<string> arguments, 
    string? workingDirectory = null)
{
    return _processRunner.RunProcess(
        "git", 
        arguments, 
        workingDirectory ?? Environment.CurrentDirectory
    );
}
```

### 3. Input Validation

Validate all user inputs before passing to commands:

```csharp
// Validate script names
if (!Regex.IsMatch(script, @"^[a-zA-Z0-9_\-:]+$"))
{
    _console.WriteError("Invalid script name...");
    return;
}
```

### 4. Error Handling

```csharp
var (output, error, exitCode) = RunGitCommand(arguments);

if (exitCode != 0)
{
    _console.WriteError(error);
    return;
}

_console.WriteSuccess(output);
```

### 5. Menu Structure

```csharp
public void ShowMenu()
{
    while (true)
    {
        _console.WriteHeader("Git Tools");
        
        _console.WriteMenuOption("1", "Status");
        _console.WriteMenuOption("2", "Commit");
        _console.WriteMenuOption("3", "Push");
        _console.WriteMenuOption("0", "Back");
        
        var choice = _console.ReadChoice();
        
        switch (choice)
        {
            case "1": ShowStatus(); break;
            case "2": ShowCommit(); break;
            // ...
            case "0": return;
        }
    }
}
```

## Why It Matters

Consistent patterns make controllers:
- Easier to implement
- Easier to maintain
- Easier to test
- Less prone to security vulnerabilities

## Evidence

- All 9 controllers follow this pattern
- 48 unit tests cover the patterns
- Migration from CmdService to SecureProcessRunner standardized the approach
