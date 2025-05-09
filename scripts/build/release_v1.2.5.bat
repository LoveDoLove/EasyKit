@echo off
echo =============================================
echo  EasyKit v1.2.5 Release Preparation Script
echo =============================================
echo.
echo This script will prepare the v1.2.5 release by:
echo  1. Verifying all version numbers are updated
echo  2. Creating a git tag for the release
echo  3. Providing instructions to push the tag
echo.
echo Press Enter to continue or Ctrl+C to cancel.
pause > nul

cls
echo Checking version numbers...
echo.

set errors=0

REM Check config file
findstr /c:"ESKIT_VERSION=1.2.5" "%~dp0..\..\config\config_eskit.bat" > nul
if errorlevel 1 (
    echo [ERROR] Version in config_eskit.bat is not 1.2.5
    set /a errors+=1
) else (
    echo [OK] config_eskit.bat has correct version
)

REM Check README.md
findstr /c:"version-1.2.5-blue.svg" "%~dp0..\..\README.md" > nul
if errorlevel 1 (
    echo [ERROR] Version in README.md badge is not 1.2.5
    set /a errors+=1
) else (
    echo [OK] README.md badge has correct version
)

REM Check installer script
findstr /c:"Name \"EasyKit v1.2.5\"" "%~dp0..\..\installer\EasyKit.nsi" > nul
if errorlevel 1 (
    echo [ERROR] Version in EasyKit.nsi is not 1.2.5
    set /a errors+=1
) else (
    echo [OK] EasyKit.nsi has correct version
)

REM Check migration guide
findstr /c:"### v1.2.5" "%~dp0..\..\MIGRATION_GUIDE.md" > nul
if errorlevel 1 (
    echo [ERROR] Version v1.2.5 not found in MIGRATION_GUIDE.md
    set /a errors+=1
) else (
    echo [OK] MIGRATION_GUIDE.md has v1.2.5 section
)

REM Check release notes exist
if not exist "%~dp0..\..\docs\release_notes_v1.2.5.md" (
    echo [ERROR] Release notes file docs\release_notes_v1.2.5.md not found
    set /a errors+=1
) else (
    echo [OK] Release notes file exists
)

echo.
if %errors% gtr 0 (
    echo Found %errors% errors. Please fix them before creating the release.
    echo.
    pause
    exit /b 1
)

echo All version references verified successfully!
echo.

echo Creating git tag for v1.2.5...
git tag -a v1.2.5 -m "Release v1.2.5 - Fixed GitHub Actions workflow after project restructuring"
if errorlevel 1 (
    echo Failed to create git tag. Please check git setup.
    pause
    exit /b 1
)

echo.
echo Tag created successfully!
echo.
echo To complete the release process:
echo.
echo 1. Push the tag to GitHub:
echo    git push origin v1.2.5
echo.
echo 2. The GitHub Actions workflow will automatically build and create a release
echo.
echo 3. Verify the workflow completes successfully in the GitHub Actions tab
echo.
pause
