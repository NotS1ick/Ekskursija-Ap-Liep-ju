using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

// Helper classes for parsing the JSON
public class QuestionItem
{
	public string activity_name { get; set; }
	public string question { get; set; }
	public List<string> options { get; set; }
	public string correct_answer { get; set; }
}

public class QuestionList
{
	public List<QuestionItem> questions { get; set; }
}

public partial class ActivityWindow : Control
{
	private Control _keyboardHint;
	private bool _isPlayerInArea = false;
	private Tween _hintTween;

	private List<QuestionItem> _allQuestions = new List<QuestionItem>();
	private string _currentActivityName = "";

	// Track quiz game state
	private int _currentMultiplier = 4;
	private QuestionItem _currentQuestionData = null;

	public override void _Ready()
	{
		_keyboardHint = GetNode<Control>("../KeyboardHint");
		_keyboardHint.Visible = false;
		_keyboardHint.Scale = new Vector2(1, 0);

		using var file = FileAccess.Open("res://questions.json", FileAccess.ModeFlags.Read);
		if (file != null)
		{
			string jsonString = file.GetAsText();
			var qList = JsonSerializer.Deserialize<QuestionList>(jsonString);
			if (qList != null && qList.questions != null)
			{
				_allQuestions = qList.questions;
				GD.Print($"Successfully loaded {_allQuestions.Count} questions from questions.json!");
			}
		}

		var rootNode2D = GetNode<Node2D>("../../..");
		foreach (var child in rootNode2D.GetChildren())
		{
			if (child is Area2D area)
			{
				area.BodyEntered += (body) =>
				{
					if (body is Player)
					{
						_currentActivityName = area.Name;
						_isPlayerInArea = true;

						_keyboardHint.Visible = true;
						_keyboardHint.PivotOffset = _keyboardHint.Size / 2;
						_hintTween?.Kill();
						_hintTween = CreateTween();
						_hintTween.TweenProperty(_keyboardHint, "scale", Vector2.One, 0.3f)
							.SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
					}
				};

				area.BodyExited += (body) =>
				{
					if (body is Player)
					{
						_currentActivityName = "";
						_isPlayerInArea = false;

						_hintTween?.Kill();
						_hintTween = CreateTween();
						_hintTween.TweenProperty(_keyboardHint, "scale", new Vector2(1, 0), 0.2f)
							.SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.In);
						_hintTween.TweenCallback(Callable.From(() => _keyboardHint.Visible = false));
					}
				};
			}
		}
	}

	public override void _Process(double delta)
	{
		if (_isPlayerInArea && Input.IsActionJustPressed("interact"))
		{
			PlayerStats.CanMove = false;
			_keyboardHint.Visible = false;
			Visible = true;
			GD.Print($"Player opened Activity: {_currentActivityName}");

			PrepareQuizUIForActivity();
		}
	}

	private void PrepareQuizUIForActivity()
	{
		var quizWindow = GetNode<Control>("QuizWindow");

		_currentQuestionData = _allQuestions.FirstOrDefault(x => x.activity_name == _currentActivityName);
		if (_currentQuestionData != null && _currentQuestionData.options.Count >= 4)
		{
			// Reset the state parameters exactly every time the UI opens!
			_currentMultiplier = 4;

			var label = quizWindow.GetNode<RichTextLabel>("MarginContainer/BoxContainer/MarginContainer/VBoxContainer/RichTextLabel");
			label.Text = _currentQuestionData.question;

			var btnList = new[] { "Button2", "Button", "Button3", "Button4" };
			var prefix = new[] { "A: ", "B: ", "C: ", "D: " };
			var container = quizWindow.GetNode<Control>("MarginContainer/BoxContainer/MarginContainer/VBoxContainer/VBoxContainer");

			for (int i = 0; i < 4; i++)
			{
				var btn = container.GetNode<Button>(btnList[i]);

				// Reset any buttons that were hidden previously!
				btn.Visible = true;

				// Apply current loaded text
				btn.Text = prefix[i] + _currentQuestionData.options[i];

				// We must safely disconnect any old listeners otherwise it will rapid-fire 10x!
				if (btn.IsConnected("pressed", new Callable(this, MethodName.OnQuizAnswerSelected)))
					btn.Disconnect("pressed", new Callable(this, MethodName.OnQuizAnswerSelected));

				// Bind the new answer value physically into the click button callback
				btn.Connect("pressed", Callable.From(() => OnQuizAnswerSelected(btn, btn.Text)));
			}
		}
		else
		{
			GD.PrintErr($"WARNING: No questions.json data found matching Area2D name: '{_currentActivityName}'");
		}
	}

	private void OnQuizAnswerSelected(Button pressedBtn, string rawButtonText)
	{
		// We have to substring out the "A: ", "B: " prefix manually to match the raw answer string!
		string answerToCheck = rawButtonText.Substring(3);

		if (answerToCheck == _currentQuestionData.correct_answer)
		{
			GD.Print($"Winner! Gained {_currentMultiplier} Point(s)!");

			// Save progress instantly!
			Database.Points += _currentMultiplier;
			if (!Database.CompletedActivities.Contains(_currentActivityName))
				Database.CompletedActivities.Add(_currentActivityName);

			Database.Save();

			// Close the window completely out
			Visible = false;
			PlayerStats.CanMove = true;
		}
		else
		{
			GD.Print("Wrong Answer!");
			_currentMultiplier = Math.Max(1, _currentMultiplier / 2); // Drops 4 -> 2 -> 1 -> 1
			pressedBtn.Visible = false; // Poof the button from the UI!
		}
	}

	void _on_button_pressed()
	{
		var marginContainer = GetNode<MarginContainer>("MarginContainer");
		var quizWindow = GetNode<Control>("QuizWindow");

		Tween transitionTween = CreateTween();

		transitionTween.TweenProperty(marginContainer, "modulate:a", 0.0f, 0.3f)
			.SetTrans(Tween.TransitionType.Sine)
			.SetEase(Tween.EaseType.InOut);

		transitionTween.TweenCallback(Callable.From(() =>
		{
			marginContainer.Visible = false;

			var color = quizWindow.Modulate;
			color.A = 0.0f;
			quizWindow.Modulate = color;

			quizWindow.Visible = true;

			quizWindow.GetNode<Button>("MarginContainer/BoxContainer/MarginContainer/VBoxContainer/VBoxContainer/Button2").GrabFocus();
		}));

		transitionTween.TweenProperty(quizWindow, "modulate:a", 1.0f, 0.3f)
			.SetTrans(Tween.TransitionType.Sine)
			.SetEase(Tween.EaseType.InOut);
	}
}
