# 1. 安装 ILSpy 命令行反编译器（需要 .NET SDK）
dotnet tool install -g ilspycmd --version 8.2.0.7535

# 2. 反编译 Terraria.exe 为 C# 源码
# 用法: ./decompile.ps1 "D:\Program Files\Steam\steamapps\common\Terraria\Terraria.exe" "../source"
param(
    [Parameter(Mandatory=$true)]
    [string]$TerrariaExe,
    [string]$OutputDir = "../source"
)

$env:PATH = "$env:PATH;$env:USERPROFILE\.dotnet\tools"

if (-not (Test-Path $TerrariaExe)) {
    Write-Error "Terraria.exe not found at: $TerrariaExe"
    exit 1
}

Write-Host "Decompiling $TerrariaExe -> $OutputDir ..."
New-Item -ItemType Directory -Force -Path $OutputDir | Out-Null
ilspycmd $TerrariaExe -p -o $OutputDir
Write-Host "Done. Output: $OutputDir"
