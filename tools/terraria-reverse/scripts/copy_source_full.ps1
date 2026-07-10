# Copy world-gen related decompiled sources into TerrariaVanilla
param(
    [string]$SourceFull = "$PSScriptRoot\..\source_full",
    [string]$Dest = "$PSScriptRoot\..\..\..\TerrariaVanilla\source"
)

$ErrorActionPreference = "Stop"

if (-not (Test-Path $SourceFull)) {
    Write-Error "Run decompile first: source_full not found"
}

# Wipe and recreate
if (Test-Path $Dest) { Remove-Item $Dest -Recurse -Force }
New-Item -ItemType Directory -Force -Path $Dest | Out-Null

# Copy entire decompiled tree (1499 files) — verbatim, no edits
Copy-Item "$SourceFull\*" $Dest -Recurse -Force

$count = (Get-ChildItem $Dest -Recurse -Filter "*.cs" | Measure-Object).Count
Write-Host "Copied $count .cs files to $Dest"
