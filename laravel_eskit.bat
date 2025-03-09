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
echo 0. Exit
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
call :Install
call :UpdateLaravelPackages
call copy .env.example .env
call php artisan key:generate
call php artisan migrate
exit /b

:Install
call :CheckSoftwareMethod composer
call composer install -W
exit /b

:UpdateLaravelPackages
call :CheckSoftwareMethod composer
call composer update -W
exit /b

:RegenerateComposerAutoload
call :CheckSoftwareMethod composer
call composer dump-autoload
exit /b

:BuildToProduction
call :CheckSoftwareMethod npm
call npm run build
exit /b

@REM Dangerous
:ResetAllCache
call :CheckSoftwareMethod composer
call php artisan cache:clear
call php artisan config:clear
call php artisan route:clear
call php artisan view:clear
call php artisan clear-compiled
call php artisan optimize
exit /b

@REM Method
:CheckSoftwareMethod
call check_software_eskit.bat %1
exit /b
