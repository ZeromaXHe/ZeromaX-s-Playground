using Domains.Bases;
using Domains.Models.ValueObjects.PlanetGenerates;
using Godot;

namespace Domains.Models.Entities.PlanetGenerates;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-02-22 23:31
public class Chunk(
    int centerId,
    Vector3 pos,
    List<int> hexFaceIds,
    List<int> neighborCenterIds,
    int id = -1) : Entity(id)
{
    // 注意，此处对应的是中心点投射到单位球上的 Point id。
    public int CenterId { get; } = centerId;

    // 这里存储是实际地块中心位置（带有星球半径）
    public Vector3 Pos { get; } = pos;

    // 已确保顺序为顺时针方向
    public List<int> HexFaceIds { get; } = hexFaceIds;

    // 已确保顺序和 HexFaceIds 对应，每个邻居共边的顶点是 HexFaceIds[i] 和 HexFaceIds[(i + 1) % HexFaceIds.Count]
    public List<int> NeighborCenterIds { get; } = neighborCenterIds;
    public List<int> TileIds { get; } = [];
    public bool Insight { get; set; }
    public ChunkLod Lod { get; set; }
}