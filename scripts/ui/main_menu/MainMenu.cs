using Godot;
using System;

public partial class MainMenu : Node
{

	private ScreenLoader _scLoader;
	private PackedScene _gameScreen;
	private PackedScene _aboutUsScreen;

	public override void _Ready()
	{
		_gameScreen = GD.Load<PackedScene>("res://scenes/game_screen.tscn");
		_aboutUsScreen = GD.Load<PackedScene>("res://scenes/about_us_screen.tscn");
		GetNode<Button>("MarginContainer/VBoxContainer/BoxContainer/TextureRect/MarginContainer/VBoxContainer/Button").GrabFocus();
		_scLoader = GetNode<ScreenLoader>("/root/ScreenLoader");
	}

	async void _on_start_pressed()
	{
		GD.Print("start pressed");
		await _scLoader.ChangeScene(_gameScreen);
	}

	async void _on_about_us_pressed()
	{
		GD.Print("about_us pressed");
		await _scLoader.ChangeScene(_aboutUsScreen);
	}

	async void _on_exit_pressed()
	{
		GD.Print("qu pressed");
		GetTree().Root.PropagateNotification((int)NotificationWMCloseRequest);
	}
}
