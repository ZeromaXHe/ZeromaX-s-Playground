using Commons.Utils;
using Domains.Bases;
using Domains.Models.ValueObjects.PlanetGenerates;
using Godot;

namespace Domains.Models.Entities.PlanetGenerates;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-02-14 11:50
public class Tile(
    int centerId,
    int chunkId,
    Vector3 unitCentroid,
    List<Vector3> unitCorners,
    List<int> hexFaceIds,
    List<int> neighborCenterIds,
    int id = -1) : Entity(id)
{
    public int ChunkId { get; } = chunkId;
    public int CenterId { get; } = centerId; // 注意，此处对应的是中心点投射到单位球上的 Point id。
    public List<int> HexFaceIds { get; } = hexFaceIds; // 已确保顺序为顺时针方向

    public bool IsPentagon() => HexFaceIds.Count == 5;
    public int PreviousIdx(int idx) => idx == 0 ? HexFaceIds.Count - 1 : idx - 1;
    public int Previous2Idx(int idx) => idx <= 1 ? HexFaceIds.Count - 2 + idx : idx - 2;
    public int NextIdx(int idx) => (idx + 1) % HexFaceIds.Count;
    public int Next2Idx(int idx) => (idx + 2) % HexFaceIds.Count;
    public int OppositeIdx(int idx) => (idx + 3) % HexFaceIds.Count;

    // 单位重心（顶点坐标的算术平均）
    public Vector3 UnitCentroid { get; } = unitCentroid;
    public Vector3 GetCentroid(float radius) => UnitCentroid * radius;

    // 单位角落
    public List<Vector3> UnitCorners { get; } = unitCorners;

    // 获取地块的形状角落顶点（顺时针顺序）
    public IEnumerable<Vector3> GetCorners(float radius, float size = 1f) =>
        from unitCorner in UnitCorners
        select UnitCentroid.Lerp(unitCorner, size)
        into pos
        select Math3dUtil.ProjectToSphere(pos, radius);

    // 按照 tile 高度查询 idx (顺时针第一个)角落的位置，相对于 Tile 中心进行插值 size 的缩放。
    public Vector3 GetFirstCorner(int idx, float radius = 1f, float size = 1f) =>
        Math3dUtil.ProjectToSphere(UnitCentroid.Lerp(UnitCorners[idx], size), radius);

    // 按照 tile 高度查询 NextIdx(idx) (顺时针第二个)角落的位置，相对于 Tile 中心进行插值 size 的缩放。
    public Vector3 GetSecondCorner(int idx, float radius = 1f, float size = 1f) =>
        GetFirstCorner(NextIdx(idx), radius, size);

    // 按照 tile 高度查询 idx (顺时针第一个)核心角落的位置，相对于 Tile 中心进行插值 size 的缩放。
    public Vector3 GetFirstSolidCorner(int idx, float radius = 1f, float size = 1f) =>
        GetFirstCorner(idx, radius, size * HexMetrics.SolidFactor);

    // 按照 tile 高度查询 NextIdx(idx) (顺时针第二个)核心角落的位置，相对于 Tile 中心进行插值 size 的缩放。
    public Vector3 GetSecondSolidCorner(int idx, float radius = 1f, float size = 1f) =>
        GetSecondCorner(idx, radius, size * HexMetrics.SolidFactor);

    public Vector3 GetEdgeMiddle(int idx, float radius = 1f, float size = 1f)
    {
        var corner1 = GetFirstCorner(idx, radius, size);
        var corner2 = GetFirstCorner(NextIdx(idx), radius, size);
        return corner1.Lerp(corner2, 0.5f);
    }

    public Vector3 GetSolidEdgeMiddle(int idx, float radius = 1f, float size = 1f) =>
        GetEdgeMiddle(idx, radius, size * HexMetrics.SolidFactor);

    public List<Vector3>? GetNeighborCommonCorners(Tile neighbor, float radius = 1f)
    {
        var idx = NeighborCenterIds.FindIndex(ncId => ncId == neighbor.CenterId);
        if (idx == -1) return null;
        return
        [
            GetFirstCorner(idx, radius),
            GetFirstCorner(NextIdx(idx), radius)
        ];
    }

    #region 水面

    public Vector3 GetFirstWaterCorner(int idx, float radius = 1f, float size = 1f) =>
        GetFirstCorner(idx, radius, size * HexMetrics.WaterFactor);

    public Vector3 GetSecondWaterCorner(int idx, float radius = 1f, float size = 1f) =>
        GetSecondCorner(idx, radius, size * HexMetrics.WaterFactor);

    #endregion

    // 已确保顺序和 HexFaceIds 对应，每个邻居共边的顶点是 HexFaceIds[i] 和 HexFaceIds[(i + 1) % HexFaceIds.Count]
    public List<int> NeighborCenterIds { get; } = neighborCenterIds;

    public int GetNeighborIdx(Tile neighbor) => NeighborCenterIds.FindIndex(cId => cId == neighbor.CenterId);
    public bool IsNeighbor(Tile tile) => NeighborCenterIds.Contains(tile.CenterId);

    public HexTileData Data { get; set; } = new();

    public int UnitId { get; set; }
    public bool HasUnit => UnitId > 0;

    public int Visibility { get; set; }
    public bool IsVisible => Visibility > 0 && Data.IsExplorable;

    #region 文明

    public int CivId { get; set; }

    #endregion
}