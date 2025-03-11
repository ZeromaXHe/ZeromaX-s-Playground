using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Dependency;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Enum;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Node;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Struct;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util;

public interface IChunk
{
    HexMesh Terrain { get; }
    HexMesh Rivers { get; }
    HexMesh Roads { get; }
    HexMesh Water { get; }
    HexMesh WaterShore { get; }
    HexMesh Estuary { get; }
    HexFeatureManager Features { get; }
    HexTileDataOverrider TileDataOverrider { get; }
}

public class ChunkTriangulation
{
    public ChunkTriangulation(IChunk chunk)
    {
        InitServices();
        _chunk = chunk;
    }

    #region 服务

    private static ITileService _tileService;
    private static IPlanetSettingService _planetSettingService;
    private static INoiseService _noiseService;

    private static void InitServices()
    {
        _tileService ??= Context.GetBean<ITileService>();
        _planetSettingService ??= Context.GetBean<IPlanetSettingService>();
        _noiseService ??= Context.GetBean<INoiseService>();
    }

    #endregion

    private readonly IChunk _chunk;

    private HexTileDataOverrider Overrider => _chunk.TileDataOverrider;

    private float GetOverrideHeight(Tile tile) =>
        Overrider.IsOverrideTile(tile)
            ? _tileService.GetOverrideHeight(tile, Overrider)
            : _tileService.GetHeight(tile);

    public void Triangulate(Tile tile)
    {
        for (var i = 0; i < tile.HexFaceIds.Count; i++)
            Triangulate(tile, i);
        if (!Overrider.IsUnderwater(tile))
        {
            if (!Overrider.HasRiver(tile) && !Overrider.HasRoads(tile))
                _chunk.Features.AddFeature(tile,
                    tile.GetCentroid(_planetSettingService.Radius + GetOverrideHeight(tile)), Overrider);
            if (Overrider.IsSpecial(tile))
                _chunk.Features.AddSpecialFeature(tile,
                    tile.GetCentroid(_planetSettingService.Radius + GetOverrideHeight(tile)), Overrider);
        }
    }

    // Godot 缠绕顺序是正面顺时针，所以从 i1 对应角落到 i2 对应角落相对于 tile 重心需要是顺时针
    private void Triangulate(Tile tile, int idx)
    {
        var height = GetOverrideHeight(tile);
        var v1 = _tileService.GetFirstSolidCorner(tile, idx, _planetSettingService.Radius + height);
        var v2 = _tileService.GetSecondSolidCorner(tile, idx, _planetSettingService.Radius + height);
        var e = new EdgeVertices(v1, v2);
        var centroid = tile.GetCentroid(_planetSettingService.Radius + height);
        if (Overrider.HasRiver(tile))
        {
            if (Overrider.HasRiverThroughEdge(tile, idx))
            {
                e.V3 = Math3dUtil.ProjectToSphere(e.V3, _planetSettingService.Radius + Overrider.StreamBedY(tile));
                if (Overrider.HasRiverBeginOrEnd(tile))
                    TriangulateWithRiverBeginOrEnd(tile, centroid, e);
                else TriangulateWithRiver(tile, idx, centroid, e);
            }
            else TriangulateAdjacentToRiver(tile, idx, centroid, e);
        }
        else
        {
            TriangulateWithoutRiver(tile, idx, centroid, e);
            if (!Overrider.IsUnderwater(tile) && !Overrider.HasRoadThroughEdge(tile, idx))
                _chunk.Features.AddFeature(tile, (centroid + e.V1 + e.V5) / 3f, Overrider);
        }

        TriangulateConnection(tile, idx, e);
        if (Overrider.IsUnderwater(tile))
            TriangulateWater(tile, idx, centroid);
    }

    private void TriangulateWater(Tile tile, int idx, Vector3 centroid)
    {
        var waterSurfaceHeight = Overrider.WaterSurfaceY(tile);
        centroid = Math3dUtil.ProjectToSphere(centroid, _planetSettingService.Radius + waterSurfaceHeight);
        var neighbor = _tileService.GetNeighborByIdx(tile, idx);
        if (!Overrider.IsUnderwater(neighbor))
            TriangulateWaterShore(tile, idx, waterSurfaceHeight, neighbor, centroid);
        else
            TriangulateOpenWater(tile, idx, waterSurfaceHeight, neighbor, centroid);
    }

    private void TriangulateWaterShore(Tile tile, int idx, float waterSurfaceHeight, Tile neighbor, Vector3 centroid)
    {
        var e1 = new EdgeVertices(
            _tileService.GetFirstWaterCorner(tile, idx, _planetSettingService.Radius + waterSurfaceHeight),
            _tileService.GetSecondWaterCorner(tile, idx, _planetSettingService.Radius + waterSurfaceHeight));
        var ids = new Vector3(tile.Id, neighbor.Id, tile.Id);
        _chunk.Water.AddTriangle([centroid, e1.V1, e1.V2], HexMesh.TriArr(HexMesh.Weights1), tis: ids);
        _chunk.Water.AddTriangle([centroid, e1.V2, e1.V3], HexMesh.TriArr(HexMesh.Weights1), tis: ids);
        _chunk.Water.AddTriangle([centroid, e1.V3, e1.V4], HexMesh.TriArr(HexMesh.Weights1), tis: ids);
        _chunk.Water.AddTriangle([centroid, e1.V4, e1.V5], HexMesh.TriArr(HexMesh.Weights1), tis: ids);
        // 使用邻居的水表面高度的话，就是希望考虑岸边地块的实际水位。(不然强行拉平岸边的话，角落两个水面不一样高时太多复杂逻辑，bug 太多)
        var neighborWaterSurfaceHeight = Overrider.WaterSurfaceY(neighbor);
        var cn1 = _tileService.GetCornerByFaceId(neighbor, tile.HexFaceIds[idx],
            _planetSettingService.Radius + neighborWaterSurfaceHeight, HexMetrics.SolidFactor);
        var cn2 = _tileService.GetCornerByFaceId(neighbor, tile.HexFaceIds[tile.NextIdx(idx)],
            _planetSettingService.Radius + neighborWaterSurfaceHeight, HexMetrics.SolidFactor);
        var e2 = new EdgeVertices(cn1, cn2);
        var neighborIdx = tile.GetNeighborIdx(neighbor);
        if (Overrider.HasRiverThroughEdge(tile, neighborIdx))
            TriangulateEstuary(e1, e2, Overrider.HasIncomingRiverThroughEdge(tile, neighborIdx), ids);
        else
        {
            _chunk.WaterShore.AddQuad([e1.V1, e1.V2, e2.V1, e2.V2],
                HexMesh.QuadArr(HexMesh.Weights1, HexMesh.Weights2),
                QuadUv(0f, 0f, 0f, 1f), tis: ids);
            _chunk.WaterShore.AddQuad([e1.V2, e1.V3, e2.V2, e2.V3],
                HexMesh.QuadArr(HexMesh.Weights1, HexMesh.Weights2),
                QuadUv(0f, 0f, 0f, 1f), tis: ids);
            _chunk.WaterShore.AddQuad([e1.V3, e1.V4, e2.V3, e2.V4],
                HexMesh.QuadArr(HexMesh.Weights1, HexMesh.Weights2),
                QuadUv(0f, 0f, 0f, 1f), tis: ids);
            _chunk.WaterShore.AddQuad([e1.V4, e1.V5, e2.V4, e2.V5],
                HexMesh.QuadArr(HexMesh.Weights1, HexMesh.Weights2),
                QuadUv(0f, 0f, 0f, 1f), tis: ids);
        }

        var nextNeighbor = _tileService.GetNeighborByIdx(tile, tile.NextIdx(idx));
        var nextNeighborWaterSurfaceHeight = Overrider.WaterSurfaceY(nextNeighbor);
        var cnn = _tileService.GetCornerByFaceId(nextNeighbor, tile.HexFaceIds[tile.NextIdx(idx)],
            _planetSettingService.Radius + nextNeighborWaterSurfaceHeight,
            Overrider.IsUnderwater(nextNeighbor) ? HexMetrics.WaterFactor : HexMetrics.SolidFactor);
        ids.Z = nextNeighbor.Id;
        _chunk.WaterShore.AddTriangle([e1.V5, e2.V5, cnn], [HexMesh.Weights1, HexMesh.Weights2, HexMesh.Weights3],
            [new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(0f, Overrider.IsUnderwater(nextNeighbor) ? 0f : 1f)],
            tis: ids);
    }

    private void TriangulateEstuary(EdgeVertices e1, EdgeVertices e2, bool incomingRiver, Vector3 ids)
    {
        _chunk.WaterShore.AddTriangle([e2.V1, e1.V2, e1.V1], [HexMesh.Weights2, HexMesh.Weights1, HexMesh.Weights1],
            [new Vector2(0f, 1f), new Vector2(0f, 0f), new Vector2(0f, 0f)], tis: ids);
        _chunk.WaterShore.AddTriangle([e2.V5, e1.V5, e1.V4], [HexMesh.Weights2, HexMesh.Weights1, HexMesh.Weights1],
            [new Vector2(0f, 1f), new Vector2(0f, 0f), new Vector2(0f, 0f)], tis: ids);
        _chunk.Estuary.AddQuad([e2.V1, e1.V2, e2.V2, e1.V3],
            [HexMesh.Weights2, HexMesh.Weights1, HexMesh.Weights2, HexMesh.Weights1],
            [new Vector2(0f, 1f), new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(0f, 0f)],
            incomingRiver
                ? [new Vector2(1.5f, 1f), new Vector2(0.7f, 1.15f), new Vector2(1f, 0.8f), new Vector2(0.5f, 1.1f)]
                :
                [
                    new Vector2(-0.5f, -0.2f), new Vector2(0.3f, -0.35f), new Vector2(0f, 0f), new Vector2(0.5f, -0.3f)
                ],
            ids);
        _chunk.Estuary.AddTriangle([e1.V3, e2.V2, e2.V4], [HexMesh.Weights1, HexMesh.Weights2, HexMesh.Weights2],
            [new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(1f, 1f)],
            incomingRiver
                ? [new Vector2(0.5f, 1.1f), new Vector2(1f, 0.8f), new Vector2(0f, 0.8f)]
                : [new Vector2(0.5f, -0.3f), new Vector2(0f, 0f), new Vector2(1f, 0f)],
            ids);
        _chunk.Estuary.AddQuad([e1.V3, e1.V4, e2.V4, e2.V5], HexMesh.QuadArr(HexMesh.Weights1, HexMesh.Weights2),
            [new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(0f, 1f)],
            incomingRiver
                ? [new Vector2(0.5f, 1.1f), new Vector2(0.3f, 1.15f), new Vector2(0f, 0.8f), new Vector2(-0.5f, 1f)]
                : [new Vector2(0.5f, -0.3f), new Vector2(0.7f, -0.35f), new Vector2(1f, 0f), new Vector2(1.5f, -0.2f)],
            ids);
    }

    private void TriangulateOpenWater(Tile tile, int idx, float waterSurfaceHeight, Tile neighbor, Vector3 centroid)
    {
        var c1 = _tileService.GetFirstWaterCorner(tile, idx, _planetSettingService.Radius + waterSurfaceHeight);
        var c2 = _tileService.GetSecondWaterCorner(tile, idx, _planetSettingService.Radius + waterSurfaceHeight);
        var ids = Vector3.One * tile.Id;
        _chunk.Water.AddTriangle([centroid, c1, c2], HexMesh.TriArr(HexMesh.Weights1), tis: ids);
        // 由更大 Id 的地块绘制水域连接
        if (tile.Id <= neighbor.Id)
            return;
        var neighborWaterSurfaceHeight = Overrider.WaterSurfaceY(neighbor);
        var cn1 = _tileService.GetCornerByFaceId(neighbor, tile.HexFaceIds[idx],
            _planetSettingService.Radius + neighborWaterSurfaceHeight, HexMetrics.WaterFactor);
        var cn2 = _tileService.GetCornerByFaceId(neighbor, tile.HexFaceIds[tile.NextIdx(idx)],
            _planetSettingService.Radius + neighborWaterSurfaceHeight, HexMetrics.WaterFactor);
        ids.Y = neighbor.Id;
        _chunk.Water.AddQuad([c1, c2, cn1, cn2], HexMesh.QuadArr(HexMesh.Weights1, HexMesh.Weights2), tis: ids);

        var nextNeighbor = _tileService.GetNeighborByIdx(tile, tile.NextIdx(idx));
        // 由最大 Id 的地块绘制水域角落三角形
        if (tile.Id <= nextNeighbor.Id)
            return;
        if (!Overrider.IsUnderwater(nextNeighbor))
            return;
        var cnn = _tileService.GetCornerByFaceId(nextNeighbor, tile.HexFaceIds[tile.NextIdx(idx)],
            _planetSettingService.Radius + Overrider.WaterSurfaceY(nextNeighbor), HexMetrics.WaterFactor);
        ids.Z = nextNeighbor.Id;
        _chunk.Water.AddTriangle([c2, cn2, cnn], [HexMesh.Weights1, HexMesh.Weights2, HexMesh.Weights3], tis: ids);
    }

    private void TriangulateAdjacentToRiver(Tile tile, int idx, Vector3 centroid, EdgeVertices e)
    {
        if (Overrider.HasRoads(tile))
            TriangulateRoadAdjacentToRiver(tile, idx, centroid, e);
        if (Overrider.HasRiverThroughEdge(tile, tile.NextIdx(idx)))
        {
            if (Overrider.HasRiverThroughEdge(tile, tile.PreviousIdx(idx)))
                centroid = _tileService.GetSolidEdgeMiddle(tile, idx,
                    _planetSettingService.Radius + GetOverrideHeight(tile),
                    0.5f * HexMetrics.InnerToOuter);
            else if (!tile.IsPentagon() && Overrider.HasRiverThroughEdge(tile, tile.Previous2Idx(idx)))
                // 注意五边形没有直线河流，一边临河另一边隔一个方向临河的情况是对应钝角河的外河岸，依然在 centroid
                centroid = _tileService.GetFirstSolidCorner(tile, idx,
                    _planetSettingService.Radius + GetOverrideHeight(tile),
                    0.25f);
        }
        else if (!tile.IsPentagon()
                 && Overrider.HasRiverThroughEdge(tile, tile.PreviousIdx(idx))
                 && Overrider.HasRiverThroughEdge(tile, tile.Next2Idx(idx)))
            // 注意五边形没有直线河流，一边临河另一边隔一个方向临河的情况是对应钝角河的外河岸，依然在 centroid
            centroid = _tileService.GetSecondSolidCorner(tile, idx,
                _planetSettingService.Radius + GetOverrideHeight(tile),
                0.25f);

        var m = new EdgeVertices(centroid.Lerp(e.V1, 0.5f), centroid.Lerp(e.V5, 0.5f));
        TriangulateEdgeStrip(m, HexMesh.Weights1, tile.Id, e, HexMesh.Weights1, tile.Id);
        TriangulateEdgeFan(centroid, m, tile.Id);

        if (!Overrider.IsUnderwater(tile) && !Overrider.HasRoadThroughEdge(tile, idx))
            _chunk.Features.AddFeature(tile, (centroid + e.V1 + e.V5) / 3f, Overrider);
    }

    private void TriangulateRoadAdjacentToRiver(Tile tile, int idx, Vector3 centroid, EdgeVertices e)
    {
        var hasRoadThroughEdge = Overrider.HasRoadThroughEdge(tile, idx);
        var previousHasRiver = Overrider.HasRiverThroughEdge(tile, tile.PreviousIdx(idx));
        var nextHasRiver = Overrider.HasRiverThroughEdge(tile, tile.NextIdx(idx));
        var interpolator = GetRoadInterpolator(tile, idx);
        var incomingRiverIdx = tile.Data.Flags.RiverInDirection();
        var outgoingRiverIdx = tile.Data.Flags.RiverOutDirection();
        var roadCenter = centroid;
        if (Overrider.HasRiverBeginOrEnd(tile))
        {
            var riverBeginOrEndIdx = Overrider.HasIncomingRiver(tile) ? incomingRiverIdx : outgoingRiverIdx;
            if (tile.IsPentagon())
                roadCenter += _tileService.GetFirstSolidCorner(tile, tile.OppositeIdx(riverBeginOrEndIdx),
                    _planetSettingService.Radius + GetOverrideHeight(tile), HexMetrics.OuterToInner / 3f) - centroid;
            else
                roadCenter += _tileService.GetSolidEdgeMiddle(tile, tile.OppositeIdx(riverBeginOrEndIdx),
                    _planetSettingService.Radius + GetOverrideHeight(tile), 1f / 3f) - centroid;
        }
        else
        {
            if (!tile.IsPentagon() && incomingRiverIdx == tile.OppositeIdx(outgoingRiverIdx))
            {
                // 河流走势是对边（直线）的情况（需要注意五边形没有对边的概念）
                Vector3 corner;
                if (previousHasRiver)
                {
                    if (!hasRoadThroughEdge && !Overrider.HasRoadThroughEdge(tile, tile.NextIdx(idx))) return;
                    corner = _tileService.GetSecondSolidCorner(tile, idx,
                        _planetSettingService.Radius + GetOverrideHeight(tile));
                }
                else
                {
                    if (!hasRoadThroughEdge && !Overrider.HasRoadThroughEdge(tile, tile.PreviousIdx(idx))) return;
                    corner = _tileService.GetFirstSolidCorner(tile, idx,
                        _planetSettingService.Radius + GetOverrideHeight(tile));
                }

                roadCenter += (corner - centroid) * 0.5f;
                if (incomingRiverIdx == tile.NextIdx(idx)
                    && (Overrider.HasRoadThroughEdge(tile, tile.Next2Idx(idx))
                        || Overrider.HasRoadThroughEdge(tile, tile.OppositeIdx(idx))))
                    _chunk.Features.AddBridge(tile, roadCenter, centroid - (corner - centroid) * 0.5f);
                centroid += (corner - centroid) * 0.25f;
            }
            else if (incomingRiverIdx == tile.PreviousIdx(outgoingRiverIdx))
            {
                // 河流走势是逆时针锐角的情况
                roadCenter -= _tileService.GetSecondCorner(tile, incomingRiverIdx,
                    _planetSettingService.Radius + GetOverrideHeight(tile), 0.2f) - centroid;
            }
            else if (incomingRiverIdx == tile.NextIdx(outgoingRiverIdx))
            {
                // 河流走势是顺时针锐角的情况
                roadCenter -= _tileService.GetFirstCorner(tile, incomingRiverIdx,
                    _planetSettingService.Radius + GetOverrideHeight(tile), 0.2f) - centroid;
            }
            else if (previousHasRiver && nextHasRiver)
            {
                // 河流走势是钝角的情况，且当前方向被夹在河流出入角中间
                if (!hasRoadThroughEdge) return;
                var offset = _tileService.GetSolidEdgeMiddle(tile, idx,
                    _planetSettingService.Radius + GetOverrideHeight(tile),
                    HexMetrics.InnerToOuter);
                roadCenter += (offset - centroid) * 0.7f;
                centroid += (offset - centroid) * 0.5f;
            }
            else if (tile.IsPentagon())
            {
                // 河流走势是钝角的情况，且当前方向在河流出入角外（即更宽阔的方向：五边形有两个方向可能）
                var firstIdx = previousHasRiver ? idx : tile.PreviousIdx(idx); // 两个可能方向中的顺时针第一个
                if (!Overrider.HasRoadThroughEdge(tile, firstIdx) &&
                    !Overrider.HasRoadThroughEdge(tile, tile.NextIdx(firstIdx))) return;
                var offset = _tileService.GetSecondSolidCorner(tile, firstIdx,
                    _planetSettingService.Radius + GetOverrideHeight(tile));
                roadCenter += (offset - centroid) * 0.25f * HexMetrics.OuterToInner;
                if (idx == firstIdx && Overrider.HasRoadThroughEdge(tile, tile.Previous2Idx(firstIdx)))
                    _chunk.Features.AddBridge(tile, roadCenter,
                        centroid - (offset - centroid) * 0.7f);
            }
            else
            {
                // 河流走势是钝角的情况，且当前方向在河流出入角外（即更宽阔的方向：六边形有三个方向可能）
                int middleIdx; // 三个可能方向中，中间的那个
                if (previousHasRiver)
                    middleIdx = tile.NextIdx(idx);
                else if (nextHasRiver)
                    middleIdx = tile.PreviousIdx(idx);
                else
                    middleIdx = idx;
                if (!Overrider.HasRoadThroughEdge(tile, middleIdx)
                    && !Overrider.HasRoadThroughEdge(tile, tile.PreviousIdx(middleIdx))
                    && !Overrider.HasRoadThroughEdge(tile, tile.NextIdx(middleIdx)))
                    return;
                var offset = _tileService.GetSolidEdgeMiddle(tile, middleIdx,
                    _planetSettingService.Radius + GetOverrideHeight(tile));
                roadCenter += (offset - centroid) * 0.25f;
                if (idx == middleIdx && Overrider.HasRoadThroughEdge(tile, tile.OppositeIdx(idx)))
                    _chunk.Features.AddBridge(tile, roadCenter,
                        centroid - (offset - centroid) * (HexMetrics.InnerToOuter * 0.7f));
            }
        }

        var mL = roadCenter.Lerp(e.V1, interpolator.X);
        var mR = roadCenter.Lerp(e.V5, interpolator.Y);
        TriangulateRoad(roadCenter, mL, mR, e, hasRoadThroughEdge, tile.Id);
        if (previousHasRiver)
            TriangulateRoadEdge(roadCenter, centroid, mL, tile.Id);
        if (nextHasRiver)
            TriangulateRoadEdge(roadCenter, mR, centroid, tile.Id);
    }

    private void TriangulateWithRiverBeginOrEnd(Tile tile, Vector3 centroid, EdgeVertices e)
    {
        var m = new EdgeVertices(centroid.Lerp(e.V1, 0.5f), centroid.Lerp(e.V5, 0.5f));
        m.V3 = Math3dUtil.ProjectToSphere(m.V3, e.V3.Length());
        TriangulateEdgeStrip(m, HexMesh.Weights1, tile.Id, e, HexMesh.Weights1, tile.Id);
        TriangulateEdgeFan(centroid, m, tile.Id);

        if (!Overrider.IsUnderwater(tile))
        {
            var reversed = Overrider.HasIncomingRiver(tile);
            var ids = Vector3.One * tile.Id;
            var riverSurfaceHeight = _planetSettingService.Radius + Overrider.RiverSurfaceY(tile);
            TriangulateRiverQuad(m.V2, m.V4, e.V2, e.V4, riverSurfaceHeight, 0.6f, reversed, ids);
            centroid = Math3dUtil.ProjectToSphere(centroid, riverSurfaceHeight);
            m.V2 = Math3dUtil.ProjectToSphere(m.V2, riverSurfaceHeight);
            m.V4 = Math3dUtil.ProjectToSphere(m.V4, riverSurfaceHeight);
            _chunk.Rivers.AddTriangle([centroid, m.V2, m.V4], HexMesh.TriArr(HexMesh.Weights1),
                reversed
                    ? [new Vector2(0.5f, 0.4f), new Vector2(1f, 0.2f), new Vector2(0f, 0.2f)]
                    : [new Vector2(0.5f, 0.4f), new Vector2(0f, 0.6f), new Vector2(1f, 0.6f)],
                tis: ids);
        }
    }

    private void TriangulateWithRiver(Tile tile, int idx, Vector3 centroid, EdgeVertices e)
    {
        Vector3 centerL;
        Vector3 centerR;
        var height = GetOverrideHeight(tile);
        if (!tile.IsPentagon() && Overrider.HasRiverThroughEdge(tile, tile.OppositeIdx(idx))) // 注意五边形没有对边的情况
        {
            // 直线河流
            centerL = _tileService.GetFirstSolidCorner(tile, tile.PreviousIdx(idx),
                _planetSettingService.Radius + height, 0.25f);
            centerR = _tileService.GetSecondSolidCorner(tile, tile.NextIdx(idx), _planetSettingService.Radius + height,
                0.25f);
        }
        else if (Overrider.HasRiverThroughEdge(tile, tile.NextIdx(idx)))
        {
            // 锐角弯
            centerL = centroid;
            centerR = centroid.Lerp(e.V5, 2f / 3f);
        }
        else if (Overrider.HasRiverThroughEdge(tile, tile.PreviousIdx(idx)))
        {
            // 锐角弯
            centerL = centroid.Lerp(e.V1, 2f / 3f);
            centerR = centroid;
        }
        else if (Overrider.HasRiverThroughEdge(tile, tile.Next2Idx(idx)))
        {
            // 钝角弯
            centerL = centroid;
            centerR = _tileService.GetSolidEdgeMiddle(tile, tile.NextIdx(idx),
                _planetSettingService.Radius + height, 0.5f * HexMetrics.InnerToOuter);
        }
        else if (Overrider.HasRiverThroughEdge(tile, tile.Previous2Idx(idx)))
        {
            // 钝角弯
            centerL = _tileService.GetSolidEdgeMiddle(tile, tile.PreviousIdx(idx),
                _planetSettingService.Radius + height, 0.5f * HexMetrics.InnerToOuter);
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
        TriangulateEdgeStrip(m, HexMesh.Weights1, tile.Id, e, HexMesh.Weights1, tile.Id);
        var ids = Vector3.One * tile.Id;
        _chunk.Terrain.AddTriangle([centerL, m.V1, m.V2], HexMesh.TriArr(HexMesh.Weights1), tis: ids);
        _chunk.Terrain.AddQuad([centerL, centroid, m.V2, m.V3], HexMesh.QuadArr(HexMesh.Weights1), tis: ids);
        _chunk.Terrain.AddQuad([centroid, centerR, m.V3, m.V4], HexMesh.QuadArr(HexMesh.Weights1), tis: ids);
        _chunk.Terrain.AddTriangle([centerR, m.V4, m.V5], HexMesh.TriArr(HexMesh.Weights1), tis: ids);

        if (!Overrider.IsUnderwater(tile))
        {
            var reversed = Overrider.HasIncomingRiverThroughEdge(tile, idx);
            var riverSurfaceHeight = Overrider.RiverSurfaceY(tile);
            TriangulateRiverQuad(centerL, centerR, m.V2, m.V4,
                _planetSettingService.Radius + riverSurfaceHeight, 0.4f, reversed, ids);
            TriangulateRiverQuad(m.V2, m.V4, e.V2, e.V4,
                _planetSettingService.Radius + riverSurfaceHeight, 0.6f, reversed, ids);
        }
    }

    private void TriangulateRiverQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4,
        float height, float v, bool reversed, Vector3 ids) =>
        TriangulateRiverQuad(v1, v2, v3, v4, height, height, v, reversed, ids);

    private void TriangulateRiverQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4,
        float height1, float height2, float v, bool reversed, Vector3 ids)
    {
        v1 = Math3dUtil.ProjectToSphere(v1, height1);
        v2 = Math3dUtil.ProjectToSphere(v2, height1);
        v3 = Math3dUtil.ProjectToSphere(v3, height2);
        v4 = Math3dUtil.ProjectToSphere(v4, height2);
        _chunk.Rivers.AddQuad([v1, v2, v3, v4], HexMesh.QuadArr(HexMesh.Weights1, HexMesh.Weights2),
            reversed ? QuadUv(1f, 0f, 0.8f - v, 0.6f - v) : QuadUv(0f, 1f, v, v + 0.2f),
            tis: ids);
    }

    private void TriangulateEdgeFan(Vector3 center, EdgeVertices edge, float id)
    {
        var ids = Vector3.One * id;
        _chunk.Terrain.AddTriangle([center, edge.V1, edge.V2], HexMesh.TriArr(HexMesh.Weights1), tis: ids);
        _chunk.Terrain.AddTriangle([center, edge.V2, edge.V3], HexMesh.TriArr(HexMesh.Weights1), tis: ids);
        _chunk.Terrain.AddTriangle([center, edge.V3, edge.V4], HexMesh.TriArr(HexMesh.Weights1), tis: ids);
        _chunk.Terrain.AddTriangle([center, edge.V4, edge.V5], HexMesh.TriArr(HexMesh.Weights1), tis: ids);
    }

    private void TriangulateEdgeStrip(EdgeVertices e1, Color w1, float id1,
        EdgeVertices e2, Color w2, float id2, bool hasRoad = false)
    {
        Vector3 ids;
        ids.X = ids.Z = id1;
        ids.Y = id2;
        _chunk.Terrain.AddQuad([e1.V1, e1.V2, e2.V1, e2.V2], HexMesh.QuadArr(w1, w2), tis: ids);
        _chunk.Terrain.AddQuad([e1.V2, e1.V3, e2.V2, e2.V3], HexMesh.QuadArr(w1, w2), tis: ids);
        _chunk.Terrain.AddQuad([e1.V3, e1.V4, e2.V3, e2.V4], HexMesh.QuadArr(w1, w2), tis: ids);
        _chunk.Terrain.AddQuad([e1.V4, e1.V5, e2.V4, e2.V5], HexMesh.QuadArr(w1, w2), tis: ids);
        if (hasRoad)
            TriangulateRoadSegment(e1.V2, e1.V3, e1.V4, e2.V2, e2.V3, e2.V4, w1, w2, ids);
    }

    private Vector2 GetRoadInterpolator(Tile tile, int idx)
    {
        Vector2 interpolator;
        if (Overrider.HasRoadThroughEdge(tile, idx))
            interpolator.X = interpolator.Y = 0.5f;
        else
        {
            interpolator.X = Overrider.HasRoadThroughEdge(tile, tile.PreviousIdx(idx)) ? 0.5f : 0.25f;
            interpolator.Y = Overrider.HasRoadThroughEdge(tile, tile.NextIdx(idx)) ? 0.5f : 0.25f;
        }

        return interpolator;
    }

    private void TriangulateWithoutRiver(Tile tile, int idx, Vector3 centroid, EdgeVertices e)
    {
        TriangulateEdgeFan(centroid, e, tile.Id);
        if (Overrider.HasRoads(tile))
        {
            var interpolator = GetRoadInterpolator(tile, idx);
            TriangulateRoad(centroid, centroid.Lerp(e.V1, interpolator.X),
                centroid.Lerp(e.V5, interpolator.Y), e, Overrider.HasRoadThroughEdge(tile, idx), tile.Id);
        }
    }

    private void TriangulateRoad(Vector3 centroid, Vector3 mL, Vector3 mR,
        EdgeVertices e, bool hasRoadThroughCellEdge, float id)
    {
        if (hasRoadThroughCellEdge)
        {
            var ids = Vector3.One * id;
            var mC = mL.Lerp(mR, 0.5f);
            TriangulateRoadSegment(mL, mC, mR, e.V2, e.V3, e.V4, HexMesh.Weights1, HexMesh.Weights1, ids);
            _chunk.Roads.AddTriangle([centroid, mL, mC], HexMesh.TriArr(HexMesh.Weights1),
                [new Vector2(1f, 0f), new Vector2(0f, 0f), new Vector2(1f, 0f)], tis: ids);
            _chunk.Roads.AddTriangle([centroid, mC, mR], HexMesh.TriArr(HexMesh.Weights1),
                [new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(0f, 0f)], tis: ids);
        }
        else TriangulateRoadEdge(centroid, mL, mR, id);
    }

    private void TriangulateRoadEdge(Vector3 centroid, Vector3 mL, Vector3 mR, float id)
    {
        var ids = Vector3.One * id;
        _chunk.Roads.AddTriangle([centroid, mL, mR], HexMesh.TriArr(HexMesh.Weights1),
            [new Vector2(1f, 0f), new Vector2(0f, 0f), new Vector2(0f, 0f)], tis: ids);
    }

    // 顶点的排序：
    // v4  v5  v6
    // v1  v2  v3
    // 0.0 1.0 0.0
    private void TriangulateRoadSegment(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, Vector3 v5, Vector3 v6,
        Color w1, Color w2, Vector3 ids)
    {
        _chunk.Roads.AddQuad([v1, v2, v4, v5], HexMesh.QuadArr(w1, w2), QuadUv(0f, 1f, 0f, 0f), tis: ids);
        _chunk.Roads.AddQuad([v2, v3, v5, v6], HexMesh.QuadArr(w1, w2), QuadUv(1f, 0f, 0f, 0f), tis: ids);
    }

    private void TriangulateWaterfallInWater(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4,
        float height1, float height2, float waterHeight, Vector3 ids)
    {
        v1 = Math3dUtil.ProjectToSphere(v1, height1);
        v2 = Math3dUtil.ProjectToSphere(v2, height1);
        v3 = Math3dUtil.ProjectToSphere(v3, height2);
        v4 = Math3dUtil.ProjectToSphere(v4, height2);
        v1 = _noiseService.Perturb(v1);
        v2 = _noiseService.Perturb(v2);
        v3 = _noiseService.Perturb(v3);
        v4 = _noiseService.Perturb(v4);
        var t = (waterHeight - height2) / (height1 - height2);
        v3 = v3.Lerp(v1, t);
        v4 = v4.Lerp(v2, t);
        _chunk.Rivers.AddQuadUnperturbed([v1, v2, v3, v4], HexMesh.QuadArr(HexMesh.Weights1, HexMesh.Weights2),
            QuadUv(0f, 1f, 0.8f, 1f), tis: ids);
    }

    private void TriangulateConnection(Tile tile, int idx, EdgeVertices e)
    {
        var tileHeight = GetOverrideHeight(tile);
        var neighbor = _tileService.GetNeighborByIdx(tile, idx);
        var neighborHeight = GetOverrideHeight(neighbor);
        // 连接将由更低的地块或相同高度时 Id 更大的地块生成
        if (tileHeight > neighborHeight ||
            (Mathf.Abs(tileHeight - neighborHeight) < 0.001f && tile.Id < neighbor.Id)) return;
        var vn1 = _tileService.GetCornerByFaceId(neighbor, tile.HexFaceIds[idx],
            _planetSettingService.Radius + neighborHeight, HexMetrics.SolidFactor);
        var vn2 = _tileService.GetCornerByFaceId(neighbor, tile.HexFaceIds[tile.NextIdx(idx)],
            _planetSettingService.Radius + neighborHeight, HexMetrics.SolidFactor);
        var en = new EdgeVertices(vn1, vn2);
        var hasRiver = Overrider.HasRiverThroughEdge(tile, idx);
        var hasRoad = Overrider.HasRoadThroughEdge(tile, idx);
        if (hasRiver)
        {
            en.V3 = Math3dUtil.ProjectToSphere(en.V3, _planetSettingService.Radius + Overrider.StreamBedY(neighbor));
            var ids = new Vector3(tile.Id, neighbor.Id, tile.Id);
            if (!Overrider.IsUnderwater(tile))
            {
                if (!Overrider.IsUnderwater(neighbor))
                    TriangulateRiverQuad(e.V2, e.V4, en.V2, en.V4,
                        _planetSettingService.Radius + Overrider.RiverSurfaceY(tile),
                        _planetSettingService.Radius + Overrider.RiverSurfaceY(neighbor), 0.8f,
                        Overrider.HasIncomingRiver(tile) && Overrider.HasIncomingRiverThroughEdge(tile, idx), ids);
                else if (Overrider.Elevation(tile) > Overrider.Elevation(neighbor))
                    TriangulateWaterfallInWater(e.V2, e.V4, en.V2, en.V4,
                        _planetSettingService.Radius + Overrider.RiverSurfaceY(tile),
                        _planetSettingService.Radius + Overrider.RiverSurfaceY(neighbor),
                        _planetSettingService.Radius + Overrider.WaterSurfaceY(neighbor), ids);
            }
            else if (!Overrider.IsUnderwater(neighbor) && Overrider.Elevation(neighbor) > Overrider.Elevation(tile))
                TriangulateWaterfallInWater(en.V4, en.V2, e.V4, e.V2,
                    _planetSettingService.Radius + Overrider.RiverSurfaceY(neighbor),
                    _planetSettingService.Radius + Overrider.RiverSurfaceY(tile),
                    _planetSettingService.Radius + Overrider.WaterSurfaceY(tile), ids);
        }

        if (Overrider.GetEdgeType(tile, neighbor) == HexEdgeType.Slope)
            TriangulateEdgeTerraces(e, tile, en, neighbor, hasRoad);
        else
            TriangulateEdgeStrip(e, HexMesh.Weights1, tile.Id, en, HexMesh.Weights2, neighbor.Id, hasRoad);

        _chunk.Features.AddWall(e, tile, en, neighbor, hasRiver, hasRoad, Overrider);

        var preNeighbor = _tileService.GetNeighborByIdx(tile, tile.PreviousIdx(idx));
        var preNeighborHeight = GetOverrideHeight(preNeighbor);
        if (tileHeight < preNeighborHeight
            || (Mathf.Abs(tileHeight - preNeighborHeight) < 0.001f && tile.Id > preNeighbor.Id))
        {
            // 连接角落的三角形由周围 3 个地块中最低或者一样高时 Id 最大的生成
            var vpn = _tileService.GetCornerByFaceId(preNeighbor, tile.HexFaceIds[idx],
                _planetSettingService.Radius + preNeighborHeight, HexMetrics.SolidFactor);
            TriangulateCorner(e.V1, tile, vpn, preNeighbor, vn1, neighbor);
        }
    }

    // 需要保证入参 bottom -> left -> right 是顺时针
    private void TriangulateCorner(Vector3 bottom, Tile bottomTile,
        Vector3 left, Tile leftTile, Vector3 right, Tile rightTile)
    {
        var edgeType1 = Overrider.GetEdgeType(bottomTile, leftTile);
        var edgeType2 = Overrider.GetEdgeType(bottomTile, rightTile);
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
        else if (Overrider.GetEdgeType(leftTile, rightTile) == HexEdgeType.Slope)
        {
            if (Overrider.Elevation(leftTile) < Overrider.Elevation(rightTile))
                TriangulateCornerCliffTerraces(right, rightTile, bottom, bottomTile, left, leftTile);
            else
                TriangulateCornerTerracesCliff(left, leftTile, right, rightTile, bottom, bottomTile);
        }
        else
            _chunk.Terrain.AddTriangle([bottom, left, right],
                [HexMesh.Weights1, HexMesh.Weights2, HexMesh.Weights3],
                tis: new Vector3(bottomTile.Id, leftTile.Id, rightTile.Id));

        _chunk.Features.AddWall(bottom, bottomTile, left, leftTile, right, rightTile, Overrider);
    }

    // 三角形靠近 tile 的左边是阶地，右边是悬崖，另一边任意的情况
    private void TriangulateCornerTerracesCliff(Vector3 begin, Tile beginTile,
        Vector3 left, Tile leftTile, Vector3 right, Tile rightTile)
    {
        var b = 1f / Mathf.Abs(Overrider.Elevation(rightTile) - Overrider.Elevation(beginTile));
        var boundary = _noiseService.Perturb(begin).Lerp(_noiseService.Perturb(right), b);
        var boundaryWeights = HexMesh.Weights1.Lerp(HexMesh.Weights3, b);
        var ids = new Vector3(beginTile.Id, leftTile.Id, rightTile.Id);
        TriangulateBoundaryTriangle(begin, HexMesh.Weights1, left, HexMesh.Weights2, boundary, boundaryWeights, ids);
        if (Overrider.GetEdgeType(leftTile, rightTile) == HexEdgeType.Slope)
            TriangulateBoundaryTriangle(left, HexMesh.Weights2, right, HexMesh.Weights3,
                boundary, boundaryWeights, ids);
        else
            _chunk.Terrain.AddTriangleUnperturbed([_noiseService.Perturb(left), _noiseService.Perturb(right), boundary],
                [HexMesh.Weights2, HexMesh.Weights3, boundaryWeights], tis: ids);
    }

    // 三角形靠近 tile 的左边是悬崖，右边是阶地，另一边任意的情况
    private void TriangulateCornerCliffTerraces(Vector3 begin, Tile beginTile,
        Vector3 left, Tile leftTile, Vector3 right, Tile rightTile)
    {
        var b = 1f / Mathf.Abs(Overrider.Elevation(leftTile) - Overrider.Elevation(beginTile));
        var boundary = _noiseService.Perturb(begin).Lerp(_noiseService.Perturb(left), b);
        var boundaryWeights = HexMesh.Weights1.Lerp(HexMesh.Weights2, b);
        var ids = new Vector3(beginTile.Id, leftTile.Id, rightTile.Id);
        TriangulateBoundaryTriangle(right, HexMesh.Weights3, begin, HexMesh.Weights1, boundary, boundaryWeights, ids);
        if (Overrider.GetEdgeType(leftTile, rightTile) == HexEdgeType.Slope)
            TriangulateBoundaryTriangle(left, HexMesh.Weights2, right, HexMesh.Weights3,
                boundary, boundaryWeights, ids);
        else
            _chunk.Terrain.AddTriangleUnperturbed([_noiseService.Perturb(left), _noiseService.Perturb(right), boundary],
                [HexMesh.Weights2, HexMesh.Weights3, boundaryWeights], tis: ids);
    }

    // 阶地和悬崖中间的半三角形
    private void TriangulateBoundaryTriangle(Vector3 begin, Color beginWeight,
        Vector3 left, Color leftWeight, Vector3 boundary, Color boundaryWeight, Vector3 ids)
    {
        var v2 = _noiseService.Perturb(HexMetrics.TerraceLerp(begin, left, 1));
        var w2 = HexMetrics.TerraceLerp(beginWeight, leftWeight, 1);
        _chunk.Terrain.AddTriangleUnperturbed([_noiseService.Perturb(begin), v2, boundary],
            [beginWeight, w2, boundaryWeight], tis: ids);
        for (var i = 2; i < HexMetrics.TerraceSteps; i++)
        {
            var v1 = v2;
            var w1 = w2;
            v2 = _noiseService.Perturb(HexMetrics.TerraceLerp(begin, left, i));
            w2 = HexMetrics.TerraceLerp(beginWeight, leftWeight, i);
            _chunk.Terrain.AddTriangleUnperturbed([v1, v2, boundary], [w1, w2, boundaryWeight], tis: ids);
        }

        _chunk.Terrain.AddTriangleUnperturbed([v2, _noiseService.Perturb(left), boundary],
            [w2, leftWeight, boundaryWeight], tis: ids);
    }

    // 处理高度不同的 beginTile 和两个高度相同的 endTile（即三角形两边是等高阶地，一边是平地）的情况
    private void TriangulateCornerTerraces(Vector3 begin, Tile beginTile,
        Vector3 left, Tile leftTile, Vector3 right, Tile rightTile)
    {
        var v3 = HexMetrics.TerraceLerp(begin, left, 1);
        var v4 = HexMetrics.TerraceLerp(begin, right, 1);
        var w3 = HexMetrics.TerraceLerp(HexMesh.Weights1, HexMesh.Weights2, 1);
        var w4 = HexMetrics.TerraceLerp(HexMesh.Weights1, HexMesh.Weights3, 1);
        var ids = new Vector3(beginTile.Id, leftTile.Id, rightTile.Id);
        _chunk.Terrain.AddTriangle([begin, v3, v4], [HexMesh.Weights1, w3, w4], tis: ids);
        for (var i = 0; i < HexMetrics.TerraceSteps; i++)
        {
            var v1 = v3;
            var v2 = v4;
            var w1 = w3;
            var w2 = w4;
            v3 = HexMetrics.TerraceLerp(begin, left, i);
            v4 = HexMetrics.TerraceLerp(begin, right, i);
            w3 = HexMetrics.TerraceLerp(HexMesh.Weights1, HexMesh.Weights2, i);
            w4 = HexMetrics.TerraceLerp(HexMesh.Weights1, HexMesh.Weights3, i);
            _chunk.Terrain.AddQuad([v1, v2, v3, v4], [w1, w2, w3, w4], tis: ids);
        }

        _chunk.Terrain.AddQuad([v3, v4, left, right], [w3, w4, HexMesh.Weights2, HexMesh.Weights3], tis: ids);
    }

    private void TriangulateEdgeTerraces(EdgeVertices begin, Tile beginTile, EdgeVertices end, Tile endTile,
        bool hasRoad)
    {
        var e2 = EdgeVertices.TerraceLerp(begin, end, 1);
        var w2 = HexMetrics.TerraceLerp(HexMesh.Weights1, HexMesh.Weights2, 1);
        var i1 = beginTile.Id;
        var i2 = endTile.Id;
        TriangulateEdgeStrip(begin, HexMesh.Weights1, i1, e2, w2, i2, hasRoad);
        for (var i = 2; i < HexMetrics.TerraceSteps; i++)
        {
            var e1 = e2;
            var w1 = w2;
            e2 = EdgeVertices.TerraceLerp(begin, end, i);
            w2 = HexMetrics.TerraceLerp(HexMesh.Weights1, HexMesh.Weights2, i);
            TriangulateEdgeStrip(e1, w1, i1, e2, w2, i2, hasRoad);
        }

        TriangulateEdgeStrip(e2, w2, i1, end, HexMesh.Weights2, i2, hasRoad);
    }

    private static Vector2[] QuadUv(float uMin, float uMax, float vMin, float vMax) =>
    [
        new Vector2(uMin, vMin), new Vector2(uMax, vMin),
        new Vector2(uMin, vMax), new Vector2(uMax, vMax)
    ];
}