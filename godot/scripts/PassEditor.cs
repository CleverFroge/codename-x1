using System;
using System.Collections.Generic;
using Godot;
using CodenameX1.World;

namespace CodenameX1;

public partial class PassEditor : Control
{
	[Export] public int DefaultSeed = 42;

	private LineEdit? _seedEdit;
	private Button? _generateBtn;
	private Button? _stepBtn;
	private Button? _runBtn;
	private Button? _runAllBtn;
	private Button? _resetBtn;
	private Label? _statusLabel;
	private ItemList? _passList;
	private WorldView? _worldView;
	private Camera2D? _camera;
	private SubViewport? _viewport;

	private readonly List<PassInfo> _passes = new();
	private int _currentPassIndex = -1;
	private bool _initialized;
	private bool _renderPending;
	private bool _cameraInitialized;

	public override void _Ready()
	{
		_seedEdit = GetNode<LineEdit>("VBox/Toolbar/SeedEdit");
		_generateBtn = GetNode<Button>("VBox/Toolbar/GenerateBtn");
		_stepBtn = GetNode<Button>("VBox/Toolbar/StepBtn");
		_runBtn = GetNode<Button>("VBox/Toolbar/RunBtn");
		_runAllBtn = GetNode<Button>("VBox/Toolbar/RunAllBtn");
		_resetBtn = GetNode<Button>("VBox/Toolbar/ResetBtn");
		_statusLabel = GetNode<Label>("VBox/Toolbar/StatusLabel");
		_passList = GetNode<ItemList>("VBox/Split/PassList");
		_worldView = GetNode<WorldView>("VBox/Split/ViewContainer/WorldViewport/WorldView");
		_camera = GetNode<Camera2D>("VBox/Split/ViewContainer/WorldViewport/WorldView/Camera2D");
		_viewport = GetNode<SubViewport>("VBox/Split/ViewContainer/WorldViewport");

		_generateBtn.Pressed += OnGenerate;
		_stepBtn.Pressed += OnStep;
		_runBtn.Pressed += OnRunToSelected;
		_runAllBtn.Pressed += OnRunAll;
		_resetBtn.Pressed += OnReset;
		_passList.ItemSelected += OnPassListItemSelected;

		_seedEdit.Text = DefaultSeed.ToString();

		_stepBtn.Disabled = true;
		_runBtn.Disabled = true;
		_runAllBtn.Disabled = true;
		_resetBtn.Disabled = true;

		if (WorldGenSession.Backend == WorldGenBackend.Native)
		{
			_generateBtn.Disabled = true;
			_statusLabel.Text = "模式：项目原生 — Pass 管线尚未接入，请返回主界面改选「泰拉瑞亚参考」。| Esc: 主界面";
		}
		else
		{
			_statusLabel.Text = "模式：泰拉瑞亚参考 | Press Generate to start. | Ctrl+Scroll: Zoom | Esc: 主界面";
		}
	}

	private void OnGenerate()
	{
		if (WorldGenSession.Backend != WorldGenBackend.Terraria)
		{
			_statusLabel!.Text = "项目原生生成逻辑尚未接入。";
			return;
		}

		if (!int.TryParse(_seedEdit!.Text, out int seed))
			seed = DefaultSeed;

		_statusLabel!.Text = "Initializing Terraria engine...";
		_generateBtn!.Disabled = true;

		_ = System.Threading.Tasks.Task.Run(() =>
		{
			try
			{
				WorldGenHostExt.Initialize((int)WorldConfig.GetSize(WorldSize.HalfSmall).W,
					(int)WorldConfig.GetSize(WorldSize.HalfSmall).H, seed);
			}
			catch (Exception ex)
			{
				GD.PrintErr($"Initialize failed: {ex}");
				CallDeferred(MethodName.OnInitializeFailed, ex.Message);
			}
		});
	}

	private void OnInitializeFailed(string message)
	{
		_statusLabel!.Text = $"Initialize failed: {message}";
		_generateBtn!.Disabled = false;
	}

	private void RefreshPassList()
	{
		_passList!.Clear();
		_passes.Clear();

		foreach (var pass in WorldGenHostExt.GetPassList())
		{
			_passes.Add(pass);
			_passList.AddItem($"{pass.Index + 1,3:D3}  {pass.Name,-32} [{pass.Category}]");
		}

		if (_passes.Count > 0)
			_passList.Select(0);

		_currentPassIndex = WorldGenHostExt.CompletedPassCount;
	}

	private void OnStep()
	{
		WorldGenHostExt.StepForward();
		_stepBtn!.Disabled = true;
		_runBtn!.Disabled = true;
		_runAllBtn!.Disabled = true;
	}

	private void OnRunToSelected()
	{
		var selected = _passList!.GetSelectedItems();
		if (selected.Length == 0) return;
		int targetIdx = selected[0];
		WorldGenHostExt.RunToPass(targetIdx);
		_stepBtn!.Disabled = true;
		_runBtn!.Disabled = true;
		_runAllBtn!.Disabled = true;
	}

	private void OnRunAll()
	{
		WorldGenHostExt.RunAll();
		_stepBtn!.Disabled = true;
		_runBtn!.Disabled = true;
		_runAllBtn!.Disabled = true;
	}

	private void OnReset()
	{
		if (!int.TryParse(_seedEdit!.Text, out int seed))
			seed = DefaultSeed;

		WorldGenHostExt.Abort();

		_stepBtn!.Disabled = true;
		_runBtn!.Disabled = true;
		_runAllBtn!.Disabled = true;
		_resetBtn!.Disabled = true;
		_passList!.Clear();
		_passes.Clear();
		_statusLabel!.Text = "Press Generate to start.";
		_initialized = false;
		_cameraInitialized = false;
	}

	private void OnPassListItemSelected(long index)
	{
		int idx = (int)index;
		if (idx >= _passes.Count) return;
		var pass = _passes[idx];
		_statusLabel!.Text = $"Pass {pass.Index + 1}/{_passes.Count}: {pass.Name} | Weight: {pass.Weight:F1} | {pass.Category}";
	}

	public override void _Process(double delta)
	{
		if (!WorldGenHostExt.IsRunning && !WorldGenHostExt.IsFinished && !_initialized)
			return;

		if (!_initialized && WorldGenHostExt.GetPassList().Count > 0)
		{
			_initialized = true;
			RefreshPassList();
			_generateBtn!.Disabled = false;
			_stepBtn!.Disabled = false;
			_runBtn!.Disabled = false;
			_runAllBtn!.Disabled = false;
			_resetBtn!.Disabled = false;
			_statusLabel!.Text = $"Ready. {_passes.Count} passes registered. Press Step to execute pass 1.";
			_renderPending = true;
		}

		var poll = WorldGenHostExt.PollProgress();

		if (poll.JustCompleted)
		{
			_currentPassIndex = poll.CompletedPassCount;
			_renderPending = true;

			string msg = $"Pass {poll.LastCompletedPassIndex + 1}/{poll.TotalPassCount}: {poll.LastCompletedPassName} completed ({poll.LastCompletedDurationMs}ms)";
			_statusLabel!.Text = msg;

			if (poll.CompletedPassCount < _passes.Count)
				_passList!.Select(poll.CompletedPassCount);

			_stepBtn!.Disabled = false;
			_runBtn!.Disabled = false;
			_runAllBtn!.Disabled = false;
		}

		if (poll.IsPaused && poll.CompletedPassCount >= poll.TotalPassCount)
		{
			_statusLabel!.Text = "All passes complete.";
			_stepBtn!.Disabled = true;
			_runBtn!.Disabled = true;
			_runAllBtn!.Disabled = true;
		}

		if (_renderPending)
		{
			_renderPending = false;
			try
			{
				var world = WorldGenHostExt.SnapshotTiles();
				_worldView!.SetWorld(world);
				if (!_cameraInitialized)
				{
					CenterCameraOnWorld();
					_cameraInitialized = true;
				}
			}
			catch (Exception ex)
			{
				GD.PrintErr($"Snapshot failed: {ex}");
			}
		}

		HandlePanZoom((float)delta);
		if (_worldView != null && _camera != null)
			_worldView.UpdateVisibleChunks(_camera);
	}

	private void CenterCameraOnWorld()
	{
		if (_camera == null || _worldView == null) return;
		var size = _worldView.GetWorldPixelSize();
		if (size == Vector2.Zero) return;
		_camera.Position = size * 0.5f;
		_camera.Zoom = new Vector2(0.35f, 0.35f);
	}

	private float _panSpeed = 800f;

	private void HandlePanZoom(float delta)
	{
		if (_camera == null) return;

		var move = Vector2.Zero;
		if (Input.IsActionPressed("ui_left")) move.X -= 1;
		if (Input.IsActionPressed("ui_right")) move.X += 1;
		if (Input.IsActionPressed("ui_up")) move.Y -= 1;
		if (Input.IsActionPressed("ui_down")) move.Y += 1;
		if (move != Vector2.Zero)
			_camera.Position += move.Normalized() * _panSpeed * delta / _camera.Zoom.X;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (_camera == null || _viewport == null) return;

		if (@event is InputEventMouseButton mb && mb.Pressed)
		{
			bool ctrl = mb.CtrlPressed;
			float step = 0.05f;
			if (mb.ButtonIndex == MouseButton.WheelUp && ctrl)
			{
				_camera.Zoom = (_camera.Zoom + Vector2.One * step).Clamp(Vector2.One * 0.05f, Vector2.One * 4f);
				GetViewport().SetInputAsHandled();
			}
			else if (mb.ButtonIndex == MouseButton.WheelDown && ctrl)
			{
				_camera.Zoom = (_camera.Zoom - Vector2.One * step).Clamp(Vector2.One * 0.05f, Vector2.One * 4f);
				GetViewport().SetInputAsHandled();
			}
		}
		if (@event is InputEventKey key && key.Pressed && !key.Echo && key.Keycode == Key.Escape)
		{
			WorldGenHostExt.Abort();
			GetTree().ChangeSceneToFile("res://scenes/main_menu/main_menu.tscn");
		}
	}
}
