using Godot;
using System;

public partial class CameraOrbit : Camera3D
{
    [Export] public float OrbitSpeed = 100;
    [Export] public float OrbitRadius = 200;
    [Export] public float Smoothness = 1;
    [Export] public Marker3D Origin;

    private Vector3 _offset;
    private Vector3 _target;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _offset = (Position - Origin.Position).Normalized() * OrbitRadius;
        _target = Origin.Position + _offset;
        Position = _target;
        LookAt(Vector3.Zero, Vector3.Up);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        float yMove = Input.GetAxis("ui_down", "ui_up");
        float xMove = Input.GetAxis("ui_left", "ui_right");

        if (Input.IsKeyPressed(Key.Minus) || Input.IsMouseButtonPressed(MouseButton.WheelDown))
        {
            OrbitRadius -= 1f;
        }
        else if (Input.IsKeyPressed(Key.Equal) || Input.IsMouseButtonPressed(MouseButton.WheelUp))
        {
            OrbitRadius += 1f;
        }

        _offset += Basis.X * xMove * (float)delta * OrbitSpeed +
                   Basis.Y * yMove * (float)delta * OrbitSpeed;
        _offset = _offset.Normalized() * OrbitRadius;
        _target = Origin.Position + _offset;
        Position = Position.Lerp(_target, (float)delta / Smoothness);
        LookAt(Origin.Position, Basis.Y);


        if (yMove != 0.0 || xMove != 0.0)
        {
            GD.Print($"y: {yMove}, x: {xMove}, _target: {_target}, _offset: {_offset}, Position: {Position}");
        }
    }
}