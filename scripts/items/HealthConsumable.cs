using Godot;
using System;

public partial class HealthConsumable : Item
{
    public override void Use(LivingEntity user)
    {
        base.Use(user);

        GD.Print(user.Health);
        user.Heal(7.6f);
        GD.Print(user.Health);
    }
}
