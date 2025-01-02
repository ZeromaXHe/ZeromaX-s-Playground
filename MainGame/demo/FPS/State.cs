using Godot;
using FrontEnd4IdleStrategyFS.FPS;

namespace ZeromaXPlayground.demo.FPS;

public abstract partial class State: StateFS
{
    [Signal]
    public delegate void TransitionEventHandler(string newStateName);
    public override StringName TransitionSignal { get => SignalName.Transition; }
}
