@echo off
setlocal EnableDelayedExpansion

REM Load configuration if available
if exist "%~dp0..\..\config\config_eskit.bat" (
    call "%~dp0..\..\config\config_eskit.bat"
    color %ESKIT_COLOR%
    title %ESKIT_TITLE_PREFIX% - Software Checker
) else (
    color 0A
    title Package Checker
)

:: ======================================
:: EasyKit Software Dependency Manager
:: ======================================

:: Check if software name was provided
if "%~1"=="" (
    echo [ERROR] No software specified. Exiting...
    if "%ESKIT_ENABLE_LOGGING%"=="true" (
        echo %DATE% %TIME% - [ERROR] Software checker called without specifying software >> "%ESKIT_LOG_PATH%\eskit.log"
    )
    exit /b 1
)

:: Set software name
set "software=%~1"

:: Log the check
if "%ESKIT_ENABLE_LOGGING%"=="true" (
    echo %DATE% %TIME% - Checking for software: %software% >> "%ESKIT_LOG_PATH%\eskit.log"
)

:: Always ask for confirmation regardless of parameters
set "askInstall=1"

:: Main software checking logic
call :Check_%software%
exit /b %errorlevel%

:: ======================================
:: Software-specific check functions
:: ======================================

:Check_choco
echo [INFO] Checking for Chocolatey...
where choco.exe >nul 2>&1
if %errorlevel% equ 0 (
    echo [OK] Chocolatey is already installed.
    exit /b 0
)

echo [WARN] Chocolatey is not installed.
choice /c YN /m "Do you want to install Chocolatey? (Y/N)"
if !errorlevel! equ 2 (
    echo [INFO] Chocolatey installation cancelled.
    exit /b 1
)

echo [INFO] Installing Chocolatey...
powershell -Command "Set-ExecutionPolicy Bypass -Scope Process -Force; [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072; iex ((New-Object System.Net.WebClient).DownloadString('https://community.chocolatey.org/install.ps1'))"
if %errorlevel% neq 0 (
    echo [ERROR] Failed to install Chocolatey.
    echo [INFO] Please install manually from: https://chocolatey.org/install
    exit /b 1
)

echo [OK] Chocolatey installed successfully.
exit /b 0

:Check_winget
echo [INFO] Checking for winget...
where winget.exe >nul 2>&1
if %errorlevel% equ 0 (
    echo [OK] Winget is already installed.
    exit /b 0
)

echo [WARN] Winget is not installed.
choice /c YN /m "Do you want to install Winget? (Y/N)"
if !errorlevel! equ 2 (
    echo [INFO] Winget installation cancelled.
    exit /b 1
)

echo [INFO] Installing Winget...
call :Check_choco
if %errorlevel% neq 0 exit /b 1

echo [INFO] Installing Winget using Chocolatey...
choco install -y winget
if %errorlevel% neq 0 (
    echo [ERROR] Failed to install Winget.
    echo [INFO] Please install manually from Microsoft Store.
    exit /b 1
)

echo [OK] Winget installed successfully.
exit /b 0

:Check_npm
echo [INFO] Checking for npm...
where npm.cmd >nul 2>&1
if %errorlevel% equ 0 (
    echo [OK] npm is already installed.
    exit /b 0
)

echo [WARN] npm is not installed.
choice /c YN /m "Do you want to install Node.js and npm? (Y/N)"
if !errorlevel! equ 2 (
    echo [INFO] Node.js installation cancelled.
    exit /b 1
)

echo [INFO] Installing Node.js and npm...

:: Try with Chocolatey first
call :Check_choco
if %errorlevel% equ 0 (
    echo [INFO] Installing Node.js using Chocolatey...
    choco install -y nodejs
    if %errorlevel% equ 0 (
        echo [OK] Node.js and npm installed successfully.
        exit /b 0
    )
    echo [WARN] Failed to install Node.js using Chocolatey. Trying Winget...
)

:: Try with Winget as fallback
call :Check_winget
if %errorlevel% equ 0 (
    echo [INFO] Installing Node.js using Winget...
    winget install OpenJS.NodeJS
    if %errorlevel% equ 0 (
        echo [OK] Node.js and npm installed successfully.
        exit /b 0
    )
)

echo [ERROR] Failed to install Node.js.
echo [INFO] Please install manually from: https://nodejs.org/
exit /b 1

:Check_ncu
echo [INFO] Checking for npm-check-updates...
where ncu.cmd >nul 2>&1
if %errorlevel% equ 0 (
    echo [OK] npm-check-updates is already installed.
    exit /b 0
)

echo [WARN] npm-check-updates is not installed.
choice /c YN /m "Do you want to install npm-check-updates? (Y/N)"
if !errorlevel! equ 2 (
    echo [INFO] npm-check-updates installation cancelled.
    exit /b 1
)

echo [INFO] Installing npm-check-updates...
call :Check_npm
if %errorlevel% neq 0 exit /b 1

npm install -g npm-check-updates
if %errorlevel% neq 0 (
    echo [ERROR] Failed to install npm-check-updates.
    exit /b 1
)

echo [OK] npm-check-updates installed successfully.
exit /b 0

:Check_composer
echo [INFO] Checking for Composer...
where composer >nul 2>&1 || where composer.bat >nul 2>&1 || where composer.phar >nul 2>&1
if %errorlevel% equ 0 (
    echo [OK] Composer is already installed.
    exit /b 0
)

echo [WARN] Composer is not installed.
choice /c YN /m "Do you want to install Composer? (Y/N)"
if !errorlevel! equ 2 (
    echo [INFO] Composer installation cancelled.
    exit /b 1
)

echo [INFO] Installing Composer...

:: Try with Chocolatey first
call :Check_choco
if %errorlevel% equ 0 (
    echo [INFO] Installing Composer using Chocolatey...
    choco install -y composer
    if %errorlevel% equ 0 (
        echo [OK] Composer installed successfully.
        exit /b 0
    )
    echo [WARN] Failed to install Composer using Chocolatey. Trying Winget...
)

:: Try with Winget as fallback
call :Check_winget
if %errorlevel% equ 0 (
    echo [INFO] Installing Composer using Winget...
    winget install Composer.Composer
    if %errorlevel% equ 0 (
        echo [OK] Composer installed successfully.
        exit /b 0
    )
)

echo [ERROR] Failed to install Composer.
echo [INFO] Please install manually from: https://getcomposer.org/download/
exit /b 1

:Check_git
echo [INFO] Checking for Git...
where git.exe >nul 2>&1
if %errorlevel% equ 0 (
    echo [OK] Git is already installed.
    exit /b 0
)

echo [WARN] Git is not installed.
choice /c YN /m "Do you want to install Git? (Y/N)"
if !errorlevel! equ 2 (
    echo [INFO] Git installation cancelled.
    exit /b 1
)

echo [INFO] Installing Git...

:: Try with Chocolatey first
call :Check_choco
if %errorlevel% equ 0 (
    echo [INFO] Installing Git using Chocolatey...
    choco install -y git
    if %errorlevel% equ 0 (
        echo [OK] Git installed successfully.
        exit /b 0
    )
    echo [WARN] Failed to install Git using Chocolatey. Trying Winget...
)

:: Try with Winget as fallback
call :Check_winget
if %errorlevel% equ 0 (
    echo [INFO] Installing Git using Winget...
    winget install Git.Git
    if %errorlevel% equ 0 (
        echo [OK] Git installed successfully.
        exit /b 0
    )
)

echo [ERROR] Failed to install Git.
echo [INFO] Please install manually from: https://git-scm.com/downloads
exit /b 1

:Check_gh
echo [INFO] Checking for GitHub CLI...
where gh.exe >nul 2>&1
if %errorlevel% equ 0 (
    echo [OK] GitHub CLI is already installed.
    
    :: Check for authentication
    gh auth status >nul 2>&1
    if %errorlevel% neq 0 (
        echo [WARN] GitHub CLI is not authenticated.
        choice /c YN /m "Do you want to login to GitHub now? (Y/N)"
        if !errorlevel! equ 2 (
            echo [INFO] GitHub authentication skipped.
            exit /b 1
        )
        
        echo [INFO] A browser window will open for you to login...
        pause
        gh auth login
        if %errorlevel% neq 0 (
            echo [ERROR] Failed to authenticate with GitHub.
            exit /b 1
        )
        echo [OK] Successfully authenticated with GitHub.
    )
    
    exit /b 0
)

echo [WARN] GitHub CLI is not installed.
choice /c YN /m "Do you want to install GitHub CLI? (Y/N)"
if !errorlevel! equ 2 (
    echo [INFO] GitHub CLI installation cancelled.
    exit /b 1
)

echo [INFO] Installing GitHub CLI...
echo [WARN] Administrator rights are required to install GitHub CLI.

:: Check if running with admin rights
net session >nul 2>&1
if %errorlevel% neq 0 (
    echo [ERROR] This script must be run as administrator to install GitHub CLI.
    echo [INFO] Please right-click on the command prompt and select "Run as administrator".
    echo [INFO] Then try again, or install GitHub CLI manually from: https://cli.github.com/
    exit /b 1
)

:: Try with Chocolatey first
call :Check_choco
if %errorlevel% equ 0 (
    echo [INFO] Installing GitHub CLI using Chocolatey...
    choco install -y gh
    
    :: Verify installation worked
    where gh.exe >nul 2>&1
    if %errorlevel% neq 0 (
        echo [ERROR] GitHub CLI installation failed. Chocolatey reported success but 'gh' command not found.
        echo [INFO] Try installing manually from: https://cli.github.com/
        exit /b 1
    )
    
    echo [OK] GitHub CLI installed successfully.
    
    :: Setup authentication
    echo [INFO] Setting up GitHub authentication...
    choice /c YN /m "Do you want to login to GitHub now? (Y/N)"
    if !errorlevel! equ 2 (
        echo [INFO] GitHub authentication skipped.
        exit /b 0
    )
    
    echo [INFO] A browser window will open for you to login...
    pause
    gh auth login
    if %errorlevel% neq 0 (
        echo [ERROR] Failed to authenticate with GitHub.
        exit /b 1
    )
    echo [OK] Successfully authenticated with GitHub.
    exit /b 0
)

:: Try direct download as fallback
echo [INFO] Installing GitHub CLI via direct download...
echo [INFO] This may take a moment...

:: Create temp directory
set "tempdir=%TEMP%\ghcli_install_%RANDOM%"
mkdir "%tempdir%" 2>nul
pushd "%tempdir%"

:: Download and extract GitHub CLI
powershell -Command "& {[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12; Invoke-WebRequest -Uri 'https://github.com/cli/cli/releases/download/v2.29.0/gh_2.29.0_windows_amd64.zip' -OutFile 'gh.zip'}"
if %errorlevel% neq 0 (
    echo [ERROR] Failed to download GitHub CLI.
    popd
    rmdir /s /q "%tempdir%" 2>nul
    exit /b 1
)

powershell -Command "& {Expand-Archive -Path 'gh.zip' -DestinationPath '.' -Force}"
if %errorlevel% neq 0 (
    echo [ERROR] Failed to extract GitHub CLI.
    popd
    rmdir /s /q "%tempdir%" 2>nul
    exit /b 1
)

:: Move to Program Files
set "ghdir=%ProgramFiles%\GitHub CLI"
mkdir "%ghdir%" 2>nul
xcopy /E /Y "gh_*\*" "%ghdir%\" >nul

:: Add to PATH
setx PATH "%PATH%;%ghdir%\bin" /M

echo [INFO] Installation completed. Please restart your terminal/command prompt.
echo [INFO] After restarting, run this script again to verify the installation.

popd
rmdir /s /q "%tempdir%" 2>nul

echo [ERROR] Failed to install GitHub CLI.
echo [INFO] Please install manually from: https://cli.github.com/
exit /b 1

:Check_unknown
echo [ERROR] Unknown software: %software%
echo [INFO] Supported software: choco, winget, npm, ncu, composer, git, gh
exit /b 1

endlocal
