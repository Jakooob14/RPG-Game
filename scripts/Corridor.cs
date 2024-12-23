using Godot;
using System;

public partial class Corridor : Node2D
{
	public bool IsOpen { get; set; } = false;

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (GlobalVariables.EntitiesInRoom && IsOpen)
		{
			IsOpen = false;
			GetNode<TileMapLayer>("Node2D/DoorLayer").CollisionEnabled = true;
		}
		else if (!GlobalVariables.EntitiesInRoom && !IsOpen)
		{
			IsOpen = true;
			GetNode<TileMapLayer>("Node2D/DoorLayer").CollisionEnabled = false;
		}
	}

	public void ToggleDoor(bool open)
	{
		GetNode<TileMapLayer>("Node2D/DoorLayer").CollisionEnabled = open;

		if (open)
		{
			
		}
	}
}
