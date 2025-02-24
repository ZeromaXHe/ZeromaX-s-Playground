using System.Collections.Generic;
using System.Linq;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Dependency;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Constant;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Node;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet;

[Tool]
public partial class HexPlanetManager : Node3D
{
    [Signal]
    public delegate void NewPlanetGeneratedEventHandler();

    [Export(PropertyHint.Range, "5, 1000")]
    public float Radius { get; set; } = 100f;

    [Export(PropertyHint.Range, "1, 100")] public int Divisions { get; set; } = 20;
    [Export(PropertyHint.Range, "1, 25")] public int ChunkDivisions { get; set; } = 5;
    [Export] private Texture2D _noiseSource;
    [Export] private PackedScene _gridChunkScene;

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

    #endregion

    #region on-ready nodes

    private FogVolume _atmosphereFog;
    private Node3D _chunks;
    private OrbitCamera _orbitCamera;
    private MeshInstance3D _selectTileViewer;

    #endregion

    private int? _selectTileCenterId;
    public int SelectViewSize { get; set; }

    public override void _Ready()
    {
        _atmosphereFog = GetNode<FogVolume>("%AtmosphereFog");
        _chunks = GetNode<Node3D>("%Chunks");
        // 此处要求 OrbitCamera 也是 [Tool]，否则编辑器里会转型失败
        _orbitCamera = GetNode<OrbitCamera>("%OrbitCamera");
        _selectTileViewer = GetNode<MeshInstance3D>("%SelectTileViewer");
        
        InitServices();

        HexMetrics.NoiseSource = _noiseSource.GetImage();
        DrawHexasphereMesh();
        _ready = true;
    }

    private void InitServices()
    {
        Context.Init();
        _chunkService = Context.GetBean<IChunkService>();
        _tileService = Context.GetBean<ITileService>();
        _faceService = Context.GetBean<IFaceService>();
        _pointService = Context.GetBean<IPointService>();
    }

    public override void _Process(double delta)
    {
        _lastUpdated += (float)delta;
        if (!_ready || _lastUpdated < 1f) return;
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
            var centerId = _tileService.SearchNearestTileId(position.Normalized());
            if (centerId != null)
            {
                if (centerId == _selectTileCenterId)
                {
                    // GD.Print($"Same tile! centerId: {centerId}, position: {position}");
                    return;
                }

                // GD.Print($"Generating New _selectTileViewer Mesh! {centerId}, position: {position}");
                _selectTileCenterId = centerId;
                var tile = _tileService.GetByCenterId((int)_selectTileCenterId);
                _selectTileViewer.Mesh = GenFlatTileMesh(tile, 1f + HexMetrics.MaxHeightRadiusRatio);
            }
            else
            {
                GD.PrintErr($"centerId not found! position: {position}");
            }
        }
        else
        {
            // GD.Print("No tile under cursor, _selectTileViewer not visible");
            _selectTileViewer.Visible = false;
            _lastUpdated = 0f;
        }
    }

    private Mesh GenFlatTileMesh(Tile tile, float scale)
    {
        var surfaceTool = new SurfaceTool();
        surfaceTool.Begin(Mesh.PrimitiveType.Triangles);
        surfaceTool.SetSmoothGroup(uint.MaxValue);
        var tiles = _tileService.GetTilesInDistance(tile, SelectViewSize);
        var vi = 0;
        foreach (var t in tiles)
        {
            var points = _tileService.GetCorners(t, Radius * scale).ToList();
            foreach (var p in points)
                surfaceTool.AddVertex(p);
            for (var i = 1; i < points.Count - 1; i++)
                if (Math3dUtil.IsRightVSeq(Vector3.Zero, points[0], points[i], points[i + 1]))
                {
                    surfaceTool.AddIndex(vi);
                    surfaceTool.AddIndex(vi + i);
                    surfaceTool.AddIndex(vi + i + 1);
                }
                else
                {
                    surfaceTool.AddIndex(vi + 0);
                    surfaceTool.AddIndex(vi + i + 1);
                    surfaceTool.AddIndex(vi + i);
                }

            vi += points.Count;
        }

        return surfaceTool.Commit();
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
        {
            return position.AsVector3();
        }

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

    public void UpdateMesh(Tile tile)
    {
        _gridChunks[tile.ChunkId].BuildMesh();
        foreach (var neighbor in _tileService.GetNeighbors(tile))
            if (neighbor.ChunkId != tile.ChunkId)
                _gridChunks[neighbor.ChunkId].BuildMesh();
    }
}