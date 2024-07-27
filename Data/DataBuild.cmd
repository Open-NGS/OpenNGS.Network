@echo off
set root=%~dp0

call databuilder.cmd
if not errorlevel 0 (
	echo error: modelbuilder.cmd
	goto FAILED
	)

echo Post Processes...
cd %root%

echo Start Post Processes...
call Game\PostBuild.cmd %~1
echo Post Processes Done
goto END

:FAILED
echo Fatal Error

:END
pause
cmd /k