echo off
echo PreBuildEvents 
set dir=C:\temp\LockedAssemblies 
if not exist %dir% (mkdir %dir%) 
echo %dir%
del "C:\temp\LockedAssemblies\*" /q 
echo on
if exist "%1"  move "%1" "%dir%\%2.locked.%random%"
if exist "%3%4.pdb" move "%3%4.pdb" "%dir%\%4.pdb.locked%random%"
if exist "%3%4.xml.locked" del "%dir%\%4.xml.locked%random%"