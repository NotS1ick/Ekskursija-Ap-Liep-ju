using Godot;
using System;

public partial class GameScreen : Control
{
	private AnimationPlayer _animationPlayer;
	private Area2D _area2D;
	private Clippy _clippy;

	// Called when the node enters the scene tree for the first time.
	public override async void _Ready()
	{
		_animationPlayer = GetNode<AnimationPlayer>("CharacterBody2D/AnimationPlayer");

		_animationPlayer.Play("intro");

		_area2D = GetNode<Area2D>("Area2D");

		_clippy = GetNode<Clippy>("CharacterBody2D/ClippyContainer");
	}

	public void _on_area_2d_body_shape_entered(Rid bodyRid, Node2D body, long bodyShapeIndex, long localShapeIndex)
	{
		GD.Print("Body shape entered via editor signal connection!");
		_clippy.PlayDialogue(new string[] { "Hello!", "How are you?" });
		_area2D.QueueFree();
	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
