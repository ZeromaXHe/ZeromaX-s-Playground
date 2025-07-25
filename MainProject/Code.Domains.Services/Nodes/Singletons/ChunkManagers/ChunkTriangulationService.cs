using Commons.Constants;
using Commons.Enums;
using Commons.Utils;
using Domains.Models.Entities.PlanetGenerates;
using Domains.Models.ValueObjects.PlanetGenerates;
using Domains.Services.Abstractions.Nodes.Singletons.ChunkManagers;
using Godot;
using Infras.Readers.Abstractions.Nodes.Singletons;
using Infras.Writers.Abstractions.PlanetGenerates;
using Nodes.Abstractions.ChunkManagers;

namespace Domains.Services.Nodes.Singletons.ChunkManagers;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-22 09:31:25
public class ChunkTriangulationService(
    ITileRepo tileRepo,
    IChunkRepo chunkRepo,
    IFaceRepo faceRepo,
    IHexPlanetManagerRepo hexPlanetManagerRepo) : IChunkTriangulationService
{
    private IChunk _chunk;
    private ChunkLod Lod => _chunk.Lod;
    private HexTileDataOverrider Overrider => _chunk.TileDataOverrider;

    private float GetOverrideHeight(Tile tile) =>
        Overrider.IsOverrideTile(tile)
            ? hexPlanetManagerRepo.GetOverrideHeight(tile, Overrider)
            : hexPlanetManagerRepo.GetHeight(tile);

    public void Triangulate(Tile tile, IChunk chunk)
    {
        _chunk = chunk;
        if (Lod == ChunkLod.JustHex)
        {
            TriangulateJustHex(tile);
            return;
        }

        if (Lod == ChunkLod.PlaneHex)
        {
            TriangulatePlaneHex(tile);
            return;
        }

        for (var i = 0; i < tile.HexFaceIds.Count; i++)
            Triangulate(tile, i);
        if (!Overrider.IsUnderwater(tile))
        {
            if (!Overrider.HasRiver(tile) && !Overrider.HasRoads(tile))
                _chunk.GetFeatures()!.AddFeature(tile,
                    tile.GetCentroid(hexPlanetManagerRepo.Radius + GetOverrideHeight(tile)), Overrider);
            if (Overrider.IsSpecial(tile))
                _chunk.GetFeatures()!.AddSpecialFeature(tile,
                    tile.GetCentroid(hexPlanetManagerRepo.Radius + GetOverrideHeight(tile)), Overrider);
        }
    }

    // 仅绘制六边形（无扰动，点平均周围地块高度）
    private void TriangulateJustHex(Tile tile)
    {
        var ids = Vector3.One * tile.Id;
        var height = GetOverrideHeight(tile);
        var waterHeight = Overrider.WaterSurfaceY(tile, hexPlanetManagerRepo.UnitHeight);
        var preNeighbor = tileRepo.GetNeighborByIdx(tile, tile.PreviousIdx(0))!;
        var neighbor = tileRepo.GetNeighborByIdx(tile, 0)!;
        var nextNeighbor = tileRepo.GetNeighborByIdx(tile, tile.NextIdx(0))!;
        var v0 = Vector3.Zero;
        var vw0 = Vector3.Zero;
        for (var i = 0; i < tile.HexFaceIds.Count; i++)
        {
            var neighborHeight = GetOverrideHeight(neighbor);
            var neighborWaterHeight = Overrider.WaterSurfaceY(neighbor, hexPlanetManagerRepo.UnitHeight);
            var preHeight = GetOverrideHeight(preNeighbor);
            var preWaterHeight = Overrider.WaterSurfaceY(preNeighbor, hexPlanetManagerRepo.UnitHeight);
            var nextHeight = GetOverrideHeight(nextNeighbor);
            var nextWaterHeight = Overrider.WaterSurfaceY(nextNeighbor, hexPlanetManagerRepo.UnitHeight);
            var avgHeight1 = (preHeight + neighborHeight + height) / 3f;
            var avgHeight2 = (neighborHeight + nextHeight + height) / 3f;
            var avgWaterHeight1 = (preWaterHeight + neighborWaterHeight + waterHeight) / 3f;
            var avgWaterHeight2 = (neighborWaterHeight + nextWaterHeight + waterHeight) / 3f;

            var v1 = tile.GetFirstCorner(i, hexPlanetManagerRepo.Radius + avgHeight1);
            if (i == 0) v0 = v1;
            var v2 = tile.GetSecondCorner(i, hexPlanetManagerRepo.Radius + avgHeight2);
            var vw1 = tile.GetFirstCorner(i, hexPlanetManagerRepo.Radius + avgWaterHeight1);
            if (i == 0) vw0 = vw1;
            var vw2 = tile.GetSecondCorner(i, hexPlanetManagerRepo.Radius + avgWaterHeight2);

            if (i > 0 && i < tile.HexFaceIds.Count - 1)
            {
                // 绘制地面
                _chunk.GetTerrain()!.AddTriangleUnperturbed([v0, v1, v2],
                    HexMeshConstant.QuadArr(HexMeshConstant.Weights1, HexMeshConstant.Weights2), tis: ids);
                // 绘制水面
                if (Overrider.IsUnderwater(tile))
                    _chunk.GetWater()!.AddTriangleUnperturbed([vw0, vw1, vw2],
                        HexMeshConstant.TriArr(HexMeshConstant.Weights1),
                        tis: ids);
            }

            preNeighbor = neighbor;
            neighbor = nextNeighbor;
            nextNeighbor = tileRepo.GetNeighborByIdx(tile, tile.Next2Idx(i));
        }
    }

    // 绘制平面六边形（有高度立面、处理接缝、但无特征、无河流）
    private void TriangulatePlaneHex(Tile tile)
    {
        var ids = Vector3.One * tile.Id;
        var height = GetOverrideHeight(tile);
        var waterSurfaceHeight = Overrider.WaterSurfaceY(tile, hexPlanetManagerRepo.UnitHeight);
        var v0 = tile.GetFirstCorner(0, hexPlanetManagerRepo.Radius + height);
        var v1 = v0;
        var vw0 = tile.GetFirstCorner(0, hexPlanetManagerRepo.Radius + waterSurfaceHeight);
        var vw1 = vw0;
        for (var i = 0; i < tile.HexFaceIds.Count; i++)
        {
            var v2 = tile.GetSecondCorner(i, hexPlanetManagerRepo.Radius + height);
            var vw2 = tile.GetSecondCorner(i, hexPlanetManagerRepo.Radius + waterSurfaceHeight);
            var neighbor = tileRepo.GetNeighborByIdx(tile, i)!;
            var nIds = new Vector3(tile.Id, neighbor.Id, tile.Id);
            var neighborHeight = GetOverrideHeight(neighbor);
            var neighborWaterSurfaceHeight = Overrider.WaterSurfaceY(neighbor, hexPlanetManagerRepo.UnitHeight);
            // 绘制陆地立面（由高的地块绘制）
            if (neighborHeight < height)
            {
                var vn1 = Math3dUtil.ProjectToSphere(v1, hexPlanetManagerRepo.Radius + neighborHeight);
                var vn2 = Math3dUtil.ProjectToSphere(v2, hexPlanetManagerRepo.Radius + neighborHeight);
                _chunk.GetTerrain()!.AddQuad([v1, v2, vn1, vn2],
                    HexMeshConstant.QuadArr(HexMeshConstant.Weights1, HexMeshConstant.Weights2), tis: nIds);
            }

            // 绘制水面立面（由高的水面绘制）
            if (Overrider.IsUnderwater(tile) && neighborWaterSurfaceHeight < waterSurfaceHeight)
            {
                var vnw1 = Math3dUtil.ProjectToSphere(vw1,
                    hexPlanetManagerRepo.Radius + neighborWaterSurfaceHeight);
                var vnw2 = Math3dUtil.ProjectToSphere(vw2,
                    hexPlanetManagerRepo.Radius + neighborWaterSurfaceHeight);
                _chunk.GetWater()!.AddQuad([vw1, vw2, vnw1, vnw2],
                    HexMeshConstant.QuadArr(HexMeshConstant.Weights1, HexMeshConstant.Weights2), tis: nIds);
            }

            // 处理接缝（目前很粗暴的对所有相邻地块的分块的 LOD 是 SimpleHex 以上时向外绘制到 Solid 边界）
            if (neighbor.ChunkId != tile.ChunkId
                && chunkRepo.GetById(neighbor.ChunkId)!.Lod >= ChunkLod.SimpleHex)
            {
                var vn1 = faceRepo.GetCornerByFaceId(neighbor, tile.HexFaceIds[i],
                    hexPlanetManagerRepo.Radius + neighborHeight, HexMetrics.SolidFactor);
                var vn2 = faceRepo.GetCornerByFaceId(neighbor, tile.HexFaceIds[tile.NextIdx(i)],
                    hexPlanetManagerRepo.Radius + neighborHeight, HexMetrics.SolidFactor);
                _chunk.GetTerrain()!.AddQuad([v1, v2, vn1, vn2],
                    HexMeshConstant.QuadArr(HexMeshConstant.Weights1, HexMeshConstant.Weights2), tis: nIds);
                if (Overrider.IsUnderwater(tile))
                {
                    var vnw1 = faceRepo.GetCornerByFaceId(neighbor, tile.HexFaceIds[i],
                        hexPlanetManagerRepo.Radius + neighborWaterSurfaceHeight, HexMetrics.WaterFactor);
                    var vnw2 = faceRepo.GetCornerByFaceId(neighbor, tile.HexFaceIds[tile.NextIdx(i)],
                        hexPlanetManagerRepo.Radius + neighborWaterSurfaceHeight, HexMetrics.WaterFactor);
                    _chunk.GetWater()!.AddQuad([vw1, vw2, vnw1, vnw2],
                        HexMeshConstant.QuadArr(HexMeshConstant.Weights1, HexMeshConstant.Weights2), tis: nIds);
                }
            }

            if (i > 0 && i < tile.HexFaceIds.Count - 1)
            {
                // 绘制地面
                _chunk.GetTerrain()!.AddTriangle([v0, v1, v2],
                    HexMeshConstant.QuadArr(HexMeshConstant.Weights1, HexMeshConstant.Weights2), tis: ids);
                // 绘制水面
                if (Overrider.IsUnderwater(tile))
                    _chunk.GetWater()!.AddTriangle([vw0, vw1, vw2], HexMeshConstant.TriArr(HexMeshConstant.Weights1),
                        tis: ids);
            }

            v1 = v2;
            vw1 = vw2;
        }
    }

    // Godot 缠绕顺序是正面顺时针，所以从 i1 对应角落到 i2 对应角落相对于 tile 重心需要是顺时针
    private void Triangulate(Tile tile, int idx)
    {
        var height = GetOverrideHeight(tile);
        var v1 = tile.GetFirstSolidCorner(idx, hexPlanetManagerRepo.Radius + height);
        var v2 = tile.GetSecondSolidCorner(idx, hexPlanetManagerRepo.Radius + height);
        var e = new EdgeVertices(v1, v2);
        var centroid = tile.GetCentroid(hexPlanetManagerRepo.Radius + height);
        var simple = IsSimple();
        if (Overrider.HasRiver(tile))
        {
            if (Overrider.HasRiverThroughEdge(tile, idx))
            {
                e.V3 = Math3dUtil.ProjectToSphere(e.V3,
                    hexPlanetManagerRepo.Radius + Overrider.StreamBedY(tile, hexPlanetManagerRepo.UnitHeight));
                if (Overrider.HasRiverBeginOrEnd(tile))
                    TriangulateWithRiverBeginOrEnd(tile, centroid, e);
                else TriangulateWithRiver(tile, idx, centroid, e);
            }
            else TriangulateAdjacentToRiver(tile, idx, centroid, e, simple);
        }
        else
        {
            TriangulateWithoutRiver(tile, idx, centroid, e, simple);
            if (!Overrider.IsUnderwater(tile) && !Overrider.HasRoadThroughEdge(tile, idx))
                _chunk.GetFeatures()!.AddFeature(tile, (centroid + e.V1 + e.V5) / 3f, Overrider);
        }

        TriangulateConnection(tile, idx, e, simple);
        if (Overrider.IsUnderwater(tile))
            TriangulateWater(tile, idx, centroid, simple);
        return;

        bool IsSimple()
        {
            if (Lod == ChunkLod.Full)
                return false;
            var neighbor = tileRepo.GetNeighborByIdx(tile, idx)!;
            if (neighbor.ChunkId == tile.ChunkId)
                return true;
            var neighborChunk = chunkRepo.GetById(neighbor.ChunkId)!;
            return neighborChunk.Lod < ChunkLod.Full;
        }
    }

    private void TriangulateWater(Tile tile, int idx, Vector3 centroid, bool simple)
    {
        var waterSurfaceHeight = Overrider.WaterSurfaceY(tile, hexPlanetManagerRepo.UnitHeight);
        centroid = Math3dUtil.ProjectToSphere(centroid, hexPlanetManagerRepo.Radius + waterSurfaceHeight);
        var neighbor = tileRepo.GetNeighborByIdx(tile, idx)!;
        if (!Overrider.IsUnderwater(neighbor))
            TriangulateWaterShore(tile, idx, waterSurfaceHeight, neighbor, centroid, simple);
        else
            TriangulateOpenWater(tile, idx, waterSurfaceHeight, neighbor, centroid);
    }

    private void TriangulateWaterShore(Tile tile, int idx, float waterSurfaceHeight,
        Tile neighbor, Vector3 centroid, bool simple)
    {
        var e1 = new EdgeVertices(
            tile.GetFirstWaterCorner(idx, hexPlanetManagerRepo.Radius + waterSurfaceHeight),
            tile.GetSecondWaterCorner(idx, hexPlanetManagerRepo.Radius + waterSurfaceHeight));
        var ids = new Vector3(tile.Id, neighbor.Id, tile.Id);
        var water = _chunk.GetWater()!;
        if (simple)
            water.AddTriangle([centroid, e1.V1, e1.V5], HexMeshConstant.TriArr(HexMeshConstant.Weights1),
                tis: ids);
        else
        {
            water.AddTriangle([centroid, e1.V1, e1.V2], HexMeshConstant.TriArr(HexMeshConstant.Weights1),
                tis: ids);
            water.AddTriangle([centroid, e1.V2, e1.V3], HexMeshConstant.TriArr(HexMeshConstant.Weights1),
                tis: ids);
            water.AddTriangle([centroid, e1.V3, e1.V4], HexMeshConstant.TriArr(HexMeshConstant.Weights1),
                tis: ids);
            water.AddTriangle([centroid, e1.V4, e1.V5], HexMeshConstant.TriArr(HexMeshConstant.Weights1),
                tis: ids);
        }

        // 使用邻居的水表面高度的话，就是希望考虑岸边地块的实际水位。(不然强行拉平岸边的话，角落两个水面不一样高时太多复杂逻辑，bug 太多)
        var neighborWaterSurfaceHeight = Overrider.WaterSurfaceY(neighbor, hexPlanetManagerRepo.UnitHeight);
        var cn1 = faceRepo.GetCornerByFaceId(neighbor, tile.HexFaceIds[idx],
            hexPlanetManagerRepo.Radius + neighborWaterSurfaceHeight, HexMetrics.SolidFactor);
        var cn2 = faceRepo.GetCornerByFaceId(neighbor, tile.HexFaceIds[tile.NextIdx(idx)],
            hexPlanetManagerRepo.Radius + neighborWaterSurfaceHeight, HexMetrics.SolidFactor);
        var e2 = new EdgeVertices(cn1, cn2);
        var neighborIdx = tile.GetNeighborIdx(neighbor);
        var waterShore = _chunk.GetWaterShore()!;
        if (simple)
            waterShore.AddQuad([e1.V1, e1.V5, e2.V1, e2.V5],
                HexMeshConstant.QuadArr(HexMeshConstant.Weights1, HexMeshConstant.Weights2),
                QuadUv(0f, 0f, 0f, 1f), tis: ids);
        else if (Overrider.HasRiverThroughEdge(tile, neighborIdx))
            TriangulateEstuary(e1, e2, Overrider.HasIncomingRiverThroughEdge(tile, neighborIdx), ids);
        else
        {
            waterShore.AddQuad([e1.V1, e1.V2, e2.V1, e2.V2],
                HexMeshConstant.QuadArr(HexMeshConstant.Weights1, HexMeshConstant.Weights2),
                QuadUv(0f, 0f, 0f, 1f), tis: ids);
            waterShore.AddQuad([e1.V2, e1.V3, e2.V2, e2.V3],
                HexMeshConstant.QuadArr(HexMeshConstant.Weights1, HexMeshConstant.Weights2),
                QuadUv(0f, 0f, 0f, 1f), tis: ids);
            waterShore.AddQuad([e1.V3, e1.V4, e2.V3, e2.V4],
                HexMeshConstant.QuadArr(HexMeshConstant.Weights1, HexMeshConstant.Weights2),
                QuadUv(0f, 0f, 0f, 1f), tis: ids);
            waterShore.AddQuad([e1.V4, e1.V5, e2.V4, e2.V5],
                HexMeshConstant.QuadArr(HexMeshConstant.Weights1, HexMeshConstant.Weights2),
                QuadUv(0f, 0f, 0f, 1f), tis: ids);
        }

        var nextNeighbor = tileRepo.GetNeighborByIdx(tile, tile.NextIdx(idx))!;
        var nextNeighborWaterSurfaceHeight = Overrider.WaterSurfaceY(nextNeighbor, hexPlanetManagerRepo.UnitHeight);
        var cnn = faceRepo.GetCornerByFaceId(nextNeighbor, tile.HexFaceIds[tile.NextIdx(idx)],
            hexPlanetManagerRepo.Radius + nextNeighborWaterSurfaceHeight,
            Overrider.IsUnderwater(nextNeighbor) ? HexMetrics.WaterFactor : HexMetrics.SolidFactor);
        ids.Z = nextNeighbor.Id;
        _chunk.GetWaterShore()!.AddTriangle([e1.V5, e2.V5, cnn],
            [HexMeshConstant.Weights1, HexMeshConstant.Weights2, HexMeshConstant.Weights3],
            [new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(0f, Overrider.IsUnderwater(nextNeighbor) ? 0f : 1f)],
            tis: ids);
    }

    private void TriangulateEstuary(EdgeVertices e1, EdgeVertices e2, bool incomingRiver, Vector3 ids)
    {
        var waterShore = _chunk.GetWaterShore()!;
        var estuary = _chunk.GetEstuary()!;
        waterShore.AddTriangle([e2.V1, e1.V2, e1.V1],
            [HexMeshConstant.Weights2, HexMeshConstant.Weights1, HexMeshConstant.Weights1],
            [new Vector2(0f, 1f), new Vector2(0f, 0f), new Vector2(0f, 0f)], tis: ids);
        waterShore.AddTriangle([e2.V5, e1.V5, e1.V4],
            [HexMeshConstant.Weights2, HexMeshConstant.Weights1, HexMeshConstant.Weights1],
            [new Vector2(0f, 1f), new Vector2(0f, 0f), new Vector2(0f, 0f)], tis: ids);
        estuary.AddQuad([e2.V1, e1.V2, e2.V2, e1.V3],
            [HexMeshConstant.Weights2, HexMeshConstant.Weights1, HexMeshConstant.Weights2, HexMeshConstant.Weights1],
            [new Vector2(0f, 1f), new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(0f, 0f)],
            incomingRiver
                ? [new Vector2(1.5f, 1f), new Vector2(0.7f, 1.15f), new Vector2(1f, 0.8f), new Vector2(0.5f, 1.1f)]
                :
                [
                    new Vector2(-0.5f, -0.2f), new Vector2(0.3f, -0.35f), new Vector2(0f, 0f), new Vector2(0.5f, -0.3f)
                ],
            ids);
        estuary.AddTriangle([e1.V3, e2.V2, e2.V4],
            [HexMeshConstant.Weights1, HexMeshConstant.Weights2, HexMeshConstant.Weights2],
            [new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(1f, 1f)],
            incomingRiver
                ? [new Vector2(0.5f, 1.1f), new Vector2(1f, 0.8f), new Vector2(0f, 0.8f)]
                : [new Vector2(0.5f, -0.3f), new Vector2(0f, 0f), new Vector2(1f, 0f)],
            ids);
        estuary.AddQuad([e1.V3, e1.V4, e2.V4, e2.V5],
            HexMeshConstant.QuadArr(HexMeshConstant.Weights1, HexMeshConstant.Weights2),
            [new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(0f, 1f)],
            incomingRiver
                ? [new Vector2(0.5f, 1.1f), new Vector2(0.3f, 1.15f), new Vector2(0f, 0.8f), new Vector2(-0.5f, 1f)]
                : [new Vector2(0.5f, -0.3f), new Vector2(0.7f, -0.35f), new Vector2(1f, 0f), new Vector2(1.5f, -0.2f)],
            ids);
    }

    private void TriangulateOpenWater(Tile tile, int idx, float waterSurfaceHeight, Tile neighbor, Vector3 centroid)
    {
        var c1 = tile.GetFirstWaterCorner(idx, hexPlanetManagerRepo.Radius + waterSurfaceHeight);
        var c2 = tile.GetSecondWaterCorner(idx, hexPlanetManagerRepo.Radius + waterSurfaceHeight);
        var ids = Vector3.One * tile.Id;
        var water = _chunk.GetWater()!;
        water.AddTriangle([centroid, c1, c2], HexMeshConstant.TriArr(HexMeshConstant.Weights1), tis: ids);
        // 由更大 Id 的地块绘制水域连接，或者是由编辑地块绘制和不编辑的邻接地块间的连接
        if (tile.Id <= neighbor.Id && !Overrider.IsOverridingTileConnection(tile, neighbor))
            return;
        var neighborWaterSurfaceHeight = Overrider.WaterSurfaceY(neighbor, hexPlanetManagerRepo.UnitHeight);
        var cn1 = faceRepo.GetCornerByFaceId(neighbor, tile.HexFaceIds[idx],
            hexPlanetManagerRepo.Radius + neighborWaterSurfaceHeight, HexMetrics.WaterFactor);
        var cn2 = faceRepo.GetCornerByFaceId(neighbor, tile.HexFaceIds[tile.NextIdx(idx)],
            hexPlanetManagerRepo.Radius + neighborWaterSurfaceHeight, HexMetrics.WaterFactor);
        ids.Y = neighbor.Id;
        water.AddQuad([c1, c2, cn1, cn2],
            HexMeshConstant.QuadArr(HexMeshConstant.Weights1, HexMeshConstant.Weights2),
            tis: ids);

        var nextNeighbor = tileRepo.GetNeighborByIdx(tile, tile.NextIdx(idx))!;
        // 由最大 Id 的地块绘制水域角落三角形，或者是由编辑地块绘制和不编辑的两个邻接地块间的连接
        if (tile.Id <= nextNeighbor.Id && !Overrider.IsOverridingTileConnection(tile, nextNeighbor))
            return;
        if (!Overrider.IsUnderwater(nextNeighbor))
            return;
        var cnn = faceRepo.GetCornerByFaceId(nextNeighbor, tile.HexFaceIds[tile.NextIdx(idx)],
            hexPlanetManagerRepo.Radius + Overrider.WaterSurfaceY(nextNeighbor, hexPlanetManagerRepo.UnitHeight),
            HexMetrics.WaterFactor);
        ids.Z = nextNeighbor.Id;
        water.AddTriangle([c2, cn2, cnn],
            [HexMeshConstant.Weights1, HexMeshConstant.Weights2, HexMeshConstant.Weights3], tis: ids);
    }

    private void TriangulateAdjacentToRiver(Tile tile, int idx, Vector3 centroid, EdgeVertices e, bool simple)
    {
        if (Overrider.HasRoads(tile))
            TriangulateRoadAdjacentToRiver(tile, idx, centroid, e);
        if (Overrider.HasRiverThroughEdge(tile, tile.NextIdx(idx)))
        {
            if (Overrider.HasRiverThroughEdge(tile, tile.PreviousIdx(idx)))
                centroid = tile.GetSolidEdgeMiddle(idx,
                    hexPlanetManagerRepo.Radius + GetOverrideHeight(tile),
                    0.5f * HexMetrics.InnerToOuter);
            else if (!tile.IsPentagon() && Overrider.HasRiverThroughEdge(tile, tile.Previous2Idx(idx)))
                // 注意五边形没有直线河流，一边临河另一边隔一个方向临河的情况是对应钝角河的外河岸，依然在 centroid
                centroid = tile.GetFirstSolidCorner(idx,
                    hexPlanetManagerRepo.Radius + GetOverrideHeight(tile),
                    0.25f);
        }
        else if (!tile.IsPentagon()
                 && Overrider.HasRiverThroughEdge(tile, tile.PreviousIdx(idx))
                 && Overrider.HasRiverThroughEdge(tile, tile.Next2Idx(idx)))
            // 注意五边形没有直线河流，一边临河另一边隔一个方向临河的情况是对应钝角河的外河岸，依然在 centroid
            centroid = tile.GetSecondSolidCorner(idx,
                hexPlanetManagerRepo.Radius + GetOverrideHeight(tile),
                0.25f);

        var m = new EdgeVertices(centroid.Lerp(e.V1, 0.5f), centroid.Lerp(e.V5, 0.5f));
        TriangulateEdgeStrip(m, HexMeshConstant.Weights1, tile.Id, e, HexMeshConstant.Weights1, tile.Id,
            simple: simple);
        TriangulateEdgeFan(centroid, m, tile.Id, simple);

        if (!Overrider.IsUnderwater(tile) && !Overrider.HasRoadThroughEdge(tile, idx))
            _chunk.GetFeatures()!.AddFeature(tile, (centroid + e.V1 + e.V5) / 3f, Overrider);
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
                roadCenter += tile.GetFirstSolidCorner(tile.OppositeIdx(riverBeginOrEndIdx),
                    hexPlanetManagerRepo.Radius + GetOverrideHeight(tile), HexMetrics.OuterToInner / 3f) - centroid;
            else
                roadCenter += tile.GetSolidEdgeMiddle(tile.OppositeIdx(riverBeginOrEndIdx),
                    hexPlanetManagerRepo.Radius + GetOverrideHeight(tile), 1f / 3f) - centroid;
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
                    corner = tile.GetSecondSolidCorner(idx,
                        hexPlanetManagerRepo.Radius + GetOverrideHeight(tile));
                }
                else
                {
                    if (!hasRoadThroughEdge && !Overrider.HasRoadThroughEdge(tile, tile.PreviousIdx(idx))) return;
                    corner = tile.GetFirstSolidCorner(idx,
                        hexPlanetManagerRepo.Radius + GetOverrideHeight(tile));
                }

                roadCenter += (corner - centroid) * 0.5f;
                if (incomingRiverIdx == tile.NextIdx(idx)
                    && (Overrider.HasRoadThroughEdge(tile, tile.Next2Idx(idx))
                        || Overrider.HasRoadThroughEdge(tile, tile.OppositeIdx(idx))))
                    _chunk.GetFeatures()!.AddBridge(tile, roadCenter, centroid - (corner - centroid) * 0.5f);
                centroid += (corner - centroid) * 0.25f;
            }
            else if (incomingRiverIdx == tile.PreviousIdx(outgoingRiverIdx))
            {
                // 河流走势是逆时针锐角的情况
                roadCenter -= tile.GetSecondCorner(incomingRiverIdx,
                    hexPlanetManagerRepo.Radius + GetOverrideHeight(tile), 0.2f) - centroid;
            }
            else if (incomingRiverIdx == tile.NextIdx(outgoingRiverIdx))
            {
                // 河流走势是顺时针锐角的情况
                roadCenter -= tile.GetFirstCorner(incomingRiverIdx,
                    hexPlanetManagerRepo.Radius + GetOverrideHeight(tile), 0.2f) - centroid;
            }
            else if (previousHasRiver && nextHasRiver)
            {
                // 河流走势是钝角的情况，且当前方向被夹在河流出入角中间
                if (!hasRoadThroughEdge) return;
                var offset = tile.GetSolidEdgeMiddle(idx,
                    hexPlanetManagerRepo.Radius + GetOverrideHeight(tile),
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
                var offset = tile.GetSecondSolidCorner(firstIdx,
                    hexPlanetManagerRepo.Radius + GetOverrideHeight(tile));
                roadCenter += (offset - centroid) * 0.25f * HexMetrics.OuterToInner;
                if (idx == firstIdx && Overrider.HasRoadThroughEdge(tile, tile.Previous2Idx(firstIdx)))
                    _chunk.GetFeatures()!.AddBridge(tile, roadCenter,
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
                var offset = tile.GetSolidEdgeMiddle(middleIdx,
                    hexPlanetManagerRepo.Radius + GetOverrideHeight(tile));
                roadCenter += (offset - centroid) * 0.25f;
                if (idx == middleIdx && Overrider.HasRoadThroughEdge(tile, tile.OppositeIdx(idx)))
                    _chunk.GetFeatures()!.AddBridge(tile, roadCenter,
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
        TriangulateEdgeStrip(m, HexMeshConstant.Weights1, tile.Id, e, HexMeshConstant.Weights1, tile.Id);
        TriangulateEdgeFan(centroid, m, tile.Id);

        if (!Overrider.IsUnderwater(tile))
        {
            var reversed = Overrider.HasIncomingRiver(tile);
            var ids = Vector3.One * tile.Id;
            var riverSurfaceHeight = hexPlanetManagerRepo.Radius +
                                     Overrider.RiverSurfaceY(tile, hexPlanetManagerRepo.UnitHeight);
            TriangulateRiverQuad(m.V2, m.V4, e.V2, e.V4, riverSurfaceHeight, 0.6f, reversed, ids);
            centroid = Math3dUtil.ProjectToSphere(centroid, riverSurfaceHeight);
            m.V2 = Math3dUtil.ProjectToSphere(m.V2, riverSurfaceHeight);
            m.V4 = Math3dUtil.ProjectToSphere(m.V4, riverSurfaceHeight);
            _chunk.GetRivers()!.AddTriangle([centroid, m.V2, m.V4], HexMeshConstant.TriArr(HexMeshConstant.Weights1),
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
            centerL = tile.GetFirstSolidCorner(tile.PreviousIdx(idx),
                hexPlanetManagerRepo.Radius + height, 0.25f);
            centerR = tile.GetSecondSolidCorner(tile.NextIdx(idx), hexPlanetManagerRepo.Radius + height,
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
            centerR = tile.GetSolidEdgeMiddle(tile.NextIdx(idx),
                hexPlanetManagerRepo.Radius + height, 0.5f * HexMetrics.InnerToOuter);
        }
        else if (Overrider.HasRiverThroughEdge(tile, tile.Previous2Idx(idx)))
        {
            // 钝角弯
            centerL = tile.GetSolidEdgeMiddle(tile.PreviousIdx(idx),
                hexPlanetManagerRepo.Radius + height, 0.5f * HexMetrics.InnerToOuter);
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
        TriangulateEdgeStrip(m, HexMeshConstant.Weights1, tile.Id, e, HexMeshConstant.Weights1, tile.Id);
        var ids = Vector3.One * tile.Id;
        var terrain = _chunk.GetTerrain()!;
        terrain.AddTriangle([centerL, m.V1, m.V2], HexMeshConstant.TriArr(HexMeshConstant.Weights1), tis: ids);
        terrain.AddQuad([centerL, centroid, m.V2, m.V3], HexMeshConstant.QuadArr(HexMeshConstant.Weights1), tis: ids);
        terrain.AddQuad([centroid, centerR, m.V3, m.V4], HexMeshConstant.QuadArr(HexMeshConstant.Weights1), tis: ids);
        terrain.AddTriangle([centerR, m.V4, m.V5], HexMeshConstant.TriArr(HexMeshConstant.Weights1), tis: ids);

        if (!Overrider.IsUnderwater(tile))
        {
            var reversed = Overrider.HasIncomingRiverThroughEdge(tile, idx);
            var riverSurfaceHeight = Overrider.RiverSurfaceY(tile, hexPlanetManagerRepo.UnitHeight);
            TriangulateRiverQuad(centerL, centerR, m.V2, m.V4,
                hexPlanetManagerRepo.Radius + riverSurfaceHeight, 0.4f, reversed, ids);
            TriangulateRiverQuad(m.V2, m.V4, e.V2, e.V4,
                hexPlanetManagerRepo.Radius + riverSurfaceHeight, 0.6f, reversed, ids);
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
        _chunk.GetRivers()!.AddQuad([v1, v2, v3, v4],
            HexMeshConstant.QuadArr(HexMeshConstant.Weights1, HexMeshConstant.Weights2),
            reversed ? QuadUv(1f, 0f, 0.8f - v, 0.6f - v) : QuadUv(0f, 1f, v, v + 0.2f),
            tis: ids);
    }

    private void TriangulateEdgeFan(Vector3 center, EdgeVertices edge, float id, bool simple = false)
    {
        var ids = Vector3.One * id;
        var terrain = _chunk.GetTerrain()!;
        if (simple)
            terrain.AddTriangle([center, edge.V1, edge.V5],
                HexMeshConstant.TriArr(HexMeshConstant.Weights1), tis: ids);
        else
        {
            terrain.AddTriangle([center, edge.V1, edge.V2],
                HexMeshConstant.TriArr(HexMeshConstant.Weights1), tis: ids);
            terrain.AddTriangle([center, edge.V2, edge.V3],
                HexMeshConstant.TriArr(HexMeshConstant.Weights1), tis: ids);
            terrain.AddTriangle([center, edge.V3, edge.V4],
                HexMeshConstant.TriArr(HexMeshConstant.Weights1), tis: ids);
            terrain.AddTriangle([center, edge.V4, edge.V5],
                HexMeshConstant.TriArr(HexMeshConstant.Weights1), tis: ids);
        }
    }

    private void TriangulateEdgeStrip(EdgeVertices e1, Color w1, float id1,
        EdgeVertices e2, Color w2, float id2, bool hasRoad = false, bool simple = false)
    {
        Vector3 ids;
        ids.X = ids.Z = id1;
        ids.Y = id2;
        var terrain = _chunk.GetTerrain()!;
        if (simple)
            terrain.AddQuad([e1.V1, e1.V5, e2.V1, e2.V5], HexMeshConstant.QuadArr(w1, w2), tis: ids);
        else
        {
            terrain.AddQuad([e1.V1, e1.V2, e2.V1, e2.V2], HexMeshConstant.QuadArr(w1, w2), tis: ids);
            terrain.AddQuad([e1.V2, e1.V3, e2.V2, e2.V3], HexMeshConstant.QuadArr(w1, w2), tis: ids);
            terrain.AddQuad([e1.V3, e1.V4, e2.V3, e2.V4], HexMeshConstant.QuadArr(w1, w2), tis: ids);
            terrain.AddQuad([e1.V4, e1.V5, e2.V4, e2.V5], HexMeshConstant.QuadArr(w1, w2), tis: ids);
        }

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

    private void TriangulateWithoutRiver(Tile tile, int idx, Vector3 centroid, EdgeVertices e, bool simple)
    {
        TriangulateEdgeFan(centroid, e, tile.Id, simple);
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
            TriangulateRoadSegment(mL, mC, mR, e.V2, e.V3, e.V4, HexMeshConstant.Weights1, HexMeshConstant.Weights1,
                ids);
            var roads = _chunk.GetRoads()!;
            roads.AddTriangle([centroid, mL, mC], HexMeshConstant.TriArr(HexMeshConstant.Weights1),
                [new Vector2(1f, 0f), new Vector2(0f, 0f), new Vector2(1f, 0f)], tis: ids);
            roads.AddTriangle([centroid, mC, mR], HexMeshConstant.TriArr(HexMeshConstant.Weights1),
                [new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(0f, 0f)], tis: ids);
        }
        else TriangulateRoadEdge(centroid, mL, mR, id);
    }

    private void TriangulateRoadEdge(Vector3 centroid, Vector3 mL, Vector3 mR, float id)
    {
        var ids = Vector3.One * id;
        _chunk.GetRoads()!.AddTriangle([centroid, mL, mR], HexMeshConstant.TriArr(HexMeshConstant.Weights1),
            [new Vector2(1f, 0f), new Vector2(0f, 0f), new Vector2(0f, 0f)], tis: ids);
    }

    // 顶点的排序：
    // v4  v5  v6
    // v1  v2  v3
    // 0.0 1.0 0.0
    private void TriangulateRoadSegment(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, Vector3 v5, Vector3 v6,
        Color w1, Color w2, Vector3 ids)
    {
        var roads = _chunk.GetRoads()!;
        roads.AddQuad([v1, v2, v4, v5], HexMeshConstant.QuadArr(w1, w2), QuadUv(0f, 1f, 0f, 0f), tis: ids);
        roads.AddQuad([v2, v3, v5, v6], HexMeshConstant.QuadArr(w1, w2), QuadUv(1f, 0f, 0f, 0f), tis: ids);
    }

    private void TriangulateWaterfallInWater(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4,
        float height1, float height2, float waterHeight, Vector3 ids)
    {
        v1 = Math3dUtil.ProjectToSphere(v1, height1);
        v2 = Math3dUtil.ProjectToSphere(v2, height1);
        v3 = Math3dUtil.ProjectToSphere(v3, height2);
        v4 = Math3dUtil.ProjectToSphere(v4, height2);
        v1 = hexPlanetManagerRepo.Perturb(v1);
        v2 = hexPlanetManagerRepo.Perturb(v2);
        v3 = hexPlanetManagerRepo.Perturb(v3);
        v4 = hexPlanetManagerRepo.Perturb(v4);
        var t = (waterHeight - height2) / (height1 - height2);
        v3 = v3.Lerp(v1, t);
        v4 = v4.Lerp(v2, t);
        _chunk.GetRivers()!.AddQuadUnperturbed([v1, v2, v3, v4],
            HexMeshConstant.QuadArr(HexMeshConstant.Weights1, HexMeshConstant.Weights2),
            QuadUv(0f, 1f, 0.8f, 1f), tis: ids);
    }

    private void TriangulateConnection(Tile tile, int idx, EdgeVertices e, bool simple)
    {
        var tileHeight = GetOverrideHeight(tile);
        var neighbor = tileRepo.GetNeighborByIdx(tile, idx)!;
        var neighborHeight = GetOverrideHeight(neighbor);
        // 连接将由更低的地块或相同高度时 Id 更大的地块生成，或者是编辑地块与非编辑地块间的连接
        if ((tileHeight > neighborHeight
             || (Mathf.Abs(tileHeight - neighborHeight) < 0.0001f * hexPlanetManagerRepo.StandardScale
                 && tile.Id < neighbor.Id))
            && !Overrider.IsOverridingTileConnection(tile, neighbor)) return;
        var vn1 = faceRepo.GetCornerByFaceId(neighbor, tile.HexFaceIds[idx],
            hexPlanetManagerRepo.Radius + neighborHeight, HexMetrics.SolidFactor);
        var vn2 = faceRepo.GetCornerByFaceId(neighbor, tile.HexFaceIds[tile.NextIdx(idx)],
            hexPlanetManagerRepo.Radius + neighborHeight, HexMetrics.SolidFactor);
        var en = new EdgeVertices(vn1, vn2);
        var hasRiver = Overrider.HasRiverThroughEdge(tile, idx);
        var hasRoad = Overrider.HasRoadThroughEdge(tile, idx);
        if (hasRiver)
        {
            en.V3 = Math3dUtil.ProjectToSphere(en.V3,
                hexPlanetManagerRepo.Radius + Overrider.StreamBedY(neighbor, hexPlanetManagerRepo.UnitHeight));
            var ids = new Vector3(tile.Id, neighbor.Id, tile.Id);
            if (!Overrider.IsUnderwater(tile))
            {
                if (!Overrider.IsUnderwater(neighbor))
                    TriangulateRiverQuad(e.V2, e.V4, en.V2, en.V4,
                        hexPlanetManagerRepo.Radius + Overrider.RiverSurfaceY(tile, hexPlanetManagerRepo.UnitHeight),
                        hexPlanetManagerRepo.Radius +
                        Overrider.RiverSurfaceY(neighbor, hexPlanetManagerRepo.UnitHeight), 0.8f,
                        Overrider.HasIncomingRiver(tile) && Overrider.HasIncomingRiverThroughEdge(tile, idx), ids);
                else if (Overrider.Elevation(tile) > Overrider.Elevation(neighbor))
                    TriangulateWaterfallInWater(e.V2, e.V4, en.V2, en.V4,
                        hexPlanetManagerRepo.Radius + Overrider.RiverSurfaceY(tile, hexPlanetManagerRepo.UnitHeight),
                        hexPlanetManagerRepo.Radius +
                        Overrider.RiverSurfaceY(neighbor, hexPlanetManagerRepo.UnitHeight),
                        hexPlanetManagerRepo.Radius +
                        Overrider.WaterSurfaceY(neighbor, hexPlanetManagerRepo.UnitHeight), ids);
            }
            else if (!Overrider.IsUnderwater(neighbor) && Overrider.Elevation(neighbor) > Overrider.Elevation(tile))
                TriangulateWaterfallInWater(en.V4, en.V2, e.V4, e.V2,
                    hexPlanetManagerRepo.Radius + Overrider.RiverSurfaceY(neighbor, hexPlanetManagerRepo.UnitHeight),
                    hexPlanetManagerRepo.Radius + Overrider.RiverSurfaceY(tile, hexPlanetManagerRepo.UnitHeight),
                    hexPlanetManagerRepo.Radius + Overrider.WaterSurfaceY(tile, hexPlanetManagerRepo.UnitHeight),
                    ids);
        }

        if (Lod > ChunkLod.SimpleHex && Overrider.GetEdgeType(tile, neighbor) == HexEdgeType.Slope)
            TriangulateEdgeTerraces(e, tile, en, neighbor, hasRoad, !hasRiver && simple);
        else
            TriangulateEdgeStrip(e, HexMeshConstant.Weights1, tile.Id,
                en, HexMeshConstant.Weights2, neighbor.Id, hasRoad, !hasRiver && simple);

        _chunk.GetFeatures()!.AddWall(e, tile, en, neighbor, hasRiver, hasRoad, Overrider, Lod);

        var preNeighbor = tileRepo.GetNeighborByIdx(tile, tile.PreviousIdx(idx))!;
        var preNeighborHeight = GetOverrideHeight(preNeighbor);
        // 连接角落的三角形由周围 3 个地块中最低或者一样高时 Id 最大的生成，或者是编辑地块与非编辑地块间的连接三角形
        if (tileHeight < preNeighborHeight
            || (Mathf.Abs(tileHeight - preNeighborHeight) < 0.0001f * hexPlanetManagerRepo.StandardScale
                && tile.Id > preNeighbor.Id)
            || Overrider.IsOverridingTileConnection(tile, preNeighbor))
        {
            var vpn = faceRepo.GetCornerByFaceId(preNeighbor, tile.HexFaceIds[idx],
                hexPlanetManagerRepo.Radius + preNeighborHeight, HexMetrics.SolidFactor);
            TriangulateCorner(e.V1, tile, vpn, preNeighbor, vn1, neighbor);
        }
    }

    // 需要保证入参 bottom -> left -> right 是顺时针
    private void TriangulateCorner(Vector3 bottom, Tile bottomTile,
        Vector3 left, Tile leftTile, Vector3 right, Tile rightTile)
    {
        var edgeType1 = Overrider.GetEdgeType(bottomTile, leftTile);
        var edgeType2 = Overrider.GetEdgeType(bottomTile, rightTile);
        if (Lod > ChunkLod.SimpleHex)
        {
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
                _chunk.GetTerrain()!.AddTriangle([bottom, left, right],
                    [HexMeshConstant.Weights1, HexMeshConstant.Weights2, HexMeshConstant.Weights3],
                    tis: new Vector3(bottomTile.Id, leftTile.Id, rightTile.Id));
        }
        else
            _chunk.GetTerrain()!.AddTriangle([bottom, left, right],
                [HexMeshConstant.Weights1, HexMeshConstant.Weights2, HexMeshConstant.Weights3],
                tis: new Vector3(bottomTile.Id, leftTile.Id, rightTile.Id));

        _chunk.GetFeatures()!.AddWall(bottom, bottomTile, left, leftTile, right, rightTile, Overrider, Lod);
    }

    // 三角形靠近 tile 的左边是阶地，右边是悬崖，另一边任意的情况
    private void TriangulateCornerTerracesCliff(Vector3 begin, Tile beginTile,
        Vector3 left, Tile leftTile, Vector3 right, Tile rightTile)
    {
        var b = 1f / Mathf.Abs(Overrider.Elevation(rightTile) - Overrider.Elevation(beginTile));
        var boundary = hexPlanetManagerRepo.Perturb(begin).Lerp(hexPlanetManagerRepo.Perturb(right), b);
        var boundaryWeights = HexMeshConstant.Weights1.Lerp(HexMeshConstant.Weights3, b);
        var ids = new Vector3(beginTile.Id, leftTile.Id, rightTile.Id);
        TriangulateBoundaryTriangle(begin, HexMeshConstant.Weights1, left, HexMeshConstant.Weights2, boundary,
            boundaryWeights, ids);
        if (Overrider.GetEdgeType(leftTile, rightTile) == HexEdgeType.Slope)
            TriangulateBoundaryTriangle(left, HexMeshConstant.Weights2, right, HexMeshConstant.Weights3,
                boundary, boundaryWeights, ids);
        else
            _chunk.GetTerrain()!.AddTriangleUnperturbed(
                [hexPlanetManagerRepo.Perturb(left), hexPlanetManagerRepo.Perturb(right), boundary],
                [HexMeshConstant.Weights2, HexMeshConstant.Weights3, boundaryWeights], tis: ids);
    }

    // 三角形靠近 tile 的左边是悬崖，右边是阶地，另一边任意的情况
    private void TriangulateCornerCliffTerraces(Vector3 begin, Tile beginTile,
        Vector3 left, Tile leftTile, Vector3 right, Tile rightTile)
    {
        var b = 1f / Mathf.Abs(Overrider.Elevation(leftTile) - Overrider.Elevation(beginTile));
        var boundary = hexPlanetManagerRepo.Perturb(begin).Lerp(hexPlanetManagerRepo.Perturb(left), b);
        var boundaryWeights = HexMeshConstant.Weights1.Lerp(HexMeshConstant.Weights2, b);
        var ids = new Vector3(beginTile.Id, leftTile.Id, rightTile.Id);
        TriangulateBoundaryTriangle(right, HexMeshConstant.Weights3, begin, HexMeshConstant.Weights1, boundary,
            boundaryWeights, ids);
        if (Overrider.GetEdgeType(leftTile, rightTile) == HexEdgeType.Slope)
            TriangulateBoundaryTriangle(left, HexMeshConstant.Weights2, right, HexMeshConstant.Weights3,
                boundary, boundaryWeights, ids);
        else
            _chunk.GetTerrain()!.AddTriangleUnperturbed(
                [hexPlanetManagerRepo.Perturb(left), hexPlanetManagerRepo.Perturb(right), boundary],
                [HexMeshConstant.Weights2, HexMeshConstant.Weights3, boundaryWeights], tis: ids);
    }

    // 阶地和悬崖中间的半三角形
    private void TriangulateBoundaryTriangle(Vector3 begin, Color beginWeight,
        Vector3 left, Color leftWeight, Vector3 boundary, Color boundaryWeight, Vector3 ids)
    {
        var v2 = hexPlanetManagerRepo.Perturb(HexMetrics.TerraceLerp(begin, left, 1));
        var w2 = HexMetrics.TerraceLerp(beginWeight, leftWeight, 1);
        var terrain = _chunk.GetTerrain()!;
        terrain.AddTriangleUnperturbed([hexPlanetManagerRepo.Perturb(begin), v2, boundary],
            [beginWeight, w2, boundaryWeight], tis: ids);
        for (var i = 2; i < HexMetrics.TerraceSteps; i++)
        {
            var v1 = v2;
            var w1 = w2;
            v2 = hexPlanetManagerRepo.Perturb(HexMetrics.TerraceLerp(begin, left, i));
            w2 = HexMetrics.TerraceLerp(beginWeight, leftWeight, i);
            terrain.AddTriangleUnperturbed([v1, v2, boundary], [w1, w2, boundaryWeight], tis: ids);
        }

        terrain.AddTriangleUnperturbed([v2, hexPlanetManagerRepo.Perturb(left), boundary],
            [w2, leftWeight, boundaryWeight], tis: ids);
    }

    // 处理高度不同的 beginTile 和两个高度相同的 endTile（即三角形两边是等高阶地，一边是平地）的情况
    private void TriangulateCornerTerraces(Vector3 begin, Tile beginTile,
        Vector3 left, Tile leftTile, Vector3 right, Tile rightTile)
    {
        var v3 = HexMetrics.TerraceLerp(begin, left, 1);
        var v4 = HexMetrics.TerraceLerp(begin, right, 1);
        var w3 = HexMetrics.TerraceLerp(HexMeshConstant.Weights1, HexMeshConstant.Weights2, 1);
        var w4 = HexMetrics.TerraceLerp(HexMeshConstant.Weights1, HexMeshConstant.Weights3, 1);
        var ids = new Vector3(beginTile.Id, leftTile.Id, rightTile.Id);
        var terrain = _chunk.GetTerrain()!;
        terrain.AddTriangle([begin, v3, v4], [HexMeshConstant.Weights1, w3, w4], tis: ids);
        for (var i = 2; i < HexMetrics.TerraceSteps; i++)
        {
            var v1 = v3;
            var v2 = v4;
            var w1 = w3;
            var w2 = w4;
            v3 = HexMetrics.TerraceLerp(begin, left, i);
            v4 = HexMetrics.TerraceLerp(begin, right, i);
            w3 = HexMetrics.TerraceLerp(HexMeshConstant.Weights1, HexMeshConstant.Weights2, i);
            w4 = HexMetrics.TerraceLerp(HexMeshConstant.Weights1, HexMeshConstant.Weights3, i);
            terrain.AddQuad([v1, v2, v3, v4], [w1, w2, w3, w4], tis: ids);
        }

        terrain.AddQuad([v3, v4, left, right], [w3, w4, HexMeshConstant.Weights2, HexMeshConstant.Weights3],
            tis: ids);
    }

    private void TriangulateEdgeTerraces(EdgeVertices begin, Tile beginTile, EdgeVertices end, Tile endTile,
        bool hasRoad, bool simple)
    {
        var e2 = EdgeVertices.TerraceLerp(begin, end, 1);
        var w2 = HexMetrics.TerraceLerp(HexMeshConstant.Weights1, HexMeshConstant.Weights2, 1);
        var i1 = beginTile.Id;
        var i2 = endTile.Id;
        TriangulateEdgeStrip(begin, HexMeshConstant.Weights1, i1, e2, w2, i2, hasRoad, simple);
        for (var i = 2; i < HexMetrics.TerraceSteps; i++)
        {
            var e1 = e2;
            var w1 = w2;
            e2 = EdgeVertices.TerraceLerp(begin, end, i);
            w2 = HexMetrics.TerraceLerp(HexMeshConstant.Weights1, HexMeshConstant.Weights2, i);
            TriangulateEdgeStrip(e1, w1, i1, e2, w2, i2, hasRoad, simple);
        }

        TriangulateEdgeStrip(e2, w2, i1, end, HexMeshConstant.Weights2, i2, hasRoad, simple);
    }

    private static Vector2[] QuadUv(float uMin, float uMax, float vMin, float vMax) =>
    [
        new(uMin, vMin), new(uMax, vMin),
        new(uMin, vMax), new(uMax, vMax)
    ];
}