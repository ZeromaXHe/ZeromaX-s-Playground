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

    private Vector2 _mouseMovement;
    public override async void _Ready()
    {
        _weaponMesh = GetNode<MeshInstance3D>("WeaponMesh");
        _weaponMesh2 = GetNode<MeshInstance3D>("WeaponMesh/WeaponMesh2");
        _weaponShadow = GetNode<MeshInstance3D>("WeaponShadow");
        _weaponShadow2 = GetNode<MeshInstance3D>("WeaponShadow/WeaponShadow2");
        await ToSignal(Owner, Node.SignalName.Ready);
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

        if (@event is InputEventMouseMotion e)
            _mouseMovement = e.Relative;
    }

    public override void _PhysicsProcess(double delta)
    {
        SwayWeapon((float)delta);
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

    private void SwayWeapon(float delta)
    {
        _mouseMovement = _mouseMovement.Clamp(WeaponType.SwayMin, WeaponType.SwayMax);
        var position = _weaponMesh.Position;
        position.X = Mathf.Lerp(position.X,
            WeaponType.Position.X - _mouseMovement.X * WeaponType.SwayAmountPosition * delta,
            WeaponType.SwaySpeedPosition);
        position.Y = Mathf.Lerp(position.Y,
            WeaponType.Position.Y + _mouseMovement.Y * WeaponType.SwayAmountPosition * delta,
            WeaponType.SwaySpeedPosition);
        _weaponMesh.Position = position;
        
        var rotationDegrees = _weaponMesh.RotationDegrees;
        rotationDegrees.Y = Mathf.Lerp(rotationDegrees.Y,
            WeaponType.Rotation.Y + _mouseMovement.X * WeaponType.SwayAmountRotation * delta,
            WeaponType.SwaySpeedRotation);
        rotationDegrees.X = Mathf.Lerp(rotationDegrees.X,
            WeaponType.Rotation.X - _mouseMovement.Y * WeaponType.SwayAmountRotation * delta,
            WeaponType.SwaySpeedRotation);
        _weaponMesh.RotationDegrees = rotationDegrees;
    }
}