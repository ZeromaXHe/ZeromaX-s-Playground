using FrontEnd4IdleStrategyFS.FPS;
using Godot;

namespace ZeromaXPlayground.demo.FPS.States;

public partial class SlidingPlayerState : PlayerMovementState
{
    // 没法多继承，所以这里先都直接在 C# 代码写逻辑了
    [Export] private float _speed = 6.0f;
    [Export] private float _acceleration = 0.1f;
    [Export] private float _deceleration = 0.25f;
    [Export] private float _tileAmount = 0.09f;

    [Export(PropertyHint.Range, "1, 6, 0.1")]
    private float _slideAnimSpeed = 4.0f;

    private ShapeCast3D _crouchShapeCast;

    public override void _Ready()
    {
        base._Ready();
        _crouchShapeCast = GetNode<ShapeCast3D>("%ShapeCast3D");
    }

    public override void Enter(StateFS previousState)
    {
        SetTilt(Player.currentRotation);
        // PlayerStateMachine/SlidingPlayerState:_speed
        // GD.Print($"track 5: {Animation.GetAnimation("Sliding").TrackGetPath(5)}");
        Animation.GetAnimation("Sliding").TrackSetKeyValue(5, 0, Player.Velocity.Length());
        Animation.SpeedScale = 1.0f;
        Animation.Play("Sliding", -1.0, _slideAnimSpeed);
    }

    public override void Exit()
    {
    }

    public override void Update(double delta)
    {
        Player.UpdateGravity((float)delta);
        Player.UpdateVelocity();
    }

    private void SetTilt(float playerRotation)
    {
        var tilt = Vector3.Zero;
        tilt.Z = Mathf.Clamp(_tileAmount * playerRotation, -0.1f, 0.1f);
        if (tilt.Z == 0f)
            tilt.Z = 0.05f;
        // CameraController:rotation
        // GD.Print($"track 3: {Animation.GetAnimation("Sliding").TrackGetPath(3)}");
        Animation.GetAnimation("Sliding").TrackSetKeyValue(3, 1, tilt);
        Animation.GetAnimation("Sliding").TrackSetKeyValue(3, 2, tilt);
    }

    private void Finish()
    {
        EmitSignal(TransitionSignal, "CrouchingPlayerState");
    }
}