@echo off
cd /d "%~dp0"
dotnet run --project TerrariaAssetExtract.csproj -c Release -- %*
