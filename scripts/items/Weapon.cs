using Godot;
using System;

public partial class Weapon : Node2D
{
    [Export] 
    public float DamageAmount = 1.0f;
    [Export]
    public float Cooldown = 1.0f;
    
    public override void _Input(InputEvent @event)
    {
        if (!@event.IsActionPressed("attack")) return;
        
        if (GetParent().GetParent() is LivingEntity parent)
        {
            if (parent.AttackTimer.IsStopped())
            {
                parent.AttackTimer.Start();
                parent.OnAttackTimerStart();
            }
            else return;
        }
        GetNode<AnimationPlayer>("AnimationPlayer").Play("swing");
    }
    
    private void OnHurtBoxBodyEntered(Node2D body)
    {
        if (body is LivingEntity livingEntity && GetParent().GetParent() is LivingEntity parent && livingEntity != parent)
        {
            livingEntity.Damage(DamageAmount, parent);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        
        Vector2 mousePosition = GetGlobalMousePosition();
        Vector2 direction = mousePosition - GlobalPosition;
        float targetAngle = direction.Angle();
        
        double newRotation = Mathf.LerpAngle(Rotation, targetAngle, 10.0f * delta);
        
        Rotation = (float)newRotation;
    }
}
