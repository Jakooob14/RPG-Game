using Godot;
using System;

public partial class HealthConsumable : Item
{
    [Export] public float HealthHealed { get; set; } = 10.0f;
    public override void Use(LivingEntity user)
    {
        base.Use(user);
        user.Heal(HealthHealed);
    }
}
