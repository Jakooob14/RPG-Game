using Godot;
using System;

public partial class GlobalVariables : Node
{
    public static Vector2 DoorSize = new Vector2(8 * 32, 2 * 32);
    public static Vector2 RoomSize = new Vector2(53 * 32 + DoorSize.X - 32, 37 * 32 + DoorSize.X - 32);

    public static bool EntitiesInRoom = false;

    public static Player Player;
    public static PlayerUi PlayerUi;
}
