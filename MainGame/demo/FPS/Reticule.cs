using FrontEnd4IdleStrategyFS.FPS;
using Godot;

namespace ZeromaXPlayground.demo.FPS;

public partial class Reticule : ReticuleFS
{
    [Export]
    public Line2D[] ReticuleLines
    {
        get => reticuleLines;
        set => reticuleLines = value;
    }

    [Export]
    public CharacterBody3D PlayerController
    {
        get => playerController;
        set => playerController = value;
    }

    [Export]
    public float ReticuleSpeed
    {
        get => reticuleSpeed;
        set => reticuleSpeed = value;
    }

    [Export]
    public float ReticuleDistance
    {
        get => reticuleDistance;
        set => reticuleDistance = value;
    }

    [Export]
    public float DotRadius
    {
        get => dotRadius;
        set => dotRadius = value;
    }

    [Export]
    public Color DotColor
    {
        get => dotColor;
        set => dotColor = value;
    }

    // 请忽略 IDE 冗余提示，需要保留此处和 partial
    public override void _Draw() => base._Draw();
    public override void _Ready() => base._Ready();
    public override void _Process(double delta) => base._Process(delta);
}