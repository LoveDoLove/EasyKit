; Common shared logic for EasyKit Inno Setup scripts

#ifndef MyAppName
#define MyAppName "EasyKit"
#endif
#ifndef MyAppVersion
#define MyAppVersion "4.2.3"
#endif
#ifndef MyAppPublisher
#define MyAppPublisher "LoveDoLove"
#endif
#ifndef MyAppURL
#define MyAppURL "https://github.com/LoveDoLove/EasyKit"
#endif
#ifndef MyAppExeName
#define MyAppExeName "EasyKit.exe"
#endif
#ifndef MyDefaultDir
#define MyDefaultDir "{commonpf64}\EasyKit"
#endif
#ifndef MyArch
#define MyArch "x64"
#endif
#ifndef MySrc
#define MySrc {src}
#endif

[Setup]
AppId={{434B4C62-695E-4C3C-889C-1F745FB22A8C}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={#MyDefaultDir}
UninstallDisplayIcon={app}\{#MyAppExeName}
DisableProgramGroupPage=yes
LicenseFile={#MySrc}\..\LICENSE
OutputBaseFilename=EasyKit-{#MyAppVersion}-{#MyArch}
SolidCompression=yes
WizardStyle=modern
SetupIconFile={#MySrc}\..\images\icon.ico

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]

[Files]
Source: "{#MySrc}\ContextMenu-win-{#MyArch}.reg"; DestDir: "{tmp}"; Flags: deleteafterinstall
Source: "{#MySrc}\..\publish\win-{#MyArch}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"

[Run]
Filename: "regedit.exe"; Parameters: "/s ""{tmp}\ContextMenu-win-{#MyArch}.reg"""; StatusMsg: "Adding context menu..."; Flags: runhidden
