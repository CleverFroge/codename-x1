using Godot;
using CodenameX1.World;
using Terraria.Port;

namespace CodenameX1;

public partial class Main : Node2D
{
	private WorldView? _worldView;
	private Camera2D? _camera;
	private Label? _status;
	private readonly WorldGenerator _gen = new();
	private float _panSpeed = 800f;

	private readonly object _genLock = new();
	private bool _generating;
	private bool _genComplete;
	private WorldState? _pendingWorld;
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
		_camera = GetNode<Camera2D>("Camera2D");
		_gen.ProgressChanged += OnProgress;
		Generate(42);
	}

	private void Generate(int seed)
	{
		if (_generating) return;
		_generating = true;
		_status!.Text = $"Generating (seed {seed})...";

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
		int seed = _pendingSeed;
		long genMs = _pendingGenMs;

		var sw = System.Diagnostics.Stopwatch.StartNew();
		_worldView!.Render(world);
		sw.Stop();

		var size = _worldView.GetWorldPixelSize();
		_camera!.Position = size * 0.5f;
		_camera.Zoom = new Vector2(0.35f, 0.35f);
		_status!.Text =
			$"Terraria-style world | seed {seed} | {world.MaxTilesX}x{world.MaxTilesY} | WASD pan, wheel zoom, R regen, Esc 主界面";
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

		var move = Vector2.Zero;
		if (Input.IsActionPressed("ui_left")) move.X -= 1;
		if (Input.IsActionPressed("ui_right")) move.X += 1;
		if (Input.IsActionPressed("ui_up")) move.Y -= 1;
		if (Input.IsActionPressed("ui_down")) move.Y += 1;
		if (move != Vector2.Zero)
			_camera!.Position += move.Normalized() * _panSpeed * (float)delta / _camera.Zoom.X;
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
