del ..\bin\Release\CatEye-*.exe
del ..\bin\Release\Version.txt
"%ProgramFiles%\nsis\makensis" /Dconfig="Release" GetVersion.nsi
"%ProgramFiles%\nsis\makensis" /Dconfig="Release" CatEyeSetup.nsi
