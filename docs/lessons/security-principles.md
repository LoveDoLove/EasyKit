# Security Principles for Development Tools

**Date:** 2025-09-09  
**Type:** Lesson  
**Status:** Current

## Problem

Development tools like EasyKit execute external commands on behalf of users. This creates security risks if user input is not properly handled.

## Key Principles

### 1. Never Trust User Input

All user input must be validated before use in commands:
- Script names
- Package names
- Branch names
- Commit messages
- File paths

### 2. Use Argument Arrays, Not String Concatenation

```csharp
// WRONG - vulnerable to injection
Process.Start("git", $"commit -m \"{message}\"");

// CORRECT - safe argument array
Process.Start("git", new[] { "commit", "-m", message });
```

### 3. Maintain Command Whitelists

Only allow known-safe commands:
```csharp
private readonly HashSet<string> _allowedCommands = new()
{
    "git", "npm", "pnpm", "composer", "php", "uv", "python", "dotnet"
};
```

### 4. Block Dangerous Characters

Detect and reject arguments containing:
- Shell metacharacters: `|`, `&`, `$`, `` ` ``, `;`
- Redirection: `<`, `>`
- Arithmetic: `+`, `=`
- Path traversal: `..`, `/` at start

### 5. Resolve Command Paths Explicitly

Don't rely on PATH alone:
```csharp
// Search for .exe, .cmd, .bat, .ps1
var extensions = new[] { ".exe", ".cmd", ".bat", ".ps1" };
```

## Why It Matters

Development tools have high privileges (they run in the user's context). A single injection vulnerability could:
- Delete files
- Execute malicious code
- Expose secrets
- Compromise the system

## Evidence

- 9 critical vulnerabilities fixed
- 48 security tests added
- All controllers migrated to SecureProcessRunner

## References

- [Secure Process Execution Decision](../../decisions/secure-process-execution.md)
- [Command Injection Prevention Solution](../../solutions/security/injection-prevention.md)
- [Windows Process Execution Patterns](./process-execution.md)
