# pnpm First Strategy

**Date:** 2025-09-09  
**Status:** Current  
**Related:** [Secure Process Execution](secure-process-execution.md)

## Context

EasyKit supports multiple package managers for different ecosystems:
- Node.js: npm, pnpm
- PHP: Composer
- Python: pip, uv

## Decision

**pnpm is the preferred package manager for Node.js projects.**

## Rationale

1. **Performance**: pnpm is significantly faster than npm for installs
2. **Disk Space**: pnpm uses content-addressable storage, deduplicating packages
3. **Strictness**: pnpm enforces strict dependency resolution, preventing hidden dependencies
4. **Workspace Support**: First-class monorepo/workspace support
5. **Lockfile Safety**: `pnpm-lock.yaml` is more deterministic than `package-lock.json`

## Implementation

- Project detection prioritizes `pnpm-lock.yaml` and `pnpm-workspace.yaml`
- Menu option 3 provides pnpm-specific commands
- Documentation recommends pnpm for new projects
- npm is available as fallback (option 2)

## Alternatives Considered

| Alternative | Reason for Rejection |
|-------------|---------------------|
| npm-only | Slower, less strict, larger disk usage |
| yarn-only | Declining popularity, less ecosystem support |
| All managers equal | Confusion for users, inconsistent behavior |

## Consequences

- New users should default to pnpm for Node.js projects
- Migration guides should be provided for npm→pnpm transitions
- Documentation should mention pnpm preference

## Evidence

- `ProjectDetector.cs` checks for `pnpm-lock.yaml` first
- `PnpmController.cs` provides comprehensive pnpm commands
- README.md documents pnpm as first-class support
