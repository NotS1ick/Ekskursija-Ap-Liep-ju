using Godot;
using System;

public static class PlayerStats
{
	public static bool CanMove { get; set; } = true;
}

public partial class Player : CharacterBody2D
{

	[Export]
	public float Speed = 300.0f;

	private AnimatedSprite2D _animatedSprite;
	private string _currentIdleAnimation = "idle_front";
	private AnimationPlayer _animationPlayer;

	public override async void _Ready()
	{
		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

		if (Database.PlayedOnce)
		{
			GlobalPosition = Database.LastPosition;
		}
		else
		{
			Database.PlayedOnce = true;
			Database.LastPosition = GlobalPosition;
		}

		PlayerStats.CanMove = false;
		_animationPlayer.Play("zoom_in");

		await ToSignal(_animationPlayer, AnimationPlayer.SignalName.AnimationFinished);
		PlayerStats.CanMove = true;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!PlayerStats.CanMove)
		{
			Velocity = Vector2.Zero;
			_animatedSprite.Play(_currentIdleAnimation);
			MoveAndSlide();
			return;
		}

		Vector2 velocity = Velocity;

		Vector2 direction = Input.GetVector("left", "right", "up", "down");

		if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Y = direction.Y * Speed;
			GD.Print(Position);

			if (direction.X < 0)
			{
				_animatedSprite.FlipH = true;
			}
			else if (direction.X > 0)
			{
				_animatedSprite.FlipH = false;
			}

			if (direction.X != 0 && direction.Y > 0)
			{
				_animatedSprite.Play("down_45deg");
				_currentIdleAnimation = "idle_down_45deg";
			}
			else if (direction.X != 0 && direction.Y < 0)
			{
				_animatedSprite.Play("up_45deg");
				_currentIdleAnimation = "idle_up_45deg";
			}
			else if (direction.X != 0)
			{
				_animatedSprite.Play("right");
				_currentIdleAnimation = "idle_side";
			}
			else if (direction.Y > 0)
			{
				_animatedSprite.Play("down");
				_currentIdleAnimation = "idle_front";
			}
			else if (direction.Y < 0)
			{
				_animatedSprite.Play("up");
				_currentIdleAnimation = "idle_back";
			}
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Y = Mathf.MoveToward(Velocity.Y, 0, Speed);

			_animatedSprite.Play(_currentIdleAnimation);
		}

		Velocity = velocity;
		MoveAndSlide();

		Database.LastPosition = GlobalPosition;
	}
}
