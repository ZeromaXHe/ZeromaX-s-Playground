using System;
using Commons.Constants;
using Contexts;
using Godot;
using GodotNodes.Abstractions.Addition;
using Nodes.Abstractions.Planets;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Nodes.Planets;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-03-26 23:10:38
[Tool]
public partial class CelestialMotionManager : Node3D, ICelestialMotionManager
{
    public CelestialMotionManager()
    {
        Context.RegisterToHolder<ICelestialMotionManager>(this);
    }

    public NodeEvent NodeEvent { get; } = new(process: true);

    private bool _ready;

    public override void _Ready()
    {
        InitOnReadyNodes();
        _ready = true;
    }

    public override void _Process(double delta) => NodeEvent.EmitProcessed(delta);

    public event Action<float>? SatelliteRadiusRatioChanged;
    public event Action<float>? SatelliteDistRatioChanged;
    public event Action? StarMoveStatusToggled;
    public event Action? PlanetMoveStatusToggled;
    public event Action? SatelliteMoveStatusToggled;

    [Export(PropertyHint.Range, "-100.0, 100.0")]
    public float RotationTimeFactor { get; set; } = 1f;

    [ExportGroup("行星恒星设置")]
    [ExportToolButton("切换恒星运行状态", Icon = "DirectionalLight3D")]
    public Callable StarMoveStatus => Callable.From(ToggleStarMoveStatus);

    private void ToggleStarMoveStatus() => StarMoveStatusToggled?.Invoke();

    [ExportToolButton("切换行星运行状态", Icon = "WorldEnvironment")]
    public Callable PlanetMoveStatus => Callable.From(TogglePlanetMoveStatus);

    private void TogglePlanetMoveStatus() => PlanetMoveStatusToggled?.Invoke();

    private float _eclipticInclinationToGalactic = 60f;

    // 黄道面相对银河系的银道面倾角
    // 相关术语：银道面 Galactic Plane
    [Export(PropertyHint.Range, "0, 180, degrees")]
    public float EclipticInclinationToGalactic
    {
        get => _eclipticInclinationToGalactic;
        set
        {
            _eclipticInclinationToGalactic = value;
            if (_ready)
                UpdateGalaxySkyRotation();
        }
    }

    private float _planetObliquity = 23.44f;

    // 行星倾角（= 黄赤交角 obliquity of the ecliptic = 23.44°）
    // 相关术语：黄道面 Ecliptic Plane，赤道面 Earth Equatorial Plane
    [Export(PropertyHint.Range, "0, 180, degrees")]
    public float PlanetObliquity
    {
        get => _planetObliquity;
        set
        {
            _planetObliquity = value;
            if (_ready)
            {
                UpdateGalaxySkyRotation();
                UpdateEclipticPlaneRotation();
            }
        }
    }

    [Export(PropertyHint.Range, "-360, 360, degrees")]
    public float PlanetRevolutionSpeed { get; set; } = 1f; // 行星公转速度（每秒转的度数）

    [Export(PropertyHint.Range, "-360, 360, degrees")]
    public float PlanetRotationSpeed { get; set; } = 10f; // 行星自转速度（每秒转的度数）

    [ExportGroup("卫星设置")]
    [ExportToolButton("切换卫星运行状态", Icon = "CSGSphere3D")]
    public Callable SatelliteMoveStatus => Callable.From(ToggleSatelliteMoveStatus);

    private void ToggleSatelliteMoveStatus() => SatelliteMoveStatusToggled?.Invoke();

    private float _satelliteRadiusRatio = 0.273f;

    // 卫星和行星的半径比
    [Export(PropertyHint.Range, "0, 1")]
    public float SatelliteRadiusRatio
    {
        get => _satelliteRadiusRatio;
        set
        {
            _satelliteRadiusRatio = value;
            if (_ready)
                SatelliteRadiusRatioChanged?.Invoke(value);
        }
    }

    private float _satelliteDistRatio = 7.5f;

    // 卫星距离（轨道半径）和行星的半径比
    // 如果大于了地日距离（行星轨道半径）的话，会被截断到小于地日距离的轨道
    // 同样也会控制大于地球半径加卫星半径
    [Export(PropertyHint.Range, "0, 100.0")]
    public float SatelliteDistRatio
    {
        get => _satelliteDistRatio;
        set
        {
            _satelliteDistRatio = value;
            if (_ready)
                SatelliteDistRatioChanged?.Invoke(value);
        }
    }

    private float _satelliteObliquity = 6.68f;

    // 卫星倾角
    // 相关术语：月球轨道面（白道面）Lunar Orbit Plane，月球赤道面 Lunar Equatorial Plane，
    // 黄白交角 obliquity of the moon path = 月球轨道倾角 Lunar Orbital Inclination = 5.14°
    // 相对黄道的月球倾角 Lunar Obliquity to Ecliptic = 1.54°
    [Export(PropertyHint.Range, "0, 180, degrees")]
    public float SatelliteObliquity
    {
        get => _satelliteObliquity;
        set
        {
            _satelliteObliquity = value;
            if (_ready)
                UpdateLunarObliquityRotation();
        }
    }

    private float _satelliteOrbitInclination = 5.14f;

    // 卫星轨道倾角
    // 相关术语：黄白交角 obliquity of the moon path = 月球轨道倾角 Lunar Orbital Inclination = 5.14°
    [Export(PropertyHint.Range, "0, 180, degrees")]
    public float SatelliteOrbitInclination
    {
        get => _satelliteOrbitInclination;
        set
        {
            _satelliteOrbitInclination = value;
            if (_ready)
                UpdateLunarOrbitPlaneRotation();
        }
    }

    [Export(PropertyHint.Range, "-360, 360, degrees")]
    public float SatelliteRevolutionSpeed { get; set; } = 30f; // 卫星公转速度（每秒转的度数）

    [Export(PropertyHint.Range, "-360, 360, degrees")]
    public float SatelliteRotationSpeed { get; set; } // 卫星自转速度（每秒转的度数）

    #region on-ready 节点

    private WorldEnvironment? _worldEnvironment;

    // 天体公转自转
    public Node3D? EclipticPlane { get; private set; }
    public Node3D? SunRevolution { get; private set; }
    public Node3D? PlanetAxis { get; private set; }
    public Node3D? LunarOrbitPlane { get; private set; }
    public Node3D? LunarRevolution { get; private set; }
    public Node3D? LunarDist { get; private set; }
    public Node3D? LunarObliquity { get; private set; }
    public Node3D? MoonAxis { get; private set; }
    public MeshInstance3D? MoonMesh { get; private set; }
    public MeshInstance3D? SunMesh { get; private set; }

    private void InitOnReadyNodes()
    {
        _worldEnvironment = GetNode<WorldEnvironment>("%WorldEnvironment");
        UpdateGalaxySkyRotation();
        // 天体公转自转
        EclipticPlane = GetNode<Node3D>("%EclipticPlane");
        UpdateEclipticPlaneRotation();
        SunRevolution = GetNode<Node3D>("%SunRevolution");
        PlanetAxis = GetNode<Node3D>("%PlanetAxis");
        LunarOrbitPlane = GetNode<Node3D>("%LunarOrbitPlane");
        UpdateLunarOrbitPlaneRotation();
        LunarRevolution = GetNode<Node3D>("%LunarRevolution");
        LunarDist = GetNode<Node3D>("%LunarDist");
        LunarObliquity = GetNode<Node3D>("%LunarObliquity");
        UpdateLunarObliquityRotation();
        MoonAxis = GetNode<Node3D>("%MoonAxis");
        MoonMesh = GetNode<MeshInstance3D>("%MoonMesh");
        SunMesh = GetNode<MeshInstance3D>("%SunMesh");
        RenderingServer.GlobalShaderParameterSet(GlobalShaderParam.DirToSun, SunMesh.GlobalPosition.Normalized());
    }

    #endregion

    private void UpdateLunarOrbitPlaneRotation() =>
        LunarOrbitPlane!.RotationDegrees = Vector3.Right * SatelliteOrbitInclination;

    private void UpdateEclipticPlaneRotation() => EclipticPlane!.RotationDegrees = Vector3.Right * PlanetObliquity;

    private void UpdateLunarObliquityRotation() =>
        LunarObliquity!.RotationDegrees = Vector3.Right * SatelliteObliquity;

    private void UpdateGalaxySkyRotation() =>
        _worldEnvironment!.Environment.SkyRotation =
            Vector3.Right * Mathf.DegToRad(PlanetObliquity - EclipticInclinationToGalactic);
}