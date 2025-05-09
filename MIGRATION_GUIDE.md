# EasyKit Project Reorganization

This document provides guidance on the recent reorganization of the EasyKit project structure.

## Version Updates

### v1.2.6 (May 9, 2025)
- Switched installer from NSIS to WiX Toolset (MSI).
- Updated build process in GitHub Actions to use WiX.
- Removed NSIS-related files and scripts.

### v1.2.5 (May 9, 2025)
- Fixed GitHub Actions workflow after project restructuring
- Added scripts to fix path issues in the old installer script (now removed)
- Added documentation for GitHub Actions workflow fixes

### v1.2.4
- Restructured project for better organization and maintainability
- Moved scripts to dedicated subdirectories
- Reorganized configuration files

## Changes Made

The project has been reorganized to improve maintainability and clarity:

1. **Script Files**: All batch scripts have been moved to dedicated subdirectories:
   - `scripts/core/` - Core functionality scripts
   - `scripts/tools/` - Tool-specific scripts (npm, Laravel, etc.)
   - `scripts/build/` - Build and release scripts
   - `scripts/github/` - GitHub Actions integration scripts

2. **Configuration**: Configuration files are now in the `config/` directory.

3. **Installer**: WiX installer files are now in the `installer/` directory.

4. **Main Entry Point**: The `run_eskit.bat` remains in the root for easy access.

## Building and Releasing

To build a package:
1. Run `run_eskit.bat`
2. Select option 8 (Build and Release)
3. Choose the appropriate build option

## GitHub Actions

GitHub Actions workflow has been updated to use the new directory structure. The build process looks for files in:
- `installer/EasyKit.wxs` - For the WiX installer script
- `scripts/**/*.bat` - For all batch files to include in the package
- `run_eskit.bat` - The main entry point

### GitHub Actions Fixes in v1.2.5

After the project restructuring in v1.2.4, some issues were found with the GitHub Actions workflow, primarily related to file paths in the old NSIS installer script. Version 1.2.5 included temporary fixes, but as of 1.2.6, the project uses WiX Toolset for MSI creation.

1. **Fix Scripts**:
   - `scripts\github\fix_all_github_issues.bat` - Comprehensive fix script
   - `scripts\github\fix_github_workflow.ps1` - PowerShell script to fix the workflow file
   - `scripts\github\fix_installer_paths.bat` - (Removed) Script to fix paths in the old installer script

2. **Documentation**:
   - `docs\github_actions_fix.md` - Detailed information about the fixes

3. **Path Fixes**:
   - Updated image paths in the old installer script: from `images\icon.ico` to `..\images\icon.ico`
   - Updated LICENSE path: from `LICENSE` to `..\LICENSE`
   - Fixed directory structure references in the installer script

For more detailed information, refer to the [GitHub Actions Fix Documentation](docs/github_actions_fix.md).

## Reverting to Old Structure (If Needed)

If you need to revert to the old structure for any reason, run:

```
cd scripts\github
prepare_for_github_actions.bat
```

This will restore the necessary files to their original locations for GitHub Actions.
