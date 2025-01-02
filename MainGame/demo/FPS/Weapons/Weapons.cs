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
    [Export] public Vector3 Position2;
    [Export] public Vector3 Rotation2;

    [ExportCategory("Visual Settings")] [Export]
    public Mesh Mesh;

    [Export] public Mesh ShadowMesh;
    [Export] public Mesh Mesh2;
    [Export] public Mesh ShadowMesh2;
    [Export] public bool Shadow;
    [Export] public float DamageAmount;
}