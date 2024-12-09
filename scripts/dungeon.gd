extends Node2D

const ROOM_ALL: PackedScene = preload("res://scenes/rooms/room_4_way.tscn")
const ROOM_NORTH: PackedScene = preload("res://scenes/rooms/room_north.tscn")
const ROOM_SIZE: int = 288

const ROOMS: Array[PackedScene] = [ROOM_ALL, ROOM_NORTH]

func _ready() -> void:
	for x in range(-3, 3):
		for y in range(-3, 3):
			var room: PackedScene = ROOMS.pick_random();
			
			match room:
				ROOM_ALL:
					generate_room(x, y, room)
				ROOM_NORTH:
					generate_room(x, y, room)
			

func generate_room(x: int, y: int, room_type: PackedScene) -> void:
	var new_room: Node = room_type.instantiate()
	new_room.position.x = ROOM_SIZE * x
	new_room.position.y = ROOM_SIZE * y
	new_room.set_meta("coordinates", Vector2i(x, y))
	
	for child in new_room.get_children():
					if child is Label:
							child.text = "%s, %s" % [x, y]
						
	add_child(new_room)