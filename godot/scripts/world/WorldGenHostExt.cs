using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace CodenameX1.World;

public static class WorldGenHostExt
{
	private static readonly Dictionary<string, string> _categoryMap = new()
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

	private static Type? _controllerType;
	private static Type? _snapshotFreqType;
	private static object? _controller;
	private static Thread? _genThread;
	private static bool _genRunning;
	private static int _completedPassCount;
	private static bool _genFinished;
	private static List<GenPass>? _passes;

	public static bool IsRunning => _genRunning;
	public static bool IsFinished => _genFinished;
	public static int CompletedPassCount => _completedPassCount;

	private static Type ControllerType
	{
		get
		{
			if (_controllerType != null) return _controllerType;

			var terrariaWG = typeof(Terraria.WorldBuilding.WorldGenerator);

			// Try GetNestedType first
			_controllerType = terrariaWG.GetNestedType("Controller", BindingFlags.Public | BindingFlags.NonPublic);

			// Fallback: try resolving by full qualified name from the assembly
			if (_controllerType == null)
			{
				_controllerType = terrariaWG.Assembly.GetType("Terraria.WorldBuilding.WorldGenerator+Controller");
			}

			if (_controllerType == null)
			{
				var allNested = terrariaWG.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic);
				Godot.GD.PrintErr(
					$"WorldGenerator.Controller not found in {terrariaWG.Assembly.FullName}. " +
					$"Available: [{string.Join(", ", allNested.Select(t => t.FullName))}]");
				throw new InvalidOperationException(
					$"WorldGenerator.Controller not found. See error log for details.");
			}
			return _controllerType;
		}
	}

	private static Type SnapshotFreqType
	{
		get
		{
			if (_snapshotFreqType != null) return _snapshotFreqType;

			var terrariaWG = typeof(Terraria.WorldBuilding.WorldGenerator);

			_snapshotFreqType = terrariaWG.GetNestedType("SnapshotFrequency", BindingFlags.Public | BindingFlags.NonPublic);
			if (_snapshotFreqType == null)
			{
				_snapshotFreqType = terrariaWG.Assembly.GetType("Terraria.WorldBuilding.WorldGenerator+SnapshotFrequency");
			}
			if (_snapshotFreqType == null)
			{
				var allNested = terrariaWG.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic);
				Godot.GD.PrintErr(
					$"WorldGenerator.SnapshotFrequency not found. Available: [{string.Join(", ", allNested.Select(t => t.FullName))}]");
				throw new InvalidOperationException("WorldGenerator.SnapshotFrequency not found.");
			}
			return _snapshotFreqType;
		}
	}

	public static void Initialize(int width, int height, int seed)
	{
		Abort();

		Terraria.Port.WorldGenHost.EnsureInitialized();

		Terraria.Main.maxTilesX = width;
		Terraria.Main.maxTilesY = height;
		Terraria.Main.worldName = "PassEditor";
		Terraria.Main.GameMode = 0;
		Terraria.Main.ActiveWorldFileData = CreateWorldMetadata(seed);

		var genVarsType = typeof(Terraria.WorldBuilding.GenVars);
		var configField = genVarsType.GetField("configuration", BindingFlags.Public | BindingFlags.Static);
		configField?.SetValue(null, WorldGenConfiguration.FromEmbeddedPath(
			"Terraria.GameContent.WorldBuilding.Configuration.json"));

		_controller = Activator.CreateInstance(ControllerType, [null])!;
		SetProp("Paused", true);
		SetProp("SnapshotFrequency", Enum.Parse(SnapshotFreqType, "None"));
		SetProp("PauseOnHashMismatch", false);

		_completedPassCount = 0;
		_genFinished = false;
		_passes = null;

		_genThread = new Thread(() =>
		{
			_genRunning = true;
			try
			{
				var progress = new GenerationProgress();
				var genMethod = typeof(Terraria.WorldGen).GetMethod("GenerateWorld",
					BindingFlags.Public | BindingFlags.Static,
					null,
					[typeof(GenerationProgress), ControllerType],
					null);
				if (genMethod != null)
				{
					bool ok = (bool)genMethod.Invoke(null, [progress, _controller])!;
					Godot.GD.Print($"WorldGen.GenerateWorld finished: {ok}");
				}
				else
				{
					Godot.GD.PrintErr("Could not find GenerateWorld(GenerationProgress, Controller) method");
				}
			}
			catch (Exception ex)
			{
				Godot.GD.PrintErr($"WorldGen thread crashed: {ex}");
			}
			finally
			{
				_genRunning = false;
				_genFinished = true;
			}
		})
		{
			IsBackground = true,
			Name = "WorldGen"
		};
		_genThread.Start();

		SpinWait.SpinUntil(() =>
		{
			try
			{
				var p = GetProp("Passes") as List<GenPass>;
				if (p != null && p.Count > 0)
				{
					_passes = p;
					return true;
				}
			}
			catch { }
			return _genFinished;
		}, 30000);
	}

	public static List<PassInfo> GetPassList()
	{
		var list = new List<PassInfo>();
		if (_passes == null)
			return list;

		int i = 0;
		foreach (var pass in _passes)
		{
			var pi = new PassInfo
			{
				Index = i,
				Name = pass.Name,
				Weight = pass.Weight,
				Enabled = pass.Enabled,
				Category = _categoryMap.GetValueOrDefault(pass.Name, "其他"),
			};

			try
			{
				var results = PassResults;
				if (i < results.Count)
					pi.DurationMs = results[i].DurationMs;
			}
			catch { }

			list.Add(pi);
			i++;
		}

		try
		{
			_completedPassCount = PassResults.Count;
		}
		catch { }

		return list;
	}

	public static void StepForward()
	{
		if (!IsAlive()) return;
		var passes = _passes;
		if (passes == null || _completedPassCount >= passes.Count) return;

		SetProp("PauseAfterPass", passes[_completedPassCount]);
		SetProp("Paused", false);
	}

	public static void RunToPass(int targetIndex)
	{
		if (!IsAlive()) return;
		var passes = _passes;
		if (passes == null || targetIndex >= passes.Count) return;

		SetProp("PauseAfterPass", passes[targetIndex]);
		SetProp("Paused", false);
	}

	public static void RunAll()
	{
		if (!IsAlive()) return;
		SetProp("PauseAfterPass", null);
		SetProp("Paused", false);
	}

	public static void Pause()
	{
		if (_controller == null) return;
		SetProp("Paused", true);
	}

	public static void Abort()
	{
		if (_controller != null && _genRunning)
		{
			try
			{
				SetProp("QueuedAbort", true);
				SetProp("Paused", false);
			}
			catch { }
		}

		if (_genThread != null && _genThread.IsAlive)
			_genThread.Join(5000);

		_genThread = null;
		_controller = null;
		_passes = null;
		_genRunning = false;
	}

	public static PollResult PollProgress()
	{
		var result = new PollResult();
		if (_controller == null || _passes == null)
			return result;

		int prevCount = _completedPassCount;

		try
		{
			_completedPassCount = PassResults.Count;
			result.IsPaused = (bool)GetProp("Paused")!;
		}
		catch { return result; }

		result.CompletedPassCount = _completedPassCount;
		result.TotalPassCount = _passes.Count;
		result.JustCompleted = _completedPassCount > prevCount;

		if (result.JustCompleted && prevCount < _passes.Count)
		{
			var pass = _passes[prevCount];
			result.LastCompletedPassName = pass.Name;
			result.LastCompletedPassIndex = prevCount;

			try
			{
				var results = PassResults;
				if (prevCount < results.Count)
					result.LastCompletedDurationMs = results[prevCount].DurationMs;
			}
			catch { }
		}

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

	private static bool IsAlive() => _controller != null && _genRunning;

	private static IList<GenPassResult> PassResults => Terraria.WorldBuilding.WorldGenerator.PassResults;

	private static object? GetProp(string name)
		=> ControllerType.GetProperty(name, BindingFlags.Public | BindingFlags.Instance)?.GetValue(_controller);

	private static void SetProp(string name, object? value)
		=> ControllerType.GetProperty(name, BindingFlags.Public | BindingFlags.Instance)?.SetValue(_controller, value);

	private static WorldFileData CreateWorldMetadata(int seed)
	{
		var data = new WorldFileData(
			Terraria.Main.GetWorldPathFromName(Terraria.Main.worldName, cloudSave: false), cloudSave: false);
		data.Name = Terraria.Main.worldName;
		data.GameMode = Terraria.Main.GameMode;
		data.CreationTime = DateTime.Now;
		data.Metadata = FileMetadata.FromCurrentSettings(FileType.World);
		data.SetFavorite(favorite: false, saveChanges: false);
		data.WorldGeneratorVersion = 1370094567425uL;
		data.UniqueId = Guid.NewGuid();
		data.SetSeed(seed.ToString());
		return data;
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
