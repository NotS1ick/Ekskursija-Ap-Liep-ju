using Godot;
using System;
using System.Threading.Tasks;

public partial class Clippy : Node
{
	[Export]
	public NodePath TextWriterPath;
	[Export]
	public NodePath ContainerPath;
	[Export]
	public NodePath SpritePath;

	private Control _container;
	private Control _sprite;
	private TextWriter _textWriter;

	bool loadedInOnce;

	public override async void _Ready()
	{
		if (TextWriterPath != null)
			_textWriter = GetNodeOrNull<TextWriter>(TextWriterPath);

		if (ContainerPath != null)
			_container = GetNodeOrNull<Control>(ContainerPath);
		else
			_container = GetNodeOrNull<Control>("BoxContainer");

		if (SpritePath != null)
			_sprite = GetNodeOrNull<Control>(SpritePath);

		if (_container != null)
		{
			_container.Visible = false;
		}

	}

	public async Task PlayDialogue(string[] text)
	{
		PlayerStats.CanMove = false;

		await PlayAnim();

		if (_textWriter != null)
			await _textWriter.PlayEffect(text);

		if (_container != null)
			_container.Visible = false;

		PlayerStats.CanMove = true;

	}


	private float _originalContainerY = 0f;
	private bool _hasOriginalContainerY = false;

	public async Task PlayAnim()
	{
		if (loadedInOnce)
		{
			if (_textWriter != null && _textWriter._richTextLabel != null)
				_textWriter._richTextLabel.Clear();
			if (_textWriter != null && _textWriter._canvasGroup != null)
				_textWriter._canvasGroup.Visible = false;
		}

		if (_container != null)
		{
			if (!_hasOriginalContainerY)
			{
				_originalContainerY = _container.Position.Y;
				_hasOriginalContainerY = true;
			}

			float baseY = _originalContainerY;

			_container.PivotOffset = new Vector2(_container.Size.X / 2, _container.Size.Y);
			_container.Scale = new Vector2(0.82f, 0);
			_container.Position = new Vector2(_container.Position.X, baseY + 280);
			_container.Visible = true;

			var scaleTween = CreateTween();
			scaleTween.TweenProperty(_container, "scale", new Vector2(1f, 1f), 0.5f);

			var posTween = CreateTween();
			posTween.TweenProperty(_container, "position:y", baseY - 20.0f, 0.2f);
			posTween.TweenProperty(_container, "position:y", baseY + 120.0f, 0.2f);
			posTween.TweenProperty(_container, "position:y", baseY, 0.1f);

			await ToSignal(posTween, Tween.SignalName.Finished);
		}
		else
		{
			await ToSignal(GetTree().CreateTimer(0.1f), SceneTreeTimer.SignalName.Timeout);
		}
	}

	public override void _Process(double delta)
	{
	}
}
