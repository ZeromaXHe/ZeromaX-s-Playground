using FrontEnd4IdleStrategyFS.Global;
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
            // C#/F# 涉及到编辑器工具在编译时就各种乱七八糟的 bug，烦死了
            // 判空也没有用，编译就是会清空属性，但其实加载已保存场景又正常……
            if (_weaponType?.Mesh != null && IsNodeReady() && Engine.IsEditorHint())
                LoadWeapon();
        }
    }

    [Export] public NoiseTexture2D SwayNoise;
    [Export] public float SwaySpeed = 1.2f;

    private bool _reset;

    [Export]
    public bool Reset
    {
        get => _reset;
        set
        {
            _reset = value;
            if (IsNodeReady() && Engine.IsEditorHint())
                LoadWeapon();
        }
    }

    private MeshInstance3D _weaponMesh;
    private MeshInstance3D _weaponMesh2;
    private MeshInstance3D _weaponShadow;
    private MeshInstance3D _weaponShadow2;

    private Vector2 _mouseMovement;
    private float _randomSwayX = 0f;
    private float _randomSwayY = 0f;
    private float _randomSwayAmount = 0f;
    private float _time = 0f;
    private float _idleSwayAdjustment = 0f;
    private float _idleSwayRotationStrength = 0f;

    public override async void _Ready()
    {
        _weaponMesh = GetNode<MeshInstance3D>("WeaponMesh");
        _weaponMesh2 = GetNode<MeshInstance3D>("WeaponMesh/WeaponMesh2");
        _weaponShadow = GetNode<MeshInstance3D>("WeaponShadow");
        _weaponShadow2 = GetNode<MeshInstance3D>("WeaponShadow/WeaponShadow2");
        if (Owner != null && Owner != this) // 傻逼教程不加这个编辑器里的 Weapon 场景本身直接卡死，自己等自己死循环
            await ToSignal(Owner, Node.SignalName.Ready);
        GD.Print("Weapon Owner _Ready");
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
        if (SwayNoise?.Noise != null) // 防止编译过程中报错，C# 这点很烦
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

        _idleSwayAdjustment = WeaponType.IdleSwayAdjustment;
        _idleSwayRotationStrength = WeaponType.IdleSwayRotationStrength;
        _randomSwayAmount = WeaponType.RandomSwayAmount;
    }

    private void SwayWeapon(float delta)
    {
        var swayRandom = GetSwayNoise();
        var swayRandomAdjusted = swayRandom * _idleSwayAdjustment;

        _time += delta * (SwaySpeed + swayRandom);
        _randomSwayX = Mathf.Sin(_time * 1.5f + swayRandomAdjusted) / _randomSwayAmount;
        _randomSwayY = Mathf.Sin(_time - swayRandomAdjusted) / _randomSwayAmount;

        _mouseMovement = _mouseMovement.Clamp(WeaponType.SwayMin, WeaponType.SwayMax);
        var position = _weaponMesh.Position;
        position.X = Mathf.Lerp(position.X,
            WeaponType.Position.X - (_mouseMovement.X * WeaponType.SwayAmountPosition + _randomSwayX) * delta,
            WeaponType.SwaySpeedPosition);
        position.Y = Mathf.Lerp(position.Y,
            WeaponType.Position.Y + (_mouseMovement.Y * WeaponType.SwayAmountPosition + _randomSwayY) * delta,
            WeaponType.SwaySpeedPosition);
        _weaponMesh.Position = position;

        var rotationDegrees = _weaponMesh.RotationDegrees;
        rotationDegrees.Y = Mathf.Lerp(rotationDegrees.Y,
            WeaponType.Rotation.Y +
            (_mouseMovement.X * WeaponType.SwayAmountRotation + _randomSwayY * _idleSwayRotationStrength) * delta,
            WeaponType.SwaySpeedRotation);
        rotationDegrees.X = Mathf.Lerp(rotationDegrees.X,
            WeaponType.Rotation.X -
            (_mouseMovement.Y * WeaponType.SwayAmountRotation + _randomSwayX * _idleSwayRotationStrength) * delta,
            WeaponType.SwaySpeedRotation);
        _weaponMesh.RotationDegrees = rotationDegrees;
    }

    private float GetSwayNoise()
    {
        var playerPosition = Vector3.Zero;
        if (!Engine.IsEditorHint())
            playerPosition = FpsGlobalNodeFS.Instance.player.GlobalPosition;
        var noiseLocation = SwayNoise.Noise.GetNoise2D(playerPosition.X, playerPosition.Y);
        return noiseLocation;
    }
}