using Godot;
using System;

public partial class Enemy : LivingEntity
{
	[Export]
	public float DamageAmount = 20f;
	
	[ExportGroup("Attack Delay")] 
	[Export] 
	public float AttackDelayMin = 0.1f;
	[Export]
	public float AttackDelayMax = 0.5f;
	
	
	private protected Timer DelayTimer = new Timer();
	
	public Room AssignedRoom { get; set; }

	public override void Attack(LivingEntity livingEntity, float damageAmount)
	{
		if (!DelayTimer.IsStopped() || !AttackTimer.IsStopped()) return;
		DelayTimer.WaitTime = GD.RandRange(AttackDelayMin, AttackDelayMax);
		DelayTimer.Timeout += () => OnAttackDelayTimerTimeout(livingEntity, damageAmount);
		DelayTimer.Start();
	}

	void OnAttackDelayTimerTimeout(LivingEntity livingEntity, float damageAmount)
	{
		if (Dead) return;
		
		foreach (Node2D overlappingBody in GetNode<Area2D>("Look/HurtBox").GetOverlappingBodies())
		{
			if (overlappingBody == livingEntity)
			{
				base.Attack(livingEntity, damageAmount);
			}
		}
	}

	public override void _Ready()
	{
		base._Ready();
		
		AddChild(DelayTimer);
		DelayTimer.OneShot = true;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (!Dead)
		{
			GetNode<Label>("DebugInfo").Text = @$"
HP: {Health}/{MaxHealth}
Cooldown: {Math.Round(AttackTimer.TimeLeft, 1)}/{AttackTimer.WaitTime}
Delay: {Math.Round(DelayTimer.TimeLeft, 1)}/{Math.Round(DelayTimer.WaitTime, 1)}";
		}

		if (!Dead)
		{
			foreach (Node2D overlappingBody in GetNode<Area2D>("Look/HurtBox").GetOverlappingBodies())
			{
				if (overlappingBody is Player player)
				{
					Attack(player, DamageAmount);
				}
			}
		}
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
		
		if (!HasNode("/root/Dungeon/Player")) return;
		CharacterBody2D player = GetNode<CharacterBody2D>("/root/Dungeon/Player");
		if (player == null) return;
		
		foreach (Node2D overlappingBody in AssignedRoom.GetNode<Area2D>("Node2D/RoomConstraints").GetOverlappingBodies())
		{
			if (overlappingBody is Player && !Dead)
			{
				// GetNode<Node2D>("Look").LookAt(player.Position);
		
				foreach (Node2D chaseOverlappingBody in GetNode<Area2D>("MaxChaseArea").GetOverlappingBodies())
				{
					if (chaseOverlappingBody is Player) return;
				}

				Velocity = GlobalPosition.DirectionTo(player.Position) * Speed * velocityMultiplier;
				
				MoveAndSlide();
				return;
			}
		}
	}

	public override void Damage(float damageAmount, LivingEntity inducer)
	{
		base.Damage(damageAmount, inducer);

		if (Dead) return;
		
		GpuParticles2D hitParticles = GetNode<GpuParticles2D>("HitParticles");
		hitParticles.LookAt(inducer.Position);
		hitParticles.RotationDegrees += 180;
	}

	public override void Die()
	{
		base.Die();
		
		AssignedRoom.AssignedEntities.Remove(this);
	}
}
