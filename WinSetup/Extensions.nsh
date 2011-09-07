; My page for extensions registration

;--------------------------------

!include "nsDialogs.nsh"

var /global extpage
var /global ext_cestage
var /global ext_cestage_state
var /global ext_cr2
var /global ext_cr2_state
var /global ext_pef
var /global ext_pef_state

!macro MUI_PAGE_EXTENSIONS
  Page custom ExtensionsCreate ExtensionsDestroy
!macroend


Function ExtensionsCreate
    nsDialogs::Create 1018
    Pop $extpage
    !insertmacro MUI_HEADER_TEXT $(ExtensionsPage_HeaderText) $(ExtensionsPage_SubHeaderText)
    ${NSD_CreateCheckBox} 0u 0u 300u 10u ".cestage"
    Pop $ext_cestage
    ${NSD_CreateCheckBox} 0u 30u 150u 10u ".CR2"
    Pop $ext_cr2
    ${NSD_CreateCheckBox} 0u 50u 150u 10u ".PEF"
    Pop $ext_pef
    nsDialogs::Show
FunctionEnd


Function ExtensionsDestroy
    ${NSD_GetState} $ext_cestage $ext_cestage_state
    ${NSD_GetState} $ext_cr2     $ext_cr2_state
    ${NSD_GetState} $ext_pef     $ext_pef_state
FunctionEnd

