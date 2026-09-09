# Security Architecture

This document describes the security architecture of EasyKit, specifically the SecureProcessRunner implementation.

## Overview

EasyKit uses `SecureProcessRunner` to prevent shell injection attacks when executing external commands. This is critical because the application executes user-facing commands from tools like Git, npm, pnpm, Composer, etc.

## Key Design Decisions

### 1. Command Whitelist
Only explicitly allowed commands can be executed:
- `git`, `npm`, `pnpm`, `corepack`, `composer`, `php`, `laravel`
- `uv`, `python`, `pip`, `dotnet`, `docker`, `node`
- `where`, `which`, `choco`, `nuget`
- `echo`, `type`, `dir`, `copy` (utility commands)

### 2. Argument Arrays
All commands use `ArgumentList` instead of string concatenation to prevent injection.

### 3. Dangerous Character Detection
Characters like `|`, `&`, `$`, `` ` ``, `;`, `<`, `>` are blocked in both commands and arguments.

### 4. Path Traversal Prevention
Arguments containing `..`, `/`, or starting with `-` are validated.

## Migration Path

All controllers were migrated from `CmdService` to `SecureProcessRunner`:
- `GitController.cs` - Fixed 9 injection points
- `NpmController.cs` - Added input validation
- `ComposerController.cs` - Added input validation
- `PnpmController.cs` - Added input validation + new commands
- `LaravelController.cs` - Fixed artisan injection

## Evidence

- Commit: `dd4ca70` - Security fixes for all controllers
- Test coverage: 48 unit tests passing
- Files modified: 9 files, +301 lines
