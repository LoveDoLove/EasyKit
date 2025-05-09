; EasyKit Installer Script 
; Created with NSIS (Nullsoft Scriptable Install System) 
 
!include "MUI2.nsh" 
!include "LogicLib.nsh" 
 
; Create output directory if it doesn't exist 
!system 'mkdir "$%TEMP%\dist" >nul 2>&1' 
 
; General 
Name "EasyKit v1.2.5" 
OutFile "build\EasyKit_Setup.exe" 
Unicode True
 
; Default installation folder 
InstallDir "$PROGRAMFILES\EasyKit" 
InstallDirRegKey HKLM "Software\EasyKit" "Install_Dir" 
 
; Request application privileges for Windows Vista/7/8/10 
RequestExecutionLevel admin 
 
; Interface Settings 
!define MUI_ABORTWARNING 
!define MUI_ICON "..\images\icon.ico" 
!define MUI_UNICON "..\images\icon.ico" 
!define MUI_WELCOMEFINISHPAGE_BITMAP "..\images\installer-welcome.bmp" 
!define MUI_HEADERIMAGE 
!define MUI_HEADERIMAGE_BITMAP "..\images\installer-header.bmp" 
!define MUI_HEADERIMAGE_RIGHT 
 
; Pages 
!insertmacro MUI_PAGE_WELCOME 
!insertmacro MUI_PAGE_LICENSE "..\LICENSE" 
!insertmacro MUI_PAGE_DIRECTORY 
!insertmacro MUI_PAGE_COMPONENTS 
!insertmacro MUI_PAGE_INSTFILES 
!insertmacro MUI_PAGE_FINISH 
 
!insertmacro MUI_UNPAGE_CONFIRM 
!insertmacro MUI_UNPAGE_INSTFILES 
 
; Languages 
!insertmacro MUI_LANGUAGE "English" 
 
; Installer Sections 
Section "EasyKit (required)" SecCore 
  SectionIn RO 
 
  ; Set output path to the installation directory 
  SetOutPath $INSTDIR 
 
  ; Copy main files 
  File "..\run_eskit.bat" 
  File "..\README.md" 
  File "..\LICENSE" 
  File "..\MIGRATION_GUIDE.md" 
 
  ; Create directories 
  CreateDirectory "$INSTDIR\logs" 
  CreateDirectory "$INSTDIR\images" 
  CreateDirectory "$INSTDIR\config" 
  CreateDirectory "$INSTDIR\config\logs" 
  CreateDirectory "$INSTDIR\scripts" 
  CreateDirectory "$INSTDIR\scripts\build" 
  CreateDirectory "$INSTDIR\scripts\core" 
  CreateDirectory "$INSTDIR\scripts\tools" 
 
  ; Copy config files 
  SetOutPath "$INSTDIR\config" 
  File "..\config\config_eskit.bat" 
 
  ; Copy images 
  SetOutPath "$INSTDIR\images" 
  File "..\images\icon.ico" 
  File "..\images\icon.jpg" 
  File "..\images\installer-header.bmp" 
  File "..\images\installer-welcome.bmp" 
 
  ; Copy scripts - Core scripts 
  SetOutPath "$INSTDIR\scripts\core" 
  File "..\scripts\core\*.bat" 
 
  ; Copy scripts - Build scripts 
  SetOutPath "$INSTDIR\scripts\build" 
  File "..\scripts\build\*.bat" 
 
  ; Copy scripts - Tool scripts 
  SetOutPath "$INSTDIR\scripts\tools" 
  File "..\scripts\tools\*.bat" 
 
  ; Set output path back to installation directory 
  SetOutPath $INSTDIR 
 
  ; Write the installation path into the registry 
  WriteRegStr HKLM "Software\EasyKit" "Install_Dir" "$INSTDIR" 
 
  ; Write the uninstall keys for Windows 
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\EasyKit" "DisplayName" "EasyKit" 
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\EasyKit" "UninstallString" '"$INSTDIR\uninstall.exe"' 
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\EasyKit" "NoModify" 1 
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\EasyKit" "NoRepair" 1 
  WriteUninstaller "$INSTDIR\uninstall.exe" 
SectionEnd 
 
Section "Desktop Shortcut" SecDesktop 
  CreateShortcut "$DESKTOP\EasyKit.lnk" "$INSTDIR\run_eskit.bat" "" "$INSTDIR\images\icon.ico" 0 
SectionEnd 
 
Section "Start Menu Shortcuts" SecStartMenu 
  CreateDirectory "$SMPROGRAMS\EasyKit" 
  CreateShortcut "$SMPROGRAMS\EasyKit\EasyKit.lnk" "$INSTDIR\run_eskit.bat" "" "$INSTDIR\images\icon.ico" 0 
  CreateShortcut "$SMPROGRAMS\EasyKit\Uninstall.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0 
SectionEnd 
 
; Section "Add to PATH" - Removed for CI compatibility 
 
; Descriptions 
!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN 
  !insertmacro MUI_DESCRIPTION_TEXT ${SecCore} "The core files required to run EasyKit." 
  !insertmacro MUI_DESCRIPTION_TEXT ${SecDesktop} "Create a shortcut to EasyKit on your desktop." 
  !insertmacro MUI_DESCRIPTION_TEXT ${SecStartMenu} "Create shortcuts to EasyKit in your Start Menu." 
  ; PATH section description removed 
!insertmacro MUI_FUNCTION_DESCRIPTION_END 
 
; Uninstaller Section 
Section "Uninstall" 
  ; Remove registry keys 
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\EasyKit" 
  DeleteRegKey HKLM "Software\EasyKit" 
 
  ; Remove main files 
  Delete $INSTDIR\run_eskit.bat 
  Delete $INSTDIR\README.md 
  Delete $INSTDIR\LICENSE 
  Delete $INSTDIR\MIGRATION_GUIDE.md 
  Delete $INSTDIR\uninstall.exe 
 
  ; Remove config files 
  Delete "$INSTDIR\config\config_eskit.bat" 
  Delete "$INSTDIR\config\logs\*.log" 
 
  ; Remove images 
  Delete "$INSTDIR\images\icon.ico" 
  Delete "$INSTDIR\images\icon.jpg" 
  Delete "$INSTDIR\images\installer-header.bmp" 
  Delete "$INSTDIR\images\installer-welcome.bmp" 
 
  ; Remove script files 
  Delete "$INSTDIR\scripts\core\*.bat" 
  Delete "$INSTDIR\scripts\build\*.bat" 
  Delete "$INSTDIR\scripts\tools\*.bat" 
 
  ; Remove log files 
  Delete "$INSTDIR\logs\*.log" 
 
  ; Remove directories 
  RMDir "$INSTDIR\config\logs" 
  RMDir "$INSTDIR\config" 
  RMDir "$INSTDIR\images" 
  RMDir "$INSTDIR\logs" 
  RMDir "$INSTDIR\scripts\core" 
  RMDir "$INSTDIR\scripts\build" 
  RMDir "$INSTDIR\scripts\tools" 
  RMDir "$INSTDIR\scripts" 
 
  ; Remove shortcuts 
  Delete "$DESKTOP\EasyKit.lnk" 
  Delete "$SMPROGRAMS\EasyKit\EasyKit.lnk" 
  Delete "$SMPROGRAMS\EasyKit\Uninstall.lnk" 
  RMDir "$SMPROGRAMS\EasyKit" 
 
  ; Remove installation directory 
  RMDir "$INSTDIR" 
 
  ; Remove from PATH variable - Removed for CI compatibility 
  ; EnVar::DeleteValue "PATH" "$INSTDIR" 
SectionEnd 
