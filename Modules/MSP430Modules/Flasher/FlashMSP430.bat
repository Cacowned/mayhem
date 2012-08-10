cd /d %~dp0
pause
msp430flasher.exe -n MSP430G2xx3 -w MSP430ModulesFirmware.txt -v -z [VCC]
pause