; CatEyeSetup program for Windows

;--------------------------------
; Common settings
SetCompressor /SOLID lzma
;ShowInstDetails show
;ShowUnInstDetails show

;--------------------------------

;General

!define PRODUCT_NAME "CatEye"
!define PKGDIR ""
!system "..\${PKGDIR}bin\${config}\GetVersion.exe"
!include "..\${PKGDIR}bin\${config}\Version.txt"

;Include files
!include "MUI2.nsh"         ; Modern UI
!include "DotNet.nsh"       ; my file for .NetFramework
!include "Extensions.nsh"   ; my file for extensions registration

;Name and file
Name "${PRODUCT_NAME} ${VERSION_SHORT}"
OutFile "..\${PKGDIR}bin\${config}\${PRODUCT_NAME}-${VERSION_LONG}-setup.exe"

;Default installation folder
InstallDir $ProgramFiles\${PRODUCT_NAME}

;Request application privileges for Windows Vista
RequestExecutionLevel admin

; Properties of executable setup file
VIAddVersionKey "ProductName" "${PRODUCT_NAME}"
VIAddVersionKey "Comments" "Software for developing raw photos"
;VIAddVersionKey /LANG=${LANG_ENGLISH} "CompanyName" ""
;VIAddVersionKey /LANG=${LANG_ENGLISH} "LegalTrademarks" ""
VIAddVersionKey "CompanyName" ""
VIAddVersionKey "LegalTrademarks" ""
VIAddVersionKey "LegalCopyright" "© Mizus Ilya & Lysakov Igor"
VIAddVersionKey "FileDescription" "Software for developing raw photos"
VIAddVersionKey "FileVersion" "${PRODUCT_VERSION}"
VIProductVersion "${PRODUCT_VERSION}"

; Variables
var /global winver

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
;!insertmacro MUI_PAGE_LICENSE "${PKGDIR}licenses\license_cateye_en_US.txt"
!insertmacro MUI_PAGE_LICENSE $(license)

!insertmacro MUI_PAGE_DOTNET        ; my page for .NetFramework warning
!insertmacro MUI_PAGE_EXTENSIONS    ; my page for extensions registration

!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES

!define MUI_FINISHPAGE_RUN "$INSTDIR\CatEye.exe"
!insertmacro MUI_PAGE_FINISH


!define MUI_UNWELCOMEFINISHPAGE_BITMAP "${PKGDIR}res\orange-uninstall.bmp"
!insertmacro MUI_UNPAGE_WELCOME
!insertmacro MUI_UNPAGE_INSTFILES
!insertmacro MUI_UNPAGE_FINISH

;--------------------------------

;Languages

!insertmacro MUI_LANGUAGE "English"             ; first language is the default language
!insertmacro MUI_LANGUAGE "Russian"

LicenseLangString license ${LANG_ENGLISH} "${PKGDIR}licenses\license_cateye_en_US.txt"
LicenseLangString license ${LANG_RUSSIAN} "${PKGDIR}licenses\license_cateye_ru_RU.txt"
LicenseData $(license)

!include "messages\Messages_en_US.nsh"          ; messages for my pages
!include "messages\Messages_ru_RU.nsh"

;--------------------------------


Function .onInit
  !insertmacro MUI_LANGDLL_DISPLAY
FunctionEnd


!macro RegisterExtension extenstion
  WriteRegStr HKLM "Software\Classes\.${extenstion}" "" "${PRODUCT_NAME}.File"
  WriteRegStr HKLM "Software\Classes\${PRODUCT_NAME}.File" "" "${PRODUCT_NAME} Stage Operations File" 
  WriteRegStr HKLM "Software\Classes\${PRODUCT_NAME}.File\DefaultIcon" "" "$INSTDIR\${PRODUCT_NAME}.exe,0"
  WriteRegStr HKLM "Software\Classes\${PRODUCT_NAME}.File\shell\open\command" "" "$\"$INSTDIR\${PRODUCT_NAME}.exe$\" $\"%1$\"" 
  ; default application for current user (for NT6.0 and newer)
  GetVersion::WindowsVersion
  Pop $winver
  ${If} $winver >= "6.0"
    DeleteRegKey HKCU "Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\.${extenstion}\UserChoice" 
    WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\.${extenstion}\UserChoice" "Progid" "${PRODUCT_NAME}.File" 
  ${EndIf}
!macroend

;--------------------------------

Section "Installer section"
  
  SetShellVarContext all
  
  ;call IsDotNETInstalled
  !insertmacro DotNetDownloadSetup
  
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
  
  ; Registering .cestage file
  !insertmacro RegisterExtension "cestage"
  
  WriteUninstaller $INSTDIR\Uninstall.exe

SectionEnd


;--------------------------------
;--------------------------------

Function un.onInit
  !insertmacro MUI_UNGETLANGUAGE
FunctionEnd


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
  
  ; Delete .cestage file registration
  DeleteRegKey HKLM "Software\Classes\${PRODUCT_NAME}.File"
  DeleteRegKey HKLM "Software\Classes\.cestage"
  DeleteRegKey HKCU "Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\.cestage"
  ; default application for current user (for NT6.0 and newer)
  GetVersion::WindowsVersion
  Pop $winver
  ${If} $winver >= "6.0"
    DeleteRegKey HKCU "Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\.cestage" 
  ${EndIf}
  
SectionEnd

