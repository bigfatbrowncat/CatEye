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
var /global ext_nef_state
var /global ext_nrf_state
var /global ext_arw_state
var /global ext_srf_state
var /global ext_sr2_state
var /global ext_dcr_state
var /global ext_kdc_state
var /global ext_orf_state
var /global ext_mrw_state
var /global ext_raf_state
var /global ext_raw_state
var /global ext_rw2_state
var /global ext_srw_state
var /global ext_bay_state
var /global ext_x3f_state
var /global ext_3fr_state


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
    ${NSD_LV_InsertColumn} $listview 2 349u "Description"
    
;    ${NSD_LV_InsertItem}  $listview 0   ""
;    ${NSD_LV_SetItemText} $listview 0 1 ".cestage"
;    ${NSD_LV_SetItemText} $listview 0 2 "${PRODUCT_NAME} stage file"
    
    ${NSD_LV_InsertItem}  $listview 0   ""
    ${NSD_LV_SetItemText} $listview 0 1 ".CR2"
    ${NSD_LV_SetItemText} $listview 0 2 "$(cr2_description)"
    
    ${NSD_LV_InsertItem}  $listview 1   ""
    ${NSD_LV_SetItemText} $listview 1 1 ".CRW"
    ${NSD_LV_SetItemText} $listview 1 2 "$(crw_description)"
    
    ${NSD_LV_InsertItem}  $listview 2   ""
    ${NSD_LV_SetItemText} $listview 2 1 ".PEF"
    ${NSD_LV_SetItemText} $listview 2 2 "$(pef_description)"
    
    ${NSD_LV_InsertItem}  $listview 3   ""
    ${NSD_LV_SetItemText} $listview 3 1 ".PTX"
    ${NSD_LV_SetItemText} $listview 3 2 "$(ptx_description)"
    
    ${NSD_LV_InsertItem}  $listview 4   ""
    ${NSD_LV_SetItemText} $listview 4 1 ".NEF"
    ${NSD_LV_SetItemText} $listview 4 2 "$(nef_description)"
    
    ${NSD_LV_InsertItem}  $listview 5   ""
    ${NSD_LV_SetItemText} $listview 5 1 ".NRF"
    ${NSD_LV_SetItemText} $listview 5 2 "$(nrf_description)"
    
    ${NSD_LV_InsertItem}  $listview 6   ""
    ${NSD_LV_SetItemText} $listview 6 1 ".ARW"
    ${NSD_LV_SetItemText} $listview 6 2 "$(arw_description)"
    
    ${NSD_LV_InsertItem}  $listview 7   ""
    ${NSD_LV_SetItemText} $listview 7 1 ".SRF"
    ${NSD_LV_SetItemText} $listview 7 2 "$(srf_description)"
    
    ${NSD_LV_InsertItem}  $listview 8   ""
    ${NSD_LV_SetItemText} $listview 8 1 ".SR2"
    ${NSD_LV_SetItemText} $listview 8 2 "$(sr2_description)"
    
    ${NSD_LV_InsertItem}  $listview 9   ""
    ${NSD_LV_SetItemText} $listview 9 1 ".DCR"
    ${NSD_LV_SetItemText} $listview 9 2 "$(dcr_description)"
    
    ${NSD_LV_InsertItem}  $listview 10   ""
    ${NSD_LV_SetItemText} $listview 10 1 ".KDC"
    ${NSD_LV_SetItemText} $listview 10 2 "$(kdc_description)"
    
    ${NSD_LV_InsertItem}  $listview 11   ""
    ${NSD_LV_SetItemText} $listview 11 1 ".ORF"
    ${NSD_LV_SetItemText} $listview 11 2 "$(orf_description)"
    
    ${NSD_LV_InsertItem}  $listview 12   ""
    ${NSD_LV_SetItemText} $listview 12 1 ".MRW"
    ${NSD_LV_SetItemText} $listview 12 2 "$(mrw_description)"
    
    ${NSD_LV_InsertItem}  $listview 13   ""
    ${NSD_LV_SetItemText} $listview 13 1 ".RAF"
    ${NSD_LV_SetItemText} $listview 13 2 "$(raf_description)"
    
    ${NSD_LV_InsertItem}  $listview 14   ""
    ${NSD_LV_SetItemText} $listview 14 1 ".RAW"
    ${NSD_LV_SetItemText} $listview 14 2 "$(raw_description)"
    
    ${NSD_LV_InsertItem}  $listview 15   ""
    ${NSD_LV_SetItemText} $listview 15 1 ".RW2"
    ${NSD_LV_SetItemText} $listview 15 2 "$(rw2_description)"
    
    ${NSD_LV_InsertItem}  $listview 16   ""
    ${NSD_LV_SetItemText} $listview 16 1 ".SRW"
    ${NSD_LV_SetItemText} $listview 16 2 "$(srw_description)"
    
    ${NSD_LV_InsertItem}  $listview 17   ""
    ${NSD_LV_SetItemText} $listview 17 1 ".BAY"
    ${NSD_LV_SetItemText} $listview 17 2 "$(bay_description)"
    
    ${NSD_LV_InsertItem}  $listview 18   ""
    ${NSD_LV_SetItemText} $listview 18 1 ".X3F"
    ${NSD_LV_SetItemText} $listview 18 2 "$(x3f_description)"
    
    ${NSD_LV_InsertItem}  $listview 19   ""
    ${NSD_LV_SetItemText} $listview 19 1 ".3FR"
    ${NSD_LV_SetItemText} $listview 19 2 "$(3fr_description)"
    
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
    ${NSD_LV_SetCheckState} $listview 4 $ext_nef_state
    ${NSD_LV_SetCheckState} $listview 5 $ext_nrf_state
    ${NSD_LV_SetCheckState} $listview 6 $ext_arw_state
    ${NSD_LV_SetCheckState} $listview 7 $ext_srf_state
    ${NSD_LV_SetCheckState} $listview 8 $ext_sr2_state
    ${NSD_LV_SetCheckState} $listview 9 $ext_dcr_state
    ${NSD_LV_SetCheckState} $listview 10 $ext_kdc_state
    ${NSD_LV_SetCheckState} $listview 11 $ext_orf_state
    ${NSD_LV_SetCheckState} $listview 12 $ext_mrw_state
    ${NSD_LV_SetCheckState} $listview 13 $ext_raf_state
    ${NSD_LV_SetCheckState} $listview 14 $ext_raw_state
    ${NSD_LV_SetCheckState} $listview 15 $ext_rw2_state
    ${NSD_LV_SetCheckState} $listview 16 $ext_srw_state
    ${NSD_LV_SetCheckState} $listview 17 $ext_bay_state
    ${NSD_LV_SetCheckState} $listview 18 $ext_x3f_state
    ${NSD_LV_SetCheckState} $listview 19 $ext_3fr_state
    
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
        ${NSD_LV_SetCheckState} $listview 4 1
        ${NSD_LV_SetCheckState} $listview 5 1
        ${NSD_LV_SetCheckState} $listview 6 1
        ${NSD_LV_SetCheckState} $listview 7 1
        ${NSD_LV_SetCheckState} $listview 8 1
        ${NSD_LV_SetCheckState} $listview 9 1
        ${NSD_LV_SetCheckState} $listview 10 1
        ${NSD_LV_SetCheckState} $listview 11 1
        ${NSD_LV_SetCheckState} $listview 12 1
        ${NSD_LV_SetCheckState} $listview 13 1
        ${NSD_LV_SetCheckState} $listview 14 1
        ${NSD_LV_SetCheckState} $listview 15 1
        ${NSD_LV_SetCheckState} $listview 16 1
        ${NSD_LV_SetCheckState} $listview 17 1
        ${NSD_LV_SetCheckState} $listview 18 1
        ${NSD_LV_SetCheckState} $listview 19 1
FunctionEnd


Function UnselectButtonChange
;        ${NSD_LV_SetCheckState} $listview 0 0
        ${NSD_LV_SetCheckState} $listview 0 0
        ${NSD_LV_SetCheckState} $listview 1 0
        ${NSD_LV_SetCheckState} $listview 2 0
        ${NSD_LV_SetCheckState} $listview 3 0
        ${NSD_LV_SetCheckState} $listview 4 0
        ${NSD_LV_SetCheckState} $listview 5 0
        ${NSD_LV_SetCheckState} $listview 6 0
        ${NSD_LV_SetCheckState} $listview 7 0
        ${NSD_LV_SetCheckState} $listview 8 0
        ${NSD_LV_SetCheckState} $listview 9 0
        ${NSD_LV_SetCheckState} $listview 10 0
        ${NSD_LV_SetCheckState} $listview 11 0
        ${NSD_LV_SetCheckState} $listview 12 0
        ${NSD_LV_SetCheckState} $listview 13 0
        ${NSD_LV_SetCheckState} $listview 14 0
        ${NSD_LV_SetCheckState} $listview 15 0
        ${NSD_LV_SetCheckState} $listview 16 0
        ${NSD_LV_SetCheckState} $listview 17 0
        ${NSD_LV_SetCheckState} $listview 18 0
        ${NSD_LV_SetCheckState} $listview 19 0
FunctionEnd


Function ExtensionsDestroy
;    ${NSD_LV_GetCheckState} $listview 0 $ext_cestage_state
    ${NSD_LV_GetCheckState} $listview 0 $ext_cr2_state
    ${NSD_LV_GetCheckState} $listview 1 $ext_crw_state
    ${NSD_LV_GetCheckState} $listview 2 $ext_pef_state
    ${NSD_LV_GetCheckState} $listview 3 $ext_ptx_state
    ${NSD_LV_GetCheckState} $listview 4 $ext_nef_state
    ${NSD_LV_GetCheckState} $listview 5 $ext_nrf_state
    ${NSD_LV_GetCheckState} $listview 6 $ext_arw_state
    ${NSD_LV_GetCheckState} $listview 7 $ext_srf_state
    ${NSD_LV_GetCheckState} $listview 8 $ext_sr2_state
    ${NSD_LV_GetCheckState} $listview 9 $ext_dcr_state
    ${NSD_LV_GetCheckState} $listview 10 $ext_kdc_state
    ${NSD_LV_GetCheckState} $listview 11 $ext_orf_state
    ${NSD_LV_GetCheckState} $listview 12 $ext_mrw_state
    ${NSD_LV_GetCheckState} $listview 13 $ext_raf_state
    ${NSD_LV_GetCheckState} $listview 14 $ext_raw_state
    ${NSD_LV_GetCheckState} $listview 15 $ext_rw2_state
    ${NSD_LV_GetCheckState} $listview 16 $ext_srw_state
    ${NSD_LV_GetCheckState} $listview 17 $ext_bay_state
    ${NSD_LV_GetCheckState} $listview 18 $ext_x3f_state
    ${NSD_LV_GetCheckState} $listview 19 $ext_3fr_state
FunctionEnd

