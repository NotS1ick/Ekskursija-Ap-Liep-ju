using Godot;
using System;
using System.Threading.Tasks;

public partial class TextWriter : Node
{
	[Export]
	public RichTextLabel _richTextLabel;
	[Export]
	public HBoxContainer _canvasGroup;
	[Export]
	private AudioStreamPlayer2D _audioStreamPlayer2D;

	public override void _Ready()
	{
		if (_richTextLabel == null)
			_richTextLabel = GetNodeOrNull<RichTextLabel>("RichTextLabel");
		if (_canvasGroup == null)
			_canvasGroup = GetNodeOrNull<HBoxContainer>("./Continue");
		if (_audioStreamPlayer2D == null)
			_audioStreamPlayer2D = GetNodeOrNull<AudioStreamPlayer2D>("../AudioStreamPlayer2D");
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
			if (_canvasGroup != null)
				_canvasGroup.Visible = false;

			string currentText = text[i];
			int visibleCharacters = 0;
			int length = currentText.Length;

			if (_richTextLabel != null)
			{
				_richTextLabel.Clear();
				_richTextLabel.AddText(currentText);

				if (visibleCharacters != _richTextLabel.VisibleCharacters)
				{
					_richTextLabel.VisibleCharacters = visibleCharacters;
				}
			}

			for (int j = visibleCharacters; j < length; j++)
			{
				if (_audioStreamPlayer2D != null)
					_audioStreamPlayer2D.Play();

				visibleCharacters += 1;

				if (_richTextLabel != null)
					_richTextLabel.VisibleCharacters = visibleCharacters;

				await ToSignal(GetTree().CreateTimer(0.05f), SceneTreeTimer.SignalName.Timeout);
			}

			if (_canvasGroup != null)
				_canvasGroup.Visible = true;

			while (!Input.IsActionJustPressed("enter"))
			{
				await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
			}
		}
	}
}
