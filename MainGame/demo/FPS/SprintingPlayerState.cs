using FrontEnd4IdleStrategyFS.Global;
using Godot;

namespace ZeromaXPlayground.demo.FPS;

public partial class SprintingPlayerState : State
{
    // 没法多继承，所以这里先都直接在 C# 代码写逻辑了
    [Export] private AnimationPlayer _animationPlayer;
    [Export] private float _topAnimSpeed = 1.6f;

    public override void Enter()
    {
        _animationPlayer.Play("Sprinting", 0.5, 1.0f);
        FpsGlobalNodeFS.Instance.player.speed = FpsGlobalNodeFS.Instance.player.speedSprinting;
    }

    public override void Update(double delta)
    {
        SetAnimationSpeed(FpsGlobalNodeFS.Instance.player.Velocity.Length());
    }

    private void SetAnimationSpeed(float spd)
    {
        var alpha = Mathf.Remap(spd, 0.0f, FpsGlobalNodeFS.Instance.player.speedSprinting,
            0.0f, 1.0f);
        _animationPlayer.SpeedScale = Mathf.Lerp(0.0f, _topAnimSpeed, alpha);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionReleased("sprint"))
        {
            EmitSignal(TransitionSignal, "WalkingPlayerState");
        }
    }
}