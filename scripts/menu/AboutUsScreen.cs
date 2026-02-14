using Godot;
using System;

public partial class AboutUsScreen : Control
{
	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("back"))
		{
			GetNode<ScTrans>("/root/ScTrans").ChangeScene("/menu/menu_screen");
		}
	}
}
