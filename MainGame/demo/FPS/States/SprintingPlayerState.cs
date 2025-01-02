using Godot;

namespace ZeromaXPlayground.demo.FPS.States;

public partial class SprintingPlayerState : PlayerMovementState
{
    // 没法多继承，所以这里先都直接在 C# 代码写逻辑了
    [Export] private float _speed = 7.0f;
    [Export] private float _acceleration = 0.1f;
    [Export] private float _deceleration = 0.25f;
    [Export] private float _topAnimSpeed = 1.6f;

    public override void Enter()
    {
        Animation.Play("Sprinting", 0.5, 1.0f);
    }

    public override void Exit()
    {
        Animation.SpeedScale = 1.0f;
    }

    public override void Update(double delta)
    {
        Player.UpdateGravity((float)delta);
        Player.UpdateInput(_speed, _acceleration, _deceleration);
        Player.UpdateVelocity();
        SetAnimationSpeed(Player.Velocity.Length());
        if (Input.IsActionJustReleased("sprint"))
            EmitSignal(TransitionSignal, "WalkingPlayerState");
    }

    private void SetAnimationSpeed(float spd)
    {
        var alpha = Mathf.Remap(spd, 0.0f, _speed, 0.0f, 1.0f);
        Animation.SpeedScale = Mathf.Lerp(0.0f, _topAnimSpeed, alpha);
    }
}