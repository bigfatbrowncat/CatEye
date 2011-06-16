/*

NSIS Modern User Interface
DotNet page

*/

!include "nsDialogs.nsh"

var /global dotnetset
var /global mui.DotNetPage
var /global mui.DotNetPage.Text
;var /global mui.DotNetPage.TopText
var /global mui.DotNetPage.CheckBox
var /global mui.DotNetPage.CheckBox.State

!macro MUI_PAGE_DOTNET
  Page custom PageDotNetInit PageDotNetDestroy
!macroend


Function PageDotNetInit
  call IsDotNetInstalled
FunctionEnd


Function PageDotNetDestroy
  ${NSD_GetState} $mui.DotNetPage.CheckBox $mui.DotNetPage.CheckBox.State
  ${If} $mui.DotNetPage.CheckBox.State == ${BST_CHECKED}
    ExecShell "" "http://www.microsoft.com/downloads/en/details.aspx?FamilyID=5b2c0358-915b-4eb5-9b1d-10e506da9d0f"
    quit
  ${EndIf}
  ${If} $mui.DotNetPage.CheckBox.State == ${BST_UNCHECKED}
    MessageBox MB_ICONINFORMATION "${PRODUCT_NAME} requires .NET Framework 2.0. Installation aborted."
    quit
  ${EndIf}
FunctionEnd


Function IsDotNETInstalled
  ; Check .NET version
  ReadRegDWORD $dotnetset HKLM 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v2.0.50727' Install
  IntCmp $dotnetset 1 go1 nonet
  ;IntCmp $dotnetset 0 go1 nonet    ; it's false
    nonet:
      call DotNetDownload
  go1:
FunctionEnd


Function DotNetDownload
  
  nsDialogs::Create 1018 
  Pop $mui.DotNetPage
  
  nsDialogs::CreateControl STATIC ${WS_VISIBLE}|${WS_CHILD}|${WS_CLIPSIBLINGS} 0 0u 0u 300u 100u "${PRODUCT_NAME} requires .NET Framework 2.0. Download and setup .NET Framework 2.0 on your system from official Microsoft site. After it run ${PRODUCT_NAME}-${VERSION_LONG}-setup.exe again.$\r$\n$\r$\nWould You like to download and install .NET Framework 2.0 from official Microsoft site?"
  Pop $mui.DotNetPage.Text
  
  ${NSD_CreateCheckBox} 0u 130u 300u 10u "Yes, download and install .NET Framework 2.0"
  Pop $mui.DotNetPage.CheckBox
  
  nsDialogs::Show 
  
FunctionEnd

