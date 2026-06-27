# XNB 资源解包脚本（基于 xnbcli）
# 用法: ./unpack_assets.ps1 "D:\Program Files\Steam\steamapps\common\Terraria\Content\Images" "../output_png"
param(
    [Parameter(Mandatory=$true)]
    [string]$ContentDir,
    [string]$OutputDir = "../output_png"
)

$xnbcli = "$PSScriptRoot\..\xnbcli"
$inputDir = "$xnbcli\input"
$outDir = "$xnbcli\output"

# 清空 input/output
Remove-Item "$inputDir\*" -ErrorAction SilentlyContinue
Remove-Item "$outDir\*" -Recurse -ErrorAction SilentlyContinue

# 复制 xnb 到 input
Write-Host "Copying XNB files from $ContentDir ..."
Copy-Item "$ContentDir\*.xnb" $inputDir -Force
$count = (Get-ChildItem $inputDir).Count
Write-Host "Copied $count files. Unpacking..."

# 运行 xnbcli
Push-Location $xnbcli
node xnbcli.js unpack input output
Pop-Location

# 复制 PNG 到最终输出
New-Item -ItemType Directory -Force -Path $OutputDir | Out-Null
Copy-Item "$outDir\*.png" $OutputDir -Force -ErrorAction SilentlyContinue
$pngCount = (Get-ChildItem $OutputDir -Filter *.png).Count
Write-Host "Done. $pngCount PNG files exported to $OutputDir"
