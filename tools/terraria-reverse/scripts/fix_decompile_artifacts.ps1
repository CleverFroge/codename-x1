# Fix ILSpy decompiler artifacts that are invalid C# but compile in original IL.
# Run after copy_source_full.ps1 / decompile_and_port.ps1
param(
    [string]$SourceRoot = "$PSScriptRoot\..\..\..\TerrariaVanilla\source"
)

$files = Get-ChildItem -Path $SourceRoot -Recurse -Filter *.cs
$changed = 0

foreach ($file in $files) {
    $text = [System.IO.File]::ReadAllText($file.FullName)
    $orig = $text

    # APIDispatchDelegate is nested in CallResult<T> (ILSpy flattens incorrectly)
    $text = [regex]::Replace($text, 'APIDispatchDelegate<([^>]+)>', 'CallResult<$1>.APIDispatchDelegate')
    $text = [regex]::Replace($text, '\(\(([\w.]+)\)\(ref (\w+)\)\)\._002Ector\(', '$2 = new $1(')

    # ((Type)(ref var)).Member -> var.Member
    $text = [regex]::Replace($text, '\(\(([\w.]+)\)\(ref (\w+)\)\)\.', '$2.')

    # ((object)(EnumType)(ref var)).ToString() -> var.ToString()
    $text = [regex]::Replace($text, '\(\(object\)\(([\w.]+)\)\(ref ([^)]+)\)\)\.ToString\(\)', '$2.ToString()')

    # ItemPair<T> from ILSpy -> SlotVector<T>.ItemPair
    $text = [regex]::Replace($text, 'ItemPair<([\w]+)>', 'SlotVector<$1>.ItemPair')
    $text = [regex]::Replace($text, '\(IEnumerable<SlotVector<([\w]+)>\.ItemPair>\)', '(IEnumerable<SlotVector<$1>.ItemPair>)')

    if ($text -ne $orig) {
        [System.IO.File]::WriteAllText($file.FullName, $text)
        $changed++
    }
}

Write-Host "Fixed $changed files under $SourceRoot"
