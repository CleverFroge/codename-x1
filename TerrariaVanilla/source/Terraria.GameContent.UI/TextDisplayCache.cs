using Terraria.GameInput;

namespace Terraria.GameContent.UI;

public class TextDisplayCache
{
	private string _originalText;

	private int _lastScreenWidth;

	private int _lastScreenHeight;

	private InputMode _lastInputMode;

	public string[] TextLines { get; private set; }

	public int AmountOfLines { get; private set; }

	public void PrepareCache(string text)
	{
		if (false | (Main.screenWidth != _lastScreenWidth) | (Main.screenHeight != _lastScreenHeight) | (_originalText != text) | (PlayerInput.CurrentInputMode != _lastInputMode))
		{
			_lastScreenWidth = Main.screenWidth;
			_lastScreenHeight = Main.screenHeight;
			_originalText = text;
			_lastInputMode = PlayerInput.CurrentInputMode;
			TextLines = Utils.WordwrapString(text, FontAssets.MouseText.Value, 460, 10, out var lineAmount);
			AmountOfLines = lineAmount;
		}
	}
}
