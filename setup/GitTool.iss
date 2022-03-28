; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define GitToolAppName "GitTool"
#ifdef CiAppVersion
  #define GitToolAppVersion CiAppVersion
#endif
#ifndef CiAppVersion
  #define GitToolAppVersion "0.1-localdev"
#endif
#define GitToolAppPublisher "CreativeCoders"
#define GitToolAppURL "https://github.com/CreativeCodersTeam/GitTools"
#define GitToolAppExeName "gt.exe"

[Setup]
AppId={{E302F521-44A5-4CB2-9ABC-A7020D3E6B5A}
AppName={#GitToolAppName}
AppVersion={#GitToolAppVersion}
AppVerName={#GitToolAppName} {#GitToolAppVersion}
AppPublisher={#GitToolAppPublisher}
AppPublisherURL={#GitToolAppURL}
AppSupportURL={#GitToolAppURL}
AppUpdatesURL={#GitToolAppURL}
DefaultDirName={autopf64}\{#GitToolAppName}
DefaultGroupName={#GitToolAppName}
DisableProgramGroupPage=yes
LicenseFile=..\LICENSE
; Remove the following line to run in administrative install mode (install for all users.)
PrivilegesRequired=lowest
OutputBaseFilename=gittool_setup_{#GitToolAppVersion}
OutputDir=..\.artifacts\setups
Compression=lzma
SolidCompression=yes
WizardStyle=modern

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: ".\PathMgr.dll";  DestDir: "{app}"; Flags: uninsneveruninstall
Source: "..\.artifacts\GitTool.Cli.Win64\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Tasks]
Name: "AddPathToEnv"; Description: "Add to PATH environment variable"

[Code]
const
  MODIFY_PATH_TASK_NAME = 'AddPathToEnv';  // Specify name of task
var
  PathIsModified: Boolean;  // Cache task selection from previous installs
// Import AddDirToPath() at setup time ('files:' prefix)
function DLLAddDirToPath(DirName: string; PathType, AddType: DWORD): DWORD;
  external 'AddDirToPath@files:PathMgr.dll stdcall setuponly';
// Import RemoveDirFromPath() at uninstall time ('{app}\' prefix)
function DLLRemoveDirFromPath(DirName: string; PathType: DWORD): DWORD;
  external 'RemoveDirFromPath@{app}\PathMgr.dll stdcall uninstallonly';
// Wrapper for AddDirToPath() DLL function
function AddDirToPath(const DirName: string): DWORD;
var
  PathType, AddType: DWORD;
begin
  // PathType = 0 - use system Path
  // PathType = 1 - use user Path
  // AddType = 0 - add to end of Path
  // AddType = 1 - add to beginning of Path
  if IsAdminInstallMode() then
    PathType := 0
  else
    PathType := 1;
  AddType := 0;
  result := DLLAddDirToPath(DirName, PathType, AddType);
end;
// Wrapper for RemoveDirFromPath() DLL function
function RemoveDirFromPath(const DirName: string): DWORD;
var
  PathType: DWORD;
begin
  // PathType = 0 - use system Path
  // PathType = 1 - use user Path
  if IsAdminInstallMode() then
    PathType := 0
  else
    PathType := 1;
  result := DLLRemoveDirFromPath(DirName, PathType);
end;
procedure RegisterPreviousData(PreviousDataKey: Integer);
begin
  // Store previous or current task selection as custom user setting
  if PathIsModified or WizardIsTaskSelected(MODIFY_PATH_TASK_NAME) then
    SetPreviousData(PreviousDataKey, MODIFY_PATH_TASK_NAME, 'true');
end;
function InitializeSetup(): Boolean;
begin
  result := true;
  // Was task selected during a previous install?
  PathIsModified := GetPreviousData(MODIFY_PATH_TASK_NAME, '') = 'true';
end;
function InitializeUninstall(): Boolean;
begin
  result := true;
  // Was task selected during a previous install?
  PathIsModified := GetPreviousData(MODIFY_PATH_TASK_NAME, '') = 'true';
end;
procedure CurStepChanged(CurStep: TSetupStep);
begin
  if CurStep = ssPostInstall then
  begin
    // Add app directory to Path at post-install step if task selected
    if PathIsModified or WizardIsTaskSelected(MODIFY_PATH_TASK_NAME) then
      AddDirToPath(ExpandConstant('{app}'));
  end;
end;
procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
begin
  if CurUninstallStep = usUninstall then
  begin
    // Remove app directory from path during uninstall if task was selected;
    // use variable because we can't use WizardIsTaskSelected() at uninstall
    if PathIsModified then
      RemoveDirFromPath(ExpandConstant('{app}'));
  end;
end;
procedure DeinitializeUninstall();
begin
  // Unload and delete PathMgr.dll and remove app dir when uninstalling
  UnloadDLL(ExpandConstant('{app}\PathMgr.dll'));
  DeleteFile(ExpandConstant('{app}\PathMgr.dll'));
  RemoveDir(ExpandConstant('{app}'));
end;
