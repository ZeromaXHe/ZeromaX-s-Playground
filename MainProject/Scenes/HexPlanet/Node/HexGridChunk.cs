using System.Linq;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Dependency;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Enum;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Struct;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Node;

[Tool]
public partial class HexGridChunk : Node3D
{
    [Export] private HexMesh _terrain;
    [Export] private HexMesh _rivers;
    [Export] private HexMesh _roads;
    [Export] private HexMesh _water;
    [Export] private HexMesh _waterShore;
    [Export] private HexMesh _estuary;
    [Export] private HexFeatureManager _features;

    private int _id;

    private float _radius;


    #region services

    private IChunkService _chunkService;
    private ITileService _tileService;

    private void InitServices()
    {
        _chunkService = Context.GetBean<IChunkService>();
        _tileService = Context.GetBean<ITileService>();
    }

    #endregion

    public void Init(int id, float radius)
    {
        _id = id;
        _radius = radius;
        InitServices();
        Refresh();
    }

    public override void _Process(double delta)
    {
        if (_id > 0)
        {
            var time = Time.GetTicksMsec();
            _terrain.Clear();
            _rivers.Clear();
            _roads.Clear();
            _water.Clear();
            _waterShore.Clear();
            _estuary.Clear();
            _features.Clear();
            var tileIds = _chunkService.GetById(_id).TileIds;
            var tiles = tileIds.Select(_tileService.GetById);
            foreach (var tile in tiles)
                Triangulate(tile);
            _terrain.Apply();
            _rivers.Apply();
            _roads.Apply();
            _water.Apply();
            _waterShore.Apply();
            _estuary.Apply();
            _features.Apply();
            GD.Print($"Chunk {_id} BuildMesh cost: {Time.GetTicksMsec() - time} ms");
        }

        SetProcess(false);
    }

    public void Refresh() => SetProcess(true);

    private void Triangulate(Tile tile)
    {
        // var corners = tile.GetCorners(_radius + tile.Height, 1f).ToList();
        for (var i = 0; i < tile.HexFaceIds.Count; i++)
            Triangulate(tile, i);
        if (!tile.IsUnderwater)
        {
            if (!tile.HasRiver && !tile.HasRoads)
                _features.AddFeature(tile, tile.GetCentroid(_radius + _tileService.GetHeight(tile)));
            if (tile.IsSpecial)
                _features.AddSpecialFeature(tile, tile.GetCentroid(_radius + _tileService.GetHeight(tile)));
        }
    }

    // Godot 缠绕顺序是正面顺时针，所以从 i1 对应角落到 i2 对应角落相对于 tile 重心需要是顺时针
    private void Triangulate(Tile tile, int idx)
    {
        var height = _tileService.GetHeight(tile);
        var v1 = _tileService.GetFirstSolidCorner(tile, idx, _radius + height);
        var v2 = _tileService.GetSecondSolidCorner(tile, idx, _radius + height);
        var e = new EdgeVertices(v1, v2);
        var centroid = tile.GetCentroid(_radius + height);
        if (tile.HasRiver)
        {
            if (_tileService.HasRiverThroughEdge(tile, idx))
            {
                e.V3 = Math3dUtil.ProjectToSphere(e.V3, _radius + _tileService.GetStreamBedHeight(tile));
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
                _features.AddFeature(tile, (centroid + e.V1 + e.V5) / 3f);
        }

        TriangulateConnection(tile, idx, e);
        if (tile.IsUnderwater)
            TriangulateWater(tile, idx, centroid);
    }

    private void TriangulateWater(Tile tile, int idx, Vector3 centroid)
    {
        var waterSurfaceHeight = _tileService.GetWaterSurfaceHeight(tile);
        centroid = Math3dUtil.ProjectToSphere(centroid, _radius + waterSurfaceHeight);
        var neighbor = _tileService.GetNeighborByIdx(tile, idx);
        if (!neighbor.IsUnderwater)
            TriangulateWaterShore(tile, idx, waterSurfaceHeight, neighbor, centroid);
        else
        {
            TriangulateOpenWater(tile, idx, waterSurfaceHeight, neighbor, centroid);
        }
    }

    private void TriangulateWaterShore(Tile tile, int idx, float waterSurfaceHeight, Tile neighbor, Vector3 centroid)
    {
        var e1 = new EdgeVertices(_tileService.GetFirstWaterCorner(tile, idx, _radius + waterSurfaceHeight),
            _tileService.GetSecondWaterCorner(tile, idx, _radius + waterSurfaceHeight));
        _water.AddTriangle([centroid, e1.V1, e1.V2]);
        _water.AddTriangle([centroid, e1.V2, e1.V3]);
        _water.AddTriangle([centroid, e1.V3, e1.V4]);
        _water.AddTriangle([centroid, e1.V4, e1.V5]);
        // 使用邻居的水表面高度的话，就是希望考虑岸边地块的实际水位。(不然强行拉平岸边的话，角落两个水面不一样高时太多复杂逻辑，bug 太多)
        var neighborWaterSurfaceHeight = _tileService.GetWaterSurfaceHeight(neighbor);
        var cn1 = _tileService.GetCornerByFaceId(neighbor, tile.HexFaceIds[idx],
            _radius + neighborWaterSurfaceHeight, HexMetrics.SolidFactor);
        var cn2 = _tileService.GetCornerByFaceId(neighbor, tile.HexFaceIds[tile.NextIdx(idx)],
            _radius + neighborWaterSurfaceHeight, HexMetrics.SolidFactor);
        var e2 = new EdgeVertices(cn1, cn2);
        if (tile.HasRiverToNeighbor(neighbor.Id))
            TriangulateEstuary(e1, e2, tile.IncomingRiverNId == neighbor.Id);
        else
        {
            _waterShore.AddQuad([e1.V1, e1.V2, e2.V1, e2.V2], uvs: QuadUv(0f, 0f, 0f, 1f));
            _waterShore.AddQuad([e1.V2, e1.V3, e2.V2, e2.V3], uvs: QuadUv(0f, 0f, 0f, 1f));
            _waterShore.AddQuad([e1.V3, e1.V4, e2.V3, e2.V4], uvs: QuadUv(0f, 0f, 0f, 1f));
            _waterShore.AddQuad([e1.V4, e1.V5, e2.V4, e2.V5], uvs: QuadUv(0f, 0f, 0f, 1f));
        }

        var nextNeighbor = _tileService.GetNeighborByIdx(tile, tile.NextIdx(idx));
        var nextNeighborWaterSurfaceHeight = _tileService.GetWaterSurfaceHeight(nextNeighbor);
        var cnn = _tileService.GetCornerByFaceId(nextNeighbor, tile.HexFaceIds[tile.NextIdx(idx)],
            _radius + nextNeighborWaterSurfaceHeight,
            nextNeighbor.IsUnderwater ? HexMetrics.WaterFactor : HexMetrics.SolidFactor);
        _waterShore.AddTriangle([e1.V5, e2.V5, cnn],
            uvs: [new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(0f, nextNeighbor.IsUnderwater ? 0f : 1f)]);
    }

    private void TriangulateEstuary(EdgeVertices e1, EdgeVertices e2, bool incomingRiver)
    {
        _waterShore.AddTriangle([e2.V1, e1.V2, e1.V1],
            uvs: [new Vector2(0f, 1f), new Vector2(0f, 0f), new Vector2(0f, 0f)]);
        _waterShore.AddTriangle([e2.V5, e1.V5, e1.V4],
            uvs: [new Vector2(0f, 1f), new Vector2(0f, 0f), new Vector2(0f, 0f)]);
        _estuary.AddQuad([e2.V1, e1.V2, e2.V2, e1.V3],
            uvs: [new Vector2(0f, 1f), new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(0f, 0f)],
            uvs2: incomingRiver
                ? [new Vector2(1.5f, 1f), new Vector2(0.7f, 1.15f), new Vector2(1f, 0.8f), new Vector2(0.5f, 1.1f)]
                :
                [
                    new Vector2(-0.5f, -0.2f), new Vector2(0.3f, -0.35f), new Vector2(0f, 0f), new Vector2(0.5f, -0.3f)
                ]);
        _estuary.AddTriangle([e1.V3, e2.V2, e2.V4],
            uvs: [new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(1f, 1f)],
            uvs2: incomingRiver
                ? [new Vector2(0.5f, 1.1f), new Vector2(1f, 0.8f), new Vector2(0f, 0.8f)]
                : [new Vector2(0.5f, -0.3f), new Vector2(0f, 0f), new Vector2(1f, 0f)]);
        _estuary.AddQuad([e1.V3, e1.V4, e2.V4, e2.V5],
            uvs: [new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(0f, 1f)],
            uvs2: incomingRiver
                ? [new Vector2(0.5f, 1.1f), new Vector2(0.3f, 1.15f), new Vector2(0f, 0.8f), new Vector2(-0.5f, 1f)]
                : [new Vector2(0.5f, -0.3f), new Vector2(0.7f, -0.35f), new Vector2(1f, 0f), new Vector2(1.5f, -0.2f)]);
    }

    private void TriangulateOpenWater(Tile tile, int idx, float waterSurfaceHeight, Tile neighbor, Vector3 centroid)
    {
        var c1 = _tileService.GetFirstWaterCorner(tile, idx, _radius + waterSurfaceHeight);
        var c2 = _tileService.GetSecondWaterCorner(tile, idx, _radius + waterSurfaceHeight);
        _water.AddTriangle([centroid, c1, c2]);
        // 由更大 Id 的地块绘制水域连接
        if (tile.Id > neighbor.Id)
        {
            var neighborWaterSurfaceHeight = _tileService.GetWaterSurfaceHeight(neighbor);
            var cn1 = _tileService.GetCornerByFaceId(neighbor, tile.HexFaceIds[idx],
                _radius + neighborWaterSurfaceHeight, HexMetrics.WaterFactor);
            var cn2 = _tileService.GetCornerByFaceId(neighbor, tile.HexFaceIds[tile.NextIdx(idx)],
                _radius + neighborWaterSurfaceHeight, HexMetrics.WaterFactor);
            _water.AddQuad([c1, c2, cn1, cn2]);

            var preNeighbor = _tileService.GetNeighborByIdx(tile, tile.PreviousIdx(idx));
            // 由最大 Id 的地块绘制水域角落三角形
            if (tile.Id > preNeighbor.Id)
            {
                if (!preNeighbor.IsUnderwater) return;
                var cpn = _tileService.GetCornerByFaceId(preNeighbor, tile.HexFaceIds[idx],
                    _radius + _tileService.GetWaterSurfaceHeight(preNeighbor), HexMetrics.WaterFactor);
                _water.AddTriangle([c1, cpn, cn1]);
            }
        }
    }

    private void TriangulateAdjacentToRiver(Tile tile, int idx, Vector3 centroid, EdgeVertices e)
    {
        if (tile.HasRoads)
            TriangulateRoadAdjacentToRiver(tile, idx, centroid, e);
        if (_tileService.HasRiverThroughEdge(tile, tile.NextIdx(idx)))
        {
            if (_tileService.HasRiverThroughEdge(tile, tile.PreviousIdx(idx)))
                centroid = _tileService.GetSolidEdgeMiddle(tile, idx, _radius + _tileService.GetHeight(tile),
                    0.5f * HexMetrics.InnerToOuter);
            else if (!tile.IsPentagon() && _tileService.HasRiverThroughEdge(tile, tile.Previous2Idx(idx)))
                // 注意五边形没有直线河流，一边临河另一边隔一个方向临河的情况是对应钝角河的外河岸，依然在 centroid
                centroid = _tileService.GetFirstSolidCorner(tile, idx, _radius + _tileService.GetHeight(tile), 0.25f);
        }
        else if (!tile.IsPentagon()
                 && _tileService.HasRiverThroughEdge(tile, tile.PreviousIdx(idx))
                 && _tileService.HasRiverThroughEdge(tile, tile.Next2Idx(idx)))
            // 注意五边形没有直线河流，一边临河另一边隔一个方向临河的情况是对应钝角河的外河岸，依然在 centroid
            centroid = _tileService.GetSecondSolidCorner(tile, idx, _radius + _tileService.GetHeight(tile), 0.25f);

        var m = new EdgeVertices(centroid.Lerp(e.V1, 0.5f), centroid.Lerp(e.V5, 0.5f));
        TriangulateEdgeStrip(m, tile.Color, e, tile.Color);
        TriangulateEdgeFan(centroid, m, tile.Color);

        if (!tile.IsUnderwater && !tile.HasRoadThroughEdge(idx))
            _features.AddFeature(tile, (centroid + e.V1 + e.V5) / 3f);
    }

    private void TriangulateRoadAdjacentToRiver(Tile tile, int idx, Vector3 centroid, EdgeVertices e)
    {
        var hasRoadThroughEdge = tile.HasRoadThroughEdge(idx);
        var previousHasRiver = _tileService.HasRiverThroughEdge(tile, tile.PreviousIdx(idx));
        var nextHasRiver = _tileService.HasRiverThroughEdge(tile, tile.NextIdx(idx));
        var interpolators = GetRoadInterpolators(tile, idx);
        var roadCenter = centroid;
        if (tile.HasRiverBeginOrEnd)
        {
            var riverBeginOrEndIdx = _tileService.GetRiverBeginOrEndIdx(tile);
            if (tile.IsPentagon())
                roadCenter += _tileService.GetFirstSolidCorner(tile, tile.OppositeIdx(riverBeginOrEndIdx),
                    _radius + _tileService.GetHeight(tile), HexMetrics.OuterToInner / 3f) - centroid;
            else
                roadCenter += _tileService.GetSolidEdgeMiddle(tile, tile.OppositeIdx(riverBeginOrEndIdx),
                    _radius + _tileService.GetHeight(tile), 1f / 3f) - centroid;
        }
        else
        {
            var incomingRiverIdx = _tileService.GetNeighborIdIdx(tile, tile.IncomingRiverNId);
            var outgoingRiverIdx = _tileService.GetNeighborIdIdx(tile, tile.OutgoingRiverNId);
            if (!tile.IsPentagon() && incomingRiverIdx == tile.OppositeIdx(outgoingRiverIdx))
            {
                // 河流走势是对边（直线）的情况（需要注意五边形没有对边的概念）
                Vector3 corner;
                if (previousHasRiver)
                {
                    if (!hasRoadThroughEdge && !tile.HasRoadThroughEdge(tile.NextIdx(idx))) return;
                    corner = _tileService.GetSecondSolidCorner(tile, idx, _radius + _tileService.GetHeight(tile));
                }
                else
                {
                    if (!hasRoadThroughEdge && !tile.HasRoadThroughEdge(tile.PreviousIdx(idx))) return;
                    corner = _tileService.GetFirstSolidCorner(tile, idx, _radius + _tileService.GetHeight(tile));
                }

                roadCenter += (corner - centroid) * 0.5f;
                if (incomingRiverIdx == tile.NextIdx(idx)
                    && (tile.HasRoadThroughEdge(tile.Next2Idx(idx))
                        || tile.HasRoadThroughEdge(tile.OppositeIdx(idx))))
                    _features.AddBridge(roadCenter, centroid - (corner - centroid) * 0.5f);
                centroid += (corner - centroid) * 0.25f;
            }
            else if (incomingRiverIdx == tile.PreviousIdx(outgoingRiverIdx))
            {
                // 河流走势是逆时针锐角的情况
                roadCenter -= _tileService.GetSecondCorner(tile, incomingRiverIdx,
                    _radius + _tileService.GetHeight(tile), 0.2f) - centroid;
            }
            else if (incomingRiverIdx == tile.NextIdx(outgoingRiverIdx))
            {
                // 河流走势是顺时针锐角的情况
                roadCenter -= _tileService.GetFirstCorner(tile, incomingRiverIdx,
                    _radius + _tileService.GetHeight(tile), 0.2f) - centroid;
            }
            else if (previousHasRiver && nextHasRiver)
            {
                // 河流走势是钝角的情况，且当前方向被夹在河流出入角中间
                if (!hasRoadThroughEdge) return;
                var offset = _tileService.GetSolidEdgeMiddle(tile, idx, _radius + _tileService.GetHeight(tile),
                    HexMetrics.InnerToOuter);
                roadCenter += (offset - centroid) * 0.7f;
                centroid += (offset - centroid) * 0.5f;
            }
            else if (tile.IsPentagon())
            {
                // 河流走势是钝角的情况，且当前方向在河流出入角外（即更宽阔的方向：五边形有两个方向可能）
                var firstIdx = previousHasRiver ? idx : tile.PreviousIdx(idx); // 两个可能方向中的顺时针第一个
                if (!tile.HasRoadThroughEdge(firstIdx) && !tile.HasRoadThroughEdge(tile.NextIdx(firstIdx))) return;
                var offset = _tileService.GetSecondSolidCorner(tile, firstIdx,
                    _radius + _tileService.GetHeight(tile));
                roadCenter += (offset - centroid) * 0.25f * HexMetrics.OuterToInner;
                if (idx == firstIdx && tile.HasRoadThroughEdge(tile.Previous2Idx(firstIdx)))
                    _features.AddBridge(roadCenter,
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
                if (!tile.HasRoadThroughEdge(middleIdx)
                    && !tile.HasRoadThroughEdge(tile.PreviousIdx(middleIdx))
                    && !tile.HasRoadThroughEdge(tile.NextIdx(middleIdx)))
                    return;
                var offset = _tileService.GetSolidEdgeMiddle(tile, middleIdx,
                    _radius + _tileService.GetHeight(tile));
                roadCenter += (offset - centroid) * 0.25f;
                if (idx == middleIdx && tile.HasRoadThroughEdge(tile.OppositeIdx(idx)))
                    _features.AddBridge(roadCenter,
                        centroid - (offset - centroid) * (HexMetrics.InnerToOuter * 0.7f));
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
            var riverSurfaceHeight = _radius + _tileService.GetRiverSurfaceHeight(tile);
            TriangulateRiverQuad(m.V2, m.V4, e.V2, e.V4, riverSurfaceHeight, 0.6f, reversed);
            centroid = Math3dUtil.ProjectToSphere(centroid, riverSurfaceHeight);
            m.V2 = Math3dUtil.ProjectToSphere(m.V2, riverSurfaceHeight);
            m.V4 = Math3dUtil.ProjectToSphere(m.V4, riverSurfaceHeight);
            _rivers.AddTriangle([centroid, m.V2, m.V4],
                uvs: reversed
                    ? [new Vector2(0.5f, 0.4f), new Vector2(1f, 0.2f), new Vector2(0f, 0.2f)]
                    : [new Vector2(0.5f, 0.4f), new Vector2(0f, 0.6f), new Vector2(1f, 0.6f)]);
        }
    }

    private void TriangulateWithRiver(Tile tile, int idx, Vector3 centroid, EdgeVertices e)
    {
        Vector3 centerL;
        Vector3 centerR;
        var height = _tileService.GetHeight(tile);
        if (!tile.IsPentagon() && _tileService.HasRiverThroughEdge(tile, tile.OppositeIdx(idx))) // 注意五边形没有对边的情况
        {
            // 直线河流
            centerL = _tileService.GetFirstSolidCorner(tile, tile.PreviousIdx(idx), _radius + height, 0.25f);
            centerR = _tileService.GetSecondSolidCorner(tile, tile.NextIdx(idx), _radius + height, 0.25f);
        }
        else if (_tileService.HasRiverThroughEdge(tile, tile.NextIdx(idx)))
        {
            // 锐角弯
            centerL = centroid;
            centerR = centroid.Lerp(e.V5, 2f / 3f);
        }
        else if (_tileService.HasRiverThroughEdge(tile, tile.PreviousIdx(idx)))
        {
            // 锐角弯
            centerL = centroid.Lerp(e.V1, 2f / 3f);
            centerR = centroid;
        }
        else if (_tileService.HasRiverThroughEdge(tile, tile.Next2Idx(idx)))
        {
            // 钝角弯
            centerL = centroid;
            centerR = _tileService.GetSolidEdgeMiddle(tile, tile.NextIdx(idx),
                _radius + height, 0.5f * HexMetrics.InnerToOuter);
        }
        else if (_tileService.HasRiverThroughEdge(tile, tile.Previous2Idx(idx)))
        {
            // 钝角弯
            centerL = _tileService.GetSolidEdgeMiddle(tile, tile.PreviousIdx(idx),
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
        _terrain.AddTriangle([centerL, m.V1, m.V2], TriColor(tile.Color));
        _terrain.AddQuad([centerL, centroid, m.V2, m.V3], QuadColor(tile.Color));
        _terrain.AddQuad([centroid, centerR, m.V3, m.V4], QuadColor(tile.Color));
        _terrain.AddTriangle([centerR, m.V4, m.V5], TriColor(tile.Color));

        if (!tile.IsUnderwater)
        {
            var reversed = tile.IncomingRiverNId == _tileService.GetNeighborByIdx(tile, idx).Id;
            var riverSurfaceHeight = _tileService.GetRiverSurfaceHeight(tile);
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
        _roads.AddTriangle([centroid, mL, mR],
            uvs: [new Vector2(1f, 0f), new Vector2(0f, 0f), new Vector2(0f, 0f)]);
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
        _rivers.AddQuadUnperturbed([v1, v2, v3, v4], uvs: QuadUv(0f, 1f, 0.8f, 1f));
    }

    private void TriangulateConnection(Tile tile, int idx, EdgeVertices e)
    {
        var tileHeight = _tileService.GetHeight(tile);
        var neighbor = _tileService.GetNeighborByIdx(tile, idx);
        var neighborHeight = _tileService.GetHeight(neighbor);
        // 连接将由更低的地块或相同高度时 Id 更大的地块生成
        if (tileHeight > neighborHeight ||
            (Mathf.Abs(tileHeight - neighborHeight) < 0.00001f && tile.Id < neighbor.Id)) return;
        var vn1 = _tileService.GetCornerByFaceId(neighbor, tile.HexFaceIds[idx],
            _radius + neighborHeight, HexMetrics.SolidFactor);
        var vn2 = _tileService.GetCornerByFaceId(neighbor, tile.HexFaceIds[tile.NextIdx(idx)],
            _radius + neighborHeight, HexMetrics.SolidFactor);
        var en = new EdgeVertices(vn1, vn2);
        var hasRiver = tile.HasRiverToNeighbor(neighbor.Id);
        var hasRoad = tile.HasRoadThroughEdge(idx);
        if (hasRiver)
        {
            en.V3 = Math3dUtil.ProjectToSphere(en.V3, _radius + _tileService.GetStreamBedHeight(neighbor));
            if (!tile.IsUnderwater)
            {
                if (!neighbor.IsUnderwater)
                    TriangulateRiverQuad(e.V2, e.V4, en.V2, en.V4,
                        _radius + _tileService.GetRiverSurfaceHeight(tile),
                        _radius + _tileService.GetRiverSurfaceHeight(neighbor), 0.8f,
                        tile.HasIncomingRiver && tile.IncomingRiverNId == neighbor.Id);
                else if (tile.Elevation > neighbor.Elevation)
                    TriangulateWaterfallInWater(e.V2, e.V4, en.V2, en.V4,
                        _radius + _tileService.GetRiverSurfaceHeight(tile),
                        _radius + _tileService.GetRiverSurfaceHeight(neighbor),
                        _radius + _tileService.GetWaterSurfaceHeight(neighbor));
            }
            else if (!neighbor.IsUnderwater && neighbor.Elevation > tile.Elevation)
                TriangulateWaterfallInWater(en.V4, en.V2, e.V4, e.V2,
                    _radius + _tileService.GetRiverSurfaceHeight(neighbor),
                    _radius + _tileService.GetRiverSurfaceHeight(tile),
                    _radius + _tileService.GetWaterSurfaceHeight(tile));
        }

        if (tile.GetEdgeType(neighbor) == HexEdgeType.Slope)
            TriangulateEdgeTerraces(e, tile, en, neighbor, hasRoad);
        else
            TriangulateEdgeStrip(e, tile.Color, en, neighbor.Color, hasRoad);

        _features.AddWall(e, tile, en, neighbor, hasRiver, hasRoad);

        var preNeighbor = _tileService.GetNeighborByIdx(tile, tile.PreviousIdx(idx));
        var preNeighborHeight = _tileService.GetHeight(preNeighbor);
        if (tileHeight < preNeighborHeight
            || (Mathf.Abs(tileHeight - preNeighborHeight) < 0.00001f && tile.Id > preNeighbor.Id))
        {
            // 连接角落的三角形由周围 3 个地块中最低或者一样高时 Id 最大的生成
            var vpn = _tileService.GetCornerByFaceId(preNeighbor, tile.HexFaceIds[idx],
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
            _terrain.AddTriangle([bottom, left, right], [bottomTile.Color, leftTile.Color, rightTile.Color]);

        _features.AddWall(bottom, bottomTile, left, leftTile, right, rightTile);
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
        if (leftTile.GetEdgeType(rightTile) == HexEdgeType.Slope)
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

        _terrain.AddTriangleUnperturbed([v2, HexMetrics.Perturb(left), boundary],
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