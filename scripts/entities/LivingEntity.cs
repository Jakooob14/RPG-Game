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

    public bool Dead { get; set; }

    private protected Timer AttackTimer = new Timer();
	
    public LivingEntity()
    {
        MaxHealth = 100;
        Health = MaxHealth;
    }

    public virtual void Damage(float damageAmount, LivingEntity inducer)
    {
        Health -= damageAmount;
        if (Health <= 0)
        {
            Die();
        }
        // TODO: Backwards knockback
    }

    public virtual async void Die()
    {
        Dead = true;
        
        foreach (Node child in GetChildren())
        {
            if (child is AnimationPlayer || child is Timer) continue;
            if (child is GpuParticles2D && child.Name == "DeathParticles") continue;
            child.QueueFree();
        }
        
        GetNode<GpuParticles2D>("DeathParticles").Emitting = true;

        await ToSignal(GetTree().CreateTimer(GetNode<GpuParticles2D>("DeathParticles").Lifetime), "timeout");

        QueueFree();
    }
    
    public virtual void Attack(LivingEntity livingEntity, float damageAmount)
    {
        if (!AttackTimer.IsStopped()) return;
        livingEntity.Damage(damageAmount, this);
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

        if (!Dead)
        {
            GetNode<Label>("DebugInfo").Text = @$"
HP: {Health}/{MaxHealth}
Cooldown: {Math.Round(AttackTimer.TimeLeft, 1)}/{AttackTimer.WaitTime}";
        }
    }
}
