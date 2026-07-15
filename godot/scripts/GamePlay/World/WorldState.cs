namespace CodenameX1.World;

public sealed class WorldState
{
	public readonly int MaxTilesX;
	public readonly int MaxTilesY;
	private readonly Dictionary<ChunkCoord, WorldChunk> _chunks = new();

	public int WorldSurface;
	public int RockLayer;
	public int ChunkCountX { get; }
	public int ChunkCountY { get; }

	public WorldState(int width, int height, bool allocateTiles = true)
	{
		MaxTilesX = width;
		MaxTilesY = height;
		ChunkCountX = (width + WorldConfig.ChunkTileSize - 1) / WorldConfig.ChunkTileSize;
		ChunkCountY = (height + WorldConfig.ChunkTileSize - 1) / WorldConfig.ChunkTileSize;
		for (int y = 0; y < ChunkCountY; y++)
		for (int x = 0; x < ChunkCountX; x++)
		{
			var coord = new ChunkCoord(x, y);
			int chunkWidth = Math.Min(WorldConfig.ChunkTileSize, width - x * WorldConfig.ChunkTileSize);
			int chunkHeight = Math.Min(WorldConfig.ChunkTileSize, height - y * WorldConfig.ChunkTileSize);
			_chunks.Add(coord, new WorldChunk(coord, chunkWidth, chunkHeight, allocateTiles));
		}
	}

	public IEnumerable<WorldChunk> Chunks => _chunks.Values;

	public bool InBounds(int x, int y) =>
		x >= 0 && x < MaxTilesX && y >= 0 && y < MaxTilesY;

	public WorldChunk GetChunk(ChunkCoord coord) => _chunks[coord];

	public ref WorldTile Tile(int x, int y)
	{
		if (!InBounds(x, y))
			throw new ArgumentOutOfRangeException();

		var chunkCoord = WorldCoordinates.TileToChunk(x, y);
		var chunk = _chunks[chunkCoord];
		int localX = x - chunkCoord.X * WorldConfig.ChunkTileSize;
		int localY = y - chunkCoord.Y * WorldConfig.ChunkTileSize;
		return ref chunk.Tile(localX, localY);
	}

	public bool IsChunkLoaded(ChunkCoord coord) => _chunks[coord].IsLoaded;

	public bool TryGetTile(int x, int y, out WorldTile tile)
	{
		if (!InBounds(x, y))
		{
			tile = default;
			return false;
		}

		var chunk = GetChunk(WorldCoordinates.TileToChunk(x, y));
		if (!chunk.IsLoaded)
		{
			tile = default;
			return false;
		}

		tile = Tile(x, y);
		return true;
	}

	public bool SetTile(int x, int y, WorldTile tile)
	{
		if (!InBounds(x, y))
			return false;

		var chunk = GetChunk(WorldCoordinates.TileToChunk(x, y));
		if (!chunk.IsLoaded)
			return false;

		Tile(x, y) = tile;
		chunk.MarkDirty();
		return true;
	}
}
