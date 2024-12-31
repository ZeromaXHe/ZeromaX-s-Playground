using FrontEnd4IdleStrategyFS.FPS;
using Godot;

namespace ZeromaXPlayground.demo.FPS;

public partial class FpsController : FpsControllerFS
{
    [Export]
    public float MouseSensitivity
    {
        get => mouseSensitivity;
        set => mouseSensitivity = value;
    }

    [Export]
    public float TiltLowerLimit
    {
        get => tiltLowerLimit;
        set => tiltLowerLimit = value;
    }

    [Export]
    public float TiltUpperLimit
    {
        get => tiltUpperLimit;
        set => tiltUpperLimit = value;
    }

    [Export]
    public Camera3D CameraController
    {
        get => cameraController;
        set => cameraController = value;
    }

    // 请忽略 IDE 冗余提示，需要保留此处和 partial
    public override void _Ready() => base._Ready();
    public override void _PhysicsProcess(double delta) => base._PhysicsProcess(delta);
    public override void _Input(InputEvent @event) => base._Input(@event);
    public override void _UnhandledInput(InputEvent @event) => base._UnhandledInput(@event);
}