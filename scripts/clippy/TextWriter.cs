using Godot;
using System;
using System.Threading.Tasks;

public partial class TextWriter : Node
{
	public RichTextLabel _richTextLabel;
	public CanvasGroup _canvasGroup;
	private AudioStreamPlayer2D _audioStreamPlayer2D;

	public override void _Ready()
	{
		_richTextLabel = GetNode<RichTextLabel>("RichTextLabel");
		_canvasGroup = GetNode<CanvasGroup>("../Continue");
		_audioStreamPlayer2D = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
	}

	public override void _Process(double delta)
	{
	}

	public async Task PlayEffect(string[] text)
	{
		GD.Print("playeffect called");
		int arrayLength = text.Length;

		for (int i = 0; i < arrayLength; i++)
		{
			_canvasGroup.Visible = false;
			string currentText = text[i];
			int visibleCharacters = 0;
			int length = currentText.Length;

			_richTextLabel.Clear();
			_richTextLabel.AddText(currentText);

			if (visibleCharacters != _richTextLabel.VisibleCharacters)
			{
				_richTextLabel.VisibleCharacters = visibleCharacters;
			}

			for (int j = visibleCharacters; j < length; j++)
			{
				_audioStreamPlayer2D.Play();
				visibleCharacters += 1;
				_richTextLabel.VisibleCharacters = visibleCharacters;

				await ToSignal(GetTree().CreateTimer(0.05f), SceneTreeTimer.SignalName.Timeout);
			}

			_canvasGroup.Visible = true;
			while (!Input.IsActionJustPressed("next"))
			{
				await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
			}
		}
	}
}
