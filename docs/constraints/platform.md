# Platform Constraints

This document describes the platform constraints for EasyKit.

## Target Platform

**Windows 10/11 only**

EasyKit is designed specifically for Windows systems and does not support Linux or macOS.

## Runtime Requirements

- **.NET 8.0 SDK** - Required for building and running
- **Windows 10 version 1809 or later** - Minimum OS version
- **Administrator privileges** - Recommended for some operations

## Why Windows-Only?

1. **Context Menu Integration** - Windows Explorer integration requires Windows API
2. **Inno Setup Installer** - Windows-only installer technology
3. **WPF GUI** - EasyKit-Gui uses WPF, which is Windows-only
4. **Tool Ecosystem** - Many development tools have Windows-first support

## Implications

### What This Means

- No cross-platform support planned
- No Linux/macOS testing required
- Windows-specific APIs can be used freely
- PowerShell and cmd.exe are primary shells

### What This Doesn't Mean

- Can still use cross-platform .NET features
- Can still follow good OOP practices
- Can still write portable code (if desired)

## Future Considerations

If cross-platform support is ever needed:
- Use `System.Environment.OSVersion` to detect platform
- Abstract Windows-specific calls behind interfaces
- Consider MAUI or Avalonia for cross-platform GUI

## Evidence

- Project targets `net8.0` with Windows-specific references
- Inno Setup used for installer
- WPF used for GUI (EasyKit-Gui)
- README documents Windows 10/11 requirement
