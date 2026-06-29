# 代号X1 启动脚本 — 必须使用 Godot Mono（C# 版）
param(
    [switch]$BuildOnly
)

$ErrorActionPreference = 'Stop'
$ProjectRoot = $PSScriptRoot
. (Join-Path $ProjectRoot 'scripts\env\Find-GodotMono.ps1')

$GodotMono = Find-GodotMonoExe
if (-not $GodotMono) {
    Write-Error @"
找不到 Godot 4.x Mono 可执行文件。

安装方式（任选其一）:
  winget install GodotEngine.GodotEngine.Mono

或设置环境变量 GODOT_MONO 指向 Godot_v*-stable_mono_win64.exe
"@
    exit 1
}

Write-Host "Godot: $GodotMono"
Write-Host 'Building C#...'
Push-Location $ProjectRoot
try {
    dotnet build CodenameX1.csproj -v minimal
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

    $perm = Join-Path $env:USERPROFILE '.nuget\packages\system.security.permissions\8.0.0\lib\net8.0\System.Security.Permissions.dll'
    if (Test-Path -LiteralPath $perm) {
        foreach ($cfg in @('Debug', 'Release')) {
            $godotBin = Join-Path $ProjectRoot ".godot\mono\temp\bin\$cfg"
            if (Test-Path -LiteralPath $godotBin) {
                Copy-Item -LiteralPath $perm -Destination $godotBin -Force
            }
        }
    }

    if ($BuildOnly) {
        Write-Host 'Build OK.'
        exit 0
    }

    Write-Host 'Launching 代号X1...'
    & $GodotMono --path $ProjectRoot @args
    exit $LASTEXITCODE
}
finally {
    Pop-Location
}
