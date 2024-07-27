@echo off
set root=%~dp0

echo Delete old file
del /s /q game\Client\*
del /s /q game\Server\*

call modelbuilder.cmd
if not errorlevel 0 (
	echo error: modelbuilder.cmd
	goto FAILED
	)
			
call databuilder.cmd
if not errorlevel 0 (
	echo error: databuilder.cmd
	goto FAILED
	)


		
echo Start Post Processes...
call Game\PostBuild.cmd  %~1
echo Post Processes Done
goto END

:FAILED
echo Fatal Error

:END
pause