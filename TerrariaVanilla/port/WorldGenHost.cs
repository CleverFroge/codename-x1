using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Terraria.IO;
using Terraria.Initializers;
using Terraria.Localization;
using Terraria.Social;
using Terraria.Utilities;
using Terraria.WorldBuilding;

namespace Terraria.Port;

/// <summary>
/// Minimal headless bootstrap for Terraria world generation (dedServ-style init, no game loop).
/// Pass 执行使用 <see cref="WorldGen.PreparePasses"/> + 本类内联调度，不调用泰拉 WorldGenerator.GenerateWorld。
/// </summary>
public static class WorldGenHost
{
	private static bool _initialized;
	private static Main? _main;

	public static bool IsInitialized => _initialized;

	public static void EnsureInitialized()
	{
		if (_initialized)
			return;

		RuntimeDependencyLoader.EnsureRegistered();

		Program.SavePath = Path.Combine(Path.GetTempPath(), "CodenameX1Terraria");
		Directory.CreateDirectory(Program.SavePath);

		LanguageManager.Instance.SetLanguage(GameCulture.DefaultCulture);
		Lang.InitializeLegacyLocalization();

		Main.dedServ = true;
		Main.showSplash = false;
		SocialAPI.Initialize(SocialMode.None);

		_main = new Main();
		LaunchInitializer.LoadParameters(_main);
		typeof(Main).GetMethod("Initialize", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!
			.Invoke(_main, null);

		_initialized = true;
	}

	/// <summary>Run generation via PreparePasses + inline pass runner.</summary>
	public static bool Generate(int width, int height, int seed, Action<string, float>? onProgress = null)
	{
		EnsureInitialized();

		Main.maxTilesX = width;
		Main.maxTilesY = height;
		Main.worldName = "CodenameX1";
		Main.GameMode = 0;
		Main.ActiveWorldFileData = CreateWorldMetadata(seed);

		var progress = new GenerationProgress();
		if (onProgress != null)
			progress.TotalWeight = 1.0;

		string lastStatus = "";
		void Report()
		{
			string msg = !string.IsNullOrEmpty(Main.statusText) ? Main.statusText : progress.Message;
			if (string.IsNullOrEmpty(msg))
				msg = "Generating...";
			float ratio = (float)Math.Clamp(progress.TotalProgress, 0.0, 1.0);
			if (msg != lastStatus || onProgress != null)
			{
				lastStatus = msg;
				onProgress?.Invoke(msg, ratio);
			}
		}

		Report();
		var passes = WorldGen.PreparePasses(progress, null);
		RunAllPassesBlocking(passes, Main.ActiveWorldFileData.Seed, GenVars.configuration, progress);
		Report();
		return true;
	}

	internal static void RunAllPassesBlocking(
		IReadOnlyList<GenPass> passes,
		int seed,
		WorldGenConfiguration configuration,
		GenerationProgress progress)
	{
		progress.TotalWeight = passes.Where(p => p.Enabled).Sum(p => p.Weight);
		while (WorldGen.Manifest.GenPassResults.Count < passes.Count)
		{
			GenPass pass = passes[WorldGen.Manifest.GenPassResults.Count];
			lock (pass)
			{
				WorldGen.Manifest.GenPassResults.Add(RunPass(pass, seed, configuration, progress));
			}
		}

		WorldGen.Finish();
	}

	private static GenPassResult RunPass(GenPass pass, int seed, WorldGenConfiguration configuration, GenerationProgress progress)
	{
		if (!pass.Enabled)
		{
			return new GenPassResult { Name = pass.Name, Skipped = true };
		}

		var stopwatch = Stopwatch.StartNew();
		Main.rand = new UnifiedRandom(seed);
		progress.Start(pass.Weight);
		try
		{
			pass.Apply(progress, configuration.GetPassConfiguration(pass.Name));
		}
		catch (Exception ex)
		{
			Trace.WriteLine($"Exception in Pass {pass.Name}: {ex}");
		}

		progress.End();
		return new GenPassResult
		{
			Name = pass.Name,
			DurationMs = (int)stopwatch.ElapsedMilliseconds,
			RandNext = WorldGen.genRand.Next(),
		};
	}

	private static WorldFileData CreateWorldMetadata(int seed)
	{
		var data = new WorldFileData(Main.GetWorldPathFromName(Main.worldName, cloudSave: false), cloudSave: false);
		data.Name = Main.worldName;
		data.GameMode = Main.GameMode;
		data.CreationTime = DateTime.Now;
		data.Metadata = FileMetadata.FromCurrentSettings(FileType.World);
		data.SetFavorite(favorite: false, saveChanges: false);
		data.WorldGeneratorVersion = 1370094567425uL;
		data.UniqueId = Guid.NewGuid();
		data.SetSeed(seed.ToString());
		return data;
	}
}
