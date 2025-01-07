@echo off
color 0A
title Laravel X Npm X Composer Easy Kit Menu

:Menu
cls
echo.
echo ================================
echo  Laravel X Npm X Composer Easy Kit Menu
echo ================================
echo.
echo 0. Exit
echo 1. Npm Menu
echo 2. Laravel Menu
echo 3. Composer Menu
echo.
set /p choice=Choose an option: 
if %choice%==0 goto Exit
if %choice%==1 call npm_eskit.bat
if %choice%==2 call laravel_eskit.bat
if %choice%==3 call composer_eskit.bat
goto Menu

:Exit
exit
