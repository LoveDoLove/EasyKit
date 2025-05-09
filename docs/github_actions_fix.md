# GitHub Actions Workflow Fix

If you're experiencing issues with the GitHub Actions workflow after the v1.2.4 update which includes project restructuring, you can use the provided fix scripts that were added in v1.2.5:

1. Run `scripts\github\fix_all_github_issues.bat` to apply all fixes in one go
2. Or run the individual fix scripts:
   - `scripts\github\fix_github_workflow.ps1` - Fixes workflow file
   - `scripts\github\fix_installer_paths.bat` - Fixes NSIS installer paths
   
The main issue with the workflow was that the NSIS installer script needed to be updated to use relative paths that match the new project structure. This affected the paths to image files and the LICENSE file.

## Technical Details

The fix addresses:
1. Image file paths in the NSIS script (changed from `images\icon.ico` to `..\images\icon.ico`)
2. LICENSE file path (changed from `LICENSE` to `..\LICENSE`)
3. Script file paths to account for the new directory structure
4. GitHub Actions workflow configuration

After applying these fixes, the GitHub Actions workflow should run successfully again.
