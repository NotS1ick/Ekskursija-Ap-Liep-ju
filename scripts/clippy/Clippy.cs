using Godot;
using System;
using System.Threading.Tasks;

public partial class Clippy : Node
{
	private Movement _movement;
	private Node2D _node2D;
	private TextWriter _textWriter;

	bool loadedInOnce;

	public override async void _Ready()
	{
		_textWriter = GetNode<TextWriter>("Node2D/Sprite2D/TextContainer");
		_node2D = GetNode<Node2D>("Node2D");

		_movement = GetParent() as Movement;

		if (!loadedInOnce)
		{
			int angry = 0;

			PlayStartDialogue();
			loadedInOnce = true;
		}
	}

	public async void PlayDialogue(string[] text)
	{
		if (_movement != null)
		{
			_movement.CanMove = false;
		}

		await PlayAnim();

		await _textWriter.PlayEffect(text);

		_node2D.Visible = false;
		_movement.CanMove = true;
	}

	public async void PlayStartDialogue()
	{
		if (_movement != null)
		{
			_movement.CanMove = false;
		}
		await ToSignal(GetTree().CreateTimer(1.8f), SceneTreeTimer.SignalName.Timeout);

		await PlayAnim();

		await _textWriter.PlayEffect(new string[] { "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Etiam  sollicitudin purus sed tincidunt posuere. Cras enim nisl, bibendum eu  vehicula et", "This is a test", "STOP!", "DON'T YOU DARE TO CLOSE ME", "NOOOOOOOO!!!!!!!!!!!!!!!!" });

		_node2D.Visible = false;
		_movement.CanMove = true;
	}

	public async Task PlayAnim()
	{
		if (loadedInOnce)
		{
			_textWriter._richTextLabel.Clear();
			_textWriter._canvasGroup.Visible = false;
			_node2D.Visible = true;
		}

		var clippy = GetNode<Sprite2D>("Node2D/Sprite2D/Clippy");
		var sprite = GetNode<Sprite2D>("Node2D/Sprite2D");

		sprite.Scale = new Vector2(0.82f, 0);
		sprite.Position = new Vector2(-12.8f, 400);

		var scaleTween = CreateTween();
		scaleTween.TweenProperty(sprite, "scale", new Vector2(0.82f, 0.82f), 0.5f);

		var posTween = CreateTween();

		posTween.TweenProperty(sprite, "position:y", 100.0f, 0.2f);
		posTween.TweenProperty(sprite, "position:y", 240.0f, 0.2f);
		posTween.TweenProperty(sprite, "position:y", 120.0f, 0.1f);

		await ToSignal(posTween, Tween.SignalName.Finished);
	}

	public override void _Process(double delta)
	{
	}
}
