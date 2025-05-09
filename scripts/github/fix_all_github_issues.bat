@echo off
echo EasyKit GitHub Actions Workflow Fix Tool
echo =======================================
echo This script fixes all issues with the GitHub Actions workflow
echo.

REM Step 1: Fix the GitHub workflow file
echo Step 1: Fixing GitHub workflow file...
powershell -ExecutionPolicy Bypass -File scripts\github\fix_github_workflow.ps1
echo.

REM Step 2: Fix the NSIS installer script
echo Step 2: Fixing NSIS installer script...
call scripts\github\fix_installer_paths.bat
echo.

REM Step 3: Verify the changes
echo Step 3: Verifying changes...
echo Checking if workflow file exists...
if exist .github\workflows\build-and-upload.yml (
  echo [OK] GitHub workflow file exists
) else (
  echo [ERROR] GitHub workflow file not found
)

echo Checking if NSIS script exists...
if exist installer\EasyKit.nsi (
  echo [OK] NSIS installer script exists
) else (
  echo [ERROR] NSIS installer script not found
)

echo.
echo All fixes have been applied.
echo.
echo To test these changes:
echo 1. Commit and push these changes to your repository
echo 2. Go to GitHub Actions tab in your repository
echo 3. Run the workflow manually using the "Run workflow" button
echo.
echo If you encounter any issues, check the logs in GitHub Actions
echo to see what went wrong.
