@echo off
REM ================================
REM  EasyKit Common Functions
REM ================================

REM Load configuration
call "%~dp0config_eskit.bat"

:DrawHeader
    echo.
    call :DrawLine
    echo  %~1
    call :DrawLine
    echo.
    goto :eof

:DrawLine
    set "line="
    for /l %%i in (1,1,%ESKIT_MENU_WIDTH%) do set "line=!line!="
    echo !line!
    goto :eof

:Log
    call %~dp0config_eskit.bat log %*
    goto :eof

:CheckUpdates
    if not "%ESKIT_CHECK_UPDATES%"=="true" goto :eof
    echo Checking for updates...
    REM Future implementation: Check online for new version
    goto :eof

:ShowTip
    if not "%ESKIT_SHOW_TIPS%"=="true" goto :eof
    echo.
    echo TIP: %~1
    echo.
    goto :eof

:ConfirmExit
    if not "%ESKIT_CONFIRM_EXIT%"=="true" goto :ConfirmExit_Yes
    choice /c YN /m "Are you sure you want to exit? (Y/N)"
    if errorlevel 2 goto :ConfirmExit_No
    :ConfirmExit_Yes
    exit /b 0
    :ConfirmExit_No
    exit /b 1

:HandleInvalidChoice
    echo.
    echo Invalid option. Please try again.
    timeout /t 2 >nul
    exit /b 1

:ReadChoice
    set choice=
    set /p choice=Choose an option: 
    if not defined choice exit /b 1
    set "numbers=0123456789"
    set "valid=true"
    for /L %%i in (0,1,9) do if "!choice:~%%i,1!" NEQ "" (
        if "!numbers:!choice:~%%i,1!=!" EQU "%numbers%" set "valid=false"
    )
    if "!valid!" EQU "false" exit /b 1
    exit /b 0

:DisplayProgressBar
    REM %1 = current progress (0-100)
    REM %2 = total width
    set "progress=%~1"
    set "width=%~2"
    if not defined width set "width=20"
    
    set /a filled=progress*width/100
    set /a empty=width-filled
    
    set "bar=["
    for /l %%i in (1,1,%filled%) do set "bar=!bar!#"
    for /l %%i in (1,1,%empty%) do set "bar=!bar! "
    set "bar=!bar!]"
    
    echo !bar! %progress%%%
    goto :eof

:CheckSoftwareGeneric
    REM %1 = software command to check
    REM %2 = install method if not found
    echo [INFO] Checking for %~1...
    where %~1 >nul 2>&1
    if %errorlevel% equ 0 (
        echo [OK] %~1 is already installed.
        exit /b 0
    )
    
    echo [WARN] %~1 is not installed.
    choice /c YN /m "Do you want to install %~1? (Y/N)"
    if !errorlevel! equ 2 (
        echo [INFO] %~1 installation cancelled.
        exit /b 1
    )
    
    call :Log "Installing %~1 via %~2"
    call :Install_%~2 %~1
    exit /b %errorlevel%

:Install_choco
    echo [INFO] Installing %~1 using Chocolatey...
    call %~dp0check_software_eskit.bat choco
    if %errorlevel% neq 0 exit /b 1
    
    choco install -y %~1
    if %errorlevel% neq 0 (
        echo [ERROR] Failed to install %~1.
        exit /b 1
    )
    
    echo [OK] %~1 installed successfully.
    exit /b 0

:Install_npm
    echo [INFO] Installing %~1 using npm...
    call %~dp0check_software_eskit.bat npm
    if %errorlevel% neq 0 exit /b 1
    
    npm install -g %~1
    if %errorlevel% neq 0 (
        echo [ERROR] Failed to install %~1.
        exit /b 1
    )
    
    echo [OK] %~1 installed successfully.
    exit /b 0
