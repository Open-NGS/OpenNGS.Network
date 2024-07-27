@echo off
set root=%~dp0
echo Sync ...

echo Delete old file
del /s /q game\Client\*
del /s /q game\Server\*

cd %root%

echo Build Model ...
call modelbuilder.cmd
echo Build Data ...
call databuilder.cmd

:CLIENT
echo.
echo.
echo Coping file ...
xcopy game\client\data\* ..\Client\Assets\StreamingAssets\data\ /r /y
xcopy game\client\Code\Zerium*.* ..\Client\Assets\Game\Scripts\Data\ /r /y
xcopy game\Localization\Localization.json ..\Client\Assets\StreamingAssets\Localization\ /r /y /q
xcopy game\client\Code\OpenNGS.UI.cs ..\Client\Assets\OpenNGS\com.openngs.ui\Runtime\UI\ /r /y
xcopy game\client\Code\OpenNGS.UI.Data.cs ..\Client\Assets\Game\Scripts\Data\ /r /y

:SERVER

:BATTLE


echo Submit ...

:END
echo Post Processes Done
cmd /k