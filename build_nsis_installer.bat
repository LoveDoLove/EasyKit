@echo off
setlocal enabledelayedexpansion

:: EasyKit NSIS Installer Builder
:: Creates a professional installer for EasyKit

title EasyKit NSIS Installer Builder
color 0A

echo =========================================
echo      EasyKit NSIS Installer Builder
echo =========================================
echo.
echo This script will create a professional installer for EasyKit using NSIS.
echo.

:: Check if NSIS is installed
set "NSIS_FOUND=0"
if exist "%PROGRAMFILES(X86)%\NSIS\makensis.exe" (
    set "NSIS_PATH=%PROGRAMFILES(X86)%\NSIS\makensis.exe"
    set "NSIS_FOUND=1"
) else if exist "%PROGRAMFILES%\NSIS\makensis.exe" (
    set "NSIS_PATH=%PROGRAMFILES%\NSIS\makensis.exe"
    set "NSIS_FOUND=1"
)

if %NSIS_FOUND%==0 (
    echo NSIS (Nullsoft Scriptable Install System) is not found on your system.
    echo.
    echo Would you like to:
    echo 1. Download and install NSIS
    echo 2. Continue with the simple batch installer instead
    echo 3. Exit
    echo.
    set /p "NSIS_CHOICE=Enter your choice (1-3): "
    
    if "!NSIS_CHOICE!"=="1" (
        echo Opening the NSIS download page...
        start "" "https://nsis.sourceforge.io/Download"
        echo.
        echo Please install NSIS and run this script again.
        goto :cleanup
    ) else if "!NSIS_CHOICE!"=="2" (
        echo Building simple batch installer instead...
        call build_package.bat
        goto :cleanup
    ) else (
        echo Exiting...
        goto :cleanup
    )
)

:: Output directory
set "OUTPUT_DIR=%~dp0dist"
if not exist "%OUTPUT_DIR%" mkdir "%OUTPUT_DIR%"

:: Check for required NSIS image files
set "MISSING_FILES=0"
if not exist "images\icon.ico" (
    echo WARNING: images\icon.ico not found. The installer will use default icons.
    set "MISSING_FILES=1"
)

if not exist "images\installer-welcome.bmp" (
    echo Creating default welcome image...
    
    :: Create a simple welcome image using PowerShell
    powershell -Command "Add-Type -AssemblyName System.Drawing; $bmp = New-Object System.Drawing.Bitmap 164, 314; $g = [System.Drawing.Graphics]::FromImage($bmp); $g.Clear([System.Drawing.Color]::LightBlue); $font = New-Object System.Drawing.Font('Arial', 14); $brush = [System.Drawing.Brushes]::Black; $g.DrawString('EasyKit Installer', $font, $brush, 20, 150); $bmp.Save('images\installer-welcome.bmp', [System.Drawing.Imaging.ImageFormat]::Bmp); $g.Dispose(); $bmp.Dispose();"
)

if not exist "images\installer-header.bmp" (
    echo Creating default header image...
    
    :: Create a simple header image using PowerShell
    powershell -Command "Add-Type -AssemblyName System.Drawing; $bmp = New-Object System.Drawing.Bitmap 150, 57; $g = [System.Drawing.Graphics]::FromImage($bmp); $g.Clear([System.Drawing.Color]::LightGreen); $font = New-Object System.Drawing.Font('Arial', 10); $brush = [System.Drawing.Brushes]::Black; $g.DrawString('EasyKit', $font, $brush, 50, 20); $bmp.Save('images\installer-header.bmp', [System.Drawing.Imaging.ImageFormat]::Bmp); $g.Dispose(); $bmp.Dispose();"
)

:: Build the installer
echo Building NSIS installer...
"%NSIS_PATH%" "EasyKit.nsi"

if %errorlevel% neq 0 (
    echo Failed to build the installer. Check for errors.
    goto :cleanup
)

echo.
echo =========================================
echo      Installer Creation Completed!
echo =========================================
echo.
echo The installer has been created at:
echo %OUTPUT_DIR%\EasyKit_Setup.exe
echo.
echo You can distribute this executable to install EasyKit on any Windows system.
echo.
pause

:cleanup
endlocal
