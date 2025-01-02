using FrontEnd4IdleStrategyFS.FPS;
using Godot;

namespace ZeromaXPlayground.demo.FPS.States;

public partial class StateMachine : StateMachineFS
{
    [Export]
    public StateFS CurrentState
    {
        get => currentState;
        set => currentState = value;
    }
    // 请忽略 IDE 冗余提示，需要保留此处和 partial
    public override void _Ready() => base._Ready();
    public override void _Process(double delta) => base._Process(delta);
    public override void _PhysicsProcess(double delta) => base._PhysicsProcess(delta);
}