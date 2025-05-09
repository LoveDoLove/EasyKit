@echo off
echo EasyKit GitHub Actions Workflow Fix v1.2.6
echo ================================================
echo This script will redirect to fix_wix_workflow.bat
echo.

echo Running fix_wix_workflow.bat...
call scripts\github\fix_wix_workflow.bat

echo.
echo Fix completed successfully.
echo Your GitHub Actions workflow should now be compatible with the WiX-based installer.
echo.
pause
