using System.Linq;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Enum;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Struct;

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
            Triangulate(tile);
        _surfaceTool.GenerateNormals();
        _surfaceTool.SetMaterial(DefaultMaterial);
        return _surfaceTool.Commit();
    }

    private void Triangulate(Tile tile)
    {
        var corners = tile.GetCorners(_radius + tile.Height, 1f).ToList();
        for (var i = 0; i < corners.Count; i++)
            Triangulate(tile, corners[i], corners[(i + 1) % corners.Count]);
    }

    private void Triangulate(Tile tile, Vector3 c1, Vector3 c2)
    {
        var centroid = tile.GetCentroid(_radius + tile.Height);
        var v1 = centroid.Lerp(c1, HexMetrics.SolidFactor);
        var v2 = centroid.Lerp(c2, HexMetrics.SolidFactor);
        var e = new EdgeVertices(v1, v2);
        TriangulateEdgeFan(centroid, e, tile.Color);
        TriangulateConnection(tile, c1, c2, e);
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

    private void TriangulateConnection(Tile tile, Vector3 c1, Vector3 c2, EdgeVertices e)
    {
        var centroid = tile.GetCentroid(_radius + tile.Height);
        var neighbor = tile.GetNeighborByDirection(centroid.DirectionTo(c1), centroid.DirectionTo(c2));
        // 连接将由更高的地块或相同高度时 Id 更大的地块生成
        if (tile.Height < neighbor.Height ||
            (Mathf.Abs(tile.Height - neighbor.Height) < 0.00001f && tile.Id < neighbor.Id)) return;
        var neighborCenter = neighbor.GetCenter(_radius + neighbor.Height);
        var cn1 = Math3dUtil.ProjectToSphere(c1, _radius + neighbor.Height);
        var cn2 = Math3dUtil.ProjectToSphere(c2, _radius + neighbor.Height);
        var vn1 = neighborCenter.Lerp(cn1, HexMetrics.SolidFactor);
        var vn2 = neighborCenter.Lerp(cn2, HexMetrics.SolidFactor);
        var en = new EdgeVertices(vn1, vn2);
        if (HexMetrics.GetEdgeType(tile.Elevation, neighbor.Elevation) == HexEdgeType.Slope)
            TriangulateEdgeTerraces(e, tile, en, neighbor);
        else
            TriangulateEdgeStrip(e, tile.Color, en, neighbor.Color);

        var otherNeighbor1 = tile.GetNeighborsByDirection(c1 - centroid, neighbor.Id)[0];
        if (tile.Height > otherNeighbor1.Height
            || (Mathf.Abs(tile.Height - otherNeighbor1.Height) < 0.00001f && tile.Id > otherNeighbor1.Id))
        {
            // 连接角落的三角形由周围 3 个地块中最高或者一样高时 Id 最大的生成
            var onCentroid = otherNeighbor1.GetCentroid(_radius + otherNeighbor1.Height);
            var con1 = Math3dUtil.ProjectToSphere(c1, _radius + otherNeighbor1.Height);
            var von1 = onCentroid.Lerp(con1, HexMetrics.SolidFactor);
            TriangulateCorner(e.V1, tile, vn1, neighbor, von1, otherNeighbor1);
        }

        var otherNeighbor2 = tile.GetNeighborsByDirection(c2 - centroid, neighbor.Id)[0];
        if (tile.Height > otherNeighbor2.Height
            || (Mathf.Abs(tile.Height - otherNeighbor2.Height) < 0.00001f && tile.Id > otherNeighbor2.Id))
        {
            // 连接角落的三角形由周围 3 个地块中最高或者一样高时 Id 最大的生成
            var onCentroid = otherNeighbor2.GetCentroid(_radius + otherNeighbor2.Height);
            var con2 = Math3dUtil.ProjectToSphere(c2, _radius + otherNeighbor2.Height);
            var von2 = onCentroid.Lerp(con2, HexMetrics.SolidFactor);
            TriangulateCorner(e.V4, tile, vn2, neighbor, von2, otherNeighbor2);
        }
    }

    private void TriangulateCorner(Vector3 top, Tile topTile, Vector3 low1, Tile lowTile1, Vector3 low2, Tile lowTile2)
    {
        var edgeType1 = HexMetrics.GetEdgeType(topTile.Elevation, lowTile1.Elevation);
        var edgeType2 = HexMetrics.GetEdgeType(topTile.Elevation, lowTile2.Elevation);
        if (edgeType1 == HexEdgeType.Slope)
        {
            if (edgeType2 == HexEdgeType.Slope)
                TriangulateCornerTerraces(top, topTile, low1, lowTile1, low2, lowTile2);
            else if (edgeType2 == HexEdgeType.Flat)
                TriangulateCornerTerraces(low1, lowTile1, top, topTile, low2, lowTile2);
            else
                TriangulateCornerTerracesCliff(top, topTile, low1, lowTile1, low2, lowTile2);
        }
        else if (edgeType2 == HexEdgeType.Slope)
        {
            if (edgeType1 == HexEdgeType.Flat)
                TriangulateCornerTerraces(low2, lowTile2, top, topTile, low1, lowTile1);
            else
                TriangulateCornerTerracesCliff(top, topTile, low2, lowTile2, low1, lowTile1);
        }
        else if (HexMetrics.GetEdgeType(lowTile1.Elevation, lowTile2.Elevation) == HexEdgeType.Slope)
        {
            if (lowTile1.Elevation < lowTile2.Elevation)
                TriangulateCornerTerracesCliff(low1, lowTile1, low2, lowTile2, top, topTile);
            else
                TriangulateCornerTerracesCliff(low2, lowTile2, low1, lowTile1, top, topTile);
        }
        else
            AddTriangle([top, low1, low2], [topTile.Color, lowTile1.Color, lowTile2.Color]);
    }

    // 处理阶地高的 topTile 和相差 1 的阶地低的 lowTile、高度和前两者均不同的 tile（即三角形一边是阶地，一边是悬崖，一边任意）的情况
    private void TriangulateCornerTerracesCliff(Vector3 top, Tile topTile, Vector3 low, Tile lowTile, Vector3 vec,
        Tile tile)
    {
        var b = 1f / Mathf.Abs(topTile.Elevation - tile.Elevation);
        var boundary = HexMetrics.Perturb(top).Lerp(HexMetrics.Perturb(vec), b);
        var boundaryColor = topTile.Color.Lerp(tile.Color, b);
        TriangulateBoundaryTriangle(top, topTile, low, lowTile, boundary, boundaryColor);
        if (HexMetrics.GetEdgeType(lowTile.Elevation, tile.Elevation) == HexEdgeType.Slope)
            TriangulateBoundaryTriangle(low, lowTile, vec, tile, boundary, boundaryColor);
        else
            AddTriangleUnperturbed([HexMetrics.Perturb(low), HexMetrics.Perturb(vec), boundary], [lowTile.Color, tile.Color, boundaryColor]);
    }

    // 阶地和悬崖中间的半三角形
    private void TriangulateBoundaryTriangle(Vector3 top, Tile topTile, Vector3 low, Tile lowTile, Vector3 boundary,
        Color boundaryColor)
    {
        var v2 = HexMetrics.Perturb(HexMetrics.TerraceLerp(top, low, 1));
        var c2 = HexMetrics.TerraceLerp(topTile.Color, lowTile.Color, 1);
        AddTriangleUnperturbed([HexMetrics.Perturb(top), v2, boundary], [topTile.Color, c2, boundaryColor]);
        for (var i = 2; i < HexMetrics.TerraceSteps; i++)
        {
            var v1 = v2;
            var c1 = c2;
            v2 = HexMetrics.Perturb(HexMetrics.TerraceLerp(top, low, i));
            c2 = HexMetrics.TerraceLerp(topTile.Color, lowTile.Color, i);
            AddTriangleUnperturbed([v1, v2, boundary], [c1, c2, boundaryColor]);
        }

        AddTriangleUnperturbed([v2, HexMetrics.Perturb(low), boundary], [c2, lowTile.Color, boundaryColor]);
    }

    // 处理高度不同的 beginTile 和两个高度相同的 endTile（即三角形两边是等高阶地，一边是平地）的情况
    private void TriangulateCornerTerraces(Vector3 begin, Tile beginTile, Vector3 end1, Tile endTile1, Vector3 end2,
        Tile endTile2)
    {
        var v3 = HexMetrics.TerraceLerp(begin, end1, 1);
        var v4 = HexMetrics.TerraceLerp(begin, end2, 1);
        var c3 = HexMetrics.TerraceLerp(beginTile.Color, endTile1.Color, 1);
        var c4 = HexMetrics.TerraceLerp(beginTile.Color, endTile2.Color, 1);
        AddTriangle([begin, v3, v4], [beginTile.Color, c3, c4]);
        for (var i = 0; i < HexMetrics.TerraceSteps; i++)
        {
            var v1 = v3;
            var v2 = v4;
            var c1 = c3;
            var c2 = c4;
            v3 = HexMetrics.TerraceLerp(begin, end1, i);
            v4 = HexMetrics.TerraceLerp(begin, end2, i);
            c3 = HexMetrics.TerraceLerp(beginTile.Color, endTile1.Color, i);
            c4 = HexMetrics.TerraceLerp(beginTile.Color, endTile2.Color, i);
            AddQuad([v1, v2, v3, v4], [c1, c2, c3, c4]);
        }

        AddQuad([v3, v4, end1, end2], [c3, c4, endTile1.Color, endTile2.Color]);
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
        var idx = Math3dUtil.SortVertices(Vector3.Zero, vs);
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

    private void AddQuad(Vector3[] vs, Color[] cs = null) =>
        AddQuadUnperturbed(vs.Select(HexMetrics.Perturb).ToArray(), cs);

    private void AddQuadUnperturbed(Vector3[] vs, Color[] cs = null)
    {
        var idx = Math3dUtil.SortVertices(Vector3.Zero, vs);
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