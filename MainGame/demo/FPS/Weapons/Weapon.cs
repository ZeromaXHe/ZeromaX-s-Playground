using FrontEnd4IdleStrategyFS.Global;
using FrontEndToolFS.FPS;
using Godot;

namespace ZeromaXPlayground.demo.FPS.Weapons;

[Tool]
public partial class Weapon : WeaponFS
{
    [Export]
    public Weapons WeaponType
    {
        get => weaponType as Weapons;
        set => weaponType = value;
    }

    [Export]
    public NoiseTexture2D SwayNoise
    {
        get => swayNoise;
        set => swayNoise = value;
    }

    [Export]
    public float SwaySpeed
    {
        get => swaySpeed;
        set => swaySpeed = value;
    }

    [Export]
    public bool Reset
    {
        get => reset;
        set => reset = value;
    }

    public override void _Ready()
    {
        base._Ready();
        weapon1 = GD.Load<Weapons>("res://Tres/FPS/Weapons/Crowbar.tres");
        weapon2 = GD.Load<Weapons>("res://Tres/FPS/Weapons/Crowbar2.tres");
        PassPlayerToFs();
    }

    private async void PassPlayerToFs()
    {
        if (Engine.IsEditorHint()) return;
        await ToSignal(Owner, Node.SignalName.Ready);
        player = FpsGlobalNodeFS.Instance.player as CharacterBody3D;
    }
    // 请忽略 IDE 冗余提示，需要保留此处和 partial
    public override void _Input(InputEvent @event) => base._Input(@event);
    public override void _PhysicsProcess(double delta) => base._PhysicsProcess(delta);
}