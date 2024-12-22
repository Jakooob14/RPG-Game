using Godot;
using System;

public partial class LivingEntity : Entity
{
    [Export]
    public float Speed = 300.0f;
    [Export] 
    public float AttackCooldown = 1.0f;
	
    public float Health { get; set; }
    public float MaxHealth { get; set; }

    private protected Timer AttackTimer = new Timer();
	
    public LivingEntity()
    {
        MaxHealth = 100;
        Health = MaxHealth;
    }

    public virtual void Damage(float damageAmount)
    {
        Health -= damageAmount;
        if (Health <= 0)
        {
            Die();
        }
        // TODO: Backwards knockback
    }

    public virtual void Die()
    {
        QueueFree();
    }
    
    public virtual void Attack(LivingEntity livingEntity, float damageAmount)
    {
        if (!AttackTimer.IsStopped()) return;
        livingEntity.Damage(damageAmount);
        AttackTimer.Start();
    }

    public override void _Ready()
    {
        base._Ready();
        
        AddChild(AttackTimer);
        AttackTimer.OneShot = true;
        AttackTimer.WaitTime = AttackCooldown;
    }
    
    public override void _Process(double delta)
    {
        base._Process(delta);
        
        GetNode<Label>("DebugInfo").Text = @$"
HP: {Health}/{MaxHealth}
Cooldown: {Math.Round(AttackTimer.TimeLeft, 1)}/{AttackTimer.WaitTime}";
    }
}
