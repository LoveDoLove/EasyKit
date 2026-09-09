# Adding New Tools

This document describes how to add support for new development tools to EasyKit.

## Overview

EasyKit supports multiple tool categories:
- Git (version control)
- npm/pnpm (Node.js package management)
- Composer (PHP dependency management)
- Laravel (PHP framework)
- uv (Python package management)
- Settings (application configuration)
- Doctor (environment diagnostics)
- Tool Marketplace (tool detection)

## Steps to Add a New Tool

### 1. Create Controller

Create a new controller in `EasyKit/Controllers/`:

```csharp
using EasyKit.Services;
using EasyKit.Helpers.Console;

namespace EasyKit.Controllers;

public class MyToolController
{
    private readonly SecureProcessRunner _processRunner;
    private readonly IConsole _console;

    public MyToolController(SecureProcessRunner processRunner, IConsole console)
    {
        _processRunner = processRunner;
        _console = console;
    }

    public void ShowMenu()
    {
        while (true)
        {
            _console.WriteHeader("My Tool");
            
            _console.WriteMenuOption("1", "Option 1");
            _console.WriteMenuOption("2", "Option 2");
            _console.WriteMenuOption("0", "Back");
            
            var choice = _console.ReadChoice();
            
            switch (choice)
            {
                case "1": HandleOption1(); break;
                case "2": HandleOption2(); break;
                case "0": return;
            }
        }
    }

    private void HandleOption1()
    {
        // Implement option 1
    }

    private void HandleOption2()
    {
        // Implement option 2
    }
}
```

### 2. Register in Program.cs

Add to the main menu:

```csharp
// In Program.cs
var myToolController = new MyToolController(processRunner, console);

// Add to menu options
_console.WriteMenuOption("9", "My Tool");

// Handle selection
case "9":
    myToolController.ShowMenu();
    break;
```

### 3. Add to Tool Marketplace

Register the tool for detection:

```csharp
// In ToolMarketplaceController.cs
new ToolInfo("My Tool", "mytool", "https://example.com/", "--version"),
```

### 4. Add Tests

Create tests in `tests/EasyKit.Tests/UnitTests/`:

```csharp
using EasyKit.Controllers;
using Xunit;

namespace EasyKit.Tests.UnitTests;

public class MyToolControllerTests
{
    [Fact]
    public void TestMethod()
    {
        // Test implementation
    }
}
```

### 5. Update Documentation

- Update README.md with new feature
- Update CHANGELOG.md
- Add to docs/ if needed

## Security Requirements

All new tools MUST:
1. Use `SecureProcessRunner` for command execution
2. Validate all user inputs
3. Use argument arrays, not string concatenation
4. Include security tests

## Evidence

- Existing controllers follow this pattern
- 9 controllers currently implemented
- Security audit passed for all
