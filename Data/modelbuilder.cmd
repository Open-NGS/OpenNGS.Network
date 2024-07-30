@echo off
set root=%~dp0

"%NGS_BIN%\ngs.exe" -b model -p ngs-game.ngproj -g

:END
echo ModelBuilder Done