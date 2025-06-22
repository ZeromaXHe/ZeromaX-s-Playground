using System;
using Godot;
using TO.Abstractions.Views.Planets;
using TO.Presenters.Views.Planets;

namespace TerraObserver.Scenes.Planets.Views;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-05 13:14:39
[Tool]
public partial class CelestialMotion : CelestialMotionFS, ICelestialMotion
{
    #region 生命周期

    // 需要忽略 IDE 省略 partial、_Ready 等的提示，必须保留它们
    public override void _Ready() => base._Ready();
    public override void _Process(double delta) => base._Process(delta);

    #endregion

    #region 事件

    public event Action? SatelliteRadiusRatioChanged;
    public override void EmitSatelliteRadiusRatioChanged() => SatelliteRadiusRatioChanged?.Invoke();
    public event Action? SatelliteDistRatioChanged;
    public override void EmitSatelliteDistRatioChanged() => SatelliteDistRatioChanged?.Invoke();

    #endregion

    #region Export 属性

    [Export(PropertyHint.Range, "-100.0, 100.0")]
    public override float RotationTimeFactor { get; set; } = 1f;

    [Export] public override Node3D? Sun { get; set; }
    [Export] public override Node3D? Moon { get; set; }


    [ExportGroup("天体运动开关")]
    // 行星公转
    [Export]
    public override bool PlanetRevolution { get; set; } = true;

    // 行星自转
    [Export] public override bool PlanetRotation { get; set; } = true;

    // 卫星公转
    [Export] public override bool SatelliteRevolution { get; set; } = true;

    // 卫星自转
    [Export] public override bool SatelliteRotation { get; set; } = true;

    [ExportGroup("行星恒星设置")]
    [ExportToolButton("切换恒星运行状态", Icon = "DirectionalLight3D")]
    public override Callable StarMoveStatus => Callable.From(ToggleStarMoveStatus);

    [ExportToolButton("切换行星运行状态", Icon = "WorldEnvironment")]
    public override Callable PlanetMoveStatus => Callable.From(TogglePlanetMoveStatus);

    // 黄道面相对银河系的银道面倾角
    // 相关术语：银道面 Galactic Plane
    [Export(PropertyHint.Range, "0, 180, degrees")]
    public override float EclipticInclinationToGalactic
    {
        get => _eclipticInclinationToGalactic;
        set
        {
            _eclipticInclinationToGalactic = value;
            EclipticInclinationToGalacticSetter();
        }
    }

    private float _eclipticInclinationToGalactic = 60f;

    // 行星倾角（= 黄赤交角 obliquity of the ecliptic = 23.44°）
    // 相关术语：黄道面 Ecliptic Plane，赤道面 Earth Equatorial Plane
    [Export(PropertyHint.Range, "0, 180, degrees")]
    public override float PlanetObliquity
    {
        get => _planetObliquity;
        set
        {
            _planetObliquity = value;
            PlanetObliquitySetter();
        }
    }

    private float _planetObliquity = 23.44f;

    [Export(PropertyHint.Range, "-360, 360, degrees")]
    public override float PlanetRevolutionSpeed { get; set; } = 1f; // 行星公转速度（每秒转的度数）

    [Export(PropertyHint.Range, "-360, 360, degrees")]
    public override float PlanetRotationSpeed { get; set; } = 10f; // 行星自转速度（每秒转的度数）

    [ExportGroup("卫星设置")]
    [ExportToolButton("切换卫星运行状态", Icon = "CSGSphere3D")]
    public override Callable SatelliteMoveStatus => Callable.From(ToggleSatelliteMoveStatus);

    // 卫星和行星的半径比
    [Export(PropertyHint.Range, "0, 1")]
    public override float SatelliteRadiusRatio
    {
        get => _satelliteRadiusRatio;
        set
        {
            _satelliteRadiusRatio = value;
            SatelliteRadiusRatioSetter();
        }
    }

    private float _satelliteRadiusRatio = 0.273f;

    // 卫星距离（轨道半径）和行星的半径比
    // 如果大于了地日距离（行星轨道半径）的话，会被截断到小于地日距离的轨道
    // 同样也会控制大于地球半径加卫星半径
    [Export(PropertyHint.Range, "0, 100.0")]
    public override float SatelliteDistRatio
    {
        get => _satelliteDistRatio;
        set
        {
            _satelliteDistRatio = value;
            SatelliteDistRatioSetter();
        }
    }

    private float _satelliteDistRatio = 7.5f;

    // 卫星倾角
    // 相关术语：月球轨道面（白道面）Lunar Orbit Plane，月球赤道面 Lunar Equatorial Plane，
    // 黄白交角 obliquity of the moon path = 月球轨道倾角 Lunar Orbital Inclination = 5.14°
    // 相对黄道的月球倾角 Lunar Obliquity to Ecliptic = 1.54°
    [Export(PropertyHint.Range, "0, 180, degrees")]
    public override float SatelliteObliquity
    {
        get => _satelliteObliquity;
        set
        {
            _satelliteObliquity = value;
            SatelliteObliquitySetter();
        }
    }

    private float _satelliteObliquity = 6.68f;

    // 卫星轨道倾角
    // 相关术语：黄白交角 obliquity of the moon path = 月球轨道倾角 Lunar Orbital Inclination = 5.14°
    [Export(PropertyHint.Range, "0, 180, degrees")]
    public override float SatelliteOrbitInclination
    {
        get => _satelliteOrbitInclination;
        set
        {
            _satelliteOrbitInclination = value;
            SatelliteOrbitInclinationSetter();
        }
    }

    private float _satelliteOrbitInclination = 5.14f;

    [Export(PropertyHint.Range, "-360, 360, degrees")]
    public override float SatelliteRevolutionSpeed { get; set; } = 30f; // 卫星公转速度（每秒转的度数）

    [Export(PropertyHint.Range, "-360, 360, degrees")]
    public override float SatelliteRotationSpeed { get; set; } // 卫星自转速度（每秒转的度数）

    #endregion
}