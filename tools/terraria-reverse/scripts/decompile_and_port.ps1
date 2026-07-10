# Decompile Terraria.exe and copy world-gen sources into TerrariaVanilla/vanilla
param(
    [Parameter(Mandatory = $false)]
    [string]$TerrariaExe = "D:\Program Files (x86)\Steam\steamapps\common\Terraria\Terraria.exe",
    [string]$OutputDir = "$PSScriptRoot\..\source_full",
    [string]$PortDir = "$PSScriptRoot\..\..\..\TerrariaVanilla\vanilla"
)

$ErrorActionPreference = "Stop"
$env:PATH = "$env:PATH;$env:USERPROFILE\.dotnet\tools"

if (-not (Test-Path $TerrariaExe)) {
    Write-Error "Terraria.exe not found: $TerrariaExe"
}

Write-Host "==> Install ilspycmd"
dotnet tool install -g ilspycmd --version 8.2.0.7535 2>$null

Write-Host "==> Decompile to $OutputDir"
New-Item -ItemType Directory -Force -Path $OutputDir | Out-Null
& ilspycmd $TerrariaExe -p -o $OutputDir

Write-Host "==> Copy world-gen files to $PortDir"
$files = @(
    "Terraria\WorldGen.cs",
    "Terraria.WorldBuilding\GenVars.cs",
    "Terraria\Main.cs",
    "Terraria.GameContent.Biomes\TerrainPass.cs",
    "Terraria.GameContent.Biomes\JunglePass.cs",
    "Terraria.GameContent.Generation.Dungeon\DungeonCrawler.cs",
    "Terraria.GameContent.Generation.Dungeon\DungeonLayoutProvider.cs",
    "Terraria.WorldBuilding\WorldGenerator.cs",
    "Terraria.WorldBuilding\GenPass.cs",
    "Terraria.WorldBuilding\GenerationProgress.cs",
    "Terraria.WorldBuilding\GameConfiguration.cs",
    "Terraria.IO\GameConfiguration.cs",
    "Terraria.WorldBuilding\GenBase.cs",
    "Terraria.WorldBuilding\StructureMap.cs",
    "Terraria.Utilities\UnifiedRandom.cs",
    "Terraria.ID\TileID.cs",
    "Terraria\Tile.cs",
    "Terraria\Liquid.cs"
)

New-Item -ItemType Directory -Force -Path $PortDir | Out-Null
foreach ($rel in $files) {
    $src = Join-Path $OutputDir $rel
    if (Test-Path $src) {
        $dest = Join-Path $PortDir $rel
        New-Item -ItemType Directory -Force -Path (Split-Path $dest) | Out-Null
        Copy-Item $src $dest -Force
        Write-Host "  OK $rel"
    } else {
        Write-Warning "  MISSING $rel"
    }
}

Write-Host "Done."

Write-Host "==> Extract embedded libs + fix ILSpy artifacts + copy full source"
& "$PSScriptRoot\extract_terrarria_libs.ps1" -TerrariaExe $TerrariaExe
& "$PSScriptRoot\fix_decompile_artifacts.ps1"
& "$PSScriptRoot\copy_source_full.ps1"
& "$PSScriptRoot\split_projectile_setdefaults.ps1"
