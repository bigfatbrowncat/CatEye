# Makefile for CatEyeSetup program for Windows
#
#====================================================================

# configuration for Debug
#------------------------

Debug: GetVersion.nsi CatEyeSetup.nsi DotNet.nsh Extensions.nsh CommCtrl.nsh
	makensis /Dconfig="Debug" GetVersion.nsi
	makensis /Dconfig="Debug" CatEyeSetup.nsi


# Clean Debug

cleandebug:
	del ..\bin\Debug\Version.txt
	del ..\bin\Debug\GetVersion.exe
	del ..\bin\Debug\CatEye-*.exe

#--------------------------------------------------------------------

# configuration for Release
#--------------------------

Release: GetVersion.nsi CatEyeSetup.nsi DotNet.nsh Extensions.nsh CommCtrl.nsh
	makensis /Dconfig="Release" GetVersion.nsi
	makensis /Dconfig="Release" CatEyeSetup.nsi

	
# Clean Release

cleanrelease:
	del ..\bin\Release\Version.txt
	del ..\bin\Release\GetVersion.exe
	del ..\bin\Release\CatEye-*.exe
