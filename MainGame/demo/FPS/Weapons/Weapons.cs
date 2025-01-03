using Godot;

namespace ZeromaXPlayground.demo.FPS.Weapons;

[Tool]
[GlobalClass]
public partial class Weapons : Resource
{
    [Export] public StringName Name;

    [ExportCategory("Weapon Orientation")] [Export]
    public Vector3 Position;

    [Export] public Vector3 Rotation;
    [Export] public Vector3 Scale1;
    [Export] public Vector3 Position2;
    [Export] public Vector3 Rotation2;
    [Export] public Vector3 Scale2;

    [ExportCategory("Weapon Sway")] [Export]
    public Vector2 SwayMin = new(-20.0f, -20f);

    [Export] public Vector2 SwayMax = new(20.0f, 20f);

    [Export(PropertyHint.Range, "0.0, 0.2, 0.01")]
    public float SwaySpeedPosition = 0.07f;

    [Export(PropertyHint.Range, "0.0, 0.2, 0.01")]
    public float SwaySpeedRotation = 0.1f;

    [Export(PropertyHint.Range, "0.0, 0.25, 0.01")]
    public float SwayAmountPosition = 0.1f;

    [Export(PropertyHint.Range, "0.0, 50, 0.1")]
    public float SwayAmountRotation = 30f;

    [Export] public float IdleSwayAdjustment = 10f;
    [Export] public float IdleSwayRotationStrength = 300f;

    [Export(PropertyHint.Range, "0.1, 10, 0.1")]
    public float RandomSwayAmount = 5f;

    [ExportCategory("Visual Settings")] [Export]
    public Mesh Mesh;

    [Export] public Mesh ShadowMesh;
    [Export] public Mesh Mesh2;
    [Export] public Mesh ShadowMesh2;
    [Export] public bool Shadow;
    [Export] public float DamageAmount;
}