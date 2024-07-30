@echo off
set root=%~dp0

"%NGS_BIN%\ngs.exe" -b data -p ngs-game.ngproj -v

:END
echo DataBuilder Done