using Godot;
using Godot.Collections;
using ZeromaXsPlaygroundProject.Scenes.Framework.Dependency;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entities;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Nodes;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Services;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Structs;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-02-12 21:07
[Tool]
public partial class HexPlanetManager : Node3D
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
        InitServices(); // 现在 4.4 甚至构造函数会执行两次！奇了怪了，不知道之前 4.3 是不是也是这样
    }

    public delegate void NewPlanetGeneratedEvent();

    public event NewPlanetGeneratedEvent NewPlanetGenerated;

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
                _planetSettingService.Radius = _radius;
                var camAttr = _worldEnvironment.CameraAttributes as CameraAttributesPractical;
                camAttr?.SetDofBlurFarDistance(_radius);
                camAttr?.SetDofBlurFarTransition(_radius / 2);
                camAttr?.SetDofBlurNearDistance(_radius / 10);
                camAttr?.SetDofBlurNearTransition(_radius / 20);
                _orbitCamera.Reset();
                var groundSphere = _groundPlaceHolder.Mesh as SphereMesh;
                groundSphere?.SetRadius(_radius * 0.99f);
                groundSphere?.SetHeight(_radius * 1.98f);
                _planetAtmosphere.Set("planet_radius", _radius);
                _planetAtmosphere.Set("atmosphere_height", _radius * 0.25f);
                _longitudeLatitude.Draw(_radius + _planetSettingService.MaxHeight * 1.25f);
                UpdateMoonMeshRadius(); // 卫星半径
                UpdateLunarDist(); // 卫星轨道半径
            }
        }
    }

    private int _divisions = 20;

    [Export(PropertyHint.Range, "1, 100")]
    public int Divisions
    {
        get => _divisions;
        set
        {
            _divisions = value;
            _chunkDivisions = Mathf.Min(Mathf.Max(1, _divisions / 4), _chunkDivisions);
            if (_ready)
            {
                _planetSettingService.Divisions = _divisions;
                _planetSettingService.ChunkDivisions = _chunkDivisions;
                _orbitCamera.Reset();
            }
        }
    }

    private int _chunkDivisions = 5;

    [Export(PropertyHint.Range, "1, 25")]
    public int ChunkDivisions
    {
        get => _chunkDivisions;
        set
        {
            _chunkDivisions = value;
            _divisions = Mathf.Max(Mathf.Min(100, _chunkDivisions * 4), _divisions);
            if (_ready)
            {
                _planetSettingService.ChunkDivisions = _chunkDivisions;
                _planetSettingService.Divisions = _divisions;
                _orbitCamera.Reset();
            }
        }
    }

    // 其实这里可以直接导入 Image, 在导入界面选择导入类型。但是导入 Image 的场景 tscn 文件会大得吓人……（等于直接按像素写一遍）
    [Export] private Texture2D _noiseSource;
    [Export] private PackedScene _unitScene;
    [Export] public ulong Seed { get; set; } = 1234;

    [Export(PropertyHint.Range, "-100.0, 100.0")]
    public float RotationTimeFactor = 1f;

    private float _eclipticInclinationToGalactic = 60f;

    [ExportSubgroup("行星恒星设置")]
    [ExportToolButton("切换恒星运行状态", Icon = "DirectionalLight3D")]
    public Callable StarMoveStatus => Callable.From(ToggleStarMoveStatus);

    private void ToggleStarMoveStatus()
    {
        if (PlanetRevolution)
        {
            PlanetRevolution = false;
            _sunRevolution.RotationDegrees = Vector3.Up * 180f;
            RenderingServer.GlobalShaderParameterSet("dir_to_sun", _sunMesh.GlobalPosition.Normalized());
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
            _planetAxis.Rotation = Vector3.Zero;
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

    [ExportSubgroup("卫星设置")]
    [ExportToolButton("切换卫星运行状态", Icon = "CSGSphere3D")]
    public Callable SatelliteMoveStatus => Callable.From(ToggleSatelliteMoveStatus);

    private void ToggleSatelliteMoveStatus()
    {
        if (SatelliteRevolution || SatelliteRotation)
        {
            SatelliteRevolution = false;
            SatelliteRotation = false;
            _lunarRevolution.RotationDegrees = Vector3.Up * 180f;
            _moonAxis.Rotation = Vector3.Zero;
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
                UpdateMoonMeshRadius();
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
                UpdateLunarDist();
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

    private bool _ready;

    private bool EditMode => _editorService.TileOverrider.EditMode;
    private int LabelMode => _editorService.LabelMode;

    private int EditingTileId { get; set; }

    private int _pathFromTileId;

    private int PathFromTileId
    {
        get => _pathFromTileId;
        set
        {
            _pathFromTileId = value;
            if (_pathFromTileId == 0)
                _selectViewService.ClearPath();
        }
    }

    private float _oldRadius;
    private int _oldDivisions;
    private int _oldChunkDivisions;
    private float _lastUpdated;

    private readonly System.Collections.Generic.Dictionary<int, HexUnit> _units = new();

    #region services

    private IUnitService _unitService;
    private IChunkService _chunkService;
    private ITileService _tileService;
    private ITileShaderService _tileShaderService;
    private ITileSearchService _tileSearchService;
    private IFaceService _faceService;
    private IPointService _pointService;
    private ISelectViewService _selectViewService;
    private IPlanetSettingService _planetSettingService;
    private INoiseService _noiseService;
    private IEditorService _editorService;

    private void InitServices()
    {
        _unitService = Context.GetBean<IUnitService>();
        _chunkService = Context.GetBean<IChunkService>();
        _tileService = Context.GetBean<ITileService>();
        _tileShaderService = Context.GetBean<ITileShaderService>();
        _tileSearchService = Context.GetBean<ITileSearchService>();
        _faceService = Context.GetBean<IFaceService>();
        _pointService = Context.GetBean<IPointService>();
        _selectViewService = Context.GetBean<ISelectViewService>();
        _planetSettingService = Context.GetBean<IPlanetSettingService>();
        _noiseService = Context.GetBean<INoiseService>();
        _editorService = Context.GetBean<IEditorService>();
        _tileService.UnitValidateLocation += OnTileServiceUnitValidateLocation;
        _editorService.EditModeChanged += OnEditorEditModeChanged;
    }

    private void OnEditorEditModeChanged(bool editMode)
    {
        UpdateSelectTileViewer();
        if (editMode)
            PathFromTileId = 0;
        else
        {
            // 游戏模式下永远不显示编辑预览网格
            _editPreviewChunk.Visible = false;
            EditingTileId = 0;
        }
    }

    private void OnTileServiceUnitValidateLocation(int unitId) => _units[unitId].ValidateLocation();

    private void CleanEventListeners()
    {
        // 不小心忽视了事件的解绑，会在编辑器下"重载已保存场景"时出问题报错！
        // 【切记】所以这里需要在退出场景树时清理事件监听！！！
        _ready = false;
        _tileService.UnitValidateLocation -= OnTileServiceUnitValidateLocation;
        _editorService.EditModeChanged -= OnEditorEditModeChanged;
    }

    #endregion

    #region on-ready nodes

    private WorldEnvironment _worldEnvironment;

    // 天体公转自转
    private Node3D _eclipticPlane;
    private Node3D _sunRevolution;
    private Node3D _planetAxis;
    private Node3D _lunarOrbitPlane;
    private Node3D _lunarRevolution;
    private Node3D _lunarDist;
    private Node3D _lunarObliquity;
    private Node3D _moonAxis;
    private MeshInstance3D _moonMesh;
    private MeshInstance3D _sunMesh;

    // 行星节点
    private Node3D _planetAtmosphere; // 大气层插件的 GDScript 节点
    private MeshInstance3D _groundPlaceHolder; // 球面占位网格
    private ChunkManager _chunkManager;
    private Node3D _unitManager; // 单位管理节点
    private OrbitCamera _orbitCamera;
    private MeshInstance3D _selectTileViewer;
    private EditPreviewChunk _editPreviewChunk;
    private HexUnitPathPool _hexUnitPathPool;
    private HexMapGenerator _hexMapGenerator;
    private LongitudeLatitude _longitudeLatitude;

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
        _lunarDist = GetNode<Node3D>("%LunarDist");
        UpdateLunarDist();
        _lunarObliquity = GetNode<Node3D>("%LunarObliquity");
        UpdateLunarObliquityRotation();
        _moonAxis = GetNode<Node3D>("%MoonAxis");
        _moonMesh = GetNode<MeshInstance3D>("%MoonMesh");
        UpdateMoonMeshRadius();
        _sunMesh = GetNode<MeshInstance3D>("%SunMesh");
        RenderingServer.GlobalShaderParameterSet("dir_to_sun", _sunMesh.GlobalPosition.Normalized());

        // 行星节点
        _planetAtmosphere = GetNode<Node3D>("%PlanetAtmosphere");
        _groundPlaceHolder = GetNode<MeshInstance3D>("%GroundPlaceHolder");
        _chunkManager = GetNode<ChunkManager>("%ChunkManager");
        _unitManager = GetNode<Node3D>("%UnitManager");
        // 此处要求 OrbitCamera 也是 [Tool]，否则编辑器里会转型失败
        _orbitCamera = GetNode<OrbitCamera>("%OrbitCamera");
        _selectTileViewer = GetNode<MeshInstance3D>("%SelectTileViewer");
        _hexMapGenerator = GetNode<HexMapGenerator>("%HexMapGenerator");
        _longitudeLatitude = GetNode<LongitudeLatitude>("%LongitudeLatitude");
        if (!Engine.IsEditorHint())
        {
            // 没有 [Tool] 特性也不需要在编辑器下使用的节点。所以这里需要判断一下再赋值，否则会强转失败
            _editPreviewChunk = GetNode<EditPreviewChunk>("%EditPreviewChunk");
            _hexUnitPathPool = GetNode<HexUnitPathPool>("%HexUnitPathPool");
        }

        _ready = true;
    }

    private void UpdateLunarOrbitPlaneRotation() =>
        _lunarOrbitPlane.RotationDegrees = Vector3.Right * SatelliteOrbitInclination;

    private void UpdateEclipticPlaneRotation() => _eclipticPlane.RotationDegrees = Vector3.Right * PlanetObliquity;
    private void UpdateLunarObliquityRotation() => _lunarObliquity.RotationDegrees = Vector3.Right * SatelliteObliquity;

    private void UpdateLunarDist() => _lunarDist.Position =
        Vector3.Back * Mathf.Clamp(Radius * SatelliteDistRatio, Radius * (1 + _satelliteRadiusRatio), 800f);

    private void UpdateGalaxySkyRotation() =>
        _worldEnvironment.Environment.SkyRotation =
            Vector3.Right * Mathf.DegToRad(PlanetObliquity - EclipticInclinationToGalactic);

    private void UpdateMoonMeshRadius()
    {
        var moonMesh = _moonMesh.Mesh as SphereMesh;
        moonMesh?.SetRadius(Radius * SatelliteRadiusRatio);
        moonMesh?.SetHeight(Radius * SatelliteRadiusRatio * 2);
    }

    #endregion

    public override void _Ready()
    {
        GD.Print("HexPlanetManager _Ready start");
        InitOnReadyNodes();
        // 不知道为啥这个时候 setter 又不生效了，没初始化 HexMetrics 里面的数据。手动赋值调用 setter 一下
        Radius = _radius;
        Divisions = _divisions;

        _noiseService.NoiseSource = _noiseSource.GetImage();
        _noiseService.InitializeHashGrid(Seed);
        DrawHexSphereMesh();
        GD.Print("HexPlanetManager _Ready end");
    }

    public override void _ExitTree() => CleanEventListeners();

    public override void _Process(double delta)
    {
        if (!_ready) return;
        var floatDelta = (float)delta;
        _tileShaderService.UpdateData(floatDelta);
        UpdateStellarRotation(floatDelta);

        _lastUpdated += floatDelta;
        if (_lastUpdated < 0.1f) return; // 每 0.1s 更新一次
        if (Mathf.Abs(_oldRadius - Radius) > 0.001f
            || _oldDivisions != Divisions
            || _oldChunkDivisions != ChunkDivisions)
            DrawHexSphereMesh();
        if (!Engine.IsEditorHint())
            UpdateSelectTileViewer();
        _lastUpdated = 0f; // 每一秒检查一次
    }

    // 更新天体旋转
    private void UpdateStellarRotation(float delta)
    {
        if (PlanetRevolution || PlanetRotation)
        {
            RenderingServer.GlobalShaderParameterSet("dir_to_sun", _planetAxis.ToLocal(_sunMesh.GlobalPosition.Normalized()));
            // 行星公转
            if (PlanetRevolution)
                _sunRevolution.RotationDegrees = RotationTimeFactor * Vector3.Up * Mathf.Wrap(
                    _sunRevolution.RotationDegrees.Y + PlanetRevolutionSpeed * delta, 0f, 360f);
            // 行星自转
            if (PlanetRotation)
                _planetAxis.RotationDegrees = RotationTimeFactor * Vector3.Up * Mathf.Wrap(
                    _planetAxis.RotationDegrees.Y + PlanetRotationSpeed * delta, 0f, 360f);
        }

        // 卫星公转
        if (SatelliteRevolution)
            _lunarRevolution.RotationDegrees = RotationTimeFactor * Vector3.Up * Mathf.Wrap(
                _lunarRevolution.RotationDegrees.Y + SatelliteRevolutionSpeed * delta, 0f, 360f);
        // 卫星自转
        if (SatelliteRotation)
            _moonAxis.RotationDegrees = RotationTimeFactor * Vector3.Up * Mathf.Wrap(
                _moonAxis.RotationDegrees.Y + SatelliteRotationSpeed * delta, 0f, 360f);
    }

    private void UpdateSelectTileViewer()
    {
        var position = GetTileCollisionPositionUnderCursor();
        if (EditMode)
            UpdateSelectTileInEditMode(position);
        else
            UpdateSelectTileInPlayMode(position);
    }

    private void UpdateSelectTileInPlayMode(Vector3 position)
    {
        if (PathFromTileId == 0)
        {
            _selectTileViewer.Visible = false;
            return;
        }

        _selectTileViewer.Visible = true;
        var mesh = _selectViewService.GenerateMeshForPlayMode(PathFromTileId, position);
        if (mesh != null)
            _selectTileViewer.Mesh = mesh;
    }

    private void UpdateSelectTileInEditMode(Vector3 position)
    {
        if (position != Vector3.Zero || EditingTileId > 0)
        {
            // 更新选择地块框
            _selectTileViewer.Visible = true;
            var mesh = _selectViewService.GenerateMeshForEditMode(EditingTileId, position);
            if (mesh != null)
                _selectTileViewer.Mesh = mesh;
        }
        else
        {
            // GD.Print("No tile under cursor, _selectTileViewer not visible");
            _selectTileViewer.Visible = false;
        }
    }

    public void UpdateEditPreviewChunk(HexTileDataOverrider tileOverrider)
    {
        var tile = GetTileUnderCursor();
        if (tile != null)
        {
            // 更新地块预览
            _editPreviewChunk.Refresh(tileOverrider, _tileService.GetTilesInDistance(tile, tileOverrider.BrushSize));
            _editPreviewChunk.Visible = true;
        }
        else
            _editPreviewChunk.Visible = false;
    }

    private Dictionary GetTileCollisionResult()
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
            return _planetAxis.ToLocal(position.AsVector3());
        return Vector3.Zero;
    }

    public Tile GetTileUnderCursor()
    {
        var pos = GetTileCollisionPositionUnderCursor();
        if (pos == Vector3.Zero) return null;
        var tileId = _tileService.SearchNearestTileId(pos);
        return tileId == null ? null : _tileService.GetById((int)tileId);
    }

    private void ClearOldData()
    {
        // 必须先清理单位，否则相关可见度事件会查询地块，放最后会空引用异常
        ClearAllUnits();
        _chunkService.Truncate();
        _tileService.Truncate();
        _pointService.Truncate();
        _faceService.Truncate();
        _selectViewService.ClearPath();
        _chunkManager.ClearOldData();
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
        RefreshAllTiles();
        NewPlanetGenerated?.Invoke(); // 触发事件，这种向直接上级事件回调的情况，不提取到 EventBus
        GD.Print($"[===DrawHexSphereMesh===] total cost: {Time.GetTicksMsec() - time} ms");
    }

    private void RefreshAllTiles()
    {
        foreach (var tile in _tileService.GetAll())
        {
            _tileSearchService.RefreshTileSearchData(tile.Id);
            _tileShaderService.RefreshTerrain(tile.Id);
            _tileShaderService.RefreshVisibility(tile.Id);
        }
    }

    private void InitHexSphere()
    {
        _chunkService.InitChunks();
        _tileService.InitTiles();
        _tileShaderService.Initialize();
        _tileSearchService.InitSearchData();
        _hexMapGenerator.GenerateMap();
        _chunkManager.InitChunkNodes();
    }

    public void SelectEditingTile(Tile tile)
    {
        EditingTileId = tile?.Id ?? 0;
    }

    public void CleanEditingTile() => EditingTileId = 0;

    public void FindPath(Tile tile)
    {
        if (PathFromTileId != 0)
        {
            if (tile == null || tile.Id == PathFromTileId)
            {
                // 重复点选同一地块，则取消选择
                PathFromTileId = 0;
            }
            else MoveUnit(tile);
        }
        else
            // 当前没有选择地块（即没有选中单位）的话，则在有单位时选择该地块
            PathFromTileId = tile == null || tile.UnitId == 0 ? 0 : tile.Id;
    }

    private void MoveUnit(Tile toTile)
    {
        var fromTile = _tileService.GetById(PathFromTileId);
        var path = _tileSearchService.FindPath(fromTile, toTile, true);
        if (path is { Count: > 1 })
        {
            // 确实有找到从出发点到 tile 的路径
            var unit = _units[fromTile.UnitId];
            _hexUnitPathPool.NewTask(unit, path);
        }

        PathFromTileId = 0;
    }

    public void CreateUnit()
    {
        var tile = GetTileUnderCursor();
        if (tile == null || tile.UnitId > 0)
        {
            GD.Print($"CreateUnit failed: tile {tile}, unitId: {tile?.UnitId}");
            return;
        }

        GD.Print($"CreateUnit at tile {tile.Id}");
        var unit = _unitScene.Instantiate<HexUnit>();
        AddUnit(unit, tile.Id, GD.Randf() * Mathf.Tau);
    }

    private void AddUnit(HexUnit unit, int tileId, float orientation)
    {
        _unitManager.AddChild(unit);
        _units[unit.Id] = unit;
        unit.TileId = tileId;
        unit.Orientation = orientation;
    }

    public void DestroyUnit()
    {
        var tile = GetTileUnderCursor();
        if (tile is not { UnitId: > 0 })
            return;
        RemoveUnit(tile.UnitId);
    }

    private void RemoveUnit(int unitId)
    {
        _units[unitId].Die();
        _units.Remove(unitId);
    }

    private void ClearAllUnits()
    {
        foreach (var unit in _units.Values)
            unit.Die();
        _units.Clear();
        _unitService.Truncate();
    }

    // 锁定经纬网的显示
    public void FixLatLon(bool toggle) => _longitudeLatitude.FixFullVisibility = toggle;

    public Vector3 GetOrbitCameraFocusPos() => _orbitCamera.GetFocusBasePos();
}