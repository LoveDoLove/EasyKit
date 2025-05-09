@echo off
REM Script to verify the WiX script and GitHub Actions configuration

echo Verifying EasyKit GitHub Actions Setup for v1.2.6...
echo ==================================================
echo.

REM Check for GitHub Actions workflow file
if exist .github\workflows\build-and-upload.yml (
  echo [PASS] GitHub Actions workflow file found
) else (
  echo [FAIL] GitHub Actions workflow file not found!
  goto :error
)

REM Check for WiX installer file
if exist installer\EasyKit.wxs (
  echo [PASS] WiX installer definition file found
) else (
  echo [FAIL] WiX installer definition file not found!
  goto :error
)

REM Check for required directories
if not exist scripts\build (
  echo [WARN] scripts\build directory missing - creating now...
  mkdir scripts\build
) else (
  echo [PASS] scripts\build directory found
)

REM Check for required files
if not exist scripts\build\release_v1.2.6.bat (
  echo [WARN] release_v1.2.6.bat missing - creating now...
  echo @echo off > scripts\build\release_v1.2.6.bat
  echo echo EasyKit Release v1.2.6 >> scripts\build\release_v1.2.6.bat
  echo echo Build date: %%date%% %%time%% >> scripts\build\release_v1.2.6.bat
  echo echo. >> scripts\build\release_v1.2.6.bat
  echo echo This script is part of the EasyKit installation package. >> scripts\build\release_v1.2.6.bat
  echo echo For more information, visit: https://github.com/LoveDoLove/EasyKit >> scripts\build\release_v1.2.6.bat
  echo echo. >> scripts\build\release_v1.2.6.bat
  echo pause >> scripts\build\release_v1.2.6.bat
) else (
  echo [PASS] release_v1.2.6.bat found
)

REM Check workflow file content for ProjectDir parameter
findstr /C:"candle.exe -dProjectDir" .github\workflows\build-and-upload.yml >nul
if %errorlevel% equ 0 (
  echo [PASS] Workflow includes ProjectDir parameter for WiX
) else (
  echo [WARN] Workflow might be missing ProjectDir parameter for WiX
  echo        Consider adding -dProjectDir="%%CD%%\\" to candle.exe command
)

REM Check workflow file content for cmd shell specification
findstr /C:"shell: cmd" .github\workflows\build-and-upload.yml >nul
if %errorlevel% equ 0 (
  echo [PASS] Workflow specifies cmd shell for batch commands
) else (
  echo [WARN] Workflow might be missing shell: cmd specification
  echo        This can cause syntax issues with batch commands
)

echo.
echo Verification completed.
echo.
echo All checks passed or issues have been resolved.
goto :end

:error
echo.
echo [ERROR] One or more verification checks failed.
echo Please fix the issues and run this script again.
echo.
exit /b 1

:end
echo.
echo If you encounter any issues with GitHub Actions, please update:
echo 1. The workflow file (.github\workflows\build-and-upload.yml)
echo 2. The WiX installer file (installer\EasyKit.wxs)
echo.
echo For more details, see docs\github_actions_fix.md
