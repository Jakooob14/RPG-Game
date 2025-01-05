using Godot;
using System;

[GlobalClass]
public partial class ItemResource : Resource
{
    [Export] public Texture2D Texture { get; set; }
    [Export] public string Name { get; set; }
}
    