# 代号X1 本地环境一键检查 / 安装
param(
    [switch]$InstallGodot
)

$ErrorActionPreference = 'Stop'
$LauncherRoot = $PSScriptRoot
$RepoRoot = Split-Path (Split-Path $LauncherRoot -Parent) -Parent
$GodotProjectRoot = Join-Path $RepoRoot 'godot'
. (Join-Path $LauncherRoot 'Find-GodotMono.ps1')

Write-Host '=== Codename X1 environment check ==='

# .NET SDK
$dotnet = Get-Command dotnet -ErrorAction SilentlyContinue
if (-not $dotnet) {
    Write-Error '.NET SDK not found. Install .NET 8+: https://dotnet.microsoft.com/download'
    exit 1
}
$dotnetVer = dotnet --version
Write-Host "[OK] dotnet $dotnetVer"

# Godot Mono
$godot = Find-GodotMonoExe
if (-not $godot) {
    Write-Host '[!!] Godot Mono not installed'
    if ($InstallGodot) {
        Write-Host 'Installing GodotEngine.GodotEngine.Mono ...'
        winget install --id GodotEngine.GodotEngine.Mono -e --accept-source-agreements --accept-package-agreements
        $godot = Find-GodotMonoExe
    }
}
if ($godot) {
    Write-Host "[OK] Godot Mono: $godot"
} else {
    Write-Error 'Godot Mono not found. Run: .\tools\Launcher\Setup-Env.ps1 -InstallGodot'
    exit 1
}

# C# 构建
Write-Host 'Building CodenameX1.csproj ...'
Push-Location $GodotProjectRoot
try {
    dotnet build CodenameX1.csproj -v minimal
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
}
finally {
    Pop-Location
}
Write-Host '[OK] C# build'

Write-Host ''
Write-Host 'Ready. Run .\Launch-X1.bat from repo root, or open godot/project.godot in Godot Mono editor and press F5.'
