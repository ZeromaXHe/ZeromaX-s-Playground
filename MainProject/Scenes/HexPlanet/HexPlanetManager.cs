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

    [Export] private Texture2D _noiseSource;
    [Export] private PackedScene _unitScene;
    [Export] public ulong Seed { get; set; } = 1234;

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
    private Node3D _planetAtmosphere; // 大气层插件的 GDScript 节点
    private MeshInstance3D _groundPlaceHolder; // 球面占位网格
    private ChunkManager _chunkManager;
    private OrbitCamera _orbitCamera;
    private MeshInstance3D _selectTileViewer;
    private EditPreviewChunk _editPreviewChunk;
    private HexUnitPathPool _hexUnitPathPool;
    private HexMapGenerator _hexMapGenerator;
    private LongitudeLatitude _longitudeLatitude;

    private void InitOnReadyNodes()
    {
        _worldEnvironment = GetNode<WorldEnvironment>("%WorldEnvironment");
        _planetAtmosphere = GetNode<Node3D>("%PlanetAtmosphere");
        _groundPlaceHolder = GetNode<MeshInstance3D>("%GroundPlaceHolder");
        _chunkManager = GetNode<ChunkManager>("%ChunkManager");
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
        _tileShaderService.UpdateData((float)delta);

        _lastUpdated += (float)delta;
        if (_lastUpdated < 0.1f) return; // 每 0.1s 更新一次
        if (Mathf.Abs(_oldRadius - Radius) > 0.001f
            || _oldDivisions != Divisions
            || _oldChunkDivisions != ChunkDivisions)
            DrawHexSphereMesh();
        if (!Engine.IsEditorHint())
            UpdateSelectTileViewer();
        _lastUpdated = 0f; // 每一秒检查一次
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
            return position.AsVector3();
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
        AddChild(unit);
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