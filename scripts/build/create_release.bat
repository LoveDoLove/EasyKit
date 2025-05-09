@echo off
setlocal enabledelayedexpansion

echo =========================================
echo           EasyKit Release Tagger
echo =========================================
echo.
echo This script helps you tag a new release of EasyKit
echo for automatic package building via GitHub Actions.
echo.

REM Get the current version from config
for /f "tokens=2 delims==" %%a in ('type "%~dp0..\..\config\config_eskit.bat" ^| find "ESKIT_VERSION"') do (
    set "CURRENT_VERSION=%%a"
    set "CURRENT_VERSION=!CURRENT_VERSION:"=!"
)

echo Current version (from config): %CURRENT_VERSION%
echo.
echo Enter new version (e.g., 1.2.3) or press Enter to keep current:
set /p "NEW_VERSION="

if "%NEW_VERSION%"=="" set "NEW_VERSION=%CURRENT_VERSION%"

echo.
echo Creating release v%NEW_VERSION%...
echo.

REM Update version in config file if changed
if not "%NEW_VERSION%"=="%CURRENT_VERSION%" (
    echo Updating version in config file...
    powershell -Command "& { (Get-Content '%~dp0config_eskit.bat') -replace 'set \"ESKIT_VERSION=%CURRENT_VERSION%\"', 'set \"ESKIT_VERSION=%NEW_VERSION%\"' | Set-Content '%~dp0config_eskit.bat' }"
)

echo.
echo To complete the release process:
echo.
echo 1. Commit all changes:
echo    git add .
echo    git commit -m "Release v%NEW_VERSION%"
echo.
echo 2. Create tag:
echo    git tag v%NEW_VERSION%
echo.
echo 3. Push changes and tag:
echo    git push origin main
echo    git push origin v%NEW_VERSION%
echo.
echo GitHub Actions will automatically:
echo - Build the installers (ZIP and NSIS)
echo - Create a release with these assets
echo - Publish the release
echo.
echo    git push origin v%NEW_VERSION%
echo.
echo GitHub Actions will automatically:
echo - Build the installers
echo - Create a release with assets
echo - Publish the release
echo.
pause
