; My page for extensions registration

;--------------------------------

!include "nsDialogs.nsh"
!include "CommCtrl.nsh"

var /global extpage
var /global listview
var /global select
var /global unselect
;var /global ext_cestage_state
var /global ext_cr2_state
var /global ext_crw_state
var /global ext_pef_state
var /global ext_ptx_state


!macro MUI_PAGE_EXTENSIONS
  Page custom ExtensionsCreate ExtensionsDestroy
!macroend


Function ExtensionsCreate
    nsDialogs::Create 1018
    Pop $extpage
    !insertmacro MUI_HEADER_TEXT $(ExtensionsPage_HeaderText) $(ExtensionsPage_SubHeaderText)
    
    ${NSD_CreateListView} 0u 5u 300u 110u ""
    Pop $listview
    
    ${NSD_LV_InsertColumn} $listview 0 20u " "
    ${NSD_LV_InsertColumn} $listview 1 60u "Extension"
    ${NSD_LV_InsertColumn} $listview 2 350u "Description"
    
;    ${NSD_LV_InsertItem}  $listview 0   ""
;    ${NSD_LV_SetItemText} $listview 0 1 ".cestage"
;    ${NSD_LV_SetItemText} $listview 0 2 "${PRODUCT_NAME} stage file"
    
    ${NSD_LV_InsertItem}  $listview 0   ""
    ${NSD_LV_SetItemText} $listview 0 1 ".CR2"
    ${NSD_LV_SetItemText} $listview 0 2 "Canon camera raw file"
    
    ${NSD_LV_InsertItem}  $listview 1   ""
    ${NSD_LV_SetItemText} $listview 1 1 ".CRW"
    ${NSD_LV_SetItemText} $listview 1 2 "Canon camera raw file"
    
    ${NSD_LV_InsertItem}  $listview 2   ""
    ${NSD_LV_SetItemText} $listview 2 1 ".PEF"
    ${NSD_LV_SetItemText} $listview 2 2 "Pentax camera raw file"
    
    ${NSD_LV_InsertItem}  $listview 3   ""
    ${NSD_LV_SetItemText} $listview 3 1 ".PTX"
    ${NSD_LV_SetItemText} $listview 3 2 "Pentax camera raw file"
    
    ; Note: SendMessage command doesn't support pipe symbol.
    ; Therefore, using !define with "/math" option to set more.
    ; Send a message to set checkboxes style to listview.
    ;!define /math _LISTVIEW_TEMP_STYLE ${LVS_EX_CHECKBOXES} | ${LVS_EX_FULLROWSELECT}
    !define /math _LISTVIEW_TEMP_STYLE ${LVS_EX_CHECKBOXES} | ${LVS_EX_FULLROWSELECT}
        SendMessage $listview ${LVM_SETEXTENDEDLISTVIEWSTYLE} 0 ${_LISTVIEW_TEMP_STYLE}
    !undef _LISTVIEW_TEMP_STYLE
    ; Set the state of checkbox ${NSD_LV_SetCheckState} "hWnd" "iItem" "State"
    ; Before using this, you must set the LVS_EX_CHECKBOXES extended style.
    
;    ${NSD_LV_SetCheckState} $listview 0 $ext_cestage_state
    ${NSD_LV_SetCheckState} $listview 0 $ext_cr2_state
    ${NSD_LV_SetCheckState} $listview 1 $ext_crw_state
    ${NSD_LV_SetCheckState} $listview 2 $ext_pef_state
    ${NSD_LV_SetCheckState} $listview 3 $ext_ptx_state
    
    ${NSD_CreateButton} 150u 125u 70u 15u "Check all"
    Pop $select
    ${NSD_OnClick} $select SelectButtonChange
    
    ${NSD_CreateButton} 230u 125u 70u 15u "Uncheck all"
    Pop $unselect
    ${NSD_OnClick} $unselect UnselectButtonChange
    
    nsDialogs::Show
FunctionEnd


Function SelectButtonChange
;        ${NSD_LV_SetCheckState} $listview 0 1
        ${NSD_LV_SetCheckState} $listview 0 1
        ${NSD_LV_SetCheckState} $listview 1 1
        ${NSD_LV_SetCheckState} $listview 2 1
        ${NSD_LV_SetCheckState} $listview 3 1
FunctionEnd


Function UnselectButtonChange
;        ${NSD_LV_SetCheckState} $listview 0 0
        ${NSD_LV_SetCheckState} $listview 0 0
        ${NSD_LV_SetCheckState} $listview 1 0
        ${NSD_LV_SetCheckState} $listview 2 0
        ${NSD_LV_SetCheckState} $listview 3 0
FunctionEnd


Function ExtensionsDestroy
;    ${NSD_LV_GetCheckState} $listview 0 $ext_cestage_state
    ${NSD_LV_GetCheckState} $listview 0 $ext_cr2_state
    ${NSD_LV_GetCheckState} $listview 1 $ext_crw_state
    ${NSD_LV_GetCheckState} $listview 2 $ext_pef_state
    ${NSD_LV_GetCheckState} $listview 3 $ext_ptx_state
FunctionEnd

