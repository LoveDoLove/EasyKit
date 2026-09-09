# Module Boundaries

This document describes the module boundaries and responsibilities in EasyKit.

## Controllers

| Controller | Responsibility | Key Methods |
|------------|---------------|-------------|
| `GitController` | Git workflow management | Status, commit, push, pull, branch, merge, stash |
| `NpmController` | npm package management | Install, update, run scripts |
| `PnpmController` | pnpm package management | Add, remove, exec, dlx |
| `ComposerController` | PHP dependency management | Require, update, install |
| `LaravelController` | Laravel Artisan commands | Artisan command execution |
| `UvController` | Python package management | Install, upgrade, remove packages |
| `DoctorController` | Environment diagnostics | Check tool installation status |
| `SettingsController` | Application settings | Configuration management |
| `ToolMarketplaceController` | Tool detection | Detect and install missing tools |

## Services

| Service | Responsibility | Key Methods |
|---------|---------------|-------------|
| `SecureProcessRunner` | Secure process execution | `RunProcess()`, `RunProcessStreaming()` |
| `ConsoleService` | Colorized output | `WriteInfo()`, `WriteSuccess()`, `WriteError()` |
| `CmdService` | Legacy process execution | (deprecated, use SecureProcessRunner) |

## Models

| Model | Responsibility |
|-------|---------------|
| `Config` | Application configuration (JSON) |
| `ProjectDetector` | Auto-detect project types |
| `ProjectDetectionResult` | Detection result data |
| `Software` | Tool presence checking |
| `MenuTheme` | Console menu theming |

## Key Integration Points

1. **Program.cs** - Entry point, initializes controllers and menu
2. **SecureProcessRunner** - Used by all controllers for command execution
3. **ProjectDetector** - Called from Program.cs to detect project type
4. **ToolMarketplaceController** - Checks tool availability for Doctor

## Dependencies

```
EasyKit
├── Controllers (depend on Services)
│   ├── GitController → SecureProcessRunner
│   ├── NpmController → SecureProcessRunner
│   ├── PnpmController → SecureProcessRunner
│   ├── ComposerController → SecureProcessRunner
│   ├── LaravelController → SecureProcessRunner
│   ├── UvController → SecureProcessRunner
│   ├── DoctorController → Software
│   └── ToolMarketplaceController → Software
├── Models
│   ├── ProjectDetector → File system
│   └── Config → JSON serialization
└── Services
    ├── SecureProcessRunner → System.Diagnostics
    └── ConsoleService → System.Console
```
