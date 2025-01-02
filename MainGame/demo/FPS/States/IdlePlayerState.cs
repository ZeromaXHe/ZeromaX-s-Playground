using FrontEnd4IdleStrategyFS.FPS;
using Godot;

namespace ZeromaXPlayground.demo.FPS.States;

public partial class IdlePlayerState : PlayerMovementState
{
    // 没法多继承，所以这里先都直接在 C# 代码写逻辑了
    [Export] private float _speed = 5.0f;
    [Export] private float _acceleration = 0.1f;
    [Export] private float _deceleration = 0.25f;

    public override async void Enter(StateFS previousState)
    {
        if (Animation.IsPlaying() && "JumpEnd".Equals(Animation.CurrentAnimation))
            await ToSignal(Player.AnimationPlayer, AnimationMixer.SignalName.AnimationFinished);
        Animation.Pause();
    }

    public override void Exit()
    {
    }

    public override void Update(double delta)
    {
        // GD.Print("IdlePlayerState Update");
        Player.UpdateGravity((float)delta);
        Player.UpdateInput(_speed, _acceleration, _deceleration);
        Player.UpdateVelocity();
        if (Input.IsActionJustPressed("crouch") && Player.IsOnFloor())
            EmitSignal(TransitionSignal, "CrouchingPlayerState");
        if (Player.Velocity.Length() > 0.0 && Player.IsOnFloor())
            EmitSignal(TransitionSignal, "WalkingPlayerState");
        if (Input.IsActionJustPressed("jump") && Player.IsOnFloor())
            EmitSignal(TransitionSignal, "JumpingPlayerState");
    }
}