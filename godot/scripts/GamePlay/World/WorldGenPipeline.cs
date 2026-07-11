using System.Diagnostics;
using Terraria;
using Terraria.IO;
using Terraria.Utilities;
using Terraria.WorldBuilding;

namespace CodenameX1.World;

/// <summary>
/// 项目自研 Pass 执行管线：负责调度、暂停、逐步执行；不依赖泰拉 WorldGenerator.GenerateWorld。
/// </summary>
public sealed class WorldGenPipeline
{
	private readonly List<GenPass> _passes;
	private readonly int _seed;
	private readonly WorldGenConfiguration _configuration;
	private readonly GenerationProgress _progress;
	private readonly object _controlLock = new();

	private Thread? _thread;
	private volatile bool _running;
	private volatile bool _paused = true;
	private GenPass? _pauseAfterPass;
	private volatile bool _abortQueued;
	private volatile bool _finished;

	public IReadOnlyList<GenPass> Passes => _passes;
	public GenerationProgress Progress => _progress;

	public bool IsRunning => _running;
	public bool IsFinished => _finished;
	public bool IsPaused => _paused;
	public int CompletedPassCount => WorldGen.Manifest.GenPassResults.Count;

	public WorldGenPipeline(
		IReadOnlyList<GenPass> passes,
		int seed,
		WorldGenConfiguration configuration,
		GenerationProgress? progress = null)
	{
		_passes = passes.ToList();
		_seed = seed;
		_configuration = configuration;
		_progress = progress ?? new GenerationProgress();
		_progress.TotalWeight = _passes.Where(p => p.Enabled).Sum(p => p.Weight);
	}

	public void Start()
	{
		if (_thread != null)
			return;

		_running = true;
		_finished = false;
		_thread = new Thread(RunLoop)
		{
			IsBackground = true,
			Name = "WorldGenPipeline",
		};
		_thread.Start();
	}

	public void StepForward()
	{
		if (!IsAlive() || CompletedPassCount >= _passes.Count)
			return;

		_pauseAfterPass = _passes[CompletedPassCount];
		_paused = false;
	}

	public void RunToPass(int targetIndex)
	{
		if (!IsAlive() || targetIndex >= _passes.Count)
			return;

		_pauseAfterPass = _passes[targetIndex];
		_paused = false;
	}

	public void RunAll()
	{
		if (!IsAlive())
			return;

		_pauseAfterPass = null;
		_paused = false;
	}

	public void Pause()
	{
		_paused = true;
		_pauseAfterPass = null;
	}

	public void Abort()
	{
		_abortQueued = true;
		_paused = false;

		if (_thread != null && _thread.IsAlive)
			_thread.Join(5000);

		_thread = null;
		_running = false;
		_finished = true;
	}

	/// <summary>同步跑完全部 Pass（游戏主场景一键生成用）。</summary>
	public void RunAllBlocking()
	{
		_pauseAfterPass = null;
		_paused = false;
		_abortQueued = false;

		while (!_abortQueued && CompletedPassCount < _passes.Count)
		{
			lock (_controlLock)
			{
				if (CompletedPassCount >= _passes.Count)
					break;

				GenPass pass = _passes[CompletedPassCount];
				lock (pass)
				{
					WorldGen.Manifest.GenPassResults.Add(RunPass(pass));
				}
			}
		}

		if (!_abortQueued && CompletedPassCount >= _passes.Count)
			WorldGen.Finish();

		_running = false;
		_finished = true;
	}

	public PipelinePollResult PollProgress(ref int previousCompletedCount)
	{
		var result = new PipelinePollResult
		{
			CompletedPassCount = CompletedPassCount,
			TotalPassCount = _passes.Count,
			IsPaused = _paused,
		};

		result.JustCompleted = result.CompletedPassCount > previousCompletedCount;
		if (result.JustCompleted && previousCompletedCount < _passes.Count)
		{
			GenPass pass = _passes[previousCompletedCount];
			result.LastCompletedPassName = pass.Name;
			result.LastCompletedPassIndex = previousCompletedCount;

			var results = WorldGen.Manifest.GenPassResults;
			if (previousCompletedCount < results.Count)
				result.LastCompletedDurationMs = results[previousCompletedCount].DurationMs;
		}

		previousCompletedCount = result.CompletedPassCount;

		if (result.CompletedPassCount >= result.TotalPassCount && result.TotalPassCount > 0)
		{
			_finished = true;
			_running = false;
			try { WorldGen.Finish(); } catch { /* headless */ }
		}

		return result;
	}

	private void RunLoop()
	{
		try
		{
			while (!_abortQueued)
			{
				if (_paused)
				{
					Thread.Sleep(10);
					continue;
				}

				lock (_controlLock)
				{
					if (WorldGen.Manifest.GenPassResults.Count >= _passes.Count)
					{
						_paused = true;
						break;
					}

					GenPass pass = _passes[WorldGen.Manifest.GenPassResults.Count];
					lock (pass)
					{
						WorldGen.Manifest.GenPassResults.Add(RunPass(pass));
					}

					if (_pauseAfterPass == pass)
						_paused = true;
				}
			}
		}
		catch (Exception ex)
		{
			Godot.GD.PrintErr($"WorldGenPipeline crashed: {ex}");
		}
		finally
		{
			_running = false;
			_finished = true;
			if (!_abortQueued && CompletedPassCount >= _passes.Count && _passes.Count > 0)
			{
				try { WorldGen.Finish(); } catch { /* headless */ }
			}
		}
	}

	private GenPassResult RunPass(GenPass pass)
	{
		if (!pass.Enabled)
		{
			return new GenPassResult
			{
				Name = pass.Name,
				Skipped = true,
			};
		}

		var stopwatch = Stopwatch.StartNew();
		Terraria.Main.rand = new UnifiedRandom(_seed);
		_progress.Start(pass.Weight);
		try
		{
			pass.Apply(_progress, _configuration.GetPassConfiguration(pass.Name));
		}
		catch (Exception ex)
		{
			Godot.GD.PrintErr($"Exception in Pass {pass.Name}: {ex}");
		}

		_progress.End();
		return new GenPassResult
		{
			Name = pass.Name,
			DurationMs = (int)stopwatch.ElapsedMilliseconds,
			RandNext = WorldGen.genRand.Next(),
		};
	}

	private bool IsAlive() => _thread != null && _running && !_finished;

	public struct PipelinePollResult
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
