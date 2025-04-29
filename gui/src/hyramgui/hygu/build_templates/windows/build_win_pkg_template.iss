; -- build_win_pkg.iss --
; Creates icon in the Programs folder of the Start Menu and creates a desktop icon.

[Setup]
WizardStyle=modern
AppName=APPNAME
AppVersion=1.1.0
AppCopyright=Copyright 2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).\nUnder the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.\n\nYou should have received a copy of the BSD License along with HELPR.
AppPublisher=Sandia National Laboratories
AppPublisherURL=https://www.example.com/

WizardSmallImageFile=icon55.bmp
WizardImageFile=icon410x797.bmp
DefaultDirName={autopf}\Sandia National Laboratories\APPNAME
UsePreviousAppDir=no
DisableDirPage=auto
LicenseFile=license_win.rtf
AllowNoIcons=yes

DefaultGroupName=Sandia National Laboratories\APPNAME
DisableProgramGroupPage=auto

UninstallDisplayIcon={app}\APPNAME.exe
Compression=lzma2
SolidCompression=yes
OutputDir=installer
SetupIconFile=icon.ico

[Files]
Source: "dist\APPNAME\*"; DestDir: "{app}"; Excludes: "*.pyc"; Flags: ignoreversion recursesubdirs;
; Source: "MyProg.chm"; DestDir: "{app}"
; Source: "Readme.txt"; DestDir: "{app}"; Flags: isreadme

[Icons]
Name: "{group}\APPNAME"; Filename: "{app}\APPNAME.exe"
Name: "{autodesktop}\APPNAME"; Filename: "{app}\APPNAME.exe"