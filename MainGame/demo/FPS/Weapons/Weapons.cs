using FrontEndToolFS.FPS;
using Godot;

namespace ZeromaXPlayground.demo.FPS.Weapons;

[Tool]
[GlobalClass]
public partial class Weapons : Resource, IWeapons
{
    [Export] public StringName Name { get; set; }


    [ExportCategory("Weapon Orientation")]
    [Export]
    public Vector3 Position { get; set; }

    [Export] public Vector3 Rotation { get; set; }
    [Export] public Vector3 Scale1 { get; set; }
    [Export] public Vector3 Position2 { get; set; }
    [Export] public Vector3 Rotation2 { get; set; }
    [Export] public Vector3 Scale2 { get; set; }

    [ExportCategory("Weapon Sway")]
    [Export]
    public Vector2 SwayMin { get; set; } = new(-20.0f, -20f);

    [Export] public Vector2 SwayMax { get; set; } = new(20.0f, 20f);

    [Export(PropertyHint.Range, "0.0, 0.2, 0.01")]
    public float SwaySpeedPosition { get; set; } = 0.07f;

    [Export(PropertyHint.Range, "0.0, 0.2, 0.01")]
    public float SwaySpeedRotation { get; set; } = 0.1f;

    [Export(PropertyHint.Range, "0.0, 0.25, 0.01")]
    public float SwayAmountPosition { get; set; } = 0.1f;

    [Export(PropertyHint.Range, "0.0, 50, 0.1")]
    public float SwayAmountRotation { get; set; } = 30f;

    [Export] public float IdleSwayAdjustment { get; set; } = 10f;
    [Export] public float IdleSwayRotationStrength { get; set; } = 300f;

    [Export(PropertyHint.Range, "0.1, 10, 0.1")]
    public float RandomSwayAmount { get; set; } = 5f;

    [ExportCategory("Visual Settings")]
    [Export]
    public Mesh Mesh { get; set; }

    [Export] public Mesh ShadowMesh { get; set; }
    [Export] public Mesh Mesh2 { get; set; }
    [Export] public Mesh ShadowMesh2 { get; set; }
    [Export] public bool Shadow { get; set; }
    [Export] public float DamageAmount { get; set; }
}