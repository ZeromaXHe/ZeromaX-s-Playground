using System.Collections.Generic;
using System.Linq;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Repository;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Node;

public partial class TileNode : MeshInstance3D
{
    private int _id;
    private HexPlanetRepository _repo;
    private int _verticesCount;

    public void InitTileNode(HexPlanetRepository repo, int id, float radius, float size)
    {
        _id = id;
        _repo = repo;

        var tile = _repo.GetTileByCenterId(_id);
        var surfaceTool = new SurfaceTool();
        surfaceTool.Begin(Mesh.PrimitiveType.Triangles);
        var points = GetTilePoints(tile, radius, size);
        var scale = (radius + tile.Height) / radius;
        BuildFlatFace(surfaceTool, points, scale);
        BuildCliffFaces(surfaceTool, points, scale, tile, radius, size);
        surfaceTool.GenerateNormals();
        var material = new StandardMaterial3D();
        material.VertexColorUseAsAlbedo = true;
        surfaceTool.SetMaterial(material);
        Mesh = surfaceTool.Commit();
    }

    private void BuildCliffFaces(SurfaceTool surfaceTool, List<Vector3> points, float scale, Tile tile, float radius,
        float size)
    {
        var tileCenter = _repo.GetPointById(tile.CenterId).Position;
        var lowerNeighbors = tile.NeighborCenterIds
            .Select(_repo.GetTileByCenterId)
            .Where(t => t.Height < tile.Height);
        var commonPoints = new List<Vector3>();
        foreach (var lower in lowerNeighbors)
        {
            var lowerPoints = GetTilePoints(lower, radius, size);
            commonPoints.Clear();
            foreach (var lowerP in lowerPoints)
            {
                foreach (var p in points)
                {
                    if (p.IsEqualApprox(lowerP))
                    {
                        commonPoints.Add(p);
                        break;
                    }
                }
            }
            if (commonPoints.Count != 2)
            {
                GD.Print("Error: tile has no 2 common points with lower neighbor");
                continue;
            }

            var lowerScale = (radius + lower.Height) / radius;
            var v0 = commonPoints[0] * lowerScale;
            var v1 = commonPoints[1] * lowerScale;
            var v2 = commonPoints[0] * scale;
            var v3 = commonPoints[1] * scale;
            surfaceTool.AddVertex(v0);
            surfaceTool.AddVertex(v1);
            surfaceTool.AddVertex(v2);
            surfaceTool.AddVertex(v3);
            var normal1 = GetNormal(v0, v1, v2);
            var center1 = (v0 + v1 + v2) / 3f;
            surfaceTool.AddIndex(_verticesCount);
            if (IsNormalPointingAwayFromOrigin(center1, normal1, tileCenter))
            {
                surfaceTool.AddIndex(_verticesCount + 2);
                surfaceTool.AddIndex(_verticesCount + 1);
            }
            else
            {
                surfaceTool.AddIndex(_verticesCount + 1);
                surfaceTool.AddIndex(_verticesCount + 2);
            }
            var normal2 = GetNormal(v3, v1, v2);
            var center2 = (v3 + v1 + v2) / 3f;
            surfaceTool.AddIndex(_verticesCount + 3);
            if (IsNormalPointingAwayFromOrigin(center2, normal2, tileCenter))
            {
                surfaceTool.AddIndex(_verticesCount + 2);
                surfaceTool.AddIndex(_verticesCount + 1);
            }
            else
            {
                surfaceTool.AddIndex(_verticesCount + 1);
                surfaceTool.AddIndex(_verticesCount + 2);
            }
            _verticesCount += 4;
        }
    }

    private List<Vector3> GetTilePoints(Tile tile, float radius, float size)
    {
        var points = new List<Vector3>();
        foreach (var faceId in tile.HexFaceIds)
        {
            var face = _repo.GetFaceById(faceId);
            var center = _repo.GetPointById(tile.CenterId);
            var pos = center.Position.Lerp(GetCenter(face), size);
            points.Add(ProjectToShpere(pos, radius, 1f));
        }

        return points;

        Vector3 GetCenter(Face face)
        {
            var center = _repo.GetFacePointsById(face.Id)
                .Select(p => p.Position)
                .Aggregate(Vector3.Zero, (a, b) => a + b);
            return center / 3f;
        }
    }

    private static Vector3 ProjectToShpere(Vector3 p, float radius, float t)
    {
        var projectionPoint = radius / p.Length();
        return p * projectionPoint * t;
    }

    private void BuildFlatFace(SurfaceTool surfaceTool, List<Vector3> points, float scale)
    {
        surfaceTool.SetColor(Color.FromHsv(GD.Randf(), GD.Randf(), GD.Randf()));
        foreach (var point in points)
        {
            surfaceTool.AddVertex(point * scale);
            _verticesCount++;
        }

        AddFaceIndex(points[0], 0, points[1], 1, points[2], 2, surfaceTool);
        AddFaceIndex(points[0], 0, points[2], 2, points[3], 3, surfaceTool);
        AddFaceIndex(points[0], 0, points[3], 3, points[4], 4, surfaceTool);
        if (points.Count > 5)
            AddFaceIndex(points[0], 0, points[4], 4, points[5], 5, surfaceTool);
        return;

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

    private static Vector3 GetNormal(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        var side1 = v2 - v1;
        var side2 = v3 - v1;
        var cross = side1.Cross(side2);
        return cross / cross.Length();
    }

    private static bool IsNormalPointingAwayFromOrigin(Vector3 surface, Vector3 normalVec) =>
        Vector3.Zero.DistanceTo(surface) < Vector3.Zero.DistanceTo(surface + normalVec);

    private static bool IsNormalPointingAwayFromOrigin(Vector3 surface, Vector3 normalVec, Vector3 origin) =>
        (surface - origin).Dot(normalVec) > 0;
}