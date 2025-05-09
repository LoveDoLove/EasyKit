# EasyKit v1.2.5 Release Notes

## Overview
This is a maintenance release that fixes GitHub Actions workflow issues introduced after the project restructuring in v1.2.4.

## What's New
- Fixed GitHub Actions workflow to properly handle the new project structure
- Updated NSIS installer script to use correct relative paths
- Added comprehensive fix scripts in the `scripts\github` directory
- Added documentation for GitHub Actions workflow fixes

## Fixed Issues
- GitHub Actions workflow errors related to image files not being found
- NSIS installer build failures in CI environment
- Path issues in installer script after project restructuring

## Fix Scripts
- `scripts\github\fix_all_github_issues.bat` - Comprehensive fix script that applies all needed changes
- `scripts\github\fix_github_workflow.ps1` - PowerShell script to fix the GitHub Actions workflow file
- `scripts\github\fix_installer_paths.bat` - Script to fix paths in the NSIS installer script

## Documentation
Added `docs\github_actions_fix.md` with detailed information about the fixes and how to apply them.

## Installation
Download the installer or ZIP package from the [releases page](https://github.com/LoveDoLove/EasyKit/releases).

## Upgrade Instructions
If you're using v1.2.4, you can update by downloading the latest release or by pulling the latest changes from the repository.

For manual upgrade from the repository:
```bash
git pull origin main
git checkout v1.2.5
```
