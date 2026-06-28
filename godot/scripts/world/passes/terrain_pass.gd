class_name TerrainPass
extends RefCounted
## 移植自 Terraria.GameContent.Biomes.TerrainPass
## 逐列随机游走 + 5 态状态机生成地表起伏

enum TerrainFeatureType {
	PLATEAU,
	HILL,
	DALE,
	MOUNTAIN,
	VALLEY,
}


class SurfaceHistory:
	var _heights: PackedFloat64Array
	var _index: int = 0

	func _init(size: int) -> void:
		_heights = PackedFloat64Array()
		_heights.resize(size)
		_heights.fill(0.0)

	func get_height(index: int) -> float:
		var size := _heights.size()
		return _heights[(index + _index) % size]

	func set_height(index: int, value: float) -> void:
		var size := _heights.size()
		_heights[(index + _index) % size] = value

	var length: int:
		get: return _heights.size()

	func record(height: float) -> void:
		_heights[_index] = height
		_index = (_index + 1) % _heights.size()


static func apply(world: WorldState, rng: GenRandom, flat_beach_padding: int = 0) -> void:
	var max_x := world.max_tiles_x
	var max_y := world.max_tiles_y
	var gv := world.gen_vars

	gv.left_beach_end = 0
	gv.right_beach_start = max_x

	var feature := TerrainFeatureType.PLATEAU
	var feature_timer := 0

	var surface_y := float(max_y) * 0.3
	surface_y *= float(rng.next_int(90, 110)) * 0.005

	var rock_y := surface_y + float(max_y) * 0.2
	rock_y *= float(rng.next_int(90, 110)) * 0.01

	var surface_low := surface_y
	var surface_high := surface_y
	var rock_low := rock_y
	var rock_high := rock_y

	var beach_cap := float(max_y) * 0.23
	var history := SurfaceHistory.new(500)
	feature_timer = gv.left_beach_end + flat_beach_padding

	for x in range(max_x):
		surface_low = minf(surface_y, surface_low)
		surface_high = maxf(surface_y, surface_high)
		rock_low = minf(rock_y, rock_low)
		rock_high = maxf(rock_y, rock_high)

		if feature_timer <= 0:
			feature = rng.next_int(0, 5) as TerrainFeatureType
			feature_timer = rng.next_int(5, 40)
			if feature == TerrainFeatureType.PLATEAU:
				feature_timer = int(float(rng.next_int(5, 30)) * 0.2)

		feature_timer -= 1

		var x_ratio := float(x) / float(max_x)
		if x_ratio > 0.45 and x_ratio < 0.55 and (feature == TerrainFeatureType.MOUNTAIN or feature == TerrainFeatureType.VALLEY):
			feature = rng.next_int(0, 3) as TerrainFeatureType
		if x_ratio > 0.48 and x_ratio < 0.52:
			feature = TerrainFeatureType.PLATEAU

		surface_y += _generate_world_surface_offset(feature, rng)

		var clamp_min := 0.17
		var clamp_max := 0.26
		if max_x <= 2500:
			clamp_min += 0.02

		if x < gv.left_beach_end + flat_beach_padding or x > gv.right_beach_start - flat_beach_padding:
			surface_y = clampf(surface_y, float(max_y) * clamp_min, beach_cap)
		elif surface_y < float(max_y) * clamp_min:
			surface_y = float(max_y) * clamp_min
			feature_timer = 0
		elif surface_y > float(max_y) * clamp_max:
			surface_y = float(max_y) * clamp_max
			feature_timer = 0

		while rng.next_int(0, 3) == 0:
			rock_y += float(rng.next_int(-2, 3))

		if rock_y < surface_y + float(max_y) * 0.06:
			rock_y += 1.0
		if rock_y > surface_y + float(max_y) * 0.35:
			rock_y -= 1.0

		history.record(surface_y)
		_fill_column(world, x, surface_y, rock_y)

		if x == gv.right_beach_start - flat_beach_padding:
			if surface_y > beach_cap:
				_retarget_surface_history(world, history, x, beach_cap)
			feature = TerrainFeatureType.PLATEAU
			feature_timer = max_x - x

	world.world_surface = int(surface_high + 25.0)
	world.rock_layer = int(rock_high)
	var rock_delta := int((float(world.rock_layer) - float(world.world_surface)) / 6.0) * 6
	world.rock_layer = int(float(world.world_surface) + float(rock_delta))

	var water_line := int((float(world.rock_layer) + float(max_y)) / 2.0) + rng.next_int(-100, 20)
	var lava_line := water_line + rng.next_int(50, 80)

	var min_gap := 20
	if rock_low < surface_high + float(min_gap):
		var mid := (rock_low + surface_high) / 2.0
		var gap := absf(rock_low - surface_high)
		if gap < float(min_gap):
			gap = float(min_gap)
		rock_low = mid + gap / 2.0
		surface_high = mid - gap / 2.0

	gv.rock_layer = rock_y
	gv.rock_layer_high = rock_high
	gv.rock_layer_low = rock_low
	gv.world_surface = surface_y
	gv.world_surface_high = surface_high
	gv.world_surface_low = surface_low
	gv.water_line = water_line
	gv.lava_line = lava_line


static func _fill_column(world: WorldState, x: int, surface_y: float, rock_y: float) -> void:
	for y in range(int(surface_y)):
		var tile := world.get_tile(x, y)
		tile.set_active(false)
	for y in range(int(surface_y), world.max_tiles_y):
		var tile := world.get_tile(x, y)
		tile.set_active(true)
		if float(y) < rock_y:
			tile.set_type(WorldConfig.TileType.DIRT)
		else:
			tile.set_type(WorldConfig.TileType.STONE)


static func _retarget_column(world: WorldState, x: int, surface_y: float) -> void:
	for y in range(int(surface_y)):
		var tile := world.get_tile(x, y)
		tile.set_active(false)
	for y in range(int(surface_y), world.max_tiles_y):
		var tile := world.get_tile(x, y)
		if tile.tile_type != WorldConfig.TileType.STONE or not tile.active:
			tile.set_active(true)
			tile.set_type(WorldConfig.TileType.DIRT)


static func _generate_world_surface_offset(feature: TerrainFeatureType, rng: GenRandom) -> float:
	var offset := 0.0
	match feature:
		TerrainFeatureType.PLATEAU:
			while rng.next_int(0, 7) == 0:
				offset += float(rng.next_int(-1, 2))
		TerrainFeatureType.HILL:
			while rng.next_int(0, 4) == 0:
				offset -= 1.0
			while rng.next_int(0, 10) == 0:
				offset += 1.0
		TerrainFeatureType.DALE:
			while rng.next_int(0, 4) == 0:
				offset += 1.0
			while rng.next_int(0, 10) == 0:
				offset -= 1.0
		TerrainFeatureType.MOUNTAIN:
			while rng.next_int(0, 2) == 0:
				offset -= 1.0
			while rng.next_int(0, 6) == 0:
				offset += 1.0
		TerrainFeatureType.VALLEY:
			while rng.next_int(0, 2) == 0:
				offset += 1.0
			while rng.next_int(0, 5) == 0:
				offset -= 1.0
	return offset


static func _retarget_surface_history(world: WorldState, history: SurfaceHistory, target_x: int, target_height: float) -> void:
	for i in range(history.length / 2):
		if history.get_height(history.length - 1) <= target_height:
			break
		for j in range(history.length - i * 2):
			var idx := history.length - j - 1
			var h := history.get_height(idx) - 1.0
			history.set_height(idx, h)
			if h <= target_height:
				break
	for k in range(history.length):
		var col_x := target_x - k
		if col_x >= 0:
			_retarget_column(world, col_x, history.get_height(history.length - k - 1))
