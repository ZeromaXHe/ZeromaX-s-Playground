using System.Collections.Generic;
using System.Linq;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Constant;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Node;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet;

[Tool]
public partial class HexPlanetManager : Node3D
{
    [Export(PropertyHint.Range, "5, 1000")]
    private float _radius = 10f;

    [Export(PropertyHint.Range, "1, 100")] private int _divisions = 4;

    [Export(PropertyHint.Range, "0.1f, 1f")]
    private float _hexSize = 1f;

    private SurfaceTool _surfaceTool;
    private MeshInstance3D _meshIns;

    private float _oldRadius;
    private int _oldDivisions;
    private float _oldHexSize;
    private float _lastUpdated;

    private readonly HashSet<int> _framePointIds = [];

    private Node3D _tiles;

    public override void _Ready()
    {
        _tiles = GetNode<Node3D>("Tiles");
        DrawHexasphereMesh();
    }

    public override void _Process(double delta)
    {
        _lastUpdated += (float)delta;
        if (_lastUpdated < 1f) return;
        if (Mathf.Abs(_oldRadius - _radius) > 0.001f || _oldDivisions != _divisions ||
            Mathf.Abs(_oldHexSize - _hexSize) > 0.001f)
        {
            DrawHexasphereMesh();
        }
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
        _oldRadius = _radius;
        _oldDivisions = _divisions;
        _oldHexSize = _hexSize;
        _lastUpdated = 0f;
        ClearOldData();
        InitHexasphere();
    }

    private void InitHexasphere()
    {
        var time = Time.GetTicksMsec();
        GD.Print($"InitHexasphere with radius {_radius}, divisions {_divisions}, start at: {time}");

        SubdivideIcosahedron();
        var time2 = Time.GetTicksMsec();
        GD.Print($"SubdivideIcosahedron cost: {time2 - time} ms");
        time = time2;

        ConstructTiles();
        time2 = Time.GetTicksMsec();
        GD.Print($"ConstructTiles cost: {time2 - time} ms");
        time = time2;

        foreach (var tile in Tile.GetAll())
        {
            var tileNode = new TileNode();
            tileNode.InitTileNode(tile.Id, _radius, _hexSize);
            _tiles.AddChild(tileNode);
        }

        time2 = Time.GetTicksMsec();
        GD.Print($"InitTileNodes cost: {time2 - time} ms");
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
            var leftSide = Subdivide(p0, p1, _divisions, true);
            var rightSide = Subdivide(p0, p2, _divisions, true);
            for (var i = 1; i <= _divisions; i++)
            {
                var previousPoints = bottomSide;
                bottomSide = Subdivide(leftSide[i], rightSide[i], i, i == _divisions);
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
                var v = from.Position.Lerp(target.Position, (float)i / count);
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
            Tile.Add(point.Id, hexFaces.Select(f => f.Id).ToList(), neighborCenters, GD.Randf() * _radius * 0.1f);
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
}