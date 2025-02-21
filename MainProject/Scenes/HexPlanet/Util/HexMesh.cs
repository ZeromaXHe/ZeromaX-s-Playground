using System.Linq;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util;

public class HexMesh
{
    private readonly SurfaceTool _surfaceTool = new();
    private int _vIdx;
    private static readonly Material DefaultMaterial = new StandardMaterial3D { VertexColorUseAsAlbedo = true };

    private float _radius;

    public Mesh BuildMesh(float radius)
    {
        _radius = radius;
        _vIdx = 0;
        _surfaceTool.Clear();
        _surfaceTool.Begin(Mesh.PrimitiveType.Triangles);
        _surfaceTool.SetSmoothGroup(uint.MaxValue);

        foreach (var tile in Tile.GetAll())
        {
            Triangulate(tile);
            // BuildCliffFaces(tile);
        }

        _surfaceTool.GenerateNormals();
        _surfaceTool.SetMaterial(DefaultMaterial);
        return _surfaceTool.Commit();
    }

    private void BuildCliffFaces(Tile tile)
    {
        var scale = (_radius + tile.Height) / _radius;
        var tileCenter = tile.GetCenter(_radius);
        var lowerNeighbors = tile.GetNeighbors()
            .Where(t => t.Height < tile.Height);
        foreach (var lower in lowerNeighbors)
        {
            var commonPoints = tile.GetNeighborCommonCorners(lower, _radius);
            if (commonPoints == null)
                continue;
            var lowerScale = (_radius + lower.Height) / _radius;
            var v0 = commonPoints[0] * lowerScale;
            var v1 = commonPoints[1] * lowerScale;
            var v2 = commonPoints[0] * scale;
            var v3 = commonPoints[1] * scale;
            AddTriangle(tileCenter, [v0, v1, v2]);
            AddTriangle(tileCenter, [v1, v2, v3]);
        }
    }

    private void Triangulate(Tile tile)
    {
        var center = tile.GetCenter(_radius + tile.Height);
        var corners = tile.GetCorners(_radius + tile.Height, 1f).ToList();
        for (var i = 1; i < corners.Count - 1; i++)
        {
            var vs = new[]
            {
                center.Lerp(corners[0], HexMetrics.SolidFactor),
                center.Lerp(corners[i], HexMetrics.SolidFactor),
                center.Lerp(corners[i + 1], HexMetrics.SolidFactor)
            };
            AddTriangle(Vector3.Zero, vs, TriColor(tile.Color));
        }

        for (var i = 0; i < corners.Count; i++)
        {
            var c1 = corners[i];
            var c2 = corners[(i + 1) % corners.Count];
            TriangulateConnection(tile, center, c1, c2);
        }
    }

    private void TriangulateConnection(Tile tile, Vector3 center, Vector3 c1, Vector3 c2)
    {
        var neighbor = tile.GetNeighborByDirection(c1 - center, c2 - center);
        // 连接将由更高的地块或 Id 更大的地块生成
        if (tile.Height < neighbor.Height || tile.Id < neighbor.Id) return;
        var neighborCenter = neighbor.GetCenter(_radius + neighbor.Height);
        var v1 = center.Lerp(c1, HexMetrics.SolidFactor);
        var v2 = center.Lerp(c2, HexMetrics.SolidFactor);
        var vn1 = neighborCenter.Lerp(c1, HexMetrics.SolidFactor);
        var vn2 = neighborCenter.Lerp(c2, HexMetrics.SolidFactor);
        AddQuad(Vector3.Zero, [v1, v2, vn1, vn2], QuadColor(tile.Color, neighbor.Color));

        var otherNeighbor1 = tile.GetNeighborsByDirection(c1 - center, neighbor.Id)[0];
        if (tile.Height > otherNeighbor1.Height || tile.Id > otherNeighbor1.Id)
        {
            // 连接角落的三角形由周围 3 个地块中最高或者一样高时 Id 最大的生成
            var onCenter = otherNeighbor1.GetCenter(_radius + otherNeighbor1.Height);
            var von1 = onCenter.Lerp(c1, HexMetrics.SolidFactor);
            AddTriangle(Vector3.Zero,
                [v1, vn1, von1],
                [tile.Color, neighbor.Color, otherNeighbor1.Color]);
        }

        var otherNeighbor2 = tile.GetNeighborsByDirection(c2 - center, neighbor.Id)[0];
        if (tile.Height > otherNeighbor2.Height || tile.Id > otherNeighbor2.Id)
        {
            // 连接角落的三角形由周围 3 个地块中最高或者一样高时 Id 最大的生成
            var onCenter = otherNeighbor2.GetCenter(_radius + otherNeighbor2.Height);
            var von2 = onCenter.Lerp(c2, HexMetrics.SolidFactor);
            AddTriangle(Vector3.Zero,
                [v2, vn2, von2],
                [tile.Color, neighbor.Color, otherNeighbor2.Color]);
        }
    }

    private static Color[] TriColor(Color c) => [c, c, c];
    private static Color[] QuadColor(Color c) => [c, c, c, c];
    private static Color[] QuadColor(Color c1, Color c2) => [c1, c1, c2, c2];

    private void AddTriangle(Vector3 origin, Vector3[] vs, Color[] cs = null)
    {
        var idx = Math3dUtil.SortVertices(origin, vs);
        for (var i = 0; i < 3; i++)
        {
            if (cs != null)
                _surfaceTool.SetColor(cs[idx[i]]);
            _surfaceTool.AddVertex(vs[idx[i]]);
        }

        _surfaceTool.AddIndex(_vIdx);
        _surfaceTool.AddIndex(_vIdx + 1);
        _surfaceTool.AddIndex(_vIdx + 2);
        _vIdx += 3;
    }

    private void AddQuad(Vector3 origin, Vector3[] vs, Color[] cs = null)
    {
        var idx = Math3dUtil.SortVertices(origin, vs);
        for (var i = 0; i < 4; i++)
        {
            if (cs != null)
                _surfaceTool.SetColor(cs[idx[i]]);
            _surfaceTool.AddVertex(vs[idx[i]]);
        }

        _surfaceTool.AddIndex(_vIdx);
        _surfaceTool.AddIndex(_vIdx + 2);
        _surfaceTool.AddIndex(_vIdx + 1);
        _surfaceTool.AddIndex(_vIdx + 1);
        _surfaceTool.AddIndex(_vIdx + 2);
        _surfaceTool.AddIndex(_vIdx + 3);
        _vIdx += 4;
    }
}