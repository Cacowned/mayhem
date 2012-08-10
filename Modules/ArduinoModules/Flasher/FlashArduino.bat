cd /d %~dp0
pause
set arg1=%1
ArduinoFlasher.exe -c avrdude.conf -v -v -v -v -p atmega328p -c arduino -P\\.\%arg1% -b 115200 -D -U flash:w:ArduinoModulesFirmware.hex:i
pause