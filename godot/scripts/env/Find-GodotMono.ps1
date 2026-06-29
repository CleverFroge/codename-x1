# 查找 Godot .NET (Mono) 可执行文件。可被 Launch-X1.ps1 dot-source，也可单独运行。
function Find-GodotMonoExe {
    if ($env:GODOT_MONO -and (Test-Path -LiteralPath $env:GODOT_MONO)) {
        return (Resolve-Path -LiteralPath $env:GODOT_MONO).Path
    }

    $searchRoots = @(
        Join-Path $env:LOCALAPPDATA 'Microsoft\WinGet\Packages'
        Join-Path $env:LOCALAPPDATA 'Programs\Godot'
        Join-Path ${env:ProgramFiles} 'Godot'
        Join-Path ${env:ProgramFiles(x86)} 'Godot'
    )

    $candidates = @()
    foreach ($root in $searchRoots) {
        if (-not (Test-Path -LiteralPath $root)) { continue }
        $candidates += Get-ChildItem -LiteralPath $root -Recurse -Filter 'Godot_v*-stable_mono_win64.exe' -ErrorAction SilentlyContinue
    }

    if ($candidates.Count -gt 0) {
        return ($candidates | Sort-Object { [version]($_.BaseName -replace '^Godot_v','' -replace '-stable_mono_win64$','') } -Descending | Select-Object -First 1).FullName
    }

    foreach ($name in @('godot', 'Godot', 'Godot_mono')) {
        $cmd = Get-Command $name -ErrorAction SilentlyContinue
        if ($cmd -and $cmd.Source -match 'mono') { return $cmd.Source }
    }

    return $null
}

if ($MyInvocation.InvocationName -ne '.') {
    $exe = Find-GodotMonoExe
    if ($exe) { Write-Output $exe; exit 0 }
    Write-Error 'Godot 4.x Mono 未找到。请运行: winget install GodotEngine.GodotEngine.Mono'
    exit 1
}
