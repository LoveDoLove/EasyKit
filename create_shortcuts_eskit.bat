@echo off
color 0A
title EasyKit Shortcut Creator
setlocal enabledelayedexpansion

:: Check if the icon file exists
if not exist "%~dp0images\icon.ico" (
    echo WARNING: Icon file not found at "%~dp0images\icon.ico"
    set "ICON_LOCATION=%SystemRoot%\System32\SHELL32.dll,0"
    echo Using default Windows icon instead.
) else (
    set "ICON_LOCATION=%~dp0images\icon.ico"
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
if %choice%==0 goto Exit
if %choice%==1 goto CreateShortcuts
if %choice%==2 goto RemoveShortcuts
goto InvalidChoice

:InvalidChoice
echo.
echo Invalid option. Please try again.
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
    ) else (
        echo Failed to create desktop shortcut.
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
        ) else (
            echo Start Menu directory created successfully.
        )
    )
    
    if exist "%appdata%\Microsoft\Windows\Start Menu\Programs\EasyKit" (
        set "errorFile=%TEMP%\shortcut_error.txt"
        powershell -Command "try { $s=(New-Object -COM WScript.Shell).CreateShortcut('%appdata%\Microsoft\Windows\Start Menu\Programs\EasyKit\EasyKit.lnk'); $s.TargetPath='%~dp0run_eskit.bat'; $s.WorkingDirectory='%~dp0'; $s.IconLocation='%ICON_LOCATION%'; $s.Save(); Write-Output 'Success'; exit 0 } catch { $_.Exception.Message | Out-File '%errorFile%'; exit 1 }" > "%TEMP%\shortcut_result.txt" 2>&1
        set pwsh_error=!errorlevel!
        if !pwsh_error! equ 0 (
            echo Start Menu shortcut created successfully!
        ) else (
            echo Failed to create Start Menu shortcut.
            if exist "%errorFile%" (
                echo Error details:
                type "%errorFile%"
                del "%errorFile%" >nul 2>&1
            )
        )
    )
    echo.
)

choice /c YN /m "Do you want to pin to taskbar? (Y/N)"
if %errorlevel% equ 1 (
    echo.
    echo NOTE: Modern Windows versions (Windows 10 after certain updates and Windows 11)
    echo do not allow programmatic pinning to the taskbar for security reasons.
    echo If this fails, you'll need to manually pin the shortcut to your taskbar.
    echo.
    echo Creating temporary shortcut for taskbar pinning...
    set "tempShortcut=%TEMP%\EasyKit_temp.lnk"
    set "errorFile=%TEMP%\shortcut_error.txt"
    
    powershell -Command "try { $s=(New-Object -COM WScript.Shell).CreateShortcut('%tempShortcut%'); $s.TargetPath='%~dp0run_eskit.bat'; $s.WorkingDirectory='%~dp0'; $s.IconLocation='%ICON_LOCATION%'; $s.Save(); Write-Output 'Success'; exit 0 } catch { $_.Exception.Message | Out-File '%errorFile%'; exit 1 }" > "%TEMP%\shortcut_result.txt" 2>&1
    set pwsh_error=!errorlevel!
    
    if !pwsh_error! equ 0 (
        echo Temporary shortcut created successfully. Pinning to taskbar...
        powershell -Command "try { $shell = New-Object -COM Shell.Application; $folder = $shell.Namespace([System.IO.Path]::GetDirectoryName('%tempShortcut%')); $item = $folder.ParseName([System.IO.Path]::GetFileName('%tempShortcut%')); $verbs = $item.Verbs(); $pinVerb = $null; foreach($verb in $verbs) { if($verb.Name -match 'Pin to Taskbar') { $pinVerb = $verb; break; } }; if($pinVerb) { $pinVerb.DoIt(); Write-Output 'Success'; exit 0 } else { Write-Output 'Pin to Taskbar verb not found'; exit 1 } } catch { $_.Exception.Message | Out-File '%errorFile%'; exit 1 }" > "%TEMP%\pin_result.txt" 2>&1
        set pin_error=!errorlevel!
        
        if !pin_error! equ 0 (
            echo Successfully pinned to taskbar!
        ) else (
            echo Failed to pin to taskbar. This may not be supported on your Windows version.
            echo.
            echo MANUAL STEPS:
            echo 1. Open File Explorer and navigate to: %~dp0
            echo 2. Right-click on "run_eskit.bat"
            echo 3. Select "Show more options" if needed
            echo 4. Click "Pin to taskbar"
            
            if exist "%errorFile%" (
                echo.
                echo Technical error details:
                type "%errorFile%"
                del "%errorFile%" >nul 2>&1
            )
        )
        del /f "%tempShortcut%" >nul 2>&1
    ) else (
        echo Failed to create temporary shortcut for taskbar pinning.
        if exist "%errorFile%" (
            echo Error details:
            type "%errorFile%"
            del "%errorFile%" >nul 2>&1
        )
    )
    echo.
)

echo All requested shortcuts have been created.
echo.
pause
goto Menu

:RemoveShortcuts
cls
echo =====================================
echo  Remove EasyKit Shortcuts
echo =====================================
echo.
echo This will remove any existing EasyKit shortcuts.
echo.

set "desktopShortcut=%userprofile%\Desktop\EasyKit.lnk"
set "startMenuFolder=%appdata%\Microsoft\Windows\Start Menu\Programs\EasyKit"
set "startMenuShortcut=%startMenuFolder%\EasyKit.lnk"

choice /c YN /m "Do you want to remove the desktop shortcut? (Y/N)"
if %errorlevel% equ 1 (
    if exist "%desktopShortcut%" (
        echo Removing desktop shortcut...
        del /f "%desktopShortcut%" >nul 2>&1
        if !errorlevel! equ 0 (
            echo Desktop shortcut removed successfully!
        ) else (
            echo Failed to remove desktop shortcut. It may be in use.
        )
    ) else (
        echo No desktop shortcut found.
    )
    echo.
)

choice /c YN /m "Do you want to remove the Start Menu shortcut? (Y/N)"
if %errorlevel% equ 1 (
    if exist "%startMenuShortcut%" (
        echo Removing Start Menu shortcut...
        del /f "%startMenuShortcut%" >nul 2>&1
        if !errorlevel! equ 0 (
            echo Start Menu shortcut removed successfully!
            if exist "%startMenuFolder%" (
                rmdir "%startMenuFolder%" >nul 2>&1
                if !errorlevel! neq 0 (
                    echo Could not remove Start Menu folder. It may contain other files.
                )
            )
        ) else (
            echo Failed to remove Start Menu shortcut. It may be in use.
        )
    ) else (
        echo No Start Menu shortcut found.
    )
    echo.
)

choice /c YN /m "Do you want to unpin from taskbar (if pinned)? (Y/N)"
if %errorlevel% equ 1 (
    echo Attempting to unpin from taskbar...
    set "errorFile=%TEMP%\unpin_error.txt"
    powershell -Command "try { $shell = New-Object -COM Shell.Application; $folder = $shell.Namespace('%~dp0'); $item = $folder.ParseName('run_eskit.bat'); $verbs = $item.Verbs(); $unpinVerb = $null; foreach($verb in $verbs) { if($verb.Name -match 'Unpin from Taskbar') { $unpinVerb = $verb; break; } }; if($unpinVerb) { $unpinVerb.DoIt(); Write-Output 'Unpinned from taskbar.'; exit 0 } else { Write-Output 'Not pinned to taskbar or operation not supported.'; exit 0 } } catch { $_.Exception.Message | Out-File '%errorFile%'; exit 1 }" > "%TEMP%\unpin_result.txt" 2>&1
    type "%TEMP%\unpin_result.txt"
    if exist "%errorFile%" (
        echo Error details:
        type "%errorFile%"
        del "%errorFile%" >nul 2>&1
    )
    echo.
)

echo All requested shortcuts have been removed.
echo.
pause
goto Menu

:Exit
endlocal
exit /b 0