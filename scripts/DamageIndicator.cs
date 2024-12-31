using Godot;
using System;

public enum DamageType
{
	Heal,
	Damage
}

public partial class DamageIndicator : Node2D
{
	public float DamageAmount;
	public DamageType DamageType;
	
	private Vector2 _velocity = Vector2.Zero;
	
	public DamageIndicator()
	{
	}
	
	public DamageIndicator(float damageAmount, DamageType damageType)
	{
		DamageAmount = damageAmount;
		DamageType = damageType;
	}
	
	public override void _Ready()
	{
		Tween tween = CreateTween();

		tween.TweenProperty(this, "scale", Vector2.Zero, 1);

		_velocity = new Vector2((float)GD.RandRange(-30.0, 30.0), (float)GD.RandRange(60.0, 80.0));
	}

	public override void _Process(double delta)
	{
		Position += _velocity * (float)delta;
	}
}
