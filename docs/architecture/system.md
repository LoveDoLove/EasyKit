# System Architecture

This document describes the overall system architecture of EasyKit.

## High-Level Architecture

EasyKit follows a controller-based architecture pattern where each development tool has its own controller handling user interactions and command execution.

```
┌─────────────────────────────────────────────────────────┐
│                     Program.cs                          │
│                 (Application Entry Point)                │
└─────────────────────────────────────────────────────────┘
                          │
                          ▼
┌─────────────────────────────────────────────────────────┐
│                    Main Menu                            │
│  ┌──────┐ ┌──────┐ ┌──────┐ ┌──────┐ ┌──────┐        │
│  │ Git  │ │ npm  │ │pnpm  │ │Comp. │ │Laravel│        │
│  └──┬───┘ └──┬───┘ └──┬───┘ └──┬───┘ └──┬───┘        │
│     │        │        │        │        │              │
│     ▼        ▼        ▼        ▼        ▼              │
│  ┌─────────────────────────────────────────────────┐   │
│  │              Controllers Layer                   │   │
│  │  (GitController, NpmController, PnpmController,  │   │
│  │   ComposerController, LaravelController,         │   │
│  │   UvController, DoctorController, SettingsCtrl)  │   │
│  └────────────────────────┬────────────────────────┘   │
│                           │                              │
│                           ▼                              │
│  ┌─────────────────────────────────────────────────┐   │
│  │              Services Layer                      │   │
│  │  (SecureProcessRunner, ConsoleService,          │   │
│  │   ProjectDetector, CmdService - legacy)         │   │
│  └────────────────────────┬────────────────────────┘   │
│                           │                              │
│                           ▼                              │
│  ┌─────────────────────────────────────────────────┐   │
│  │              Models Layer                        │   │
│  │  (Config, Software, MenuTheme,                  │   │
│  │   ProjectDetectionResult)                        │   │
│  └─────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────┘
```

## Data Flow

1. **User Input**: Menu selection via keyboard
2. **Controller Processing**: Controller handles user interaction
3. **Service Execution**: Services execute commands securely
4. **Output Display**: ConsoleService renders colored output
5. **Result Return**: Results displayed to user

## Security Architecture

```
┌─────────────────────────────────────────────────────────┐
│                 User Input                              │
└─────────────────────────────────────────────────────────┘
                          │
                          ▼
┌─────────────────────────────────────────────────────────┐
│              Input Validation                           │
│  - Command whitelist check                              │
│  - Dangerous character detection                        │
│  - Path traversal prevention                            │
└─────────────────────────────────────────────────────────┘
                          │
                          ▼
┌─────────────────────────────────────────────────────────┐
│            SecureProcessRunner                          │
│  - ArgumentList construction                            │
│  - Process execution with timeouts                      │
│  - Output/error capture                                 │
└─────────────────────────────────────────────────────────┘
                          │
                          ▼
┌─────────────────────────────────────────────────────────┐
│              External Command                           │
│  (git, npm, pnpm, composer, etc.)                       │
└─────────────────────────────────────────────────────────┘
```

## Key Design Patterns

### 1. Controller Pattern
Each tool has its own controller handling:
- Menu display
- User input processing
- Command execution
- Result handling

### 2. Service Pattern
Shared services provide:
- Secure process execution (`SecureProcessRunner`)
- Console output (`ConsoleService`)
- Tool detection (`Software`)
- Project detection (`ProjectDetector`)

### 3. Configuration Pattern
Settings stored in JSON:
- Location: `%APPDATA%\EasyKit\config.json`
- Keys: `enable_logging`, `log_path`, `show_tips`, `menu_width`, `version`

## Module Relationships

| Module | Depends On | Used By |
|--------|-----------|---------|
| GitController | SecureProcessRunner, ConsoleService | Program.cs |
| NpmController | SecureProcessRunner, ConsoleService | Program.cs |
| PnpmController | SecureProcessRunner, ConsoleService | Program.cs |
| ComposerController | SecureProcessRunner, ConsoleService | Program.cs |
| LaravelController | SecureProcessRunner, ConsoleService | Program.cs |
| UvController | SecureProcessRunner, ConsoleService | Program.cs |
| DoctorController | Software, ConsoleService | Program.cs |
| ToolMarketplaceController | Software, ConsoleService | Program.cs |
| ProjectDetector | File system | Program.cs, DoctorController |
| SecureProcessRunner | System.Diagnostics | All controllers |

## Extension Points

To add a new tool:
1. Create controller in `Controllers/`
2. Register in `Program.cs` menu
3. Add to `ToolMarketplaceController._essentialTools`
4. Add tests in `tests/`
5. Update documentation
