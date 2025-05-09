# GitHub Actions Workflow Fix

If you're experiencing issues with the GitHub Actions workflow for EasyKit v1.2.6, which now uses WiX Toolset instead of NSIS, you can use the provided fix scripts:

1. Run `scripts\github\fix_all_github_issues.bat` to apply all fixes in one go
2. Or run the individual fix scripts:
   - `scripts\github\fix_wix_workflow.bat` - Creates a properly formatted workflow file for WiX
   - `scripts\github\verify_github_actions_setup.bat` - Verifies your setup is correct

## Latest Fixes (v1.2.6)

For the v1.2.6 release, these fixes address the following issues:

1. **Undefined Preprocessor Variable**: Added `-dProjectDir` parameter to the `candle.exe` command
2. **Missing Build Files**: Ensures the `scripts\build` directory and `release_v1.2.6.bat` file exist
3. **PowerShell Syntax Issues**: Fixed shell-specific syntax in workflow commands by using `shell: cmd` explicitly
4. **Workflow Stability**: Made the entire workflow more reliable with proper error handling

## Technical Details

The fixes address:

1. WiX Toolset configuration issues (preprocessor variables)
2. Missing file and directory errors
3. Shell syntax compatibility (PowerShell vs CMD)
4. GitHub Actions workflow reliability improvements

After applying these fixes, the GitHub Actions workflow should build both the MSI installer and ZIP package successfully when triggered by a tag or manually.

## Common Issues

If you still encounter issues, check:

1. The WiX installer definition file (`installer\EasyKit.wxs`) for any path issues
2. Make sure all referenced files exist in your repository
3. Verify that the GitHub workflow file (`.github\workflows\build-and-upload.yml`) is properly formatted

For further assistance, please open an issue on the GitHub repository.
