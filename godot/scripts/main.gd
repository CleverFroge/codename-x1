extends Node2D

@onready var world_renderer: WorldRenderer = $WorldRenderer
@onready var camera: Camera2D = $Camera2D
@onready var status_label: Label = $UI/StatusLabel

var _world_gen := WorldGen.new()
var _pan_speed := 600.0
var _zoom_step := 0.1
var _min_zoom := 0.1
var _max_zoom := 4.0


func _ready() -> void:
	_world_gen.progress_changed.connect(_on_gen_progress)
	_world_gen.generation_finished.connect(_on_gen_finished)
	_generate_world()


func _generate_world(seed: int = 42) -> void:
	status_label.text = "Generating world (seed %d)..." % seed
	await get_tree().process_frame
	_world_gen.generate(WorldConfig.WorldSize.DEV, seed)


func _on_gen_progress(ratio: float, message: String) -> void:
	status_label.text = "%s (%.0f%%)" % [message, ratio * 100.0]


func _on_gen_finished(world: WorldState) -> void:
	world_renderer.render(world)
	var size_px := world_renderer.get_world_size_pixels()
	camera.position = size_px * 0.5
	camera.zoom = Vector2(0.5, 0.5)
	status_label.text = "World ready — seed %d | %dx%d tiles | WASD pan, wheel zoom, R regen" % [
		_world_gen.seed_value,
		world.max_tiles_x,
		world.max_tiles_y,
	]
	print("World generated: seed=%d surface=%d rock=%d" % [
		_world_gen.seed_value,
		world.world_surface,
		world.rock_layer,
	])


func _process(delta: float) -> void:
	var move := Vector2.ZERO
	if Input.is_action_pressed("ui_left"):
		move.x -= 1.0
	if Input.is_action_pressed("ui_right"):
		move.x += 1.0
	if Input.is_action_pressed("ui_up"):
		move.y -= 1.0
	if Input.is_action_pressed("ui_down"):
		move.y += 1.0
	if move != Vector2.ZERO:
		camera.position += move.normalized() * _pan_speed * delta / camera.zoom.x


func _unhandled_input(event: InputEvent) -> void:
	if event is InputEventMouseButton:
		var mb := event as InputEventMouseButton
		if mb.pressed:
			if mb.button_index == MOUSE_BUTTON_WHEEL_UP:
				camera.zoom = (camera.zoom + Vector2.ONE * _zoom_step).clampf(_min_zoom, _max_zoom)
			elif mb.button_index == MOUSE_BUTTON_WHEEL_DOWN:
				camera.zoom = (camera.zoom - Vector2.ONE * _zoom_step).clampf(_min_zoom, _max_zoom)
	if event is InputEventKey:
		var key := event as InputEventKey
		if key.pressed and not key.echo and key.keycode == KEY_R:
			_generate_world(randi())
