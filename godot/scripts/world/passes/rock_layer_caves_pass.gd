class_name RockLayerCavesPass
extends RefCounted
## 移植自 Terraria RockLayerCaves Pass
## 在岩石层用 TileRunner 反复醉汉游走挖空洞穴

const CAVE_DENSITY := 0.00012

static func apply(world: WorldState, rng: GenRandom) -> void:
	var gv := world.gen_vars
	var rock_start := int(gv.rock_layer_low)
	var count := int(float(world.max_tiles_x * world.max_tiles_y) * CAVE_DENSITY)

	for _i in range(count):
		var x := rng.next_int(0, world.max_tiles_x)
		var y := rng.next_int(rock_start, world.max_tiles_y)
		var strength := float(rng.next_int(5, 13))
		var steps := rng.next_int(30, 90)
		TileRunner.run(world, rng, float(x), float(y), strength, steps, WorldConfig.TileType.AIR)

	# 地表附近少量竖井（对齐 Tunnels pass 简化版）
	var tunnel_count := int(float(world.max_tiles_x) * 0.003)
	for _i in range(tunnel_count):
		var x := rng.next_int(0, world.max_tiles_x)
		var y := rng.next_int(int(gv.world_surface_high), int(gv.rock_layer_high))
		TileRunner.run(world, rng, float(x), float(y), float(rng.next_int(3, 7)), rng.next_int(15, 40), WorldConfig.TileType.AIR)
