# Development Workflow

This document describes the development workflow for EasyKit.

## Prerequisites

- .NET 8.0 SDK
- Visual Studio 2022+ or JetBrains Rider
- Git
- pnpm (for Node.js dependencies)

## Setting Up

```powershell
# Clone the repository
git clone https://github.com/LoveDoLove/EasyKit.git
cd EasyKit

# Build the solution
dotnet build EasyKit.sln

# Run tests
dotnet test tests/EasyKit.Tests/EasyKit.Tests.csproj

# Run the application
dotnet run --project EasyKit/EasyKit.csproj
```

## Development Steps

1. **Create a feature branch**
   ```powershell
   git checkout -b feature/your-feature-name
   ```

2. **Make changes**
   - Follow coding conventions in CONTRIBUTING.md
   - Add unit tests for new functionality
   - Update documentation as needed

3. **Run tests**
   ```powershell
   dotnet test tests/EasyKit.Tests/EasyKit.Tests.csproj
   ```
   Ensure all 48 tests pass.

4. **Build and verify**
   ```powershell
   dotnet build EasyKit.sln
   dotnet run --project EasyKit/EasyKit.csproj
   ```

5. **Commit with conventional commits**
   ```powershell
   git add .
   git commit -m "feat: add uv tool support"
   ```

6. **Push and create PR**
   ```powershell
   git push origin feature/your-feature-name
   ```

## Code Review Checklist

- [ ] All tests pass
- [ ] Security review completed (no string concatenation for commands)
- [ ] Documentation updated
- [ ] CHANGELOG.md updated
- [ ] Follows coding conventions

## Branch Strategy

- `main` - Stable release branch
- `feature/*` - Feature branches
- `fix/*` - Bug fix branches
- `release/*` - Release preparation branches
