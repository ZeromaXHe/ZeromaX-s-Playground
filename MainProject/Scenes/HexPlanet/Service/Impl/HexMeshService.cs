using System.Linq;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Enum;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Node.Interface;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Struct;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service.Impl;

public class HexMeshService(
    IChunkService chunkService,
    ITileService tileService,
    IWallMeshService wallMeshService) : IHexMeshService
{
    private float _radius;
    private IHexGridChunk _chunk;

    public void Triangulate(float radius, IHexGridChunk hexGridChunk)
    {
        _radius = radius;
        _chunk = hexGridChunk;
        var tileIds = chunkService.GetById(hexGridChunk.Id).TileIds;
        var tiles = tileIds.Select(tileService.GetById);
        foreach (var tile in tiles)
            Triangulate(tile);
    }

    private void Triangulate(Tile tile)
    {
        // var corners = tile.GetCorners(_radius + tile.Height, 1f).ToList();
        for (var i = 0; i < tile.HexFaceIds.Count; i++)
            Triangulate(tile, i);
        if (!tile.IsUnderwater && !tile.HasRiver && !tile.HasRoads)
            _chunk.Features.AddFeature(tile, tile.GetCentroid(_radius + tileService.GetHeight(tile)));
    }

    // Godot 缠绕顺序是正面顺时针，所以从 i1 对应角落到 i2 对应角落相对于 tile 重心需要是顺时针
    private void Triangulate(Tile tile, int idx)
    {
        var height = tileService.GetHeight(tile);
        var v1 = tileService.GetFirstSolidCorner(tile, idx, _radius + height);
        var v2 = tileService.GetSecondSolidCorner(tile, idx, _radius + height);
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
        else
        {
            TriangulateWithoutRiver(tile, idx, centroid, e);
            if (!tile.IsUnderwater && !tile.HasRoadThroughEdge(idx))
                _chunk.Features.AddFeature(tile, (centroid + e.V1 + e.V5) / 3f);
        }

        TriangulateConnection(tile, idx, e);
        if (tile.IsUnderwater)
            TriangulateWater(tile, idx, centroid);
    }

    private void TriangulateWater(Tile tile, int idx, Vector3 centroid)
    {
        var waterSurfaceHeight = tileService.GetWaterSurfaceHeight(tile);
        centroid = Math3dUtil.ProjectToSphere(centroid, _radius + waterSurfaceHeight);
        var neighbor = tileService.GetNeighborByIdx(tile, idx);
        if (!neighbor.IsUnderwater)
            TriangulateWaterShore(tile, idx, waterSurfaceHeight, neighbor, centroid);
        else
        {
            TriangulateOpenWater(tile, idx, waterSurfaceHeight, neighbor, centroid);
        }
    }

    private void TriangulateWaterShore(Tile tile, int idx, float waterSurfaceHeight, Tile neighbor, Vector3 centroid)
    {
        var e1 = new EdgeVertices(tileService.GetFirstWaterCorner(tile, idx, _radius + waterSurfaceHeight),
            tileService.GetSecondWaterCorner(tile, idx, _radius + waterSurfaceHeight));
        _chunk.Water.AddTriangle([centroid, e1.V1, e1.V2]);
        _chunk.Water.AddTriangle([centroid, e1.V2, e1.V3]);
        _chunk.Water.AddTriangle([centroid, e1.V3, e1.V4]);
        _chunk.Water.AddTriangle([centroid, e1.V4, e1.V5]);
        // 使用邻居的水表面高度的话，就是希望考虑岸边地块的实际水位。暂时不按这个逻辑做，目前直接按地块水位平移至岸边。
        // var neighborWaterSurfaceHeight = tileService.GetWaterSurfaceHeight(neighbor);
        var cn1 = tileService.GetCornerByFaceId(neighbor, tile.HexFaceIds[idx],
            _radius + waterSurfaceHeight, HexMetrics.SolidFactor);
        var cn2 = tileService.GetCornerByFaceId(neighbor, tile.HexFaceIds[tile.NextIdx(idx)],
            _radius + waterSurfaceHeight, HexMetrics.SolidFactor);
        var e2 = new EdgeVertices(cn1, cn2);
        if (tile.HasRiverToNeighbor(neighbor.Id))
            TriangulateEstuary(e1, e2, tile.IncomingRiverNId == neighbor.Id);
        else
        {
            _chunk.WaterShore.AddQuad([e1.V1, e1.V2, e2.V1, e2.V2], uvs: QuadUv(0f, 0f, 0f, 1f));
            _chunk.WaterShore.AddQuad([e1.V2, e1.V3, e2.V2, e2.V3], uvs: QuadUv(0f, 0f, 0f, 1f));
            _chunk.WaterShore.AddQuad([e1.V3, e1.V4, e2.V3, e2.V4], uvs: QuadUv(0f, 0f, 0f, 1f));
            _chunk.WaterShore.AddQuad([e1.V4, e1.V5, e2.V4, e2.V5], uvs: QuadUv(0f, 0f, 0f, 1f));
        }

        var nextNeighbor = tileService.GetNeighborByIdx(tile, tile.NextIdx(idx));
        var cnn = tileService.GetCornerByFaceId(nextNeighbor, tile.HexFaceIds[tile.NextIdx(idx)],
            _radius + tileService.GetWaterSurfaceHeight(nextNeighbor),
            nextNeighbor.IsUnderwater ? HexMetrics.WaterFactor : HexMetrics.SolidFactor);
        _chunk.WaterShore.AddTriangle([e1.V5, e2.V5, cnn],
            uvs: [new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(0f, nextNeighbor.IsUnderwater ? 0f : 1f)]);
    }

    private void TriangulateEstuary(EdgeVertices e1, EdgeVertices e2, bool incomingRiver)
    {
        _chunk.WaterShore.AddTriangle([e2.V1, e1.V2, e1.V1],
            uvs: [new Vector2(0f, 1f), new Vector2(0f, 0f), new Vector2(0f, 0f)]);
        _chunk.WaterShore.AddTriangle([e2.V5, e1.V5, e1.V4],
            uvs: [new Vector2(0f, 1f), new Vector2(0f, 0f), new Vector2(0f, 0f)]);
        _chunk.Estuary.AddQuad([e2.V1, e1.V2, e2.V2, e1.V3],
            uvs: [new Vector2(0f, 1f), new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(0f, 0f)],
            uvs2: incomingRiver
                ? [new Vector2(1.5f, 1f), new Vector2(0.7f, 1.15f), new Vector2(1f, 0.8f), new Vector2(0.5f, 1.1f)]
                :
                [
                    new Vector2(-0.5f, -0.2f), new Vector2(0.3f, -0.35f), new Vector2(0f, 0f), new Vector2(0.5f, -0.3f)
                ]);
        _chunk.Estuary.AddTriangle([e1.V3, e2.V2, e2.V4],
            uvs: [new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(1f, 1f)],
            uvs2: incomingRiver
                ? [new Vector2(0.5f, 1.1f), new Vector2(1f, 0.8f), new Vector2(0f, 0.8f)]
                : [new Vector2(0.5f, -0.3f), new Vector2(0f, 0f), new Vector2(1f, 0f)]);
        _chunk.Estuary.AddQuad([e1.V3, e1.V4, e2.V4, e2.V5],
            uvs: [new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(0f, 1f)],
            uvs2: incomingRiver
                ? [new Vector2(0.5f, 1.1f), new Vector2(0.3f, 1.15f), new Vector2(0f, 0.8f), new Vector2(-0.5f, 1f)]
                : [new Vector2(0.5f, -0.3f), new Vector2(0.7f, -0.35f), new Vector2(1f, 0f), new Vector2(1.5f, -0.2f)]);
    }

    private void TriangulateOpenWater(Tile tile, int idx, float waterSurfaceHeight, Tile neighbor, Vector3 centroid)
    {
        var c1 = tileService.GetFirstWaterCorner(tile, idx, _radius + waterSurfaceHeight);
        var c2 = tileService.GetSecondWaterCorner(tile, idx, _radius + waterSurfaceHeight);
        _chunk.Water.AddTriangle([centroid, c1, c2]);
        // 由更大 Id 的地块绘制水域连接
        if (tile.Id > neighbor.Id)
        {
            var neighborWaterSurfaceHeight = tileService.GetWaterSurfaceHeight(neighbor);
            var cn1 = tileService.GetCornerByFaceId(neighbor, tile.HexFaceIds[idx],
                _radius + neighborWaterSurfaceHeight, HexMetrics.WaterFactor);
            var cn2 = tileService.GetCornerByFaceId(neighbor, tile.HexFaceIds[tile.NextIdx(idx)],
                _radius + neighborWaterSurfaceHeight, HexMetrics.WaterFactor);
            _chunk.Water.AddQuad([c1, c2, cn1, cn2]);

            var preNeighbor = tileService.GetNeighborByIdx(tile, tile.PreviousIdx(idx));
            // 由最大 Id 的地块绘制水域角落三角形
            if (tile.Id > preNeighbor.Id)
            {
                if (!preNeighbor.IsUnderwater) return;
                var cpn = tileService.GetCornerByFaceId(preNeighbor, tile.HexFaceIds[idx],
                    _radius + tileService.GetWaterSurfaceHeight(preNeighbor), HexMetrics.WaterFactor);
                _chunk.Water.AddTriangle([c1, cpn, cn1]);
            }
        }
    }

    private void TriangulateAdjacentToRiver(Tile tile, int idx, Vector3 centroid, EdgeVertices e)
    {
        if (tile.HasRoads)
            TriangulateRoadAdjacentToRiver(tile, idx, centroid, e);
        if (tileService.HasRiverThroughEdge(tile, tile.NextIdx(idx)))
        {
            if (tileService.HasRiverThroughEdge(tile, tile.PreviousIdx(idx)))
                centroid = tileService.GetSolidEdgeMiddle(tile, idx, _radius + tileService.GetHeight(tile),
                    0.5f * HexMetrics.InnerToOuter);
            else if (tileService.HasRiverThroughEdge(tile, tile.Previous2Idx(idx)))
                centroid = tileService.GetFirstSolidCorner(tile, idx, _radius + tileService.GetHeight(tile), 0.25f);
        }
        else if (tileService.HasRiverThroughEdge(tile, tile.PreviousIdx(idx)) &&
                 tileService.HasRiverThroughEdge(tile, tile.Next2Idx(idx)))
            centroid = tileService.GetSecondSolidCorner(tile, idx, _radius + tileService.GetHeight(tile), 0.25f);

        var m = new EdgeVertices(centroid.Lerp(e.V1, 0.5f), centroid.Lerp(e.V5, 0.5f));
        TriangulateEdgeStrip(m, tile.Color, e, tile.Color);
        TriangulateEdgeFan(centroid, m, tile.Color);

        if (!tile.IsUnderwater && !tile.HasRoadThroughEdge(idx))
            _chunk.Features.AddFeature(tile, (centroid + e.V1 + e.V5) / 3f);
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
                _radius + tileService.GetHeight(tile), 1f / 3f) - centroid;
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
                    corner = tileService.GetSecondSolidCorner(tile, idx, _radius + tileService.GetHeight(tile));
                }
                else
                {
                    if (!hasRoadThroughEdge && !tile.HasRoadThroughEdge(tile.PreviousIdx(idx))) return;
                    corner = tileService.GetFirstSolidCorner(tile, idx, _radius + tileService.GetHeight(tile));
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
                var offset = tileService.GetSolidEdgeMiddle(tile, idx, _radius + tileService.GetHeight(tile),
                    HexMetrics.InnerToOuter);
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
                roadCenter +=
                    tileService.GetSolidEdgeMiddle(tile, middleIdx, _radius + tileService.GetHeight(tile), 0.25f) -
                    centroid;
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

        if (!tile.IsUnderwater)
        {
            var reversed = tile.HasIncomingRiver;
            var riverSurfaceHeight = _radius + tileService.GetRiverSurfaceHeight(tile);
            TriangulateRiverQuad(m.V2, m.V4, e.V2, e.V4, riverSurfaceHeight, 0.6f, reversed);
            centroid = Math3dUtil.ProjectToSphere(centroid, riverSurfaceHeight);
            m.V2 = Math3dUtil.ProjectToSphere(m.V2, riverSurfaceHeight);
            m.V4 = Math3dUtil.ProjectToSphere(m.V4, riverSurfaceHeight);
            _chunk.Rivers.AddTriangle([centroid, m.V2, m.V4],
                uvs: reversed
                    ? [new Vector2(0.5f, 0.4f), new Vector2(1f, 0.2f), new Vector2(0f, 0.2f)]
                    : [new Vector2(0.5f, 0.4f), new Vector2(0f, 0.6f), new Vector2(1f, 0.6f)]);
        }
    }

    private void TriangulateWithRiver(Tile tile, int idx, Vector3 centroid, EdgeVertices e)
    {
        Vector3 centerL;
        Vector3 centerR;
        var height = tileService.GetHeight(tile);
        if (tileService.HasRiverThroughEdge(tile, tile.OppositeIdx(idx)))
        {
            centerL = tileService.GetFirstSolidCorner(tile, tile.PreviousIdx(idx), _radius + height, 0.25f);
            centerR = tileService.GetSecondSolidCorner(tile, tile.NextIdx(idx), _radius + height, 0.25f);
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
            centerR = tileService.GetSolidEdgeMiddle(tile, tile.NextIdx(idx),
                _radius + height, 0.5f * HexMetrics.InnerToOuter);
        }
        else if (tileService.HasRiverThroughEdge(tile, tile.Previous2Idx(idx)))
        {
            centerL = tileService.GetSolidEdgeMiddle(tile, tile.PreviousIdx(idx),
                _radius + height, 0.5f * HexMetrics.InnerToOuter);
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
        _chunk.Terrain.AddTriangle([centerL, m.V1, m.V2], TriColor(tile.Color));
        _chunk.Terrain.AddQuad([centerL, centroid, m.V2, m.V3], QuadColor(tile.Color));
        _chunk.Terrain.AddQuad([centroid, centerR, m.V3, m.V4], QuadColor(tile.Color));
        _chunk.Terrain.AddTriangle([centerR, m.V4, m.V5], TriColor(tile.Color));

        if (!tile.IsUnderwater)
        {
            var reversed = tile.IncomingRiverNId == tileService.GetNeighborByIdx(tile, idx).Id;
            var riverSurfaceHeight = tileService.GetRiverSurfaceHeight(tile);
            TriangulateRiverQuad(centerL, centerR, m.V2, m.V4,
                _radius + riverSurfaceHeight, 0.4f, reversed);
            TriangulateRiverQuad(m.V2, m.V4, e.V2, e.V4,
                _radius + riverSurfaceHeight, 0.6f, reversed);
        }
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
        _chunk.Rivers.AddQuad([v1, v2, v3, v4],
            uvs: reversed
                ? QuadUv(1f, 0f, 0.8f - v, 0.6f - v)
                : QuadUv(0f, 1f, v, v + 0.2f));
    }

    private void TriangulateEdgeFan(Vector3 center, EdgeVertices edge, Color color)
    {
        _chunk.Terrain.AddTriangle([center, edge.V1, edge.V2], TriColor(color));
        _chunk.Terrain.AddTriangle([center, edge.V2, edge.V3], TriColor(color));
        _chunk.Terrain.AddTriangle([center, edge.V3, edge.V4], TriColor(color));
        _chunk.Terrain.AddTriangle([center, edge.V4, edge.V5], TriColor(color));
    }

    private void TriangulateEdgeStrip(EdgeVertices e1, Color c1, EdgeVertices e2, Color c2, bool hasRoad = false)
    {
        _chunk.Terrain.AddQuad([e1.V1, e1.V2, e2.V1, e2.V2], QuadColor(c1, c2));
        _chunk.Terrain.AddQuad([e1.V2, e1.V3, e2.V2, e2.V3], QuadColor(c1, c2));
        _chunk.Terrain.AddQuad([e1.V3, e1.V4, e2.V3, e2.V4], QuadColor(c1, c2));
        _chunk.Terrain.AddQuad([e1.V4, e1.V5, e2.V4, e2.V5], QuadColor(c1, c2));
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
            _chunk.Roads.AddTriangle([centroid, mL, mC],
                uvs: [new Vector2(1f, 0f), new Vector2(0f, 0f), new Vector2(1f, 0f)]);
            _chunk.Roads.AddTriangle([centroid, mC, mR],
                uvs: [new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(0f, 0f)]);
        }
        else TriangulateRoadEdge(centroid, mL, mR);
    }

    private void TriangulateRoadEdge(Vector3 centroid, Vector3 mL, Vector3 mR)
    {
        _chunk.Roads.AddTriangle([centroid, mL, mR],
            uvs: [new Vector2(1f, 0f), new Vector2(0f, 0f), new Vector2(0f, 0f)]);
    }

    // 顶点的排序：
    // v4  v5  v6
    // v1  v2  v3
    // 0.0 1.0 0.0
    private void TriangulateRoadSegment(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, Vector3 v5, Vector3 v6)
    {
        _chunk.Roads.AddQuad([v1, v2, v4, v5], uvs: QuadUv(0f, 1f, 0f, 0f));
        _chunk.Roads.AddQuad([v2, v3, v5, v6], uvs: QuadUv(1f, 0f, 0f, 0f));
    }

    private void TriangulateWaterfallInWater(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4,
        float height1, float height2, float waterHeight)
    {
        v1 = Math3dUtil.ProjectToSphere(v1, height1);
        v2 = Math3dUtil.ProjectToSphere(v2, height1);
        v3 = Math3dUtil.ProjectToSphere(v3, height2);
        v4 = Math3dUtil.ProjectToSphere(v4, height2);
        v1 = HexMetrics.Perturb(v1);
        v2 = HexMetrics.Perturb(v2);
        v3 = HexMetrics.Perturb(v3);
        v4 = HexMetrics.Perturb(v4);
        var t = (waterHeight - height2) / (height1 - height2);
        v3 = v3.Lerp(v1, t);
        v4 = v4.Lerp(v2, t);
        _chunk.Rivers.AddQuadUnperturbed([v1, v2, v3, v4], uvs: QuadUv(0f, 1f, 0.8f, 1f));
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
        var hasRiver = tile.HasRiverToNeighbor(neighbor.Id);
        var hasRoad = tile.HasRoadThroughEdge(idx);
        if (hasRiver)
        {
            en.V3 = Math3dUtil.ProjectToSphere(en.V3, _radius + tileService.GetStreamBedHeight(neighbor));
            if (!tile.IsUnderwater)
            {
                if (!neighbor.IsUnderwater)
                    TriangulateRiverQuad(e.V2, e.V4, en.V2, en.V4,
                        _radius + tileService.GetRiverSurfaceHeight(tile),
                        _radius + tileService.GetRiverSurfaceHeight(neighbor), 0.8f,
                        tile.HasIncomingRiver && tile.IncomingRiverNId == neighbor.Id);
                else if (tile.Elevation > neighbor.Elevation)
                    TriangulateWaterfallInWater(e.V2, e.V4, en.V2, en.V4,
                        _radius + tileService.GetRiverSurfaceHeight(tile),
                        _radius + tileService.GetRiverSurfaceHeight(neighbor),
                        _radius + tileService.GetWaterSurfaceHeight(neighbor));
            }
            else if (!neighbor.IsUnderwater && neighbor.Elevation > tile.Elevation)
                TriangulateWaterfallInWater(en.V4, en.V2, e.V4, e.V2,
                    _radius + tileService.GetRiverSurfaceHeight(neighbor),
                    _radius + tileService.GetRiverSurfaceHeight(tile),
                    _radius + tileService.GetWaterSurfaceHeight(tile));
        }

        if (tile.GetEdgeType(neighbor) == HexEdgeType.Slope)
            TriangulateEdgeTerraces(e, tile, en, neighbor, hasRoad);
        else
            TriangulateEdgeStrip(e, tile.Color, en, neighbor.Color, hasRoad);

        wallMeshService.AddWall(_chunk.Features.Walls, e, tile, en, neighbor, hasRiver, hasRoad);

        var preNeighbor = tileService.GetNeighborByIdx(tile, tile.PreviousIdx(idx));
        var preNeighborHeight = tileService.GetHeight(preNeighbor);
        if (tileHeight < preNeighborHeight
            || (Mathf.Abs(tileHeight - preNeighborHeight) < 0.00001f && tile.Id > preNeighbor.Id))
        {
            // 连接角落的三角形由周围 3 个地块中最低或者一样高时 Id 最大的生成
            var vpn = tileService.GetCornerByFaceId(preNeighbor, tile.HexFaceIds[idx],
                _radius + preNeighborHeight, HexMetrics.SolidFactor);
            TriangulateCorner(e.V1, tile, vpn, preNeighbor, vn1, neighbor);
        }
    }

    // 需要保证入参 bottom -> left -> right 是顺时针
    private void TriangulateCorner(Vector3 bottom, Tile bottomTile,
        Vector3 left, Tile leftTile, Vector3 right, Tile rightTile)
    {
        var edgeType1 = bottomTile.GetEdgeType(leftTile);
        var edgeType2 = bottomTile.GetEdgeType(rightTile);
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
        else if (leftTile.GetEdgeType(rightTile) == HexEdgeType.Slope)
        {
            if (leftTile.Elevation < rightTile.Elevation)
                TriangulateCornerCliffTerraces(right, rightTile, bottom, bottomTile, left, leftTile);
            else
                TriangulateCornerTerracesCliff(left, leftTile, right, rightTile, bottom, bottomTile);
        }
        else
            _chunk.Terrain.AddTriangle([bottom, left, right], [bottomTile.Color, leftTile.Color, rightTile.Color]);
        
        wallMeshService.AddWall(_chunk.Features.Walls, bottom, bottomTile, left, leftTile, right, rightTile);
    }

    // 三角形靠近 tile 的左边是阶地，右边是悬崖，另一边任意的情况
    private void TriangulateCornerTerracesCliff(Vector3 begin, Tile beginTile,
        Vector3 left, Tile leftTile, Vector3 right, Tile rightTile)
    {
        var b = 1f / Mathf.Abs(rightTile.Elevation - beginTile.Elevation);
        var boundary = HexMetrics.Perturb(begin).Lerp(HexMetrics.Perturb(right), b);
        var boundaryColor = beginTile.Color.Lerp(rightTile.Color, b);
        TriangulateBoundaryTriangle(begin, beginTile, left, leftTile, boundary, boundaryColor);
        if (leftTile.GetEdgeType(rightTile) == HexEdgeType.Slope)
            TriangulateBoundaryTriangle(left, leftTile, right, rightTile, boundary, boundaryColor);
        else
            _chunk.Terrain.AddTriangleUnperturbed([HexMetrics.Perturb(left), HexMetrics.Perturb(right), boundary],
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
        if (leftTile.GetEdgeType(rightTile) == HexEdgeType.Slope)
            TriangulateBoundaryTriangle(left, leftTile, right, rightTile, boundary, boundaryColor);
        else
            _chunk.Terrain.AddTriangleUnperturbed([HexMetrics.Perturb(left), HexMetrics.Perturb(right), boundary],
                [leftTile.Color, rightTile.Color, boundaryColor]);
    }

    // 阶地和悬崖中间的半三角形
    private void TriangulateBoundaryTriangle(Vector3 begin, Tile beginTile,
        Vector3 left, Tile leftTile, Vector3 boundary, Color boundaryColor)
    {
        var v2 = HexMetrics.Perturb(HexMetrics.TerraceLerp(begin, left, 1));
        var c2 = HexMetrics.TerraceLerp(beginTile.Color, leftTile.Color, 1);
        _chunk.Terrain.AddTriangleUnperturbed([HexMetrics.Perturb(begin), v2, boundary],
            [beginTile.Color, c2, boundaryColor]);
        for (var i = 2; i < HexMetrics.TerraceSteps; i++)
        {
            var v1 = v2;
            var c1 = c2;
            v2 = HexMetrics.Perturb(HexMetrics.TerraceLerp(begin, left, i));
            c2 = HexMetrics.TerraceLerp(beginTile.Color, leftTile.Color, i);
            _chunk.Terrain.AddTriangleUnperturbed([v1, v2, boundary], [c1, c2, boundaryColor]);
        }

        _chunk.Terrain.AddTriangleUnperturbed([v2, HexMetrics.Perturb(left), boundary],
            [c2, leftTile.Color, boundaryColor]);
    }

    // 处理高度不同的 beginTile 和两个高度相同的 endTile（即三角形两边是等高阶地，一边是平地）的情况
    private void TriangulateCornerTerraces(Vector3 begin, Tile beginTile,
        Vector3 left, Tile leftTile, Vector3 right, Tile rightTile)
    {
        var v3 = HexMetrics.TerraceLerp(begin, left, 1);
        var v4 = HexMetrics.TerraceLerp(begin, right, 1);
        var c3 = HexMetrics.TerraceLerp(beginTile.Color, leftTile.Color, 1);
        var c4 = HexMetrics.TerraceLerp(beginTile.Color, rightTile.Color, 1);
        _chunk.Terrain.AddTriangle([begin, v3, v4], [beginTile.Color, c3, c4]);
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
            _chunk.Terrain.AddQuad([v1, v2, v3, v4], [c1, c2, c3, c4]);
        }

        _chunk.Terrain.AddQuad([v3, v4, left, right], [c3, c4, leftTile.Color, rightTile.Color]);
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