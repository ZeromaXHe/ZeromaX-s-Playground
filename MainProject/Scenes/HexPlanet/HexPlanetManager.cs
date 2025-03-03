using System.Collections.Generic;
using System.Linq;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Dependency;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Node;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet;

[Tool]
public partial class HexPlanetManager : Node3D
{
    // Godot C# 的生命周期方法执行顺序：
    // 父节点构造函数 -> 子节点构造函数
    // -> 父节点 _EnterTree() -> 子节点 _EnterTree()（从上到下）
    // -> 子节点 _Ready()（从下到上） -> 父节点 _Ready() 【特别注意这里的顺序！！！】
    // -> 父节点 _Process() -> 子节点 _Process()（从上到下）
    // -> 子节点 _ExitTree()（从下到上） -> 父节点 _ExitTree() 【特别注意这里的顺序！！！】
    public HexPlanetManager()
    {
        Context.Init();
        InitServices();
    }

    [Signal]
    public delegate void NewPlanetGeneratedEventHandler();

    private float _radius = 100f;

    [Export(PropertyHint.Range, "5, 1000")]
    public float Radius
    {
        get => _radius;
        set
        {
            _radius = value;
            HexMetrics.Radius = _radius;
            RenderingServer.GlobalShaderParameterSet("radius", _radius);
            RenderingServer.GlobalShaderParameterSet("max_height", HexMetrics.MaxHeight);
            if (_ready)
            {
                _orbitCamera.Reset();
                _atmosphereFog.Size = Vector3.One * _radius * 2.7f;
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
            HexMetrics.Divisions = _divisions;
            _chunkDivisions = Mathf.Min(Mathf.Max(1, _divisions / 4), _chunkDivisions);
            if (_ready)
                _orbitCamera.Reset();
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
            HexMetrics.Divisions = _divisions;
            if (_ready)
                _orbitCamera.Reset();
        }
    }

    [Export] private Texture2D _noiseSource;
    [Export] private PackedScene _gridChunkScene;
    [Export] private PackedScene _unitScene;
    [Export] public ulong Seed { get; set; } = 1234;

    private bool _ready;

    private bool _editMode;
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

    private readonly Dictionary<int, HexGridChunk> _gridChunks = new();
    private readonly Dictionary<int, HexUnit> _units = new();

    #region services

    private IUnitService _unitService;
    private IChunkService _chunkService;
    private ITileService _tileService;
    private ITileShaderService _tileShaderService;
    private ITileSearchService _tileSearchService;
    private IFaceService _faceService;
    private IPointService _pointService;
    private IAStarService _aStarService;
    private ISelectViewService _selectViewService;

    private void InitServices()
    {
        _unitService = Context.GetBean<IUnitService>();
        _chunkService = Context.GetBean<IChunkService>();
        _tileService = Context.GetBean<ITileService>();
        _tileShaderService = Context.GetBean<ITileShaderService>();
        _tileSearchService = Context.GetBean<ITileSearchService>();
        _faceService = Context.GetBean<IFaceService>();
        _pointService = Context.GetBean<IPointService>();
        _aStarService = Context.GetBean<IAStarService>();
        _selectViewService = Context.GetBean<ISelectViewService>();
        _chunkService.RefreshChunk += id => _gridChunks[id].Refresh();
        _chunkService.RefreshChunkTileLabel +=
            (chunkId, tileId, text) => _gridChunks[chunkId].RefreshTileLabel(tileId, text);
        _tileService.UnitValidateLocation += unitId => _units[unitId].ValidateLocation();
    }

    #endregion

    #region on-ready nodes

    private FogVolume _atmosphereFog;
    private Node3D _chunks;
    private OrbitCamera _orbitCamera;
    private MeshInstance3D _selectTileViewer;
    private HexUnitPathPool _hexUnitPathPool;

    private void InitOnReadyNodes()
    {
        _atmosphereFog = GetNode<FogVolume>("%AtmosphereFog");
        _chunks = GetNode<Node3D>("%Chunks");
        // 此处要求 OrbitCamera 也是 [Tool]，否则编辑器里会转型失败
        _orbitCamera = GetNode<OrbitCamera>("%OrbitCamera");
        _selectTileViewer = GetNode<MeshInstance3D>("%SelectTileViewer");
        // 没有 [Tool] 特性也不需要在编辑器下使用，所以这里判断一下，否则会强转失败
        if (!Engine.IsEditorHint())
            _hexUnitPathPool = GetNode<HexUnitPathPool>("%HexUnitPathPool");
        _ready = true;
    }

    #endregion

    public override void _Ready()
    {
        InitOnReadyNodes();
        // 不知道为啥这个时候 setter 又不生效了，没初始化 HexMetrics 里面的数据。手动赋值调用 setter 一下
        Radius = _radius;
        Divisions = _divisions;

        HexMetrics.NoiseSource = _noiseSource.GetImage();
        HexMetrics.InitializeHashGrid(Seed);
        DrawHexasphereMesh();
    }

    public override void _Process(double delta)
    {
        if (!_ready) return;
        _tileShaderService.UpdateData((float)delta);

        _lastUpdated += (float)delta;
        if (_lastUpdated < 0.1f) return; // 每 0.1s 更新一次
        if (Mathf.Abs(_oldRadius - Radius) > 0.001f
            || _oldDivisions != Divisions
            || _oldChunkDivisions != ChunkDivisions)
            DrawHexasphereMesh();
        if (!Engine.IsEditorHint())
            UpdateSelectTileViewer();
        _lastUpdated = 0f; // 每一秒检查一次
    }

    private void UpdateSelectTileViewer()
    {
        var position = GetTileCollisionPositionUnderCursor();
        if (_editMode)
            UpdateSelectTileInEditMode(position);
        else
            UpdateSelectTileInPlayMode(position);
    }

    private void UpdateSelectTileInPlayMode(Vector3 position)
    {
        if (_pathFromTileId == 0)
        {
            _selectTileViewer.Visible = false;
            return;
        }

        _selectTileViewer.Visible = true;
        var mesh = _selectViewService.GenerateMeshForPlayMode(_pathFromTileId, position);
        if (mesh != null)
            _selectTileViewer.Mesh = mesh;
    }

    private void UpdateSelectTileInEditMode(Vector3 position)
    {
        if (position != Vector3.Zero)
        {
            _selectTileViewer.Visible = true;
            var mesh = _selectViewService.GenerateMeshForEditMode(position);
            if (mesh != null)
                _selectTileViewer.Mesh = mesh;
        }
        else
        {
            // GD.Print("No tile under cursor, _selectTileViewer not visible");
            _selectTileViewer.Visible = false;
        }
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
            return position.AsVector3();
        return Vector3.Zero;
    }

    public Tile GetTileUnderCursor()
    {
        var pos = GetTileCollisionPositionUnderCursor();
        if (pos == Vector3.Zero) return null;
        var tileId = _tileService.SearchNearestTileId(pos.Normalized());
        return tileId == null ? null : _tileService.GetById((int)tileId);
    }

    private void ClearOldData()
    {
        _chunkService.Truncate();
        _tileService.Truncate();
        _pointService.Truncate();
        _faceService.Truncate();
        _aStarService.ClearOldData();
        _selectViewService.ClearPath();
        _gridChunks.Clear();
        foreach (var child in _chunks.GetChildren())
            child.QueueFree();
        ClearAllUnits();
    }

    private void DrawHexasphereMesh()
    {
        _oldRadius = Radius;
        _oldDivisions = Divisions;
        _oldChunkDivisions = ChunkDivisions;
        _lastUpdated = 0f;
        ClearOldData();
        _chunkService.InitChunks(ChunkDivisions);
        InitHexasphere();
        _aStarService.Init();
        _tileShaderService.Initialize();
        _tileSearchService.InitSearchData(_tileService.GetCount());
        RefreshAllTiles();
        EmitSignal(SignalName.NewPlanetGenerated);
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

    private void InitHexasphere()
    {
        GD.Print($"InitHexasphere with radius {Radius}, divisions {Divisions}, start at: {Time.GetTicksMsec()}");
        _pointService.SubdivideIcosahedron(Divisions);
        _tileService.InitTiles();
        BuildMesh();
    }

    private void BuildMesh()
    {
        var time = Time.GetTicksMsec();
        foreach (var id in _chunkService.GetAll().Select(chunk => chunk.Id))
        {
            var hexGridChunk = _gridChunkScene.Instantiate<HexGridChunk>();
            hexGridChunk.Name = $"HexGridChunk{id}";
            _chunks.AddChild(hexGridChunk); // 必须先加入场景树，否则 _Ready() 还没执行
            hexGridChunk.Init(id);
            _gridChunks.Add(id, hexGridChunk);
        }

        GD.Print($"BuildMesh cost: {Time.GetTicksMsec() - time} ms");
    }

    public void SetEditMode(bool mode)
    {
        _editMode = mode;
        RenderingServer.GlobalShaderParameterSet("hex_map_edit_mode", mode);
        PathFromTileId = 0;
        UpdateSelectTileViewer();
        foreach (var gridChunk in _gridChunks.Values)
            gridChunk.ShowUi(!mode); // 游戏模式下才显示地块 UI
    }

    public void FindPath(Tile tile)
    {
        if (_pathFromTileId != 0)
        {
            if (tile == null || tile.Id == _pathFromTileId)
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
        var fromTile = _tileService.GetById(_pathFromTileId);
        var path = _aStarService.FindPath(fromTile, toTile);
        if (path is { Count: > 1 })
        {
            // 确实有找到从出发点到 tile 的路径
            var unit = _units[fromTile.UnitId];
            _hexUnitPathPool.NewTask(unit, path, toTile.Id);
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
}