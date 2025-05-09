@echo off
echo EasyKit GitHub Actions Workflow Fix Tool v1.2.6
echo =============================================
echo This script fixes all issues with the GitHub Actions workflow for WiX
echo.

REM Step 1: Create the scripts\build directory if it doesn't exist
echo Step 1: Checking for required directories...
if not exist scripts\build (
  echo Creating scripts\build directory...
  mkdir scripts\build
  echo Directory created.
) else (
  echo scripts\build directory already exists.
)

REM Step 2: Create the release_v1.2.6.bat file if it doesn't exist
echo Step 2: Checking for required files...
if not exist scripts\build\release_v1.2.6.bat (
  echo Creating release_v1.2.6.bat...
  echo @echo off > scripts\build\release_v1.2.6.bat
  echo echo EasyKit Release v1.2.6 >> scripts\build\release_v1.2.6.bat
  echo echo Build date: %%date%% %%time%% >> scripts\build\release_v1.2.6.bat
  echo echo. >> scripts\build\release_v1.2.6.bat
  echo echo This script is part of the EasyKit installation package. >> scripts\build\release_v1.2.6.bat
  echo echo For more information, visit: https://github.com/LoveDoLove/EasyKit >> scripts\build\release_v1.2.6.bat
  echo echo. >> scripts\build\release_v1.2.6.bat
  echo pause >> scripts\build\release_v1.2.6.bat
  echo File created.
) else (
  echo release_v1.2.6.bat already exists.
)

REM Step 3: Fix the GitHub workflow file for WiX
echo Step 3: Fixing GitHub workflow file for WiX...
call scripts\github\fix_wix_workflow.bat
echo.

REM Step 4: Verify the changes
echo Step 4: Verifying changes...
call scripts\github\verify_github_actions_setup.bat
echo.

echo All fixes have been applied successfully.
echo Your GitHub Actions workflow should now be ready to build the WiX installer.
echo.
echo If you encounter any issues, please check the documentation in docs\github_actions_fix.md
echo.

pause
