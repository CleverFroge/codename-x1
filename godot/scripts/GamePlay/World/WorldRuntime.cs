namespace CodenameX1.World;

public sealed class WorldRuntime
{
	public const int ActiveRadius = 2;
	public const int RetainedRadius = ActiveRadius + 1;

	private const double FixedStepSeconds = 1.0 / 60.0;
	private const int MaxTicksPerUpdate = 4;

	private readonly WorldState _world;
	private readonly HashSet<ChunkCoord> _activeChunks = new();
	private readonly HashSet<ChunkCoord> _retainedChunks = new();
	private ChunkCoord? _playerChunk;
	private double _tickAccumulator;

	public IReadOnlySet<ChunkCoord> ActiveChunks => _activeChunks;
	public IReadOnlySet<ChunkCoord> RetainedChunks => _retainedChunks;
	public event Action<IReadOnlySet<ChunkCoord>>? Tick;

	public WorldRuntime(WorldState world)
	{
		_world = world;
	}

	public void Update(ChunkCoord playerChunk, double delta)
	{
		if (_playerChunk != playerChunk)
		{
			_playerChunk = playerChunk;
			PopulateChunks(_activeChunks, playerChunk, ActiveRadius);
			PopulateChunks(_retainedChunks, playerChunk, RetainedRadius);
		}

		_tickAccumulator += delta;
		for (int ticks = 0; _tickAccumulator >= FixedStepSeconds && ticks < MaxTicksPerUpdate; ticks++)
		{
			_tickAccumulator -= FixedStepSeconds;
			Tick?.Invoke(_activeChunks);
		}
		if (_tickAccumulator >= FixedStepSeconds)
			_tickAccumulator = 0;
	}

	private void PopulateChunks(HashSet<ChunkCoord> chunks, ChunkCoord center, int radius)
	{
		chunks.Clear();
		int minX = Math.Max(0, center.X - radius);
		int minY = Math.Max(0, center.Y - radius);
		int maxX = Math.Min(_world.ChunkCountX - 1, center.X + radius);
		int maxY = Math.Min(_world.ChunkCountY - 1, center.Y + radius);
		for (int y = minY; y <= maxY; y++)
		for (int x = minX; x <= maxX; x++)
			chunks.Add(new ChunkCoord(x, y));
	}
}
