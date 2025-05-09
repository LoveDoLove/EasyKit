!define PRODUCT_NAME "EasyKit"
!define PRODUCT_VERSION "1.2.8" ; You might want to update this dynamically
!define PRODUCT_PUBLISHER "Your Company Name"
!define PRODUCT_WEB_SITE "https://example.com"
!define PRODUCT_DIR_REGKEY "Software\Microsoft\Windows\CurrentVersion\App Paths\EasyKit.exe"
!define PRODUCT_UNINST_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}"
!define PRODUCT_UNINST_ROOT_KEY "HKLM"

; MUI 2 defines
!include "MUI2.nsh"

; MUI Settings
!define MUI_ABORTWARNING
!define MUI_ICON "${NSISDIR}\Contrib\Graphics\Icons\modern-install.ico"
!define MUI_UNICON "${NSISDIR}\Contrib\Graphics\Icons\modern-uninstall.ico"

; Language Selection Dialog Settings
!define MUI_LANGDLL_REGISTRY_ROOT "${PRODUCT_UNINST_ROOT_KEY}"
!define MUI_LANGDLL_REGISTRY_KEY "${PRODUCT_UNINST_KEY}"
!define MUI_LANGDLL_REGISTRY_VALUENAME "NSIS:Language"

; Welcome page
!insertmacro MUI_PAGE_WELCOME
; License page
; !insertmacro MUI_PAGE_LICENSE "path\to\license.txt"
; Directory page
!insertmacro MUI_PAGE_DIRECTORY
; Instfiles page
!insertmacro MUI_PAGE_INSTFILES
; Finish page
!define MUI_FINISHPAGE_RUN "$INSTDIR\run_eskit.bat"
!insertmacro MUI_PAGE_FINISH

; Uninstaller pages
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES

; Language files
!insertmacro MUI_LANGUAGE "English"

; Installer attributes
Name "${PRODUCT_NAME} ${PRODUCT_VERSION}"
OutFile "build\EasyKit_Setup.exe"
InstallDir "$PROGRAMFILES\${PRODUCT_NAME}"
InstallDirRegKey HKLM "${PRODUCT_DIR_REGKEY}" ""
ShowInstDetails show
ShowUnInstDetails show

Section "MainSection" SEC01
  SetOutPath "$INSTDIR"
  SetOverwrite ifnewer
  ; Files to install
  File "run_eskit.bat"
  File "README.md"
  File "LICENSE"
  File /r "scripts\"
  File /r "images\"
  File /r "config\"
  ; Add other files and folders here

  ; Store installation folder
  WriteRegStr HKLM "${PRODUCT_DIR_REGKEY}" "" "$INSTDIR\run_eskit.bat"
  ; Create uninstaller
  WriteUninstaller "$INSTDIR\Uninstall.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayName" "$(^Name)"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "UninstallString" "$INSTDIR\Uninstall.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayIcon" "$INSTDIR\images\icon.ico"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayVersion" "${PRODUCT_VERSION}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "URLInfoAbout" "${PRODUCT_WEB_SITE}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "Publisher" "${PRODUCT_PUBLISHER}"
SectionEnd

Section "Uninstall" UNSEC01
  Delete "$INSTDIR\*.*" ; Adjust as needed
  Delete /REBOOTOK "$INSTDIR\Uninstall.exe"
  RMDir /r "$INSTDIR\scripts"
  RMDir /r "$INSTDIR\images"
  RMDir /r "$INSTDIR\config\logs" ; remove logs subdir first
  RMDir /r "$INSTDIR\config"
  RMDir /REBOOTOK "$INSTDIR"

  DeleteRegKey ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}"
  DeleteRegKey HKLM "${PRODUCT_DIR_REGKEY}"
  SetAutoClose true
SectionEnd
