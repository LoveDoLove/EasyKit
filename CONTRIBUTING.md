# Contributing to EasyKit

Thank you for your interest in contributing to EasyKit! This document provides guidelines for contributing.

## Development Environment

### Prerequisites
- .NET 8.0 SDK
- Visual Studio 2022+ or JetBrains Rider
- Git

### Setting Up
1. Fork the repository
2. Clone your fork: `git clone https://github.com/LoveDoLove/EasyKit.git`
3. Create a feature branch: `git checkout -b feature/your-feature-name`

## Code Style Guidelines

### C# Conventions
- Use `var` for inferred types
- Follow Microsoft C# coding conventions
- Use XML documentation comments for public methods
- Keep methods focused and under 50 lines where possible

### Naming Conventions
- Classes: PascalCase (`ToolMarketplaceController`)
- Methods: PascalCase (`ShowMarketplace()`)
- Private fields: underscore prefix (`_console`)
- Constants: PascalCase (`NPM_TOOL_NAME`)

### Git Commit Messages
Follow [Conventional Commits](https://www.conventionalcommits.org/):

```
feat: add uv tool detection
fix: resolve npm version parsing issue
docs: update README with new features
refactor: extract tool detection logic
chore: update dependencies
```

## Adding New Tools

### 1. Update ToolMarketplaceController
Add your tool to `_essentialTools` list in `EasyKit/Controllers/ToolMarketplaceController.cs`:

```csharp
new ToolInfo("My Tool", "mytool", "https://example.com/", "--version"),
```

### 2. Add Detection Method (Optional)
For custom detection logic, add a method to `Software.cs`:

```csharp
private bool Check_mytool()
{
    // Custom detection logic
}
```

### 3. Create Controller (Optional)
If your tool needs a dedicated controller, create `Controllers/MyToolController.cs` following the pattern in `NpmController.cs`.

## Pull Request Process

1. Ensure your code passes all tests
2. Update documentation if needed
3. Submit a pull request with a clear description
4. Respond to review feedback

## Reporting Issues

Use GitHub Issues to report bugs or request features. Include:
- Steps to reproduce
- Expected behavior
- Actual behavior
- Screenshots (if applicable)
