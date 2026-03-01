using Godot;
using System.Threading.Tasks;

public partial class ScreenLoader : CanvasLayer
{
	private AnimationPlayer _animationPlayer;

	public override void _Ready()
	{
		_animationPlayer = GetNode<AnimationPlayer>("Anim");
	}

	public async Task ChangeScene(PackedScene target)
	{
		_animationPlayer.Play("dissolve");
		await ToSignal(_animationPlayer, AnimationPlayer.SignalName.AnimationFinished);
		GetTree().ChangeSceneToPacked(target);
		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		_animationPlayer.PlayBackwards("dissolve");
	}
}
