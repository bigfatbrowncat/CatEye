; CatEyeSetup-Release.nsi

;--------------------------------
; Common settings
SetCompressor /SOLID lzma
ShowInstDetails show
ShowUnInstDetails show

;--------------------------------
;Include Modern UI
!include "MUI2.nsh"

;--------------------------------
;General

!define PRODUCT_NAME "CatEye"
!define PRODUCT_VERSION "0.3"
!define PKGDIR ""
!define config "Release"

;Name and file
Name "${PRODUCT_NAME} ${PRODUCT_VERSION}"
;OutFile "..\packages\${PRODUCT_NAME}_${PRODUCT_VERSION}-setup.exe"
OutFile "..\${PKGDIR}bin\${config}\Setup.exe"

;Default installation folder
InstallDir $ProgramFiles\${PRODUCT_NAME}

;Request application privileges for Windows Vista
RequestExecutionLevel user

;--------------------------------
;Interface Settings

!define MUI_ABORTWARNING
; Icons of installer and uninstaller
!define MUI_ICON "${PKGDIR}res\CatEyeSetup.ico"
!define MUI_UNICON "${PKGDIR}res\CatEyeRemove.ico"

;--------------------------------
;Pages

!define MUI_WELCOMEFINISHPAGE_BITMAP "${PKGDIR}res\orange.bmp"
!insertmacro MUI_PAGE_WELCOME

!define MUI_LICENSEPAGE_CHECKBOX
!insertmacro MUI_PAGE_LICENSE "${PKGDIR}license.txt"

;Page custom IsGtkInstalled

!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES

!define MUI_UNWELCOMEFINISHPAGE_BITMAP "${PKGDIR}res\orange-uninstall.bmp"
!insertmacro MUI_UNPAGE_WELCOME
!insertmacro MUI_UNPAGE_INSTFILES

;--------------------------------
;Languages

!insertmacro MUI_LANGUAGE "English"
!insertmacro MUI_LANGUAGE "Russian"

;--------------------------------

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
      MessageBox MB_ICONINFORMATION "Download and setup .NET Framework 2.0 on your system from official Microsoft site. After it run ${PRODUCT_NAME}_${PRODUCT_VERSION}-setup.exe again"
      ;Exec "$PROGRAMFILES\Internet Explorer\iexplore.exe http://www.microsoft.com/downloads/en/details.aspx?FamilyID=5b2c0358-915b-4eb5-9b1d-10e506da9d0f"
      ExecShell "" "http://www.microsoft.com/downloads/en/details.aspx?FamilyID=5b2c0358-915b-4eb5-9b1d-10e506da9d0f"
      abort
FunctionEnd


Function IsGtkInstalled
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
  MessageBox MB_ICONINFORMATION "${PRODUCT_NAME} requires GTK# v2.12.10. GTK# would be installed on Your system."
  
    SetOutPath $TEMP
    File "${PKGDIR}gtk-sharp-2.12.10.win32.msi"
    ExecWait "msiexec.exe /i $TEMP\gtk-sharp-2.12.10.win32.msi"
    Delete "$TEMP\gtk-sharp-2.12.10.win32.msi"
  
FunctionEnd

;--------------------------------

Section "Installer section"
  
  SetShellVarContext all
  
  call IsDotNETInstalled
  call IsGtkInstalled
  
  ; Set output path to the installation directory.
  SetOutPath $INSTDIR
  
  ; Put files there
  File "..\${PKGDIR}bin\${config}\CatEye.exe"
  File "..\${PKGDIR}bin\${config}\CatEye.Core.dll"
  File "..\${PKGDIR}bin\${config}\CatEye.Widgets.dll"
  File "..\${PKGDIR}bin\${config}\default.cestage"
  File "${PKGDIR}dcraw.exe"
  
  CreateShortCut  $DESKTOP\${PRODUCT_NAME}.lnk $INSTDIR\CatEye.exe
  CreateDirectory $SMPROGRAMS\${PRODUCT_NAME}
  CreateShortCut  $SMPROGRAMS\${PRODUCT_NAME}\${PRODUCT_NAME}.lnk $INSTDIR\CatEye.exe
  CreateShortCut  $SMPROGRAMS\${PRODUCT_NAME}\Uninstall.lnk $INSTDIR\Uninstall.exe
  ;CreateShortCut $%userprofile%\Desktop\${PRODUCT_NAME}.lnk $INSTDIR\CatEye.exe
  ; Check windows version
  ;var /global winver
  ;ReadRegStr $winver HKLM "SOFTWARE\Microsoft\Windows NT\CurrentVersion" CurrentVersion
  ;MessageBox MB_ICONINFORMATION $winver
  ;IntCmp $winver "6.0" eq lt gt
  ;  eq:
  ;    ;MessageBox MB_ICONINFORMATION "eq"
  ;    ; system Vista
  ;    CreateDirectory "$%userprofile%\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\${PRODUCT_NAME}"
  ;    CreateShortCut "$%userprofile%\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\${PRODUCT_NAME}\CatEye.lnk" $INSTDIR\CatEye.exe
  ;    CreateShortCut "$%userprofile%\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\${PRODUCT_NAME}\Uninstall.lnk" $INSTDIR\Uninstall.exe
  ;    goto ok
  ;  lt:
      ;MessageBox MB_ICONINFORMATION "lt"
      ; XP or earlier
      ; need path
  ;    goto ok
  ;  gt:
      ;MessageBox MB_ICONINFORMATION "gt"
      ; maybe Seven
      ; Vista code
  ;    CreateDirectory "$%userprofile%\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\${PRODUCT_NAME}"
  ;    CreateShortCut "$%userprofile%\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\${PRODUCT_NAME}\CatEye.lnk" $INSTDIR\CatEye.exe
  ;    CreateShortCut "$%userprofile%\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\${PRODUCT_NAME}\Uninstall.lnk" $INSTDIR\Uninstall.exe
  ;    goto ok
  ;  ok:
  
  WriteRegStr HKLM SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME} "DisplayName" "${PRODUCT_NAME} ${PRODUCT_VERSION}"
  WriteRegStr HKLM SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME} "UninstallString" "$INSTDIR\Uninstall.exe"
  WriteUninstaller $INSTDIR\Uninstall.exe

SectionEnd


Function .onInit
  !insertmacro MUI_LANGDLL_DISPLAY
FunctionEnd

;--------------------------------

Section "un.Installer section"
  
  SetShellVarContext all
  
  Delete $INSTDIR\dcraw.exe
  
  
  Delete $INSTDIR\CatEye.Core.dll
  Delete $INSTDIR\CatEye.Widgets.dll
  Delete $INSTDIR\default.cestage
  Delete $INSTDIR\CatEye.exe
  Delete $INSTDIR\Uninstall.exe
  RMDir  $INSTDIR
  Delete $DESKTOP\${PRODUCT_NAME}.lnk
  Delete $SMPROGRAMS\${PRODUCT_NAME}\${PRODUCT_NAME}.lnk
  Delete $SMPROGRAMS\${PRODUCT_NAME}\Uninstall.lnk
  RmDir  $SMPROGRAMS\${PRODUCT_NAME}
  
  ; Check windows version
  ;var /global unwinver
  ;ReadRegStr $unwinver HKLM "SOFTWARE\Microsoft\Windows NT\CurrentVersion" CurrentVersion
  ;MessageBox MB_ICONINFORMATION $winver
  ;IntCmp $unwinver "6.0" eq lt gt
  ;  eq:
      ;MessageBox MB_ICONINFORMATION "eq"
      ; system Vista
  ;    Delete "$%userprofile%\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\${PRODUCT_NAME}\CatEye.lnk"
  ;    Delete "$%userprofile%\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\${PRODUCT_NAME}\Uninstall.lnk"
  ;    RMDir /r "$%userprofile%\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\${PRODUCT_NAME}"
  ;    goto ok
  ;  lt:
      ;MessageBox MB_ICONINFORMATION "lt"
      ; XP or earlier
      ; need path
  ;    goto ok
  ;  gt:
      ;MessageBox MB_ICONINFORMATION "gt"
      ; maybe Seven
      ; Vista code
  ;    Delete "$%userprofile%\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\${PRODUCT_NAME}\CatEye.lnk"
  ;    Delete "$%userprofile%\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\${PRODUCT_NAME}\Uninstall.lnk"
  ;    RMDir /r "$%userprofile%\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\${PRODUCT_NAME}"
  ;    goto ok
  ;  ok:
    
  DeleteRegKey HKLM SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}\DisplayName
  DeleteRegKey HKLM SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}\UninstallString
  DeleteRegKey HKLM SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}
SectionEnd


Function un.onInit
  !insertmacro MUI_UNGETLANGUAGE
FunctionEnd

