using FrontEnd4IdleStrategyFS.FPS;
using Godot;

namespace ZeromaXPlayground.demo.FPS.States;

public abstract partial class State: StateFS
{
    [Signal]
    public delegate void TransitionEventHandler(string newStateName);
    public override StringName TransitionSignal { get => SignalName.Transition; }
}
