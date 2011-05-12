; CatEye_setup.nsi

;--------------------------------

!define PRODUCT_NAME "CatEye"
!define PRODUCT_VERSION "0.3"

;!include "MUI.nsh"
;!insertmacro MUI_PAGE_LICENSE "license.txt"

; The name of the installer
Name "CatEye setup / remove program"

; The file to write
OutFile "..\bin\Release\Setup.exe"

; The default installation directory
InstallDir $ProgramFiles\${PRODUCT_NAME}

; Request application privileges for Windows Vista
RequestExecutionLevel user

; Icons of installer and uninstaller
icon CatEyeSetup.ico
;UninstallIcon cateye-small.ico

SetCompressor /SOLID lzma
ShowInstDetails show
ShowUnInstDetails show

LicenseForceSelection checkbox
LicenseData "license.txt"

;--------------------------------

; Pages

Page license
Page directory
Page instfiles
;UninstPage uninstConfirm
;UninstPage instfiles

;--------------------------------

Function .onInit

  MessageBox MB_YESNO "This will install ${PRODUCT_NAME} version ${PRODUCT_VERSION} on yor computer. Do you wish to continue?" IDYES setup IDNO stop
  stop:
    abort
  setup:
    call IsDotNETInstalled
    ;call IsGtkInstalled
FunctionEnd



Function IsDotNETInstalled
  ; Check .NET version
  var /global dotnetset
  ReadRegDWORD $dotnetset HKLM 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v2.0.50727' Install
  IntCmp $dotnetset 1 go1 nonet
    nonet:
     ;MessageBox MB_OK|MB_ICONINFORMATION "${PRODUCT_NAME} requires that the .NET Framework 2.0 was installed." IDOK dotnetload
      ;  dotnetload:
         call DotNetDownload
  go1:
FunctionEnd



Function DotNetDownload
  MessageBox MB_YESNO "${PRODUCT_NAME} requires .NET Framework 2.0. Would You like downoad and install .NET Framework 2.0?" IDYES download IDNO notload
    notload:
      MessageBox MB_ICONINFORMATION "Installation aborted."
      abort
    download:
      MessageBox MB_ICONINFORMATION "Download and setup .NET Framework 2.0 on your system. After it run ${PRODUCT_NAME}_${PRODUCT_VERSION}-setup.exe again"
      Exec "$PROGRAMFILES\Internet Explorer\iexplore.exe http://www.microsoft.com/downloads/en/details.aspx?FamilyID=5b2c0358-915b-4eb5-9b1d-10e506da9d0f"
      abort
FunctionEnd



;Function IsGtkInstalled
; Check Gtk version
;var /global gtkset
;ReadRegStr HKEY_LOCAL_MACHINE SOFTWARE\Novell\CtkSharp\Version 
;ReadRegStr $gtkset HKLM 'SOFTWARE\Novell\GtkSharp\Version' @
;MessageBox MB_ICONINFORMATION $gtkset
;IntCmp $gtkset 1 go2 nogtk
;  nogtk:
    ;MessageBox MB_OK|MB_ICONINFORMATION "${PRODUCT_NAME} requires that the .NET Framework 2.0 was installed." IDOK dotnetload
    ;  dotnetload:
        ;call DotNetDownload
;go2:
;  SetOutPath $TEMP
;  File "gtk-sharp-2.12.10.win32.msi"
;  ExecWait "msiexec.exe /i $TEMP\gtk-sharp-2.12.10.win32.msi"
;  Delete "$TEMP\gtk-sharp-2.12.10.win32.msi"
  
;FunctionEnd

;--------------------------------

Section "Installer section"

  ; Set output path to the installation directory.
  SetOutPath $INSTDIR
  
  ; Put file there
  File ..\bin\Release\CatEye.exe
  File ..\bin\Release\CatEye.Core.dll
  File ..\bin\Release\default.cestage
  File ..\bin\Release\dcraw.exe
;  CreateShortCut $%userprofile%\Desktop\CatEye.lnk $INSTDIR\CatEye.exe
;  CreateDirectory "$%userprofile%\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\${PRODUCT_NAME}" ; for Vista
;  CreateShortCut "$%userprofile%\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\${PRODUCT_NAME}\CatEye.lnk" $INSTDIR\CatEye.exe ; for Vista
;  CreateShortCut "$%userprofile%\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\${PRODUCT_NAME}\Uninstall.lnk" $INSTDIR\Uninstall.exe ; for Vista
  WriteRegStr HKLM SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME} "DisplayName" "${PRODUCT_NAME}-${PRODUCT_VERSION}"
  WriteRegStr HKLM SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME} "UninstallString" "$INSTDIR\Uninstall.exe"
  WriteUninstaller $INSTDIR\Uninstall.exe


SectionEnd

;--------------------------------

Section "un.Installer section"
;  Delete $INSTDIR\dcraw.exe
;  Delete $INSTDIR\CatEye.exe
;  Delete $INSTDIR\Uninstall.exe
;  RMDir $INSTDIR
;  Delete $%userprofile%\Desktop\CatEye.lnk
;  Delete "$%userprofile%\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\CatEye\CatEye.lnk"
;  Delete "$%userprofile%\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\CatEye\Uninstall.lnk"
;  RMDir "$%userprofile%\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\CatEye"
  DeleteRegKey HKLM SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}\DisplayName
  DeleteRegKey HKLM SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}\UninstallString
  DeleteRegKey HKLM SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}
SectionEnd
