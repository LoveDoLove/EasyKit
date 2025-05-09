@echo off
setlocal enabledelayedexpansion

REM Load configuration
call "%~dp0config_eskit.bat"

color %ESKIT_COLOR%
title %ESKIT_TITLE_PREFIX% - Shortcut Creator

REM Log the startup
if "%ESKIT_ENABLE_LOGGING%"=="true" (
    echo %DATE% %TIME% - Opened Shortcut Creator >> "%ESKIT_LOG_PATH%\eskit.log"
)

REM Check if the icon file exists
if not exist "%ESKIT_ICON_PATH%" (
    echo WARNING: Icon file not found at "%ESKIT_ICON_PATH%"
    if "%ESKIT_ENABLE_LOGGING%"=="true" (
        echo %DATE% %TIME% - Icon file not found at %ESKIT_ICON_PATH% >> "%ESKIT_LOG_PATH%\eskit.log"
    )
    set "ICON_LOCATION=%SystemRoot%\System32\SHELL32.dll,0"
    echo Using default Windows icon instead.
) else (
    set "ICON_LOCATION=%ESKIT_ICON_PATH%"
)

:Menu
cls
echo =====================================
echo  EasyKit Shortcut Manager
echo =====================================
echo.
echo 1. Create Shortcuts
echo 2. Remove Shortcuts
echo 0. Exit
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
if %choice% GTR 2 goto InvalidChoice

if %choice%==0 goto Exit
if %choice%==1 goto CreateShortcuts
if %choice%==2 goto RemoveShortcuts
goto InvalidChoice

:InvalidChoice
echo.
echo Invalid option. Please try again.
if "%ESKIT_ENABLE_LOGGING%"=="true" (
    echo %DATE% %TIME% - Invalid menu choice attempted in Shortcut Creator >> "%ESKIT_LOG_PATH%\eskit.log"
)
timeout /t 2 >nul
goto Menu

:CreateShortcuts
cls
echo =====================================
echo  Create EasyKit Shortcuts
echo =====================================
echo.
echo This will create shortcuts for EasyKit.
echo.

echo Current settings:
echo - Target: %~dp0run_eskit.bat
echo - Working directory: %~dp0
echo - Icon location: %ICON_LOCATION%
echo.

choice /c YN /m "Do you want to create a desktop shortcut? (Y/N)"
if %errorlevel% equ 1 (
    echo Creating desktop shortcut...
    set "errorFile=%TEMP%\shortcut_error.txt"
    
    echo Command will use these paths:
    echo - Desktop target: %userprofile%\Desktop\EasyKit.lnk
    echo - Script path: %~dp0run_eskit.bat
    echo - Working dir: %~dp0
    echo - Icon: %ICON_LOCATION%
    echo.
    
    powershell -Command "try { $s=(New-Object -COM WScript.Shell).CreateShortcut('%userprofile%\Desktop\EasyKit.lnk'); $s.TargetPath='%~dp0run_eskit.bat'; $s.WorkingDirectory='%~dp0'; $s.IconLocation='%ICON_LOCATION%'; $s.Save(); Write-Output 'Success'; exit 0 } catch { $_.Exception.Message | Out-File '%errorFile%'; exit 1 }" > "%TEMP%\shortcut_result.txt" 2>&1
    set pwsh_error=%errorlevel%
    if !pwsh_error! equ 0 (
        echo Desktop shortcut created successfully!
        if "%ESKIT_ENABLE_LOGGING%"=="true" (
            echo %DATE% %TIME% - Desktop shortcut created successfully >> "%ESKIT_LOG_PATH%\eskit.log"
        )
    ) else (
        echo Failed to create desktop shortcut.
        if "%ESKIT_ENABLE_LOGGING%"=="true" (
            echo %DATE% %TIME% - Failed to create desktop shortcut, error code: !pwsh_error! >> "%ESKIT_LOG_PATH%\eskit.log"
        )
        if exist "%errorFile%" (
            echo Error details:
            type "%errorFile%"
            del "%errorFile%" >nul 2>&1
        )
        echo.
        echo Check if:
        echo - You have permissions to write to your desktop
        echo - The icon file exists and is accessible
        echo - PowerShell is installed and running correctly
    )
    echo.
)

choice /c YN /m "Do you want to create a Start Menu shortcut? (Y/N)"
if %errorlevel% equ 1 (
    echo Creating Start Menu shortcut...
    if not exist "%appdata%\Microsoft\Windows\Start Menu\Programs\EasyKit" (
        mkdir "%appdata%\Microsoft\Windows\Start Menu\Programs\EasyKit" 2>nul
        set mkdir_error=!errorlevel!
        if !mkdir_error! neq 0 (
            echo Failed to create Start Menu directory. Error code: !mkdir_error!
            if "%ESKIT_ENABLE_LOGGING%"=="true" (
                echo %DATE% %TIME% - Failed to create Start Menu directory, error code: !mkdir_error! >> "%ESKIT_LOG_PATH%\eskit.log"
            )
        ) else (
            echo Start Menu directory created successfully.
        )
    )
    
    powershell -Command "try { $s=(New-Object -COM WScript.Shell).CreateShortcut('%appdata%\Microsoft\Windows\Start Menu\Programs\EasyKit\EasyKit.lnk'); $s.TargetPath='%~dp0run_eskit.bat'; $s.WorkingDirectory='%~dp0'; $s.IconLocation='%ICON_LOCATION%'; $s.Save(); Write-Output 'Success'; exit 0 } catch { $_.Exception.Message | Out-File '%errorFile%'; exit 1 }" > "%TEMP%\shortcut_result.txt" 2>&1
    set pwsh_error=%errorlevel%
    if !pwsh_error! equ 0 (
        echo Start Menu shortcut created successfully!
        if "%ESKIT_ENABLE_LOGGING%"=="true" (
            echo %DATE% %TIME% - Start Menu shortcut created successfully >> "%ESKIT_LOG_PATH%\eskit.log"
        )
    ) else (
        echo Failed to create Start Menu shortcut.
        if "%ESKIT_ENABLE_LOGGING%"=="true" (
            echo %DATE% %TIME% - Failed to create Start Menu shortcut, error code: !pwsh_error! >> "%ESKIT_LOG_PATH%\eskit.log"
        )
        if exist "%errorFile%" (
            echo Error details:
            type "%errorFile%"
            del "%errorFile%" >nul 2>&1
        )
    )
    echo.
)

pause
goto Menu

:RemoveShortcuts
cls
echo =====================================
echo  Remove EasyKit Shortcuts
echo =====================================
echo.
echo This will remove any EasyKit shortcuts.
echo.

choice /c YN /m "Do you want to remove the desktop shortcut? (Y/N)"
if %errorlevel% equ 1 (
    if exist "%userprofile%\Desktop\EasyKit.lnk" (
        del "%userprofile%\Desktop\EasyKit.lnk" >nul 2>&1
        if !errorlevel! equ 0 (
            echo Desktop shortcut removed successfully!
            if "%ESKIT_ENABLE_LOGGING%"=="true" (
                echo %DATE% %TIME% - Desktop shortcut removed successfully >> "%ESKIT_LOG_PATH%\eskit.log"
            )
        ) else (
            echo Failed to remove desktop shortcut.
            if "%ESKIT_ENABLE_LOGGING%"=="true" (
                echo %DATE% %TIME% - Failed to remove desktop shortcut >> "%ESKIT_LOG_PATH%\eskit.log"
            )
        )
    ) else (
        echo No desktop shortcut found.
    )
    echo.
)

choice /c YN /m "Do you want to remove the Start Menu shortcut? (Y/N)"
if %errorlevel% equ 1 (
    if exist "%appdata%\Microsoft\Windows\Start Menu\Programs\EasyKit\EasyKit.lnk" (
        del "%appdata%\Microsoft\Windows\Start Menu\Programs\EasyKit\EasyKit.lnk" >nul 2>&1
        if !errorlevel! equ 0 (
            echo Start Menu shortcut removed successfully!
            if "%ESKIT_ENABLE_LOGGING%"=="true" (
                echo %DATE% %TIME% - Start Menu shortcut removed successfully >> "%ESKIT_LOG_PATH%\eskit.log"
            )
            
            REM Try to remove the directory if it's empty
            rmdir "%appdata%\Microsoft\Windows\Start Menu\Programs\EasyKit" >nul 2>&1
        ) else (
            echo Failed to remove Start Menu shortcut.
            if "%ESKIT_ENABLE_LOGGING%"=="true" (
                echo %DATE% %TIME% - Failed to remove Start Menu shortcut >> "%ESKIT_LOG_PATH%\eskit.log"
            )
        )
    ) else (
        echo No Start Menu shortcut found.
    )
    echo.
)

pause
goto Menu

:Exit
if "%ESKIT_ENABLE_LOGGING%"=="true" (
    echo %DATE% %TIME% - Exiting Shortcut Creator >> "%ESKIT_LOG_PATH%\eskit.log"
)
endlocal
exit /b 0
