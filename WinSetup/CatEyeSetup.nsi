; CatEyeSetup program for Windows
; for test commit

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
!define MUI_HEADERIMAGE_BITMAP "${PKGDIR}res\top_left.bmp"

;--------------------------------

;Pages

!define MUI_WELCOMEFINISHPAGE_BITMAP "${PKGDIR}res\install_left.bmp"
!insertmacro MUI_PAGE_WELCOME

!define MUI_LICENSEPAGE_CHECKBOX
!insertmacro MUI_PAGE_LICENSE $(license)

!insertmacro MUI_PAGE_DOTNET        ; my page for .NetFramework warning
!insertmacro MUI_PAGE_EXTENSIONS    ; my page for extensions registration

!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES

;!define MUI_FINISHPAGE_TEXT_REBOOTNOW "Reboot now"
;!define MUI_FINISHPAGE_RUN "$INSTDIR\CatEye.exe"
!insertmacro MUI_PAGE_FINISH


!define MUI_UNWELCOMEFINISHPAGE_BITMAP "${PKGDIR}res\uninstall_left.bmp"
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
  SetRebootFlag true
FunctionEnd


!macro RegisterCestage extenstion
  WriteRegStr HKLM "Software\Classes\.${extenstion}" "" "${PRODUCT_NAME}.CestageFile"
  WriteRegStr HKLM "Software\Classes\${PRODUCT_NAME}.CestageFile" "" "$(cestage_description)" 
  WriteRegStr HKLM "Software\Classes\${PRODUCT_NAME}.CestageFile\DefaultIcon" "" "$INSTDIR\res\ico\cestage.ico"
  WriteRegStr HKLM "Software\Classes\${PRODUCT_NAME}.CestageFile\shell\open\command" "" "$\"$INSTDIR\${PRODUCT_NAME}.exe$\" $\"%1$\""
  WriteRegStr HKLM "Software\Classes\${PRODUCT_NAME}.CestageFile\shell\$(queue)\command" "" "$\"$INSTDIR\${PRODUCT_NAME}.exe$\" $\"-q$\" $\"%1$\""
  ; default application for current user (for NT6.0 and newer)
  GetVersion::WindowsVersion
  Pop $winver
  ${If} $winver >= "6.0"
    DeleteRegKey HKCU "Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\.${extenstion}\UserChoice" 
    WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\.${extenstion}\UserChoice" "Progid" "${PRODUCT_NAME}.CestageFile" 
  ${EndIf}
!macroend


!macro RegisterExtension extenstion ext_state
${If} ${ext_state} == 1
  WriteRegStr HKLM "Software\Classes\.${extenstion}" "" "${PRODUCT_NAME}.File"
   ; default application for current user (for NT6.0 and newer)
   GetVersion::WindowsVersion
   Pop $winver
   ${If} $winver >= "6.0"
     DeleteRegKey HKCU "Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\.${extenstion}\UserChoice" 
     WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\.${extenstion}\UserChoice" "Progid" "${PRODUCT_NAME}.File" 
   ${EndIf}
${EndIf}
!macroend


;--------------------------------

Section  "Installer section"
  
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

  ; Resources Install
  CreateDirectory "$INSTDIR\res\ico"
  SetOutPath "$INSTDIR\res\ico"
  File "..\${PKGDIR}bin\${config}\res\ico\*.*"
  SetOutPath "$INSTDIR"

  ; GtkInstall
  File /r "${PKGDIR}gtk-embedded\*.*"
  SetOutPath "$INSTDIR\res"
  File "..\${PKGDIR}bin\${config}\res\win-gtkrc"
  SetOutPath "$INSTDIR"
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
;  File "..\${PKGDIR}bin\${config}\dcraw.exe"
  File "..\${PKGDIR}bin\${config}\ssrl.dll"


  CreateShortCut  $DESKTOP\${PRODUCT_NAME}.lnk $INSTDIR\CatEye.exe
  CreateDirectory $SMPROGRAMS\${PRODUCT_NAME}
  CreateShortCut  $SMPROGRAMS\${PRODUCT_NAME}\${PRODUCT_NAME}.lnk $INSTDIR\CatEye.exe
  CreateShortCut  $SMPROGRAMS\${PRODUCT_NAME}\Uninstall.lnk $INSTDIR\Uninstall.exe

  WriteRegStr HKLM SOFTWARE\${PRODUCT_NAME} "Version" "${PRODUCT_VERSION}"
  WriteRegStr HKLM SOFTWARE\${PRODUCT_NAME} "SetupDir" "$INSTDIR"
  
  WriteRegStr HKLM SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME} "DisplayName" "${PRODUCT_NAME} ${PRODUCT_VERSION}"
  WriteRegStr HKLM SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME} "DisplayIcon" "$INSTDIR\res\ico\cateye.ico"
  WriteRegStr HKLM SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME} "UninstallString" "$INSTDIR\Uninstall.exe"
  
  ; Registering open with CatEye
  WriteRegStr HKLM "Software\Classes\${PRODUCT_NAME}.File" "" "${PRODUCT_NAME} File" 
  WriteRegStr HKLM "Software\Classes\${PRODUCT_NAME}.File\DefaultIcon" "" "$INSTDIR\res\ico\cateye-raw.ico"
  WriteRegStr HKLM "Software\Classes\${PRODUCT_NAME}.File\shell\open\command" "" "$\"$INSTDIR\${PRODUCT_NAME}.exe$\" $\"%1$\"" 
  WriteRegStr HKLM "Software\Classes\${PRODUCT_NAME}.File\shell\$(queue)\command" "" "$\"$INSTDIR\${PRODUCT_NAME}.exe$\" $\"-q$\" $\"%1$\""
  
  ; Registering .cestage file
  !insertmacro RegisterCestage "cestage"
  
  ; Registering extensions
    !insertmacro RegisterExtension "CR2" $ext_cr2_state
    !insertmacro RegisterExtension "CRW" $ext_crw_state
    !insertmacro RegisterExtension "PEF" $ext_pef_state
    !insertmacro RegisterExtension "PTX" $ext_ptx_state
    !insertmacro RegisterExtension "NEF" $ext_nef_state
    !insertmacro RegisterExtension "NRF" $ext_nrf_state
    !insertmacro RegisterExtension "ARW" $ext_arw_state
    !insertmacro RegisterExtension "SRF" $ext_srf_state
    !insertmacro RegisterExtension "SR2" $ext_sr2_state
    !insertmacro RegisterExtension "DCR" $ext_dcr_state
    !insertmacro RegisterExtension "KDC" $ext_kdc_state
    !insertmacro RegisterExtension "ORF" $ext_orf_state
    !insertmacro RegisterExtension "MRW" $ext_mrw_state
    !insertmacro RegisterExtension "RAF" $ext_raf_state
    !insertmacro RegisterExtension "RAW" $ext_raw_state
    !insertmacro RegisterExtension "RW2" $ext_rw2_state
    !insertmacro RegisterExtension "SRW" $ext_srw_state
    !insertmacro RegisterExtension "BAY" $ext_bay_state
    !insertmacro RegisterExtension "X3F" $ext_x3f_state
    !insertmacro RegisterExtension "3FR" $ext_3fr_state
  
  WriteUninstaller $INSTDIR\Uninstall.exe

SectionEnd


;--------------------------------
;--------------------------------

Function un.onInit
  !insertmacro MUI_UNGETLANGUAGE
  SetRebootFlag true
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
  Delete "$INSTDIR\res\win-gtkrc"
  
  RmDir /r "$INSTDIR\etc"
  RmDir /r "$INSTDIR\lib"
  RmDir /r "$INSTDIR\share"
  RmDir /r "$INSTDIR\res"
FunctionEnd


Function un.ResDelete
  Delete "$INSTDIR\res\ico\cateye.ico"
  Delete "$INSTDIR\res\ico\cateye-raw.ico"
  Delete "$INSTDIR\res\ico\cestage.ico"
  RmDir /r "$INSTDIR\res\ico"
FunctionEnd


!macro UnRegisterExtension extenstion

  DeleteRegKey HKLM "Software\Classes\.${extenstion}"
  ; default application for current user (for NT6.0 and newer)
  GetVersion::WindowsVersion
  Pop $winver
  ${If} $winver >= "6.0"
    DeleteRegKey HKCU "Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\.${extenstion}"
  ${EndIf}

!macroend


;--------------------------------

Section "un.Installer section"
  
  SetShellVarContext all
  
  call un.LicensesDel
  call un.ResDelete
  call un.GtkDelete
  
  Delete $INSTDIR\ssrl.dll
;  Delete $INSTDIR\dcraw.exe
  Delete $INSTDIR\CatEye.Core.dll
  Delete $INSTDIR\CatEye.Gtk.UI.Widgets.dll
  Delete $INSTDIR\CatEye.UI.Base.dll
  Delete $INSTDIR\CatEye.Widgets.dll"
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
  DeleteRegKey HKLM "Software\Classes\${PRODUCT_NAME}.CestageFile"
  DeleteRegKey HKLM "Software\Classes\.cestage"
  DeleteRegKey HKCU "Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\.cestage"
  ; default application for current user (for NT6.0 and newer)
  GetVersion::WindowsVersion
  Pop $winver
  ${If} $winver >= "6.0"
    DeleteRegKey HKCU "Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\.cestage" 
  ${EndIf}
  
  ; Unregistering extensions
  !insertmacro UnRegisterExtension "CR2"
  !insertmacro UnRegisterExtension "CRW"
  !insertmacro UnRegisterExtension "PEF"
  !insertmacro UnRegisterExtension "PTX"
  !insertmacro UnRegisterExtension "NEF"
  !insertmacro UnRegisterExtension "NRF"
  !insertmacro UnRegisterExtension "ARW"
  !insertmacro UnRegisterExtension "SRF"
  !insertmacro UnRegisterExtension "SR2"
  !insertmacro UnRegisterExtension "DCR"
  !insertmacro UnRegisterExtension "KDC"
  !insertmacro UnRegisterExtension "ORF"
  !insertmacro UnRegisterExtension "MRW"
  !insertmacro UnRegisterExtension "RAF"
  !insertmacro UnRegisterExtension "RAW"
  !insertmacro UnRegisterExtension "RW2"
  !insertmacro UnRegisterExtension "SRW"
  !insertmacro UnRegisterExtension "BAY"
  !insertmacro UnRegisterExtension "X3F"
  !insertmacro UnRegisterExtension "3FR"
  
  ; Unregistering opening with CatEye
  DeleteRegKey HKLM "Software\Classes\${PRODUCT_NAME}.File\shell\$(queue)\command"
  DeleteRegKey HKLM "Software\Classes\${PRODUCT_NAME}.File\shell\open\command"
  DeleteRegKey HKLM "Software\Classes\${PRODUCT_NAME}.File\DefaultIcon"
  DeleteRegKey HKLM "Software\Classes\${PRODUCT_NAME}.File"
  
SectionEnd

