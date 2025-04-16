using Apps.Applications.Planets;
using Apps.Contexts;
using Apps.Nodes;
using Domains.Models.Entities.PlanetGenerates;
using Domains.Models.Singletons.Planets;
using Domains.Repos.PlanetGenerates;
using Domains.Services.Navigations;
using Domains.Services.PlanetGenerates;
using Domains.Services.Shaders;
using Domains.Services.Uis;
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
    public HexPlanetManager() => InitServices(); // 现在 4.4 甚至构造函数会执行两次！奇了怪了，不知道之前 4.3 是不是也是这样
    // 调用两次构造函数（_EnterTree()、_Ready() 也一样）居然是好久以前（2020 年 7 月 3.2.2）以来一直的问题：
    // https://github.com/godotengine/godot-docs/issues/2930#issuecomment-662407208
    // https://github.com/godotengine/godot/issues/40970
    public override void _EnterTree() => NodeContext.Instance.RegisterSingleton<IHexPlanetManager>(this);

    public event IHexPlanetManager.NewPlanetGeneratedEvent? NewPlanetGenerated;

    private float _radius = 100f;

    [Export(PropertyHint.Range, "5, 1000")]
    public float Radius
    {
        get => _radius;
        set
        {
            _radius = value;
            if (_ready)
            {
                _planetConfig!.Radius = _radius;
                var camAttr = _planetCamera!.Attributes as CameraAttributesPractical;
                camAttr?.SetDofBlurFarDistance(_radius);
                camAttr?.SetDofBlurFarTransition(_radius / 2);
                camAttr?.SetDofBlurNearDistance(_radius / 10);
                camAttr?.SetDofBlurNearTransition(_radius / 20);
                _orbitCamera!.Reset();
                var groundSphere = _groundPlaceHolder!.Mesh as SphereMesh;
                groundSphere?.SetRadius(_radius * 0.99f);
                groundSphere?.SetHeight(_radius * 1.98f);
                _planetAtmosphere!.Set("planet_radius", _radius);
                _planetAtmosphere.Set("atmosphere_height", _radius * 0.25f);
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
            if (_ready)
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
            if (_ready)
            {
                _planetConfig!.ChunkDivisions = _chunkDivisions;
                _planetConfig.Divisions = _divisions;
                _orbitCamera!.Reset();
            }
        }
    }

    // 其实这里可以直接导入 Image, 在导入界面选择导入类型。但是导入 Image 的场景 tscn 文件会大得吓人……（等于直接按像素写一遍）
    [Export] private Texture2D? _noiseSource;
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
            if (_ready)
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
            if (_ready)
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
            if (_ready)
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
            if (_ready)
                _celestialMotionManager!.SatelliteRotation = value;
        }
    }

    private bool _ready;

    private float _oldRadius;
    private int _oldDivisions;
    private int _oldChunkDivisions;
    private float _lastUpdated;

    #region 服务与存储

    private IChunkService? _chunkService;
    private ITileService? _tileService;
    private ITileRepo? _tileRepo;
    private ITileShaderService? _tileShaderService;
    private ITileSearchService? _tileSearchService;
    private IPlanetConfig? _planetConfig;
    private INoiseConfig? _noiseConfig;
    private IEditorService? _editorService;
    private IHexPlanetManagerApplication? _hexPlanetManagerApplication;

    private void InitServices()
    {
        _chunkService = Context.GetBeanFromHolder<IChunkService>();
        _tileService = Context.GetBeanFromHolder<ITileService>();
        _tileRepo = Context.GetBeanFromHolder<ITileRepo>();
        _tileShaderService = Context.GetBeanFromHolder<ITileShaderService>();
        _tileSearchService = Context.GetBeanFromHolder<ITileSearchService>();
        _planetConfig = Context.GetBeanFromHolder<IPlanetConfig>();
        _noiseConfig = Context.GetBeanFromHolder<INoiseConfig>();
        _editorService = Context.GetBeanFromHolder<IEditorService>();
        _editorService.EditModeChanged += OnEditorEditModeChanged;
        _hexPlanetManagerApplication = Context.GetBeanFromHolder<IHexPlanetManagerApplication>();
    }

    private void OnEditorEditModeChanged(bool editMode)
    {
        UpdateSelectTileViewer();
        if (editMode)
            _unitManager!.PathFromTileId = 0; // 清除单位移动路径 UI
        else
        {
            // 游戏模式下永远不显示编辑预览网格
            _editPreviewChunk!.Hide();
            _selectTileViewer!.CleanEditingTile();
        }
    }

    private void CleanEventListeners()
    {
        // 不小心忽视了事件的解绑，会在编辑器下"重载已保存场景"时出问题报错！
        // 【切记】所以这里需要在退出场景树时清理事件监听！！！
        _ready = false;
        _editorService!.EditModeChanged -= OnEditorEditModeChanged;
    }

    #endregion

    #region on-ready nodes

    private CelestialMotionManager? _celestialMotionManager; // 天体运动
    private Node3D? _planetContainer; // 所有行星相关节点的父节点，用于整体一起自转

    // 行星节点
    private Node3D? _planetAtmosphere; // 大气层插件的 GDScript 节点
    private MeshInstance3D? _groundPlaceHolder; // 球面占位网格
    private ChunkManager? _chunkManager;
    private UnitManager? _unitManager; // 单位管理节点
    private OrbitCamera? _orbitCamera;
    private Camera3D? _planetCamera; // 行星主摄像机
    private SelectTileViewer? _selectTileViewer;
    private EditPreviewChunk? _editPreviewChunk;
    private HexMapGenerator? _hexMapGenerator;
    private LongitudeLatitude? _longitudeLatitude;

    private void InitOnReadyNodes()
    {
        _celestialMotionManager = GetNode<CelestialMotionManager>("%CelestialMotionManager");
        _celestialMotionManager.PlanetRotation = _planetRotation;
        _celestialMotionManager.PlanetRevolution = _planetRevolution;
        _celestialMotionManager.SatelliteRotation = _satelliteRotation;
        _celestialMotionManager.SatelliteRevolution = _satelliteRevolution;

        _planetContainer = GetNode<Node3D>("%PlanetContainer");
        // 行星节点
        _planetAtmosphere = GetNode<Node3D>("%PlanetAtmosphere");
        _groundPlaceHolder = GetNode<MeshInstance3D>("%GroundPlaceHolder");
        _chunkManager = GetNode<ChunkManager>("%ChunkManager");
        // 此处要求 OrbitCamera 也是 [Tool]，否则编辑器里会转型失败
        _orbitCamera = GetNode<OrbitCamera>("%OrbitCamera");
        _planetCamera = GetNode<Camera3D>("%PlanetCamera");
        _hexMapGenerator = GetNode<HexMapGenerator>("%HexMapGenerator");
        _longitudeLatitude = GetNode<LongitudeLatitude>("%LongitudeLatitude");
        if (!Engine.IsEditorHint())
        {
            // 没有 [Tool] 特性也不需要在编辑器下使用的节点。所以这里需要判断一下再赋值，否则会强转失败
            _unitManager = GetNode<UnitManager>("%UnitManager");
            _selectTileViewer = GetNode<SelectTileViewer>("%SelectTileViewer");
            _editPreviewChunk = GetNode<EditPreviewChunk>("%EditPreviewChunk");
        }

        _ready = true;
    }

    #endregion

    public override void _Ready()
    {
        GD.Print("HexPlanetManager _Ready start");
        InitOnReadyNodes();
        // 在 _ready = true 后初始化相关数据。手动赋值调用 setter 一下
        Radius = _radius;
        Divisions = _divisions;

        _noiseConfig!.NoiseSource = _noiseSource!.GetImage();
        _noiseConfig.InitializeHashGrid(Seed);
        DrawHexSphereMesh();
        GD.Print("HexPlanetManager _Ready end");
    }

    public override void _ExitTree() => CleanEventListeners();

    public override void _Process(double delta)
    {
        if (!_ready) return;
        var floatDelta = (float)delta;
        _tileShaderService!.UpdateData(floatDelta);

        _lastUpdated += floatDelta;
        if (_lastUpdated < 0.1f) return; // 每 0.1s 更新一次
        if (Mathf.Abs(_oldRadius - Radius) > 0.001f
            || _oldDivisions != Divisions
            || _oldChunkDivisions != ChunkDivisions)
            DrawHexSphereMesh();
        if (!Engine.IsEditorHint())
        {
            UpdateSelectTileViewer();
            _hexPlanetManagerApplication!.UpdateCivTerritory();
        }

        _lastUpdated = 0f; // 每一秒检查一次
    }

    private void UpdateSelectTileViewer()
    {
        var position = GetTileCollisionPositionUnderCursor();
        _selectTileViewer!.Update(_unitManager!.PathFromTileId, position);
    }

    public bool UpdateUiInEditMode()
    {
        if (!_editorService!.TileOverrider.EditMode) return false;
        // 编辑模式下更新预览网格
        UpdateEditPreviewChunk();
        if (Input.IsActionJustPressed("destroy_unit"))
        {
            DestroyUnit();
            return true;
        }

        if (Input.IsActionJustPressed("create_unit"))
        {
            CreateUnit();
            return true;
        }

        return false;
    }

    private void UpdateEditPreviewChunk()
    {
        var tile = GetTileUnderCursor();
        // 更新地块预览
        _editPreviewChunk!.Update(tile);
    }

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

    private Vector3 GetTileCollisionPositionUnderCursor()
    {
        var result = GetTileCollisionResult();
        if (result is { Count: > 0 } && result.TryGetValue("position", out var position))
            return ToPlanetLocal(position.AsVector3());
        return Vector3.Zero;
    }

    public Tile? GetTileUnderCursor()
    {
        var pos = GetTileCollisionPositionUnderCursor();
        if (pos == Vector3.Zero) return null;
        var tileId = _tileService!.SearchNearestTileId(pos);
        return tileId == null ? null : _tileRepo!.GetById((int)tileId);
    }

    private void ClearOldData()
    {
        // 必须先清理单位，否则相关可见度事件会查询地块，放最后会空引用异常
        _unitManager?.ClearAllUnits(); // 注意编辑器内 _unitManager == null
        _chunkManager!.ClearOldData();
        _hexPlanetManagerApplication!.ClearOldData();
    }

    private void DrawHexSphereMesh()
    {
        var time = Time.GetTicksMsec();
        GD.Print($"[===DrawHexSphereMesh===] radius {Radius}, divisions {Divisions}, start at: {time}");
        _oldRadius = Radius;
        _oldDivisions = Divisions;
        _oldChunkDivisions = ChunkDivisions;
        _lastUpdated = 0f;
        ClearOldData();
        InitHexSphere();
        _hexPlanetManagerApplication!.InitCivilization();
        _hexPlanetManagerApplication.RefreshAllTiles();
        NewPlanetGenerated?.Invoke(); // 触发事件，这种向直接上级事件回调的情况，不提取到 EventBus
        GD.Print($"[===DrawHexSphereMesh===] total cost: {Time.GetTicksMsec() - time} ms");
    }

    private void InitHexSphere()
    {
        _chunkService!.InitChunks();
        _tileService!.InitTiles();
        _tileShaderService!.Initialize();
        _tileSearchService!.InitSearchData();
        _hexMapGenerator!.GenerateMap();
        _chunkManager!.InitChunkNodes();
    }

    public void SelectEditingTile(Tile tile) => _selectTileViewer!.SelectEditingTile(tile);
    public void CleanEditingTile() => _selectTileViewer!.CleanEditingTile();

    #region 单位相关

    public void FindPath(Tile? tile)
    {
        _unitManager!.FindPath(tile);
    }

    private void CreateUnit()
    {
        var tile = GetTileUnderCursor();
        if (tile == null || tile.UnitId > 0)
        {
            GD.Print($"CreateUnit failed: tile {tile}, unitId: {tile?.UnitId}");
            return;
        }

        GD.Print($"CreateUnit at tile {tile.Id}");
        _unitManager!.AddUnit(tile.Id, GD.Randf() * Mathf.Tau);
    }

    private void DestroyUnit()
    {
        var tile = GetTileUnderCursor();
        if (tile is not { UnitId: > 0 })
            return;
        _unitManager!.RemoveUnit(tile.UnitId);
    }

    #endregion

    // 锁定经纬网的显示
    public void FixLatLon(bool toggle) => _longitudeLatitude!.FixFullVisibility = toggle;

    public Vector3 GetOrbitCameraFocusPos() => _orbitCamera!.GetFocusBasePos();

    public Vector3 ToPlanetLocal(Vector3 global) => _planetContainer!.ToLocal(global);
}