; Common shared logic for EasyKit Inno Setup scripts

#ifndef MyAppName
#define MyAppName "EasyKit"
#endif
#ifndef MyAppVersion
#define MyAppVersion "4.2.1"
#endif
#ifndef MyAppPublisher
#define MyAppPublisher "LoveDoLove"
#endif
#ifndef MyAppURL
#define MyAppURL "https://lovedolove.hidns.co/"
#endif
#ifndef MyAppExeName
#define MyAppExeName "EasyKit.exe"
#endif

[Setup]
AppId={{434B4C62-695E-4C3C-889C-1F745FB22A8C}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={#MyDefaultDir}
UninstallDisplayIcon={app}\{#MyAppExeName}
DisableProgramGroupPage=yes
LicenseFile=D:\Projects\CSharpProjects\EasyKit\LICENSE
;PrivilegesRequired=lowest
OutputBaseFilename=EasyKit-{#MyAppVersion}-{#MyArch}
SolidCompression=yes
WizardStyle=modern
SetupIconFile=D:\Projects\CSharpProjects\EasyKit\images\icon.ico

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]

[Files]
Source: "{#MyRegFile}"; DestDir: "{tmp}"; Flags: deleteafterinstall
Source: "{#MyPublishFolder}*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"

[Run]
Filename: "regedit.exe"; Parameters: "/s ""{tmp}\{#MyRegFileName}"""; StatusMsg: "Adding context menu..."; Flags: runhidden
