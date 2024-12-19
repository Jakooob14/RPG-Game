using Godot;
using System;

public partial class RoomArea : Area2D
{
	private void OnBodyEntered(Node2D body)
	{
		if (body.Name != "Player") return;
		if (!GetParent().GetParent().HasMeta("coordinates")) return;
		Vector2I coordinates = (Vector2I)GetParent().GetParent().GetMeta("coordinates");
		GD.Print($"Hey Player! You are @ {coordinates.X}, {coordinates.Y}");
	}
}
