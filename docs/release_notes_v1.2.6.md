# Release Notes - EasyKit v1.2.6

**Release Date:** May 9, 2025

## Changes

- Switched installer technology from NSIS to WiX Toolset (MSI).
- Updated the GitHub Actions build workflow to use WiX for creating MSI packages.
- Removed all NSIS-related scripts and configuration files from the project.
- Updated version numbers across project files to 1.2.6.
- Added `EasyKit.wxs` for WiX installer definition.
- Created `scripts\build\release_v1.2.6.bat` to support WiX and the new version.

## Bug Fixes

- Fixed undefined preprocessor variable `$(var.ProjectDir)` in WiX build process
- Resolved missing build files error in the GitHub Actions workflow
- Enhanced GitHub Actions workflow to ensure required directories and files exist

## Installation

Download the installer from the [releases page](https://github.com/LoveDoLove/EasyKit/releases/tag/v1.2.6) and run the setup.

## Known Issues

None at this time.