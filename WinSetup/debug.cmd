del ..\bin\Debug\CatEye-*.exe
del ..\bin\Debug\Version.txt
"%ProgramFiles%\nsis\makensis" /Dconfig="Debug" GetVersion.nsi
"%ProgramFiles%\nsis\makensis" /Dconfig="Debug" CatEyeSetup.nsi
