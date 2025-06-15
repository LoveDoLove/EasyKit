# EasyKit Features

EasyKit is a modern toolkit for web developers, built on **.NET 8.0** and designed to automate common tasks, simplify project maintenance, and eliminate repetitive processes on Windows. It features auto-detection of essential tool paths, a user-friendly console UI, and robust logging. 

## Main Modules

- **npm:** Manage Node.js packages, run scripts, audit security, and troubleshoot npm issues. [Learn more](./features/npm.md)
- **Composer:** Manage PHP dependencies, install/update packages, and optimize Composer workflows. [Learn more](./features/composer.md)
- **Git:** Integrate with Git for version control, including repository management and diagnostics. [Learn more](./features/git.md)
- **Laravel:** Streamline Laravel development with quick setup, dependency management, cache reset, route listing, and diagnostics. [Learn more](./features/laravel.md)
- **Settings:** Customize EasyKit to fit your workflow, including menu width, logging, tips, and confirmations. [Learn more](./features/settings.md)
- **Tool Marketplace:** Discover, check, and install essential tools (Node.js, npm, PHP, Composer, Git) directly from EasyKit. The Tool Marketplace checks for installed tools, lists missing ones, and opens download pages for easy installation. [Learn more](./features/toolmarketplace.md)

## Logging & Configuration

EasyKit uses [Serilog](https://serilog.net/) for structured logging and [System.Text.Json](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-overview) for fast, modern JSON serialization. Logs are written to both the console and rolling files by default. Example setup:

```csharp
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
```

For more, see the official [Serilog documentation](https://github.com/serilog/serilog) and [Context7 Serilog docs](https://context7.com/library/serilog/serilog).

---

EasyKit is built on **.NET 8.0** and is designed primarily for Windows environments. For troubleshooting and more details, see the [troubleshooting guide](./troubleshooting.md).