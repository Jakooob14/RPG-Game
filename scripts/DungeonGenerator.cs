using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

	}

	void GenerateRooms()
	{
		GD.Print("Regenerating...");
		_rooms.Clear();
		
		// Create a starting room at 0,0
		Node2D startingRoom = (Node2D)_roomAll.Instantiate();
		_rooms[Vector2I.Zero] = new Room(startingRoom);

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
					
					Room newRoom = new Room((Node2D)_roomAll.Instantiate());
					newRooms[room.Key + direction] = newRoom;
					roomCount++;

					if (room.Value.ConnectedRooms[direction] == null)
					{
						ConnectRooms(room.Value, newRoom, direction);
					}
				}
			}

			foreach (KeyValuePair<Vector2I, Room> newRoom in newRooms)
			{
				_rooms[newRoom.Key] = newRoom.Value;
			}
		}
	}

	void ConnectRooms(Room room1, Room room2, Vector2I direction)
	{
		// Connects the current and previous room
		room1.ConnectedRooms[direction] = room2;
		room2.ConnectedRooms[-direction] = room1;

		room1.NumberOfConnections++;
		room2.NumberOfConnections++;
		
		// RemoveWall(room1.RoomRef, direction);
		// RemoveWall(room2.RoomRef, -direction);
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
				wall.QueueFree(); // Remove the wall
			}
		}
	}
	private void LoadDungeon()
	{
	    Node mapNode = GetNode<Node>(".");
	    Texture2D branchSprite = GD.Load<Texture2D>("res://assets/map_nodes3.png");

	    // Clear previous map
	    for (int i = 0; i < mapNode.GetChildCount(); i++)
	    {
	        mapNode.GetChild(i).QueueFree();
	    }
	    
	    // Iterate through dungeon keys and generate map
       foreach (KeyValuePair<Vector2I, Room> room in _rooms)
       {
           // Create node sprite
           Node2D temp = room.Value.RoomRef;
           temp.Name = $"Room {temp.Position.X},{temp.Position.Y}";
           
           // switch (room.Value.NumberOfConnections)
           // {
           //  case 1:
	          //   temp = _roomWest.Instantiate<Node2D>();
           //
	          //   Vector2I[] directions = { Vector2I.Down, Vector2I.Left, Vector2I.Right, Vector2I.Up };
           //
	          //   foreach (Vector2I direction in directions)
	          //   {
	    	     //    if (room.Value.ConnectedRooms[direction] != null)
	    	     //    {
	    		    //     temp.RotationDegrees = room.Value.DirectionRotations[direction];
	    	     //    }
	          //   }
	          //   
	          //   break;
           // }
           
           
           
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
           
           Vector2I[] directions = { Vector2I.Down, Vector2I.Left, Vector2I.Right, Vector2I.Up };

           foreach (Vector2I direction in directions)
           {
	           if (room.Value.ConnectedRooms[direction] != null)
	           {
		           RemoveWall(temp, direction);
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

	        // Check for connected room at (1, 0)
	        Room value;
	        connectedRooms.TryGetValue(new Vector2I(1, 0), out value);
	        if (value != null)
	        {
	            var temp2 = new Sprite2D();
	            temp2.Texture = branchSprite;
	            temp2.ZIndex = 5;
	            temp2.Scale = new Vector2(10, 10);
	            temp2.Position = new Vector2(room.Key.X * _roomSize + _roomSize / 2.0f, room.Key.Y * _roomSize);
	            mapNode.AddChild(temp2);
	        }

	        // Check for connected room at (0, 1)
	        Room value2;
	        connectedRooms.TryGetValue(new Vector2I(0, 1), out value2);
	        if (value2 != null)
	        {
	            var temp2 = new Sprite2D();
	            temp2.Texture = branchSprite;
	            temp2.ZIndex = 5;
	            temp2.RotationDegrees = 90;
	            temp2.Scale = new Vector2(10, 10);
	            temp2.Position = new Vector2(room.Key.X * _roomSize, room.Key.Y * _roomSize + _roomSize / 2.0f);
	            mapNode.AddChild(temp2);
	        }
	    }
	}
}


