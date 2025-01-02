using Godot;

namespace ZeromaXPlayground.demo.FPS.States;

public partial class CrouchingPlayerState : PlayerMovementState
{
    // 没法多继承，所以这里先都直接在 C# 代码写逻辑了
    [Export] private float _speed = 3.0f;
    [Export] private float _acceleration = 0.1f;
    [Export] private float _deceleration = 0.25f;

    [Export(PropertyHint.Range, "1, 6, 0.1")]
    private float _crouchingSpeed = 4.0f;

    private ShapeCast3D _crouchShapeCast;

    public override void _Ready()
    {
        base._Ready();
        _crouchShapeCast = GetNode<ShapeCast3D>("%ShapeCast3D");
    }

    public override void Enter()
    {
        Animation.Play("Crouch", -1.0, _crouchingSpeed);
    }

    public override void Exit()
    {
    }

    public override void Update(double delta)
    {
        Player.UpdateGravity((float)delta);
        Player.UpdateInput(_speed, _acceleration, _deceleration);
        Player.UpdateVelocity();
        if (Input.IsActionJustReleased("crouch"))
            Uncrouch();
    }

    private async void Uncrouch()
    {
        if (!_crouchShapeCast.IsColliding() && !Input.IsActionPressed("crouch"))
        {
            Animation.Play("Crouch", -1.0, -_crouchingSpeed * 1.5f, true);
            if (Animation.IsPlaying())
                await ToSignal(Animation, AnimationMixer.SignalName.AnimationFinished);
            EmitSignal(TransitionSignal, "IdlePlayerState");
        }
        else if (_crouchShapeCast.IsColliding())
        {
            await ToSignal(GetTree().CreateTimer(0.1f), Timer.SignalName.Timeout);
            Uncrouch();
        }
    }
}