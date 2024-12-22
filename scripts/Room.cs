using System.Collections.Generic;
using Godot;

public partial class Room : Node2D
{
    public Node2D RoomRef { get; set; }
    public Vector2I RoomPosition { get; set; }
    
    public readonly Dictionary<Vector2I, float> DirectionRotations = new()
    {
        { new Vector2I(1, 0), 180f },
        { new Vector2I(-1, 0), 0f },
        { new Vector2I(0, 1), 270f },
        { new Vector2I(0, -1), 90f }
    };
    
    public readonly Dictionary<Vector2I, Room> ConnectedRooms = new ()
    {
        { new Vector2I(1, 0), null },
        { new Vector2I(-1, 0), null },
        { new Vector2I(0, 1), null },
        { new Vector2I(0, -1), null }
    };

    public int NumberOfConnections = 0;

    public Room(Node2D roomRef)
    {
        RoomRef = roomRef;
    }

    public Room()
    {
    }
}