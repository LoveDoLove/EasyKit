# EasyKit Project Reorganization

This document provides guidance on the recent reorganization of the EasyKit project structure.

## Changes Made

The project has been reorganized to improve maintainability and clarity:

1. **Script Files**: All batch scripts have been moved to dedicated subdirectories:
   - `scripts/core/` - Core functionality scripts
   - `scripts/tools/` - Tool-specific scripts (npm, Laravel, etc.)
   - `scripts/build/` - Build and release scripts
   - `scripts/github/` - GitHub Actions integration scripts

2. **Configuration**: Configuration files are now in the `config/` directory.

3. **Installer**: NSIS installer files are now in the `installer/` directory.

4. **Main Entry Point**: The `run_eskit.bat` remains in the root for easy access.

## Building and Releasing

To build a package:
1. Run `run_eskit.bat`
2. Select option 8 (Build and Release)
3. Choose the appropriate build option

## GitHub Actions

GitHub Actions workflow has been updated to use the new directory structure. The build process looks for files in:
- `installer/EasyKit.nsi` - For the NSIS installer script
- `scripts/**/*.bat` - For all batch files to include in the package
- `run_eskit.bat` - The main entry point

## Reverting to Old Structure (If Needed)

If you need to revert to the old structure for any reason, run:

```
cd scripts\github
prepare_for_github_actions.bat
```

This will restore the necessary files to their original locations for GitHub Actions.
