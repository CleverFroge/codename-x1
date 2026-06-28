namespace CodenameX1.World;

/// <summary>Terraria WorldGen.AddPasses 核心 Pass 移植（标准世界，无 secret seed）。</summary>
public static class WorldGenPasses
{
	public static void Reset(WorldState world, GenRandom rng)
	{
		var gv = world.GenVars;
		gv.ScaleForWorld(world.MaxTilesX);
		gv.Crimson = rng.Next(2) == 0;
		gv.EvilStone = gv.Crimson ? TileIds.Crimstone : TileIds.Ebonstone;
		gv.DungeonSide = rng.Next(2);
		gv.CrimsonLeft = rng.Next(2) == 0;

		if (rng.Next(2) == 0) { gv.Copper = 166; }
		if (rng.Next(2) == 0) { gv.Iron = 167; }
		if (rng.Next(2) == 0) { gv.Silver = 168; }
		if (rng.Next(2) == 0) { gv.Gold = 169; }

		double scale = world.MaxTilesX / 4200.0;
		if (gv.DungeonSide == 0)
			gv.JungleOriginX = (int)(world.MaxTilesX * (1.0 - rng.Next(15, 30) * 0.01));
		else
			gv.JungleOriginX = (int)(world.MaxTilesX * (rng.Next(15, 30) * 0.01));

		gv.LeftBeachEnd = rng.Next(
			gv.BeachSandRandomCenter - gv.BeachSandRandomWidthRange,
			gv.BeachSandRandomCenter + gv.BeachSandRandomWidthRange);
		if (gv.DungeonSide == 1) gv.LeftBeachEnd += gv.BeachSandDungeonExtraWidth;
		else gv.LeftBeachEnd += gv.BeachSandJungleExtraWidth;

		gv.RightBeachStart = world.MaxTilesX - rng.Next(
			gv.BeachSandRandomCenter - gv.BeachSandRandomWidthRange,
			gv.BeachSandRandomCenter + gv.BeachSandRandomWidthRange);
		if (gv.DungeonSide == 0) gv.RightBeachStart -= gv.BeachSandDungeonExtraWidth;
		else gv.RightBeachStart -= gv.BeachSandJungleExtraWidth;

		gv.NumTunnels = 0;
		gv.NumMCaves = 0;
		gv.NumIslandHouses = 0;
	}

	public static void OceanSand(WorldState world, GenRandom rng)
	{
		var gv = world.GenVars;
		for (int pass = 0; pass < 3; pass++)
		{
			if (pass == 1) continue;
			int start = pass == 0 ? 0 : gv.RightBeachStart;
			int end = pass == 0 ? gv.LeftBeachEnd : world.MaxTilesX;
			int depth = rng.Next(50, 100);
			for (int x = start; x < end; x++)
			{
				if (rng.Next(2) == 0)
				{
					depth += rng.Next(-1, 2);
					depth = Math.Clamp(depth, 50, 200);
				}
				for (int y = 0; y < (world.WorldSurface + world.RockLayer) / 2; y++)
				{
					if (!world.Tile(x, y).Active) continue;
					int d = depth;
					if (x - start < d) d = x - start;
					if (end - x < d) d = end - x;
					d += rng.Next(5);
					for (int sy = y; sy < y + d && sy < world.MaxTilesY; sy++)
					{
						if (x > start + rng.Next(5) && x < end - rng.Next(5))
							world.Tile(x, sy).Type = TileIds.Sand;
					}
					break;
				}
			}
		}
	}

	public static void SandPatches(WorldState world, GenRandom rng)
	{
		int count = (int)(world.MaxTilesX * 0.013);
		for (int i = 0; i < count; i++)
		{
			int x = rng.Next(0, world.MaxTilesX);
			int y = rng.Next((int)world.GenVars.WorldSurfaceHigh, world.MaxTilesY);
			WorldGenTools.TileRunner(world, rng, x, y, rng.Next(15, 70), rng.Next(20, 130), TileIds.Sand);
		}
	}

	public static void Tunnels(WorldState world, GenRandom rng)
	{
		var gv = world.GenVars;
		int count = (int)(world.MaxTilesX * 0.0015);
		for (int i = 0; i < count && gv.NumTunnels < gv.MaxTunnels - 1; i++)
		{
			int x = rng.Next(Math.Min(450, world.MaxTilesX / 4), Math.Max(451, world.MaxTilesX - 450));
			while (x > world.MaxTilesX * 0.4 && x < world.MaxTilesX * 0.6)
				x = rng.Next(Math.Min(450, world.MaxTilesX / 4), Math.Max(451, world.MaxTilesX - 450));

			var xs = new int[10];
			var ys = new int[10];
			bool onSand;
			do
			{
				onSand = false;
				x %= world.MaxTilesX;
				int y = 0;
				for (int t = 0; t < 10; t++)
				{
					while (!world.Tile(x, y).Active) y++;
					if (world.Tile(x, y).Type == TileIds.Sand) onSand = true;
					xs[t] = x;
					ys[t] = y - rng.Next(11, 16);
					x += rng.Next(5, 11);
				}
			} while (onSand);

			gv.TunnelX[gv.NumTunnels++] = xs[5];
			for (int t = 0; t < 10; t++)
			{
				WorldGenTools.TileRunner(world, rng, xs[t], ys[t], rng.Next(5, 8), rng.Next(6, 9), TileIds.Dirt, addTile: true, -2, -0.3);
				WorldGenTools.TileRunner(world, rng, xs[t], ys[t], rng.Next(5, 8), rng.Next(6, 9), TileIds.Dirt, addTile: true, 2, -0.3);
			}
		}
	}

	public static void MountainCaves(WorldState world, GenRandom rng)
	{
		var gv = world.GenVars;
		gv.NumMCaves = 0;
		int count = (int)(world.MaxTilesX * 0.001);
		for (int i = 0; i < count; i++)
		{
			int x = rng.Next((int)(world.MaxTilesX * 0.25), (int)(world.MaxTilesX * 0.75));
			while (x > world.MaxTilesX * 0.4 && x < world.MaxTilesX * 0.6)
				x = rng.Next((int)(world.MaxTilesX * 0.25), (int)(world.MaxTilesX * 0.75));

			int y = 0;
			while (!world.Tile(x, y).Active) y++;
			if (y >= world.WorldSurface) continue;

			if (gv.NumMCaves < gv.MaxMCaves)
			{
				gv.MCaveX[gv.NumMCaves] = x;
				gv.MCaveY[gv.NumMCaves] = y;
				gv.NumMCaves++;
			}
			WorldGenTools.Cavinator(world, rng, x, y, rng.Next(40, 50));
		}
	}

	public static void DirtLayerCaves(WorldState world, GenRandom rng)
	{
		var gv = world.GenVars;
		int count = (int)(world.MaxTilesX * world.MaxTilesY * 3E-05);
		double surfaceHigh = gv.WorldSurfaceHigh;
		for (int i = 0; i < count; i++)
		{
			int type = rng.Next(6) == 0 ? TileIds.DigWater : TileIds.Dig;
			int x = rng.Next(0, world.MaxTilesX);
			int y = rng.Next((int)gv.WorldSurfaceLow, (int)gv.RockLayerHigh + 1);
			while (((x < gv.SmallHolesBeachAvoidance || x > world.MaxTilesX - gv.SmallHolesBeachAvoidance) && y < surfaceHigh) ||
			       (x >= world.MaxTilesX * 0.45 && x <= world.MaxTilesX * 0.55 && y < world.WorldSurface))
			{
				x = rng.Next(0, world.MaxTilesX);
				y = rng.Next((int)gv.WorldSurfaceLow, (int)gv.RockLayerHigh + 1);
			}
			WorldGenTools.TileRunner(world, rng, x, y, rng.Next(5, 15), rng.Next(30, 200), type);
		}
	}

	public static void RockLayerCaves(WorldState world, GenRandom rng)
	{
		var gv = world.GenVars;
		int count = (int)(world.MaxTilesX * world.MaxTilesY * 0.00013);
		int yMax = Math.Max((int)gv.RockLayerHigh + 1, gv.LavaLine - 20);
		for (int i = 0; i < count; i++)
		{
			int type = rng.Next(10) == 0 ? TileIds.DigWater : TileIds.Dig;
			WorldGenTools.TileRunner(world, rng,
				rng.Next(0, world.MaxTilesX),
				rng.Next((int)gv.RockLayerHigh, yMax),
				rng.Next(6, 20), rng.Next(50, 300), type);
		}
	}

	public static void SurfaceCaves(WorldState world, GenRandom rng)
	{
		var gv = world.GenVars;
		int c1 = (int)(world.MaxTilesX * 0.002);
		int c2 = (int)(world.MaxTilesX * 0.0007);
		int c3 = (int)(world.MaxTilesX * 0.0003);

		for (int i = 0; i < c1; i++)
		{
			int x = RandomSurfaceX(world, rng, 0.45, 0.55);
			for (int y = 0; y < gv.WorldSurfaceHigh; y++)
			{
				if (!world.Tile(x, y).Active) continue;
				WorldGenTools.TileRunner(world, rng, x, y, rng.Next(3, 6), rng.Next(5, 50), TileIds.Dig,
					speedX: rng.Next(-10, 11) * 0.1, speedY: 1);
				break;
			}
		}
		for (int i = 0; i < c2; i++)
		{
			int x = RandomSurfaceX(world, rng, 0.43, 0.57);
			for (int y = 0; y < gv.WorldSurfaceHigh; y++)
			{
				if (!world.Tile(x, y).Active) continue;
				WorldGenTools.TileRunner(world, rng, x, y, rng.Next(3, 6), rng.Next(5, 50), TileIds.Dig,
					speedX: rng.Next(-10, 11) * 0.1, speedY: 2);
				break;
			}
		}
		for (int i = 0; i < c3; i++)
		{
			int x = RandomSurfaceX(world, rng, 0.4, 0.6);
			for (int y = 0; y < gv.WorldSurfaceHigh; y++)
			{
				if (!world.Tile(x, y).Active) continue;
				WorldGenTools.TileRunner(world, rng, x, y, rng.Next(3, 6), rng.Next(5, 50), TileIds.Dig,
					speedX: rng.Next(-10, 11) * 0.1, speedY: 3);
				break;
			}
		}
	}

	public static void Grass(WorldState world, GenRandom rng)
	{
		int count = (int)(world.MaxTilesX * world.MaxTilesY * 0.002);
		for (int i = 0; i < count; i++)
		{
			ScatterGrass(world, rng, rng.Next(1, world.MaxTilesX - 1),
				rng.Next((int)world.GenVars.WorldSurfaceLow, (int)world.GenVars.WorldSurfaceHigh));
			ScatterGrass(world, rng, rng.Next(1, world.MaxTilesX - 1),
				rng.Next(5, (int)world.GenVars.WorldSurfaceLow));
		}
	}

	private static void ScatterGrass(WorldState world, GenRandom rng, int x, int y)
	{
		if (y >= world.MaxTilesY - 1) y = world.MaxTilesY - 2;
		ref var center = ref world.Tile(x, y);
		if (!IsDirtPocket(world, x, y)) return;
		center.Active = true;
		center.Type = TileIds.Grass;
	}

	private static bool IsDirtPocket(WorldState world, int x, int y)
	{
		bool Active(int tx, int ty) => world.Tile(tx, ty).Active;
		bool IsDirt(int tx, int ty) => world.Tile(tx, ty).Type == TileIds.Dirt;
		return Active(x - 1, y) && IsDirt(x - 1, y)
		       && Active(x + 1, y) && IsDirt(x + 1, y)
		       && Active(x, y - 1) && IsDirt(x, y - 1)
		       && Active(x, y + 1) && IsDirt(x, y + 1);
	}

	public static void Jungle(WorldState world, GenRandom rng)
	{
		int center = world.GenVars.JungleOriginX;
		int width = (int)(world.MaxTilesX * 0.08);
		for (int x = Math.Max(0, center - width); x < Math.Min(world.MaxTilesX, center + width); x++)
		for (int y = (int)world.GenVars.WorldSurfaceLow; y < world.MaxTilesY - 50; y++)
		{
			ref var t = ref world.Tile(x, y);
			if (!t.Active) continue;
			if (t.Type == TileIds.Dirt || t.Type == TileIds.Grass)
			{
				t.Type = TileIds.Mud;
				if (!world.Tile(x, y - 1).Active)
					t.Type = TileIds.JungleGrass;
			}
		}
	}

	public static void FloatingIslands(WorldState world, GenRandom rng)
	{
		var gv = world.GenVars;
		int count = (int)(world.MaxTilesX * 0.0008) + gv.SkyLakes;
		for (int i = 0; i < count; i++)
		{
			bool placed = false;
			for (int attempt = 0; attempt < world.MaxTilesX && !placed; attempt++)
			{
				int x = rng.Next((int)(world.MaxTilesX * 0.1), (int)(world.MaxTilesX * 0.9));
				while (x > world.MaxTilesX / 2 - 150 && x < world.MaxTilesX / 2 + 150)
					x = rng.Next((int)(world.MaxTilesX * 0.1), (int)(world.MaxTilesX * 0.9));

				bool ok = true;
				for (int h = 0; h < gv.NumIslandHouses; h++)
				{
					if (x > gv.FloatingIslandHouseX[h] - 180 && x < gv.FloatingIslandHouseX[h] + 180)
					{
						ok = false;
						break;
					}
				}
				if (!ok) continue;

				int groundY = 0;
				for (int y = 200; y < world.WorldSurface; y++)
				{
					if (world.Tile(x, y).Active) { groundY = y; break; }
				}
				if (groundY == 0) continue;

				int islandY = Math.Min(rng.Next(90, groundY - 100), (int)gv.WorldSurfaceLow - 50);
				PlaceIsland(world, rng, x, islandY);
				if (gv.NumIslandHouses < gv.FloatingIslandHouseX.Length)
				{
					gv.FloatingIslandHouseX[gv.NumIslandHouses] = x;
					gv.FloatingIslandHouseY[gv.NumIslandHouses] = islandY;
					gv.NumIslandHouses++;
				}
				placed = true;
			}
		}
	}

	private static void PlaceIsland(WorldState world, GenRandom rng, int cx, int cy)
	{
		int w = rng.Next(40, 70);
		int h = rng.Next(12, 20);
		for (int x = cx - w; x <= cx + w; x++)
		for (int y = cy - h; y <= cy + h; y++)
		{
			if (!world.InBounds(x, y)) continue;
			double dx = (x - cx) / (double)w;
			double dy = (y - cy) / (double)h;
			if (dx * dx + dy * dy <= 1.0)
			{
				ref var t = ref world.Tile(x, y);
				t.Active = true;
				t.Type = TileIds.Cloud;
			}
		}
		// 岛心泥土
		for (int x = cx - w / 2; x <= cx + w / 2; x++)
		for (int y = cy; y <= cy + 4; y++)
		{
			if (!world.InBounds(x, y)) continue;
			ref var t = ref world.Tile(x, y);
			t.Active = true;
			t.Type = TileIds.Dirt;
		}
	}

	public static void OresAndShinies(WorldState world, GenRandom rng)
	{
		var gv = world.GenVars;
		int area = world.MaxTilesX * world.MaxTilesY;

		void Ore(int n, int yMin, int yMax, int sMin, int sMax, int stMin, int stMax, int ore)
		{
			yMax = Math.Min(yMax, gv.LavaLine - 20);
			if (yMax <= yMin) return;
			for (int i = 0; i < n; i++)
				WorldGenTools.TileRunner(world, rng, rng.Next(0, world.MaxTilesX), rng.Next(yMin, yMax),
					rng.Next(sMin, sMax), rng.Next(stMin, stMax), ore);
		}

		Ore((int)(area * 6E-05), (int)gv.WorldSurfaceLow, (int)gv.WorldSurfaceHigh, 3, 6, 2, 6, gv.Copper);
		Ore((int)(area * 8E-05), (int)gv.WorldSurfaceHigh, (int)gv.RockLayerHigh, 3, 7, 3, 7, gv.Copper);
		Ore((int)(area * 0.0002), (int)gv.RockLayerLow, world.MaxTilesY, 4, 9, 4, 8, gv.Copper);

		Ore((int)(area * 3E-05), (int)gv.WorldSurfaceLow, (int)gv.WorldSurfaceHigh, 3, 7, 2, 5, gv.Iron);
		Ore((int)(area * 8E-05), (int)gv.WorldSurfaceHigh, (int)gv.RockLayerHigh, 3, 6, 3, 6, gv.Iron);
		Ore((int)(area * 0.0002), (int)gv.RockLayerLow, world.MaxTilesY, 4, 9, 4, 8, gv.Iron);

		Ore((int)(area * 2.6E-05), (int)gv.WorldSurfaceHigh, (int)gv.RockLayerHigh, 3, 6, 3, 6, gv.Silver);
		Ore((int)(area * 0.00015), (int)gv.RockLayerLow, world.MaxTilesY, 4, 9, 4, 8, gv.Silver);

		Ore((int)(area * 0.00012), (int)gv.RockLayerLow, world.MaxTilesY, 4, 8, 4, 8, gv.Gold);

		int evilOre = gv.Crimson ? TileIds.Crimtane : TileIds.Demonite;
		Ore((int)(area * 4.25E-05), world.WorldSurface, world.RockLayer, 3, 6, 4, 8, evilOre);
	}

	public static void Underworld(WorldState world, GenRandom rng)
	{
		var gv = world.GenVars;
		int hellTop = world.MaxTilesY - rng.Next(150, 190);
		int lavaTop = world.MaxTilesY - rng.Next(40, 70);

		for (int x = 0; x < world.MaxTilesX; x++)
		{
			hellTop += rng.Next(-3, 4);
			hellTop = Math.Clamp(hellTop, world.MaxTilesY - 190, world.MaxTilesY - 160);
			for (int y = hellTop - 20 - rng.Next(3); y < world.MaxTilesY; y++)
			{
				ref var t = ref world.Tile(x, y);
				if (y >= hellTop)
				{
					t.Active = false;
					t.Liquid = 0;
					t.Lava = false;
				}
				else
					t.Type = TileIds.Ash;
			}
		}

		for (int x = 10; x < world.MaxTilesX - 10; x++)
		{
			lavaTop += rng.Next(-10, 11);
			lavaTop = Math.Clamp(lavaTop, world.MaxTilesY - 120, world.MaxTilesY - 60);
			for (int y = lavaTop; y < world.MaxTilesY - 10; y++)
			{
				ref var t = ref world.Tile(x, y);
				if (!t.Active)
				{
					t.Lava = true;
					t.Liquid = 255;
				}
			}
		}

		for (int i = 0; i < world.MaxTilesX; i++)
		{
			if (rng.Next(50) != 0) continue;
			int y = world.MaxTilesY - 65;
			while (!world.Tile(i, y).Active && y > world.MaxTilesY - 135) y--;
			int startY = y + rng.Next(20, 50);
			if (startY < hellTop - 30) continue;
			WorldGenTools.TileRunner(world, rng, rng.Next(0, world.MaxTilesX), startY,
				rng.Next(8, 14), rng.Next(40, 80), TileIds.Ash, addTile: true,
				speedY: rng.Next(1, 3), noYChange: true);
		}
	}

	public static void CorruptionAndCrimson(WorldState world, GenRandom rng)
	{
		var gv = world.GenVars;
		int patches = Math.Max(1, (int)(world.MaxTilesX * 0.00045));
		int avoid = gv.EvilBiomeBeachAvoidance;
		int jungleMargin = (int)(world.MaxTilesX * 0.12);

		for (int p = 0; p < patches; p++)
		{
			int cx = rng.Next(avoid + 200, world.MaxTilesX - avoid - 200);
			if (Math.Abs(cx - gv.JungleOriginX) < jungleMargin)
				cx = cx < gv.JungleOriginX ? gv.JungleOriginX - jungleMargin : gv.JungleOriginX + jungleMargin;

			int left = cx - rng.Next(200) - 100;
			int right = cx + rng.Next(200) + 100;
			left = Math.Max(avoid, left);
			right = Math.Min(world.MaxTilesX - avoid, right);

			for (int x = left; x < right; x++)
			for (int y = (int)gv.WorldSurfaceLow; y < world.WorldSurface + 10; y++)
			{
				ref var t = ref world.Tile(x, y);
				if (!t.Active) continue;
				if (t.Type is TileIds.Dirt or TileIds.Stone or TileIds.Grass)
					t.Type = gv.EvilStone;
			}

			int surfaceY = WorldGenTools.SurfaceY(world, cx);
			WorldGenTools.ChasmRunner(world, rng, cx, surfaceY, rng.Next(30, 50), gv.Crimson);
		}
	}

	public static void Lakes(WorldState world, GenRandom rng)
	{
		double scale = world.MaxTilesX / 4200.0;
		int count = rng.Next((int)(scale * 3), (int)(scale * 6) + 1);
		var gv = world.GenVars;
		int lakeAvoid = gv.BeachSandRandomCenter + 20;

		for (int i = 0; i < count; i++)
		{
			int x = rng.Next(lakeAvoid, world.MaxTilesX - lakeAvoid);
			while (x > world.MaxTilesX * 0.45 && x < world.MaxTilesX * 0.55)
				x = rng.Next(lakeAvoid, world.MaxTilesX - lakeAvoid);

			int y = rng.Next((int)gv.WorldSurfaceHigh, (int)gv.RockLayerLow);
			WorldGenTools.TileRunner(world, rng, x, y, rng.Next(12, 25), rng.Next(80, 130), TileIds.DigWater);
		}
	}

	private static int RandomSurfaceX(WorldState world, GenRandom rng, double innerMin, double innerMax)
	{
		var gv = world.GenVars;
		int x = rng.Next(0, world.MaxTilesX);
		while ((x > world.MaxTilesX * innerMin && x < world.MaxTilesX * innerMax) ||
		       x < gv.LeftBeachEnd + 20 || x > gv.RightBeachStart - 20)
			x = rng.Next(0, world.MaxTilesX);
		return x;
	}
}
