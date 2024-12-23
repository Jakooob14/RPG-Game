using System;
using Godot;
using System.Collections.Generic;
using System.Linq;
using Vector2 = Godot.Vector2;

public partial class DungeonGenerator : Node
{
    private readonly PackedScene _roomScene = ResourceLoader.Load<PackedScene>("res://scenes/rooms/room.tscn");
    private readonly PackedScene _floorScene = ResourceLoader.Load<PackedScene>("res://scenes/rooms/floor.tscn");
    private readonly PackedScene _doorScene = ResourceLoader.Load<PackedScene>("res://scenes/rooms/doors.tscn");

    private readonly HashSet<Room> _rooms = new();
    private Vector2 _roomSize = GlobalVariables.RoomSize;

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
        if (@event is InputEventKey eventKey && eventKey.Pressed)
        {
            if (eventKey.Keycode == Key.F)
            {
                GenerateRooms();
                LoadDungeon();
            }
            else if (eventKey.Keycode == Key.E)
            {
                UnloadDungeon();
            }
        }
    }

    private void GenerateRooms()
    {
        GD.Print("Regenerating...");
        UnloadDungeon();

        // Create the starting room at (0,0)
        Room startingRoom = _roomScene.Instantiate<Room>();
        startingRoom.RoomPosition = Vector2I.Zero;
        _rooms.Add(startingRoom);

        int roomCount = 1;
        int tries = 0;

        while (roomCount < _maxRooms)
        {
            HashSet<Room> newRooms = new HashSet<Room>();

            foreach (Room room in _rooms)
            {
                tries++;
                GetNode<Label>("%Player/%MainCamera/Label").Text = $"Room count: {roomCount}/{_maxRooms}\nTries: {tries}";

                Vector2I direction = new[] { Vector2I.Down, Vector2I.Left, Vector2I.Right, Vector2I.Up }[GD.RandRange(0, 3)];
                Vector2I newPosition = room.RoomPosition + direction;

                if (_rooms.All(room1 => room1.RoomPosition != newPosition) && GD.RandRange(0, 100) <= _roomChance)
                {
                    Room newRoom = _roomScene.Instantiate<Room>();
                    newRoom.RoomPosition = newPosition;

                    newRooms.Add(newRoom);
                    ConnectRooms(room, newRoom, direction);
                    roomCount++;
                }
            }

            foreach (Room newRoom in newRooms)
            {
                _rooms.Add(newRoom);
            }
        }
    }

    private void ConnectRooms(Room room1, Room room2, Vector2I direction)
    {
        // Connects the current and previous room
        room1.ConnectedRooms[direction] = room2;
        room2.ConnectedRooms[-direction] = room1;

        room1.NumberOfConnections++;
        room2.NumberOfConnections++;
		
        // Remove connected walls
        RemoveWall(room1, direction);
        RemoveWall(room2, -direction);
    }

    private void RemoveWall(Room room, Vector2I direction)
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
            Node2D wall = room?.GetNodeOrNull<Node2D>("./Node2D/" + wallNodeName);
            wall?.QueueFree();
        }
    }

    private void LoadDungeon()
    {
        foreach (Node child in GetChildren())
        {
            child.QueueFree();
        }

        var roomNode = new Node { Name = "Rooms" };
        var doorNode = new Node { Name = "Doors" };
        var floorNode = new Node { Name = "Floors" };

        AddChild(roomNode);
        AddChild(doorNode);
        AddChild(floorNode);

        foreach (Room room in _rooms)
        {
            room.Name = $"Room {room.RoomPosition.X},{room.RoomPosition.Y}";

            if (room.GetParent() == null)
            {
                roomNode.AddChild(room);
                GenerateRoomContent(room);
            }

            foreach (KeyValuePair<Vector2I,Room> connectedRoom in room.ConnectedRooms)
            {
                if (connectedRoom.Value != null)
                {
                    CreateDoor(doorNode, room.RoomPosition, connectedRoom.Key);
                }
            }
        }
    }

    private void CreateDoor(Node parent, Vector2I roomPosition, Vector2I direction)
    {
        var door = _doorScene.Instantiate<Node2D>();
        door.Position = roomPosition * _roomSize + direction * (_roomSize / 2);
        door.RotationDegrees = direction == Vector2I.Up || direction == Vector2I.Down ? 90 : 0;
        parent.AddChild(door);
    }
    
    private void UnloadDungeon()
    {
        foreach (Room room in _rooms)
        {
            room.QueueFree();
        }
        _rooms.Clear();
    
        GD.Print("Unloaded");
    }
    
    private void GenerateRoomContent(Room room)
    {
        Enemy enemy = ResourceLoader.Load<PackedScene>("res://scenes/enemy.tscn").Instantiate<Enemy>();
        enemy.Position = room.RoomPosition * _roomSize;
        enemy.AssignedRoom = room;
        room.AssignedEntities.Add(enemy);
        AddChild(enemy);
    }
}
