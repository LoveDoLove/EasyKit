@echo off
setlocal enabledelayedexpansion

:: EasyKit Installer
:: Created: May 9, 2025
:: Version: 1.0.0

title EasyKit Installer
color 0A

echo =========================================
echo          EasyKit Installer
echo =========================================
echo.
echo This installer will set up EasyKit on your system.
echo.

:: Define default installation path
set "DEFAULT_INSTALL_PATH=%ProgramFiles%\EasyKit"
set "INSTALL_PATH=%DEFAULT_INSTALL_PATH%"

:: Ask for installation location
echo Default installation path: %DEFAULT_INSTALL_PATH%
set /p "CHANGE_PATH=Would you like to change the installation path? (Y/N): "
if /i "%CHANGE_PATH%"=="Y" (
    set /p "INSTALL_PATH=Enter new installation path: "
)

:: Create installation directory
echo.
echo Creating installation directory...
if not exist "%INSTALL_PATH%" (
    mkdir "%INSTALL_PATH%" 2>nul
    if !errorlevel! neq 0 (
        echo Failed to create installation directory. Please run as administrator or choose a different path.
        goto :cleanup
    )
)

:: Create directories
mkdir "%INSTALL_PATH%\logs" 2>nul
mkdir "%INSTALL_PATH%\images" 2>nul
mkdir "%INSTALL_PATH%\backups" 2>nul

:: Copy files
echo Copying files...
xcopy /E /I /Y "*.bat" "%INSTALL_PATH%\"
if exist "LICENSE" xcopy /Y "LICENSE" "%INSTALL_PATH%\"
if exist "README.md" xcopy /Y "README.md" "%INSTALL_PATH%\"
if exist "images\icon.ico" xcopy /Y "images\icon.ico" "%INSTALL_PATH%\images\"
if exist "images\icon.jpg" xcopy /Y "images\icon.jpg" "%INSTALL_PATH%\images\"

:: Create shortcuts
echo Creating shortcuts...
powershell -Command "try { $s=(New-Object -COM WScript.Shell).CreateShortcut('%userprofile%\Desktop\EasyKit.lnk'); $s.TargetPath='%INSTALL_PATH%\run_eskit.bat'; $s.WorkingDirectory='%INSTALL_PATH%'; $s.IconLocation='%INSTALL_PATH%\images\icon.ico'; $s.Save(); Write-Output 'Success'; exit 0 } catch { $_.Exception.Message; exit 1 }"

:: Create Start Menu shortcut
if not exist "%appdata%\Microsoft\Windows\Start Menu\Programs\EasyKit" (
    mkdir "%appdata%\Microsoft\Windows\Start Menu\Programs\EasyKit" 2>nul
)
powershell -Command "try { $s=(New-Object -COM WScript.Shell).CreateShortcut('%appdata%\Microsoft\Windows\Start Menu\Programs\EasyKit\EasyKit.lnk'); $s.TargetPath='%INSTALL_PATH%\run_eskit.bat'; $s.WorkingDirectory='%INSTALL_PATH%'; $s.IconLocation='%INSTALL_PATH%\images\icon.ico'; $s.Save(); Write-Output 'Success'; exit 0 } catch { $_.Exception.Message; exit 1 }"

:: Add to PATH (optional)
set /p "ADD_TO_PATH=Would you like to add EasyKit to your PATH? (Y/N): "
if /i "%ADD_TO_PATH%"=="Y" (
    echo Adding to PATH...
    setx PATH "%PATH%;%INSTALL_PATH%" /M
    if !errorlevel! neq 0 (
        echo Failed to add to PATH. You may need administrator privileges.
    )
)

:: Create uninstaller
echo Creating uninstaller...
set "UNINSTALLER=%INSTALL_PATH%\uninstall_eskit.bat"
(
    echo @echo off
    echo setlocal enabledelayedexpansion
    echo.
    echo title EasyKit Uninstaller
    echo color 0C
    echo.
    echo echo =========================================
    echo echo          EasyKit Uninstaller
    echo echo =========================================
    echo echo.
    echo echo This will uninstall EasyKit from your system.
    echo echo Installation directory: %INSTALL_PATH%
    echo echo.
    echo set /p "CONFIRM=Are you sure you want to uninstall EasyKit? (Y/N): "
    echo if /i "%%CONFIRM%%"=="Y" ^(
    echo     echo Removing shortcuts...
    echo     if exist "%%userprofile%%\Desktop\EasyKit.lnk" del "%%userprofile%%\Desktop\EasyKit.lnk"
    echo     if exist "%%appdata%%\Microsoft\Windows\Start Menu\Programs\EasyKit\EasyKit.lnk" del "%%appdata%%\Microsoft\Windows\Start Menu\Programs\EasyKit\EasyKit.lnk"
    echo     rmdir "%%appdata%%\Microsoft\Windows\Start Menu\Programs\EasyKit" 2^>nul
    echo.
    echo     echo Removing installation directory...
    echo     cd %%~dp0..
    echo     rmdir /S /Q "%INSTALL_PATH%" 2^>nul
    echo     if exist "%INSTALL_PATH%" ^(
    echo         echo Failed to remove some files. Please delete manually.
    echo         pause
    echo     ^) else ^(
    echo         echo EasyKit has been successfully uninstalled!
    echo     ^)
    echo ^) else ^(
    echo     echo Uninstallation cancelled.
    echo ^)
    echo.
    echo pause
) > "%UNINSTALLER%"

:: Finish installation
echo.
echo =========================================
echo       Installation Completed!
echo =========================================
echo.
echo EasyKit has been installed to:
echo %INSTALL_PATH%
echo.
echo Shortcuts have been created on your desktop and Start Menu.
echo.
echo To uninstall, run uninstall_eskit.bat from the installation directory
echo or use Add/Remove Programs.
echo.
echo Thank you for installing EasyKit!
echo.
pause

:cleanup
endlocal
