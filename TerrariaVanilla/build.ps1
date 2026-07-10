$ErrorActionPreference = "Stop"
Set-Location $PSScriptRoot
dotnet build TerrariaVanilla.csproj -c Debug
exit $LASTEXITCODE
