using CodenameX1.World;
using Terraria;
using Terraria.WorldBuilding;

namespace CodenameX1;

/// <summary>
/// PassEditor 宿主：使用项目自研 <see cref="WorldGenPipeline"/>，Pass 来自泰拉或（未来）原生目录。
/// </summary>
public static class WorldGenHostExt
{
	private static readonly Dictionary<string, string> CategoryMap = new()
	{
		["Terrain"] = "地形塑造", ["Dunes"] = "地形塑造", ["Ocean Sand"] = "地形塑造",
		["Sand Patches"] = "地形塑造", ["Floating Islands"] = "地形塑造", ["Underworld"] = "地形塑造",
		["Beaches"] = "地形塑造",
		["Tunnels"] = "洞穴生成", ["Mount Caves"] = "洞穴生成", ["Small Holes"] = "洞穴生成",
		["Dirt Layer Caves"] = "洞穴生成", ["Rock Layer Caves"] = "洞穴生成", ["Surface Caves"] = "洞穴生成",
		["Wavy Caves"] = "洞穴生成", ["Mountain Caves"] = "洞穴生成", ["Create Ocean Caves"] = "洞穴生成",
		["Spider Caves"] = "洞穴生成", ["Gem Caves"] = "洞穴生成", ["Moss"] = "洞穴生成",
		["Dirt Wall Backgrounds"] = "墙体填充", ["Rocks In Dirt"] = "墙体填充", ["Dirt In Rocks"] = "墙体填充",
		["Clay"] = "墙体填充", ["Dirt To Mud"] = "墙体填充", ["Silt"] = "墙体填充", ["Slush"] = "墙体填充",
		["Wall Variety"] = "墙体填充", ["Dirt Rock Wall Runner"] = "墙体填充", ["Wood Tree Walls"] = "墙体填充",
		["Cave Walls"] = "墙体填充", ["Grass Wall"] = "墙体填充", ["Muds Walls In Jungle"] = "墙体填充",
		["Grass"] = "地表特征", ["Spreading Grass"] = "地表特征",
		["Surface Ore and Stone"] = "地表特征", ["Place Fallen Log"] = "地表特征",
		["Jungle"] = "生物群落", ["Mud Caves To Grass"] = "群落修整", ["Full Desert"] = "生物群落",
		["Mushroom Patches"] = "生物群落", ["Marble"] = "生物群落", ["Granite"] = "生物群落",
		["Corruption"] = "生物群落", ["Micro Biomes"] = "生物群落",
		["Shinies"] = "矿石资源", ["Gems"] = "矿石资源", ["Altars"] = "矿石资源",
		["Lakes"] = "液体", ["Shimmer"] = "液体", ["Wet Jungle"] = "液体",
		["Settle Liquids"] = "液体", ["Settle Liquids Again"] = "液体",
		["Dual Dungeons Dither Snake"] = "地牢", ["Dungeon"] = "地牢",
		["Gravitating Sand"] = "清理", ["Clean Up Dirt"] = "清理", ["Remove Water From Sand"] = "清理",
		["Smooth World"] = "清理", ["Quick Cleanup"] = "清理", ["Tile Cleanup"] = "清理",
		["Remove Broken Traps"] = "清理", ["Final Cleanup"] = "清理",
		["Pyramids"] = "结构", ["Living Trees"] = "结构", ["Jungle Temple"] = "结构",
		["Hives"] = "结构", ["Oasis"] = "结构", ["Floating Island Houses"] = "结构",
		["Traps"] = "结构", ["Lihzahrd Altars"] = "结构", ["Temple"] = "结构",
		["Jungle Chests"] = "战利品", ["Life Crystals"] = "战利品", ["Statues"] = "战利品",
		["Buried Chests"] = "战利品", ["Surface Chests"] = "战利品",
		["Jungle Chests Placement"] = "战利品", ["Water Chests"] = "战利品", ["Hellforge"] = "战利品",
		["Webs"] = "装饰", ["Shell Piles"] = "装饰", ["Waterfalls"] = "装饰", ["Ice"] = "装饰",
		["Pots"] = "装饰", ["Piles"] = "装饰", ["Sunflowers"] = "装饰", ["Planting Trees"] = "装饰",
		["Herbs"] = "装饰", ["Dye Plants"] = "装饰", ["Webs And Honey"] = "装饰", ["Weeds"] = "装饰",
		["Glowing Mushrooms and Jungle Plants"] = "装饰", ["Jungle Plants"] = "装饰",
		["Vines"] = "装饰", ["Flowers"] = "装饰", ["Mushrooms"] = "装饰",
		["Gems In Ice Biome"] = "装饰", ["Random Gems"] = "装饰", ["Moss Grass"] = "装饰",
		["Larva"] = "装饰", ["Cactus, Palm Trees, & Coral"] = "装饰",
		["Water Plants"] = "装饰", ["Stalac"] = "装饰",
		["Spawn Point"] = "游戏逻辑", ["Guide"] = "游戏逻辑",
		["Skyblock"] = "秘密种子",
		["clear"] = "初始化", ["Reset"] = "初始化",
	};

	private static WorldGenPipeline? _pipeline;
	private static IReadOnlyList<GenPass>? _passes;
	private static int _pollPreviousCount;

	public static bool IsRunning => _pipeline?.IsRunning ?? false;
	public static bool IsFinished => _pipeline?.IsFinished ?? false;
	public static int CompletedPassCount => _pipeline?.CompletedPassCount ?? 0;

	public static void Initialize(int width, int height, int seed)
	{
		Abort();

		if (WorldGenSession.Backend == WorldGenBackend.Native)
			throw new InvalidOperationException("项目原生 Pass 尚未实现。");

		_pollPreviousCount = 0;
		_pipeline = TerrariaPassCatalog.CreatePipeline(width, height, seed);
		_passes = _pipeline.Passes;
		_pipeline.Start();
	}

	public static List<PassInfo> GetPassList()
	{
		var list = new List<PassInfo>();
		if (_passes == null)
			return list;

		int i = 0;
		var results = WorldGen.Manifest.GenPassResults;
		foreach (var pass in _passes)
		{
			var pi = new PassInfo
			{
				Index = i,
				Name = pass.Name,
				Weight = pass.Weight,
				Enabled = pass.Enabled,
				Category = CategoryMap.GetValueOrDefault(pass.Name, "其他"),
			};

			if (i < results.Count)
				pi.DurationMs = results[i].DurationMs;

			list.Add(pi);
			i++;
		}

		return list;
	}

	public static void StepForward() => _pipeline?.StepForward();
	public static void RunToPass(int targetIndex) => _pipeline?.RunToPass(targetIndex);
	public static void RunAll() => _pipeline?.RunAll();
	public static void Pause() => _pipeline?.Pause();

	public static void Abort()
	{
		_pipeline?.Abort();
		_pipeline = null;
		_passes = null;
		_pollPreviousCount = 0;
	}

	public static PollResult PollProgress()
	{
		var result = new PollResult();
		if (_pipeline == null || _passes == null)
			return result;

		var poll = _pipeline.PollProgress(ref _pollPreviousCount);
		result.CompletedPassCount = poll.CompletedPassCount;
		result.TotalPassCount = poll.TotalPassCount;
		result.IsPaused = poll.IsPaused;
		result.JustCompleted = poll.JustCompleted;
		result.LastCompletedPassName = poll.LastCompletedPassName;
		result.LastCompletedPassIndex = poll.LastCompletedPassIndex;
		result.LastCompletedDurationMs = poll.LastCompletedDurationMs;
		return result;
	}

	public static WorldState SnapshotTiles()
	{
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

	public struct PollResult
	{
		public int CompletedPassCount;
		public int TotalPassCount;
		public bool IsPaused;
		public bool JustCompleted;
		public string LastCompletedPassName;
		public int LastCompletedPassIndex;
		public int LastCompletedDurationMs;
	}
}
