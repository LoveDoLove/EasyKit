@echo off
color 0A
title Package Checker

setlocal

if "%~1"=="" (
    echo No software specified. Exiting...
    exit /b 1
)

set "software=%~1"

if /i "%software%"=="choco" (
    REM Check if choco is installed
    where choco.exe >nul 2>&1
    if %ERRORLEVEL% EQU 0 (
        echo choco is already installed.
    ) else (
        echo choco is not installed. Automatically installing choco...
        powershell -Command "Set-ExecutionPolicy Bypass -Scope Process -Force; [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072; iex ((New-Object System.Net.WebClient).DownloadString('https://community.chocolatey.org/install.ps1'))"
        if %ERRORLEVEL% NEQ 0 (
            echo Failed to install choco.
            exit /b 1
        )
    )
)

if /i "%software%"=="winget" (
    REM Check if winget is installed
    where winget.exe >nul 2>&1
    if %ERRORLEVEL% EQU 0 (
        echo winget is already installed.
    ) else (
        echo winget is not installed. Installing winget using choco...
        choco install -y winget
        if %ERRORLEVEL% NEQ 0 (
            echo Failed to install winget.
            exit /b 1
        )
    )
)

if /i "%software%"=="npm" (
    REM Check if npm is installed
    where npm.cmd >nul 2>&1
    if %errorlevel% neq 0 (
        echo npm is not installed. Installing npm...
        choco install -y nodejs
        if %errorlevel% neq 0 (
            echo Failed to install npm using choco. Trying to install using winget...
            powershell -Command "winget install OpenJS.NodeJS"
            if errorlevel 1 (
                echo Failed to install npm using winget.
                exit /b 1
            )
        )
    ) else (
        echo npm is already installed.
    )
)

if /i "%software%"=="ncu" (
    REM Check if ncu is installed
    where ncu.cmd >nul 2>&1
    if %errorlevel% neq 0 (
        echo npm-check-updates is not installed. Installing npm-check-updates...
        npm install -g npm-check-updates
        if %errorlevel% neq 0 (
            echo Failed to install npm-check-updates.
            exit /b 1
        )
    ) else (
        echo npm-check-updates is already installed.
    )
)

if /i "%software%"=="composer" (
    REM Check if composer is installed
    where /q composer.phar
    if %errorlevel% neq 0 (
        echo Composer is not installed. Installing Composer using choco...
        powershell -Command "choco install -y composer"
        if %errorlevel% neq 0 (
            echo Failed to install Composer using choco. Trying to install using winget...
            powershell -Command "winget install Composer.Composer"
            if errorlevel 1 (
                echo Failed to install Composer using winget.
                exit /b 1
            )
        )
    ) else (
        echo Composer is already installed.
    )
)

endlocal
