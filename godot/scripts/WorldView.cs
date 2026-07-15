using Godot;
using CodenameX1.World;

namespace CodenameX1;

public partial class WorldView : Node2D
{
	private static readonly Color AirColor = new(0.05f, 0.08f, 0.15f);
	private static readonly Color WaterColor = new(0.2f, 0.4f, 0.85f, 0.9f);
	private static readonly Color LavaColor = new(0.9f, 0.25f, 0.05f, 0.95f);
	private static readonly Color HoneyColor = new(0.85f, 0.65f, 0.1f, 0.9f);
	private static readonly Color UnknownTileColor = new(1f, 0f, 1f);

	private static readonly Dictionary<int, Color> TileColors = new()
	{
		[0] = new(0.55f, 0.36f, 0.18f),       // Dirt
		[1] = new(0.45f, 0.45f, 0.48f),       // Stone
		[2] = new(0.25f, 0.55f, 0.2f),        // Grass
		[3] = new(0.9f, 0.88f, 0.85f),        // Wood
		[6] = new(0.55f, 0.5f, 0.42f),        // Iron
		[7] = new(0.7f, 0.45f, 0.3f),         // Copper
		[8] = new(0.85f, 0.72f, 0.2f),        // Gold
		[9] = new(0.75f, 0.78f, 0.82f),       // Silver
		[16] = new(0.2f, 0.2f, 0.28f),        // Obsidian
		[17] = new(0.2f, 0.2f, 0.28f),        // Obsidian (alt)
		[22] = new(0.3f, 0.15f, 0.4f),        // Demonite
		[24] = new(0.25f, 0.15f, 0.6f),       // Mythril
		[25] = new(0.35f, 0.2f, 0.45f),       // Ebonstone
		[27] = new(0.85f, 0.8f, 0.72f),       // Pearlstone
		[33] = new(0.6f, 0.55f, 0.45f),       // Silt
		[35] = new(0.6f, 0.5f, 0.35f),        // Tin
		[36] = new(0.55f, 0.55f, 0.6f),       // Lead
		[37] = new(0.6f, 0.6f, 0.62f),        // Tungsten
		[38] = new(0.75f, 0.65f, 0.3f),       // Platinum
		[40] = new(0.62f, 0.45f, 0.35f),      // Clay
		[41] = new(0.6f, 0.55f, 0.45f),       // Clay (alt)
		[53] = new(0.85f, 0.78f, 0.45f),      // Sand
		[57] = new(0.35f, 0.15f, 0.1f),       // Ash
		[58] = new(0.5f, 0.25f, 0.15f),       // Hellstone
		[59] = new(0.35f, 0.22f, 0.12f),      // Mud
		[60] = new(0.15f, 0.5f, 0.12f),       // Jungle Grass
		[61] = new(0.55f, 0.75f, 0.85f),      // Snow
		[62] = new(0.55f, 0.75f, 0.85f),      // Snow Block
		[63] = new(0.65f, 0.65f, 0.68f),      // Stone Slab
		[65] = new(0.25f, 0.5f, 0.3f),        // Jungle Vine
		[66] = new(0.18f, 0.18f, 0.18f),      // Mushroom
		[67] = new(0.45f, 0.35f, 0.35f),      // Mushroom Grass
		[68] = new(0.5f, 0.8f, 0.85f),        // Ice
		[69] = new(0.35f, 0.55f, 0.65f),      // Corrupt Ice
		[70] = new(0.45f, 0.25f, 0.55f),      // Flesh Ice
		[71] = new(0.55f, 0.75f, 0.85f),      // Hallowed Ice
		[72] = new(0.35f, 0.6f, 0.55f),	      // Chlorophyte
		[77] = new(0.55f, 0.35f, 0.15f),      // Palm Wood
		[81] = new(0.15f, 0.15f, 0.2f),       // Cactus
		[82] = new(0.4f, 0.65f, 0.3f),        // Coral
		[147] = new(0.55f, 0.75f, 0.85f),     // Snow
		[148] = new(0.6f, 0.7f, 0.75f),       // Snow Brick
		[150] = new(0.4f, 0.6f, 0.5f),        // Sandstone
		[151] = new(0.42f, 0.32f, 0.22f),     // Hardened Sand
		[152] = new(0.42f, 0.35f, 0.25f),     // Desert Fossil
		[161] = new(0.42f, 0.38f, 0.28f),     // Hive
		[162] = new(0.35f, 0.42f, 0.32f),     // Lihzahrd Brick
		[163] = new(0.32f, 0.38f, 0.28f),     // Lihzahrd Altar
		[177] = new(0.2f, 0.4f, 0.5f),        // Chlorophyte Ore
		[189] = new(0.92f, 0.95f, 1f),        // Cloud
		[190] = new(0.88f, 0.9f, 0.95f),      // Rain Cloud
		[192] = new(0.75f, 0.62f, 0.32f),     // Sunplate
		[203] = new(0.55f, 0.15f, 0.2f),      // Crimstone
		[204] = new(0.6f, 0.1f, 0.15f),       // Crimtane
		[211] = new(0.2f, 0.2f, 0.25f),       // Obsidian Brick
		[213] = new(0.75f, 0.7f, 0.58f),      // Smooth Sandstone
		[224] = new(0.55f, 0.75f, 0.85f),     // Diamond
		[232] = new(0.35f, 0.38f, 0.42f),     // Glass
		[261] = new(0.55f, 0.52f, 0.5f),      // Granite
		[263] = new(0.88f, 0.84f, 0.8f),      // Marble
	};

	private WorldState? _world;
	private Sprite2D? _legacySprite;
	private readonly Dictionary<ChunkCoord, Sprite2D> _visibleChunks = new();
	private readonly Stack<Sprite2D> _spritePool = new();

	public void SetWorld(WorldState world)
	{
		_world = world;
		ClearVisibleChunks();
		_legacySprite ??= GetNodeOrNull<Sprite2D>("WorldSprite");
		if (_legacySprite != null)
			_legacySprite.Visible = false;
	}

	public void UpdateVisibleChunks(Camera2D camera)
	{
		if (_world == null)
			return;

		var visible = GetVisibleChunks(camera);
		foreach (var coord in visible)
		{
			if (_visibleChunks.ContainsKey(coord))
				continue;

			var sprite = AcquireSprite();
			RenderChunk(sprite, coord);
			_visibleChunks.Add(coord, sprite);
		}

		var noLongerVisible = _visibleChunks
			.Where(pair => !visible.Contains(pair.Key))
			.ToArray();
		foreach (var pair in noLongerVisible)
		{
			ReleaseSprite(pair.Value);
			_visibleChunks.Remove(pair.Key);
		}
	}

	public HashSet<ChunkCoord> GetVisibleChunks(Camera2D camera)
	{
		if (_world == null)
			return new();

		int chunkPixelSize = WorldConfig.ChunkTileSize * WorldConfig.TilePixelSize;
		var viewportSize = camera.GetViewport().GetVisibleRect().Size / camera.Zoom;
		var viewMin = camera.GetScreenCenterPosition() - viewportSize * 0.5f;
		var viewMax = camera.GetScreenCenterPosition() + viewportSize * 0.5f;
		int minX = Mathf.Clamp(Mathf.FloorToInt(viewMin.X / chunkPixelSize) - 1, 0, _world.ChunkCountX - 1);
		int minY = Mathf.Clamp(Mathf.FloorToInt(viewMin.Y / chunkPixelSize) - 1, 0, _world.ChunkCountY - 1);
		int maxX = Mathf.Clamp(Mathf.FloorToInt(viewMax.X / chunkPixelSize) + 1, 0, _world.ChunkCountX - 1);
		int maxY = Mathf.Clamp(Mathf.FloorToInt(viewMax.Y / chunkPixelSize) + 1, 0, _world.ChunkCountY - 1);

		var visible = new HashSet<ChunkCoord>();
		for (int y = minY; y <= maxY; y++)
		for (int x = minX; x <= maxX; x++)
			visible.Add(new ChunkCoord(x, y));
		return visible;
	}

	public void RefreshChunk(ChunkCoord coord)
	{
		if (_visibleChunks.TryGetValue(coord, out var sprite))
			RenderChunk(sprite, coord);
	}

	private void RenderChunk(Sprite2D sprite, ChunkCoord coord)
	{
		var chunk = _world!.GetChunk(coord);
		var data = BuildPixelData(chunk);
		var image = Image.CreateFromData(chunk.Width, chunk.Height, false, Image.Format.Rgba8, data);
		sprite.Texture = ImageTexture.CreateFromImage(image);
		sprite.Position = new Vector2(
			coord.X * WorldConfig.ChunkTileSize * WorldConfig.TilePixelSize,
			coord.Y * WorldConfig.ChunkTileSize * WorldConfig.TilePixelSize);
		sprite.Scale = new Vector2(WorldConfig.TilePixelSize, WorldConfig.TilePixelSize);
	}

	private byte[] BuildPixelData(WorldChunk chunk)
	{
		var data = new byte[chunk.Width * chunk.Height * 4];
		int startX = chunk.Coord.X * WorldConfig.ChunkTileSize;
		int startY = chunk.Coord.Y * WorldConfig.ChunkTileSize;
		for (int y = 0; y < chunk.Height; y++)
		for (int x = 0; x < chunk.Width; x++)
		{
			Color c = GetTileColor(ref _world!.Tile(startX + x, startY + y));
			int i = (y * chunk.Width + x) * 4;
			data[i] = (byte)(c.R * 255);
			data[i + 1] = (byte)(c.G * 255);
			data[i + 2] = (byte)(c.B * 255);
			data[i +3] = (byte)(c.A * 255);
		}

		return data;
	}

	private static Color GetTileColor(ref WorldTile t)
	{
		if (t.Liquid > 0)
		{
			if (t.Lava) return LavaColor;
			if (t.Honey) return HoneyColor;
			return WaterColor;
		}

		if (!t.Active)
			return AirColor;

		return TileColors.GetValueOrDefault(t.Type, UnknownTileColor);
	}

	private Sprite2D AcquireSprite()
	{
		var sprite = _spritePool.Count > 0 ? _spritePool.Pop() : new Sprite2D();
		AddChild(sprite);
		sprite.Centered = false;
		sprite.Visible = true;
		return sprite;
	}

	private void ReleaseSprite(Sprite2D sprite)
	{
		RemoveChild(sprite);
		sprite.Texture = null;
		_spritePool.Push(sprite);
	}

	private void ClearVisibleChunks()
	{
		foreach (var sprite in _visibleChunks.Values)
			ReleaseSprite(sprite);
		_visibleChunks.Clear();
	}

	public Vector2 GetWorldPixelSize() =>
		_world == null ? Vector2.Zero : new Vector2(_world.MaxTilesX * WorldConfig.TilePixelSize, _world.MaxTilesY * WorldConfig.TilePixelSize);
}
