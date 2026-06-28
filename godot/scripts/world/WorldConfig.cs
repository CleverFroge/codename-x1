namespace CodenameX1.World;

public enum WorldSize
{
	Dev,
	HalfSmall,
	Small,
}

public static class WorldConfig
{
	public const int TilePixelSize = 2;

	public static (int W, int H) GetSize(WorldSize size) => size switch
	{
		WorldSize.Dev => (840, 240),
		WorldSize.HalfSmall => (2100, 600),
		WorldSize.Small => (4200, 1200),
		_ => (840, 240),
	};
}
