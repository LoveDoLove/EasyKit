# Dependency Constraints

This document describes the dependency constraints for EasyKit.

## Package Manager Policy

### Node.js Dependencies

**pnpm is the preferred package manager.**

- Use `pnpm` for installing Node.js dependencies
- `package-lock.json` should NOT be committed
- `pnpm-lock.yaml` SHOULD be committed
- npm is available as fallback but not recommended

### .NET Dependencies

- Use NuGet for .NET packages
- Reference projects via project references where possible
- Keep dependencies minimal

### Python Dependencies

**uv is the preferred package manager.**

- Use `uv` for Python package management
- `pyproject.toml` for project configuration
- `uv.lock` for lockfile

## Current Dependencies

### EasyKit (CLI)

| Dependency | Purpose |
|------------|---------|
| ReadLine | Console input handling |
| CommonUtilities | Shared utilities |

### EasyKit.Tests

| Dependency | Purpose |
|------------|---------|
| xunit | Test framework |
| xunit.runner.visualstudio | Test runner |
| Microsoft.NET.Test.Sdk | Test SDK |
| coverlet.collector | Code coverage |

### EasyKit-Gui (WPF)

| Dependency | Purpose |
|------------|---------|
| None external | Uses built-in WPF |

## Constraint Rules

1. **No runtime dependencies for core functionality** - EasyKit CLI should work without extra installs
2. **pnpm for website/docs** - The VitePress website uses pnpm
3. **Keep NuGet packages minimal** - Only add what's necessary
4. **Lockfiles required** - Commit lockfiles for reproducible builds

## Evidence

- `pnpm-workspace.yaml` exists for monorepo structure
- `package.json` uses pnpm semantics
- `.gitignore` excludes npm lockfiles, includes pnpm lockfile
