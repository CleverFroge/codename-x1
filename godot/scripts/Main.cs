using Godot;
using CodenameX1.World;
using Terraria.Port;

namespace CodenameX1;

public partial class Main : Node2D
{
	private WorldView? _worldView;
	private Camera2D? _camera;
	private PlayerController? _player;
	private Label? _status;
	private readonly WorldGenerator _gen = new();

	private readonly object _genLock = new();
	private bool _generating;
	private bool _genComplete;
	private WorldState? _pendingWorld;
	private WorldState? _world;
	private WorldRuntime? _runtime;
	private WorldSnapshotStore? _pendingSnapshot;
	private WorldSnapshotStore? _snapshotStore;
	private int _pendingSeed;
	private long _pendingGenMs;
	private string? _progressMsg;
	private float _progressRatio = -1f;

	public override void _Ready()
	{
		RuntimeDependencyLoader.EnsureRegistered();

		_status = GetNode<Label>("UI/StatusLabel");
		_status.Text = "Starting... | Esc: 主界面";

		_worldView = GetNode<WorldView>("WorldView");
		_camera = GetNode<Camera2D>("Player/Camera2D");
		_player = GetNode<PlayerController>("Player");
		_gen.ProgressChanged += OnProgress;
		Generate(42);
	}

	private void Generate(int seed)
	{
		if (_generating) return;
		_snapshotStore?.FlushLoadedDirtyChunks();
		_generating = true;
		_status!.Text = $"Generating (seed {seed})...";
		if (WorldSnapshotStore.TryOpen(seed, out var snapshot))
		{
			lock (_genLock)
			{
				_pendingWorld = snapshot.World;
				_pendingSnapshot = snapshot;
				_pendingSeed = seed;
				_pendingGenMs = 0;
				_genComplete = true;
			}
			return;
		}

		Task.Run(() =>
		{
			try
			{
				var sw = System.Diagnostics.Stopwatch.StartNew();
				var world = _gen.Generate(WorldSize.HalfSmall, seed);
				sw.Stop();
				lock (_genLock)
				{
					_pendingWorld = world;
					_pendingSnapshot = null;
					_pendingSeed = seed;
					_pendingGenMs = sw.ElapsedMilliseconds;
					_genComplete = true;
				}
			}
			catch (Exception ex)
			{
				var detail = ex.InnerException?.Message ?? ex.Message;
				GD.PrintErr($"World generation failed: {ex}");
				lock (_genLock)
				{
					_progressMsg = $"Generation failed: {detail}";
					_progressRatio = 0;
					_generating = false;
				}
			}
		});
	}

	private void OnProgress(string msg, float ratio)
	{
		lock (_genLock)
		{
			_progressMsg = msg;
			_progressRatio = ratio;
		}
	}

	private void FinishGenerate()
	{
		var world = _pendingWorld!;
		_world = world;
		_runtime = new WorldRuntime(world);
		int seed = _pendingSeed;
		_snapshotStore = _pendingSnapshot ?? WorldSnapshotStore.Create(world, seed);
		_pendingSnapshot = null;
		long genMs = _pendingGenMs;

		var sw = System.Diagnostics.Stopwatch.StartNew();
		_worldView!.SetWorld(world);
		var size = _worldView.GetWorldPixelSize();
		var spawn = new Vector2(
			world.MaxTilesX / 2f * WorldConfig.TilePixelSize,
			Mathf.Max(0, world.WorldSurface - 24) * WorldConfig.TilePixelSize);
		_player!.Spawn(spawn, size);
		_camera.Zoom = new Vector2(0.35f, 0.35f);
		UpdateRuntimeChunks(_player.GetChunkPosition(), 0);
		sw.Stop();
		_generating = false;
		GD.Print($"Generate {genMs}ms, Render {sw.ElapsedMilliseconds}ms, seed={seed}");
	}

	public override void _Process(double delta)
	{
		lock (_genLock)
		{
			if (_progressMsg != null)
			{
				_status!.Text = $"{_progressMsg} ({_progressRatio * 100:F0}%)";
				_progressMsg = null;
			}
			if (_genComplete)
			{
				_genComplete = false;
				FinishGenerate();
			}
		}

		var world = _world;
		if (world == null || _generating)
			return;

		_player!.Tick((float)delta, world);
		var tile = _player.GetTilePosition();
		var chunk = _player.GetChunkPosition();
		UpdateRuntimeChunks(chunk, delta);
		_status!.Text =
			$"seed {_pendingSeed} | tile ({tile.X}, {tile.Y}) | chunk ({chunk.X}, {chunk.Y}) | active {_runtime.ActiveChunks.Count} | retained {_runtime.RetainedChunks.Count} | ←→ move, Space jump, wheel zoom, R regen, Esc 主界面";
	}

	private void UpdateRuntimeChunks(ChunkCoord playerChunk, double delta)
	{
		_runtime!.Update(playerChunk, delta);
		var requiredChunks = _worldView!.GetVisibleChunks(_camera!);
		requiredChunks.UnionWith(_runtime.RetainedChunks);
		_snapshotStore!.EnsureLoaded(requiredChunks);
		_snapshotStore.UnloadOutside(requiredChunks);
		_worldView.UpdateVisibleChunks(_camera);
	}

	public override void _ExitTree()
	{
		_snapshotStore?.FlushLoadedDirtyChunks();
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton mb && mb.Pressed)
		{
			float step = 0.05f;
			if (mb.ButtonIndex == MouseButton.WheelUp)
				_camera!.Zoom = (_camera.Zoom + Vector2.One * step).Clamp(Vector2.One * 0.05f, Vector2.One * 4f);
			else if (mb.ButtonIndex == MouseButton.WheelDown)
				_camera!.Zoom = (_camera.Zoom - Vector2.One * step).Clamp(Vector2.One * 0.05f, Vector2.One * 4f);
		}
		if (@event is InputEventKey key && key.Pressed && !key.Echo)
		{
			if (key.Keycode == Key.R)
				Generate(Random.Shared.Next());
			else if (key.Keycode == Key.Escape)
				GetTree().ChangeSceneToFile("res://scenes/main_menu/main_menu.tscn");
		}
	}
}
