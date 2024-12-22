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
		_animationPlayer.Play("room_opacity");
	}
	
	private void OnBodyExited(Node2D body)
	{
		if (body.Name != "Player" || _overlay == null || _animationPlayer == null) return;
		
		_animationPlayer.PlayBackwards("room_opacity");
		// _overlay.Visible = true;
	}
}
