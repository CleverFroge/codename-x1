namespace CodenameX1.World;

public sealed class WorldChunk
{
	private WorldTile[]? _tiles;

	public ChunkCoord Coord { get; }
	public int Width { get; }
	public int Height { get; }
	public bool IsLoaded => _tiles != null;
	public bool IsDirty { get; private set; }

	public WorldChunk(ChunkCoord coord, int width, int height, bool allocateTiles)
	{
		Coord = coord;
		Width = width;
		Height = height;
		if (allocateTiles)
			_tiles = new WorldTile[width * height];
	}

	public ref WorldTile Tile(int localX, int localY) =>
		ref GetTiles()[localY * Width + localX];

	public void MarkDirty() => IsDirty = true;

	public void MarkClean() => IsDirty = false;

	public WorldTile[] Unload()
	{
		var tiles = GetTiles();
		_tiles = null;
		return tiles;
	}

	public void Load(WorldTile[] tiles)
	{
		if (tiles.Length != Width * Height)
			throw new ArgumentException("Chunk tile count does not match its dimensions.", nameof(tiles));

		_tiles = tiles;
		IsDirty = false;
	}

	private WorldTile[] GetTiles() =>
		_tiles ?? throw new InvalidOperationException($"Chunk {Coord} is not loaded.");
}
