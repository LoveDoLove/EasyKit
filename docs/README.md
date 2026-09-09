# EasyKit Documentation

## Project Overview

EasyKit is a .NET 8 Windows desktop toolkit designed for web developers. It provides a unified console interface for managing common development tools including Git, Node.js (npm/pnpm), PHP (Composer/uv), and Laravel.

## Architecture

### Solution Structure

```
EasyKit/
├── EasyKit/                    # Console CLI application
│   ├── Controllers/            # Menu controllers for each tool category
│   │   ├── GitController.cs    # Git workflow management
│   │   ├── NpmController.cs    # npm package management
│   │   ├── ComposerController.cs  # PHP Composer management
│   │   ├── LaravelController.cs    # Laravel Artisan commands
│   │   ├── SettingsController.cs   # Application settings
│   │   └── ToolMarketplaceController.cs  # Tool detection & installation
│   ├── Models/
│   │   ├── Config.cs           # Application configuration (JSON-based)
│   │   ├── Software.cs         # OS detection and tool presence checks
│   │   ├── MenuTheme.cs        # Console menu theming
│   │   └── ReadLineModel.cs    # ReadLine abstraction layer
│   ├── Services/
│   │   ├── CmdService.cs       # Cross-platform process execution
│   │   └── ConsoleService.cs   # Colorized console output wrapper
│   ├── Helpers/Console/
│   │   └── ConfirmationHelper.cs  # User confirmation dialogs
│   ├── UI/ConsoleUI/
│   │   ├── MenuView.cs         # Styled menu rendering
│   │   ├── PromptView.cs       # User input prompts
│   │   ├── NotificationView.cs # Status notifications
│   │   ├── HeaderView.cs       # Application header
│   │   ├── ProgressView.cs     # Progress indicators
│   │   └── KeyboardShortcut.cs # Keyboard shortcut handling
│   ├── Utilities/
│   │   └── ConsoleUtilities.cs # Console manipulation utilities
│   ├── Program.cs              # Application entry point
│   └── EasyKit.csproj          # Project file
├── EasyKit-Gui/                # WPF GUI application (feature branch)
│   ├── Views/
│   │   ├── HomePage.cs         # Main dashboard
│   │   └── Modules/            # Module views (Git, npm, etc.)
│   ├── Utilities/              # GUI-specific helpers
│   └── EasyKit-Gui.csproj      # WPF project file
├── docs/                       # Documentation
│   ├── README.md               # This file
│   └── architecture.md         # Detailed architecture guide
├── images/                     # Assets
│   └── icon.jpg                # Application icon
└── easykit.sln                 # Visual Studio solution
```

## Key Components

### CmdService
The `CmdService` in `Services/CmdService.cs` wraps process execution with three modes:

1. **RunProcess**: Captures output/error/exit code (non-interactive)
2. **RunProcessWithStreaming**: Streams output in real-time (interactive)
3. **RunProcessInNewCmdWindow**: Launches in a separate window (long-running)

### Config
Configuration is stored as JSON in `%APPDATA%\EasyKit\config.json` with these defaults:

| Setting | Default | Description |
|---------|---------|-------------|
| `enable_logging` | `true` | Enable file logging |
| `log_path` | Auto | Path to log file |
| `show_tips` | `true` | Show helpful tips |
| `menu_width` | `100` | Menu display width |
| `version` | `4.2.1` | Current app version |

### Software Detection
The `Software` class in `Models/Software.cs` provides:

- OS platform detection (Windows/Linux/macOS)
- Extensible tool presence checking via `CheckSoftware(string)`
- Named check methods (`Check_git`, `Check_node`, etc.)

## Tool Marketplace

The Tool Marketplace (`ToolMarketplaceController`) detects installed development tools:

| Tool | Command | Version Check |
|------|---------|---------------|
| Node.js | `node` | `--version` |
| npm | `npm` | `--version` |
| pnpm | `pnpm` | `--version` |
| uv | `uv` | `--version` |
| PHP | `php` | `--version` |
| Composer | `composer` | `--version` |
| Git | `git` | `--version` |
| dotnet | `dotnet` | `--version` |

## Getting Started

### Prerequisites
- Windows 10/11
- .NET 8.0 SDK

### Building from Source
```powershell
cd EasyKit
dotnet build
dotnet run --project EasyKit/EasyKit.csproj
```

### Running Tests
```powershell
dotnet test
```

## Configuration

### Custom Tool Paths
To add custom tool paths, set them in the environment or config:

```json
{
  "tool_paths": {
    "node": "C:\\path\\to\\node.exe",
    "npm": "C:\\path\\to\\npm.cmd",
    "php": "C:\\path\\to\\php.exe",
    "composer": "C:\\path\\to\\composer.phar",
    "dotnet": "C:\\path\\to\\dotnet.exe"
  }
}
```

### Logging
Logs are stored in `%APPDATA%\EasyKit\logs\EasyKit.log`

## Contributing

See [CONTRIBUTING.md](../CONTRIBUTING.md) for guidelines.

## License

MIT License - See [LICENSE](../LICENSE) for details.
