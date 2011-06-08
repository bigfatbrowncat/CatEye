del ..\bin\Debug\CatEye-*.exe
del ..\bin\Debug\Version.txt
makensis /Dconfig="Debug" GetVersion.nsi
makensis /Dconfig="Debug" CatEyeSetup.nsi
