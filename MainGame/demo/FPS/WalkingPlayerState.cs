using Godot;

namespace ZeromaXPlayground.demo.FPS;

public partial class WalkingPlayerState : PlayerMovementState
{
    // 没法多继承，所以这里先都直接在 C# 代码写逻辑了
    [Export] private float _topAnimSpeed = 2.2f;
    [Export] private float _speed = 5.0f;
    [Export] private float _acceleration = 0.1f;
    [Export] private float _deceleration = 0.25f;

    public override void Enter()
    {
        AnimationPlayer.Play("Walking", -1.0, 1.0f);
    }

    public override void Update(double delta)
    {
        Player.UpdateGravity((float)delta);
        Player.UpdateInput(_speed, _acceleration, _deceleration);
        Player.UpdateVelocity();
        SetAnimationSpeed(Player.Velocity.Length());
        if (Player.Velocity.Length() == 0.0)
            EmitSignal(TransitionSignal, "IdlePlayerState");
        if (Input.IsActionPressed("sprint") && Player.IsOnFloor())
            EmitSignal(TransitionSignal, "SprintingPlayerState");
    }

    private void SetAnimationSpeed(float spd)
    {
        var alpha = Mathf.Remap(spd, 0.0f, _speed, 0.0f, 1.0f);
        AnimationPlayer.SpeedScale = Mathf.Lerp(0.0f, _topAnimSpeed, alpha);
    }
}