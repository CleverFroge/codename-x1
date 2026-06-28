using System.Numerics;

namespace CodenameX1.World;

/// <summary>移植 Terraria WorldGen.TileRunner / ChasmRunner / Cavinator。</summary>
public static class WorldGenTools
{
	private static bool CanClearDuringGeneration(int type) =>
		type is TileIds.Dirt or TileIds.Stone or TileIds.Grass or TileIds.Mud or TileIds.JungleGrass
			or TileIds.Copper or TileIds.Iron or TileIds.Silver or TileIds.Gold
			or TileIds.Demonite or TileIds.Crimtane;

	public static void TileRunner(
		WorldState world,
		GenRandom rng,
		double i,
		double j,
		double strength,
		int steps,
		int type,
		bool addTile = false,
		double speedX = 0,
		double speedY = 0,
		bool noYChange = false)
	{
		double num = strength;
		double stepsLeft = steps;
		var pos = new Vector2((float)i, (float)j);
		var vel = speedX != 0 || speedY != 0
			? new Vector2((float)speedX, (float)speedY)
			: new Vector2(rng.Next(-10, 11) * 0.1f, rng.Next(-10, 11) * 0.1f);

		int beachDistance = world.GenVars.BeachBordersWidth;
		var gv = world.GenVars;

		while (num > 0 && stepsLeft > 0)
		{
			num = strength * (stepsLeft / steps);
			stepsLeft -= 1;

			int x0 = Math.Max(1, (int)(pos.X - num * 0.5));
			int x1 = Math.Min(world.MaxTilesX - 1, (int)(pos.X + num * 0.5));
			int y0 = Math.Max(1, (int)(pos.Y - num * 0.5));
			int y1 = Math.Min(world.MaxTilesY - 1, (int)(pos.Y + num * 0.5));

			for (int x = x0; x < x1; x++)
			{
				double localStrength = strength;
				if (x < beachDistance + 50 || x >= world.MaxTilesX - beachDistance - 50)
					localStrength = strength * 0.5;

				for (int y = y0; y < y1; y++)
				{
					double jitter = 1.0 + rng.Next(-10, 11) * 0.015;
					if (Math.Abs(x - pos.X) + Math.Abs(y - pos.Y) >= localStrength * 0.5 * jitter)
						continue;

					ref var tile = ref world.Tile(x, y);

					if (type < 0)
					{
						if (tile.Active && tile.Type == TileIds.Sand)
							continue;
						if (!tile.Active || !CanClearDuringGeneration(tile.Type))
							continue;
						if (type == TileIds.DigWater && (y < gv.WaterLine || y > gv.LavaLine))
						{
							tile.Liquid = 255;
							tile.Lava = y > gv.LavaLine;
						}
						tile.Active = false;
						continue;
					}

					if (tile.Active && tile.Type == TileIds.Sand && type != TileIds.Sand)
						continue;

					if (addTile)
					{
						if (y < world.WorldSurface && type != TileIds.Sand && type != TileIds.Mud)
							continue;
						tile.Active = true;
						tile.Type = type;
						tile.Liquid = 0;
						tile.Lava = false;
						continue;
					}

					if (!tile.Active)
						continue;

					if (type is TileIds.Copper or TileIds.Iron or TileIds.Silver or TileIds.Gold
					    or TileIds.Demonite or TileIds.Crimtane)
					{
						if (tile.Type != TileIds.Stone)
							continue;
						tile.Type = type;
						continue;
					}

					if (tile.Type == TileIds.Stone || tile.Type == TileIds.Dirt)
						tile.Type = type;
				}
			}

			pos += vel;
			ApplyStrengthAcceleration(ref pos, ref vel, ref stepsLeft, num, rng);

			vel.X += rng.Next(-10, 11) * 0.05f;
			if (!noYChange)
				vel.Y += rng.Next(-10, 11) * 0.05f;

			vel.X = Math.Clamp(vel.X, -1f, 1f);
			if (!noYChange)
				vel.Y = Math.Clamp(vel.Y, -1f, 1f);
			else if (type != TileIds.Mud && num < 3)
				vel.Y = Math.Clamp(vel.Y, -1f, 1f);
		}
	}

	private static void ApplyStrengthAcceleration(ref Vector2 pos, ref Vector2 vel, ref double stepsLeft, double num, GenRandom rng)
	{
		if (num <= 50) return;
		pos += vel;
		stepsLeft -= 1;
		vel.X += rng.Next(-10, 11) * 0.05f;
		vel.Y += rng.Next(-10, 11) * 0.05f;
		if (num <= 100) return;
		pos += vel;
		stepsLeft -= 1;
		vel.X += rng.Next(-10, 11) * 0.05f;
		vel.Y += rng.Next(-10, 11) * 0.05f;
		if (num <= 150) return;
		pos += vel;
		stepsLeft -= 1;
		vel.X += rng.Next(-10, 11) * 0.05f;
		vel.Y += rng.Next(-10, 11) * 0.05f;
	}

	public static void ChasmRunner(WorldState world, GenRandom rng, int startX, int startY, int steps, bool crimson)
	{
		double surface = world.WorldSurface;
		bool branched = false;
		double width = rng.Next(5) + 7;
		double stepsLeft = steps;
		var pos = new Vector2(startX, startY);
		var vel = new Vector2(rng.Next(-10, 11) * 0.1f, (float)(rng.Next(11) * 0.2 + 0.5));
		int evilType = crimson ? TileIds.Crimstone : TileIds.Ebonstone;

		while (width > 0)
		{
			if (stepsLeft > 0)
			{
				width += rng.Next(3);
				width -= rng.Next(3);
				width = Math.Clamp(width, 7, 20);
			}
			else if (pos.Y > surface + 45)
				width -= rng.Next(4);

			int x0 = Math.Max(0, (int)(pos.X - width * 0.5));
			int x1 = Math.Min(world.MaxTilesX - 1, (int)(pos.X + width * 0.5));
			int y0 = Math.Max(0, (int)(pos.Y - width * 0.5));
			int y1 = Math.Min(world.MaxTilesY - 1, (int)(pos.Y + width * 0.5));

			for (int x = x0; x <= x1; x++)
			for (int y = y0; y <= y1; y++)
			{
				if (Math.Abs(x - pos.X) + Math.Abs(y - pos.Y) >= width * 0.5)
					continue;
				ref var tile = ref world.Tile(x, y);
				if (tile.Active && tile.Type is TileIds.Stone or TileIds.Dirt or TileIds.Grass)
					tile.Type = evilType;
				else if (tile.Active)
					tile.Active = false;
				else
					tile.Active = false;
			}

			if (!branched && pos.Y > startY + 20)
			{
				ChasmRunnerSideways(world, rng, (int)pos.X, (int)pos.Y, -1, rng.Next(20, 40), evilType);
				ChasmRunnerSideways(world, rng, (int)pos.X, (int)pos.Y, 1, rng.Next(20, 40), evilType);
				branched = true;
			}

			if (pos.Y > world.RockLayer)
				break;

			pos += vel;
			vel.X += rng.Next(-10, 11) * 0.05f;
			stepsLeft -= 1;
		}
	}

	private static void ChasmRunnerSideways(WorldState world, GenRandom rng, int x, int y, int dir, int steps, int evilType)
	{
		for (int s = 0; s < steps; s++)
		{
			int cx = x + dir * s;
			if (!world.InBounds(cx, y)) break;
			for (int dy = -3; dy <= 3; dy++)
			{
				int cy = y + dy;
				if (!world.InBounds(cx, cy)) continue;
				ref var tile = ref world.Tile(cx, cy);
				if (tile.Active && tile.Type is TileIds.Stone or TileIds.Dirt or TileIds.Grass)
					tile.Type = evilType;
				else
					tile.Active = false;
			}
		}
	}

	public static void Cavinator(WorldState world, GenRandom rng, int i, int j, int steps)
	{
		double radius = rng.Next(7, 15);
		int dir = rng.Next(2) == 0 ? -1 : 1;
		var pos = new Vector2(i, j);
		var vel = new Vector2(dir, (float)(rng.Next(10, 20) * 0.01));
		int left = steps;

		while (left-- > 0)
		{
			double r = radius * rng.Next(80, 120) * 0.01;
			int x0 = Math.Max(0, (int)(pos.X - r * 0.5));
			int x1 = Math.Min(world.MaxTilesX, (int)(pos.X + r * 0.5));
			int y0 = Math.Max(0, (int)(pos.Y - r * 0.5));
			int y1 = Math.Min(world.MaxTilesY, (int)(pos.Y + r * 0.5));

			for (int x = x0; x < x1; x++)
			for (int y = y0; y < y1; y++)
			{
				if (Math.Abs(x - pos.X) + Math.Abs(y - pos.Y) < r * 0.5)
				{
					ref var t = ref world.Tile(x, y);
					if (t.Active && CanClearDuringGeneration(t.Type))
						t.Active = false;
				}
			}

			pos += vel;
			vel.Y += rng.Next(-10, 11) * 0.01f;
		}
	}

	public static int SurfaceY(WorldState world, int x)
	{
		for (int y = 0; y < world.MaxTilesY; y++)
			if (world.Tile(x, y).Active)
				return y;
		return world.MaxTilesY - 1;
	}
}
