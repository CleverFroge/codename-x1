namespace CodenameX1.World;

public sealed class WorldState
{
	public readonly int MaxTilesX;
	public readonly int MaxTilesY;
	public readonly WorldTile[,] Tiles;
	public readonly GenVars GenVars = new();

	public int WorldSurface;
	public int RockLayer;

	public WorldState(int width, int height)
	{
		MaxTilesX = width;
		MaxTilesY = height;
		Tiles = new WorldTile[width, height];
		for (int x = 0; x < width; x++)
		for (int y = 0; y < height; y++)
			Tiles[x, y] = new WorldTile();
	}

	public ref WorldTile Tile(int x, int y) => ref Tiles[x, y];

	public bool InBounds(int x, int y) =>
		x >= 0 && x < MaxTilesX && y >= 0 && y < MaxTilesY;
}
