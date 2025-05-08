@echo off
color 0A
title Laravel Menu

:Menu
cls
echo.
echo ================================
echo  Laravel Menu
echo ================================
echo.
echo 0. Back to Main Menu
echo 1. Quick Auto Setup
echo 2. Install Laravel Packages
echo 3. Update Laravel Packages
echo 4. Regenerate Composer AutoLoad Files
echo 5. Build To Production
echo.
echo Dangerous:
echo 44. Reset All Cache
echo.
set /p choice=Choose an option: 
if %choice%==0 goto Exit
if %choice%==1 goto Setup
if %choice%==2 goto Install
if %choice%==3 goto UpdateLaravelPackages
if %choice%==4 goto RegenerateComposerAutoload
if %choice%==5 goto BuildToProduction
if %choice%==44 goto ResetAllCache
goto Menu

:Setup
echo.
echo Starting Laravel quick setup...
call :Install
if %errorlevel% neq 0 goto OperationFailed
call :UpdateLaravelPackages
if %errorlevel% neq 0 goto OperationFailed
echo Creating .env file...
call copy .env.example .env
if %errorlevel% neq 0 (
    echo Failed to create .env file!
    goto OperationFailed
)
echo Generating application key...
call php artisan key:generate
if %errorlevel% neq 0 goto OperationFailed
echo Running migrations...
call php artisan migrate
if %errorlevel% neq 0 goto OperationFailed
echo.
echo Laravel setup completed successfully!
pause
goto Menu

:Install
echo.
echo Installing Laravel packages...
call :CheckSoftwareMethod composer
if %errorlevel% neq 0 goto OperationFailed
call composer install
if %errorlevel% neq 0 goto OperationFailed
echo.
echo Laravel packages installed successfully!
pause
goto Menu

:UpdateLaravelPackages
echo.
echo Updating Laravel packages...
call :CheckSoftwareMethod composer
if %errorlevel% neq 0 goto OperationFailed
call composer update
if %errorlevel% neq 0 goto OperationFailed
echo.
echo Laravel packages updated successfully!
pause
goto Menu

:RegenerateComposerAutoload
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
echo WARNING: This will clear all Laravel cache!
choice /c YN /m "Are you sure you want to proceed? (Y/N)"
if %errorlevel% equ 2 goto Menu
if %errorlevel% equ 1 (
    echo.
    echo Clearing Laravel caches...
    call php artisan cache:clear
    call php artisan config:clear
    call php artisan route:clear
    call php artisan view:clear
    call php artisan clear-compiled
    call php artisan optimize
    echo.
    echo All Laravel caches cleared successfully!
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
