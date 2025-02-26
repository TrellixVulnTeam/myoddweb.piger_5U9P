#define APP_NAME "Myoddweb Action Monitor (Piger)";
#define APP_PUBLISHER "myoddweb.com";
#define APP_URL "http://www.myoddweb.com";
#define APP_DIR "{pf}\myoddweb\piger";
#define APP_INCLUDE "..\includes\";

; The source files.
#define APP_SOURCE86 "..\Output\Release\x86\";
#define APP_SOURCE64 "..\Output\Release\x64\";

; set the version number based on the classifier version number.
#define APP_VERSION GetFileVersion( APP_SOURCE86 + 'ActionMonitor.exe' );

; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{E0B5F430-234F-47AE-912F-089E79259B3D}
AppVersion={#APP_VERSION}
VersionInfoVersion={#APP_VERSION}
AppName=Piger
AppVerName=Piger Alpha {#APP_VERSION}
AppPublisher={#APP_PUBLISHER}
AppPublisherURL={#APP_URL}
AppSupportURL={#APP_URL}
AppUpdatesURL={#APP_URL}
DefaultDirName={pf}\myoddweb\Piger
DefaultGroupName=Piger
OutputDir=/
OutputBaseFilename=piger-{#APP_VERSION}
Compression=lzma
SolidCompression=true

; "ArchitecturesInstallIn64BitMode=x64" requests that the install be
; done in "64-bit mode" on x64, meaning it should use the native
; 64-bit Program Files directory and the 64-bit view of the registry.
ArchitecturesInstallIn64BitMode=x64
ArchitecturesAllowed=x86 x64

; for now make sure that only admins can install this
; this is for the registry entry.
PrivilegesRequired=admin
AppMutex=MyOddweb_com_ActionMonitor

[Languages]
Name: english; MessagesFile: compiler:Default.isl

[Registry]
Root: HKLM; Subkey: SOFTWARE\Microsoft\Windows\CurrentVersion\Run; ValueType: string; ValueName: Piger; ValueData: """{app}\ActionMonitor.exe"""; Flags: uninsdeletevalue

[Tasks]
; Plugins
Name: pluginloader; Description: "Create a 'Learn/Unlearn' action to learn new simple actions."; GroupDescription: "Plugins";
Name: pluginapppaths; Description: "Parse all the common applications on your system and create actions on the fly."; GroupDescription: "Plugins";
Name: pluginontop; Description: "Create 'OnTop' command to keep any window as the topmost window, (like vlc/winamp etc...)."; GroupDescription: "Plugins";
Name: plugindolly; Description: "Hello Dolly sample plugin (c++)"; GroupDescription: "Plugins"; Flags: unchecked
Name: plugindollynet; Description: "Hello Dolly sample plugin (.NET)"; GroupDescription: "Plugins"; Flags: unchecked
; scripts
; make sure that this remains item #5 as the [code] bellow disables it.
Name: pluginpowershell3; Description: "Powershell 3 plugins"; GroupDescription: "Scripts";
Name: plugincsharp; Description: "C# Sharp plugins"; GroupDescription: "Scripts";

[InstallDelete]
; remove old files that the user might no longer want
; we remove them to make sure that they don't cause some weird conflict
; and that they are not present if the user does not want them.
Type: files; Name: "{userappdata}\myoddweb\ActionMonitor\RootCommands\__in\LoaderPlugin.amp"
Type: files; Name: "{userappdata}\myoddweb\ActionMonitor\RootCommands\__in\AppPaths.amp"
Type: files; Name: "{userappdata}\myoddweb\ActionMonitor\RootCommands\__in\Dolly.amp"
Type: files; Name: "{userappdata}\myoddweb\ActionMonitor\RootCommands\__in\OnTop.amp"
Type: files; Name: "{userappdata}\myoddweb\ActionMonitor\RootCommands\__in\SamplePlugin.amp"

; remove old python file as things are now moved to {app}\python\ folder.
Type: files; Name: "{app}\python35.dll"
Type: files; Name: "{app}\python35.zip"
Type: files; Name: "{app}\python6435.dll"

Type: files; Name: "{app}\python36.dll"
Type: files; Name: "{app}\python36.zip"
Type: files; Name: "{app}\python6436.dll"

; remove the powershell dll, in case the user removed powershell, (or something weird).
; that way we will not try and execute something that might not exist.
Type: files; Name: "{app}\AMPowerShellCmdLets.dll"

[Files]
;
; All the plugins
; Remember to update the task list as well as the InstallDelete above!
;
; x86 plugins
Source: {#APP_SOURCE86}LoaderPlugin.amp; DestDir: {userappdata}\myoddweb\ActionMonitor\RootCommands\__in\; Flags: ignoreversion; Check: not IsWin64 and IsTaskSelected('pluginloader')
Source: {#APP_SOURCE86}AppPaths.amp; DestDir: {userappdata}\myoddweb\ActionMonitor\RootCommands\__in\; Flags: ignoreversion; Check: not IsWin64 and IsTaskSelected('pluginapppaths')
Source: {#APP_SOURCE86}Dolly.amp; DestDir: {userappdata}\myoddweb\ActionMonitor\RootCommands\__in\; Flags: ignoreversion; Check: not IsWin64 and IsTaskSelected('plugindolly')
Source: {#APP_SOURCE86}OnTop.amp; DestDir: {userappdata}\myoddweb\ActionMonitor\RootCommands\__in\; Flags: ignoreversion; Check: not IsWin64 and IsTaskSelected('pluginontop')

; x64 plugins
Source: {#APP_SOURCE64}LoaderPlugin64.amp; DestName:LoaderPlugin.amp; DestDir: {userappdata}\myoddweb\ActionMonitor\RootCommands\__in\; Flags: ignoreversion; Check: IsWin64 and IsTaskSelected('pluginloader')
Source: {#APP_SOURCE64}AppPaths64.amp; DestName:AppPaths.amp; DestDir: {userappdata}\myoddweb\ActionMonitor\RootCommands\__in\; Flags: ignoreversion; Check: IsWin64 and IsTaskSelected('pluginapppaths')
Source: {#APP_SOURCE64}Dolly64.amp; DestName:Dolly.amp; DestDir: {userappdata}\myoddweb\ActionMonitor\RootCommands\__in\; Flags: ignoreversion; Check: IsWin64 and IsTaskSelected('plugindolly')
Source: {#APP_SOURCE64}OnTop64.amp; DestName:OnTop.amp; DestDir: {userappdata}\myoddweb\ActionMonitor\RootCommands\__in\; Flags: ignoreversion; Check: IsWin64 and IsTaskSelected('pluginontop')

; common
Source: {#APP_SOURCE64}Dolly.NET.amp-net; DestDir: {userappdata}\myoddweb\ActionMonitor\RootCommands\__in\; Flags: ignoreversion; Check: IsTaskSelected('plugindollynet')

; any commands we might want to add.
Source: .\RootCommands\*; DestDir: {userappdata}\myoddweb\ActionMonitor\RootCommands\; Flags: recursesubdirs createallsubdirs
Source: .\RootCommandsPS\*; DestDir: {userappdata}\myoddweb\ActionMonitor\RootCommands\; Flags: recursesubdirs createallsubdirs; Check: "IsTaskSelected('pluginpowershell3') and IsPowershell3Installed"
Source: .\RootCommandsCS\*; DestDir: {userappdata}\myoddweb\ActionMonitor\RootCommands\; Flags: recursesubdirs createallsubdirs; Check: "IsTaskSelected('plugincsharp') and IsPowershell3Installed"

;
; All the plugins
;

Source: {#APP_INCLUDE}vc_redist.x86.exe; DestDir: {tmp}; Flags: deleteafterinstall; Check: "not IsWin64"
Source: {#APP_INCLUDE}vc_redist.x64.exe; DestDir: {tmp}; Flags: deleteafterinstall; Check: IsWin64
;
; be sure to build the latest exe.
; select "Release Any CPU" to ensure that both x64 and x86 are built.
;
; x86 App
Source: {#APP_SOURCE86}ActionMonitor.exe; DestDir: {app}; Flags: ignoreversion; Check: "not IsWin64"
Source: {#APP_SOURCE86}hook.dll; DestDir: {app}; Flags: ignoreversion; Check: "not IsWin64"
Source: {#APP_SOURCE86}python37.dll; DestDir: {app}; Flags: ignoreversion; Check: "not IsWin64"

; x64 App
Source: {#APP_SOURCE64}ActionMonitor64.exe; DestName:ActionMonitor.exe; DestDir: {app}; Flags: ignoreversion; Check: IsWin64
Source: {#APP_SOURCE64}hook64.dll; DestDir: {app}; Flags: ignoreversion; Check: IsWin64
Source: {#APP_SOURCE64}python37.dll; DestDir: {app}; Flags: ignoreversion; Check: IsWin64

; common
Source: {#APP_INCLUDE}python86\*.*; DestDir: {app}\python\; Flags: recursesubdirs createallsubdirs; Check: "not IsWin64"
Source: {#APP_INCLUDE}python64\*.*; DestDir: {app}\python\; Flags: recursesubdirs createallsubdirs; Check: IsWin64

; the .NET files are always built in the x64 folder _and_ the x86
; So we can use either folders, they are both valid
Source: {#APP_SOURCE64}ActionMonitor.dll; DestDir: {app}; Flags: ignoreversion;
Source: {#APP_SOURCE64}ActionMonitor.Interfaces.dll; DestDir: {app}; Flags: ignoreversion;
Source: {#APP_SOURCE64}myoddweb.commandlineparser.dll; DestDir: {app}; Flags: ignoreversion;
Source: {#APP_SOURCE64}ActionMonitor.Shell.exe; DestDir: {app}; Flags: ignoreversion;

; default config
Source: profile.xml; DestDir: {userappdata}\myoddweb\ActionMonitor\; Flags: onlyifdoesntexist

[Run]
Filename: {tmp}\vc_redist.x86.exe; Parameters: "/install /passive /quiet /norestart"; StatusMsg: Installing VC++ 2017 Redistributables...(Please wait a few minutes); Check: "not IsWin64"
Filename: {tmp}\vc_redist.x64.exe; Parameters: "/install /passive /quiet /norestart"; StatusMsg: Installing VC++ 2017 Redistributables...(Please wait a few minutes); Check: IsWin64

Filename: {app}\ActionMonitor.exe; Description: {cm:LaunchProgram,Piger}; Flags: nowait postinstall

[Icons]
Name: {group}\Piger; Filename: {app}\ActionMonitor.exe
Name: {group}\{cm:ProgramOnTheWeb,Piger}; Filename: {#APP_URL}
Name: {group}\{cm:UninstallProgram,Piger}; Filename: {uninstallexe}
Name: {group}\{cm:UninstallProgram, Piger}; Filename: {uninstallexe}

[Code]
function IsPowershell3Installed(): Boolean;
var
  Install: Cardinal;
begin
  Result := false;
  if RegQueryDWordValue(HKEY_LOCAL_MACHINE, 'SOFTWARE\Microsoft\PowerShell\3', 'Install', Install) then
  begin
    // Successfully read the value
    Result := (1 = Install )
    Log('The Install is: ' + IntToStr(Install));
  end;
end;

// Disable Tasks
procedure CurPageChanged(CurPageID: Integer);
var
  Index: Integer;
begin
  // if we have entered the tasks selection page, disable the specified Tasks.
  if CurPageID = wpSelectTasks then
  begin
    // this number is the powershell task number! don't change it!
    Index := 5
    WizardForm.TasksList.ItemEnabled[Index] := IsPowershell3Installed();
  end;

end;
// Disable Tasks - END
