using Godot;

namespace ZeromaXPlayground.demo.FPS.Weapons;

[Tool]
public partial class Weapon : Node3D
{
    private Weapons _weaponType;

    [Export]
    public Weapons WeaponType
    {
        get => _weaponType;
        set
        {
            _weaponType = value;
            if (IsNodeReady() && Engine.IsEditorHint())
                LoadWeapon();
        }
    }

    private MeshInstance3D _weaponMesh;
    private MeshInstance3D _weaponMesh2;
    private MeshInstance3D _weaponShadow;
    private MeshInstance3D _weaponShadow2;

    public override void _Ready()
    {
        _weaponMesh = GetNode<MeshInstance3D>("WeaponMesh");
        _weaponMesh2 = GetNode<MeshInstance3D>("WeaponMesh/WeaponMesh2");
        _weaponShadow = GetNode<MeshInstance3D>("WeaponShadow");
        _weaponShadow2 = GetNode<MeshInstance3D>("WeaponShadow/WeaponShadow2");
        LoadWeapon();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("weapon1"))
        {
            _weaponType = GD.Load<Weapons>("res://Tres/FPS/Weapons/Crowbar.tres");
            LoadWeapon();
        }

        if (@event.IsActionPressed("weapon2"))
        {
            _weaponType = GD.Load<Weapons>("res://Tres/FPS/Weapons/Crowbar2.tres");
            LoadWeapon();
        }
    }

    private void LoadWeapon()
    {
        _weaponMesh.Mesh = WeaponType.Mesh;
        _weaponShadow.Mesh = WeaponType.ShadowMesh;
        _weaponMesh2.Mesh = WeaponType.Mesh2;
        _weaponShadow2.Mesh = WeaponType.ShadowMesh2;
        _weaponMesh.Position = _weaponShadow.Position = WeaponType.Position;
        _weaponMesh2.Position = _weaponShadow2.Position = WeaponType.Position2;
        _weaponMesh.RotationDegrees = _weaponShadow.RotationDegrees = WeaponType.Rotation;
        _weaponMesh2.RotationDegrees = _weaponShadow2.RotationDegrees = WeaponType.Rotation2;
        _weaponShadow.Visible = _weaponShadow2.Visible = WeaponType.Shadow;
    }
}