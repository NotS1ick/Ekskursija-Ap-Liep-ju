using Godot;
using System;

public partial class Movement : CharacterBody2D
{
    private Camera2D _camera;
    private AnimatedSprite2D _animatedSprite2d;
    private Vector2 _lastDirection = Vector2.Down;

    [Export]
    public float Speed = 400;

    public bool CanMove = true;

    public override void _Ready()
    {

        _camera = GetNode<Camera2D>("Camera2D");
        _camera.MakeCurrent();
        _animatedSprite2d = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 direction = Vector2.Zero;

        if (CanMove)
        {
            direction = Input.GetVector("left", "right", "up", "down");
        }

        if (direction != Vector2.Zero)
        {
            Velocity = direction.Normalized() * Speed;
            _lastDirection = direction;
        }
        else
        {
            Velocity = Vector2.Zero;
        }

        MoveAndSlide();
        UpdateAnimation(direction);
    }

    private void UpdateAnimation(Vector2 direction)
    {
        string newAnimation = "";
        bool flipH = false;

        if (direction != Vector2.Zero)
        {
            if (direction.X != 0 && direction.Y != 0)
            {
                if (direction.Y < 0)
                {
                    newAnimation = "up_45deg";
                }
                else
                {
                    newAnimation = "down_45deg";
                }
                flipH = direction.X < 0;
            }
            else if (direction.X != 0)
            {
                newAnimation = "right";
                flipH = direction.X < 0;
            }
            else
            {
                if (direction.Y < 0)
                {
                    newAnimation = "up";
                }
                else
                {
                    newAnimation = "down";
                }
            }
        }
        else
        {
            if (_lastDirection.X != 0 && _lastDirection.Y != 0)
            {
                if (_lastDirection.Y < 0)
                {
                    newAnimation = "idle_up_45deg";
                }
                else
                {
                    newAnimation = "idle_down_45deg";
                }
                flipH = _lastDirection.X < 0;
            }
            else if (_lastDirection.X != 0)
            {
                newAnimation = "idle_side";
                flipH = _lastDirection.X < 0;
            }
            else
            {
                if (_lastDirection.Y < 0)
                {
                    newAnimation = "idle_back";
                }
                else
                {
                    newAnimation = "idle_front";
                }
            }
        }

        _animatedSprite2d.FlipH = flipH;

        if (_animatedSprite2d.Animation != newAnimation || !_animatedSprite2d.IsPlaying())
        {
            _animatedSprite2d.Play(newAnimation);
        }
    }
}
