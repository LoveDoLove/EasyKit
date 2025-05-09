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
echo.

REM Create a temporary directory for update check
set "TEMP_DIR=%TEMP%\EasyKit_Update"
if exist "%TEMP_DIR%" rmdir /S /Q "%TEMP_DIR%" >nul 2>&1
mkdir "%TEMP_DIR%" >nul 2>&1

REM Use PowerShell to fetch the latest release information from GitHub
powershell -Command "& {
    try {
        $releaseUrl = 'https://api.github.com/repos/LoveDoLove/EasyKit/releases/latest'
        $release = Invoke-RestMethod -Uri $releaseUrl -Headers @{'Accept'='application/vnd.github.v3+json'}
        $latest_version = $release.tag_name -replace 'v', ''
        $zip_url = ($release.assets | Where-Object { $_.name -like '*.zip' }).browser_download_url
        $installer_url = ($release.assets | Where-Object { $_.name -like '*Setup.exe' }).browser_download_url
        
        # Write info to temporary files
        $latest_version | Out-File -FilePath '%TEMP_DIR%\latest_version.txt'
        $zip_url | Out-File -FilePath '%TEMP_DIR%\zip_url.txt'
        $installer_url | Out-File -FilePath '%TEMP_DIR%\installer_url.txt'
        $release.body | Out-File -FilePath '%TEMP_DIR%\release_notes.txt'
        
        Write-Host 'Successfully retrieved release information.'
        exit 0
    } catch {
        Write-Host 'Failed to retrieve release information: ' + $_.Exception.Message
        exit 1
    }
}"

if %ERRORLEVEL% neq 0 (
    echo Failed to check for updates. Please make sure you have an internet connection.
    echo.
    pause
    goto Menu
)

REM Compare versions
if not exist "%TEMP_DIR%\latest_version.txt" (
    echo Failed to retrieve version information.
    echo.
    pause
    goto Menu
)

set /p LATEST_VERSION=<"%TEMP_DIR%\latest_version.txt"
set /p ZIP_URL=<"%TEMP_DIR%\zip_url.txt"
set /p INSTALLER_URL=<"%TEMP_DIR%\installer_url.txt"

echo.
echo Current version: %ESKIT_VERSION%
echo Latest version: %LATEST_VERSION%
echo.

REM Compare version numbers (this is a simple string comparison)
if "%ESKIT_VERSION%"=="%LATEST_VERSION%" (
    echo You have the latest version.
    echo.
    pause
    goto Menu
) else (
    echo A newer version is available!
    echo.
    echo Available update options:
    echo 1. Download and install automatically
    echo 2. Download installer and run manually
    echo 3. View release notes
    echo 4. Cancel
    echo.
    
    set update_choice=
    set /p update_choice=Choose an option: 
    
    if "%update_choice%"=="1" goto AutoUpdate
    if "%update_choice%"=="2" goto ManualUpdate
    if "%update_choice%"=="3" goto ShowReleaseNotes
    goto Menu
)

:AutoUpdate
cls
echo.
echo =====================================
echo  Automatic Update
echo =====================================
echo.
echo Creating backup first...
call :BackupScripts_Internal

echo.
echo Downloading update...
powershell -Command "& { Invoke-WebRequest -Uri '%ZIP_URL%' -OutFile '%TEMP_DIR%\EasyKit.zip' }"

if %ERRORLEVEL% neq 0 (
    echo Failed to download update.
    echo.
    pause
    goto Menu
)

echo.
echo Extracting update...
powershell -Command "& { Add-Type -Assembly 'System.IO.Compression.FileSystem'; [System.IO.Compression.ZipFile]::ExtractToDirectory('%TEMP_DIR%\EasyKit.zip', '%TEMP_DIR%\extracted') }"

if %ERRORLEVEL% neq 0 (
    echo Failed to extract update.
    echo.
    pause
    goto Menu
)

echo.
echo Installing update...
xcopy /E /Y "%TEMP_DIR%\extracted\*.bat" "%~dp0" >nul 2>&1
xcopy /E /Y "%TEMP_DIR%\extracted\*.md" "%~dp0" >nul 2>&1
xcopy /E /Y "%TEMP_DIR%\extracted\*.nsi" "%~dp0" >nul 2>&1
xcopy /E /Y "%TEMP_DIR%\extracted\images\*.*" "%~dp0images\" >nul 2>&1

echo.
echo Update completed successfully!
echo.
pause
goto Menu

:ManualUpdate
cls
echo.
echo =====================================
echo  Manual Update
echo =====================================
echo.
echo Downloading installer...
echo.
powershell -Command "& { Invoke-WebRequest -Uri '%INSTALLER_URL%' -OutFile '%USERPROFILE%\Desktop\EasyKit_Setup.exe' }"

if %ERRORLEVEL% neq 0 (
    echo Failed to download installer.
    echo.
    pause
    goto Menu
)

echo.
echo Installer downloaded to your desktop as 'EasyKit_Setup.exe'
echo Please run it to update EasyKit.
echo.
pause
goto Menu

:ShowReleaseNotes
cls
echo.
echo =====================================
echo  Release Notes
echo =====================================
echo.
echo Latest Version Release Notes:
echo.
type "%TEMP_DIR%\release_notes.txt"
echo.
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
call :BackupScripts_Internal
echo.
pause
goto Menu

:BackupScripts_Internal
echo Creating backup of EasyKit scripts...

set "BACKUP_FOLDER=%~dp0backups\backup_%DATE:~-4,4%-%DATE:~-7,2%-%DATE:~-10,2%_%TIME:~0,2%-%TIME:~3,2%-%TIME:~6,2%"
set "BACKUP_FOLDER=%BACKUP_FOLDER: =0%"

if not exist "%~dp0backups" (
    mkdir "%~dp0backups" >nul 2>&1
)

mkdir "%BACKUP_FOLDER%" >nul 2>&1
if %errorlevel% neq 0 (
    echo Failed to create backup folder.
    exit /b 1
)

echo Backing up batch files to %BACKUP_FOLDER%...
copy "%~dp0*.bat" "%BACKUP_FOLDER%" >nul 2>&1
if %errorlevel% neq 0 (
    echo Failed to backup files.
    exit /b 1
) else (
    echo Backup completed successfully!
    exit /b 0
)

:ViewReleaseNotes
cls
echo.
echo =====================================
echo  Release Notes
echo =====================================
echo.

if exist "%TEMP_DIR%\release_notes.txt" (
    echo Latest Version Release Notes:
    echo.
    type "%TEMP_DIR%\release_notes.txt"
) else (
    echo EasyKit Version %ESKIT_VERSION% Release Notes:
    echo.
    echo - Added configuration system for easier customization
    echo - Added common library with shared functions
    echo - Improved UI with better menu structure
    echo - Added settings menu for customization
    echo - Added update manager
    echo - Added logging capabilities
    echo - Added GitHub Actions for automated builds and releases
)

echo.
pause
goto Menu
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
