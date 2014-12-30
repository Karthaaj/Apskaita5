; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

[Setup]
AppName=Apskaita5 update
AppID=Apskaita5MD
AppVerName=Apskaita5 v. 2013-10-22 update
AppPublisher=Marius Dagys
AppPublisherURL=http://www.tax.lt/
AppSupportURL=http://www.tax.lt/
AppUpdatesURL=http://www.tax.lt/
DefaultDirName={pf}\Apskaita5
DisableDirPage=yes
DefaultGroupName=Apskaita5
DisableProgramGroupPage=yes
OutputDir=E:\My Documents\Inno Output
OutputBaseFilename=Apskaita5_setup
MinVersion=0,4.0.1381sp4
Compression=lzma
SolidCompression=yes
CreateUninstallRegKey=no
UpdateUninstallLogAppName=no

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"

[Files]
Source: "E:\My Documents\My Projects\Apskaita5\Apskaita3\Apskaita\bin\x86\Release\Apskaita5.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\My Documents\My Projects\Apskaita5\Apskaita3\Apskaita\bin\x86\Release\CSLA.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\My Documents\My Projects\Apskaita5\Apskaita3\Apskaita\bin\x86\Release\AccDataAccessLayer.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\My Documents\My Projects\Apskaita5\Apskaita3\Apskaita\bin\x86\Release\AccControls.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\My Documents\My Projects\Apskaita5\Apskaita3\Apskaita\bin\x86\Release\AccWebCrawler.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\My Documents\My Projects\Apskaita5\Apskaita3\Apskaita\bin\x86\Release\ApskaitaObjects.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\My Documents\My Projects\Apskaita5\Apskaita3\Apskaita\bin\x86\Release\AccCommon.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\My Documents\My Projects\Apskaita5\Apskaita3\Apskaita\bin\x86\Release\InvoiceInfo.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\My Documents\My Projects\Apskaita5\Apskaita3\Apskaita\bin\x86\Release\sqlite3.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\My Documents\My Projects\Apskaita5\Apskaita3\Apskaita\bin\x86\Release\AccMigration.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\My Documents\My Projects\Apskaita5\Apskaita3\Apskaita\bin\x86\Release\Reports\*"; DestDir: "{app}\Reports"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "E:\My Documents\My Projects\Apskaita5\Apskaita3\Apskaita\bin\x86\Release\SqlDepositories\*"; DestDir: "{app}\SqlDepositories"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "E:\My Documents\My Projects\Apskaita5\Apskaita3\Apskaita\bin\x86\Release\DbStructure\*"; DestDir: "{app}\DbStructure"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "E:\My Documents\My Projects\Apskaita5\Apskaita3\Apskaita\bin\x86\Release\tsp.dat"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\My Documents\My Projects\Apskaita5\Apskaita3\Apskaita\bin\x86\Release\WorkTime.dat"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\My Documents\My Projects\Apskaita5\Apskaita3\Apskaita\bin\x86\Release\PublicHolidays.dat"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\My Documents\My Projects\Apskaita5\Apskaita3\Apskaita\bin\x86\Release\FinancialStatements_Short.str"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\My Documents\My Projects\Apskaita5\Apskaita3\Apskaita\bin\x86\Release\CommonCodes.dat"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\My Documents\My Projects\Apskaita5\Apskaita3\Apskaita\bin\x86\Release\WTClasses.dat"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\My Documents\My Projects\Apskaita5\Apskaita3\Apskaita\bin\x86\Release\LastUpdateA5.txt"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\My Documents\My Projects\Apskaita5\Apskaita3\Apskaita\bin\x86\Release\MySQL_accsecurity.sql"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\My Documents\My Projects\Apskaita5\Apskaita3\Apskaita\bin\x86\Release\FFData\FR0572(4).ffdata"; DestDir: "{app}\FFData"; Flags: onlyifdoesntexist
Source: "E:\My Documents\My Projects\Apskaita5\Apskaita3\Apskaita\bin\x86\Release\FFData\SAM-v03.ffdata"; DestDir: "{app}\FFData"; Flags: onlyifdoesntexist
Source: "E:\My Documents\My Projects\Apskaita5\Apskaita3\Apskaita\bin\x86\Release\MXFD\FR0572(4).mxfd"; DestDir: "{app}\MXFD"; Flags: onlyifdoesntexist
Source: "E:\My Documents\My Projects\Apskaita5\Apskaita3\Apskaita\bin\x86\Release\MXFD\SAM-v03.mxfd"; DestDir: "{app}\MXFD"; Flags: onlyifdoesntexist
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Run]
Filename: "{app}\Apskaita5.exe"; Description: "{cm:LaunchProgram,Apskaita5}"; Flags: nowait postinstall skipifsilent

[Code]
function InitializeSetup: Boolean;
begin
    if Not RegValueExists(HKEY_LOCAL_MACHINE,'Software\Microsoft\Windows\CurrentVersion\Uninstall\Apskaita5MD_is1', 'UninstallString') then begin
      MsgBox('The installation of Apskaita5 update requires Apskaita5 to be installed.' + #13#10 + 'Install Apskaita5 before installing this update.' + #13#10#13#10 + 'The setup of update will be terminated.', mbInformation, MB_OK);
      Result := False;
      end
    else begin
      Result := True;
      end
    end;
end.
