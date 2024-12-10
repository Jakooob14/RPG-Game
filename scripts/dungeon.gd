extends Node2D

const ROOM_ALL: PackedScene = preload("res://scenes/rooms/room_4_way.tscn")
const ROOM_NORTH: PackedScene = preload("res://scenes/rooms/room_north.tscn")
const ROOM_SIZE: int = 288

const ROOMS: Array[PackedScene] = [ROOM_ALL, ROOM_NORTH]

var available_door_connections: Array[Vector2i] = []
var used_door_connections: Array[Vector2i] = []


func _ready() -> void:
	
	var starting_room: Node = generate_room(0, 0, ROOM_ALL)
	add_child(starting_room)
	
#	for x in range(-3, 3):
#		for y in range(-3, 3):
	var room: PackedScene = ROOMS.pick_random()
	var new_room: Node = generate_room(0, 0, room)
			
	for connection in available_door_connections.size():
		print("Available door connections: %s" % available_door_connections[connection])
		
			
			
			

func generate_room(x: int, y: int, room_type: PackedScene) -> Node:
	var new_room: Node = room_type.instantiate()
	new_room.position.x = ROOM_SIZE * x
	new_room.position.y = ROOM_SIZE * y
	new_room.set_meta("coordinates", Vector2i(x, y))
	var door_connections: Vector4i = new_room.get_meta("door_connections") # North, East, South, West
	for i in range(4):
		if door_connections[i] == 1:
			available_door_connections.append(Vector2i(x, y))
	
	for child in new_room.get_children():
					if child is Label:
							child.text = "%s, %s" % [x, y]
						
	return new_room