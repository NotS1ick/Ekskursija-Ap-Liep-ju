using Godot;

public partial class AppManager : Node
{
	public override void _Ready()
	{
		GetTree().AutoAcceptQuit = false;
		Database.Load();
	}

	public override void _Notification(int what)
	{
		if (what == NotificationWMCloseRequest)
		{
			GD.Print("App is about to quit! Saving data...");
			Database.Save();

			GetTree().Quit();
		}
	}
}
