using FrontEndToolFS.Tool;
using Godot;

namespace ZeromaXPlayground.game.HexPlane.Map;

public partial class HexMapCamera : HexMapCameraFS
{
    [Export]
    public double StickMinZoom
    {
        get => _stickMinZoom;
        set => _stickMinZoom = value;
    }

    [Export]
    public double StickMaxZoom
    {
        get => _stickMaxZoom;
        set => _stickMaxZoom = value;
    }

    [Export]
    public double SwivelMinZoom
    {
        get => _swivelMinZoom;
        set => _swivelMinZoom = value;
    }

    [Export]
    public double SwivelMaxZoom
    {
        get => _swivelMaxZoom;
        set => _swivelMaxZoom = value;
    }

    [Export]
    public double MoveSpeedMinZoom
    {
        get => _moveSpeedMinZoom;
        set => _moveSpeedMinZoom = value;
    }

    [Export]
    public double MoveSpeedMaxZoom
    {
        get => _moveSpeedMaxZoom;
        set => _moveSpeedMaxZoom = value;
    }

    [Export]
    public float RotationSpeed
    {
        get => _rotationSpeed;
        set => _rotationSpeed = value;
    }

    [Export]
    public HexGridFS Grid
    {
        get => _grid;
        set => _grid = value;
    }

    // 请忽略 IDE 冗余提示，需要保留此处和 partial
    public override void _Ready() => base._Ready();
    public override void _UnhandledInput(InputEvent e) => base._UnhandledInput(e);
    public override void _Process(double delta) => base._Process(delta);
}