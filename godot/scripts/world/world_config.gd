class_name WorldConfig
extends RefCounted
## Terraria 世界尺寸与方块 ID 常量（对齐反编译源码 TileID）

enum TileType {
	AIR = -1,
	DIRT = 0,
	STONE = 1,
}

enum WorldSize {
	SMALL,   # 4200 x 1200
	MEDIUM,  # 6400 x 1800
	LARGE,   # 8400 x 2400
	DEV,     # 840 x 240 — 快速迭代
}

const SIZE_PRESETS := {
	WorldSize.SMALL: Vector2i(4200, 1200),
	WorldSize.MEDIUM: Vector2i(6400, 1800),
	WorldSize.LARGE: Vector2i(8400, 2400),
	WorldSize.DEV: Vector2i(840, 240),
}

const TILE_PIXEL_SIZE := 16

static func get_dimensions(size: WorldSize) -> Vector2i:
	return SIZE_PRESETS[size]
