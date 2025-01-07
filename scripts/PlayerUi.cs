using Godot;
using System;

public partial class PlayerUi : CanvasLayer
{
	private Label _healthLabel;
	private TextureRect _primaryTexture;

	public override void _Ready()
	{
		GlobalVariables.PlayerUi = this;

		_primaryTexture = GetNode<TextureRect>("%Items/PrimaryItem/TextureRect");
		
		_healthLabel = GetNode<Label>("Control/Health");
		_primaryTexture.Texture = null;
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
		_primaryTexture.Texture = GlobalVariables.Player.PrimaryItem == null ? null : GlobalVariables.Player.PrimaryItem.GetNode<Sprite2D>("ItemSprite").Texture;
	}
}
