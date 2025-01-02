using FrontEnd4IdleStrategyFS.Global;
using Godot;

namespace ZeromaXPlayground.demo.FPS;

public partial class IdlePlayerState : State
{
    public override void Update(double delta)
    {
        // GD.Print("IdlePlayerState Update");
        if (FpsGlobalNodeFS.Instance.player.Velocity.Length() > 0.0)
        {
            EmitSignal(TransitionSignal, "WalkingPlayerState");
        }
    }
}