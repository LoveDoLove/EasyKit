# Security Constraints

This document describes the security constraints that all EasyKit development must respect.

## Core Security Principles

### 1. No Shell Execution

Never use `UseShellExecute = true` or execute through `cmd.exe`/`powershell.exe` with concatenated strings.

```csharp
// WRONG - shell execution
Process.Start("cmd", $"/c git commit -m \"{message}\"");

// CORRECT - direct execution
_processRunner.RunProcess("git", new[] { "commit", "-m", message });
```

### 2. Command Whitelist

Only allowed commands can execute. The whitelist is defined in `SecureProcessRunner`:

```csharp
private readonly HashSet<string> _allowedCommands = new(StringComparer.OrdinalIgnoreCase)
{
    "git", "npm", "pnpm", "corepack", "composer", "php", "laravel",
    "uv", "python", "pip", "dotnet", "docker", "node",
    "where", "which", "choco", "nuget", "echo", "type", "dir", "copy"
};
```

### 3. Input Validation

All user input must be validated:

```csharp
// Script names
if (!Regex.IsMatch(script, @"^[a-zA-Z0-9_\-:]+$"))
    throw new SecurityException("Invalid script name");

// Package names
if (!Regex.IsMatch(package, @"^[a-zA-Z0-9_\-/.]+$"))
    throw new SecurityException("Invalid package name");

// Branch names
if (!Regex.IsMatch(branch, @"^[a-zA-Z0-9_\-/.]+$"))
    throw new SecurityException("Invalid branch name");
```

### 4. Dangerous Character Detection

The following characters are blocked in commands and arguments:

| Character | Risk |
|-----------|------|
| `\`|`\`` | Pipe - command chaining |
| `&` | AND operator - command chaining |
| `$` | Variable expansion |
| `` ` `` | Backtick - command substitution |
| `;` | Separator - command chaining |
| `<` `>` | Redirection |
| `(` `)` | Subshell |
| `{` `}` | Brace expansion |
| `[` `]` | Glob expansion |
| `!` | History expansion |
| `#` | Comment |
| `%` | Environment variable |
| `^` | Escape character |
| `+` `=` | Arithmetic |

## Security Review Checklist

Before merging any code that executes commands:

- [ ] Uses `SecureProcessRunner`
- [ ] No string concatenation for arguments
- [ ] Input validation present
- [ ] Security tests added
- [ ] No shell execution (`UseShellExecute = false`)

## Consequences of Violation

Security vulnerabilities in EasyKit could lead to:
- Remote code execution
- Data destruction
- Credential theft
- System compromise

## Evidence

- 9 critical vulnerabilities fixed in audit
- 48 security tests added
- All controllers use SecureProcessRunner
- Security audit report: `reports/comprehensive-audit-report.md`
