# MonoGame Vector2.Floor() is void; Terraria uses Utils.Floor extension returning Vector2.
param(
    [string]$SourceRoot = "$PSScriptRoot\..\..\..\TerrariaVanilla\source"
)

$files = Get-ChildItem -Path $SourceRoot -Recurse -Filter *.cs
$changed = 0

foreach ($file in $files) {
    $text = [System.IO.File]::ReadAllText($file.FullName)
    $orig = $text

    # vec.Floor() -> Utils.Floor(vec), skip already-wrapped calls
    $text = [regex]::Replace($text,
        '(?<!Utils\.Floor\()\b([\w\d_]+(?:\.[\w\d_]+|\[[^\]]+\])*)\.Floor\(\)',
        'Utils.Floor($1)')

    # (expr).Floor() — chained / parenthesized Vector2 expressions
    for ($pass = 0; $pass -lt 8 -and $text -match '\)\.Floor\(\)'; $pass++) {
        $text = [regex]::Replace($text,
            '\(([^()]*(?:\([^()]*\)[^()]*)*)\)\.Floor\(\)',
            'Utils.Floor($1)')
    }

    if ($text -ne $orig) {
        [System.IO.File]::WriteAllText($file.FullName, $text)
        $changed++
    }
}

Write-Host "Fixed Vector2.Floor calls in $changed files"
