@echo off
setlocal enabledelayedexpansion

REM Load configuration
call "%~dp0config_eskit.bat"

color %ESKIT_COLOR%
title %ESKIT_TITLE_PREFIX% - Update Manager

:Menu
cls
echo.
echo =====================================
echo  EasyKit Update Manager
echo =====================================
echo.

echo  0. Back to Main Menu
echo  1. Check for Updates
echo  2. Backup Current Scripts
echo  3. View Release Notes
echo.

set choice=
set /p choice=Choose an option: 
if not defined choice (
    echo.
    echo Invalid option. Please try again.
    timeout /t 2 >nul
    goto Menu
)

if "%choice%"=="0" goto Exit
if "%choice%"=="1" goto CheckForUpdates
if "%choice%"=="2" goto BackupScripts
if "%choice%"=="3" goto ViewReleaseNotes
goto Menu

:CheckForUpdates
cls
echo.
echo =====================================
echo  Check for Updates
echo =====================================
echo.
echo Current version: %ESKIT_VERSION%
echo.
echo Checking for updates...

REM Future implementation: Check online for updates
REM For now, just simulate the check
timeout /t 2 >nul

echo.
echo No updates found. You have the latest version.
echo.
pause
goto Menu

REM Future implementation: Check online for updates
REM For now, just simulate the check
timeout /t 2 >nul

echo.
echo No updates found. You have the latest version.
echo.
pause
goto Menu

:BackupScripts
cls
echo.
echo =====================================
echo  Backup Scripts
echo =====================================
echo.
echo Creating backup of EasyKit scripts...

set "BACKUP_FOLDER=%~dp0backups\backup_%DATE:~-4,4%-%DATE:~-7,2%-%DATE:~-10,2%_%TIME:~0,2%-%TIME:~3,2%-%TIME:~6,2%"
set "BACKUP_FOLDER=%BACKUP_FOLDER: =0%"

if not exist "%~dp0backups" (
    mkdir "%~dp0backups" >nul 2>&1
)

mkdir "%BACKUP_FOLDER%" >nul 2>&1
if %errorlevel% neq 0 (
    echo Failed to create backup folder.
    pause
    goto Menu
)

echo Backing up batch files to %BACKUP_FOLDER%...
copy "%~dp0*.bat" "%BACKUP_FOLDER%" >nul 2>&1
if %errorlevel% neq 0 (
    echo Failed to backup files.
) else (
    echo Backup completed successfully!
)

pause
goto Menu

:ViewReleaseNotes
cls
echo.
echo =====================================
echo  Release Notes
echo =====================================
echo.
echo EasyKit Version %ESKIT_VERSION% Release Notes:
echo.
echo - Added configuration system for easier customization
echo - Added common library with shared functions
echo - Improved UI with better menu structure
echo - Added settings menu for customization
echo - Added update manager
echo - Added logging capabilities
echo - Added more intuitive error handling
echo.
echo Visit %ESKIT_UPDATE_URL% for the latest updates and release notes.
echo.
pause
goto Menu

:Exit
exit /b 0
echo - Added more intuitive error handling
echo.
echo Visit %ESKIT_UPDATE_URL% for the latest updates and release notes.
echo.
pause
goto Menu

:Exit
exit /b 0
