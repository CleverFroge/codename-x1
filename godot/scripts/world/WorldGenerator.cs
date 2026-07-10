namespace CodenameX1.World;

/// <summary>
/// 世界生成入口。调用 TerrariaVanilla DLL 的 WorldGen.GenerateWorld。
/// </summary>
public sealed class WorldGenerator
{
	public int Seed { get; private set; }
	public WorldState? World { get; private set; }

	public event Action<string, float>? ProgressChanged;

	public WorldState Generate(WorldSize size = WorldSize.HalfSmall, int seed = -1)
	{
		Seed = seed >= 0 ? seed : Random.Shared.Next();

		var (tw, th) = GetTerrariaSize(size);
		Report("Terraria WorldGen", 0);
		World = TerrariaWorldExporter.Generate(tw, th, Seed, Report);
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
