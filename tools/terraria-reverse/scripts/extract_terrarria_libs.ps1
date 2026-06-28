# Extract embedded library DLLs from Terraria.exe for TerrariaVanilla compile references.
param(
    [string]$TerrariaExe = "D:\Program Files (x86)\Steam\steamapps\common\Terraria\Terraria.exe",
    [string]$OutDir = "$PSScriptRoot\..\libs"
)

$ErrorActionPreference = "Stop"
if (-not (Test-Path $TerrariaExe)) { Write-Error "Not found: $TerrariaExe" }

New-Item -ItemType Directory -Force -Path $OutDir | Out-Null
$asm = [Reflection.Assembly]::LoadFile((Resolve-Path $TerrariaExe).Path)

$wanted = @(
    "Terraria.Libraries.ReLogic.ReLogic.dll",
    "Terraria.Libraries.JSON.NET.Newtonsoft.Json.dll",
    "Terraria.Libraries.DotNetZip.Ionic.Zip.CF.dll",
    "Terraria.Libraries.CsvHelper.CsvHelper.dll",
    "Terraria.Libraries.MP3Sharp.MP3Sharp.dll",
    "Terraria.Libraries.NVorbis.NVorbis.dll",
    "Terraria.Libraries.Steamworks.NET.Windows.Steamworks.NET.dll",
    "Terraria.Libraries.SteelSeries.SteelSeriesEngineWrapper.dll",
    "Terraria.Libraries.RailSDK.Windows.RailSDK.Net.dll"
)

$nameMap = @{
    "Terraria.Libraries.ReLogic.ReLogic.dll" = "ReLogic.dll"
    "Terraria.Libraries.JSON.NET.Newtonsoft.Json.dll" = "Newtonsoft.Json.dll"
    "Terraria.Libraries.DotNetZip.Ionic.Zip.CF.dll" = "Ionic.Zip.CF.dll"
    "Terraria.Libraries.CsvHelper.CsvHelper.dll" = "CsvHelper.dll"
    "Terraria.Libraries.MP3Sharp.MP3Sharp.dll" = "MP3Sharp.dll"
    "Terraria.Libraries.NVorbis.NVorbis.dll" = "NVorbis.dll"
    "Terraria.Libraries.Steamworks.NET.Windows.Steamworks.NET.dll" = "Steamworks.NET.dll"
    "Terraria.Libraries.SteelSeries.SteelSeriesEngineWrapper.dll" = "SteelSeriesEngineWrapper.dll"
    "Terraria.Libraries.RailSDK.Windows.RailSDK.Net.dll" = "RailSDK.Net.dll"
}

foreach ($res in $wanted) {
    $stream = $asm.GetManifestResourceStream($res)
    if ($null -eq $stream) { Write-Warning "Missing resource: $res"; continue }
    $name = $nameMap[$res]
    $path = Join-Path $OutDir $name
    $fs = [System.IO.File]::Create($path)
    try { $stream.CopyTo($fs) } finally { $fs.Close(); $stream.Close() }
    Write-Host "Extracted $name"
}

Write-Host "Done -> $OutDir"
