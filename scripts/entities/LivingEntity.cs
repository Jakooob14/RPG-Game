using Godot;
using System;
using System.ComponentModel;

public partial class LivingEntity : Entity
{
    [Export]
    public float Speed = 300.0f;
    [Export] 
    public float AttackCooldown = 1.0f;
    
    [ExportGroup("Knockback")]
    [Export] 
    public float KnockbackMultiplier = 1.0f;
    [Export] 
    public float KnockbackFriction = 0.1f;
    [ExportGroup("Knockback")]
    [Export]
    public bool KnockbackSpeedOverride = false;
    [ExportGroup("Knockback")]
    [Export]
    public float KnockbackSpeedOverrideValue = 300.0f;
	
    public float Health { get; set; }
    public float MaxHealth { get; set; }

    public bool Dead { get; set; }

    public Timer AttackTimer = new Timer();

    public Vector2 CurrentKnockback = Vector2.Zero;

    private protected Vector2 InitialKnockback = Vector2.Zero;
	
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
        
        GetNode<AnimationPlayer>("AnimationPlayer").Play("hit");
        Knockback(inducer.GlobalPosition, KnockbackMultiplier);
    }
    public virtual void Heal(float healAmount)
    {
        Health += healAmount;
    }
    
    public virtual void Knockback(Vector2 fromPosition, float knockbackMultiplier = 1.0f)
    {
        Vector2 direction = (GlobalPosition - fromPosition).Normalized();
        float knockbackSpeed = (KnockbackSpeedOverride ? KnockbackSpeedOverrideValue : Speed);

        CurrentKnockback = direction * knockbackSpeed * knockbackMultiplier * 2.0f;
        InitialKnockback = CurrentKnockback;
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
        OnAttackTimerStart();
    }

    public virtual void OnAttackTimerStart(){}
    public virtual void OnAttackTimerEnd(){}

    public override void _Ready()
    {
        base._Ready();
        
        AddChild(AttackTimer);
        AttackTimer.OneShot = true;
        AttackTimer.WaitTime = AttackCooldown;
        AttackTimer.Timeout += OnAttackTimerEnd;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        
        if (Math.Abs(CurrentKnockback.X) < 20.0f) CurrentKnockback.X = 0;
        if (Math.Abs(CurrentKnockback.Y) < 20.0f) CurrentKnockback.Y = 0;
        
        CurrentKnockback = CurrentKnockback.Lerp(Vector2.Zero, KnockbackFriction);
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
