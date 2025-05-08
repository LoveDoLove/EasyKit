@echo off
color 0A
title Npm Menu

:Menu
cls
echo.
echo ================================
echo  Npm Menu
echo ================================
echo.
echo 0. Back to Main Menu
echo 1. Install Npm Packages
echo 2. Update Npm Packages
echo 3. Build To Production
echo.
echo Dangerous:
echo 44. Reset All Cache
echo.
set /p choice=Choose an option: 
if %choice%==0 goto Exit
if %choice%==1 goto InstallNpmPackages
if %choice%==2 goto UpdateNpmPackages
if %choice%==3 goto BuildToProduction
if %choice%==44 goto ResetAllCache
goto Menu

:InstallNpmPackages
echo.
echo Installing npm packages...
call :CheckSoftwareMethod npm
if %errorlevel% neq 0 goto OperationFailed
call npm install
if %errorlevel% neq 0 goto OperationFailed
echo.
echo Npm packages installed successfully!
pause
goto Menu

:UpdateNpmPackages
echo.
echo Updating npm packages...
call :CheckSoftwareMethod npm
if %errorlevel% neq 0 goto OperationFailed
call :CheckSoftwareMethod ncu
if %errorlevel% neq 0 goto OperationFailed
call ncu -u
if %errorlevel% neq 0 goto OperationFailed
call npm install
if %errorlevel% neq 0 goto OperationFailed
echo.
echo Npm packages updated successfully!
pause
goto Menu

:BuildToProduction
echo.
echo Building for production...
call :CheckSoftwareMethod npm
if %errorlevel% neq 0 goto OperationFailed
call npm run build
if %errorlevel% neq 0 goto OperationFailed
echo.
echo Build completed successfully!
pause
goto Menu

@REM Dangerous
:ResetAllCache
echo.
echo WARNING: This will clear all npm cache!
choice /c YN /m "Are you sure you want to proceed? (Y/N)"
if %errorlevel% equ 2 goto Menu
if %errorlevel% equ 1 (
    call :CheckSoftwareMethod npm
    if %errorlevel% neq 0 goto OperationFailed
    call npm cache clean --force
    if %errorlevel% neq 0 goto OperationFailed
    echo.
    echo Npm cache cleared successfully!
    pause
)
goto Menu

:OperationFailed
echo.
echo Operation failed with errors!
pause
goto Menu

@REM Method
:CheckSoftwareMethod
call check_software_eskit.bat %1
exit /b

:Exit
exit /b 0
