class_name TileRunner
extends RefCounted
## 移植自 Terraria WorldGen.TileRunner（WorldGen.cs:77042）
## 衰减半径醉汉游走 — 洞穴/矿脉的统一画笔

static func run(
	world: WorldState,
	rng: GenRandom,
	start_x: float,
	start_y: float,
	strength: float,
	steps: int,
	tile_type: int,
) -> void:
	var pos := Vector2(start_x, start_y)
	var vel := Vector2(
		rng.next_int(-1, 2),
		rng.next_int(-1, 2),
	)
	var strength_now := strength
	var steps_left := steps

	while strength_now > 0.0 and steps_left > 0:
		var radius := strength * (float(steps_left) / float(steps))
		_paint_circle(world, rng, pos, strength, radius, tile_type)

		pos += vel
		vel.x += rng.next_int(-1, 2)
		vel.y += rng.next_int(-1, 2)

		strength_now = radius
		steps_left -= 1


static func _paint_circle(
	world: WorldState,
	rng: GenRandom,
	center: Vector2,
	strength: float,
	radius: float,
	tile_type: int,
) -> void:
	var half := int(ceilf(radius)) + 1
	var cx := int(center.x)
	var cy := int(center.y)

	for dx in range(-half, half + 1):
		for dy in range(-half, half + 1):
			var tx := cx + dx
			var ty := cy + dy
			if not world.in_bounds(tx, ty):
				continue

			var jitter := 1.0 + float(rng.next_int(-10, 11)) * 0.015
			var threshold := strength * 0.5 * jitter
			if absf(dx) + absf(dy) < threshold:
				var tile := world.get_tile(tx, ty)
				if tile_type < 0:
					tile.set_active(false)
				else:
					tile.set_active(true)
					tile.set_type(tile_type)
