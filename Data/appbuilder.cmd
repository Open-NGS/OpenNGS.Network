@echo off
set root=%~dp0

.\Tools\appbuilder\net8.0\appbuilder.exe -p Game\Data.project

:END
echo AppBuilder Done