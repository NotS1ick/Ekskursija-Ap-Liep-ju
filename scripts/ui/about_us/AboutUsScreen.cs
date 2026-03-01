using Godot;

public partial class AboutUsScreen : Control
{


	private ScreenLoader _scLoader;

	public override void _Ready()
	{
		_scLoader = GetNode<ScreenLoader>("/root/ScreenLoader");
	}

	public override async void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("back"))
		{
			var mainMenuScreen = GD.Load<PackedScene>("res://scenes/main_menu_screen.tscn");

			await _scLoader.ChangeScene(mainMenuScreen);
		}

		if (@event.IsActionPressed("enter"))
		{
			SetProcessInput(false);

			var gameScreen = GD.Load<PackedScene>("res://scenes/game_screen.tscn");

			if (_scLoader != null && gameScreen != null)
			{
				await _scLoader.ChangeScene(gameScreen);
			}
		}
	}

	async void _on_button_pressed()
	{
		var authorsNSources = GD.Load<PackedScene>("res://scenes/about_us_author_sources.tscn");

		await _scLoader.ChangeScene(authorsNSources);
	}
}
