using Godot;
using CodenameX1.World;

namespace CodenameX1;

public partial class PlayerController : Node2D
{
	[Export] public float MoveSpeed = 400f;

	private Vector2 _worldPixelSize;

	public void Spawn(Vector2 position, Vector2 worldPixelSize)
	{
		_worldPixelSize = worldPixelSize;
		Position = position.Clamp(Vector2.Zero, GetMaxWorldPosition());
		QueueRedraw();
	}

	public void Tick(float delta)
	{
		var move = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		if (move == Vector2.Zero)
			return;

		Position = (Position + move * MoveSpeed * delta).Clamp(Vector2.Zero, GetMaxWorldPosition());
	}

	public Vector2I GetTilePosition() =>
		new(
			Mathf.FloorToInt(Position.X / WorldConfig.TilePixelSize),
			Mathf.FloorToInt(Position.Y / WorldConfig.TilePixelSize));

	public ChunkCoord GetChunkPosition()
	{
		var tile = GetTilePosition();
		return WorldCoordinates.TileToChunk(tile.X, tile.Y);
	}

	public override void _Draw()
	{
		DrawCircle(Vector2.Zero, 6f, Colors.Gold);
		DrawArc(Vector2.Zero, 6f, 0f, Mathf.Tau, 16, Colors.White, 1f);
	}

	private Vector2 GetMaxWorldPosition() =>
		new(
			Mathf.Max(0f, _worldPixelSize.X - WorldConfig.TilePixelSize),
			Mathf.Max(0f, _worldPixelSize.Y - WorldConfig.TilePixelSize));
}
