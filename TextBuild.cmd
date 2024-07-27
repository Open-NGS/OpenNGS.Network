@echo off

set UnityExe="C:\Program Files\Unity\Hub\Editor\2022.3.13f1c1\Editor\Unity.exe"

echo "Running UniqueChars"
%UnityExe% -quit -batchmode -accept-apiupdate -projectPath "Client" -executeMethod unique_chars.UniqueChars
echo "UniqueChars completed"

echo "Running FontImporter"
%UnityExe% -quit -batchmode -accept-apiupdate -projectPath "Client" -executeMethod FontAssetGenerator.FontImporter
echo "FontImporter completed"
