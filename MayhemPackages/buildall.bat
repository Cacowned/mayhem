@echo off
forfiles /p specs /m *.nuspec /c "cmd /c cd .. & build @fname"
pause 
