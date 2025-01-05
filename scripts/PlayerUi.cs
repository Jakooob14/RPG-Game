using Godot;
using System;

public partial class PlayerUi : CanvasLayer
{
	private Label _healthLabel;

	public override void _Ready()
	{
		GlobalVariables.PlayerUi = this;
		_healthLabel = GetNode<Label>("Control/Health");
	}

	public void UpdateHealth(float? newHealth = null)
	{
		if (newHealth == null)
		{
			newHealth = GlobalVariables.Player.Health;
		}
		_healthLabel.Text = $"Health: {newHealth}";
	}

	public void UpdateItems()
	{
		GetNode<TextureRect>("%Items/PrimaryItem/TextureRect").Texture = GlobalVariables.Player.PrimaryItem.GetNode<Sprite2D>("ItemSprite").Texture;
	}
}
