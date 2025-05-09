@echo off
setlocal enabledelayedexpansion

REM Load configuration
call "%~dp0..\..\config\config_eskit.bat"

color %ESKIT_COLOR%
title %ESKIT_TITLE_PREFIX% - NPM Menu

REM Log the startup
if "%ESKIT_ENABLE_LOGGING%"=="true" (
    echo %DATE% %TIME% - Opened NPM Menu >> "%ESKIT_LOG_PATH%\eskit.log"
)

:Menu
cls
echo.
echo =====================================
echo  NPM Menu
echo =====================================
echo.
echo 0. Back to Main Menu
echo 1. Install Npm Packages
echo 2. Update Npm Packages
echo 3. Build For Production
echo 4. Build For Development
echo 5. Run Security Audit
echo 6. Run Custom Script
echo 7. Show Package Info
echo.
echo Dangerous:
echo 9. Reset All Cache
echo.
echo H. Help
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
if "%choice%"=="1" goto InstallNpmPackages
if "%choice%"=="2" goto UpdateNpmPackages
if "%choice%"=="3" goto BuildToProduction
if "%choice%"=="4" goto BuildToDevelopment
if "%choice%"=="5" goto RunSecurityAudit
if "%choice%"=="6" goto RunCustomScript
if "%choice%"=="7" goto ShowPackageInfo
if "%choice%"=="9" goto ResetAllCache
if /i "%choice%"=="H" goto Help
if /i "%choice%"=="h" goto Help
echo Invalid option. Please try again.
timeout /t 2 >nul
goto Menu

:InstallNpmPackages
echo.
echo Installing npm packages...
call :CheckSoftwareMethod npm
if %errorlevel% neq 0 goto OperationFailed
echo.
echo This may take a while depending on the number of packages...
call npm install --no-fund --loglevel=error
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
if %errorlevel% neq 0 (
    echo npm-check-updates (ncu) is not installed.
    echo Installing npm-check-updates globally...
    call npm install -g npm-check-updates
    if %errorlevel% neq 0 goto OperationFailed
)
echo.
echo Checking for updates (this may take a moment)...
call ncu -u
if %errorlevel% neq 0 goto OperationFailed
echo.
echo Installing updated packages...
call npm install --no-fund --loglevel=error
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
echo.
echo This may take a while...
call npm run build
if %errorlevel% neq 0 goto OperationFailed
echo.
echo Build completed successfully!
pause
goto Menu

:BuildToDevelopment
echo.
echo Building for development...
call :CheckSoftwareMethod npm
if %errorlevel% neq 0 goto OperationFailed
echo.
echo Starting development build...
call npm run dev
if %errorlevel% neq 0 goto OperationFailed
echo.
echo Development build running!
pause
goto Menu

:RunSecurityAudit
echo.
echo Running security audit...
call :CheckSoftwareMethod npm
if %errorlevel% neq 0 goto OperationFailed
echo.
echo Checking for vulnerabilities...
call npm audit
set audit_status=%errorlevel%
if %audit_status% neq 0 (
    echo.
    echo Security vulnerabilities found!
    echo Would you like to fix them?
    choice /c YN /m "Fix vulnerabilities? (Y/N)"
    if !errorlevel! equ 1 (
        echo.
        echo Attempting to fix vulnerabilities...
        call npm audit fix
        echo.
        echo Attempted to fix vulnerabilities. Check results above.
    )
) else (
    echo.
    echo No security vulnerabilities found!
)
pause
goto Menu

:RunCustomScript
echo.
echo Available npm scripts:
call :CheckSoftwareMethod npm
if %errorlevel% neq 0 goto OperationFailed
echo.
echo Fetching available scripts...
call npm run --silent
echo.
set /p script=Enter script name to run (or leave empty to cancel): 
if "%script%"=="" goto Menu
echo.
echo Running npm script: %script%
call npm run %script%
set run_status=%errorlevel%
if %run_status% neq 0 (
    echo.
    echo Script "%script%" exited with error code %run_status%
) else (
    echo.
    echo Script "%script%" completed successfully
)
pause
goto Menu

:ShowPackageInfo
echo.
echo Package information:
call :CheckSoftwareMethod npm
if %errorlevel% neq 0 goto OperationFailed
echo.
echo Local packages:
call npm list --depth=0
echo.
echo Global packages:
call npm list -g --depth=0 2>nul
if %errorlevel% neq 0 echo Unable to retrieve global packages. You may need to run as administrator.
echo.
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
    echo.
    echo Clearing npm cache...
    call npm cache clean --force
    if %errorlevel% neq 0 goto OperationFailed
    echo.
    echo Npm cache cleared successfully!
    pause
)
goto Menu

:Help
echo.
echo ================================
echo  Npm Menu Help
echo ================================
echo.
echo 0. Back to Main Menu - Return to the main EasyKit menu
echo 1. Install Npm Packages - Run 'npm install' to install dependencies
echo 2. Update Npm Packages - Update dependencies using npm-check-updates
echo 3. Build For Production - Run 'npm run build' for production build
echo 4. Build For Development - Run 'npm run dev' for development
echo 5. Run Security Audit - Check for vulnerabilities with 'npm audit'
echo 6. Run Custom Script - Run any script defined in package.json
echo 7. Show Package Info - List installed packages and versions
echo 9. Reset All Cache - Clear npm cache (use with caution)
echo.
pause
goto Menu

:OperationFailed
echo.
echo Operation failed with errors!
if "%ESKIT_ENABLE_LOGGING%"=="true" (
    echo %DATE% %TIME% - Operation failed with error code: %errorlevel% >> "%ESKIT_LOG_PATH%\eskit.log"
)
echo Error code: %errorlevel%
pause
goto Menu

@REM Method
:CheckSoftwareMethod
if "%ESKIT_ENABLE_LOGGING%"=="true" (
    echo %DATE% %TIME% - Checking for software: %1 >> "%ESKIT_LOG_PATH%\eskit.log"
)
call "%~dp0check_software_eskit.bat" %1
exit /b %errorlevel%

:Exit
if "%ESKIT_ENABLE_LOGGING%"=="true" (
    echo %DATE% %TIME% - Exiting NPM Menu >> "%ESKIT_LOG_PATH%\eskit.log"
)
endlocal
exit /b 0
