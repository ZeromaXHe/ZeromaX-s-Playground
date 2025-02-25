using System.Linq;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Enum;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Node.Interface;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Struct;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service.Impl;

public class HexMeshService(IChunkService chunkService, ITileService tileService) : IHexMeshService
{
    private float _radius;
    private IHexMesh _terrain;
    private IHexMesh _rivers;
    private IHexMesh _roads;

    public void Triangulate(float radius, int chunkId, IHexMesh terrain, IHexMesh rivers, IHexMesh roads)
    {
        _radius = radius;
        _terrain = terrain;
        _rivers = rivers;
        _roads = roads;
        var tileIds = chunkService.GetById(chunkId).TileIds;
        var tiles = tileIds.Select(tileService.GetById);
        foreach (var tile in tiles)
            Triangulate(tile);
    }

    private void Triangulate(Tile tile)
    {
        // var corners = tile.GetCorners(_radius + tile.Height, 1f).ToList();
        for (var i = 0; i < tile.HexFaceIds.Count; i++)
            Triangulate(tile, i);
    }

    // Godot 缠绕顺序是正面顺时针，所以从 i1 对应角落到 i2 对应角落相对于 tile 重心需要是顺时针
    private void Triangulate(Tile tile, int idx)
    {
        var height = tileService.GetHeight(tile);
        var v1 = tileService.GetFirstSolidCorner(tile, idx, _radius);
        var v2 = tileService.GetSecondSolidCorner(tile, idx, _radius);
        var e = new EdgeVertices(v1, v2);
        var centroid = tile.GetCentroid(_radius + height);
        if (tile.HasRiver)
        {
            if (tileService.HasRiverThroughEdge(tile, idx))
            {
                e.V3 = Math3dUtil.ProjectToSphere(e.V3, _radius + tileService.GetStreamBedHeight(tile));
                if (tile.HasRiverBeginOrEnd)
                    TriangulateWithRiverBeginOrEnd(tile, idx, centroid, e);
                else TriangulateWithRiver(tile, idx, centroid, e);
            }
            else TriangulateAdjacentToRiver(tile, idx, centroid, e);
        }
        else TriangulateWithoutRiver(tile, idx, centroid, e);

        TriangulateConnection(tile, idx, e);
    }

    private void TriangulateAdjacentToRiver(Tile tile, int idx, Vector3 centroid, EdgeVertices e)
    {
        if (tile.HasRoads)
            TriangulateRoadAdjacentToRiver(tile, idx, centroid, e);
        if (tileService.HasRiverThroughEdge(tile, tile.NextIdx(idx)))
        {
            if (tileService.HasRiverThroughEdge(tile, tile.PreviousIdx(idx)))
                centroid = tileService.GetSolidEdgeMiddle(tile, idx, _radius, 0.5f * HexMetrics.InnerToOuter);
            else if (tileService.HasRiverThroughEdge(tile, tile.Previous2Idx(idx)))
                centroid = tileService.GetFirstSolidCorner(tile, idx, _radius, 0.25f);
        }
        else if (tileService.HasRiverThroughEdge(tile, tile.PreviousIdx(idx)) &&
                 tileService.HasRiverThroughEdge(tile, tile.Next2Idx(idx)))
            centroid = tileService.GetSecondSolidCorner(tile, idx, _radius, 0.25f);

        var m = new EdgeVertices(centroid.Lerp(e.V1, 0.5f), centroid.Lerp(e.V5, 0.5f));
        TriangulateEdgeStrip(m, tile.Color, e, tile.Color);
        TriangulateEdgeFan(centroid, m, tile.Color);
    }

    private void TriangulateRoadAdjacentToRiver(Tile tile, int idx, Vector3 centroid, EdgeVertices e)
    {
        var hasRoadThroughEdge = tile.HasRoadThroughEdge(idx);
        var previousHasRiver = tileService.HasRiverThroughEdge(tile, tile.PreviousIdx(idx));
        var nextHasRiver = tileService.HasRiverThroughEdge(tile, tile.NextIdx(idx));
        var interpolators = GetRoadInterpolators(tile, idx);
        var roadCenter = centroid;
        if (tile.HasRiverBeginOrEnd)
        {
            var riverBeginOrEndIdx = tileService.GetRiverBeginOrEndIdx(tile);
            roadCenter += tileService.GetSolidEdgeMiddle(tile, tile.OppositeIdx(riverBeginOrEndIdx),
                _radius, 1f / 3f) - centroid;
        }
        else
        {
            var incomingRiverIdx = tileService.GetNeighborIdIdx(tile, tile.IncomingRiverNId);
            var outgoingRiverIdx = tileService.GetNeighborIdIdx(tile, tile.OutgoingRiverNId);
            if (incomingRiverIdx == tile.OppositeIdx(outgoingRiverIdx))
            {
                Vector3 corner;
                if (previousHasRiver)
                {
                    if (!hasRoadThroughEdge && !tile.HasRoadThroughEdge(tile.NextIdx(idx))) return;
                    corner = tileService.GetSecondSolidCorner(tile, idx, _radius);
                }
                else
                {
                    if (!hasRoadThroughEdge && !tile.HasRoadThroughEdge(tile.PreviousIdx(idx))) return;
                    corner = tileService.GetFirstSolidCorner(tile, idx, _radius);
                }

                roadCenter += (corner - centroid) * 0.5f;
                centroid += (corner - centroid) * 0.25f;
            }
            else if (incomingRiverIdx == tile.PreviousIdx(outgoingRiverIdx))
            {
                roadCenter -= tileService.GetSecondCorner(tile, incomingRiverIdx,
                    _radius + tileService.GetHeight(tile), 0.2f) - centroid;
            }
            else if (incomingRiverIdx == tile.NextIdx(outgoingRiverIdx))
            {
                roadCenter -= tileService.GetFirstCorner(tile, incomingRiverIdx,
                    _radius + tileService.GetHeight(tile), 0.2f) - centroid;
            }
            else if (previousHasRiver && nextHasRiver)
            {
                if (!hasRoadThroughEdge) return;
                var offset = tileService.GetSolidEdgeMiddle(tile, idx, _radius, HexMetrics.InnerToOuter);
                roadCenter += (offset - centroid) * 0.7f;
                centroid += (offset - centroid) * 0.5f;
            }
            else
            {
                int middleIdx;
                if (previousHasRiver)
                    middleIdx = tile.NextIdx(idx);
                else if (nextHasRiver)
                    middleIdx = tile.PreviousIdx(idx);
                else
                    middleIdx = idx;
                if (!tile.HasRoadThroughEdge(middleIdx)
                    && !tile.HasRoadThroughEdge(tile.PreviousIdx(middleIdx))
                    && !tile.HasRoadThroughEdge(tile.NextIdx(middleIdx)))
                    return;
                roadCenter += tileService.GetSolidEdgeMiddle(tile, middleIdx, _radius, 0.25f) - centroid;
            }
        }

        var mL = roadCenter.Lerp(e.V1, interpolators.X);
        var mR = roadCenter.Lerp(e.V5, interpolators.Y);
        TriangulateRoad(roadCenter, mL, mR, e, hasRoadThroughEdge);
        if (previousHasRiver)
            TriangulateRoadEdge(roadCenter, centroid, mL);
        if (nextHasRiver)
            TriangulateRoadEdge(roadCenter, mR, centroid);
    }

    private void TriangulateWithRiverBeginOrEnd(Tile tile, int idx, Vector3 centroid, EdgeVertices e)
    {
        var m = new EdgeVertices(centroid.Lerp(e.V1, 0.5f), centroid.Lerp(e.V5, 0.5f));
        m.V3 = Math3dUtil.ProjectToSphere(m.V3, e.V3.Length());
        TriangulateEdgeStrip(m, tile.Color, e, tile.Color);
        TriangulateEdgeFan(centroid, m, tile.Color);

        var reversed = tile.HasIncomingRiver;
        var riverSurfaceHeight = _radius + tileService.GetRiverSurfaceHeight(tile);
        TriangulateRiverQuad(m.V2, m.V4, e.V2, e.V4, riverSurfaceHeight, 0.6f, reversed);
        centroid = Math3dUtil.ProjectToSphere(centroid, riverSurfaceHeight);
        m.V2 = Math3dUtil.ProjectToSphere(m.V2, riverSurfaceHeight);
        m.V4 = Math3dUtil.ProjectToSphere(m.V4, riverSurfaceHeight);
        _rivers.AddTriangle([centroid, m.V2, m.V4],
            uvs: reversed
                ? [new Vector2(0.5f, 0.4f), new Vector2(1f, 0.2f), new Vector2(0f, 0.2f)]
                : [new Vector2(0.5f, 0.4f), new Vector2(0f, 0.6f), new Vector2(1f, 0.6f)]);
    }

    private void TriangulateWithRiver(Tile tile, int idx, Vector3 centroid, EdgeVertices e)
    {
        Vector3 centerL;
        Vector3 centerR;
        if (tileService.HasRiverThroughEdge(tile, tile.OppositeIdx(idx)))
        {
            centerL = tileService.GetFirstSolidCorner(tile, tile.PreviousIdx(idx), _radius, 0.25f);
            centerR = tileService.GetSecondSolidCorner(tile, tile.NextIdx(idx), _radius, 0.25f);
        }
        else if (tileService.HasRiverThroughEdge(tile, tile.NextIdx(idx)))
        {
            centerL = centroid;
            centerR = centroid.Lerp(e.V5, 2f / 3f);
        }
        else if (tileService.HasRiverThroughEdge(tile, tile.PreviousIdx(idx)))
        {
            centerL = centroid.Lerp(e.V1, 2f / 3f);
            centerR = centroid;
        }
        else if (tileService.HasRiverThroughEdge(tile, tile.Next2Idx(idx)))
        {
            centerL = centroid;
            centerR = tileService.GetSolidEdgeMiddle(tile, tile.NextIdx(idx), _radius,
                0.5f * HexMetrics.InnerToOuter);
        }
        else if (tileService.HasRiverThroughEdge(tile, tile.Previous2Idx(idx)))
        {
            centerL = tileService.GetSolidEdgeMiddle(tile, tile.PreviousIdx(idx), _radius,
                0.5f * HexMetrics.InnerToOuter);
            centerR = centroid;
        }
        else
        {
            centerL = centerR = centroid;
        }

        centroid = centerL.Lerp(centerR, 0.5f);

        var m = new EdgeVertices(centerL.Lerp(e.V1, 0.5f), centerR.Lerp(e.V5, 0.5f), 1f / 6f);
        m.V3 = Math3dUtil.ProjectToSphere(m.V3, e.V3.Length());
        centroid = Math3dUtil.ProjectToSphere(centroid, e.V3.Length());
        TriangulateEdgeStrip(m, tile.Color, e, tile.Color);
        _terrain.AddTriangle([centerL, m.V1, m.V2], TriColor(tile.Color));
        _terrain.AddQuad([centerL, centroid, m.V2, m.V3], QuadColor(tile.Color));
        _terrain.AddQuad([centroid, centerR, m.V3, m.V4], QuadColor(tile.Color));
        _terrain.AddTriangle([centerR, m.V4, m.V5], TriColor(tile.Color));

        var reversed = tile.IncomingRiverNId == tileService.GetNeighborByIdx(tile, idx).Id;
        TriangulateRiverQuad(centerL, centerR, m.V2, m.V4,
            _radius + tileService.GetRiverSurfaceHeight(tile), 0.4f, reversed);
        TriangulateRiverQuad(m.V2, m.V4, e.V2, e.V4,
            _radius + tileService.GetRiverSurfaceHeight(tile), 0.6f, reversed);
    }

    private void TriangulateRiverQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4,
        float height, float v, bool reversed) =>
        TriangulateRiverQuad(v1, v2, v3, v4, height, height, v, reversed);

    private void TriangulateRiverQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4,
        float height1, float height2, float v, bool reversed)
    {
        v1 = Math3dUtil.ProjectToSphere(v1, height1);
        v2 = Math3dUtil.ProjectToSphere(v2, height1);
        v3 = Math3dUtil.ProjectToSphere(v3, height2);
        v4 = Math3dUtil.ProjectToSphere(v4, height2);
        _rivers.AddQuad([v1, v2, v3, v4],
            uvs: reversed
                ? QuadUv(1f, 0f, 0.8f - v, 0.6f - v)
                : QuadUv(0f, 1f, v, v + 0.2f));
    }

    private void TriangulateEdgeFan(Vector3 center, EdgeVertices edge, Color color)
    {
        _terrain.AddTriangle([center, edge.V1, edge.V2], TriColor(color));
        _terrain.AddTriangle([center, edge.V2, edge.V3], TriColor(color));
        _terrain.AddTriangle([center, edge.V3, edge.V4], TriColor(color));
        _terrain.AddTriangle([center, edge.V4, edge.V5], TriColor(color));
    }

    private void TriangulateEdgeStrip(EdgeVertices e1, Color c1, EdgeVertices e2, Color c2, bool hasRoad = false)
    {
        _terrain.AddQuad([e1.V1, e1.V2, e2.V1, e2.V2], QuadColor(c1, c2));
        _terrain.AddQuad([e1.V2, e1.V3, e2.V2, e2.V3], QuadColor(c1, c2));
        _terrain.AddQuad([e1.V3, e1.V4, e2.V3, e2.V4], QuadColor(c1, c2));
        _terrain.AddQuad([e1.V4, e1.V5, e2.V4, e2.V5], QuadColor(c1, c2));
        if (hasRoad)
            TriangulateRoadSegment(e1.V2, e1.V3, e1.V4, e2.V2, e2.V3, e2.V4);
    }

    private Vector2 GetRoadInterpolators(Tile tile, int idx)
    {
        Vector2 interpolators;
        if (tile.HasRoadThroughEdge(idx))
            interpolators.X = interpolators.Y = 0.5f;
        else
        {
            interpolators.X = tile.HasRoadThroughEdge(tile.PreviousIdx(idx)) ? 0.5f : 0.25f;
            interpolators.Y = tile.HasRoadThroughEdge(tile.NextIdx(idx)) ? 0.5f : 0.25f;
        }

        return interpolators;
    }

    private void TriangulateWithoutRiver(Tile tile, int idx, Vector3 centroid, EdgeVertices e)
    {
        TriangulateEdgeFan(centroid, e, tile.Color);
        if (tile.HasRoads)
        {
            var interpolators = GetRoadInterpolators(tile, idx);
            TriangulateRoad(centroid, centroid.Lerp(e.V1, interpolators.X),
                centroid.Lerp(e.V5, interpolators.Y), e, tile.HasRoadThroughEdge(idx));
        }
    }

    private void TriangulateRoad(Vector3 centroid, Vector3 mL, Vector3 mR, EdgeVertices e, bool hasRoadThroughCellEdge)
    {
        if (hasRoadThroughCellEdge)
        {
            var mC = mL.Lerp(mR, 0.5f);
            TriangulateRoadSegment(mL, mC, mR, e.V2, e.V3, e.V4);
            _roads.AddTriangle([centroid, mL, mC],
                uvs: [new Vector2(1f, 0f), new Vector2(0f, 0f), new Vector2(1f, 0f)]);
            _roads.AddTriangle([centroid, mC, mR],
                uvs: [new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(0f, 0f)]);
        }
        else TriangulateRoadEdge(centroid, mL, mR);
    }

    private void TriangulateRoadEdge(Vector3 centroid, Vector3 mL, Vector3 mR)
    {
        _roads.AddTriangle([centroid, mL, mR], uvs: [new Vector2(1f, 0f), new Vector2(0f, 0f), new Vector2(0f, 0f)]);
    }

    // 顶点的排序：
    // v4  v5  v6
    // v1  v2  v3
    // 0.0 1.0 0.0
    private void TriangulateRoadSegment(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, Vector3 v5, Vector3 v6)
    {
        _roads.AddQuad([v1, v2, v4, v5], uvs: QuadUv(0f, 1f, 0f, 0f));
        _roads.AddQuad([v2, v3, v5, v6], uvs: QuadUv(1f, 0f, 0f, 0f));
    }

    private void TriangulateConnection(Tile tile, int idx, EdgeVertices e)
    {
        var tileHeight = tileService.GetHeight(tile);
        var neighbor = tileService.GetNeighborByIdx(tile, idx);
        var neighborHeight = tileService.GetHeight(neighbor);
        // 连接将由更低的地块或相同高度时 Id 更大的地块生成
        if (tileHeight > neighborHeight ||
            (Mathf.Abs(tileHeight - neighborHeight) < 0.00001f && tile.Id < neighbor.Id)) return;
        var vn1 = tileService.GetCornerByFaceId(neighbor, tile.HexFaceIds[idx],
            _radius + neighborHeight, HexMetrics.SolidFactor);
        var vn2 = tileService.GetCornerByFaceId(neighbor, tile.HexFaceIds[tile.NextIdx(idx)],
            _radius + neighborHeight, HexMetrics.SolidFactor);
        var en = new EdgeVertices(vn1, vn2);
        if (neighbor.HasRiverToNeighbor(tile.Id))
        {
            en.V3 = Math3dUtil.ProjectToSphere(en.V3, _radius + tileService.GetStreamBedHeight(neighbor));
            TriangulateRiverQuad(e.V2, e.V4, en.V2, en.V4,
                _radius + tileService.GetRiverSurfaceHeight(tile),
                _radius + tileService.GetRiverSurfaceHeight(neighbor), 0.8f,
                tile.HasIncomingRiver && tile.IncomingRiverNId == neighbor.Id);
        }

        if (HexMetrics.GetEdgeType(tile.Elevation, neighbor.Elevation) == HexEdgeType.Slope)
            TriangulateEdgeTerraces(e, tile, en, neighbor, tile.HasRoadThroughEdge(idx));
        else
            TriangulateEdgeStrip(e, tile.Color, en, neighbor.Color, tile.HasRoadThroughEdge(idx));

        var otherNeighbor1 = tileService.GetCornerNeighborsByIdx(tile, idx, neighbor.Id)[0];
        var otherNeighbor1Height = tileService.GetHeight(otherNeighbor1);
        if (tileHeight < otherNeighbor1Height
            || (Mathf.Abs(tileHeight - otherNeighbor1Height) < 0.00001f && tile.Id > otherNeighbor1.Id))
        {
            // 连接角落的三角形由周围 3 个地块中最低或者一样高时 Id 最大的生成
            var von1 = tileService.GetCornerByFaceId(otherNeighbor1, tile.HexFaceIds[idx],
                _radius + otherNeighbor1Height, HexMetrics.SolidFactor);
            TriangulateCorner(e.V1, tile, von1, otherNeighbor1, vn1, neighbor);
        }

        var otherNeighbor2 = tileService.GetCornerNeighborsByIdx(tile, tile.NextIdx(idx), neighbor.Id)[0];
        var otherNeighbor2Height = tileService.GetHeight(otherNeighbor2);
        if (tileHeight < otherNeighbor2Height
            || (Mathf.Abs(tileHeight - otherNeighbor2Height) < 0.00001f && tile.Id > otherNeighbor2.Id))
        {
            // 连接角落的三角形由周围 3 个地块中最低或者一样高时 Id 最大的生成
            var von2 = tileService.GetCornerByFaceId(otherNeighbor2, tile.HexFaceIds[tile.NextIdx(idx)],
                _radius + otherNeighbor2Height, HexMetrics.SolidFactor);
            TriangulateCorner(e.V5, tile, vn2, neighbor, von2, otherNeighbor2);
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
            _terrain.AddTriangle([bottom, left, right], [bottomTile.Color, leftTile.Color, rightTile.Color]);
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
            _terrain.AddTriangleUnperturbed([HexMetrics.Perturb(left), HexMetrics.Perturb(right), boundary],
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
            _terrain.AddTriangleUnperturbed([HexMetrics.Perturb(left), HexMetrics.Perturb(right), boundary],
                [leftTile.Color, rightTile.Color, boundaryColor]);
    }

    // 阶地和悬崖中间的半三角形
    private void TriangulateBoundaryTriangle(Vector3 begin, Tile beginTile,
        Vector3 left, Tile leftTile, Vector3 boundary, Color boundaryColor)
    {
        var v2 = HexMetrics.Perturb(HexMetrics.TerraceLerp(begin, left, 1));
        var c2 = HexMetrics.TerraceLerp(beginTile.Color, leftTile.Color, 1);
        _terrain.AddTriangleUnperturbed([HexMetrics.Perturb(begin), v2, boundary],
            [beginTile.Color, c2, boundaryColor]);
        for (var i = 2; i < HexMetrics.TerraceSteps; i++)
        {
            var v1 = v2;
            var c1 = c2;
            v2 = HexMetrics.Perturb(HexMetrics.TerraceLerp(begin, left, i));
            c2 = HexMetrics.TerraceLerp(beginTile.Color, leftTile.Color, i);
            _terrain.AddTriangleUnperturbed([v1, v2, boundary], [c1, c2, boundaryColor]);
        }

        _terrain.AddTriangleUnperturbed([v2, HexMetrics.Perturb(left), boundary], [c2, leftTile.Color, boundaryColor]);
    }

    // 处理高度不同的 beginTile 和两个高度相同的 endTile（即三角形两边是等高阶地，一边是平地）的情况
    private void TriangulateCornerTerraces(Vector3 begin, Tile beginTile,
        Vector3 left, Tile leftTile, Vector3 right, Tile rightTile)
    {
        var v3 = HexMetrics.TerraceLerp(begin, left, 1);
        var v4 = HexMetrics.TerraceLerp(begin, right, 1);
        var c3 = HexMetrics.TerraceLerp(beginTile.Color, leftTile.Color, 1);
        var c4 = HexMetrics.TerraceLerp(beginTile.Color, rightTile.Color, 1);
        _terrain.AddTriangle([begin, v3, v4], [beginTile.Color, c3, c4]);
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
            _terrain.AddQuad([v1, v2, v3, v4], [c1, c2, c3, c4]);
        }

        _terrain.AddQuad([v3, v4, left, right], [c3, c4, leftTile.Color, rightTile.Color]);
    }

    private void TriangulateEdgeTerraces(EdgeVertices begin, Tile beginTile, EdgeVertices end, Tile endTile,
        bool hasRoad)
    {
        var e2 = EdgeVertices.TerraceLerp(begin, end, 1);
        var c2 = HexMetrics.TerraceLerp(beginTile.Color, endTile.Color, 1);
        TriangulateEdgeStrip(begin, beginTile.Color, e2, c2, hasRoad);
        for (var i = 2; i < HexMetrics.TerraceSteps; i++)
        {
            var e1 = e2;
            var c1 = c2;
            e2 = EdgeVertices.TerraceLerp(begin, end, i);
            c2 = HexMetrics.TerraceLerp(beginTile.Color, endTile.Color, i);
            TriangulateEdgeStrip(e1, c1, e2, c2, hasRoad);
        }

        TriangulateEdgeStrip(e2, c2, end, endTile.Color, hasRoad);
    }

    private static Color[] TriColor(Color c) => [c, c, c];
    private static Color[] QuadColor(Color c) => [c, c, c, c];
    private static Color[] QuadColor(Color c1, Color c2) => [c1, c1, c2, c2];

    private static Vector2[] QuadUv(float uMin, float uMax, float vMin, float vMax) =>
    [
        new Vector2(uMin, vMin), new Vector2(uMax, vMin),
        new Vector2(uMin, vMax), new Vector2(uMax, vMax)
    ];
}