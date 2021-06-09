@echo off
cd
cd KeyChart.GUI
cd
dotnet build -c release
IF ERRORLEVEL 1 GOTO FAILED
start dotrun
GOTO END

:FAILED
pause

:END