using Godot;

[GlobalClass]
public partial class Item : Node2D
{
    [Export] public string ItemName { get; set; }
    
    public virtual void Use(LivingEntity user)
    {
        GD.Print("Used " + Name);
    }
}