using Godot;
using System;

public partial class AboutUsAuthorSources : Control
{

    private ScreenLoader _scLoader;
    private ScrollContainer _scrollContainer;

    private float _scrollSpeed = 50f;
    private float _exactVerticalScroll = 0f;
    private bool _isAutoScrolling = true;

    public override void _Ready()
    {
        _scLoader = GetNode<ScreenLoader>("/root/ScreenLoader");
        _scrollContainer = GetNode<ScrollContainer>("MarginContainer/ScrollContainer");

        var vScrollBar = _scrollContainer.GetVScrollBar();
        if (vScrollBar != null)
        {
            vScrollBar.Modulate = new Color(1, 1, 1, 0);
        }

        ApplyStarWarsPerspective();
    }

    private void ApplyStarWarsPerspective()
    {
        var marginContainer = GetNode<MarginContainer>("MarginContainer");
        RemoveChild(marginContainer);

        var subViewportContainer = new SubViewportContainer();
        subViewportContainer.Name = "PerspectiveContainer";
        subViewportContainer.SetAnchorsPreset(LayoutPreset.FullRect);
        subViewportContainer.Stretch = true;

        var subViewport = new SubViewport();
        subViewport.Name = "PerspectiveViewport";
        subViewport.TransparentBg = true;

        AddChild(subViewportContainer);
        subViewportContainer.AddChild(subViewport);
        subViewport.AddChild(marginContainer);

        marginContainer.SetAnchorsPreset(LayoutPreset.FullRect);

        var shader = new Shader();
        shader.Code = @"
        shader_type canvas_item;

        uniform float far = 3.5;
        uniform float near = 1.0;

        void fragment() {
            float inv_far = 1.0 / far;
            float inv_near = 1.0 / near;
            
            float inv_d = mix(inv_far, inv_near, UV.y);
            float d = 1.0 / inv_d;
            
            float u = (UV.x - 0.5) * (d / near) + 0.5;
            
            float v = (far - d) / (far - near);
            
            if (u < 0.0 || u > 1.0 || v < 0.0 || v > 1.0) {
                COLOR = vec4(0.0);
            } else {
                COLOR = texture(TEXTURE, vec2(u, v));
                COLOR.a *= smoothstep(0.05, 0.4, UV.y);
            }
        }
        ";
        subViewportContainer.Material = new ShaderMaterial { Shader = shader };
    }

    public override void _Process(double delta)
    {
        if (_isAutoScrolling)
        {
            int expectedScroll = Mathf.RoundToInt(_exactVerticalScroll);

            if (Mathf.Abs(_scrollContainer.ScrollVertical - expectedScroll) > 1)
            {
                _isAutoScrolling = false;
            }
            else
            {
                _exactVerticalScroll += _scrollSpeed * (float)delta;
                _scrollContainer.ScrollVertical = Mathf.RoundToInt(_exactVerticalScroll);
            }
        }
    }

    public override async void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("back"))
        {
            SetProcessInput(false);

            var aboutUsScreen = GD.Load<PackedScene>("res://scenes/about_us_screen.tscn");

            if (_scLoader != null && aboutUsScreen != null)
            {
                await _scLoader.ChangeScene(aboutUsScreen);
            }
        }

        if (@event.IsActionPressed("enter"))
        {
            SetProcessInput(false);

            var gameScreen = GD.Load<PackedScene>("res://scenes/game_screen.tscn");

            if (_scLoader != null && gameScreen != null)
            {
                await _scLoader.ChangeScene(gameScreen);
            }
        }
    }
}
