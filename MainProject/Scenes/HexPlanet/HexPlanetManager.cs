using System.Collections.Generic;
using System.Linq;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Dependency;
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
    public HexPlanetManager() => Context.Init();
    [Signal]
    public delegate void NewPlanetGeneratedEventHandler();

    [Export(PropertyHint.Range, "5, 1000")]
    public float Radius { get; set; } = 100f;

    [Export(PropertyHint.Range, "1, 100")] public int Divisions { get; set; } = 20;
    [Export(PropertyHint.Range, "1, 25")] public int ChunkDivisions { get; set; } = 5;
    [Export] private Texture2D _noiseSource;
    [Export] private PackedScene _gridChunkScene;
    [Export] public ulong Seed { get; set; } = 1234;

    private bool _ready;

    private float _oldRadius;
    private int _oldDivisions;
    private int _oldChunkDivisions;
    private float _lastUpdated;

    private readonly Dictionary<int, HexGridChunk> _gridChunks = new();

    #region services

    private IChunkService _chunkService;
    private ITileService _tileService;
    private IFaceService _faceService;
    private IPointService _pointService;
    private ISelectViewService _selectViewService;

    private void InitServices()
    {
        _chunkService = Context.GetBean<IChunkService>();
        _tileService = Context.GetBean<ITileService>();
        _faceService = Context.GetBean<IFaceService>();
        _pointService = Context.GetBean<IPointService>();
        _selectViewService = Context.GetBean<ISelectViewService>();
        _chunkService.RefreshChunk += id => _gridChunks[id].Refresh();
    }

    #endregion

    #region on-ready nodes

    private FogVolume _atmosphereFog;
    private Node3D _chunks;
    private OrbitCamera _orbitCamera;
    private MeshInstance3D _selectTileViewer;

    private void InitOnReadyNodes()
    {
        _atmosphereFog = GetNode<FogVolume>("%AtmosphereFog");
        _chunks = GetNode<Node3D>("%Chunks");
        // 此处要求 OrbitCamera 也是 [Tool]，否则编辑器里会转型失败
        _orbitCamera = GetNode<OrbitCamera>("%OrbitCamera");
        _selectTileViewer = GetNode<MeshInstance3D>("%SelectTileViewer");
    }

    #endregion

    public override void _Ready()
    {
        InitOnReadyNodes();
        InitServices();

        HexMetrics.NoiseSource = _noiseSource.GetImage();
        HexMetrics.InitializeHashGrid(Seed);
        DrawHexasphereMesh();
        _ready = true;
    }

    public override void _Process(double delta)
    {
        _lastUpdated += (float)delta;
        if (!_ready || _lastUpdated < 0.1f) return; // 每 0.1s 更新一次
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
        if (position != Vector3.Zero)
        {
            _selectTileViewer.Visible = true;
            var mesh = _selectViewService.GenerateMesh(position, Radius);
            if (mesh != null)
                _selectTileViewer.Mesh = mesh;
        }
        else
        {
            // GD.Print("No tile under cursor, _selectTileViewer not visible");
            _selectTileViewer.Visible = false;
            _lastUpdated = 0f;
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

    public int? GetTileIdUnderCursor()
    {
        var pos = GetTileCollisionPositionUnderCursor();
        return pos == Vector3.Zero ? null : _tileService.SearchNearestTileId(pos.Normalized());
    }

    private void ClearOldData()
    {
        _chunkService.ClearData();
        _tileService.ClearData();
        _pointService.ClearData();
        _faceService.ClearData();
        _gridChunks.Clear();
        foreach (var child in _chunks.GetChildren())
            child.QueueFree();
    }

    private void DrawHexasphereMesh()
    {
        _oldRadius = Radius;
        _oldDivisions = Divisions;
        _oldChunkDivisions = ChunkDivisions;
        _lastUpdated = 0f;
        ClearOldData();
        _orbitCamera.Reset(Radius);
        _atmosphereFog.Size = Vector3.One * Radius * 2.7f;
        _chunkService.InitChunks(ChunkDivisions);
        InitHexasphere();
        EmitSignal(SignalName.NewPlanetGenerated);
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
            hexGridChunk.Init(id, Radius);
            _gridChunks.Add(id, hexGridChunk);
        }

        GD.Print($"BuildMesh cost: {Time.GetTicksMsec() - time} ms");
    }
}