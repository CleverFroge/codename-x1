namespace CodenameX1.World;

/// <summary>
/// 世界生成入口。
/// 完整还原：运行 decompile_and_port.ps1 后启用 TerrariaVanilla 工程，调用 Terraria.WorldGen.GenerateWorld。
/// 当前仍使用过渡实现 WorldGenPasses（非原版，待替换）。
/// </summary>
public sealed class WorldGenerator
{
	public int Seed { get; private set; }
	public WorldState? World { get; private set; }

	public event Action<string, float>? ProgressChanged;

	/// <summary>设为 true 时调用 Terraria.WorldGen.GenerateWorld（需 TerrariaVanilla 引用）。</summary>
	public const bool UseTerrariaVanillaPort = true;

	public WorldState Generate(WorldSize size = WorldSize.HalfSmall, int seed = -1)
	{
		Seed = seed >= 0 ? seed : Random.Shared.Next();

		if (UseTerrariaVanillaPort)
		{
			var (tw, th) = GetTerrariaSize(size);
			Report("Terraria WorldGen", 0);
			var world = TerrariaWorldExporter.Generate(tw, th, Seed, Report);
			World = world;
			Report("Done", 1f);
			return world;
		}

		var rng = new GenRandom(Seed);
		var (w, h) = WorldConfig.GetSize(size);
		World = new WorldState(w, h);

		// TODO: 替换为 Terraria.WorldGen.GenerateWorld — 97 Pass 原版流水线
		Report("Reset", 0);
		WorldGenPasses.Reset(World, rng);
		TerrainPass.Apply(World, rng);
		WorldGenPasses.OceanSand(World, rng);
		WorldGenPasses.SandPatches(World, rng);
		WorldGenPasses.Tunnels(World, rng);
		WorldGenPasses.MountainCaves(World, rng);
		WorldGenPasses.DirtLayerCaves(World, rng);
		WorldGenPasses.RockLayerCaves(World, rng);
		WorldGenPasses.SurfaceCaves(World, rng);
		WorldGenPasses.Grass(World, rng);
		WorldGenPasses.Jungle(World, rng);
		WorldGenPasses.FloatingIslands(World, rng);
		WorldGenPasses.OresAndShinies(World, rng);
		WorldGenPasses.Underworld(World, rng);
		WorldGenPasses.CorruptionAndCrimson(World, rng);
		WorldGenPasses.Lakes(World, rng);

		Report("Done", 1f);
		return World;
	}

	private void Report(string msg, float ratio) => ProgressChanged?.Invoke(msg, ratio);

	private static (int W, int H) GetTerrariaSize(WorldSize size) => size switch
	{
		WorldSize.Small => (4200, 1200),
		WorldSize.HalfSmall => (4200, 1200),
		WorldSize.Dev => (840, 240),
		_ => (4200, 1200),
	};
}
