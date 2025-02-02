using Godot;
using System;

public partial class Player : LivingEntity
{
	[Signal] public delegate void UpdateHealthEventHandler(float newHealth);

	private Item _primaryItem;

	public Item PrimaryItem
	{
		get => _primaryItem;
		set
		{
			_primaryItem = value;
			GlobalVariables.PlayerUi.UpdateItems();
		}
	}

	public override void Damage(float damageAmount, LivingEntity inducer)
	{
		base.Damage(damageAmount, inducer);
		
		if (Health <= 0)
		{
			GlobalVariables.PlayerUi.UpdateHealth(0);
			return;
		}
		
		GlobalVariables.PlayerUi.UpdateHealth();
	
	}

	public override void Heal(float healAmount)
	{
		base.Heal(healAmount);
		
		GlobalVariables.PlayerUi.UpdateHealth();
	
	}

	public override void Die()
	{
		base.Die();

		GlobalVariables.PlayerUi.GetNode<Control>("Control/DeathScreen").Visible = true;
	}

	public override void _Ready()
	{
		base._Ready();

		GlobalVariables.Player = this;
		GlobalVariables.PlayerUi.UpdateHealth(MaxHealth);
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		float velocityMultiplier = 1.0f;
		if (Math.Abs(CurrentKnockback.X) > 0.0f || Math.Abs(CurrentKnockback.Y) > 0.0f)
		{
			Velocity = CurrentKnockback;
			MoveAndSlide();
			velocityMultiplier = 1.0f - (CurrentKnockback.X + CurrentKnockback.Y) / (InitialKnockback.X + InitialKnockback.Y);
		}
		else
		{
			velocityMultiplier = 1;
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
		
		Velocity = velocity * velocityMultiplier;
		
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
		if (@event.IsActionPressed("use_primary_item"))
		{
			PrimaryItem.Use(this);
			PrimaryItem.QueueFree();
			PrimaryItem = null;
		}
	}

	// public override void _UnhandledInput(InputEvent @event)
	// {
	// 	if (@event is InputEventMouseButton mouseButtonEvent)
	// 	{
	// 		Camera2D camera = GetNode<Camera2D>("%MainCamera");
	// 		if (camera == null) return;
	// 		if (mouseButtonEvent.ButtonIndex == MouseButton.WheelDown && mouseButtonEvent.Pressed)
	// 		{
	// 			camera.SetZoom(camera.GetZoom() / 1.2f);
	// 		} else if (mouseButtonEvent.ButtonIndex == MouseButton.WheelUp && mouseButtonEvent.Pressed)
	// 		{
	// 			camera.SetZoom(camera.GetZoom() * 1.2f);
	// 		}
	// 	}
	// }

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
