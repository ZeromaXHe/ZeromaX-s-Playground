using System.Collections.Generic;
using System.Linq;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util;

public class HexMesh
{
    private readonly SurfaceTool _surfaceTool = new();
    private int _verticesCount;
    private static readonly Material DefaultMaterial = new StandardMaterial3D { VertexColorUseAsAlbedo = true };

    private float _radius;
    private float _hexSize;
    
    public Mesh BuildMesh(float radius, float hexSize)
    {
        _radius = radius;
        _hexSize = hexSize;
        _verticesCount = 0;
        _surfaceTool.Clear();
        _surfaceTool.Begin(Mesh.PrimitiveType.Triangles);
        _surfaceTool.SetSmoothGroup(uint.MaxValue);

        foreach (var tile in Tile.GetAll())
        {
            var points = tile.GetPoints(_radius, _hexSize);
            var scale = (_radius + tile.Height) / _radius;
            BuildFlatFace(tile, points, scale);
            if (Mathf.Abs(_hexSize - 1f) < 0.00001f) // 1.0f 时才构建悬崖立面
                BuildCliffFaces(points, scale, tile);
        }

        _surfaceTool.GenerateNormals();
        _surfaceTool.SetMaterial(DefaultMaterial);
        return _surfaceTool.Commit();
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
            var lowerPoints = lower.GetPoints(_radius, _hexSize);
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

            var lowerScale = (_radius + lower.Height) / _radius;
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

    private void BuildFlatFace(Tile tile, List<Vector3> points, float scale)
    {
        _surfaceTool.SetColor(tile.Color);
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