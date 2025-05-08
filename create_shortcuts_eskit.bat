@echo off
color 0A
title EasyKit Shortcut Creator

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
set /p choice=Choose an option: 
if %choice%==0 goto Exit
if %choice%==1 goto CreateShortcuts
if %choice%==2 goto RemoveShortcuts
goto Menu

:CreateShortcuts
cls
echo =====================================
echo  Create EasyKit Shortcuts
echo =====================================
echo.
echo This will create shortcuts for EasyKit.
echo.

choice /c YN /m "Do you want to create a desktop shortcut? (Y/N)"
if %errorlevel% equ 1 (
    echo Creating desktop shortcut...
    powershell "$s=(New-Object -COM WScript.Shell).CreateShortcut('%userprofile%\Desktop\EasyKit.lnk');$s.TargetPath='%~dp0run_eskit.bat';$s.WorkingDirectory='%~dp0';$s.IconLocation='%SystemRoot%\System32\SHELL32.dll,21';$s.Save()"
    echo Desktop shortcut created successfully!
    echo.
)

choice /c YN /m "Do you want to create a Start Menu shortcut? (Y/N)"
if %errorlevel% equ 1 (
    echo Creating Start Menu shortcut...
    if not exist "%appdata%\Microsoft\Windows\Start Menu\Programs\EasyKit" mkdir "%appdata%\Microsoft\Windows\Start Menu\Programs\EasyKit"
    powershell "$s=(New-Object -COM WScript.Shell).CreateShortcut('%appdata%\Microsoft\Windows\Start Menu\Programs\EasyKit\EasyKit.lnk');$s.TargetPath='%~dp0run_eskit.bat';$s.WorkingDirectory='%~dp0';$s.IconLocation='%SystemRoot%\System32\SHELL32.dll,21';$s.Save()"
    echo Start Menu shortcut created successfully!
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
        del /f "%desktopShortcut%"
        echo Desktop shortcut removed successfully!
    ) else (
        echo No desktop shortcut found.
    )
    echo.
)

choice /c YN /m "Do you want to remove the Start Menu shortcut? (Y/N)"
if %errorlevel% equ 1 (
    if exist "%startMenuShortcut%" (
        echo Removing Start Menu shortcut...
        del /f "%startMenuShortcut%"
        if exist "%startMenuFolder%" (
            rmdir "%startMenuFolder%"
        )
        echo Start Menu shortcut removed successfully!
    ) else (
        echo No Start Menu shortcut found.
    )
    echo.
)

echo All requested shortcuts have been removed.
echo.
pause
goto Menu

:Exit
exit /b 0