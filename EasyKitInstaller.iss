; Inno Setup Script for EasyKit

#define MyAppName "EasyKit"
#define MyAppVersion "3.1.9"

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
; Include all Nuitka output files from dist\run_easykit.dist/ (EXE, DLLs, etc.)
Source: "dist\run_easykit.dist\*"; DestDir: "{app}"; Flags: recursesubdirs createallsubdirs ignoreversion
; Include docs and images
Source: "docs\*"; DestDir: "{app}\docs"; Flags: recursesubdirs createallsubdirs
Source: "images\*"; DestDir: "{app}\images"; Flags: recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\EasyKit_v{#MyAppVersion}.exe"
Name: "{userdesktop}\{#MyAppName}"; Filename: "{app}\EasyKit_v{#MyAppVersion}.exe"

[Registry]
Root: HKCU; SubKey: "Software\Microsoft\Windows\CurrentVersion\Uninstall\{#MyAppName}"; ValueType: string; ValueName: "DisplayName"; ValueData: "{#MyAppName}"; Flags: uninsdeletekey

[Run]
Filename: "{app}\EasyKit_v{#MyAppVersion}.exe"; Description: "Launch {#MyAppName}"; Flags: nowait postinstall skipifsilent
