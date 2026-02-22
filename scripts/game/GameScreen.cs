using Godot;
using System;

public partial class GameScreen : Control
{
    private AnimationPlayer _animationPlayer;

    // Called when the node enters the scene tree for the first time.
    public override async void _Ready()
    {
        _animationPlayer = GetNode<AnimationPlayer>("CharacterBody2D/AnimationPlayer");

        _animationPlayer.Play("intro");

        await ToSignal(_animationPlayer, AnimationPlayer.AnimationFinished);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
