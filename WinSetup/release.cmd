del ..\bin\Release\CatEye-*.exe
del ..\bin\Release\Version.txt
makensis /Dconfig="Release" GetVersion.nsi
makensis /Dconfig="Release" CatEyeSetup.nsi
