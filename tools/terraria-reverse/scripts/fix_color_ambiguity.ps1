# Fix MonoGame Color constructor ambiguity on .NET 8 (CS0121).
param(
    [string]$SourceRoot = "$PSScriptRoot\..\..\..\TerrariaVanilla\source"
)

$files = Get-ChildItem -Path $SourceRoot -Recurse -Filter *.cs
$changed = 0

foreach ($file in $files) {
    $text = [System.IO.File]::ReadAllText($file.FullName)
    $orig = $text

    # new Color(250, 250, 250, 200)
    $text = [regex]::Replace($text, 'new Color\((\d+)\s*,\s*(\d+)\s*,\s*(\d+)\s*,\s*(\d+)\s*\)',
        'new Color((byte)$1, (byte)$2, (byte)$3, (byte)$4)')

    # new Color(expr.R, expr.G, expr.B, literal) — byte props still need last arg cast
    $text = [regex]::Replace($text, 'new Color\(([^,]+),\s*([^,]+),\s*([^,]+),\s*(\d+)\s*\)',
        'new Color($1, $2, $3, (byte)$4)')

    # new Color(int, int, int, byteExpr) — first three int literals
    $text = [regex]::Replace($text, 'new Color\((\d+)\s*,\s*(\d+)\s*,\s*(\d+)\s*,\s*\(byte\)([^)]+)\)',
        'new Color((byte)$1, (byte)$2, (byte)$3, (byte)$4)')

    # Microsoft.Xna.Framework.Color mixed byte channels + int alpha literal
    $text = [regex]::Replace($text,
        'new Microsoft\.Xna\.Framework\.Color\(([^,]+\.R),\s*([^,]+\.G),\s*([^,]+\.B),\s*(\d+)\)',
        'new Microsoft.Xna.Framework.Color($1, $2, $3, (byte)$4)')
    $text = [regex]::Replace($text,
        'new Microsoft\.Xna\.Framework\.Color\((\d+)\s*,\s*(\d+)\s*,\s*(\d+)\s*,\s*([a-zA-Z_]\w*)\)',
        'new Microsoft.Xna.Framework.Color((byte)$1, (byte)$2, (byte)$3, $4)')
    $text = [regex]::Replace($text,
        'new Microsoft\.Xna\.Framework\.Color\((\d+)\s*,\s*(\d+)\s*,\s*(\d+)\s*,\s*mouseTextColor\)',
        'new Microsoft.Xna.Framework.Color((byte)$1, (byte)$2, (byte)$3, mouseTextColor)')
    $text = [regex]::Replace($text,
        'new Microsoft\.Xna\.Framework\.Color\((mouseTextColor),\s*(mouseTextColor),\s*(mouseTextColor),\s*0\)',
        'new Microsoft.Xna.Framework.Color($1, $2, $3, (byte)0)')
    $text = [regex]::Replace($text,
        'new Microsoft\.Xna\.Framework\.Color\((\([^\)]+\)),\s*(\([^\)]+\)),\s*0,\s*mouseTextColor\)',
        'new Microsoft.Xna.Framework.Color((byte)$1, (byte)$2, (byte)0, mouseTextColor)')
    $text = [regex]::Replace($text,
        'new Microsoft\.Xna\.Framework\.Color\(([a-zA-Z_]\w*),\s*([a-zA-Z_]\w*),\s*([a-zA-Z_]\w*),\s*0\)',
        'new Microsoft.Xna.Framework.Color($1, $2, $3, (byte)0)')
    $text = [regex]::Replace($text,
        'new Microsoft\.Xna\.Framework\.Color\(([a-zA-Z_]\w*),\s*\1,\s*\1,\s*255\)',
        'new Microsoft.Xna.Framework.Color($1, $1, $1, (byte)255)')

    if ($text -ne $orig) {
        [System.IO.File]::WriteAllText($file.FullName, $text)
        $changed++
    }
}

Write-Host "Fixed Color ambiguity in $changed files"
