; CatEyeSetup-Debug.nsi

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
!define PKGDIR ""
!system "..\${PKGDIR}bin\${config}\GetVersion.exe"
!include "..\${PKGDIR}bin\${config}\Version.txt"


;Name and file
Name "${PRODUCT_NAME} ${VERSION_SHORT}"
OutFile "..\${PKGDIR}bin\${config}\${PRODUCT_NAME}-${VERSION_LONG}-setup.exe"

;Default installation folder
InstallDir $ProgramFiles\${PRODUCT_NAME}

;Request application privileges for Windows Vista
;RequestExecutionLevel user

; Properties of executable setup file
VIAddVersionKey "ProductName" "${PRODUCT_NAME}"
VIAddVersionKey "Comments" "Software for developing raw photos"
;VIAddVersionKey /LANG=${LANG_ENGLISH} "CompanyName" ""
;VIAddVersionKey /LANG=${LANG_ENGLISH} "LegalTrademarks" ""
VIAddVersionKey "LegalCopyright" "© Mizus Ilya & Lysakov Igor"
VIAddVersionKey "FileDescription" "Software for developing raw photos"
VIAddVersionKey "FileVersion" "${PRODUCT_VERSION}"
VIProductVersion "${PRODUCT_VERSION}"


;--------------------------------
;Interface Settings

BrandingText /TRIMLEFT " "

!define MUI_ABORTWARNING
; Icons of installer and uninstaller
!define MUI_ICON "${PKGDIR}res\CatEyeSetup.ico"
!define MUI_UNICON "${PKGDIR}res\CatEyeRemove.ico"

!define MUI_HEADERIMAGE
!define MUI_HEADERIMAGE_BITMAP "${PKGDIR}res\win.bmp"

;--------------------------------
;Pages

!define MUI_WELCOMEFINISHPAGE_BITMAP "${PKGDIR}res\orange.bmp"
!insertmacro MUI_PAGE_WELCOME

!define MUI_LICENSEPAGE_CHECKBOX
!insertmacro MUI_PAGE_LICENSE "${PKGDIR}licenses\license_cateye.txt"

;Page custom IsGtkInstalled

!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES

!define MUI_UNWELCOMEFINISHPAGE_BITMAP "${PKGDIR}res\orange-uninstall.bmp"
!insertmacro MUI_UNPAGE_WELCOME
!insertmacro MUI_UNPAGE_INSTFILES

!define MUI_FINISHPAGE_RUN "$INSTDIR\CatEye.exe"
!insertmacro MUI_PAGE_FINISH

;--------------------------------
;Languages

!insertmacro MUI_LANGUAGE "English"
;!insertmacro MUI_LANGUAGE "Russian"

;--------------------------------

;Function .onInit
;  !insertmacro MUI_LANGDLL_DISPLAY
;FunctionEnd

Function IsDotNETInstalled
  ; Check .NET version
  var /global dotnetset
  ReadRegDWORD $dotnetset HKLM 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v2.0.50727' Install
  IntCmp $dotnetset 1 go1 nonet
    nonet:
      call DotNetDownload
  go1:
FunctionEnd


Function DotNetDownload
  MessageBox MB_YESNO "${PRODUCT_NAME} requires .NET Framework 2.0. Would You like downoad and install .NET Framework 2.0?" IDYES download IDNO notload
    notload:
      MessageBox MB_ICONINFORMATION "Installation aborted."
      abort
    download:
      MessageBox MB_ICONINFORMATION "Download and setup .NET Framework 2.0 on your system from official Microsoft site. After it run ${PRODUCT_NAME}-${VERSION_LONG}-setup.exe again."
      ExecShell "" "http://www.microsoft.com/downloads/en/details.aspx?FamilyID=5b2c0358-915b-4eb5-9b1d-10e506da9d0f"
      quit
FunctionEnd


;--------------------------------

Section "Installer section"
  
  SetShellVarContext all
  
  call IsDotNETInstalled
  
  ; Set output path to the installation directory.
  SetOutPath $INSTDIR
  
  ; LicensesInstall
  CreateDirectory "$INSTDIR\licenses"
  SetOutPath "$INSTDIR\licenses"
  File /r "${PKGDIR}licenses\*.*"
  SetOutPath "$INSTDIR"


  ; GtkInstall
  File /r "${PKGDIR}gtk-embedded\*.*"
  File "${PKGDIR}gtk-postinstall.bat"
  Exec "${PKGDIR}gtk-postinstall.bat"
  Delete "${PKGDIR}gtk-postinstall.bat"


  ; Put files there
  File "..\${PKGDIR}bin\${config}\CatEye.exe"
  File "${PKGDIR}CatEye.exe.config"
  File "..\${PKGDIR}bin\${config}\CatEye.Core.dll"
  File "..\${PKGDIR}bin\${config}\CatEye.UI.Base.dll"
  File "..\${PKGDIR}bin\${config}\CatEye.Gtk.UI.Widgets.dll"
  File "..\${PKGDIR}bin\${config}\default.cestage"
  File "..\${PKGDIR}bin\${config}\dcraw.exe"
  
  CreateShortCut  $DESKTOP\${PRODUCT_NAME}.lnk $INSTDIR\CatEye.exe
  CreateDirectory $SMPROGRAMS\${PRODUCT_NAME}
  CreateShortCut  $SMPROGRAMS\${PRODUCT_NAME}\${PRODUCT_NAME}.lnk $INSTDIR\CatEye.exe
  CreateShortCut  $SMPROGRAMS\${PRODUCT_NAME}\Uninstall.lnk $INSTDIR\Uninstall.exe

  WriteRegStr HKLM SOFTWARE\${PRODUCT_NAME} "Version" "${PRODUCT_VERSION}"
  WriteRegStr HKLM SOFTWARE\${PRODUCT_NAME} "SetupDir" "$INSTDIR"
  
  WriteRegStr HKLM SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME} "DisplayName" "${PRODUCT_NAME} ${PRODUCT_VERSION}"
  WriteRegStr HKLM SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME} "DisplayIcon" "$INSTDIR\CatEye.exe"
  WriteRegStr HKLM SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME} "UninstallString" "$INSTDIR\Uninstall.exe"
  WriteUninstaller $INSTDIR\Uninstall.exe

SectionEnd

;--------------------------------

;Function un.onInit
;  !insertmacro MUI_UNGETLANGUAGE
;FunctionEnd


Function un.LicensesDel
  RmDir /r "$INSTDIR\licenses"
FunctionEnd


Function un.GtkDelete
  Delete "$INSTDIR\atksharpglue-2.dll"
  Delete "$INSTDIR\freetype6.dll"
  Delete "$INSTDIR\gdk-pixbuf-query-loaders.exe"
  Delete "$INSTDIR\gdksharpglue-2.dll"
  Delete "$INSTDIR\gladesharpglue-2.dll"
  Delete "$INSTDIR\glibsharpglue-2.dll"
  Delete "$INSTDIR\gspawn-win32-helper-console.exe"
  Delete "$INSTDIR\gspawn-win32-helper.exe"
  Delete "$INSTDIR\gtk-query-immodules-2.0.exe"
  Delete "$INSTDIR\gtksharpglue-2.dll"
  Delete "$INSTDIR\intl.dll"
  Delete "$INSTDIR\jpeg62.dll"
  Delete "$INSTDIR\libatk-1.0-0.dll"
  Delete "$INSTDIR\libcairo-2.dll"
  Delete "$INSTDIR\libexpat-1.dll"
  Delete "$INSTDIR\libexpat1.dll"
  Delete "$INSTDIR\libfontconfig-1.dll"
  Delete "$INSTDIR\libgailutil-18.dll"
  Delete "$INSTDIR\libgdk-win32-2.0-0.dll"
  Delete "$INSTDIR\libgdk_pixbuf-2.0-0.dll"
  Delete "$INSTDIR\libgio-2.0-0.dll"
  Delete "$INSTDIR\libglade-2.0-0.dll"
  Delete "$INSTDIR\libglib-2.0-0.dll"
  Delete "$INSTDIR\libgmodule-2.0-0.dll"
  Delete "$INSTDIR\libgobject-2.0-0.dll"
  Delete "$INSTDIR\libgthread-2.0-0.dll"
  Delete "$INSTDIR\libgtk-win32-2.0-0.dll"
  Delete "$INSTDIR\libjpeg7.dll"
  Delete "$INSTDIR\libpango-1.0-0.dll"
  Delete "$INSTDIR\libpangocairo-1.0-0.dll"
  Delete "$INSTDIR\libpangoft2-1.0-0.dll"
  Delete "$INSTDIR\libpangowin32-1.0-0.dll"
  Delete "$INSTDIR\libpng12-0.dll"
  Delete "$INSTDIR\librsvg-2-2.dll"
  Delete "$INSTDIR\libtiff3.dll"
  Delete "$INSTDIR\libtiffxx3.dll"
  Delete "$INSTDIR\libxml2-2.dll"
  Delete "$INSTDIR\MonoPosixHelper.dll"
  Delete "$INSTDIR\pango-querymodules.exe"
  Delete "$INSTDIR\pangosharpglue-2.dll"
  Delete "$INSTDIR\zlib1.dll"
  
  RmDir /r "$INSTDIR\etc"
  RmDir /r "$INSTDIR\lib"
  RmDir /r "$INSTDIR\share"
FunctionEnd

;--------------------------------
Section "un.Installer section"
  
  SetShellVarContext all
  
  call un.LicensesDel
  call un.GtkDelete
  
  Delete $INSTDIR\dcraw.exe
  Delete $INSTDIR\CatEye.Core.dll
  Delete $INSTDIR\CatEye.Gtk.UI.Widgets.dll
  Delete $INSTDIR\CatEye.UI.Base.dll
  Delete $INSTDIR\default.cestage
  Delete $INSTDIR\CatEye.exe.config
  Delete $INSTDIR\CatEye.exe
  Delete $INSTDIR\Uninstall.exe
  RMDir  $INSTDIR
  Delete $DESKTOP\${PRODUCT_NAME}.lnk
  Delete $SMPROGRAMS\${PRODUCT_NAME}\${PRODUCT_NAME}.lnk
  Delete $SMPROGRAMS\${PRODUCT_NAME}\Uninstall.lnk
  RmDir  $SMPROGRAMS\${PRODUCT_NAME}
  
  DeleteRegKey HKLM SOFTWARE\${PRODUCT_NAME}\Version
  DeleteRegKey HKLM SOFTWARE\${PRODUCT_NAME}\SetupDir
  DeleteRegKey HKLM SOFTWARE\${PRODUCT_NAME}
  
  DeleteRegKey HKLM SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}\DisplayIcon
  DeleteRegKey HKLM SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}\DisplayName
  DeleteRegKey HKLM SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}\UninstallString
  DeleteRegKey HKLM SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}
SectionEnd

