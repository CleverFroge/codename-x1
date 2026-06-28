class_name GenRandom
extends RefCounted
## 对齐 Terraria UnifiedRandom / C# Random.Next(min, max) 语义

var _rng: RandomNumberGenerator = RandomNumberGenerator.new()


func seed_from(value: int) -> void:
	_rng.seed = value


func next_int(min_value: int, max_value: int) -> int:
	# C# Random.Next(min, max) → [min, max)
	return _rng.randi_range(min_value, max_value - 1)


func next_float() -> float:
	return _rng.randf()
