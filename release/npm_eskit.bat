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
echo 0. Exit
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
call :CheckSoftwareMethod npm
call npm install
exit /b

:UpdateNpmPackages
call :CheckSoftwareMethod npm
call :CheckSoftwareMethod ncu
call ncu -u
call npm install
exit /b

:BuildToProduction
call :CheckSoftwareMethod npm
call npm run build
exit /b

@REM Dangerous
:ResetAllCache
call :CheckSoftwareMethod npm
call npm cache clean --force
exit /b

@REM Method
:CheckSoftwareMethod
call check_software_eskit.bat %1
exit /b
