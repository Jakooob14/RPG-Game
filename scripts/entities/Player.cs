using Godot;
using System;

public partial class Player : LivingEntity
{
	[Signal]
	public delegate void UpdateHealthEventHandler(float newHealth);
	
	public override void Damage(float damageAmount, LivingEntity inducer)
	{
		base.Damage(damageAmount, inducer);
		
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
		base._PhysicsProcess(delta);
		
		if (Math.Abs(CurrentKnockback.X) > 0.0f || Math.Abs(CurrentKnockback.Y) > 0.0f)
		{
			Velocity = CurrentKnockback;
			MoveAndSlide();
			// return;
		}
		
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
		
		if (Math.Abs(CurrentKnockback.X) > 1.0f && Math.Abs(CurrentKnockback.Y) > 1.0f)
		{
			Velocity /= CurrentKnockback;
		}
		
		MoveAndSlide();
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("attack"))
		{
			foreach (Node2D overlappingBody in GetNode<Area2D>("Look/HurtBox").GetOverlappingBodies())
			{
				if (overlappingBody is LivingEntity livingEntity && !livingEntity.HasMethod("Player"))
				{
					Attack(livingEntity, 0);
				}
			}
		}
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseButtonEvent)
		{
			Camera2D camera = GetNode<Camera2D>("%MainCamera");
			if (camera == null) return;
			if (mouseButtonEvent.ButtonIndex == MouseButton.WheelDown && mouseButtonEvent.Pressed)
			{
				camera.SetZoom(camera.GetZoom() / 1.2f);
			} else if (mouseButtonEvent.ButtonIndex == MouseButton.WheelUp && mouseButtonEvent.Pressed)
			{
				camera.SetZoom(camera.GetZoom() * 1.2f);
			}
		}
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		
		if (!Dead)
		{
			GetNode<Label>("DebugInfo").Text = @$"
HP: {Health}/{MaxHealth}
Cooldown: {Math.Round(AttackTimer.TimeLeft, 1)}/{AttackTimer.WaitTime}";
		}
	}
}
