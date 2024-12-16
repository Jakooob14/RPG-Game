using Godot;
using System;
using System.Collections.Generic;
using System.Numerics;
using RPGHra.scripts;
using Vector2 = Godot.Vector2;

public partial class DungeonGenerator : Node
{
	private readonly PackedScene _roomAll = ResourceLoader.Load<PackedScene>("res://scenes/rooms/room_4_way.tscn");
	private readonly PackedScene _roomNorth = ResourceLoader.Load<PackedScene>("res://scenes/rooms/room_north.tscn");
	private readonly PackedScene _roomEast = ResourceLoader.Load<PackedScene>("res://scenes/rooms/room_east.tscn");
	private readonly PackedScene _roomSouth = ResourceLoader.Load<PackedScene>("res://scenes/rooms/room_south.tscn");
	private readonly PackedScene _roomWest = ResourceLoader.Load<PackedScene>("res://scenes/rooms/room_west.tscn");
	private readonly PackedScene _floor = ResourceLoader.Load<PackedScene>("res://scenes/rooms/floor.tscn");
	
	private List<Room> _rooms = new ();
	Dictionary<Vector2I, Room> rooms = new ();

	[Export]
	private int _roomSize = 288;
	[Export]
	private int _maxRooms = 20;
	
	public override void _Ready()
	{
		
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventKey eventKey)
			if (eventKey.Pressed && eventKey.Keycode == Key.F)
			{
				GenerateRooms();
				LoadMap();
				// foreach (Room room in _rooms)
				// {
				// 	room.RoomRef.QueueFree();
				// }
				// _rooms.Clear();
				//
				// GenerateRooms();
			}

	}

	void GenerateRooms()
	{
		rooms.Clear();
		
		// Create a starting room at 0,0
		Node2D startingRoom = (Node2D)_roomAll.Instantiate();
		rooms[Vector2I.Zero] = new Room(startingRoom);

		int roomCount = 1;
		int tries = 0;
		
		while (roomCount < _maxRooms)
		{
			Dictionary<Vector2I, Room> newRooms = new ();
			
			// Go through every occupied room
			foreach (KeyValuePair<Vector2I, Room> room in rooms)
			{
				tries++;
				GetNode<Label>("%Player/Camera2D/Label").Text = $"Room count: {roomCount}/{_maxRooms}\nTries: {tries}";
				
				// Pick a random direction and check all rooms until it creates a room
				Vector2I direction = new[] { Vector2I.Down, Vector2I.Left, Vector2I.Right, Vector2I.Up }[GD.RandRange(0, 3)];
				
				if (!rooms.ContainsKey(room.Key + direction))
				{
					// 30% chance to spawn a room
					if (GD.RandRange(0, 100) > 30) continue;
					
					Room newRoom = new Room((Node2D)_roomAll.Instantiate());
					newRooms[room.Key + direction] = newRoom;
					roomCount++;
					
					_rooms.Add(newRoom);
					
					ConnectRooms(room.Value, newRoom, direction);

					// if (direction == new Vector2I(1, 0))
					// {
					// 	ConnectRooms(room.Value, newRoom, new Vector2I(1, 0));
					// } else if ()
					
					// switch (direction)
					// {
					// 	case var v when v.Equals(Vector2I.Up):
					// 		GD.Print(direction);
					// 		ConnectRooms(room.Value, newRoom, direction);
					// 		break;
					// 		
					// }
				}
			}

			foreach (KeyValuePair<Vector2I, Room> newRoom in newRooms)
			{
				rooms[newRoom.Key] = newRoom.Value;
			}
		}

		// foreach (KeyValuePair<Vector2I, Room> room in rooms)
		// {
		// 	Node2D newRoom = room.Value.RoomRef;
		// 	newRoom.Position = room.Key * _roomSize;
		// 	if (newRoom.GetParent() == null)
		// 	{
		// 		foreach (Node child in newRoom.GetChildren())
		// 		{
		// 			if (child is Label)
		// 			{
		// 				((Label)child).Text = $"{room.Key.X}, {room.Key.Y};";
		// 				foreach (KeyValuePair<Vector2I,Room> connectedRoom in room.Value.ConnectedRooms)
		// 				{
		// 					if (connectedRoom.Value == null) continue;
		// 					((Label)child).Text += $"\n{connectedRoom.Key.X}, {connectedRoom.Key.Y}";
		// 				}
		// 			}
		// 		}
		// 		AddChild(newRoom);
		// 	}
		// }
	}

	void ConnectRooms(Room room1, Room room2, Vector2I direction)
	{
		room1.ConnectedRooms[direction] = room2;
		room2.ConnectedRooms[-direction] = room1;

		room1.NumberOfConnections++;
		room2.NumberOfConnections++;
	}

	private void LoadMap()
	{
		Node mapNode = GetNode<Node>(".");
		Texture2D nodeSprite = GD.Load<Texture2D>("res://assets/map_nodes1.png");
		Texture2D branchSprite = GD.Load<Texture2D>("res://assets/map_nodes3.png");
		
		// Clear previous map
		for (int i = 0; i < mapNode.GetChildCount(); i++)
		{
			mapNode.GetChild(i).QueueFree();
		}

		// Iterate through dungeon keys and generate map
		foreach (KeyValuePair<Vector2I, Room> room in rooms)
		{
			// Create node sprite
			var temp = new Sprite2D();
			temp.Texture = nodeSprite;
			temp.ZIndex = 1;
			temp.Position = room.Key * 10;
			mapNode.AddChild(temp);

			// Get connected rooms
			var connectedRooms = room.Value.ConnectedRooms;
			
			// Check for connected room at (1, 0)
			Room value;
			connectedRooms.TryGetValue(new Vector2I(1, 0), out value);
			if (value != null)
			{
				temp = new Sprite2D();
				temp.Texture = branchSprite;
				mapNode.AddChild(temp);
				temp.ZIndex = 0;
				temp.Position = new Vector2(room.Key.X * 10 + 5, room.Key.Y * 10 + 0.5f);
			}
			
			// Check for connected room at (0, 1)
			Room value2;
			connectedRooms.TryGetValue(new Vector2I(0, 1), out value2);
			if (value2 != null)
			{
				temp = new Sprite2D();
				temp.Texture = branchSprite;
				temp.ZIndex = 0;
				temp.RotationDegrees = 90;
				temp.Position = new Vector2(room.Key.X * 10 -0.5f, room.Key.Y * 10 + 5);
				mapNode.AddChild(temp);
			}
		}
	}
}


