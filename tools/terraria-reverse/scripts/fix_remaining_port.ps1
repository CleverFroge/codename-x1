# Additional MonoGame / .NET 8 / Steamworks port fixes (idempotent).
param(
    [string]$SourceRoot = "$PSScriptRoot\..\..\..\godot\TerrariaVanilla\source"
)

$files = Get-ChildItem -Path $SourceRoot -Recurse -Filter *.cs
$changed = 0

foreach ($file in $files) {
    $text = [System.IO.File]::ReadAllText($file.FullName)
    $orig = $text

    # Steam Callback dispatch delegate type
    $text = [regex]::Replace($text, '\(DispatchDelegate<([^>]+)>\)', '(Callback<$1>.DispatchDelegate)')

    # JToken / dictionary TryGetValue uses out on .NET
    $text = $text -replace '\.TryGetValue\(([^,]+),\s*ref\s', '.TryGetValue($1, out '
    $text = $text -replace 'TryGetId\(([^,]+),\s*ref\s', 'TryGetId($1, out '
    $text = $text -replace 'TryGetName\(([^,]+),\s*ref\s', 'TryGetName($1, out '

    # Newtonsoft protected member access via explicit base
    if ($file.Name -eq 'EasyDeserializationJsonContractResolver.cs') {
        $text = $text -replace '\(\(DefaultContractResolver\)this\)\.', 'base.'
    }

    # MonoGame GraphicsAdapter uses Description instead of DeviceName (not WinForms Screen)
    $text = $text -replace 'adapter\.DeviceName', 'adapter.Description'

    # MonoGame Texture2D ctor: mipMap named param -> positional bool
    $text = $text -replace 'new Texture2D\(([^,]+),\s*([^,]+),\s*([^,]+),\s*mipMap:\s*false\s*,\s*SurfaceFormat\.Color\)',
        'new Texture2D($1, $2, $3, false, SurfaceFormat.Color)'

    # MonoGame removed ContentLost on dynamic buffers
    $text = [regex]::Replace($text, '\._vertexBuffer\.ContentLost \+= delegate\s*\{[^}]*\};', '')
    $text = [regex]::Replace($text, '\._indexBuffer\.ContentLost \+= delegate\s*\{[^}]*\};', '')
    $text = [regex]::Replace($text, '_vertexBuffer\.ContentLost \+= delegate\s*\{[^}]*\};', '')
    $text = [regex]::Replace($text, '_indexBuffer\.ContentLost \+= delegate\s*\{[^}]*\};', '')

    # ShaderData string EffectParameter: SetValue overload differs
    if ($file.Name -eq 'ShaderData.cs') {
        $text = $text -replace 'new EffectParameter<string>\(\(v\) => param\.SetValue\(v\)\)',
            'new EffectParameter<string>((Action<string>)(s => param.SetValue(s)))'
    }

    # SocialAPI: Platform field shadows ReLogic.OS.Platform
    if ($file.Name -eq 'SocialAPI.cs') {
        $text = $text -replace '(?<![.\w])Platform\.IsWindows', 'ReLogic.OS.Platform.IsWindows'
    }

    # QuickLoad MessageBox ambiguity
    if ($file.Name -eq 'QuickLoad.cs') {
        $text = $text -replace '(?<![.\w])MessageBox\.', 'Terraria.Utilities.MessageBox.'
    }

    # Lobby ELobbyType from bool decompile artifact
    if ($file.Name -eq 'Lobby.cs') {
        $text = $text -replace '\(ELobbyType\)\(!inviteOnly\)',
            '(inviteOnly ? ELobbyType.k_ELobbyTypeFriendsOnly : ELobbyType.k_ELobbyTypePublic)'
    }

    # GetAuthSessionTicket: 4th parameter is ref, not out
    if ($file.Name -eq 'NetClientSocialModule.cs') {
        $text = $text -replace 'GetAuthSessionTicket\(_authData, _authData\.Length, out _authDataLength, out val\)',
            'GetAuthSessionTicket(_authData, _authData.Length, out _authDataLength, ref val)'
    }

    # OGGAudioTrack Tags.All type
    if ($file.Name -eq 'OGGAudioTrack.cs') {
        $text = $text -replace 'IDictionary<string, IList<string>> all = _vorbisReader\.Tags\.All;',
            'var all = _vorbisReader.Tags.All;'
        $text = $text -replace 'private void TryReadingTag\(IDictionary<string, IList<string>> tags',
            'private void TryReadingTag(System.Collections.Generic.IReadOnlyDictionary<string, System.Collections.Generic.IReadOnlyList<string>> tags'
    }

    # GlyphTagHandler: switch on int delta, not enum
    if ($file.Name -eq 'GlyphTagHandler.cs') {
        $text = $text -replace 'switch \(inputTypeForHandle - 5\)', 'switch ((int)inputTypeForHandle - 5)'
    }

    # WorldGenConfiguration ILSpy nullable placeholder (only if not already patched)
    if ($file.Name -eq 'WorldGenConfiguration.cs' -and $text -match '_003F') {
        $text = [regex]::Replace($text,
            '(?s)public WorldGenConfiguration\(JObject configurationRoot\)\s*: base\(configurationRoot\)\s*\{.*?\n\t\}',
            @'
public WorldGenConfiguration(JObject configurationRoot)
		: base(configurationRoot)
	{
		JObject biomeRoot = (JObject)configurationRoot["Biomes"];
		if (biomeRoot == null)
		{
			biomeRoot = new JObject();
		}
		_biomeRoot = biomeRoot;
		JObject passRoot = (JObject)configurationRoot["Passes"];
		if (passRoot == null)
		{
			passRoot = new JObject();
		}
		_passRoot = passRoot;
	}
'@)
    }

    # DualDungeonLayoutProvider Point -> Vector2 for ClosestPointOnLine
    if ($file.Name -eq 'DualDungeonLayoutProvider.cs') {
        $text = $text -replace '\(Vector2D\)\(item2\.Center\)\.ClosestPointOnLine\(',
            '(Vector2D)Utils.ClosestPointOnLine(new Vector2(item2.Center.X, item2.Center.Y), '
        $text = $text -replace '\(Vector2D\)Utils\.ClosestPointOnLine\(\(Vector2\)item2\.Center, ',
            '(Vector2D)Utils.ClosestPointOnLine(new Vector2(item2.Center.X, item2.Center.Y), '
    }

    # DebugKeyboard protected GetUnprocessedLedColor
    if ($file.Name -eq 'DebugKeyboard.cs') {
        $text = $text -replace 'new Color\(\(\(RgbDevice\)this\)\.GetUnprocessedLedColor\(i\)\)',
            'new Color(GetUnprocessedLedColor(i))'
    }

    # TileDrawing decompile duplicate out var
    if ($file.Name -eq 'TileDrawing.cs') {
        $text = $text -replace 'out var windTimeLeft, out var directionX, out directionX\)',
            'out var windTimeLeft, out var directionX, out var directionY)'
    }

    # Color(int, byte) mixed ambiguity
    $text = [regex]::Replace($text, 'new Color\((\d+)\s*,\s*\(byte\)([^,]+),\s*(\d+)\s*,\s*([^)]+)\)',
        'new Color((byte)$1, (byte)$2, (byte)$3, $4)')
    $text = [regex]::Replace($text, 'new Microsoft\.Xna\.Framework\.Color\((\d+)\s*,\s*\(byte\)([^,]+),\s*(\d+)\s*,\s*([^)]+)\)',
        'new Microsoft.Xna.Framework.Color((byte)$1, (byte)$2, (byte)$3, $4)')
    $text = [regex]::Replace($text, 'new Color\((\d+)\s*,\s*(\d+)\s*,\s*(\d+)\s*,\s*([a-zA-Z_][\w.]*)\)',
        'new Color((byte)$1, (byte)$2, (byte)$3, $4)')

    # byte mouseTextColor mixed with int literal 0
    $text = $text -replace 'new Microsoft\.Xna\.Framework\.Color\(mouseTextColor, mouseTextColor, mouseTextColor, 0\)',
        'new Microsoft.Xna.Framework.Color(mouseTextColor, mouseTextColor, mouseTextColor, (byte)0)'
    $text = $text -replace 'new Microsoft\.Xna\.Framework\.Color\(\(byte\)([^,]+), \(byte\)([^,]+), 0, mouseTextColor\)',
        'new Microsoft.Xna.Framework.Color((byte)$1, (byte)$2, (byte)0, mouseTextColor)'

    # Dust type id must not be byte-cast when > 255
    $text = $text -replace '\(byte\)267\b', '267'

    if ($text -ne $orig) {
        [System.IO.File]::WriteAllText($file.FullName, $text)
        $changed++
    }
}

Write-Host "Fixed remaining port issues in $changed files"
