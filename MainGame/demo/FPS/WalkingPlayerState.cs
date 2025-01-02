using FrontEnd4IdleStrategyFS.Global;
using Godot;

namespace ZeromaXPlayground.demo.FPS;

public partial class WalkingPlayerState : State
{
    // 没法多继承，所以这里先都直接在 C# 代码写逻辑了
    [Export] private AnimationPlayer _animationPlayer;
    [Export] private float _topAnimSpeed = 2.2f;

    public override void Enter()
    {
        _animationPlayer.Play("Walking", -1.0, 1.0f);
        FpsGlobalNodeFS.Instance.player.speed = FpsGlobalNodeFS.Instance.player.speedDefault;
    }

    public override void Update(double delta)
    {
        // GD.Print("WalkingPlayerState Update");
        SetAnimationSpeed(FpsGlobalNodeFS.Instance.player.Velocity.Length());
        if (FpsGlobalNodeFS.Instance.player.Velocity.Length() == 0.0)
        {
            EmitSignal(TransitionSignal, "IdlePlayerState");
        }
    }

    private void SetAnimationSpeed(float spd)
    {
        var alpha = Mathf.Remap(spd, 0.0f, FpsGlobalNodeFS.Instance.player.speedDefault,
            0.0f, 1.0f);
        _animationPlayer.SpeedScale = Mathf.Lerp(0.0f, _topAnimSpeed, alpha);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("sprint") && FpsGlobalNodeFS.Instance.player.IsOnFloor())
        {
            EmitSignal(TransitionSignal, "SprintingPlayerState");
        }
    }
}