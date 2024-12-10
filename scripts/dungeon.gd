extends Node2D

const ROOM_ALL: PackedScene = preload("res://scenes/rooms/room_4_way.tscn")
const ROOM_NORTH: PackedScene = preload("res://scenes/rooms/room_north.tscn")
const ROOM_EAST: PackedScene = preload("res://scenes/rooms/room_east.tscn")
const ROOM_SOUTH: PackedScene = preload("res://scenes/rooms/room_south.tscn")
const ROOM_WEST: PackedScene = preload("res://scenes/rooms/room_west.tscn")

const ROOM_SIZE: int = 288

const ROOMS: Array[PackedScene] = [ROOM_ALL, ROOM_NORTH]

var available_door_connections: Array[Vector3i] = []
var used_door_connections: Array[Vector2i] = []


func _ready() -> void:
	var starting_room: Node = generate_room(0, 0, ROOM_ALL)
	add_child(starting_room)
	
	for connection in available_door_connections.duplicate():
		match connection.z:
			0:
				add_child(generate_room(connection.x, connection.y, ROOM_SOUTH))
			1:
				add_child(generate_room(connection.x, connection.y, ROOM_WEST))
			2:
				add_child(generate_room(connection.x, connection.y, ROOM_NORTH))
			3:
				add_child(generate_room(connection.x, connection.y, ROOM_EAST))
		
		
			
			
			

func generate_room(x: int, y: int, room_type: PackedScene) -> Node:
	print("Generating room at %s, %s" % [x, y])
	var new_room: Node = room_type.instantiate()
	new_room.position.x = ROOM_SIZE * x
	new_room.position.y = ROOM_SIZE * y
	new_room.set_meta("coordinates", Vector2i(x, y))
	var door_connections: Vector4i = new_room.get_meta("door_connections") # North, East, South, West
	
	if Vector2i(x, y) in available_door_connections:
		available_door_connections.erase(Vector2i(x, y))
		used_door_connections.append(Vector2i(x, y))
	
	for i in range(4):
		if door_connections[i] == 1:
			available_door_connections.append(Vector3i(x + int(i == 1) - int(i == 3), y + int(i == 2) - int(i == 0), i))
	
	for child in new_room.get_children():
					if child is Label:
							child.text = "%s, %s" % [x, y]
						
	return new_room
