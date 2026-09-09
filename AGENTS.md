---
l0_domains:
  architecture: "System structure, module boundaries, integration points"
  decisions: "Key engineering choices and their rationale"
  solutions: "Diagnosed fix patterns for recurring issues and bugs"
  lessons: "Reusable engineering principles distilled from completed work"
  workflows: "Development, testing, and deployment procedures"
  constraints: "Security, platform, and compatibility boundaries"
---

# EasyKit Agent Memory

## Project Identity

EasyKit is a .NET 8 Windows desktop toolkit for web developers. It provides a unified console interface for managing development tools including Git, npm/pnpm, Composer, Laravel, and uv (Python).

**Repository:** https://github.com/LoveDoLove/EasyKit  
**Version:** 4.2.2.0  
**Platform:** Windows 10/11  
**Framework:** .NET 8.0

## Critical Rules

1. **Security First:** All command execution must use `SecureProcessRunner` with argument arrays - never shell interpretation
2. **Windows Target:** This is a Windows-only application; do not introduce cross-platform abstractions without explicit approval
3. **Conventional Commits:** Use `feat:`, `fix:`, `docs:`, `refactor:`, `test:`, `chore:` prefixes
4. **Test Coverage:** New features require unit tests; maintain 48+ passing tests
5. **pnpm First:** Use pnpm for Node.js dependencies; npm is fallback only

## Architecture Overview

```
EasyKit/
├── Controllers/     # Menu handlers (Git, Npm, Pnpm, Composer, Laravel, Uv, Doctor, Settings)
├── Models/          # Data models (Config, ProjectDetector, Software)
├── Services/        # Core services (SecureProcessRunner, ConsoleService)
├── Helpers/         # UI helpers (ConfirmationHelper)
├── UI/ConsoleUI/    # Console interface components
└── Program.cs       # Application entry point
```

**Key Components:**
- `SecureProcessRunner` - Shell injection prevention via command whitelist and argument arrays
- `ProjectDetector` - Automatic project type detection (Git, Node.js, pnpm, Python, uv, PHP, .NET, Docker)
- `DoctorController` - Environment diagnostics (option 8)
- `ToolMarketplaceController` - Tool detection and installation guidance

## Critical Constraints

- **Platform:** Windows 10/11 only
- **Runtime:** .NET 8.0 SDK required
- **Security:** No shell execution; all commands use structured arguments
- **Package Manager:** pnpm preferred for Node.js; uv for Python
- **Config Location:** `%APPDATA%\EasyKit\config.json`

## Verification Requirements

Before marking work complete:
- [ ] All 48 tests pass: `dotnet test tests/EasyKit.Tests/EasyKit.Tests.csproj`
- [ ] Build succeeds: `dotnet build EasyKit.sln`
- [ ] Security review: No string concatenation for command arguments
- [ ] Documentation updated: README.md, CHANGELOG.md

## Memory Navigation

### Architecture
- System structure: `docs/architecture/README.md`
- Module boundaries: `docs/architecture/modules.md`
- Security design: `docs/architecture/security.md`

### Decisions
- Engineering choices: `docs/decisions/README.md`
- Package manager selection: `docs/decisions/pnpm-first.md`
- Security architecture: `docs/decisions/secure-process-execution.md`

### Solutions
- Bug fixes: `docs/solutions/README.md`
- Command injection prevention: `docs/solutions/security/injection-prevention.md`

### Lessons
- Engineering principles: `docs/lessons/README.md`
- Windows process execution: `docs/lessons/windows/process-execution.md`

### Workflows
- Development: `docs/workflows/README.md`
- Testing: `docs/workflows/testing.md`
- Release: `docs/workflows/release.md`

### Constraints
- Platform limitations: `docs/constraints/README.md`
- Security boundaries: `docs/constraints/security.md`

## Related Documentation

- [README.md](../README.md) - User-facing project introduction
- [CHANGELOG.md](../CHANGELOG.md) - Version history
- [CONTRIBUTING.md](../CONTRIBUTING.md) - Contribution guidelines
- [Audit Report](../reports/comprehensive-audit-report.md) - Security audit findings
