@echo off
setlocal enabledelayedexpansion

:: EasyKit Package Builder
:: Creates a distributable package of EasyKit

title EasyKit Package Builder
color 0A

echo =========================================
echo          EasyKit Package Builder
echo =========================================
echo.
echo This script will create a distributable package of EasyKit.
echo.

:: Define output directory
set "OUTPUT_DIR=%~dp0dist"
if not exist "%OUTPUT_DIR%" mkdir "%OUTPUT_DIR%"

:: Define package name with date
for /f "tokens=2-4 delims=/ " %%a in ('date /t') do (
    set "DATE=%%c-%%a-%%b"
)
set "PACKAGE_NAME=EasyKit_Setup_%DATE%.zip"

:: Check if 7-Zip is installed
where 7z.exe >nul 2>&1
if %errorlevel% neq 0 (
    echo 7-Zip is not found. Using built-in zip command...
    set USE_POWERSHELL=1
) else (
    set USE_POWERSHELL=0
)

:: Files to include
echo Creating file list...
set "FILES_TO_INCLUDE=*.bat LICENSE README.md images\*.* .gitignore"
echo - Including main batch files
echo - Including documentation
echo - Including images
echo - Excluding logs directory
echo - Excluding backup files

:: Create the package
echo.
echo Creating package %PACKAGE_NAME%...

if %USE_POWERSHELL%==1 (
    :: Use PowerShell for compression
    powershell -Command "Add-Type -Assembly 'System.IO.Compression.FileSystem'; [System.IO.Compression.ZipFile]::CreateFromDirectory('%~dp0', '%OUTPUT_DIR%\%PACKAGE_NAME%', [System.IO.Compression.CompressionLevel]::Optimal, $false)"
) else (
    :: Use 7-Zip if available
    7z a -tzip "%OUTPUT_DIR%\%PACKAGE_NAME%" %FILES_TO_INCLUDE% -xr!*.bak -xr!logs -xr!backups -xr!dist
)

if %errorlevel% neq 0 (
    echo Failed to create package.
    goto :cleanup
)

:: Create a simple launcher
echo Creating launcher...
set "LAUNCHER=%OUTPUT_DIR%\EasyKit_Setup.bat"

(
    echo @echo off
    echo setlocal
    echo.
    echo echo Extracting EasyKit...
    echo.
    echo :: Create temp directory
    echo set "TEMP_DIR=%%TEMP%%\EasyKit_Setup"
    echo if exist "%%TEMP_DIR%%" rmdir /S /Q "%%TEMP_DIR%%"
    echo mkdir "%%TEMP_DIR%%"
    echo.
    echo :: Extract the zip file
    echo powershell -Command "Expand-Archive -Path '%PACKAGE_NAME%' -DestinationPath '%%TEMP_DIR%%' -Force"
    echo.
    echo :: Run the installer
    echo start "" "%%TEMP_DIR%%\install_eskit.bat"
    echo.
    echo exit /b
) > "%LAUNCHER%"

echo.
echo =========================================
echo       Package Creation Completed!
echo =========================================
echo.
echo Package has been created at:
echo %OUTPUT_DIR%\%PACKAGE_NAME%
echo.
echo Launcher has been created at:
echo %OUTPUT_DIR%\EasyKit_Setup.bat
echo.
echo To distribute EasyKit, share the following files:
echo 1. %PACKAGE_NAME%
echo 2. EasyKit_Setup.bat
echo.
echo Thank you for using EasyKit Package Builder!
echo.
pause

:cleanup
endlocal
