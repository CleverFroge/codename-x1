using Terraria;
using Terraria.Port;
using Terraria.WorldBuilding;

namespace CodenameX1.World;

/// <summary>Runs Terraria.WorldGen.GenerateWorld and copies Main.tile into CodenameX1 WorldState.</summary>
public static class TerrariaWorldExporter
{
	public static WorldState Generate(int width, int height, int seed, Action<string, float>? onProgress = null)
	{
		RuntimeDependencyLoader.EnsureRegistered();

		var progress = new Terraria.WorldBuilding.GenerationProgress();
		if (onProgress != null)
			progress.TotalWeight = 1.0;

		TerrariaPassCatalog.SetupWorld(width, height, seed);
		var passes = TerrariaPassCatalog.LoadPasses(progress);
		var pipeline = new WorldGenPipeline(passes, seed, Terraria.WorldBuilding.GenVars.configuration, progress);
		pipeline.RunAllBlocking();

		int w = Terraria.Main.maxTilesX;
		int h = Terraria.Main.maxTilesY;
		var world = new WorldState(w, h);
		world.WorldSurface = (int)Terraria.Main.worldSurface;
		world.RockLayer = (int)Terraria.Main.rockLayer;

		for (int x = 0; x < w; x++)
		for (int y = 0; y < h; y++)
		{
			Terraria.Tile src = Terraria.Main.tile[x, y];
			ref WorldTile dst = ref world.Tile(x, y);
			if (src == null)
			{
				dst.Clear();
				continue;
			}

			dst.Active = src.active();
			dst.Type = src.type;
			dst.Liquid = src.liquid;
			dst.Lava = src.lava();
			dst.Honey = src.honey();
		}

		return world;
	}
}
