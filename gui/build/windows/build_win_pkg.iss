; -- build_win_pkg.iss --
; Creates icon in the Programs folder of the Start Menu and creates a desktop icon.

[Setup]
WizardStyle=modern
ArchitecturesInstallIn64BitMode=x64compatible
AppName=HyRAM+
AppVersion=6.0.0
AppCopyright=Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).\n
AppPublisher=Sandia National Laboratories
AppPublisherURL=https://hyram.sandia.gov/
SignTool=Sectigo_250401 $f

WizardSmallImageFile=icon55.bmp
WizardImageFile=icon410x797.bmp
WizardSizePercent=140, 120

DefaultDirName={autopf}\Sandia National Laboratories\HyRAM
UsePreviousAppDir=yes
DisableDirPage=auto
LicenseFile=license_win.rtf
AllowNoIcons=yes

DefaultGroupName=Sandia National Laboratories\HyRAM
DisableProgramGroupPage=auto

UninstallDisplayIcon={app}\HyRAM.exe
Compression=lzma2
SolidCompression=yes
OutputDir=installer
SetupIconFile=icon.ico

[Files]
Source: "dist\HyRAM\*"; DestDir: "{app}"; Excludes: "*.pyc"; Flags: ignoreversion recursesubdirs;
; Source: "MyProg.chm"; DestDir: "{app}"
; Source: "Readme.txt"; DestDir: "{app}"; Flags: isreadme

[Icons]
Name: "{group}\HyRAM"; Filename: "{app}\HyRAM.exe"
Name: "{autodesktop}\HyRAM"; Filename: "{app}\HyRAM.exe"