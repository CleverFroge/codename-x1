using Godot;
using CodenameX1.World;

namespace CodenameX1;

public partial class WorldView : Node2D
{
	private static readonly Dictionary<int, Color> TileColors = new()
	{
		[TileIds.Dirt] = new Color(0.55f, 0.36f, 0.18f),
		[TileIds.Stone] = new Color(0.45f, 0.45f, 0.48f),
		[TileIds.Grass] = new Color(0.25f, 0.55f, 0.2f),
		[TileIds.Sand] = new Color(0.85f, 0.78f, 0.45f),
		[TileIds.Mud] = new Color(0.35f, 0.22f, 0.12f),
		[TileIds.JungleGrass] = new Color(0.15f, 0.5f, 0.12f),
		[TileIds.Ash] = new Color(0.35f, 0.15f, 0.1f),
		[TileIds.Cloud] = new Color(0.92f, 0.95f, 1f),
		[TileIds.Copper] = new Color(0.7f, 0.45f, 0.3f),
		[TileIds.Iron] = new Color(0.55f, 0.5f, 0.48f),
		[TileIds.Silver] = new Color(0.75f, 0.78f, 0.82f),
		[TileIds.Gold] = new Color(0.85f, 0.72f, 0.2f),
		[TileIds.Ebonstone] = new Color(0.35f, 0.2f, 0.45f),
		[TileIds.Crimstone] = new Color(0.55f, 0.15f, 0.2f),
		[TileIds.Demonite] = new Color(0.3f, 0.15f, 0.4f),
		[TileIds.Crimtane] = new Color(0.6f, 0.1f, 0.15f),
	};

	private Sprite2D? _sprite;
	private WorldState? _world;

	public void Render(WorldState world)
	{
		_world = world;
		int w = world.MaxTilesX;
		int h = world.MaxTilesY;
		var data = new byte[w * h * 4];
		var air = new Color(0.05f, 0.08f, 0.15f);
		var water = new Color(0.2f, 0.4f, 0.85f, 0.9f);
		var lava = new Color(0.9f, 0.25f, 0.05f, 0.95f);

		for (int y = 0; y < h; y++)
		for (int x = 0; x < w; x++)
		{
			ref var t = ref world.Tile(x, y);
			Color c;
			if (t.Liquid > 0)
				c = t.Lava ? lava : water;
			else if (!t.Active)
				c = air;
			else
				c = TileColors.GetValueOrDefault(t.Type, Colors.Magenta);

			int i = (y * w + x) * 4;
			data[i] = (byte)(c.R * 255);
			data[i + 1] = (byte)(c.G * 255);
			data[i + 2] = (byte)(c.B * 255);
			data[i + 3] = (byte)(c.A * 255);
		}

		var img = Image.CreateFromData(w, h, false, Image.Format.Rgba8, data);
		var tex = ImageTexture.CreateFromImage(img);
		_sprite ??= GetNode<Sprite2D>("WorldSprite");
		_sprite.Texture = tex;
		_sprite.Scale = new Vector2(WorldConfig.TilePixelSize, WorldConfig.TilePixelSize);
		_sprite.Position = Vector2.Zero;
	}

	public Vector2 GetWorldPixelSize() =>
		_world == null ? Vector2.Zero : new Vector2(_world.MaxTilesX * WorldConfig.TilePixelSize, _world.MaxTilesY * WorldConfig.TilePixelSize);
}
