using Apps.Applications.Planets;
using Apps.Contexts;
using Apps.Nodes;
using Domains.Models.Entities.PlanetGenerates;
using Domains.Models.Singletons.Planets;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Dependency;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Nodes;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Nodes.Planets;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-02-12 21:07
[Tool]
public partial class HexPlanetManager : Node3D, IHexPlanetManager
{
    // Godot C# 的生命周期方法执行顺序：
    // 父节点构造函数 -> 子节点构造函数
    // （构造函数这个顺序存疑。目前 4.4 发现闪退现象，因为 HexGridChunk 构造函数优先于 HexPlanetManager 构造函数执行了！
    // 目前猜想可能是因为引入 uid 后，[Export] PackageScene 挂载的脚本构造函数生命周期提前了？）
    // -> 父节点 _EnterTree() -> 子节点 _EnterTree()（从上到下）
    // -> 子节点 _Ready()（从下到上） -> 父节点 _Ready() 【特别注意这里的顺序！！！】
    // -> 父节点 _Process() -> 子节点 _Process()（从上到下）
    // -> 子节点 _ExitTree()（从下到上） -> 父节点 _ExitTree() 【特别注意这里的顺序！！！】
    public HexPlanetManager()
    {
        InitApps(); // 现在 4.4 甚至构造函数会执行两次！奇了怪了，不知道之前 4.3 是不是也是这样
        NodeContext.Instance.RegisterSingleton<IHexPlanetManager>(this);
    }
    // 调用两次构造函数（_EnterTree()、_Ready() 也一样）居然是好久以前（2020 年 7 月 3.2.2）以来一直的问题：
    // https://github.com/godotengine/godot-docs/issues/2930#issuecomment-662407208
    // https://github.com/godotengine/godot/issues/40970

    public event IHexPlanetManager.NewPlanetGeneratedEvent? NewPlanetGenerated;

    private float _radius = 100f;

    [Export(PropertyHint.Range, "5, 1000")]
    public float Radius
    {
        get => _radius;
        set
        {
            _radius = value;
            if (NodeReady)
            {
                _planetConfig!.Radius = _radius;
                var camAttr = PlanetCamera!.Attributes as CameraAttributesPractical;
                camAttr?.SetDofBlurFarDistance(_radius);
                camAttr?.SetDofBlurFarTransition(_radius / 2);
                camAttr?.SetDofBlurNearDistance(_radius / 10);
                camAttr?.SetDofBlurNearTransition(_radius / 20);
                _orbitCamera!.Reset();
                var groundSphere = GroundPlaceHolder!.Mesh as SphereMesh;
                groundSphere?.SetRadius(_radius * 0.99f);
                groundSphere?.SetHeight(_radius * 1.98f);
                PlanetAtmosphere!.Set("planet_radius", _radius);
                PlanetAtmosphere.Set("atmosphere_height", _radius * 0.25f);
                _longitudeLatitude!.Draw(_radius + _planetConfig.MaxHeight * 1.25f);

                _celestialMotionManager!.UpdateMoonMeshRadius(); // 卫星半径
                _celestialMotionManager.UpdateLunarDist(); // 卫星轨道半径
            }
        }
    }

    private int _divisions = 20;

    [Export(PropertyHint.Range, "1, 200")]
    public int Divisions
    {
        get => _divisions;
        set
        {
            _divisions = value;
            _chunkDivisions = Mathf.Min(Mathf.Max(1, _divisions / 10), _chunkDivisions);
            if (NodeReady)
            {
                _planetConfig!.Divisions = _divisions;
                _planetConfig.ChunkDivisions = _chunkDivisions;
                _orbitCamera!.Reset();
            }
        }
    }

    private int _chunkDivisions = 5;

    [Export(PropertyHint.Range, "1, 20")]
    public int ChunkDivisions
    {
        get => _chunkDivisions;
        set
        {
            _chunkDivisions = value;
            _divisions = Mathf.Max(Mathf.Min(200, _chunkDivisions * 10), _divisions);
            if (NodeReady)
            {
                _planetConfig!.ChunkDivisions = _chunkDivisions;
                _planetConfig.Divisions = _divisions;
                _orbitCamera!.Reset();
            }
        }
    }

    // 其实这里可以直接导入 Image, 在导入界面选择导入类型。但是导入 Image 的场景 tscn 文件会大得吓人……（等于直接按像素写一遍）
    [Export] public Texture2D? NoiseSource { get; set; }
    [Export] public ulong Seed { get; set; } = 1234;

    private bool _planetRevolution = true;

    // 行星公转
    [ExportGroup("天体运动")]
    [Export]
    public bool PlanetRevolution
    {
        get => _planetRevolution;
        set
        {
            _planetRevolution = value;
            if (NodeReady)
                _celestialMotionManager!.PlanetRevolution = value;
        }
    }

    private bool _planetRotation = true;

    // 行星自转
    [Export]
    public bool PlanetRotation
    {
        get => _planetRotation;
        set
        {
            _planetRotation = value;
            if (NodeReady)
                _celestialMotionManager!.PlanetRotation = value;
        }
    }

    private bool _satelliteRevolution = true;

    // 卫星公转
    [Export]
    public bool SatelliteRevolution
    {
        get => _satelliteRevolution;
        set
        {
            _satelliteRevolution = value;
            if (NodeReady)
                _celestialMotionManager!.SatelliteRevolution = value;
        }
    }

    private bool _satelliteRotation = true;

    // 卫星自转
    [Export]
    public bool SatelliteRotation
    {
        get => _satelliteRotation;
        set
        {
            _satelliteRotation = value;
            if (NodeReady)
                _celestialMotionManager!.SatelliteRotation = value;
        }
    }

    public bool NodeReady { get; private set; }

    public float OldRadius { get; set; }
    public int OldDivisions { get; set; }
    public int OldChunkDivisions { get; set; }
    public float LastUpdated { get; set; }

    #region 应用服务

    private IPlanetConfig? _planetConfig;
    private IHexPlanetManagerApp? _hexPlanetManagerApplication;

    private void InitApps()
    {
        _planetConfig = Context.GetBeanFromHolder<IPlanetConfig>();
        _hexPlanetManagerApplication = Context.GetBeanFromHolder<IHexPlanetManagerApp>();
    }

    #endregion

    #region on-ready nodes

    private CelestialMotionManager? _celestialMotionManager; // 天体运动
    public Node3D? PlanetContainer { get; private set; } // 所有行星相关节点的父节点，用于整体一起自转

    // 行星节点
    public Node3D? PlanetAtmosphere { get; private set; } // 大气层插件的 GDScript 节点
    public MeshInstance3D? GroundPlaceHolder { get; private set; } // 球面占位网格
    private OrbitCamera? _orbitCamera;
    public Camera3D? PlanetCamera { get; private set; } // 行星主摄像机
    private LongitudeLatitude? _longitudeLatitude;

    private void InitOnReadyNodes()
    {
        _celestialMotionManager = GetNode<CelestialMotionManager>("%CelestialMotionManager");
        _celestialMotionManager.PlanetRotation = _planetRotation;
        _celestialMotionManager.PlanetRevolution = _planetRevolution;
        _celestialMotionManager.SatelliteRotation = _satelliteRotation;
        _celestialMotionManager.SatelliteRevolution = _satelliteRevolution;

        PlanetContainer = GetNode<Node3D>("%PlanetContainer");
        // 行星节点
        PlanetAtmosphere = GetNode<Node3D>("%PlanetAtmosphere");
        GroundPlaceHolder = GetNode<MeshInstance3D>("%GroundPlaceHolder");
        // 此处要求 OrbitCamera 也是 [Tool]，否则编辑器里会转型失败
        _orbitCamera = GetNode<OrbitCamera>("%OrbitCamera");
        PlanetCamera = GetNode<Camera3D>("%PlanetCamera");
        _longitudeLatitude = GetNode<LongitudeLatitude>("%LongitudeLatitude");
        NodeReady = true;
    }

    #endregion

    public override void _Ready()
    {
        GD.Print("HexPlanetManager _Ready start");
        InitOnReadyNodes();
        _hexPlanetManagerApplication!.OnReady();
        GD.Print("HexPlanetManager _Ready end");
    }

    public override void _ExitTree()
    {
        // 不小心忽视了事件的解绑，会在编辑器下"重载已保存场景"时出问题报错！
        // 【切记】所以这里需要在退出场景树时清理事件监听！！！
        NodeReady = false;
        _hexPlanetManagerApplication!.OnExitTree();
    }

    public override void _Process(double delta) => _hexPlanetManagerApplication!.OnProcess(delta);

    private Godot.Collections.Dictionary GetTileCollisionResult()
    {
        var spaceState = GetWorld3D().DirectSpaceState;
        var camera = GetViewport().GetCamera3D();
        var mousePos = GetViewport().GetMousePosition();
        var origin = camera.ProjectRayOrigin(mousePos);
        var end = origin + camera.ProjectRayNormal(mousePos) * 2000f;
        var query = PhysicsRayQueryParameters3D.Create(origin, end);
        return spaceState.IntersectRay(query);
    }

    // TODO: 下面两个方法，相关逻辑在 APP 层和节点层上下翻飞，需要重构
    public bool UpdateUiInEditMode() => _hexPlanetManagerApplication!.UpdateUiInEditMode();
    public Tile? GetTileUnderCursor() => _hexPlanetManagerApplication!.GetTileUnderCursor();

    public Vector3 GetTileCollisionPositionUnderCursor()
    {
        var result = GetTileCollisionResult();
        if (result is { Count: > 0 } && result.TryGetValue("position", out var position))
            return ToPlanetLocal(position.AsVector3());
        return Vector3.Zero;
    }

    public void EmitNewPlanetGenerated()
    {
        NewPlanetGenerated?.Invoke(); // 触发事件
    }

    // 锁定经纬网的显示
    public void FixLatLon(bool toggle) => _longitudeLatitude!.FixFullVisibility = toggle;

    public Vector3 ToPlanetLocal(Vector3 global) => PlanetContainer!.ToLocal(global);
}