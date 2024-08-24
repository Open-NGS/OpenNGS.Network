
@ECHO OFF

IF "%~1" == "" GOTO :HELP

pkgsplit.cmd Client/Packages/1% https://git.eegames.net/openngs/client/1%.git main

GOTO :END

:HELP
ECHO usage: pkgsp ^<packagename^>

:END


