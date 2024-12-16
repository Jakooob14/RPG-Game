using System.Collections.Generic;
using Godot;

namespace RPGHra.scripts;

class Room
{
    public Node2D RoomRef { get; set; }
	
    public Dictionary<Vector2I, Room> ConnectedRooms = new ()
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
}