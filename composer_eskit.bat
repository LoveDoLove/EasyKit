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
echo 0. Back to Main Menu
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
