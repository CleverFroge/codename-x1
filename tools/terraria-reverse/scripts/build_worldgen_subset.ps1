# Route A: build TerrariaVanilla world-gen subset + ReLogic from source.
$ErrorActionPreference = "Stop"
$scripts = $PSScriptRoot
$root = "$scripts\..\..\..\godot\TerrariaVanilla"

Write-Host "==> Apply decompile / MonoGame port fixes"
& "$scripts\fix_decompile_artifacts.ps1"
& "$scripts\fix_port_artifacts.ps1"
& "$scripts\fix_color_ambiguity.ps1"
& "$scripts\fix_monogame_api.ps1"
& "$scripts\fix_port_artifacts.ps1"
& "$scripts\fix_remaining_port.ps1"
& "$scripts\fix_monogame_api.ps1"
& "$scripts\fix_port_artifacts.ps1"

Write-Host "==> Build ReLogic (MonoGame source)"
dotnet build "$root\ReLogic\ReLogic.csproj"
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host "==> Build TerrariaVanilla (full source port)"
dotnet build "$root\TerrariaVanilla.csproj"
exit $LASTEXITCODE
