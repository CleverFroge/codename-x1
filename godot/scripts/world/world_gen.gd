class_name WorldGen
extends RefCounted
## Terraria WorldGen.GenerateWorld 精简复现：Reset → Pass 流水线 → Finish

signal progress_changed(ratio: float, message: String)
signal generation_finished(world: WorldState)

var world: WorldState
var rng: GenRandom = GenRandom.new()
var seed_value: int = 0


func generate(world_size: WorldConfig.WorldSize = WorldConfig.WorldSize.DEV, seed: int = -1) -> WorldState:
	_reset(world_size, seed)
	_add_passes()
	return world


func _reset(world_size: WorldConfig.WorldSize, seed: int) -> void:
	var dims := WorldConfig.get_dimensions(world_size)
	world = WorldState.new(dims.x, dims.y)
	seed_value = seed if seed >= 0 else randi()
	rng.seed_from(seed_value)
	world.gen_vars = GenVars.new()


func _add_passes() -> void:
	_run_pass("Terrain", 0.0, 0.5, func():
		TerrainPass.apply(world, rng)
	)
	_run_pass("RockLayerCaves", 0.5, 1.0, func():
		RockLayerCavesPass.apply(world, rng)
	)
	generation_finished.emit(world)


func _run_pass(name: String, start: float, end: float, callback: Callable) -> void:
	progress_changed.emit(start, "Generating: %s" % name)
	callback.call()
	progress_changed.emit(end, "Done: %s" % name)
