using Godot;
using System;

public partial class PlayerUi : CanvasLayer
{
	private Label _healthLabel;

	public override void _Ready()
	{
		_healthLabel = GetNode<Label>("Control/Health");
	}

	private void OnPlayerUpdateHealth(float newHealth)
	{
		if (_healthLabel == null) return;

		_healthLabel.Text = $"Health: {newHealth}";
	}
}
