using System;
using System.IO;
using System.Reflection;
using Terraria.IO;
using Terraria.Initializers;
using Terraria.Localization;
using Terraria.Social;
using Terraria.WorldBuilding;

namespace Terraria.Port;

/// <summary>
/// Minimal headless bootstrap for Terraria.WorldGen.GenerateWorld (dedServ-style init, no game loop).
/// </summary>
public static class WorldGenHost
{
	private static bool _initialized;
	private static Main? _main;

	public static bool IsInitialized => _initialized;

	public static void EnsureInitialized()
	{
		if (_initialized)
		{
			return;
		}

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
		typeof(Main).GetMethod("Initialize", BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(_main, null);

		_initialized = true;
	}

	/// <summary>Run the vanilla 97-pass pipeline. Returns false if generation aborted.</summary>
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
		{
			progress.TotalWeight = 1.0;
		}

		string lastStatus = "";
		void Report()
		{
			string msg = !string.IsNullOrEmpty(Main.statusText) ? Main.statusText : progress.Message;
			if (string.IsNullOrEmpty(msg))
			{
				msg = "Generating...";
			}
			float ratio = (float)Math.Clamp(progress.TotalProgress, 0.0, 1.0);
			if (msg != lastStatus || onProgress != null)
			{
				lastStatus = msg;
				onProgress?.Invoke(msg, ratio);
			}
		}

		Report();
		bool ok = WorldGen.GenerateWorld(progress, null);
		Report();
		return ok;
	}

	/// <summary>Headless metadata without writing favorites (avoids Newtonsoft + System.Security.Permissions in Godot).</summary>
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
