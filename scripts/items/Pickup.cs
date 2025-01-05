using Godot;

public partial class Pickup : Node2D
{
    [Export] public PackedScene ItemScene { get; set; }
    
    private Item _item;

    public override void _Ready()
    {
        if (ItemScene == null)
        {
            QueueFree();
            return;
        }
        
        _item = ItemScene.Instantiate<Item>();
        AddChild(_item);
        GetNode<Sprite2D>("ItemSprite").QueueFree();
    }

    private void OnPickupAreaBodyEntered(Node2D body)
    {
        if (body is Player player)
        {
            _item.Use(player);
            player.PrimaryItem = _item;
            QueueFree();
        }
    }
}