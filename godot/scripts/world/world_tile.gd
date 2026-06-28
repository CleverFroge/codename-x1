class_name WorldTile
extends RefCounted
## 对应 Terraria.Tile 的精简版：active + type

var active: bool = false
var tile_type: int = WorldConfig.TileType.DIRT


func clear() -> void:
	active = false
	tile_type = WorldConfig.TileType.DIRT


func set_active(value: bool) -> void:
	active = value


func set_type(value: int) -> void:
	tile_type = value
