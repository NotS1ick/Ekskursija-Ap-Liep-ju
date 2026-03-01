using Godot;

public partial class TitleScreen : Control
{
	[Export]
	private PackedScene _screen;
	private ScreenLoader _scLoader;

	public override void _Ready()
	{
		_scLoader = GetNode<ScreenLoader>("/root/ScreenLoader");
	}

	public override async void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventKey keyEvent && keyEvent.Pressed && !keyEvent.Echo)
		{
			await _scLoader.ChangeScene(_screen);
		}
	}
}
