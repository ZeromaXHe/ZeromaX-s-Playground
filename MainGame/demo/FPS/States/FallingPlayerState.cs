using FrontEnd4IdleStrategyFS.FPS;
using Godot;

namespace ZeromaXPlayground.demo.FPS.States;

public partial class FallingPlayerState : PlayerMovementState
{
    // 没法多继承，所以这里先都直接在 C# 代码写逻辑了
    [Export] private float _speed = 5.0f;
    [Export] private float _acceleration = 0.1f;
    [Export] private float _deceleration = 0.25f;
    [Export] private float _doubleJumpVelocity = 4.5f;

    // 二段跳
    private bool _doubleJump;

    public override void Enter(StateFS previousState)
    {
        Animation.Pause();
    }

    public override void Exit()
    {
        _doubleJump = false;
    }

    public override void Update(double delta)
    {
        Player.UpdateGravity((float)delta);
        Player.UpdateInput(_speed, _acceleration, _deceleration);
        Player.UpdateVelocity();
        if (Input.IsActionJustPressed("jump") && !_doubleJump)
        {
            _doubleJump = true;
            var velocity = Player.Velocity;
            velocity.Y = _doubleJumpVelocity;
            Player.Velocity = velocity;
        }

        if (Player.IsOnFloor())
        {
            Animation.Play("JumpEnd");
            EmitSignal(TransitionSignal, "IdlePlayerState");
        }
    }
}