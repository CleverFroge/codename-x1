extends Node
## 检测 C# 是否成功加载；若仍停在 Loading 则提示用户换 Mono 版 Godot。

func _ready() -> void:
	await get_tree().create_timer(0.5).timeout
	var scene := get_tree().current_scene
	if scene == null:
		return
	var label: Label = scene.get_node_or_null("UI/StatusLabel")
	if label == null:
		return
	if label.text != "Loading...":
		return
	label.text = "C# 未加载 — 请用 Godot 4.7 Mono 打开本项目（不是 4.2 标准版）"
	push_error("CodenameX1: C# scripts not running. Open with Godot 4.7 Mono and Build Project.")
