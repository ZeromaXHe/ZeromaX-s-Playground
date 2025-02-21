using System.Collections.Generic;
using System.Linq;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Constant;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Node;
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

    private readonly HashSet<int> _framePointIds = [];
    private readonly VpTree<Vector3> _tilePointVpTree = new();
    private readonly VpTree<Vector3> _chunkPointVpTree = new();
    private readonly Dictionary<int, HexGridChunk> _gridChunks = new();

    private FogVolume _atmosphereFog;
    private Node3D _chunks;
    private OrbitCamera _orbitCamera;
    private MeshInstance3D _selectTileViewer;
    private int? _selectTileCenterId;
    public int SelectViewSize { get; set; }

    public override void _Ready()
    {
        _atmosphereFog = GetNode<FogVolume>("%AtmosphereFog");
        _chunks = GetNode<Node3D>("%Chunks");
        // 此处要求 OrbitCamera 也是 [Tool]，否则编辑器里会转型失败
        _orbitCamera = GetNode<OrbitCamera>("%OrbitCamera");
        _selectTileViewer = GetNode<MeshInstance3D>("%SelectTileViewer");

        HexMetrics.NoiseSource = _noiseSource.GetImage();
        DrawHexasphereMesh();
        _ready = true;
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
            _tilePointVpTree.Search(position.Normalized(), 1, out var results, out var distances);
            var centerId = Point.GetIdByPosition(results[0]);
            if (centerId != null)
            {
                if (centerId == _selectTileCenterId)
                {
                    // GD.Print(
                    //     $"Same tile! centerId: {centerId}, result: {results[0]}, position: {position}, dist: {string.Join(", ", distances)}");
                    return;
                }

                // GD.Print(
                //     $"Generating New _selectTileViewer Mesh! {centerId}, result: {results[0]}, position: {position}, dist: {string.Join(", ", distances)}");
                _selectTileCenterId = centerId;
                var tile = Tile.GetByCenterId((int)_selectTileCenterId);
                _selectTileViewer.Mesh = GenFlatTileMesh(tile, 1f + HexMetrics.MaxHeightRadiusRatio);
            }
            else
            {
                GD.PrintErr($"centerId not found! position: {results[0]}");
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
        var tiles = tile.GetTilesInDistance(SelectViewSize);
        var vi = 0;
        foreach (var t in tiles)
        {
            var points = t.GetCorners(Radius * scale).ToList();
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
        if (pos == Vector3.Zero) return null;
        _tilePointVpTree.Search(pos.Normalized(), 1, out var results, out _);
        return Point.GetIdByPosition(results[0]);
    }

    private void ClearOldData()
    {
        Chunk.Truncate();
        Tile.Truncate();
        Point.Truncate();
        Face.Truncate();
        _framePointIds.Clear();
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
        InitChunks();
        InitHexasphere();
        EmitSignal(SignalName.NewPlanetGenerated);
    }

    private void InitChunks()
    {
        var time = Time.GetTicksMsec();
        var points = IcosahedronConstants.Vertices;
        var indices = IcosahedronConstants.Indices;
        var framePoints = new List<Vector3>(points);
        foreach (var v in points)
            Chunk.Add(v);
        for (var idx = 0; idx < indices.Count; idx += 3)
        {
            var p0 = points[indices[idx]];
            var p1 = points[indices[idx + 1]];
            var p2 = points[indices[idx + 2]];
            var leftSide = Subdivide(p0, p1, ChunkDivisions, true);
            var rightSide = Subdivide(p0, p2, ChunkDivisions, true);
            for (var i = 1; i <= ChunkDivisions; i++)
                Subdivide(leftSide[i], rightSide[i], i, i == ChunkDivisions);
        }

        _chunkPointVpTree.Create(Chunk.GetAll().Select(c => c.Pos).ToArray(),
            (p0, p1) => p0.DistanceTo(p1));
        GD.Print($"InitChunks radius {Radius}, chunkDivisions {ChunkDivisions}, cost: {Time.GetTicksMsec() - time}");
        return;

        List<Vector3> Subdivide(Vector3 from, Vector3 target, int count, bool checkFrameExist)
        {
            var segments = new List<Vector3> { from };

            for (var i = 1; i < count; i++)
            {
                // 注意这里用 Slerp 而不是 Lerp，让所有的点都在单位球面而不是单位正二十面体上，方便我们后面 VP 树找最近点
                var v = from.Slerp(target, (float)i / count);
                Vector3 newPoint = default;
                if (checkFrameExist)
                {
                    var existingPoint = framePoints.FirstOrDefault(candidatePoint => candidatePoint.IsEqualApprox(v));
                    if (existingPoint != default)
                        newPoint = existingPoint;
                }

                if (newPoint == default)
                {
                    newPoint = v;
                    Chunk.Add(v);
                    if (checkFrameExist)
                        framePoints.Add(newPoint);
                }

                segments.Add(newPoint);
            }

            segments.Add(target);
            return segments;
        }
    }

    private void InitHexasphere()
    {
        var time = Time.GetTicksMsec();
        GD.Print($"InitHexasphere with radius {Radius}, divisions {Divisions}, start at: {time}");

        SubdivideIcosahedron();
        var time2 = Time.GetTicksMsec();
        GD.Print($"SubdivideIcosahedron cost: {time2 - time} ms");
        time = time2;

        ConstructTiles();
        time2 = Time.GetTicksMsec();
        GD.Print($"ConstructTiles cost: {time2 - time} ms");
        time = time2;

        _tilePointVpTree.Create(Tile.GetAll().Select(p => Point.GetById(p.CenterId).Position).ToArray(),
            (p0, p1) => p0.DistanceTo(p1));
        time2 = Time.GetTicksMsec();
        GD.Print($"_tilePointVpTree Create cost: {time2 - time} ms");

        BuildMesh();
    }

    private void SubdivideIcosahedron()
    {
        var points = IcosahedronConstants.Vertices
            .Select(v =>
            {
                var p = Point.Add(v);
                _framePointIds.Add(p.Id);
                return p;
            })
            .ToList();
        var indices = IcosahedronConstants.Indices;
        for (var idx = 0; idx < indices.Count; idx += 3)
        {
            var p0 = points[indices[idx]];
            var p1 = points[indices[idx + 1]];
            var p2 = points[indices[idx + 2]];
            var bottomSide = new List<Point> { p0 };
            var leftSide = Subdivide(p0, p1, Divisions, true);
            var rightSide = Subdivide(p0, p2, Divisions, true);
            for (var i = 1; i <= Divisions; i++)
            {
                var previousPoints = bottomSide;
                bottomSide = Subdivide(leftSide[i], rightSide[i], i, i == Divisions);
                for (var j = 0; j < i; j++)
                {
                    Face.Add(previousPoints[j], bottomSide[j], bottomSide[j + 1]);
                    if (j == 0) continue;
                    Face.Add(previousPoints[j - 1], previousPoints[j], bottomSide[j]);
                }
            }
        }

        return;

        List<Point> Subdivide(Point from, Point target, int count, bool checkFrameExist)
        {
            var segments = new List<Point> { from };

            for (var i = 1; i < count; i++)
            {
                // 注意这里用 Slerp 而不是 Lerp，让所有的 Point 都在单位球面而不是单位正二十面体上，方便我们后面 VP 树找最近点
                var v = from.Position.Slerp(target.Position, (float)i / count);
                Point newPoint = null;
                if (checkFrameExist)
                {
                    var existingPoint = _framePointIds.Select(Point.GetById)
                        .FirstOrDefault(candidatePoint => candidatePoint.IsOverlapping(v));
                    if (existingPoint != null)
                        newPoint = existingPoint;
                }

                if (newPoint == null)
                {
                    newPoint = Point.Add(v);
                    if (checkFrameExist)
                        _framePointIds.Add(newPoint.Id);
                }

                segments.Add(newPoint);
            }

            segments.Add(target);
            return segments;
        }
    }

    private void ConstructTiles()
    {
        foreach (var point in Point.GetAll())
        {
            var hexFaces = GetOrderedFaces(point);
            var neighborCenters = GetNeighbourCenterIds(hexFaces, point)
                .Select(c => c.Id)
                .ToList();
            _chunkPointVpTree.Search(point.Position, 1, out var results, out _);
            var chunk = Chunk.GetByPos(results[0]);
            var tile = Tile.Add(point.Id, chunk.Id, hexFaces.Select(f => f.Id).ToList(), neighborCenters);
            chunk.TileIds.Add(tile.Id);
        }

        return;

        List<Face> GetOrderedFaces(Point center)
        {
            var faces = center.FaceIds.Select(Face.GetById).ToList();
            if (faces.Count == 0) return faces;
            // 第二个面必须保证和第一个面形成顺时针方向，从而保证所有都是顺时针
            var second =
                faces.First(face =>
                    face.Id != faces[0].Id
                    && face.IsAdjacentTo(faces[0])
                    && Math3dUtil.IsRightVSeq(Vector3.Zero, center.Position, faces[0].Center, face.Center));
            var orderedList = new List<Face> { faces[0], second };
            var currentFace = orderedList[1];
            while (orderedList.Count < faces.Count)
            {
                var existingIds = orderedList.Select(face => face.Id).ToList();
                var neighbour = faces.First(face =>
                    !existingIds.Contains(face.Id) && face.IsAdjacentTo(currentFace));
                currentFace = neighbour;
                orderedList.Add(currentFace);
            }

            return orderedList;
        }

        List<Point> GetNeighbourCenterIds(List<Face> hexFaces, Point center)
        {
            var neighbourCenters = new List<Point>();
            foreach (var p in
                     from face in hexFaces
                     from p in face.GetOtherPoints(center)
                     where !neighbourCenters.Contains(p)
                     select p)
                neighbourCenters.Add(p);
            return neighbourCenters;
        }
    }

    private void BuildMesh()
    {
        var time = Time.GetTicksMsec();
        for (var i = 0; i < Chunk.GetCount(); i++)
        {
            var hexGridChunk = _gridChunkScene.Instantiate<HexGridChunk>();
            hexGridChunk.Name = $"HexGridChunk{i}";
            _chunks.AddChild(hexGridChunk); // 必须先加入场景树，否则 _Ready() 还没执行
            hexGridChunk.Init(i, Radius);
            _gridChunks.Add(i, hexGridChunk);
        }

        GD.Print($"BuildMesh cost: {Time.GetTicksMsec() - time} ms");
    }

    public void UpdateMesh(Tile tile)
    {
        _gridChunks[tile.ChunkId].BuildMesh();
        foreach (var neighbor in tile.GetNeighbors())
            if (neighbor.ChunkId != tile.ChunkId)
                _gridChunks[neighbor.ChunkId].BuildMesh();
    }
}