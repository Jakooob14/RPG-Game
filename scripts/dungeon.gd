extends Node2D

@onready var player: CharacterBody2D = $Player
const ROOM = preload("res://scenes/room_4_way.tscn")
const ROOM_SIZE = 288

func _ready() -> void:

	for i in range(6):
		for j in range(6):
			var new_room = ROOM.instantiate()
			new_room.position.x = ROOM_SIZE * i
			new_room.position.y = ROOM_SIZE * j
			add_child(new_room)
			
	player.position = Vector2(6 / 2 * ROOM_SIZE - ROOM_SIZE / 2 + 16, 6 / 2 * ROOM_SIZE - ROOM_SIZE / 2 + 16)
