using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Node;

public partial class TileNode : MeshInstance3D
{
    private int _id;
    private int _verticesCount;

    public void InitTileNode(int id, float radius, float size)
    {
        _id = id;

        var tile = Tile.GetByCenterId(_id);
        var surfaceTool = new SurfaceTool();
        surfaceTool.Begin(Mesh.PrimitiveType.Triangles);
        surfaceTool.SetSmoothGroup(uint.MaxValue);
        var points = tile.GetPoints(radius, size);
        var scale = (radius + tile.Height) / radius;
        BuildFlatFace(surfaceTool, points, scale);
        if (Math.Abs(size - 1f) < 0.00001f)
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
        var tileCenter = Point.GetById(tile.CenterId).Position;
        var lowerNeighbors = tile.NeighborCenterIds
            .Select(Tile.GetByCenterId)
            .Where(t => t.Height < tile.Height);
        var commonPoints = new List<Vector3>();
        foreach (var lower in lowerNeighbors)
        {
            var lowerPoints = lower.GetPoints(radius, size);
            commonPoints.Clear();
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

            var lowerScale = (radius + lower.Height) / radius;
            var v0 = commonPoints[0] * lowerScale;
            var v1 = commonPoints[1] * lowerScale;
            var v2 = commonPoints[0] * scale;
            var v3 = commonPoints[1] * scale;
            surfaceTool.AddVertex(v0);
            surfaceTool.AddVertex(v1);
            surfaceTool.AddVertex(v2);
            surfaceTool.AddVertex(v3);
            AddFaceIndex(surfaceTool, tileCenter, v0, _verticesCount,
                v1, _verticesCount + 1, v2, _verticesCount + 2);
            AddFaceIndex(surfaceTool, tileCenter, v1, _verticesCount + 1,
                v2, _verticesCount + 2, v3, _verticesCount + 3);

            _verticesCount += 4;
        }
    }

    private void BuildFlatFace(SurfaceTool surfaceTool, List<Vector3> points, float scale)
    {
        surfaceTool.SetColor(Color.FromHsv(GD.Randf(), GD.Randf(), GD.Randf()));
        foreach (var point in points)
        {
            surfaceTool.AddVertex(point * scale);
            _verticesCount++;
        }

        AddFaceIndex(surfaceTool, Vector3.Zero, points[0], 0, points[1], 1, points[2], 2);
        AddFaceIndex(surfaceTool, Vector3.Zero, points[0], 0, points[2], 2, points[3], 3);
        AddFaceIndex(surfaceTool, Vector3.Zero, points[0], 0, points[3], 3, points[4], 4);
        if (points.Count > 5)
            AddFaceIndex(surfaceTool, Vector3.Zero, points[0], 0, points[4], 4, points[5], 5);
    }

    private static void AddFaceIndex(SurfaceTool surfaceTool, Vector3 origin, Vector3 v0, int i0, Vector3 v1, int i1,
        Vector3 v2, int i2)
    {
        var center = (v0 + v1 + v2) / 3f;
        // 决定缠绕顺序
        var normal = Math3dUtil.GetNormal(v0, v1, v2);
        surfaceTool.AddIndex(i0);
        if (Math3dUtil.IsNormalAwayFromOrigin(center, normal, origin))
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