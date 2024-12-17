using System;
using Godot;
using System.Collections.Generic;
using RPGHra.scripts;
using Vector2 = Godot.Vector2;

public partial class DungeonGenerator : Node
{
	private readonly PackedScene _room = ResourceLoader.Load<PackedScene>("res://scenes/rooms/room.tscn");
	private readonly PackedScene _floor = ResourceLoader.Load<PackedScene>("res://scenes/rooms/floor.tscn");
	private readonly PackedScene _doors = ResourceLoader.Load<PackedScene>("res://scenes/rooms/doors.tscn");
	
	private readonly Dictionary<Vector2I, Room> _rooms = new ();

	[Export]
	private int _roomSize = 288;
	[Export]
	private int _maxRooms = 20;
	[Export]
	private int _roomChance = 30;
	
	public override void _Ready()
	{
		GenerateRooms();
		LoadDungeon();
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventKey eventKey)
			if (eventKey.Pressed && eventKey.Keycode == Key.F)
			{
				GenerateRooms();
				LoadDungeon();
			}
			else if (eventKey.Pressed && eventKey.Keycode == Key.E)
			{
				UnloadDungeon();
			}

	}

	void GenerateRooms()
	{
		GD.Print("Regenerating...");
		UnloadDungeon();
		// foreach (var room in _rooms.Values)
		// {
		// 	try
		// 	{
		// 		room.RoomRef.QueueFree();
		// 	}
		// 	catch (ObjectDisposedException e) { }
		// }
		// _rooms.Clear();
		
		// Create a starting room at 0,0
		_rooms[Vector2I.Zero] = new Room((Node2D)_room.Instantiate());

		int roomCount = 1;
		int tries = 0;
		
		while (roomCount < _maxRooms)
		{
			Dictionary<Vector2I, Room> newRooms = new ();
			
			// Go through every occupied room
			foreach (KeyValuePair<Vector2I, Room> room in _rooms)
			{
				tries++;
				GetNode<Label>("%Player/Camera2D/Label").Text = $"Room count: {roomCount}/{_maxRooms}\nTries: {tries}";
				
				// Pick a random direction and check all rooms until it creates a room
				Vector2I direction = new[] { Vector2I.Down, Vector2I.Left, Vector2I.Right, Vector2I.Up }[GD.RandRange(0, 3)];
				
				if (!_rooms.ContainsKey(room.Key + direction))
				{
					// Based on a defined chance, spawns a room and adds the connections
					if (GD.RandRange(0, 100) > _roomChance) continue;
					
					newRooms[room.Key + direction] = new Room((Node2D)_room.Instantiate());
					
					// if (room.Value.ConnectedRooms[direction] == null)
					// {
						ConnectRooms(room.Value, newRooms[room.Key + direction], direction);
					// }
					roomCount++;
				}
			}

			foreach (KeyValuePair<Vector2I, Room> newRoom in newRooms)
			{
				_rooms[newRoom.Key] = newRoom.Value;
			}

			newRooms.Clear();
		}
	}

	void ConnectRooms(Room room1, Room room2, Vector2I direction)
	{
		// Connects the current and previous room
		room1.ConnectedRooms[direction] = room2;
		room2.ConnectedRooms[-direction] = room1;

		room1.NumberOfConnections++;
		room2.NumberOfConnections++;
		
		// Remove connected walls
		RemoveWall(room1.RoomRef, direction);
		RemoveWall(room2.RoomRef, -direction);
	}
	
	void RemoveWall(Node2D room, Vector2I direction)
	{
		string wallNodeName = direction switch
		{
			var d when d == Vector2I.Left => "LeftWall",
			var d when d == Vector2I.Right => "RightWall",
			var d when d == Vector2I.Up => "TopWall",
			var d when d == Vector2I.Down => "BottomWall",
			_ => null
		};

		if (wallNodeName != null)
		{
			var wall = room.GetNodeOrNull<Node2D>("./Node2D/" + wallNodeName);
			if (wall != null)
			{
				wall.QueueFree();
			}
		}
	}
	private void LoadDungeon()
	{
	    Node mapNode = GetNode<Node>(".");
	    
	    // Clear previous map
	    foreach (Node child in mapNode.GetChildren())
	    {
		    child.QueueFree();
	    }

	    // Iterate through dungeon keys and generate map
       foreach (KeyValuePair<Vector2I, Room> room in _rooms)
       {
           // Create node sprite
           Node2D temp = room.Value.RoomRef;
           temp.SetName($"Room {room.Key.X},{room.Key.Y}");
           temp.SetMeta("coordinates", room.Key);
           
           foreach (Node child in temp.GetNode<Node2D>("./Node2D").GetChildren())
           {
	            if (child is Label label)
	            {
	                label.Text = $"{room.Key.X}, {room.Key.Y};";
	                foreach (KeyValuePair<Vector2I,Room> connectedRoom in room.Value.ConnectedRooms)
	                {
           		        if (connectedRoom.Value == null) continue;
           		        label.Text += $"\n{connectedRoom.Key.X}, {connectedRoom.Key.Y}";
	                }
	            }
           }
           
           temp.Position = room.Key * _roomSize;
           
           if (temp.GetParent() == null)
           {
            foreach (Node child in temp.GetChildren())
            {
	            if (child is Label label)
	            {
	    	        label.Text = $"{room.Key.X}, {room.Key.Y};";
	    	        foreach (KeyValuePair<Vector2I,Room> connectedRoom in room.Value.ConnectedRooms)
	    	        {
	    		        if (connectedRoom.Value == null) continue;
	    		        label.Text += $"\n{connectedRoom.Key.X}, {connectedRoom.Key.Y}";
	    	        }
	            }
            }
            
            mapNode.AddChild(temp);
           }

           
	        // Get connected rooms
	        var connectedRooms = room.Value.ConnectedRooms;

	        // Check for connected room at 1,0
	        if (connectedRooms[new Vector2I(1, 0)] != null)
	        {
				Node2D doors = _doors.Instantiate<Node2D>();
	            doors.Position = new Vector2(room.Key.X * _roomSize + _roomSize / 2.0f - 16, room.Key.Y * _roomSize);
	            mapNode.AddChild(doors);
	        }
	        
	        // Check for connected room at 0,1
	        if (connectedRooms[new Vector2I(0, 1)] != null)
	        {
				Node2D doors = _doors.Instantiate<Node2D>();
	            doors.RotationDegrees = 90;
	            doors.Position = new Vector2(room.Key.X * _roomSize, room.Key.Y * _roomSize + _roomSize / 2.0f - 16);
	            mapNode.AddChild(doors);
	        }
	    }
	}

	private void UnloadDungeon()
	{
		foreach (Room room in _rooms.Values)
		{
			room.RoomRef.QueueFree();
		}
		_rooms.Clear();

		GD.Print("Unloaded");
	}
}


