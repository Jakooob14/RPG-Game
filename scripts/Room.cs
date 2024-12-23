using System.Collections.Generic;
using Godot;

public partial class Room : Node2D
{
    private Vector2I _roomPosition;

    public Vector2I RoomPosition
    {
        get => _roomPosition;
        set
        {
            _roomPosition = value;
            Position = value * GlobalVariables.RoomSize;
        }
    }

    public HashSet<LivingEntity> AssignedEntities { get; set; } = new HashSet<LivingEntity>();
    
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

    public Room()
    {
    }

    public override void _Ready()
    {
        base._Ready();
        
        // Debug
        Label debugLabel = GetNode<Label>("Node2D/Debug");
        if (debugLabel != null)
        {
            debugLabel.Text = $"{RoomPosition.X},{RoomPosition.Y}";
            foreach (KeyValuePair<Vector2I,Room> connectedRoom in ConnectedRooms)
            {
                if (connectedRoom.Value == null) continue;

                debugLabel.Text += $"\n  > {connectedRoom.Value.RoomPosition.X},{connectedRoom.Value.RoomPosition.Y}";
            }
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        // bool livingEntityInRoom = false;
        // bool playerInRoom = false;
        // foreach (Node2D overlappingBody in RoomRef.GetNode<Area2D>("Node2D/RoomConstraints").GetOverlappingBodies())
        // {
        //     GD.Print("asd");
        //     if (overlappingBody is not Player)
        //     {
        //         GD.Print("notplayer");
        //         if (overlappingBody is LivingEntity) livingEntityInRoom = true;
        //     }
        //     else
        //     {
        //         GD.Print("player");
        //         playerInRoom = true;
        //     }
        // }
        //
        // if (livingEntityInRoom && playerInRoom)
        // {
        //     GD.Print("Not killed all " + GD.RandRange(0, 10000000));
        // }
    }
}