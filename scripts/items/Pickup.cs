using System;
using System.Collections.Generic;
using Godot;

public partial class Pickup : Node2D
{
    [Export] public PackedScene ItemScene { get; set; }
    [Export(PropertyHint.Range, "0,100,0.1")] public float SpawnChance { get; set; } = 100.0f;
    [Export] public bool InstantConsume { get; set; } = false;
    
    [ExportGroup("Items")]
    [Export] public PackedScene[] ItemScenes { get; set; } = Array.Empty<PackedScene>();
    [Export(PropertyHint.Range, "0,100,0.1")] public float[] ItemWeights { get; set; } = Array.Empty<float>();
    
    private Item _item;

    public override void _Ready()
    {
        if (ItemScene == null && ItemScenes == null)
        {
            
            QueueFree();
            return;
        }
        
        if (ItemScene != null)
        {
            _item = ItemScene.Instantiate<Item>();
            AddChild(_item);
            GetNode<Sprite2D>("ItemSprite").QueueFree();
            return;
        }

        if (ItemScenes != null)
        {
            for (int j = 0; j < 10; j++)
            {
                double p = GD.RandRange(0.0f, 100.0f);
                PackedScene chosenScene = null;
                for (int i = 0; i < ItemScenes.Length; i++)
                {
                    if (p <= ItemWeights[i] & p > 0)
                    {
                        chosenScene = ItemScenes[i];
                    }
                    p -= ItemWeights[i];
                }
                
                if (chosenScene == null) return;

                _item = chosenScene.Instantiate<Item>();
                AddChild(_item);
                GetNode<Sprite2D>("ItemSprite").QueueFree();
            }
        }
    }

    private void OnPickupAreaBodyEntered(Node2D body)
    {
        if (body is Player player)
        {
            if (player.PrimaryItem != null) return;
            if (!InstantConsume)
            {
                RemoveChild(_item);
                GlobalVariables.PlayerUi.AddChild(_item);
                player.PrimaryItem = _item;
            }
            else
            {
                _item.Use(player);
                QueueFree();
            }
        }
    }
}
