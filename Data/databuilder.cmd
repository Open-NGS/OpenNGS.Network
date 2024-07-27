@echo off
set root=%~dp0

.\Tools\ngsdata\net8.0\ngsdata.exe -b data -p Game\Data.project -v

:END
echo DataBuilder Done