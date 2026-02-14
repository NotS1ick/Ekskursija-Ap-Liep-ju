using Godot;
using System.Threading.Tasks;

public partial class Button : Godot.Button
{
	public override void _Ready()
	{
		if (GetIndex() == 0)
		{
			GrabFocus();
		}
	}

	public override void _Pressed()
	{
		if (Text == "Sākt spēli")
		{
			GetNode<ScTrans>("/root/ScTrans").ChangeScene("/game/GameScreen");
		}
		else if (Text == "Par spēli")
		{
			GetNode<ScTrans>("/root/ScTrans").ChangeScene("/menu/about_us/about_us_screen");
		}
		else if (Text == "Iziet" || Name == "Button3")
		{
			GetTree().Quit();
		}
	}
}
