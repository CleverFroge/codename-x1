using Godot;
using CodenameX1.World;

namespace CodenameX1;

public partial class PlayerController : Node2D
{
	[Export] public float MoveSpeed = 120f;
	[Export] public float Gravity = 800f;
	[Export] public float JumpSpeed = 260f;

	private static readonly Vector2 HitboxSize = new(10f, 14f);
	private Vector2 _worldPixelSize;
	private Vector2 _velocity;

	public void Spawn(Vector2 position, Vector2 worldPixelSize)
	{
		_worldPixelSize = worldPixelSize;
		Position = position.Clamp(Vector2.Zero, GetMaxWorldPosition());
		_velocity = Vector2.Zero;
		QueueRedraw();
	}

	public void Tick(float delta, WorldState world)
	{
		float move = Input.GetAxis("ui_left", "ui_right");
		_velocity.X = Mathf.MoveToward(_velocity.X, move * MoveSpeed, MoveSpeed * 8f * delta);
		_velocity.Y = Mathf.Min(_velocity.Y + Gravity * delta, Gravity);
		if (IsGrounded(world) && Input.IsActionJustPressed("ui_accept"))
			_velocity.Y = -JumpSpeed;

		MoveWithCollision(world, _velocity * delta);
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

	private void MoveWithCollision(WorldState world, Vector2 movement)
	{
		int steps = Mathf.Max(1, Mathf.CeilToInt(movement.Length() / WorldConfig.TilePixelSize));
		var step = movement / steps;
		for (int i = 0; i < steps; i++)
		{
			if (step.X != 0f)
			{
				Position = new Vector2(Position.X + step.X, Position.Y);
				if (Collides(world))
				{
					Position = new Vector2(Position.X - step.X, Position.Y);
					_velocity.X = 0f;
				}
			}

			if (step.Y != 0f)
			{
				Position = new Vector2(Position.X, Position.Y + step.Y);
				if (Collides(world))
				{
					Position = new Vector2(Position.X, Position.Y - step.Y);
					_velocity.Y = 0f;
				}
			}
		}

		Position = Position.Clamp(GetMinWorldPosition(), GetMaxWorldPosition());
	}

	private bool IsGrounded(WorldState world)
	{
		Position += Vector2.Down;
		bool grounded = Collides(world);
		Position -= Vector2.Down;
		return grounded;
	}

	private bool Collides(WorldState world)
	{
		var halfSize = HitboxSize * 0.5f;
		int minX = Mathf.FloorToInt((Position.X - halfSize.X) / WorldConfig.TilePixelSize);
		int maxX = Mathf.FloorToInt((Position.X + halfSize.X - 0.01f) / WorldConfig.TilePixelSize);
		int minY = Mathf.FloorToInt((Position.Y - halfSize.Y) / WorldConfig.TilePixelSize);
		int maxY = Mathf.FloorToInt((Position.Y + halfSize.Y - 0.01f) / WorldConfig.TilePixelSize);
		for (int y = minY; y <= maxY; y++)
		for (int x = minX; x <= maxX; x++)
		{
			if (!world.TryGetTile(x, y, out var tile) || tile.Active)
				return true;
		}

		return false;
	}

	private Vector2 GetMinWorldPosition() => HitboxSize * 0.5f;

	private Vector2 GetMaxWorldPosition() =>
		new(
			Mathf.Max(GetMinWorldPosition().X, _worldPixelSize.X - HitboxSize.X * 0.5f),
			Mathf.Max(GetMinWorldPosition().Y, _worldPixelSize.Y - HitboxSize.Y * 0.5f));
}
