using System.Linq;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Dependency;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entities;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entities.Civs;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Nodes;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Nodes.Planets;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Services;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Services.Civs;

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
                var camAttr = _planetCamera.Attributes as CameraAttributesPractical;
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

                _celestialMotionManager.UpdateMoonMeshRadius(); // 卫星半径
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
                _planetSettingService.Divisions = _divisions;
                _planetSettingService.ChunkDivisions = _chunkDivisions;
                _orbitCamera.Reset();
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
                _planetSettingService.ChunkDivisions = _chunkDivisions;
                _planetSettingService.Divisions = _divisions;
                _orbitCamera.Reset();
            }
        }
    }

    // 其实这里可以直接导入 Image, 在导入界面选择导入类型。但是导入 Image 的场景 tscn 文件会大得吓人……（等于直接按像素写一遍）
    [Export] private Texture2D _noiseSource;
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
                _celestialMotionManager.PlanetRevolution = value;
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
                _celestialMotionManager.PlanetRotation = value;
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
                _celestialMotionManager.SatelliteRevolution = value;
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
                _celestialMotionManager.SatelliteRotation = value;
        }
    }

    private bool _ready;

    private float _oldRadius;
    private int _oldDivisions;
    private int _oldChunkDivisions;
    private float _lastUpdated;

    #region services

    private ILodMeshCacheService _lodMeshCacheService;
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
    private ICivService _civService;

    private void InitServices()
    {
        _lodMeshCacheService = Context.GetBean<ILodMeshCacheService>();
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
        _editorService.EditModeChanged += OnEditorEditModeChanged;
        _civService = Context.GetBean<ICivService>();
    }

    private void OnEditorEditModeChanged(bool editMode)
    {
        UpdateSelectTileViewer();
        if (editMode)
            _unitManager.PathFromTileId = 0; // 清除单位移动路径 UI
        else
        {
            // 游戏模式下永远不显示编辑预览网格
            _editPreviewChunk.Hide();
            _selectTileViewer.CleanEditingTile();
        }
    }

    private void CleanEventListeners()
    {
        // 不小心忽视了事件的解绑，会在编辑器下"重载已保存场景"时出问题报错！
        // 【切记】所以这里需要在退出场景树时清理事件监听！！！
        _ready = false;
        _editorService.EditModeChanged -= OnEditorEditModeChanged;
    }

    #endregion

    #region on-ready nodes

    private CelestialMotionManager _celestialMotionManager; // 天体运动
    private Node3D _planetContainer; // 所有行星相关节点的父节点，用于整体一起自转

    // 行星节点
    private Node3D _planetAtmosphere; // 大气层插件的 GDScript 节点
    private MeshInstance3D _groundPlaceHolder; // 球面占位网格
    private ChunkManager _chunkManager;
    private UnitManager _unitManager; // 单位管理节点
    private OrbitCamera _orbitCamera;
    private Camera3D _planetCamera; // 行星主摄像机
    private SelectTileViewer _selectTileViewer;
    private EditPreviewChunk _editPreviewChunk;
    private HexMapGenerator _hexMapGenerator;
    private LongitudeLatitude _longitudeLatitude;

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

        _lastUpdated += floatDelta;
        if (_lastUpdated < 0.1f) return; // 每 0.1s 更新一次
        if (Mathf.Abs(_oldRadius - Radius) > 0.001f
            || _oldDivisions != Divisions
            || _oldChunkDivisions != ChunkDivisions)
            DrawHexSphereMesh();
        if (!Engine.IsEditorHint())
        {
            UpdateSelectTileViewer();
            UpdateCivTerritory();
        }

        _lastUpdated = 0f; // 每一秒检查一次
    }

    private void UpdateCivTerritory()
    {
        foreach (var civ in _civService.GetAll())
        {
            var tile = _tileService.GetById(civ.TileIds[GD.RandRange(0, civ.TileIds.Count - 1)]);
            var conquerTile = _tileService.GetNeighbors(tile).FirstOrDefault(n => !n.Data.IsUnderwater && n.CivId <= 0);
            if (conquerTile == null) continue;
            UpdateTileCivId(conquerTile, civ);
        }
    }

    private void UpdateTileCivId(Tile tile, Civ civ)
    {
        civ.TileIds.Add(tile.Id);
        tile.CivId = civ.Id;
        _tileShaderService.RefreshCiv(tile.Id);
    }

    private void UpdateSelectTileViewer()
    {
        var position = GetTileCollisionPositionUnderCursor();
        _selectTileViewer.Update(_unitManager.PathFromTileId, position);
    }

    public bool UpdateUiInEditMode()
    {
        if (!_editorService.TileOverrider.EditMode) return false;
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
        _editPreviewChunk.Update(tile);
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
        _unitManager?.ClearAllUnits(); // 注意编辑器内 _unitManager == null
        _chunkService.Truncate();
        _tileService.Truncate();
        _pointService.Truncate();
        _faceService.Truncate();
        _civService.Truncate();
        _selectViewService.ClearPath();
        _chunkManager.ClearOldData();
        _lodMeshCacheService.RemoveAllLodMeshes();
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
        InitCivilization();
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

    private void InitCivilization()
    {
        // 在可见分块的陆地分块中随机
        var tiles = _chunkService.GetAll()
            .Where(c => c.Insight)
            .SelectMany(c => c.TileIds)
            .Select(id => _tileService.GetById(id))
            .Where(t => !t.Data.IsUnderwater)
            .ToList();
        for (var i = 0; i < 8; i++)
        {
            var idx = GD.RandRange(0, tiles.Count - 1);
            var tile = tiles[idx];
            var civ = _civService.Add(new Color(
                Mathf.Lerp(0.3f, 1f, GD.Randf()),
                Mathf.Lerp(0.3f, 1f, GD.Randf()),
                Mathf.Lerp(0.3f, 1f, GD.Randf())));
            UpdateTileCivId(tile, civ);
            tiles[idx] = tiles[^1];
            tiles.RemoveAt(tiles.Count - 1);
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

    public void SelectEditingTile(Tile tile) => _selectTileViewer.SelectEditingTile(tile);
    public void CleanEditingTile() => _selectTileViewer.CleanEditingTile();

    #region 单位相关

    public void FindPath(Tile tile)
    {
        _unitManager.FindPath(tile);
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
        _unitManager.AddUnit(tile.Id, GD.Randf() * Mathf.Tau);
    }

    public void DestroyUnit()
    {
        var tile = GetTileUnderCursor();
        if (tile is not { UnitId: > 0 })
            return;
        _unitManager.RemoveUnit(tile.UnitId);
    }

    #endregion

    // 锁定经纬网的显示
    public void FixLatLon(bool toggle) => _longitudeLatitude.FixFullVisibility = toggle;

    public Vector3 GetOrbitCameraFocusPos() => _orbitCamera.GetFocusBasePos();

    public Vector3 ToPlanetLocal(Vector3 global) => _planetContainer.ToLocal(global);
}