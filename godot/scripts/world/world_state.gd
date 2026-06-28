class_name WorldState
extends RefCounted
## 对应 Terraria Main.tile[,] 二维世界表

var max_tiles_x: int
var max_tiles_y: int
var tiles: Array = []

var world_surface: int = 0
var rock_layer: int = 0

var gen_vars: GenVars = GenVars.new()


func _init(width: int, height: int) -> void:
	max_tiles_x = width
	max_tiles_y = height
	tiles.resize(width)
	for x in range(width):
		var column: Array = []
		column.resize(height)
		for y in range(height):
			column[y] = WorldTile.new()
		tiles[x] = column


func get_tile(x: int, y: int) -> WorldTile:
	if x < 0 or x >= max_tiles_x or y < 0 or y >= max_tiles_y:
		return null
	return tiles[x][y]


func in_bounds(x: int, y: int) -> bool:
	return x >= 0 and x < max_tiles_x and y >= 0 and y < max_tiles_y
