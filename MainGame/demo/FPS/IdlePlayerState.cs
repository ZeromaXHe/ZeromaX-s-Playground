using FrontEnd4IdleStrategyFS.Global;
using Godot;

namespace ZeromaXPlayground.demo.FPS;

public partial class IdlePlayerState : State
{
    // 没法多继承，所以这里先都直接在 C# 代码写逻辑了
    [Export] private AnimationPlayer _animationPlayer;

    public override void Enter()
    {
        _animationPlayer.Pause();
    }

    public override void Update(double delta)
    {
        // GD.Print("IdlePlayerState Update");
        if (FpsGlobalNodeFS.Instance.player.Velocity.Length() > 0.0
            && FpsGlobalNodeFS.Instance.player.IsOnFloor())
        {
            EmitSignal(TransitionSignal, "WalkingPlayerState");
        }
    }
}