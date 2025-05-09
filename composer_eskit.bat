@echo off
setlocal enabledelayedexpansion

REM Load configuration
call "%~dp0config_eskit.bat"

color %ESKIT_COLOR%
title %ESKIT_TITLE_PREFIX% - Composer Menu

REM Log the startup
if "%ESKIT_ENABLE_LOGGING%"=="true" (
    echo %DATE% %TIME% - Opened Composer Menu >> "%ESKIT_LOG_PATH%\eskit.log"
)

:Menu
cls
echo.
echo =====================================
echo  Composer Menu
echo =====================================
echo.
echo.
echo 0. Back to Main Menu
echo 1. Install Packages
echo 2. Update Packages
echo 3. Regenerate AutoLoad Files
echo 4. Require New Package
echo 5. Create New Project
echo 6. Validate composer.json
echo.
echo Dangerous:
echo 44. Clear Composer Cache
echo.
set choice=
set /p choice=Choose an option: 
if not defined choice goto InvalidChoice
set "numbers=0123456789"
set "valid=true"
for /L %%i in (0,1,9) do if "!choice:~%%i,1!" NEQ "" (
    if "!numbers:!choice:~%%i,1!=!" EQU "%numbers%" set "valid=false"
)
if "!valid!" EQU "false" goto InvalidChoice
if %choice% LSS 0 goto InvalidChoice
if %choice% GTR 6 (
    if %choice% NEQ 44 goto InvalidChoice
)

if %choice%==0 goto Exit
if %choice%==1 goto Install
if %choice%==2 goto UpdatePackages
if %choice%==3 goto RegenerateAutoload
if %choice%==4 goto RequirePackage
if %choice%==5 goto CreateProject
if %choice%==6 goto ValidateJson
if %choice%==44 goto ClearCache
goto Menu

:InvalidChoice
echo.
echo Invalid option. Please try again.
timeout /t 2 >nul
goto Menu

:Install
echo.
echo Installing Composer packages...
call :CheckSoftwareMethod composer
if %errorlevel% neq 0 goto OperationFailed
call composer install
if %errorlevel% neq 0 goto OperationFailed
echo.
echo Composer packages installed successfully!
pause
goto Menu

:UpdatePackages
echo.
echo Updating Composer packages...
call :CheckSoftwareMethod composer
if %errorlevel% neq 0 goto OperationFailed
call composer update
if %errorlevel% neq 0 goto OperationFailed
echo.
echo Composer packages updated successfully!
pause
goto Menu

:RegenerateAutoload
echo.
echo Regenerating Composer autoload files...
call :CheckSoftwareMethod composer
if %errorlevel% neq 0 goto OperationFailed
call composer dump-autoload
if %errorlevel% neq 0 goto OperationFailed
echo.
echo Composer autoload files regenerated successfully!
pause
goto Menu

:RequirePackage
echo.
echo Require a new package...
call :CheckSoftwareMethod composer
if %errorlevel% neq 0 goto OperationFailed
set /p package=Enter package name (e.g. vendor/package): 
if "%package%"=="" (
    echo Package name cannot be empty!
    pause
    goto Menu
)
choice /c YN /m "Add as dev dependency? (Y/N)"
if !errorlevel! equ 1 (
    call composer require --dev %package%
) else (
    call composer require %package%
)
if %errorlevel% neq 0 goto OperationFailed
echo.
echo Package '%package%' added successfully!
pause
goto Menu

:CreateProject
echo.
echo Create a new Composer project...
call :CheckSoftwareMethod composer
if %errorlevel% neq 0 goto OperationFailed
set /p package=Enter package template (e.g. laravel/laravel): 
if "%package%"=="" (
    echo Package template cannot be empty!
    pause
    goto Menu
)
set /p directory=Enter directory name (leave empty for current directory): 
if "%directory%"=="" (
    call composer create-project %package% .
) else (
    call composer create-project %package% %directory%
)
if %errorlevel% neq 0 goto OperationFailed
echo.
echo Project created successfully!
pause
goto Menu

:ValidateJson
echo.
echo Validating composer.json file...
call :CheckSoftwareMethod composer
if %errorlevel% neq 0 goto OperationFailed
call composer validate
if %errorlevel% neq 0 (
    echo.
    echo Validation failed! There are issues with your composer.json file.
    pause
    goto Menu
)
echo.
echo composer.json is valid!
pause
goto Menu

:ClearCache
echo.
echo WARNING: This will clear all Composer cache!
choice /c YN /m "Are you sure you want to proceed? (Y/N)"
if %errorlevel% equ 2 goto Menu
if %errorlevel% equ 1 (
    call :CheckSoftwareMethod composer
    if %errorlevel% neq 0 goto OperationFailed
    call composer clear-cache
    if %errorlevel% neq 0 goto OperationFailed
    echo.
    echo Composer cache cleared successfully!
    pause
)
goto Menu

:OperationFailed
echo.
echo Operation failed with errors!
if "%ESKIT_ENABLE_LOGGING%"=="true" (
    echo %DATE% %TIME% - Operation failed with error code: %errorlevel% >> "%ESKIT_LOG_PATH%\eskit.log"
)
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
    echo %DATE% %TIME% - Exiting Composer Menu >> "%ESKIT_LOG_PATH%\eskit.log"
)
endlocal
exit /b 0
