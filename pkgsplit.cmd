
@ECHO OFF

IF "%~1" == "" GOTO :HELP


git subtree push --prefix %1 %2 %3

if %errorlevel% neq 0 GOTO :END

git rm -rf %1
git commit -m "提交删除记录"

git subtree add --prefix %1 %2 %3

GOTO :END

:HELP
ECHO usage: pkgsplit ^<prefix^> ^<repository^> ^<ref^>

:END


