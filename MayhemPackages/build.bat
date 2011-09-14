@echo off
IF [%1] == [] (
echo Need a parameter!
GOTO FAIL
)
echo Copying
echo %~1
mkdir tmp\%~1\lib\net40
copy specs\%~1.nuspec tmp\%~1
robocopy "..\bin\Packages\%~1" "tmp\%~1\lib\net40" /MIR /XF *.pdb >nul
cd tmp\%~1
nuget pack
xcopy /Q /Y %~1.*.nupkg ..\..
cd ..\..
rmdir /S /Q tmp
:FAIL