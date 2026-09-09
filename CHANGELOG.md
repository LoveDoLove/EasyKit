# Changelog

All notable changes to EasyKit will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- Support for `uv` Python package manager in Tool Marketplace
- Support for `pnpm` package manager in Tool Marketplace
- .NET SDK tool detection
- Configuration for custom tool paths
- GitHub API-based version checking

### Changed
- Improved tool detection reliability
- Enhanced error messages for missing tools
- Updated documentation

### Fixed
- Fixed npm version check false positives
- Fixed process execution timeout issues

## [4.2.1] - 2025-01-15

### Added
- Git submodule management (add, update, remove)
- Stash operations in Git controller
- Production build support for Laravel
- Database seeding command for Laravel

### Changed
- Improved Git status parsing
- Enhanced menu color schemes

### Fixed
- Fixed branch detection in Git controller
- Resolved path handling in subprocess calls

## [4.1.8] - 2024-11-20

### Added
- Tool Marketplace for detecting missing dependencies
- Composer diagnostics
- PHP version checking

### Changed
- Updated to .NET 8.0
- Improved console output formatting

### Fixed
- Fixed composer.json parsing
- Resolved Windows path handling

## [4.0.6] - 2024-06-15

### Added
- Initial .NET port of EasyKit
- Basic Git, npm, Composer, and Laravel support
- Settings management
- Logging framework

### Changed
- Migrated from Python to .NET Core
- Restructured project layout
