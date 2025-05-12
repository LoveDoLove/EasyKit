; Inno Setup Script for EasyKit

#define MyAppName "EasyKit"
#define MyAppVersion "3.2.1"

[Setup]
AppName={#MyAppName}
AppVersion={#MyAppVersion}
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}
OutputBaseFilename=EasyKitSetup_{#MyAppVersion}
Compression=lzma
SolidCompression=yes
WizardStyle=modern

[Files]
; Include all Nuitka output files from dist\main.dist/ (EXE, DLLs, etc.)
Source: "dist\main.dist\*"; DestDir: "{app}"; Flags: recursesubdirs createallsubdirs ignoreversion
; Include docs and images
Source: "docs\*"; DestDir: "{app}\docs"; Flags: recursesubdirs createallsubdirs
Source: "images\*"; DestDir: "{app}\images"; Flags: recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\EasyKit_v{#MyAppVersion}.exe"
Name: "{userdesktop}\{#MyAppName}"; Filename: "{app}\EasyKit_v{#MyAppVersion}.exe"

[Registry]
Root: HKCU; SubKey: "Software\Microsoft\Windows\CurrentVersion\Uninstall\{#MyAppName}"; ValueType: string; ValueName: "DisplayName"; ValueData: "{#MyAppName}"; Flags: uninsdeletekey

; Add context menu for folders
Root: HKCR; Subkey: "Directory\shell\EasyKit"; ValueType: string; ValueName: ""; ValueData: "Open with EasyKit"; Flags: uninsdeletekey
Root: HKCR; SubKey: "Directory\shell\EasyKit"; ValueType: string; ValueName: "Icon"; ValueData: "{app}\images\icon.ico"; Flags: uninsdeletekey
Root: HKCR; Subkey: "Directory\shell\EasyKit\command"; ValueType: string; ValueName: ""; ValueData: """{app}\EasyKit_v{#MyAppVersion}.exe"" ""%1"""; Flags: uninsdeletekey

; Add context menu for all files
Root: HKCR; Subkey: "*\shell\EasyKit"; ValueType: string; ValueName: ""; ValueData: "Open with EasyKit"; Flags: uninsdeletekey
Root: HKCR; SubKey: "*\shell\EasyKit"; ValueType: string; ValueName: "Icon"; ValueData: "{app}\images\icon.ico"; Flags: uninsdeletekey
Root: HKCR; Subkey: "*\shell\EasyKit\command"; ValueType: string; ValueName: ""; ValueData: """{app}\EasyKit_v{#MyAppVersion}.exe"" ""%1"""; Flags: uninsdeletekey

[Run]
Filename: "{app}\EasyKit_v{#MyAppVersion}.exe"; Description: "Launch {#MyAppName}"; Flags: nowait postinstall skipifsilent
