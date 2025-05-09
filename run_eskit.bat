@echo off
setlocal enabledelayedexpansion

REM Load configuration
call "%~dp0config_eskit.bat"

color %ESKIT_COLOR%
title %ESKIT_TITLE_PREFIX% - Main Menu

REM Log the startup
if "%ESKIT_ENABLE_LOGGING%"=="true" (
    echo %DATE% %TIME% - Started EasyKit v%ESKIT_VERSION% >> "%ESKIT_LOG_PATH%\eskit.log"
)

if "%ESKIT_CHECK_UPDATES%"=="true" (
    echo Checking for updates...
    REM Future implementation: Check online for new version
)

:Menu
cls
echo.
echo =====================================
echo  EasyKit Main Menu v%ESKIT_VERSION%
echo =====================================
echo.

echo  0. Exit
echo  1. Npm Menu
echo  2. Laravel Menu
echo  3. Composer Menu
echo  4. Git Menu
echo  5. Create Shortcuts
echo  6. Settings
echo  7. Update Manager
echo  8. Build and Release
echo.

if "%ESKIT_SHOW_TIPS%"=="true" (
    echo TIP: You can customize EasyKit by editing the config_eskit.bat file.
    echo.
)

set choice=
set /p choice=Choose an option: 
if not defined choice (
    echo.
    echo Invalid option. Please try again.
    timeout /t 2 >nul
    goto Menu
)

if "%choice%"=="0" goto Exit
if "%choice%"=="1" call "%~dp0npm_eskit.bat"
if "%choice%"=="2" call "%~dp0laravel_eskit.bat"
if "%choice%"=="3" call "%~dp0composer_eskit.bat"
if "%choice%"=="4" call "%~dp0git_eskit.bat"
if "%choice%"=="5" call "%~dp0create_shortcuts_eskit.bat"
if "%choice%"=="6" goto Settings
if "%choice%"=="7" call "%~dp0update_eskit.bat"
if "%choice%"=="8" goto BuildReleaseMenu
goto Menu

:Settings
cls
echo.
echo =====================================
echo  EasyKit Settings
echo =====================================
echo.

echo  0. Back to Main Menu
echo  1. Toggle Tips (Current: %ESKIT_SHOW_TIPS%)
echo  2. Toggle Logging (Current: %ESKIT_ENABLE_LOGGING%)
echo  3. Toggle Update Checking (Current: %ESKIT_CHECK_UPDATES%)
echo  4. Change Display Color (Current: %ESKIT_COLOR%)
echo  5. View Logs
echo.

set choice=
set /p choice=Choose an option: 
if not defined choice (
    echo.
    echo Invalid option. Please try again.
    timeout /t 2 >nul
    goto Settings
)

if "%choice%"=="0" goto Menu
if "%choice%"=="1" goto ToggleTips
if "%choice%"=="2" goto ToggleLogging
if "%choice%"=="3" goto ToggleUpdateChecking
if "%choice%"=="4" goto ChangeColor
if "%choice%"=="5" start "" "%ESKIT_LOG_PATH%\eskit.log"
goto Settings

:ToggleTips
if "%ESKIT_SHOW_TIPS%"=="true" (
    set "ESKIT_SHOW_TIPS=false"
) else (
    set "ESKIT_SHOW_TIPS=true"
)
REM Future implementation: Save settings to config file
goto Settings

:ToggleLogging
if "%ESKIT_ENABLE_LOGGING%"=="true" (
    set "ESKIT_ENABLE_LOGGING=false"
) else (
    set "ESKIT_ENABLE_LOGGING=true"
)
REM Future implementation: Save settings to config file
goto Settings

:ToggleUpdateChecking
if "%ESKIT_CHECK_UPDATES%"=="true" (
    set "ESKIT_CHECK_UPDATES=false"
) else (
    set "ESKIT_CHECK_UPDATES=true"
)
REM Future implementation: Save settings to config file
goto Settings

:ChangeColor
cls
echo.
echo =====================================
echo  Change Display Color
echo =====================================
echo.
echo Available colors:
echo   0 = Black      8 = Gray
echo   1 = Blue       9 = Light Blue
echo   2 = Green      A = Light Green
echo   3 = Aqua       B = Light Aqua
echo   4 = Red        C = Light Red
echo   5 = Purple     D = Light Purple
echo   6 = Yellow     E = Light Yellow
echo   7 = White      F = Bright White
echo.
echo Current background color: %ESKIT_COLOR:~0,1%
echo Current text color: %ESKIT_COLOR:~1,1%
echo.
set /p bg=Enter background color (0-9, A-F): 
set /p fg=Enter text color (0-9, A-F): 
set "ESKIT_COLOR=%bg%%fg%"
color %ESKIT_COLOR%
REM Future implementation: Save settings to config file
goto Settings

:Exit
if "%ESKIT_ENABLE_LOGGING%"=="true" (
    echo %DATE% %TIME% - Exiting EasyKit >> "%ESKIT_LOG_PATH%\eskit.log"
)

if "%ESKIT_CONFIRM_EXIT%"=="true" (
    choice /c YN /m "Are you sure you want to exit? (Y/N)"
    if errorlevel 2 goto Menu
)

echo Exiting EasyKit...
exit /b 0

:BuildReleaseMenu
cls
echo.
echo =====================================
echo  EasyKit Build and Release
echo =====================================
echo.

echo  0. Back to Main Menu
echo  1. Build Package (ZIP)
echo  2. Build NSIS Installer
echo  3. Create New Release
echo  4. View GitHub Actions Guide
echo.

set choice=
set /p choice=Choose an option: 
if not defined choice (
    echo.
    echo Invalid option. Please try again.
    timeout /t 2 >nul
    goto BuildReleaseMenu
)

if "%choice%"=="0" goto Menu
if "%choice%"=="1" call "%~dp0build_package.bat"
if "%choice%"=="2" call "%~dp0build_nsis_installer.bat"
if "%choice%"=="3" call "%~dp0create_release.bat"
if "%choice%"=="4" goto GitHubActionsGuide
goto BuildReleaseMenu

:GitHubActionsGuide
cls
echo.
echo =====================================
echo  GitHub Actions Guide
echo =====================================
echo.
echo EasyKit uses GitHub Actions for automated builds and releases.
echo.
echo Workflow File:
echo - .github\workflows\build-and-release.yml  : Builds packages and creates releases
echo.
echo How to create a release:
echo 1. Commit all your changes
echo 2. Tag the release with: git tag v1.x.x
echo 3. Push the tag with: git push origin v1.x.x
echo.
echo GitHub will automatically:
echo - Build the installers (ZIP and NSIS)
echo - Create a release with those assets
echo - Publish the release on GitHub
echo.
echo To perform these steps easily, use Option 3 "Create New Release"
echo from the Build and Release menu.
echo.
pause
goto BuildReleaseMenu
