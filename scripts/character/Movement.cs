using Godot;
using System;

public partial class Movement : CharacterBody2D
{
	private Camera2D _camera;

	[Export]
	public float Speed = 400;
	
	public override void _Ready()
	{

		_camera = GetNode<Camera2D>("Camera2D");
		_camera.MakeCurrent();
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 direction = Vector2.Zero;
		
		if (Input.IsActionPressed("up"))
		{
			direction.Y -= 1;
		}
		if (Input.IsActionPressed("down"))
		{
			direction.Y += 1;
		}
		if (Input.IsActionPressed("left"))
		{
			direction.X -= 1;
		}
		if (Input.IsActionPressed("right"))
		{
			direction.X += 1;
		}

		if (direction != Vector2.Zero)
		{
			direction = direction.Normalized();
			Velocity = direction * Speed;
		}
		else
		{
			Velocity = Vector2.Zero;
		}

		MoveAndSlide();
	}
}
