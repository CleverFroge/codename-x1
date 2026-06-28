# Fix regressions and remaining ILSpy / API port artifacts.
param(
    [string]$SourceRoot = "$PSScriptRoot\..\..\..\godot\TerrariaVanilla\source"
)

$files = Get-ChildItem -Path $SourceRoot -Recurse -Filter *.cs
$changed = 0

foreach ($file in $files) {
    $text = [System.IO.File]::ReadAllText($file.FullName)
    $orig = $text

    # ILSpy explicit implicit operator calls
    $text = $text -replace 'Vector2D\.op_Implicit\(', '(Vector2D)('

    # fix_monogame_api.ps1 wrongly wrapped DrawData ctor args (vec.Floor() inside ctor)
    $text = [regex]::Replace($text,
        'Utils\.Floor\(new DrawData\(([^,]+),\s*(\w+)\)\s*(\+[^,]+),',
        'new DrawData($1, Utils.Floor($2)$3,')

    # Named DrawData ctor: position arg wrongly wrapped
    $text = [regex]::Replace($text,
        'Utils\.Floor\(new DrawData\((.+?position:\s*)([^,\)]+)\),\s*(sourceRect:)',
        'new DrawData($1Utils.Floor($2), $3')

    # DrawData(tex, (expr)), ...
    $text = [regex]::Replace($text,
        'Utils\.Floor\(new DrawData\(([^,]+),\s*(\([^)]+\))\),\s*',
        'new DrawData($1, Utils.Floor($2), ')

    # DrawData(tex, (expr)) + offset, ...
    $text = [regex]::Replace($text,
        'Utils\.Floor\(new DrawData\(([^,]+),\s*(\([^)]+\))\)\s*\+([^,]+),\s*',
        'new DrawData($1, Utils.Floor(($2) + $3), ')

    # Steamworks.NET: output parameters use out, not ref
    if ($file.FullName -match 'Terraria\.Social\.(Steam|Base)\\') {
        $lines = $text -split "`n"
        for ($i = 0; $i -lt $lines.Count; $i++) {
            $line = $lines[$i]
            if ($line -match 'Utils\.Swap\(') { continue }
            if ($line -match 'GetAuthSessionTicket') { continue }
            if ($line -match '\bSteam[A-Za-z0-9_]+\.') {
                $lines[$i] = $line -replace ', ref ', ', out '
                $lines[$i] = $lines[$i] -replace '\(ref ', '(out '
            }
        }
        $text = $lines -join "`n"
    }

    if ($text -ne $orig) {
        [System.IO.File]::WriteAllText($file.FullName, $text)
        $changed++
    }
}

Write-Host "Fixed port artifacts in $changed files"
