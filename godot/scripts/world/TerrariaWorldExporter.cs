using Terraria;
using Terraria.Port;

namespace CodenameX1.World;

/// <summary>Runs Terraria.WorldGen.GenerateWorld and copies Main.tile into CodenameX1 WorldState.</summary>
public static class TerrariaWorldExporter
{
	public static WorldState Generate(int width, int height, int seed, Action<string, float>? onProgress = null)
	{
		RuntimeDependencyLoader.EnsureRegistered();

		if (!WorldGenHost.Generate(width, height, seed, onProgress))
		{
			throw new InvalidOperationException("Terraria WorldGen.GenerateWorld returned false.");
		}

		var world = new WorldState(width, height);
		world.WorldSurface = (int)Terraria.Main.worldSurface;
		world.RockLayer = (int)Terraria.Main.rockLayer;

		for (int x = 0; x < width; x++)
		for (int y = 0; y < height; y++)
		{
			Tile src = Terraria.Main.tile[x, y];
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
