using Godot;
using CodenameX1.World;

namespace CodenameX1;

/// <summary>
/// 主界面：进入游戏或 Pass Editor（先选生成逻辑）。
/// 仅 UI 系统代码放在 scripts/System/（命名空间用 CodenameX1，勿用 .System）。
/// </summary>
public partial class MainMenu : Control
{
	private const string GameScene = "res://scenes/main/main.tscn";
	private const string PassEditorScene = "res://scenes/main_menu/pass_editor.tscn";

	private Control? _mainPanel;
	private Control? _modePanel;

	public override void _Ready()
	{
		_mainPanel = GetNode<Control>("Center/MainPanel");
		_modePanel = GetNode<Control>("Center/ModePanel");

		GetNode<Button>("Center/MainPanel/VBox/GameBtn").Pressed += () =>
			GetTree().ChangeSceneToFile(GameScene);
		GetNode<Button>("Center/MainPanel/VBox/PassEditorBtn").Pressed += ShowModeSelect;

		GetNode<Button>("Center/ModePanel/VBox/NativeBtn").Pressed += () =>
			OpenPassEditor(WorldGenBackend.Native);
		GetNode<Button>("Center/ModePanel/VBox/TerrariaBtn").Pressed += () =>
			OpenPassEditor(WorldGenBackend.Terraria);
		GetNode<Button>("Center/ModePanel/VBox/BackBtn").Pressed += ShowMain;

		ShowMain();
	}

	private void ShowMain()
	{
		_mainPanel!.Visible = true;
		_modePanel!.Visible = false;
	}

	private void ShowModeSelect()
	{
		_mainPanel!.Visible = false;
		_modePanel!.Visible = true;
	}

	private void OpenPassEditor(WorldGenBackend backend)
	{
		WorldGenSession.Backend = backend;
		GetTree().ChangeSceneToFile(PassEditorScene);
	}
}
