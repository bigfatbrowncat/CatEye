!define PKGDIR ""
!define File "..\${PKGDIR}bin\${config}\CatEye.exe"


OutFile "..\${PKGDIR}bin\${config}\GetVersion.exe"
SilentInstall silent
 
Section
 
 ## Get file version
 GetDllVersion "${File}" $R0 $R1
  IntOp $R2 $R0 / 0x00010000
  IntOp $R3 $R0 & 0x0000FFFF
  IntOp $R4 $R1 / 0x00010000
  IntOp $R5 $R1 & 0x0000FFFF
  StrCpy $R1 "$R2.$R3.$R4.$R5"
 
 ## Write it to a !define for use in main script
 FileOpen $R0 "..\${PKGDIR}bin\${config}\Version.txt" w
  FileWrite $R0 '!define PRODUCT_VERSION "$R1"'
  FileWriteByte $R0 "13"
  FileWriteByte $R0 "10"
  StrCpy $R1 "$R2.$R3.$R4"
  FileWrite $R0 '!define VERSION_LONG "$R1"'
  FileWriteByte $R0 "13"
  FileWriteByte $R0 "10"
  StrCpy $R1 "$R2.$R3"
  FileWrite $R0 '!define VERSION_SHORT "$R1"'
  FileWriteByte $R0 "13"
  FileWriteByte $R0 "10"
 FileClose $R0
 
SectionEnd
