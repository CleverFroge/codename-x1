# 代号X1 启动脚本 — 必须使用 Godot Mono（C# 版）
$GodotMono = "C:\Users\15778\AppData\Local\Microsoft\WinGet\Packages\GodotEngine.GodotEngine.Mono_Microsoft.Winget.Source_8wekyb3d8bbwe\Godot_v4.7-stable_mono_win64\Godot_v4.7-stable_mono_win64.exe"
$Project = Join-Path $PSScriptRoot "project.godot"

if (-not (Test-Path $GodotMono)) {
    Write-Error "找不到 Godot Mono，请安装: winget install GodotEngine.GodotEngine.Mono"
    exit 1
}

Write-Host "Building C#..."
Push-Location $PSScriptRoot
dotnet build | Out-Host
if ($LASTEXITCODE -ne 0) { Pop-Location; exit $LASTEXITCODE }
Pop-Location

Write-Host "Launching 代号X1..."
& $GodotMono --path $PSScriptRoot $args
