using Terraria;
using Terraria.IO;
using Terraria.Port;
using Terraria.WorldBuilding;

namespace CodenameX1.World;

/// <summary>
/// 从泰拉 WorldGen.AddPasses 加载 GenPass 列表；仅引用 Pass 实现，不调用泰拉管线调度。
/// </summary>
public static class TerrariaPassCatalog
{
	public static void SetupWorld(int width, int height, int seed, string worldName = "CodenameX1")
	{
		WorldGenHost.EnsureInitialized();

		Terraria.Main.maxTilesX = width;
		Terraria.Main.maxTilesY = height;
		Terraria.Main.worldName = worldName;
		Terraria.Main.GameMode = 0;
		Terraria.Main.ActiveWorldFileData = CreateWorldMetadata(seed, worldName);
	}

	public static IReadOnlyList<GenPass> LoadPasses(GenerationProgress? progress = null)
	{
		return WorldGen.PreparePasses(progress, null);
	}

	public static WorldGenPipeline CreatePipeline(int width, int height, int seed, GenerationProgress? progress = null)
	{
		SetupWorld(width, height, seed, "PassEditor");
		var passes = LoadPasses(progress);
		return new WorldGenPipeline(passes, seed, GenVars.configuration, progress);
	}

	private static WorldFileData CreateWorldMetadata(int seed, string worldName)
	{
		var data = new WorldFileData(Terraria.Main.GetWorldPathFromName(worldName, cloudSave: false), cloudSave: false);
		data.Name = worldName;
		data.GameMode = Terraria.Main.GameMode;
		data.CreationTime = DateTime.Now;
		data.Metadata = FileMetadata.FromCurrentSettings(FileType.World);
		data.SetFavorite(favorite: false, saveChanges: false);
		data.WorldGeneratorVersion = 1370094567425uL;
		data.UniqueId = Guid.NewGuid();
		data.SetSeed(seed.ToString());
		return data;
	}
}
