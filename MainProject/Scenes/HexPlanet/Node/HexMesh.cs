using System.Collections.Generic;
using System.Linq;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Enum;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Struct;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Node;

[Tool]
public partial class HexMesh: MeshInstance3D
{
    private readonly SurfaceTool _surfaceTool = new();
    private int _vIdx;
    private static readonly Material DefaultMaterial = new StandardMaterial3D { VertexColorUseAsAlbedo = true };

    private float _radius;

    public void BuildMesh(float radius, IEnumerable<Tile> tiles)
    {
        _radius = radius;
        _vIdx = 0;
        // 清理之前的碰撞体
        foreach (var child in GetChildren())
            child.QueueFree();
        _surfaceTool.Clear();
        _surfaceTool.Begin(Mesh.PrimitiveType.Triangles);
        _surfaceTool.SetSmoothGroup(uint.MaxValue);
        foreach (var tile in tiles)
            Triangulate(tile);
        _surfaceTool.GenerateNormals();
        _surfaceTool.SetMaterial(DefaultMaterial);
        Mesh = _surfaceTool.Commit();
        CreateTrimeshCollision();
    }

    private void Triangulate(Tile tile)
    {
        // var corners = tile.GetCorners(_radius + tile.Height, 1f).ToList();
        for (var i = 0; i < tile.HexFaceIds.Count; i++)
            Triangulate(tile, i, (i + 1) % tile.HexFaceIds.Count);
    }

    // Godot 缠绕顺序是正面顺时针，所以从 i1 对应角落到 i2 对应角落相对于 tile 重心需要是顺时针
    private void Triangulate(Tile tile, int i1, int i2)
    {
        var v1 = tile.GetCorner(i1, _radius + tile.Height, HexMetrics.SolidFactor);
        var v2 = tile.GetCorner(i2, _radius + tile.Height, HexMetrics.SolidFactor);
        var e = new EdgeVertices(v1, v2);
        var centroid = tile.GetCentroid(_radius + tile.Height);
        TriangulateEdgeFan(centroid, e, tile.Color);
        TriangulateConnection(tile, i1, i2, e);
    }

    private void TriangulateEdgeFan(Vector3 center, EdgeVertices edge, Color color)
    {
        AddTriangle([center, edge.V1, edge.V2], TriColor(color));
        AddTriangle([center, edge.V2, edge.V3], TriColor(color));
        AddTriangle([center, edge.V3, edge.V4], TriColor(color));
    }

    private void TriangulateEdgeStrip(EdgeVertices e1, Color c1, EdgeVertices e2, Color c2)
    {
        AddQuad([e1.V1, e1.V2, e2.V1, e2.V2], QuadColor(c1, c2));
        AddQuad([e1.V2, e1.V3, e2.V2, e2.V3], QuadColor(c1, c2));
        AddQuad([e1.V3, e1.V4, e2.V3, e2.V4], QuadColor(c1, c2));
    }

    private void TriangulateConnection(Tile tile, int i1, int i2, EdgeVertices e)
    {
        var neighbor = tile.GetNeighborByDirection(i1, i2);
        // 连接将由更低的地块或相同高度时 Id 更大的地块生成
        if (tile.Height > neighbor.Height ||
            (Mathf.Abs(tile.Height - neighbor.Height) < 0.00001f && tile.Id < neighbor.Id)) return;
        var vn1 = neighbor.GetCornerByFaceId(tile.HexFaceIds[i1],
            _radius + neighbor.Height, HexMetrics.SolidFactor);
        var vn2 = neighbor.GetCornerByFaceId(tile.HexFaceIds[i2],
            _radius + neighbor.Height, HexMetrics.SolidFactor);
        var en = new EdgeVertices(vn1, vn2);
        if (HexMetrics.GetEdgeType(tile.Elevation, neighbor.Elevation) == HexEdgeType.Slope)
            TriangulateEdgeTerraces(e, tile, en, neighbor);
        else
            TriangulateEdgeStrip(e, tile.Color, en, neighbor.Color);

        var otherNeighbor1 = tile.GetNeighborsByDirection(i1, neighbor.Id)[0];
        if (tile.Height < otherNeighbor1.Height
            || (Mathf.Abs(tile.Height - otherNeighbor1.Height) < 0.00001f && tile.Id > otherNeighbor1.Id))
        {
            // 连接角落的三角形由周围 3 个地块中最低或者一样高时 Id 最大的生成
            var von1 = otherNeighbor1.GetCornerByFaceId(tile.HexFaceIds[i1],
                _radius + otherNeighbor1.Height, HexMetrics.SolidFactor);
            TriangulateCorner(e.V1, tile, von1, otherNeighbor1, vn1, neighbor);
        }

        var otherNeighbor2 = tile.GetNeighborsByDirection(i2, neighbor.Id)[0];
        if (tile.Height < otherNeighbor2.Height
            || (Mathf.Abs(tile.Height - otherNeighbor2.Height) < 0.00001f && tile.Id > otherNeighbor2.Id))
        {
            // 连接角落的三角形由周围 3 个地块中最低或者一样高时 Id 最大的生成
            var von2 = otherNeighbor2.GetCornerByFaceId(tile.HexFaceIds[i2],
                _radius + otherNeighbor2.Height, HexMetrics.SolidFactor);
            TriangulateCorner(e.V4, tile, vn2, neighbor, von2, otherNeighbor2);
        }
    }

    // 需要保证入参 bottom -> left -> right 是顺时针
    private void TriangulateCorner(Vector3 bottom, Tile bottomTile,
        Vector3 left, Tile leftTile, Vector3 right, Tile rightTile)
    {
        var edgeType1 = HexMetrics.GetEdgeType(bottomTile.Elevation, leftTile.Elevation);
        var edgeType2 = HexMetrics.GetEdgeType(bottomTile.Elevation, rightTile.Elevation);
        if (edgeType1 == HexEdgeType.Slope)
        {
            if (edgeType2 == HexEdgeType.Slope)
                TriangulateCornerTerraces(bottom, bottomTile, left, leftTile, right, rightTile);
            else if (edgeType2 == HexEdgeType.Flat)
                TriangulateCornerTerraces(left, leftTile, right, rightTile, bottom, bottomTile);
            else
                TriangulateCornerTerracesCliff(bottom, bottomTile, left, leftTile, right, rightTile);
        }
        else if (edgeType2 == HexEdgeType.Slope)
        {
            if (edgeType1 == HexEdgeType.Flat)
                TriangulateCornerTerraces(right, rightTile, bottom, bottomTile, left, leftTile);
            else
                TriangulateCornerCliffTerraces(bottom, bottomTile, left, leftTile, right, rightTile);
        }
        else if (HexMetrics.GetEdgeType(leftTile.Elevation, rightTile.Elevation) == HexEdgeType.Slope)
        {
            if (leftTile.Elevation < rightTile.Elevation)
                TriangulateCornerCliffTerraces(right, rightTile, bottom, bottomTile, left, leftTile);
            else
                TriangulateCornerTerracesCliff(left, leftTile, right, rightTile, bottom, bottomTile);
        }
        else
            AddTriangle([bottom, left, right], [bottomTile.Color, leftTile.Color, rightTile.Color]);
    }

    // 三角形靠近 tile 的左边是阶地，右边是悬崖，另一边任意的情况
    private void TriangulateCornerTerracesCliff(Vector3 begin, Tile beginTile,
        Vector3 left, Tile leftTile, Vector3 right, Tile rightTile)
    {
        var b = 1f / Mathf.Abs(rightTile.Elevation - beginTile.Elevation);
        var boundary = HexMetrics.Perturb(begin).Lerp(HexMetrics.Perturb(right), b);
        var boundaryColor = beginTile.Color.Lerp(rightTile.Color, b);
        TriangulateBoundaryTriangle(begin, beginTile, left, leftTile, boundary, boundaryColor);
        if (HexMetrics.GetEdgeType(leftTile.Elevation, rightTile.Elevation) == HexEdgeType.Slope)
            TriangulateBoundaryTriangle(left, leftTile, right, rightTile, boundary, boundaryColor);
        else
            AddTriangleUnperturbed([HexMetrics.Perturb(left), HexMetrics.Perturb(right), boundary],
                [leftTile.Color, rightTile.Color, boundaryColor]);
    }

    // 三角形靠近 tile 的左边是悬崖，右边是阶地，另一边任意的情况
    private void TriangulateCornerCliffTerraces(Vector3 begin, Tile beginTile,
        Vector3 left, Tile leftTile, Vector3 right, Tile rightTile)
    {
        var b = 1f / Mathf.Abs(leftTile.Elevation - beginTile.Elevation);
        var boundary = HexMetrics.Perturb(begin).Lerp(HexMetrics.Perturb(left), b);
        var boundaryColor = beginTile.Color.Lerp(leftTile.Color, b);
        TriangulateBoundaryTriangle(right, rightTile, begin, beginTile, boundary, boundaryColor);
        if (HexMetrics.GetEdgeType(leftTile.Elevation, rightTile.Elevation) == HexEdgeType.Slope)
            TriangulateBoundaryTriangle(left, leftTile, right, rightTile, boundary, boundaryColor);
        else
            AddTriangleUnperturbed([HexMetrics.Perturb(left), HexMetrics.Perturb(right), boundary],
                [leftTile.Color, rightTile.Color, boundaryColor]);
    }

    // 阶地和悬崖中间的半三角形
    private void TriangulateBoundaryTriangle(Vector3 begin, Tile beginTile,
        Vector3 left, Tile leftTile, Vector3 boundary, Color boundaryColor)
    {
        var v2 = HexMetrics.Perturb(HexMetrics.TerraceLerp(begin, left, 1));
        var c2 = HexMetrics.TerraceLerp(beginTile.Color, leftTile.Color, 1);
        AddTriangleUnperturbed([HexMetrics.Perturb(begin), v2, boundary], [beginTile.Color, c2, boundaryColor]);
        for (var i = 2; i < HexMetrics.TerraceSteps; i++)
        {
            var v1 = v2;
            var c1 = c2;
            v2 = HexMetrics.Perturb(HexMetrics.TerraceLerp(begin, left, i));
            c2 = HexMetrics.TerraceLerp(beginTile.Color, leftTile.Color, i);
            AddTriangleUnperturbed([v1, v2, boundary], [c1, c2, boundaryColor]);
        }

        AddTriangleUnperturbed([v2, HexMetrics.Perturb(left), boundary], [c2, leftTile.Color, boundaryColor]);
    }

    // 处理高度不同的 beginTile 和两个高度相同的 endTile（即三角形两边是等高阶地，一边是平地）的情况
    private void TriangulateCornerTerraces(Vector3 begin, Tile beginTile,
        Vector3 left, Tile leftTile, Vector3 right, Tile rightTile)
    {
        var v3 = HexMetrics.TerraceLerp(begin, left, 1);
        var v4 = HexMetrics.TerraceLerp(begin, right, 1);
        var c3 = HexMetrics.TerraceLerp(beginTile.Color, leftTile.Color, 1);
        var c4 = HexMetrics.TerraceLerp(beginTile.Color, rightTile.Color, 1);
        AddTriangle([begin, v3, v4], [beginTile.Color, c3, c4]);
        for (var i = 0; i < HexMetrics.TerraceSteps; i++)
        {
            var v1 = v3;
            var v2 = v4;
            var c1 = c3;
            var c2 = c4;
            v3 = HexMetrics.TerraceLerp(begin, left, i);
            v4 = HexMetrics.TerraceLerp(begin, right, i);
            c3 = HexMetrics.TerraceLerp(beginTile.Color, leftTile.Color, i);
            c4 = HexMetrics.TerraceLerp(beginTile.Color, rightTile.Color, i);
            AddQuad([v1, v2, v3, v4], [c1, c2, c3, c4]);
        }

        AddQuad([v3, v4, left, right], [c3, c4, leftTile.Color, rightTile.Color]);
    }

    private void TriangulateEdgeTerraces(EdgeVertices begin, Tile beginTile, EdgeVertices end, Tile endTile)
    {
        var e2 = EdgeVertices.TerraceLerp(begin, end, 1);
        var c2 = HexMetrics.TerraceLerp(beginTile.Color, endTile.Color, 1);
        TriangulateEdgeStrip(begin, beginTile.Color, e2, c2);
        for (var i = 2; i < HexMetrics.TerraceSteps; i++)
        {
            var e1 = e2;
            var c1 = c2;
            e2 = EdgeVertices.TerraceLerp(begin, end, i);
            c2 = HexMetrics.TerraceLerp(beginTile.Color, endTile.Color, i);
            TriangulateEdgeStrip(e1, c1, e2, c2);
        }

        TriangulateEdgeStrip(e2, c2, end, endTile.Color);
    }

    private static Color[] TriColor(Color c) => [c, c, c];
    private static Color[] QuadColor(Color c) => [c, c, c, c];
    private static Color[] QuadColor(Color c1, Color c2) => [c1, c1, c2, c2];

    private void AddTriangle(Vector3[] vs, Color[] cs = null) =>
        AddTriangleUnperturbed(vs.Select(HexMetrics.Perturb).ToArray(), cs);

    private void AddTriangleUnperturbed(Vector3[] vs, Color[] cs = null)
    {
        for (var i = 0; i < 3; i++)
        {
            if (cs != null)
                _surfaceTool.SetColor(cs[i]);
            _surfaceTool.AddVertex(vs[i]);
        }

        _surfaceTool.AddIndex(_vIdx);
        _surfaceTool.AddIndex(_vIdx + 1);
        _surfaceTool.AddIndex(_vIdx + 2);
        _vIdx += 3;
    }

    private void AddQuad(Vector3[] vs, Color[] cs = null) =>
        AddQuadUnperturbed(vs.Select(HexMetrics.Perturb).ToArray(), cs);

    private void AddQuadUnperturbed(Vector3[] vs, Color[] cs = null)
    {
        for (var i = 0; i < 4; i++)
        {
            if (cs != null)
                _surfaceTool.SetColor(cs[i]);
            _surfaceTool.AddVertex(vs[i]);
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