@echo off
cd /d "%~dp0.."
dotnet run --project TerrariaAssetExtract\TerrariaAssetExtract.csproj -c Release -- %*
