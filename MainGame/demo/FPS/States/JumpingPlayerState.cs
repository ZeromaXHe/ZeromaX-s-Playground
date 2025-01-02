using FrontEnd4IdleStrategyFS.FPS;
using Godot;

namespace ZeromaXPlayground.demo.FPS.States;

public partial class JumpingPlayerState : PlayerMovementState
{
    // 没法多继承，所以这里先都直接在 C# 代码写逻辑了
    [Export] private float _speed = 6.0f;
    [Export] private float _acceleration = 0.1f;
    [Export] private float _deceleration = 0.25f;
    [Export] private float _jumpVelocity = 4.5f;
    [Export] private float _doubleJumpVelocity = 4.5f;

    [Export(PropertyHint.Range, "0.5, 1.0, 0.01")]
    private float _inputMultiplier = 0.85f;
    // 二段跳
    private bool _doubleJump;

    public override void Enter(StateFS previousState)
    {
        var velocity = Player.Velocity;
        velocity.Y += _jumpVelocity;
        Player.Velocity = velocity;
        Animation.Play("JumpStart");
    }

    public override void Exit()
    {
        _doubleJump = false;
    }

    public override void Update(double delta)
    {
        Player.UpdateGravity((float)delta);
        Player.UpdateInput(_speed * _inputMultiplier, _acceleration, _deceleration);
        Player.UpdateVelocity();
        if (Input.IsActionJustPressed("jump") && !_doubleJump)
        {
            _doubleJump = true;
            var velocity = Player.Velocity;
            velocity.Y += _doubleJumpVelocity;
            Player.Velocity = velocity;
        }
        // 跳跃强度逻辑（相当于飞到空中再控制垂直向上速度减半……）
        if (Input.IsActionJustReleased("jump") && Player.Velocity.Y > 0)
        {
            var velocity = Player.Velocity;
            velocity.Y /= 2.0f;
            Player.Velocity = velocity;
        }
        if (Player.IsOnFloor())
        {
            Animation.Play("JumpEnd");
            EmitSignal(TransitionSignal, "IdlePlayerState");
        }
    }
}