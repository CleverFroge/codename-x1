# Split Projectile.SetDefaults if-else chain to fix Roslyn CS8078 (expression too complex).
param(
    [string]$File = "$PSScriptRoot\..\..\..\godot\TerrariaVanilla\source\Terraria\Projectile.cs",
    [int]$BlocksPerChunk = 250
)

$lines = [System.IO.File]::ReadAllLines($File)
$setDefaultsStart = 435
$setDefaultsEnd = 10078
$preamble = $lines[$setDefaultsStart..545]
$chainStart = 546
$chainEnd = 10077

$blocks = @()
$i = $chainStart
while ($i -le $chainEnd) {
    $line = $lines[$i]
    if ($line -match '^\t\t(if \(type ==|else if \(type ==)') {
        $blockLines = New-Object System.Collections.Generic.List[string]
        [void]$blockLines.Add($line)
        $depth = 0
        $i++
        while ($i -le $chainEnd) {
            $l = $lines[$i]
            [void]$blockLines.Add($l)
            $depth += ([regex]::Matches($l, '\{')).Count
            $depth -= ([regex]::Matches($l, '\}')).Count
            if ($depth -le 0) { break }
            $i++
        }
        $blocks += ,@($blockLines.ToArray())
    } else {
        $i++
    }
}

Write-Host "Found $($blocks.Count) type blocks"

$chunks = @()
for ($c = 0; $c -lt $blocks.Count; $c += $BlocksPerChunk) {
    $end = [Math]::Min($c + $BlocksPerChunk - 1, $blocks.Count - 1)
    $chunks += ,@($blocks[$c..$end])
}

function Fix-FirstBlock([string[]]$block) {
    if ($block[0] -match '^\t\telse if \(type ==') {
        $copy = @($block)
        $copy[0] = $copy[0] -replace '^\t\telse if ', "`t`tif "
        return $copy
    }
    return $block
}

$out = New-Object System.Collections.Generic.List[string]
$out.AddRange([string[]]$lines[0..($setDefaultsStart - 1)])

$out.Add("`tpublic void SetDefaults(int Type)")
$out.Add("`t{")
foreach ($pl in $preamble[2..($preamble.Length - 1)]) { [void]$out.Add($pl) }
for ($ci = 0; $ci -lt $chunks.Count; $ci++) {
    [void]$out.Add("`t`tif (SetDefaults_Chunk$ci()) return;")
}
[void]$out.Add("`t}")

for ($ci = 0; $ci -lt $chunks.Count; $ci++) {
    [void]$out.Add("")
    [void]$out.Add("`tprivate bool SetDefaults_Chunk$ci()")
    [void]$out.Add("`t{")
    for ($bi = 0; $bi -lt $chunks[$ci].Count; $bi++) {
        $block = $chunks[$ci][$bi]
        if ($bi -eq 0 -and $ci -gt 0) { $block = Fix-FirstBlock $block }
        for ($li = 0; $li -lt $block.Length; $li++) {
            if ($li -eq $block.Length - 1) {
                [void]$out.Add("`t`t`treturn true;")
            }
            [void]$out.Add($block[$li])
        }
    }
    [void]$out.Add("`t`treturn false;")
    [void]$out.Add("`t}")
}

$tailStart = $setDefaultsEnd + 1
$out.AddRange([string[]]$lines[$tailStart..($lines.Length - 1)])
[System.IO.File]::WriteAllLines($File, $out)
Write-Host "Split SetDefaults into $($chunks.Count) chunks"
