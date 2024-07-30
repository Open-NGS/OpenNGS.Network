@echo off
set root=%~dp0
@REM set SERVER_ROOT=..\..\Technologies\NGSFrameworks\Server


echo PostBuild start
cd %root%
echo Current: %root%
echo Parameter: %~1

:CLIENT
@rem 删除旧生成文件
del /s /q ..\..\client\packages\com.openngs.ui\Runtime\UI\Generated\*
@rem 拷贝OpenNGS.UI相关的文件
xcopy build\Client\Code\OpenNGS.UI*.* ..\client\packages\com.openngs.ui\Runtime\UI\Generated\ /r /y /q
@rem 删除本次的UI相关文件，避免拷贝到下面 OpenNGS.Game仓库中
del /s /q build\Client\Code\OpenNGS.UI*.*

@rem 拷贝core相关的结构
del /s /q ..\..\client\packages\com.openngs.game\OpenNGS.Game\OpenNGS\*
xcopy build\Client\Code\OpenNGS.core*.* ..\client\packages\com.openngs.game\OpenNGS.Game\Data\ /r /y /q
del /s /q build\Client\Code\OpenNGS.core*.*

@rem 拷贝system相关的结构
del /s /q ..\..\client\packages\com.openngs.game.systems\OpenNGS.Game.Systems\Generated\*common*.cs
REM del /s /q ..\..\client\packages\com.openngs.game.systems\OpenNGS.Game.Systems\Generated\*data*.cs
xcopy build\Client\Code\*common*.cs ..\client\packages\com.openngs.game.systems\OpenNGS.Game.Systems\Generated\* /r /y /q
xcopy build\Client\Code\*data*.cs ..\client\packages\com.openngs.game.systems\OpenNGS.Game.Systems\Generated\* /r /y /q

del /s /q ..\..\client\packages\com.openngs.game.systems\OpenNGS.Game.Systems\Generated\Service\*service*.cs
xcopy build\Client\Code\*service*.cs ..\client\packages\com.openngs.game.systems\OpenNGS.Game.Systems\Generated\Service\* /r /y /q

@rem 拷贝数据
xcopy build\Client\Data\* ..\Client\Assets\StreamingAssets\data\ /r /y /q
xcopy Localization\Localization.json ..\Client\Assets\StreamingAssets\Localization\ /r /y /q


@rem service的需要再确认怎么拷贝
REM del ..\..\client\packages\com.openngs.game\OpenNGS.Game\OpenNGS\*.Service.cs
REM xcopy Client\Service\Code\*Service.cs ..\..\client\packages\com.openngs.game\OpenNGS.Game\Protocol\Services\ /r /y /q
REM xcopy Client\Service\Code\*Client.cs ..\..\client\packages\com.openngs.game\OpenNGS.Game\Protocol\ServicesClient\ /r /y /q
REM xcopy Client\Service\Code\*Common.cs ..\..\client\packages\com.openngs.game\OpenNGS.Game\OpenNGS\ /r /y /q

IF "%~1"=="-s" goto SERVER
goto END

:SERVER
@REM set SERVER_ROOT=..\..\Technologies\NGSFrameworks\Server
@REM xcopy ServerData\* %SERVER_ROOT%\bin\assets\ /r /y
@REM xcopy ..\Client\Assets\StreamingAssets\data\map\map_s1\*.bin ..\Server\bin\assets\map\map_s1\* /r /y


:END
echo PostBuild Done