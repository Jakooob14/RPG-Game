using Godot;
using System;

public partial class Player : LivingEntity
{
	
	[Signal]
	public delegate void UpdateHealthEventHandler(float newHealth);
	
	public override void Damage(float damageAmount)
	{
		base.Damage(damageAmount);
		
		if (Health <= 0)
		{
			EmitSignal(SignalName.UpdateHealth, 0);
			return;
		}
		
		EmitSignal(SignalName.UpdateHealth, Health);
	
	}

	public override void _Ready()
	{
		base._Ready();
		
		EmitSignal(SignalName.UpdateHealth, MaxHealth);
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		
		Vector2 direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		if (direction != Vector2.Zero)
		{
			velocity = direction * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Y = Mathf.MoveToward(Velocity.Y, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
		
		GetNode<Node2D>("Look").LookAt(GetGlobalMousePosition());
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("attack"))
		{
			foreach (Node2D overlappingBody in GetNode<Area2D>("Look/PlayerAttackArea").GetOverlappingBodies())
			{
				if (overlappingBody is LivingEntity livingEntity && !livingEntity.HasMethod("Player"))
				{
					Attack(livingEntity, 5);
				}
			}
		}
	}
}
