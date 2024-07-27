@echo off
set root=%~dp0

.\Tools\ngsdata\net8.0\ngsdata.exe -b model -p Game\Data.project -g -v

:END
echo ModelBuilder Done