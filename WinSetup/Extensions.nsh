; My page for extensions registration

;--------------------------------

!include "nsDialogs.nsh"
!include "CommCtrl.nsh"

var /global extpage
;var /global ext_cestage
var /global ext_cestage_state
;var /global ext_cr2
var /global ext_cr2_state
;var /global ext_pef
var /global ext_pef_state
var /global listview
var /global select
var /global unselect

!macro MUI_PAGE_EXTENSIONS
  Page custom ExtensionsCreate ExtensionsDestroy
!macroend


Function ExtensionsCreate
    nsDialogs::Create 1018
    Pop $extpage
    !insertmacro MUI_HEADER_TEXT $(ExtensionsPage_HeaderText) $(ExtensionsPage_SubHeaderText)
;    ${NSD_CreateCheckBox} 0u 0u 300u 10u ".cestage"
;    Pop $ext_cestage
;    ${NSD_CreateCheckBox} 0u 30u 150u 10u ".CR2"
;    Pop $ext_cr2
;    ${NSD_CreateCheckBox} 0u 50u 150u 10u ".PEF"
;    Pop $ext_pef
    
    ${NSD_CreateListView} 0u 5u 300u 110u ""
    Pop $listview
    
    ${NSD_LV_InsertColumn} $listview 0 20u "#"
    ${NSD_LV_InsertColumn} $listview 1 60u "Extension"
    ${NSD_LV_InsertColumn} $listview 2 350u "Description"
    
    ${NSD_LV_InsertItem}  $listview 0   ""
    ${NSD_LV_SetItemText} $listview 0 1 ".cestage"
    ${NSD_LV_SetItemText} $listview 0 2 "${PRODUCT_NAME} stage file"
    
    ${NSD_LV_InsertItem}  $listview 1   ""
    ${NSD_LV_SetItemText} $listview 1 1 ".CR2"
    ${NSD_LV_SetItemText} $listview 1 2 "Canon camera raw file"
    
    ${NSD_LV_InsertItem}  $listview 2   ""
    ${NSD_LV_SetItemText} $listview 2 1 ".PEF"
    ${NSD_LV_SetItemText} $listview 2 2 "Pentax camera raw file"
    
    ; Note: SendMessage command doesn't support pipe symbol.
    ; Therefore, using !define with "/math" option to set more.
    ; Send a message to set checkboxes style to listview.
    ;!define /math _LISTVIEW_TEMP_STYLE ${LVS_EX_CHECKBOXES} | ${LVS_EX_FULLROWSELECT}
    !define _LISTVIEW_TEMP_STYLE ${LVS_EX_CHECKBOXES}
    SendMessage $listview ${LVM_SETEXTENDEDLISTVIEWSTYLE} 0 ${_LISTVIEW_TEMP_STYLE}
    !undef _LISTVIEW_TEMP_STYLE
    ; Set the state of checkbox ${NSD_LV_SetCheckState} "hWnd" "iItem" "State"
    ; Before using this, you must set the LVS_EX_CHECKBOXES extended style.
    ${NSD_LV_SetCheckState} $listview 0 1
    ${NSD_LV_SetCheckState} $listview 1 1
    ${NSD_LV_SetCheckState} $listview 2 1
    
    ${NSD_CreateButton} 150u 125u 70u 15u "Check all"
    Pop $select
    
    ${NSD_CreateButton} 230u 125u 70u 15u "Uncheck all"
    Pop $unselect
    
    nsDialogs::Show
FunctionEnd


Function ExtensionsDestroy
;    ${NSD_GetState} $ext_cestage $ext_cestage_state
;    ${NSD_GetState} $ext_cr2     $ext_cr2_state
;    ${NSD_GetState} $ext_pef     $ext_pef_state

    ${NSD_LV_GetCheckState} $listview 0 $ext_cestage_state
    ${NSD_LV_GetCheckState} $listview 1 $ext_cr2_state
    ${NSD_LV_GetCheckState} $listview 2 $ext_pef_state

FunctionEnd

