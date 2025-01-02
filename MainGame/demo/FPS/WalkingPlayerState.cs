using FrontEnd4IdleStrategyFS.Global;
using Godot;

namespace ZeromaXPlayground.demo.FPS;

public partial class WalkingPlayerState : State
{
    public override void Update(double delta)
    {
        // GD.Print("WalkingPlayerState Update");
        if (FpsGlobalNodeFS.Instance.player.Velocity.Length() == 0.0)
        {
            EmitSignal(TransitionSignal, "IdlePlayerState");
        }
    }
}