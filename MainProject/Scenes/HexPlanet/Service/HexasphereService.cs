using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Constant;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Node;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Repository;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service;

public class HexasphereService
{
    private readonly float _radius;
    private readonly int _divisions;
    private readonly float _hexSize;
    private readonly HexPlanetRepository _repo;

    public HexasphereService(float radius, int divisions, float hexSize)
    {
        var time = Time.GetTicksMsec();
        GD.Print($"new HexasphereService with radius {radius}, divisions {divisions}, start at: {time}");
        _radius = radius;
        _divisions = divisions;
        _hexSize = hexSize;
        _repo = new HexPlanetRepository();

        SubdivideIcosahedron();
        var time2 = Time.GetTicksMsec();
        GD.Print($"SubdivideIcosahedron cost: {time2 - time} ms");
        time = time2;
        ConstructTiles();
        time2 = Time.GetTicksMsec();
        GD.Print($"ConstructTiles cost: {time2 - time} ms");
    }

    private readonly HashSet<int> _framePointIds = [];

    private const float PointComparisonAccuracy = 0.0001f;

    private void SubdivideIcosahedron()
    {
        var points = IcosahedronConstants.Vertices
            .Select(v =>
            {
                var p = _repo.AddPoint(v);
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
                    InitFace(previousPoints[j], bottomSide[j], bottomSide[j + 1]);
                    if (j == 0) continue;
                    InitFace(previousPoints[j - 1], previousPoints[j], bottomSide[j]);
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
                    var existingPoint = _framePointIds.Select(id => _repo.GetPointById(id))
                        .FirstOrDefault(candidatePoint => IsOverlapping(candidatePoint, v));
                    if (existingPoint != null)
                        newPoint = existingPoint;
                }

                if (newPoint == null)
                {
                    newPoint = _repo.AddPoint(v);
                    if (checkFrameExist)
                        _framePointIds.Add(newPoint.Id);
                }

                segments.Add(newPoint);
            }

            segments.Add(target);
            return segments;

            static bool IsOverlapping(Point p, Vector3 v) =>
                Mathf.Abs(p.Position.X - v.X) <= PointComparisonAccuracy &&
                Mathf.Abs(p.Position.Y - v.Y) <= PointComparisonAccuracy &&
                Mathf.Abs(p.Position.Z - v.Z) <= PointComparisonAccuracy;
        }

        void InitFace(Point p0, Point p1, Point p2)
        {
            var center = (p0.Position + p1.Position + p2.Position) / 3f;
            // 决定缠绕顺序
            var normal = GetNormal(p0, p1, p2);
            List<int> pointIds = IsNormalPointingAwayFromOrigin(center, normal)
                ? [p0.Id, p2.Id, p1.Id]
                : [p0.Id, p1.Id, p2.Id];
            _repo.AddFace(center, pointIds);
        }
    }

    private static Vector3 GetNormal(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        var side1 = v2 - v1;
        var side2 = v3 - v1;
        var cross = side1.Cross(side2);
        return cross / cross.Length();
    }

    private static Vector3 GetNormal(Point point1, Point point2, Point point3) =>
        GetNormal(point1.Position, point2.Position, point3.Position);

    private static bool IsNormalPointingAwayFromOrigin(Vector3 surface, Vector3 normalVec) =>
        Vector3.Zero.DistanceTo(surface) < Vector3.Zero.DistanceTo(surface + normalVec);

    private void ConstructTiles()
    {
        foreach (var point in _repo.GetAllPoints())
        {
            var hexFaces = GetOrderedFaces(point);
            var neighborCenters = GetNeighbourCenterIds(hexFaces, point)
                .Select(c => c.Id)
                .ToList();
            _repo.AddTile(point.Id, hexFaces.Select(f => f.Id).ToList(), neighborCenters, GD.Randf() * _radius * 0.1f);
        }

        return;

        List<Face> GetOrderedFaces(Point center)
        {
            var faces = _repo.GetPointFacesByPointId(center.Id).ToList();
            if (faces.Count == 0) return faces;
            var orderedList = new List<Face> { faces[0] };
            var currentFace = orderedList[0];
            while (orderedList.Count < faces.Count)
            {
                var existingIds = orderedList.Select(face => face.Id).ToList();
                var neighbour = faces.First(face =>
                    !existingIds.Contains(face.Id) && IsAdjacentToFace(face, currentFace));
                currentFace = neighbour;
                orderedList.Add(currentFace);
            }

            return orderedList;

            bool IsAdjacentToFace(Face a, Face b)
            {
                var aIds = _repo.GetFacePointIdsById(a.Id);
                var bIds = _repo.GetFacePointIdsById(b.Id);
                return aIds.Intersect(bIds).ToList().Count == 2;
            }
        }

        List<Point> GetNeighbourCenterIds(List<Face> hexFaces, Point center)
        {
            var neighbourCenters = new List<Point>();
            foreach (var face in hexFaces)
            foreach (var p in GetOtherPoints(face, center))
                if (neighbourCenters.FirstOrDefault(centerPoint => centerPoint.Id == p.Id) == null)
                    neighbourCenters.Add(p);
            return neighbourCenters;

            List<Point> GetOtherPoints(Face face, Point point)
            {
                if (_repo.GetFacePointsById(face.Id).All(facePoint => facePoint.Id != point.Id))
                {
                    throw new ArgumentException("Given point must be one of the points on the face!");
                }

                return _repo.GetFacePointsById(face.Id).Where(p => p.Id != point.Id).ToList();
            }
        }
    }

    public IEnumerable<Tile> GetAllTiles() => _repo.GetAllTiles();
    
    public HexPlanetRepository GetRepo() => _repo;
    
    public void BuildFaces(SurfaceTool surfaceTool)
    {
        var time = Time.GetTicksMsec();
        var vi = 0;
        foreach (var tile in _repo.GetAllTiles())
        {
            var points = new List<Vector3>();
            foreach (var faceId in tile.HexFaceIds)
            {
                var face = _repo.GetFaceById(faceId);
                var center = _repo.GetPointById(tile.CenterId);
                var pos = center.Position.Lerp(GetCenter(face), _hexSize);
                points.Add(ProjectToShpere(pos, _radius, 0.5f));
            }

            surfaceTool.SetColor(Color.FromHsv(GD.Randf(), GD.Randf(), GD.Randf()));
            var heightMultiplier = (float)GD.RandRange(1, 1.05);
            foreach (var point in points)
                surfaceTool.AddVertex(point * heightMultiplier);
            AddFaceIndex(points[0], vi, points[1], vi + 1, points[2], vi + 2, surfaceTool);
            AddFaceIndex(points[0], vi, points[2], vi + 2, points[3], vi + 3, surfaceTool);
            AddFaceIndex(points[0], vi, points[3], vi + 3, points[4], vi + 4, surfaceTool);
            if (points.Count > 5)
                AddFaceIndex(points[0], vi, points[4], vi + 4, points[5], vi + 5, surfaceTool);

            vi += points.Count;
        }

        GD.Print($"BuildFaces cost: {Time.GetTicksMsec() - time} ms");
        return;

        Vector3 GetCenter(Face face)
        {
            var center = _repo.GetFacePointsById(face.Id)
                .Select(p => p.Position)
                .Aggregate(Vector3.Zero, (a, b) => a + b);
            return center / 3f;
        }

        static Vector3 ProjectToShpere(Vector3 p, float radius, float t)
        {
            var projectionPoint = radius / p.Length();
            return p * projectionPoint * t;
        }

        static void AddFaceIndex(Vector3 v0, int i0, Vector3 v1, int i1, Vector3 v2, int i2, SurfaceTool surfaceTool)
        {
            var center = (v0 + v1 + v2) / 3f;
            // 决定缠绕顺序
            var normal = GetNormal(v0, v1, v2);
            surfaceTool.AddIndex(i0);
            if (IsNormalPointingAwayFromOrigin(center, normal))
            {
                surfaceTool.AddIndex(i2);
                surfaceTool.AddIndex(i1);
            }
            else
            {
                surfaceTool.AddIndex(i1);
                surfaceTool.AddIndex(i2);
            }
        }
    }
}