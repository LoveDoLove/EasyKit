@echo off
color 0A
title Composer Menu

:Menu
cls
echo.
echo ================================
echo  Composer Menu
echo ================================
echo.
echo 0. Exit
echo 1. Install Packages
echo 2. Update Packages
echo 3. Regenerate AutoLoad Files
echo.
set /p choice=Choose an option: 
if %choice%==0 goto Exit
if %choice%==1 goto Install
if %choice%==2 goto UpdatePackages
if %choice%==3 goto RegenerateAutoload
goto Menu

:Install
call :CheckSoftwareMethod composer
call composer install -W
exit /b

:UpdatePackages
call :CheckSoftwareMethod composer
call composer update -W
exit /b

:RegenerateAutoload
call :CheckSoftwareMethod composer
call composer dump-autoload
exit /b

@REM Method
:CheckSoftwareMethod
call check_software_eskit.bat %1
exit /b
