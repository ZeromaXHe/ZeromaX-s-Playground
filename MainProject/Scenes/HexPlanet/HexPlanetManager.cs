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
    public float Radius { get; set; } = 10f;

    [Export(PropertyHint.Range, "1, 100")] public int Divisions { get; set; } = 4;

    [Export(PropertyHint.Range, "0.1f, 1f")]
    public float HexSize { get; private set; } = 1f;

    private bool _ready;

    private SurfaceTool _surfaceTool;
    private int _verticesCount;
    private static readonly Material DefaultMaterial = new StandardMaterial3D { VertexColorUseAsAlbedo = true };
    private MeshInstance3D _meshIns;

    private float _oldRadius;
    private int _oldDivisions;
    private float _oldHexSize;
    private float _lastUpdated;

    private readonly HashSet<int> _framePointIds = [];
    private readonly VpTree<Vector3> _pointVpTree = new();

    private FogVolume _atmosphereFog;
    private Node3D _tiles;
    private OrbitCamera _orbitCamera;
    private MeshInstance3D _selectTileViewer;
    private int? _selectTileCenterId;

    public override void _Ready()
    {
        _atmosphereFog = GetNode<FogVolume>("%AtmosphereFog");
        _tiles = GetNode<Node3D>("%Tiles");
        // 此处要求 OrbitCamera 也是 [Tool]，否则编辑器里会转型失败
        _orbitCamera = GetNode<OrbitCamera>("%OrbitCamera");
        _selectTileViewer = GetNode<MeshInstance3D>("%SelectTileViewer");

        _surfaceTool = new SurfaceTool();
        _meshIns = new MeshInstance3D();
        AddChild(_meshIns);

        DrawHexasphereMesh();
        _ready = true;
    }

    public override void _Process(double delta)
    {
        _lastUpdated += (float)delta;
        if (!_ready || _lastUpdated < 1f) return;
        if (Mathf.Abs(_oldRadius - Radius) > 0.001f || _oldDivisions != Divisions ||
            Mathf.Abs(_oldHexSize - HexSize) > 0.001f)
        {
            DrawHexasphereMesh();
        }

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
            _pointVpTree.Search(position.Normalized(), 1, out var results, out var distances);
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
                _selectTileViewer.Mesh = GenFlatTileMesh(tile, 1.1f);
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
        // surfaceTool.SetSmoothGroup(uint.MaxValue);
        var points = tile.GetPoints(Radius, HexSize);
        foreach (var point in points)
            surfaceTool.AddVertex(point * scale);
        Math3dUtil.AddFaceIndex(surfaceTool, Vector3.Zero, points[0], 0, points[1], 1, points[2], 2);
        Math3dUtil.AddFaceIndex(surfaceTool, Vector3.Zero, points[0], 0, points[2], 2, points[3], 3);
        Math3dUtil.AddFaceIndex(surfaceTool, Vector3.Zero, points[0], 0, points[3], 3, points[4], 4);
        if (points.Count > 5)
            Math3dUtil.AddFaceIndex(surfaceTool, Vector3.Zero, points[0], 0, points[4], 4, points[5], 5);
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
        _pointVpTree.Search(pos.Normalized(), 1, out var results, out var distances);
        return Point.GetIdByPosition(results[0]);
    }

    private void ClearOldData()
    {
        Tile.Truncate();
        Point.Truncate();
        Face.Truncate();
        _framePointIds.Clear();
        foreach (var child in _tiles.GetChildren())
            child.QueueFree();
    }

    private void DrawHexasphereMesh()
    {
        _oldRadius = Radius;
        _oldDivisions = Divisions;
        _oldHexSize = HexSize;
        _lastUpdated = 0f;
        ClearOldData();
        _orbitCamera.Reset(Radius);
        InitHexasphere();
        _atmosphereFog.Size = Vector3.One * Radius * 2.7f;
        _pointVpTree.Create(Tile.GetAll().Select(p => Point.GetById(p.CenterId).Position).ToArray(),
            (p0, p1) => p0.DistanceTo(p1));
        EmitSignal(SignalName.NewPlanetGenerated);
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
            var height = GD.Randf() * Radius * 0.1f;
            Tile.Add(point.Id, hexFaces.Select(f => f.Id).ToList(), neighborCenters, height);
        }

        return;

        List<Face> GetOrderedFaces(Point center)
        {
            var faces = center.FaceIds.Select(Face.GetById).ToList();
            if (faces.Count == 0) return faces;
            var orderedList = new List<Face> { faces[0] };
            var currentFace = orderedList[0];
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

    public void BuildMesh()
    {
        var time = Time.GetTicksMsec();
        _verticesCount = 0;
        // 清理之前的碰撞体
        foreach (var child in _meshIns.GetChildren())
            child.QueueFree();

        _surfaceTool.Clear();
        _surfaceTool.Begin(Mesh.PrimitiveType.Triangles);
        _surfaceTool.SetSmoothGroup(uint.MaxValue);

        foreach (var tile in Tile.GetAll())
        {
            var points = tile.GetPoints(Radius, HexSize);
            var scale = (Radius + tile.Height) / Radius;
            BuildFlatFace(points, scale);
            if (Mathf.Abs(HexSize - 1f) < 0.00001f) // 1.0f 时才构建悬崖立面
                BuildCliffFaces(points, scale, tile);
        }

        _surfaceTool.GenerateNormals();
        _surfaceTool.SetMaterial(DefaultMaterial);
        _meshIns.Mesh = _surfaceTool.Commit();
        _meshIns.CreateTrimeshCollision();
        GD.Print($"BuildMesh cost: {Time.GetTicksMsec() - time} ms");
    }

    private void BuildCliffFaces(List<Vector3> points, float scale, Tile tile)
    {
        var tileCenter = Point.GetById(tile.CenterId).Position;
        var lowerNeighbors = tile.NeighborCenterIds
            .Select(Tile.GetByCenterId)
            .Where(t => t.Height < tile.Height);
        var commonPoints = new List<Vector3>();
        foreach (var lower in lowerNeighbors)
        {
            commonPoints.Clear();
            var lowerPoints = lower.GetPoints(Radius, HexSize);
            foreach (var lowerP in lowerPoints)
            {
                foreach (var p in points.Where(p => p.IsEqualApprox(lowerP)))
                {
                    commonPoints.Add(p);
                    break;
                }
            }

            if (commonPoints.Count != 2)
            {
                GD.Print("Error: tile has no 2 common points with lower neighbor");
                continue;
            }

            var lowerScale = (Radius + lower.Height) / Radius;
            var v0 = commonPoints[0] * lowerScale;
            var v1 = commonPoints[1] * lowerScale;
            var v2 = commonPoints[0] * scale;
            var v3 = commonPoints[1] * scale;
            _surfaceTool.AddVertex(v0);
            _surfaceTool.AddVertex(v1);
            _surfaceTool.AddVertex(v2);
            _surfaceTool.AddVertex(v3);
            Math3dUtil.AddFaceIndex(_surfaceTool, tileCenter, v0, _verticesCount,
                v1, _verticesCount + 1, v2, _verticesCount + 2);
            Math3dUtil.AddFaceIndex(_surfaceTool, tileCenter, v1, _verticesCount + 1,
                v2, _verticesCount + 2, v3, _verticesCount + 3);

            _verticesCount += 4;
        }
    }

    private void BuildFlatFace(List<Vector3> points, float scale)
    {
        _surfaceTool.SetColor(Color.FromHsv(GD.Randf(), GD.Randf(), GD.Randf()));
        foreach (var point in points)
        {
            _surfaceTool.AddVertex(point * scale);
        }

        Math3dUtil.AddFaceIndex(_surfaceTool, Vector3.Zero, points[0], _verticesCount,
            points[1], _verticesCount + 1, points[2], _verticesCount + 2);
        Math3dUtil.AddFaceIndex(_surfaceTool, Vector3.Zero, points[0], _verticesCount,
            points[2], _verticesCount + 2, points[3], _verticesCount + 3);
        Math3dUtil.AddFaceIndex(_surfaceTool, Vector3.Zero, points[0], _verticesCount,
            points[3], _verticesCount + 3, points[4], _verticesCount + 4);
        if (points.Count > 5)
            Math3dUtil.AddFaceIndex(_surfaceTool, Vector3.Zero, points[0], _verticesCount,
                points[4], _verticesCount + 4, points[5], _verticesCount + 5);

        _verticesCount += points.Count;
    }
}