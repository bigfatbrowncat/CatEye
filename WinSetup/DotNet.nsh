; My page for .NetFramework warning

;--------------------------------

!include "nsDialogs.nsh"

; Variables
var /global dotnetset
var /global mypage
var /global label
var /global checkbox
var /global checkboxstate
var /global address
var /global download
var /global NextButton
var /global platform

!macro MUI_PAGE_DOTNET
  Page custom WarningCreate WarningDestroy
!macroend


Function WarningCreate
  ; Check .NET v2.0 installation
  ReadRegDWORD $dotnetset HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v2.0.50727" Install
  ;${If} $dotnetset == 1
  ;  MessageBox MB_ICONINFORMATION ".NetFramework 2.0 already installed. Push <Ok> to quit."
  ;  quit
  ;${EndIf}
  ${If} $dotnetset == 0
    nsDialogs::Create 1018
    Pop $mypage
    !insertmacro MUI_HEADER_TEXT ".NET Framework 2.0 Warning" "${PRODUCT_NAME} requires Microsoft .NET Framework 2.0."
    ${NSD_CreateLabel} 0u 0u 300u 120u "${PRODUCT_NAME} requires Microsoft .NET Framework 2.0.$\r$\n$\r$\nNow you need to download .NET Framework from it's official site (about 20 - 50 MB depending on Your system) and install it to your system in order to use ${PRODUCT_NAME}. If you have the .NET Framework 2.0 redistributable already downloaded you may close the ${PRODUCT_NAME} ${VERSION_SHORT} Setup now, install it and then restart the ${PRODUCT_NAME} ${VERSION_SHORT} Setup.$\r$\n$\r$\nWould You like to download and setup .NetFramework 2.0 on your system during installation of ${PRODUCT_NAME}? If not close installer."
    Pop $label
    ${NSD_CreateCheckBox} 0u 130u 300u 10u "Yes, download and install .NetFramework 2.0."
    Pop $checkbox
    ${NSD_SetState} $checkbox $checkboxstate
    ${NSD_OnClick} $checkbox NextButtonChange
    GetDlgItem $NextButton $HWNDPARENT 1
    Call NextButtonChange
    ;EnableWindow $NextButton 0
    nsDialogs::Show
  ${EndIf}
FunctionEnd


Function NextButtonChange
  ;GetDlgItem $NextButton $HWNDPARENT 1
  ${NSD_GetState} $checkbox $checkboxstate
  ${If} $checkboxstate == ${BST_UNCHECKED}
    EnableWindow $NextButton 0                  ; button disabled
  ${EndIf}
  ${If} $checkboxstate == ${BST_CHECKED}
    EnableWindow $NextButton 1                  ; button enabled
  ${EndIf}
FunctionEnd


Function WarningDestroy
  ${NSD_GetState} $checkbox $checkboxstate
FunctionEnd


!macro DotNetDownloadSetup
  call DotNetDownloadSetup
!macroend


Function DotNetDownloadSetup
  ${If} $checkboxstate == ${BST_UNCHECKED}
    MessageBox MB_ICONINFORMATION "Your need .NetFramework 2.0 install on Your system. Installation of .NetFramework 2.0 aborted."
    quit
  ${EndIf}
  ${If} $checkboxstate == ${BST_CHECKED}
    ; check platform
    GetVersion::WindowsPlatformArchitecture
    Pop $platform
    ;page for .NetFramework download
    ;ExecShell "" "http://www.microsoft.com/downloads/en/details.aspx?FamilyID=5b2c0358-915b-4eb5-9b1d-10e506da9d0f"
    ${If} $platform == "32"
      StrCpy $address "http://www.microsoft.com/downloads/info.aspx?na=46&SrcFamilyId=5B2C0358-915B-4EB5-9B1D-10E506DA9D0F&SrcDisplayLang=en&u=http%3a%2f%2fdownload.microsoft.com%2fdownload%2fc%2f6%2fe%2fc6e88215-0178-4c6c-b5f3-158ff77b1f38%2fNetFx20SP2_x86.exe"
    ${EndIf}
    ${If} $platform == "64"
      StrCpy $address "http://www.microsoft.com/downloads/info.aspx?na=41&srcfamilyid=5b2c0358-915b-4eb5-9b1d-10e506da9d0f&srcdisplaylang=en&u=http%3a%2f%2fdownload.microsoft.com%2fdownload%2fc%2f6%2fe%2fc6e88215-0178-4c6c-b5f3-158ff77b1f38%2fNetFx20SP2_x64.exe"
    ${EndIf}
    ;${If} $platform == "ia64"
    ;  StrCpy $address "http://www.microsoft.com/downloads/info.aspx?na=41&srcfamilyid=5b2c0358-915b-4eb5-9b1d-10e506da9d0f&srcdisplaylang=en&u=http%3a%2f%2fdownload.microsoft.com%2fdownload%2fc%2f6%2fe%2fc6e88215-0178-4c6c-b5f3-158ff77b1f38%2fNetFx20SP2_ia64.exe"
    ;${EndIf}
    ;SetDetailsView hide
    inetc::get /caption "Downloading .NET Framework 2.0" /canceltext "Cancel download" $address "$TEMP\dotnetfx.exe" /end
    ;SetDetailsView show
    Pop $download
    ${If} $download != "OK"
      Delete "$TEMP\dotnetfx.exe"
      Abort "${PRODUCT_NAME} requires Microsoft .NET Framework 2.0. Installation cancelled."
    ${EndIf}
    ExecWait "$TEMP\dotnetfx.exe"
    Delete "$TEMP\dotnetfx.exe"
  ${EndIf}
FunctionEnd
