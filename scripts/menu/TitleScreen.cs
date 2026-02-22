using Godot;
using System;

public partial class TitleScreen : Node2D
{

    public override async void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventKey keyEvent && keyEvent.Pressed && !keyEvent.Echo)
        {
            await GetNode<ScTrans>("/root/ScTrans").ChangeScene("/menu/menu_screen");
        }
    }
}
