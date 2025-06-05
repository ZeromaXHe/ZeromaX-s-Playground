using Godot;
using Godot.Abstractions.Extensions.Planets;
using TO.FSharp.Commons.Constants.Shaders;

namespace TerraObserver.Scenes.Planets.Views;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-05 13:14:39
[Tool]
public partial class CelestialMotion : Node3D, ICelestialMotion
{
    #region 依赖

    public IPlanet Planet
    {
        get => _planet;
        set { _planet = value; }
    }

    private IPlanet _planet = null!;

    #endregion

    #region 事件和 Export 属性

    [Export(PropertyHint.Range, "-100.0, 100.0")]
    public float RotationTimeFactor { get; set; } = 1f;

    [Export] public Node3D? Sun { get; set; }
    [Export] public Node3D? Moon { get; set; }


    [ExportGroup("天体运动开关")]
    // 行星公转
    [Export]
    public bool PlanetRevolution { get; set; } = true;

    // 行星自转
    [Export] public bool PlanetRotation { get; set; } = true;

    // 卫星公转
    [Export] public bool SatelliteRevolution { get; set; } = true;

    // 卫星自转
    [Export] public bool SatelliteRotation { get; set; } = true;

    [ExportGroup("行星恒星设置")]
    [ExportToolButton("切换恒星运行状态", Icon = "DirectionalLight3D")]
    public Callable StarMoveStatus => Callable.From(ToggleStarMoveStatus);

    private void ToggleStarMoveStatus()
    {
        if (PlanetRevolution)
        {
            PlanetRevolution = false;
            SunRevolution!.RotationDegrees = Vector3.Up * 180f;
            RenderingServer.GlobalShaderParameterSet(GlobalShaderParam.DirToSun,
                Sun!.GlobalPosition.Normalized());
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
            // 将黄道面和天空盒还原
            EclipticPlane!.RotationDegrees = Vector3.Right * EclipticPlane.RotationDegrees.X;
            _worldEnvironment!.Environment.SkyRotation = Vector3.Right * _worldEnvironment!.Environment.SkyRotation.X;
        }
        else
            PlanetRotation = true;
    }

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

    private float _eclipticInclinationToGalactic = 60f;

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

    private float _planetObliquity = 23.44f;

    [Export(PropertyHint.Range, "-360, 360, degrees")]
    public float PlanetRevolutionSpeed { get; set; } = 1f; // 行星公转速度（每秒转的度数）

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
            LunarRevolution!.RotationDegrees = Vector3.Up * 180f;
            MoonAxis!.Rotation = Vector3.Zero;
        }
        else
        {
            SatelliteRevolution = true;
            SatelliteRotation = true;
        }
    }

    // 卫星和行星的半径比
    [Export(PropertyHint.Range, "0, 1")]
    public float SatelliteRadiusRatio
    {
        get => _satelliteRadiusRatio;
        set
        {
            _satelliteRadiusRatio = value;
            if (_ready)
            {
                var moonMesh = (Moon as MeshInstance3D)?.Mesh as SphereMesh;
                moonMesh?.SetRadius(Planet.Radius * SatelliteRadiusRatio);
                moonMesh?.SetHeight(Planet.Radius * SatelliteRadiusRatio * 2);
            }
        }
    }

    private float _satelliteRadiusRatio = 0.273f;

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
                LunarDist!.Position = Vector3.Back * Mathf.Clamp(Planet.Radius * SatelliteDistRatio,
                    Planet.Radius * (1 + SatelliteRadiusRatio), 800f);
        }
    }

    private float _satelliteDistRatio = 7.5f;

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

    private float _satelliteObliquity = 6.68f;

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

    private float _satelliteOrbitInclination = 5.14f;

    [Export(PropertyHint.Range, "-360, 360, degrees")]
    public float SatelliteRevolutionSpeed { get; set; } = 30f; // 卫星公转速度（每秒转的度数）

    [Export(PropertyHint.Range, "-360, 360, degrees")]
    public float SatelliteRotationSpeed { get; set; } // 卫星自转速度（每秒转的度数）

    #endregion

    #region 内部变量、属性

    private WorldEnvironment? _worldEnvironment;

    // 天体公转自转
    private Node3D? EclipticPlane { get; set; }
    private Node3D? SunRevolution { get; set; }
    private Node3D? LunarOrbitPlane { get; set; }
    private Node3D? LunarRevolution { get; set; }
    private Node3D? LunarDist { get; set; }
    private Node3D? LunarObliquity { get; set; }
    private Node3D? MoonAxis { get; set; }
    private RemoteTransform3D? MoonTransform { get; set; }
    private RemoteTransform3D? SunTransform { get; set; }

    #endregion

    #region 生命周期

    private bool _ready;

    public override void _Ready()
    {
        _worldEnvironment = GetNode<WorldEnvironment>("%WorldEnvironment");
        UpdateGalaxySkyRotation();
        // 天体公转自转
        EclipticPlane = GetNode<Node3D>("%EclipticPlane");
        UpdateEclipticPlaneRotation();
        SunRevolution = GetNode<Node3D>("%SunRevolution");
        LunarOrbitPlane = GetNode<Node3D>("%LunarOrbitPlane");
        UpdateLunarOrbitPlaneRotation();
        LunarRevolution = GetNode<Node3D>("%LunarRevolution");
        LunarDist = GetNode<Node3D>("%LunarDist");
        LunarObliquity = GetNode<Node3D>("%LunarObliquity");
        UpdateLunarObliquityRotation();
        MoonAxis = GetNode<Node3D>("%MoonAxis");
        MoonTransform = GetNode<RemoteTransform3D>("%MoonTransform");
        SunTransform = GetNode<RemoteTransform3D>("%SunTransform");
        if (Sun != null)
            SunTransform.RemotePath = SunTransform.GetPathTo(Sun);
        if (Moon != null)
            MoonTransform.RemotePath = MoonTransform.GetPathTo(Moon);
        RenderingServer.GlobalShaderParameterSet(GlobalShaderParam.DirToSun, SunTransform.GlobalPosition.Normalized());
        _ready = true;
    }

    public override void _Process(double delta)
    {
        if (!_ready) return;
        var deltaF = (float)delta;
        if (PlanetRevolution || PlanetRotation)
        {
            RenderingServer.GlobalShaderParameterSet(GlobalShaderParam.DirToSun,
                SunTransform!.GlobalPosition.Normalized());
            // 行星公转
            if (PlanetRevolution)
                SunRevolution!.RotationDegrees = RotationTimeFactor * Vector3.Up * Mathf.Wrap(
                    SunRevolution.RotationDegrees.Y + PlanetRevolutionSpeed * deltaF, 0f, 360f);
            // 行星自转
            if (PlanetRotation)
            {
                // 用黄道面整体旋转表示所有天体相对运动（所以 deltaF 取负）
                var eclipticRotDeg = EclipticPlane!.RotationDegrees;
                EclipticPlane.RotationDegrees = Vector3.Right * eclipticRotDeg.X + RotationTimeFactor * Vector3.Up *
                    Mathf.Wrap(
                        eclipticRotDeg.Y + PlanetRotationSpeed * -deltaF, 0f, 360f);
                // 用天空盒的旋转表示银河背景的相对运动（所以 deltaF 取负）
                var skyRotation = _worldEnvironment!.Environment.SkyRotation;
                _worldEnvironment.Environment.SkyRotation = Vector3.Right * skyRotation.X + RotationTimeFactor *
                    Vector3.Up * Mathf.Wrap(
                        skyRotation.Y + Mathf.DegToRad(PlanetRotationSpeed) * -deltaF, 0f, Mathf.Tau);
            }
        }

        // 卫星公转
        if (SatelliteRevolution)
            LunarRevolution!.RotationDegrees = RotationTimeFactor * Vector3.Up * Mathf.Wrap(
                LunarRevolution.RotationDegrees.Y + SatelliteRevolutionSpeed * deltaF, 0f, 360f);
        // 卫星自转
        if (SatelliteRotation)
            MoonAxis!.RotationDegrees = RotationTimeFactor * Vector3.Up * Mathf.Wrap(
                MoonAxis.RotationDegrees.Y + SatelliteRotationSpeed * deltaF, 0f, 360f);
    }

    #endregion

    /// 更新月球轨道平面的旋转角度
    private void UpdateLunarOrbitPlaneRotation() =>
        LunarOrbitPlane!.RotationDegrees = Vector3.Right * SatelliteOrbitInclination;

    /// <summary>
    /// 更新黄道面的倾角。
    /// </summary>
    private void UpdateEclipticPlaneRotation() => EclipticPlane!.RotationDegrees = Vector3.Right * PlanetObliquity;

    /// <summary>
    /// 更新月球倾斜角。
    /// </summary>
    private void UpdateLunarObliquityRotation() =>
        LunarObliquity!.RotationDegrees = Vector3.Right * SatelliteObliquity;

    /// <summary>
    /// 更新银河星系天空盒的旋转角度。
    /// </summary>
    private void UpdateGalaxySkyRotation() =>
        _worldEnvironment!.Environment.SkyRotation =
            Vector3.Right * Mathf.DegToRad(PlanetObliquity - EclipticInclinationToGalactic);
}