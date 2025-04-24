using System;
using Apps.Queries.Contexts;
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
        NodeContext.Instance.RegisterSingleton<ICelestialMotionManager>(this);
        Context.RegisterToHolder<ICelestialMotionManager>(this);
    }

    public NodeEvent NodeEvent { get; } = new(process: true);

    private bool _ready;

    public override void _Ready()
    {
        InitOnReadyNodes();
        _ready = true;
    }

    public override void _ExitTree() => NodeContext.Instance.DestroySingleton<ICelestialMotionManager>();

    public override void _Process(double delta)
    {
        if (!_ready) return;
        UpdateStellarRotation((float)delta);
    }

    public event Action<float>? SatelliteRadiusRatioChanged;
    public event Action<float>? SatelliteDistRatioChanged;

    [Export(PropertyHint.Range, "-100.0, 100.0")]
    public float RotationTimeFactor = 1f;

    [ExportGroup("行星恒星设置")]
    [ExportToolButton("切换恒星运行状态", Icon = "DirectionalLight3D")]
    public Callable StarMoveStatus => Callable.From(ToggleStarMoveStatus);

    private void ToggleStarMoveStatus()
    {
        if (PlanetRevolution)
        {
            PlanetRevolution = false;
            _sunRevolution!.RotationDegrees = Vector3.Up * 180f;
            RenderingServer.GlobalShaderParameterSet(GlobalShaderParam.DirToSun, _sunMesh!.GlobalPosition.Normalized());
        }
        else
            PlanetRevolution = true;
    }

    [ExportToolButton("切换行星运行状态", Icon = "WorldEnvironment")]
    public Callable PlanetMoveStatus => Callable.From(TogglePlanetMoveStatus);

    private void TogglePlanetMoveStatus()
    {
        if (PlanetRotation)
        {
            PlanetRotation = false;
            _planetAxis!.Rotation = Vector3.Zero;
        }
        else
            PlanetRotation = true;
    }

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

    [Export] public bool PlanetRevolution { get; set; } = true; // 行星公转

    [Export(PropertyHint.Range, "-360, 360, degrees")]
    public float PlanetRevolutionSpeed { get; set; } = 1f; // 行星公转速度（每秒转的度数）

    [Export] public bool PlanetRotation { get; set; } = true; // 行星自转

    [Export(PropertyHint.Range, "-360, 360, degrees")]
    public float PlanetRotationSpeed { get; set; } = 10f; // 行星自转速度（每秒转的度数）

    [ExportGroup("卫星设置")]
    [ExportToolButton("切换卫星运行状态", Icon = "CSGSphere3D")]
    public Callable SatelliteMoveStatus => Callable.From(ToggleSatelliteMoveStatus);

    private void ToggleSatelliteMoveStatus()
    {
        if (SatelliteRevolution || SatelliteRotation)
        {
            SatelliteRevolution = false;
            SatelliteRotation = false;
            _lunarRevolution!.RotationDegrees = Vector3.Up * 180f;
            _moonAxis!.Rotation = Vector3.Zero;
        }
        else
        {
            SatelliteRevolution = true;
            SatelliteRotation = true;
        }
    }

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

    [Export] public bool SatelliteRevolution { get; set; } = true; // 卫星公转

    [Export(PropertyHint.Range, "-360, 360, degrees")]
    public float SatelliteRevolutionSpeed { get; set; } = 30f; // 卫星公转速度（每秒转的度数）

    [Export] public bool SatelliteRotation { get; set; } = true; // 卫星自转

    [Export(PropertyHint.Range, "-360, 360, degrees")]
    public float SatelliteRotationSpeed { get; set; } // 卫星自转速度（每秒转的度数）

    #region on-ready 节点

    private WorldEnvironment? _worldEnvironment;

    // 天体公转自转
    private Node3D? _eclipticPlane;
    private Node3D? _sunRevolution;
    private Node3D? _planetAxis;
    private Node3D? _lunarOrbitPlane;
    private Node3D? _lunarRevolution;
    public Node3D? LunarDist { get; private set; }
    private Node3D? _lunarObliquity;
    private Node3D? _moonAxis;
    public MeshInstance3D? MoonMesh { get; private set; }
    private MeshInstance3D? _sunMesh;

    private void InitOnReadyNodes()
    {
        _worldEnvironment = GetNode<WorldEnvironment>("%WorldEnvironment");
        UpdateGalaxySkyRotation();
        // 天体公转自转
        _eclipticPlane = GetNode<Node3D>("%EclipticPlane");
        UpdateEclipticPlaneRotation();
        _sunRevolution = GetNode<Node3D>("%SunRevolution");
        _planetAxis = GetNode<Node3D>("%PlanetAxis");
        _lunarOrbitPlane = GetNode<Node3D>("%LunarOrbitPlane");
        UpdateLunarOrbitPlaneRotation();
        _lunarRevolution = GetNode<Node3D>("%LunarRevolution");
        LunarDist = GetNode<Node3D>("%LunarDist");
        _lunarObliquity = GetNode<Node3D>("%LunarObliquity");
        UpdateLunarObliquityRotation();
        _moonAxis = GetNode<Node3D>("%MoonAxis");
        MoonMesh = GetNode<MeshInstance3D>("%MoonMesh");
        _sunMesh = GetNode<MeshInstance3D>("%SunMesh");
        RenderingServer.GlobalShaderParameterSet(GlobalShaderParam.DirToSun, _sunMesh.GlobalPosition.Normalized());
    }

    #endregion

    private void UpdateLunarOrbitPlaneRotation() =>
        _lunarOrbitPlane!.RotationDegrees = Vector3.Right * SatelliteOrbitInclination;

    private void UpdateEclipticPlaneRotation() => _eclipticPlane!.RotationDegrees = Vector3.Right * PlanetObliquity;

    private void UpdateLunarObliquityRotation() =>
        _lunarObliquity!.RotationDegrees = Vector3.Right * SatelliteObliquity;

    private void UpdateGalaxySkyRotation() =>
        _worldEnvironment!.Environment.SkyRotation =
            Vector3.Right * Mathf.DegToRad(PlanetObliquity - EclipticInclinationToGalactic);

    // 更新天体旋转
    private void UpdateStellarRotation(float delta)
    {
        if (PlanetRevolution || PlanetRotation)
        {
            RenderingServer.GlobalShaderParameterSet(GlobalShaderParam.DirToSun,
                _planetAxis!.ToLocal(_sunMesh!.GlobalPosition.Normalized()));
            // 行星公转
            if (PlanetRevolution)
                _sunRevolution!.RotationDegrees = RotationTimeFactor * Vector3.Up * Mathf.Wrap(
                    _sunRevolution.RotationDegrees.Y + PlanetRevolutionSpeed * delta, 0f, 360f);
            // 行星自转
            if (PlanetRotation)
            {
                _planetAxis.RotationDegrees = RotationTimeFactor * Vector3.Up * Mathf.Wrap(
                    _planetAxis.RotationDegrees.Y + PlanetRotationSpeed * delta, 0f, 360f);
                RenderingServer.GlobalShaderParameterSet(GlobalShaderParam.InvPlanetMatrix,
                    _planetAxis.Transform.Inverse());
            }
        }

        // 卫星公转
        if (SatelliteRevolution)
            _lunarRevolution!.RotationDegrees = RotationTimeFactor * Vector3.Up * Mathf.Wrap(
                _lunarRevolution.RotationDegrees.Y + SatelliteRevolutionSpeed * delta, 0f, 360f);
        // 卫星自转
        if (SatelliteRotation)
            _moonAxis!.RotationDegrees = RotationTimeFactor * Vector3.Up * Mathf.Wrap(
                _moonAxis.RotationDegrees.Y + SatelliteRotationSpeed * delta, 0f, 360f);
    }
}