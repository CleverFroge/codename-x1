namespace CodenameX1.World;

/// <summary>移植 Terraria.GameContent.Biomes.TerrainPass</summary>
public static class TerrainPass
{
	private enum TerrainFeature { Plateau, Hill, Dale, Mountain, Valley }

	private sealed class SurfaceHistory
	{
		private readonly double[] _heights;
		private int _index;

		public SurfaceHistory(int size)
		{
			_heights = new double[size];
		}

		public double this[int index]
		{
			get => _heights[(index + _index) % _heights.Length];
			set => _heights[(index + _index) % _heights.Length] = value;
		}

		public int Length => _heights.Length;

		public void Record(double h)
		{
			_heights[_index] = h;
			_index = (_index + 1) % _heights.Length;
		}
	}

	public static void Apply(WorldState world, GenRandom rng, int flatBeachPadding = 0)
	{
		var gv = world.GenVars;
		var feature = TerrainFeature.Plateau;
		int featureTimer = 0;

		double surfaceY = world.MaxTilesY * 0.3;
		surfaceY *= rng.Next(90, 110) * 0.005;
		double rockY = surfaceY + world.MaxTilesY * 0.2;
		rockY *= rng.Next(90, 110) * 0.01;

		double surfaceLow = surfaceY, surfaceHigh = surfaceY;
		double rockLow = rockY, rockHigh = rockY;
		double beachCap = world.MaxTilesY * 0.23;
		var history = new SurfaceHistory(500);
		featureTimer = gv.LeftBeachEnd + flatBeachPadding;

		for (int x = 0; x < world.MaxTilesX; x++)
		{
			surfaceLow = Math.Min(surfaceY, surfaceLow);
			surfaceHigh = Math.Max(surfaceY, surfaceHigh);
			rockLow = Math.Min(rockY, rockLow);
			rockHigh = Math.Max(rockY, rockHigh);

			if (featureTimer <= 0)
			{
				feature = (TerrainFeature)rng.Next(0, 5);
				featureTimer = rng.Next(5, 40);
				if (feature == TerrainFeature.Plateau)
					featureTimer = (int)(rng.Next(5, 30) * 0.2);
			}
			featureTimer--;

			double xr = (double)x / world.MaxTilesX;
			if (xr > 0.45 && xr < 0.55 && feature is TerrainFeature.Mountain or TerrainFeature.Valley)
				feature = (TerrainFeature)rng.Next(0, 3);
			if (xr > 0.48 && xr < 0.52)
				feature = TerrainFeature.Plateau;

			surfaceY += SurfaceOffset(feature, rng);

			double clampMin = world.MaxTilesX <= 2500 ? 0.19 : 0.17;
			const double clampMax = 0.26;

			if (x < gv.LeftBeachEnd + flatBeachPadding || x > gv.RightBeachStart - flatBeachPadding)
				surfaceY = Math.Clamp(surfaceY, world.MaxTilesY * clampMin, beachCap);
			else if (surfaceY < world.MaxTilesY * clampMin)
			{
				surfaceY = world.MaxTilesY * clampMin;
				featureTimer = 0;
			}
			else if (surfaceY > world.MaxTilesY * clampMax)
			{
				surfaceY = world.MaxTilesY * clampMax;
				featureTimer = 0;
			}

			while (rng.Next(0, 3) == 0)
				rockY += rng.Next(-2, 3);
			if (rockY < surfaceY + world.MaxTilesY * 0.06) rockY += 1;
			if (rockY > surfaceY + world.MaxTilesY * 0.35) rockY -= 1;

			history.Record(surfaceY);
			FillColumn(world, x, surfaceY, rockY);

			if (x == gv.RightBeachStart - flatBeachPadding)
			{
				if (surfaceY > beachCap)
					RetargetHistory(world, history, x, beachCap);
				feature = TerrainFeature.Plateau;
				featureTimer = world.MaxTilesX - x;
			}
		}

		world.WorldSurface = (int)(surfaceHigh + 25);
		world.RockLayer = (int)rockHigh;
		int rockDelta = (int)((world.RockLayer - world.WorldSurface) / 6.0) * 6;
		world.RockLayer = world.WorldSurface + rockDelta;

		gv.WaterLine = (int)((world.RockLayer + world.MaxTilesY) / 2.0) + rng.Next(-100, 20);
		gv.LavaLine = gv.WaterLine + rng.Next(50, 80);

		gv.RockLayer = world.RockLayer;
		gv.RockLayerHigh = rockHigh;
		gv.RockLayerLow = rockLow;
		gv.WorldSurface = world.WorldSurface;
		gv.WorldSurfaceHigh = surfaceHigh;
		gv.WorldSurfaceLow = surfaceLow;
	}

	private static void FillColumn(WorldState world, int x, double surfaceY, double rockY)
	{
		for (int y = 0; y < (int)surfaceY; y++)
			world.Tile(x, y).Active = false;
		for (int y = (int)surfaceY; y < world.MaxTilesY; y++)
		{
			ref var t = ref world.Tile(x, y);
			t.Active = true;
			t.Type = y < (int)rockY ? TileIds.Dirt : TileIds.Stone;
		}
	}

	private static double SurfaceOffset(TerrainFeature f, GenRandom rng)
	{
		double n = 0;
		switch (f)
		{
			case TerrainFeature.Plateau:
				while (rng.Next(0, 7) == 0) n += rng.Next(-1, 2);
				break;
			case TerrainFeature.Hill:
				while (rng.Next(0, 4) == 0) n -= 1;
				while (rng.Next(0, 10) == 0) n += 1;
				break;
			case TerrainFeature.Dale:
				while (rng.Next(0, 4) == 0) n += 1;
				while (rng.Next(0, 10) == 0) n -= 1;
				break;
			case TerrainFeature.Mountain:
				while (rng.Next(0, 2) == 0) n -= 1;
				while (rng.Next(0, 6) == 0) n += 1;
				break;
			case TerrainFeature.Valley:
				while (rng.Next(0, 2) == 0) n += 1;
				while (rng.Next(0, 5) == 0) n -= 1;
				break;
		}
		return n;
	}

	private static void RetargetHistory(WorldState world, SurfaceHistory history, int targetX, double targetHeight)
	{
		for (int i = 0; i < history.Length / 2; i++)
		{
			if (history[history.Length - 1] <= targetHeight) break;
			for (int j = 0; j < history.Length - i * 2; j++)
			{
				int idx = history.Length - j - 1;
				double h = history[idx] - 1;
				history[idx] = h;
				if (h <= targetHeight) break;
			}
		}
		for (int k = 0; k < history.Length; k++)
		{
			int col = targetX - k;
			if (col < 0) continue;
			double h = history[history.Length - k - 1];
			for (int y = 0; y < (int)h; y++) world.Tile(col, y).Active = false;
		}
	}
}
