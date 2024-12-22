using Godot;
using System;

public partial class Enemy : LivingEntity
{
	[ExportGroup("Attack Delay")] 
	[Export] 
	public float AttackDelayMin = 0.1f;
	
	[ExportGroup("Attack Delay")]
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
		foreach (Node2D overlappingBody in GetNode<Area2D>("Look/AttackArea").GetOverlappingBodies())
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

		GetNode<Label>("DebugInfo").Text = @$"
HP: {Health}/{MaxHealth}
Cooldown: {Math.Round(AttackTimer.TimeLeft, 1)}/{AttackTimer.WaitTime}
Delay: {Math.Round(DelayTimer.TimeLeft, 1)}/{Math.Round(DelayTimer.WaitTime, 1)}";
		
		foreach (Node2D overlappingBody in GetNode<Area2D>("Look/AttackArea").GetOverlappingBodies())
		{
			if (overlappingBody is Player player)
			{
				Attack(player, 5);
			}
		}
	}
	
	public override void _PhysicsProcess(double delta)
	{
		if (!HasNode("/root/Dungeon/Player")) return;
		CharacterBody2D player = GetNode<CharacterBody2D>("/root/Dungeon/Player");
		if (player == null) return;
		
		foreach (Node2D overlappingBody in AssignedRoom.RoomRef.GetNode<Area2D>("Node2D/RoomConstraints").GetOverlappingBodies())
		{
			if (overlappingBody is Player)
			{
				GetNode<Node2D>("Look").LookAt(player.Position);
		
				foreach (Node2D chaseOverlappingBody in GetNode<Area2D>("MaxChaseArea").GetOverlappingBodies())
				{
					if (chaseOverlappingBody is Player) return;
				}

				Velocity = Position.DirectionTo(player.Position) * Speed;

				MoveAndSlide();
				return;
			}
		}
		
	}
}
