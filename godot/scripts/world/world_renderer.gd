class_name WorldRenderer
extends TileMapLayer
## 将 WorldState 渲染到 TileMapLayer

const SOURCE_ID := 0
const ATLAS_DIRT := Vector2i(0, 0)
const ATLAS_STONE := Vector2i(1, 0)

var _world: WorldState


func _ready() -> void:
	tile_set = _build_tileset()


func render(world: WorldState) -> void:
	_world = world
	clear()
	for x in range(world.max_tiles_x):
		for y in range(world.max_tiles_y):
			var tile := world.get_tile(x, y)
			if not tile.active:
				continue
			var atlas := ATLAS_DIRT if tile.tile_type == WorldConfig.TileType.DIRT else ATLAS_STONE
			set_cell(Vector2i(x, y), SOURCE_ID, atlas)


func get_world_size_pixels() -> Vector2:
	if _world == null:
		return Vector2.ZERO
	return Vector2(
		_world.max_tiles_x * WorldConfig.TILE_PIXEL_SIZE,
		_world.max_tiles_y * WorldConfig.TILE_PIXEL_SIZE,
	)


func _build_tileset() -> TileSet:
	var ts := TileSet.new()
	ts.tile_size = Vector2i(WorldConfig.TILE_PIXEL_SIZE, WorldConfig.TILE_PIXEL_SIZE)

	var atlas := TileSetAtlasSource.new()
	var img := Image.create(32, 16, false, Image.FORMAT_RGBA8)
	img.fill_rect(Rect2i(0, 0, 16, 16), Color(0.55, 0.36, 0.18))
	img.fill_rect(Rect2i(16, 0, 16, 16), Color(0.45, 0.45, 0.48))
	atlas.texture = ImageTexture.create_from_image(img)
	atlas.create_tile(ATLAS_DIRT)
	atlas.create_tile(ATLAS_STONE)
	ts.add_source(atlas, SOURCE_ID)
	return ts
