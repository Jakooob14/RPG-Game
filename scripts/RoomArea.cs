using Godot;
using System;

public partial class RoomArea : Area2D
{
	private Sprite2D _overlay;
	private AnimationPlayer _animationPlayer;

	public override void _Ready()
	{
		_overlay = GetNode<Sprite2D>("../Overlay");
		_animationPlayer = GetNode<AnimationPlayer>("../AnimationPlayer");
	}
	
	private void OnBodyEntered(Node2D body)
	{
		if (body.Name != "Player" || _overlay == null || _animationPlayer == null) return;
		// if (!GetParent().GetParent().HasMeta("coordinates")) return;
		// Vector2I coordinates = (Vector2I)GetParent().GetParent().GetMeta("coordinates");
		// _overlay.Visible = false;
		_animationPlayer.Play("room_opacity");

		// Camera2D mainCamera = GetNode<Camera2D>("/root/Dungeon/%MainCamera");
		// if (mainCamera == null) return;
		// Vector2 roomSize = GlobalVariables.RoomSize;
		// mainCamera.Position = new Vector2(coordinates.X * roomSize.X, coordinates.Y * roomSize.Y);
	}
	
	private void OnBodyExited(Node2D body)
	{
		if (body.Name != "Player" || _overlay == null || _animationPlayer == null) return;
		
		_animationPlayer.PlayBackwards("room_opacity");
		// _overlay.Visible = true;
	}
}
