using Godot;

namespace CodenameX1.World;

public sealed class WorldSnapshotStore
{
	private const int FormatMagic = 0x58315731;
	private const int FormatVersion = 1;

	private readonly string _directory;

	public WorldState World { get; }
	public int Seed { get; }

	private WorldSnapshotStore(WorldState world, int seed)
	{
		World = world;
		Seed = seed;
		_directory = ProjectSettings.GlobalizePath($"user://worlds/world-{seed}");
	}

	public static WorldSnapshotStore Create(WorldState world, int seed)
	{
		var store = new WorldSnapshotStore(world, seed);
		Directory.CreateDirectory(store._directory);
		store.WriteMetadata();
		foreach (var chunk in world.Chunks)
			store.WriteChunk(chunk);
		return store;
	}

	public static bool TryOpen(int seed, out WorldSnapshotStore? store)
	{
		store = null;
		string directory = ProjectSettings.GlobalizePath($"user://worlds/world-{seed}");
		string metadataPath = Path.Combine(directory, "world.bin");
		if (!File.Exists(metadataPath) || !File.Exists(Path.Combine(directory, "chunk-0-0.bin")))
			return false;

		using var reader = new BinaryReader(File.OpenRead(metadataPath));
		if (reader.ReadInt32() != FormatMagic || reader.ReadInt32() != FormatVersion)
			return false;
		if (reader.ReadInt32() != seed)
			return false;

		var world = new WorldState(reader.ReadInt32(), reader.ReadInt32(), allocateTiles: false)
		{
			WorldSurface = reader.ReadInt32(),
			RockLayer = reader.ReadInt32(),
		};
		store = new WorldSnapshotStore(world, seed);
		return true;
	}

	public void EnsureLoaded(IEnumerable<ChunkCoord> coordinates)
	{
		foreach (var coord in coordinates)
		{
			var chunk = World.GetChunk(coord);
			if (!chunk.IsLoaded)
				ReadChunk(chunk);
		}
	}

	public void UnloadOutside(IReadOnlySet<ChunkCoord> keepLoaded)
	{
		foreach (var chunk in World.Chunks)
		{
			if (!chunk.IsLoaded || keepLoaded.Contains(chunk.Coord))
				continue;

			if (chunk.IsDirty)
				WriteChunk(chunk);
			chunk.Unload();
		}
	}

	public void FlushLoadedDirtyChunks()
	{
		foreach (var chunk in World.Chunks)
		{
			if (chunk.IsLoaded && chunk.IsDirty)
				WriteChunk(chunk);
		}
	}

	private void WriteMetadata()
	{
		using var writer = new BinaryWriter(File.Create(Path.Combine(_directory, "world.bin")));
		writer.Write(FormatMagic);
		writer.Write(FormatVersion);
		writer.Write(Seed);
		writer.Write(World.MaxTilesX);
		writer.Write(World.MaxTilesY);
		writer.Write(World.WorldSurface);
		writer.Write(World.RockLayer);
	}

	private void WriteChunk(WorldChunk chunk)
	{
		using var writer = new BinaryWriter(File.Create(ChunkPath(chunk.Coord)));
		writer.Write(chunk.Width);
		writer.Write(chunk.Height);
		for (int y = 0; y < chunk.Height; y++)
		for (int x = 0; x < chunk.Width; x++)
		{
			ref var tile = ref chunk.Tile(x, y);
			writer.Write(tile.Active);
			writer.Write(tile.Type);
			writer.Write(tile.Liquid);
			writer.Write(tile.Lava);
			writer.Write(tile.Honey);
		}
		chunk.MarkClean();
	}

	private void ReadChunk(WorldChunk chunk)
	{
		using var reader = new BinaryReader(File.OpenRead(ChunkPath(chunk.Coord)));
		int width = reader.ReadInt32();
		int height = reader.ReadInt32();
		if (width != chunk.Width || height != chunk.Height)
			throw new InvalidDataException($"Chunk {chunk.Coord} has incompatible dimensions.");

		var tiles = new WorldTile[width * height];
		for (int y = 0; y < height; y++)
		for (int x = 0; x < width; x++)
		{
			ref var tile = ref tiles[y * width + x];
			tile.Active = reader.ReadBoolean();
			tile.Type = reader.ReadInt32();
			tile.Liquid = reader.ReadByte();
			tile.Lava = reader.ReadBoolean();
			tile.Honey = reader.ReadBoolean();
		}
		chunk.Load(tiles);
	}

	private string ChunkPath(ChunkCoord coord) =>
		Path.Combine(_directory, $"chunk-{coord.X}-{coord.Y}.bin");
}
