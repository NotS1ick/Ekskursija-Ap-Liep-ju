using Godot;
using System.Threading.Tasks;

public partial class ScTrans : CanvasLayer
{
    private AnimationPlayer _animationPlayer;

    public override void _Ready()
    {
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }

    public async Task ChangeScene(string target)
    {
        _animationPlayer.Play("dissolve");
        await ToSignal(_animationPlayer, "animation_finished");
        GetTree().ChangeSceneToFile($"res://scenes/{target}.tscn");
        _animationPlayer.PlayBackwards("dissolve");
    }
}
