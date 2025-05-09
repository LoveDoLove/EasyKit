@echo off
echo Updating NSIS installer script for GitHub Actions...

REM Make backup of the original NSIS file
copy "installer\EasyKit.nsi" "installer\EasyKit.nsi.bak" /Y
echo Created backup of original NSIS file

REM Create the updated NSIS file
echo ; EasyKit Installer Script > installer\EasyKit.nsi
echo ; Created with NSIS (Nullsoft Scriptable Install System) >> installer\EasyKit.nsi
echo. >> installer\EasyKit.nsi
echo !include "MUI2.nsh" >> installer\EasyKit.nsi
echo !include "LogicLib.nsh" >> installer\EasyKit.nsi
echo. >> installer\EasyKit.nsi
echo ; Create output directory if it doesn't exist >> installer\EasyKit.nsi
echo !system 'mkdir "$%%TEMP%%\dist" ^>nul 2^>^&1' >> installer\EasyKit.nsi
echo. >> installer\EasyKit.nsi
echo ; General >> installer\EasyKit.nsi
echo Name "EasyKit" >> installer\EasyKit.nsi
echo OutFile "build\EasyKit_Setup.exe" >> installer\EasyKit.nsi
echo Unicode True >> installer\EasyKit.nsi
echo. >> installer\EasyKit.nsi
echo ; Default installation folder >> installer\EasyKit.nsi
echo InstallDir "$PROGRAMFILES\EasyKit" >> installer\EasyKit.nsi
echo InstallDirRegKey HKLM "Software\EasyKit" "Install_Dir" >> installer\EasyKit.nsi
echo. >> installer\EasyKit.nsi
echo ; Request application privileges for Windows Vista/7/8/10 >> installer\EasyKit.nsi
echo RequestExecutionLevel admin >> installer\EasyKit.nsi
echo. >> installer\EasyKit.nsi
echo ; Interface Settings >> installer\EasyKit.nsi
echo !define MUI_ABORTWARNING >> installer\EasyKit.nsi
echo !define MUI_ICON "..\images\icon.ico" >> installer\EasyKit.nsi
echo !define MUI_UNICON "..\images\icon.ico" >> installer\EasyKit.nsi
echo !define MUI_WELCOMEFINISHPAGE_BITMAP "..\images\installer-welcome.bmp" >> installer\EasyKit.nsi
echo !define MUI_HEADERIMAGE >> installer\EasyKit.nsi
echo !define MUI_HEADERIMAGE_BITMAP "..\images\installer-header.bmp" >> installer\EasyKit.nsi
echo !define MUI_HEADERIMAGE_RIGHT >> installer\EasyKit.nsi
echo. >> installer\EasyKit.nsi
echo ; Pages >> installer\EasyKit.nsi
echo !insertmacro MUI_PAGE_WELCOME >> installer\EasyKit.nsi
echo !insertmacro MUI_PAGE_LICENSE "..\LICENSE" >> installer\EasyKit.nsi
echo !insertmacro MUI_PAGE_DIRECTORY >> installer\EasyKit.nsi
echo !insertmacro MUI_PAGE_COMPONENTS >> installer\EasyKit.nsi
echo !insertmacro MUI_PAGE_INSTFILES >> installer\EasyKit.nsi
echo !insertmacro MUI_PAGE_FINISH >> installer\EasyKit.nsi
echo. >> installer\EasyKit.nsi
echo !insertmacro MUI_UNPAGE_CONFIRM >> installer\EasyKit.nsi
echo !insertmacro MUI_UNPAGE_INSTFILES >> installer\EasyKit.nsi
echo. >> installer\EasyKit.nsi
echo ; Languages >> installer\EasyKit.nsi
echo !insertmacro MUI_LANGUAGE "English" >> installer\EasyKit.nsi
echo. >> installer\EasyKit.nsi
echo ; Installer Sections >> installer\EasyKit.nsi
echo Section "EasyKit (required)" SecCore >> installer\EasyKit.nsi
echo   SectionIn RO >> installer\EasyKit.nsi
echo. >> installer\EasyKit.nsi
echo   ; Set output path to the installation directory >> installer\EasyKit.nsi
echo   SetOutPath $INSTDIR >> installer\EasyKit.nsi
echo. >> installer\EasyKit.nsi
echo   ; Copy main files >> installer\EasyKit.nsi
echo   File "..\run_eskit.bat" >> installer\EasyKit.nsi
echo   File "..\README.md" >> installer\EasyKit.nsi
echo   File "..\LICENSE" >> installer\EasyKit.nsi
echo   File "..\MIGRATION_GUIDE.md" >> installer\EasyKit.nsi
echo. >> installer\EasyKit.nsi
echo   ; Create directories >> installer\EasyKit.nsi
echo   CreateDirectory "$INSTDIR\logs" >> installer\EasyKit.nsi
echo   CreateDirectory "$INSTDIR\images" >> installer\EasyKit.nsi
echo   CreateDirectory "$INSTDIR\config" >> installer\EasyKit.nsi
echo   CreateDirectory "$INSTDIR\config\logs" >> installer\EasyKit.nsi
echo   CreateDirectory "$INSTDIR\scripts" >> installer\EasyKit.nsi
echo   CreateDirectory "$INSTDIR\scripts\build" >> installer\EasyKit.nsi
echo   CreateDirectory "$INSTDIR\scripts\core" >> installer\EasyKit.nsi
echo   CreateDirectory "$INSTDIR\scripts\tools" >> installer\EasyKit.nsi
echo. >> installer\EasyKit.nsi
echo   ; Copy config files >> installer\EasyKit.nsi
echo   SetOutPath "$INSTDIR\config" >> installer\EasyKit.nsi
echo   File "..\config\config_eskit.bat" >> installer\EasyKit.nsi
echo. >> installer\EasyKit.nsi
echo   ; Copy images >> installer\EasyKit.nsi
echo   SetOutPath "$INSTDIR\images" >> installer\EasyKit.nsi
echo   File "..\images\icon.ico" >> installer\EasyKit.nsi
echo   File "..\images\icon.jpg" >> installer\EasyKit.nsi
echo   File "..\images\installer-header.bmp" >> installer\EasyKit.nsi
echo   File "..\images\installer-welcome.bmp" >> installer\EasyKit.nsi
echo. >> installer\EasyKit.nsi
echo   ; Copy scripts - Core scripts >> installer\EasyKit.nsi
echo   SetOutPath "$INSTDIR\scripts\core" >> installer\EasyKit.nsi
echo   File "..\scripts\core\*.bat" >> installer\EasyKit.nsi
echo. >> installer\EasyKit.nsi
echo   ; Copy scripts - Build scripts >> installer\EasyKit.nsi
echo   SetOutPath "$INSTDIR\scripts\build" >> installer\EasyKit.nsi
echo   File "..\scripts\build\*.bat" >> installer\EasyKit.nsi
echo. >> installer\EasyKit.nsi
echo   ; Copy scripts - Tool scripts >> installer\EasyKit.nsi
echo   SetOutPath "$INSTDIR\scripts\tools" >> installer\EasyKit.nsi
echo   File "..\scripts\tools\*.bat" >> installer\EasyKit.nsi
echo. >> installer\EasyKit.nsi
echo   ; Set output path back to installation directory >> installer\EasyKit.nsi
echo   SetOutPath $INSTDIR >> installer\EasyKit.nsi
echo. >> installer\EasyKit.nsi
echo   ; Write the installation path into the registry >> installer\EasyKit.nsi
echo   WriteRegStr HKLM "Software\EasyKit" "Install_Dir" "$INSTDIR" >> installer\EasyKit.nsi
echo. >> installer\EasyKit.nsi
echo   ; Write the uninstall keys for Windows >> installer\EasyKit.nsi
echo   WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\EasyKit" "DisplayName" "EasyKit" >> installer\EasyKit.nsi
echo   WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\EasyKit" "UninstallString" '"$INSTDIR\uninstall.exe"' >> installer\EasyKit.nsi
echo   WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\EasyKit" "NoModify" 1 >> installer\EasyKit.nsi
echo   WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\EasyKit" "NoRepair" 1 >> installer\EasyKit.nsi
echo   WriteUninstaller "$INSTDIR\uninstall.exe" >> installer\EasyKit.nsi
echo SectionEnd >> installer\EasyKit.nsi
echo. >> installer\EasyKit.nsi
echo Section "Desktop Shortcut" SecDesktop >> installer\EasyKit.nsi
echo   CreateShortcut "$DESKTOP\EasyKit.lnk" "$INSTDIR\run_eskit.bat" "" "$INSTDIR\images\icon.ico" 0 >> installer\EasyKit.nsi
echo SectionEnd >> installer\EasyKit.nsi
echo. >> installer\EasyKit.nsi
echo Section "Start Menu Shortcuts" SecStartMenu >> installer\EasyKit.nsi
echo   CreateDirectory "$SMPROGRAMS\EasyKit" >> installer\EasyKit.nsi
echo   CreateShortcut "$SMPROGRAMS\EasyKit\EasyKit.lnk" "$INSTDIR\run_eskit.bat" "" "$INSTDIR\images\icon.ico" 0 >> installer\EasyKit.nsi
echo   CreateShortcut "$SMPROGRAMS\EasyKit\Uninstall.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0 >> installer\EasyKit.nsi
echo SectionEnd >> installer\EasyKit.nsi
echo. >> installer\EasyKit.nsi
echo ; Section "Add to PATH" - Removed for CI compatibility >> installer\EasyKit.nsi
echo. >> installer\EasyKit.nsi
echo ; Descriptions >> installer\EasyKit.nsi
echo !insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN >> installer\EasyKit.nsi
echo   !insertmacro MUI_DESCRIPTION_TEXT ${SecCore} "The core files required to run EasyKit." >> installer\EasyKit.nsi
echo   !insertmacro MUI_DESCRIPTION_TEXT ${SecDesktop} "Create a shortcut to EasyKit on your desktop." >> installer\EasyKit.nsi
echo   !insertmacro MUI_DESCRIPTION_TEXT ${SecStartMenu} "Create shortcuts to EasyKit in your Start Menu." >> installer\EasyKit.nsi
echo   ; PATH section description removed >> installer\EasyKit.nsi
echo !insertmacro MUI_FUNCTION_DESCRIPTION_END >> installer\EasyKit.nsi
echo. >> installer\EasyKit.nsi
echo ; Uninstaller Section >> installer\EasyKit.nsi
echo Section "Uninstall" >> installer\EasyKit.nsi
echo   ; Remove registry keys >> installer\EasyKit.nsi
echo   DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\EasyKit" >> installer\EasyKit.nsi
echo   DeleteRegKey HKLM "Software\EasyKit" >> installer\EasyKit.nsi
echo. >> installer\EasyKit.nsi
echo   ; Remove main files >> installer\EasyKit.nsi
echo   Delete $INSTDIR\run_eskit.bat >> installer\EasyKit.nsi
echo   Delete $INSTDIR\README.md >> installer\EasyKit.nsi
echo   Delete $INSTDIR\LICENSE >> installer\EasyKit.nsi
echo   Delete $INSTDIR\MIGRATION_GUIDE.md >> installer\EasyKit.nsi
echo   Delete $INSTDIR\uninstall.exe >> installer\EasyKit.nsi
echo. >> installer\EasyKit.nsi
echo   ; Remove config files >> installer\EasyKit.nsi
echo   Delete "$INSTDIR\config\config_eskit.bat" >> installer\EasyKit.nsi
echo   Delete "$INSTDIR\config\logs\*.log" >> installer\EasyKit.nsi
echo. >> installer\EasyKit.nsi
echo   ; Remove images >> installer\EasyKit.nsi
echo   Delete "$INSTDIR\images\icon.ico" >> installer\EasyKit.nsi
echo   Delete "$INSTDIR\images\icon.jpg" >> installer\EasyKit.nsi
echo   Delete "$INSTDIR\images\installer-header.bmp" >> installer\EasyKit.nsi
echo   Delete "$INSTDIR\images\installer-welcome.bmp" >> installer\EasyKit.nsi
echo. >> installer\EasyKit.nsi
echo   ; Remove script files >> installer\EasyKit.nsi
echo   Delete "$INSTDIR\scripts\core\*.bat" >> installer\EasyKit.nsi
echo   Delete "$INSTDIR\scripts\build\*.bat" >> installer\EasyKit.nsi
echo   Delete "$INSTDIR\scripts\tools\*.bat" >> installer\EasyKit.nsi
echo. >> installer\EasyKit.nsi
echo   ; Remove log files >> installer\EasyKit.nsi
echo   Delete "$INSTDIR\logs\*.log" >> installer\EasyKit.nsi
echo. >> installer\EasyKit.nsi
echo   ; Remove directories >> installer\EasyKit.nsi
echo   RMDir "$INSTDIR\config\logs" >> installer\EasyKit.nsi
echo   RMDir "$INSTDIR\config" >> installer\EasyKit.nsi
echo   RMDir "$INSTDIR\images" >> installer\EasyKit.nsi
echo   RMDir "$INSTDIR\logs" >> installer\EasyKit.nsi
echo   RMDir "$INSTDIR\scripts\core" >> installer\EasyKit.nsi
echo   RMDir "$INSTDIR\scripts\build" >> installer\EasyKit.nsi
echo   RMDir "$INSTDIR\scripts\tools" >> installer\EasyKit.nsi
echo   RMDir "$INSTDIR\scripts" >> installer\EasyKit.nsi
echo. >> installer\EasyKit.nsi
echo   ; Remove shortcuts >> installer\EasyKit.nsi
echo   Delete "$DESKTOP\EasyKit.lnk" >> installer\EasyKit.nsi
echo   Delete "$SMPROGRAMS\EasyKit\EasyKit.lnk" >> installer\EasyKit.nsi
echo   Delete "$SMPROGRAMS\EasyKit\Uninstall.lnk" >> installer\EasyKit.nsi
echo   RMDir "$SMPROGRAMS\EasyKit" >> installer\EasyKit.nsi
echo. >> installer\EasyKit.nsi
echo   ; Remove installation directory >> installer\EasyKit.nsi
echo   RMDir "$INSTDIR" >> installer\EasyKit.nsi
echo. >> installer\EasyKit.nsi
echo   ; Remove from PATH variable - Removed for CI compatibility >> installer\EasyKit.nsi
echo   ; EnVar::DeleteValue "PATH" "$INSTDIR" >> installer\EasyKit.nsi
echo SectionEnd >> installer\EasyKit.nsi

echo NSIS installer script updated successfully
echo.
echo You need to test this by running the GitHub Actions workflow manually
echo.
echo Note: The paths have been fixed to account for the new project structure
echo.
