using System.Collections.Generic;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Base;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Structs;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entities;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-02-14 11:50
public class Tile(
    int centerId,
    int chunkId,
    Vector3 unitCentroid,
    List<int> hexFaceIds,
    List<int> neighborCenterIds,
    int id = -1) : AEntity(id)
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

    // 已确保顺序和 HexFaceIds 对应，每个邻居共边的顶点是 HexFaceIds[i] 和 HexFaceIds[(i + 1) % HexFaceIds.Count]
    public List<int> NeighborCenterIds { get; } = neighborCenterIds;

    public int GetNeighborIdx(Tile neighbor) =>
        NeighborCenterIds.FindIndex(cId => cId == neighbor.CenterId);

    public HexTileData Data { get; set; } = new();
    
    public int UnitId { get; set; }
    public bool HasUnit => UnitId > 0;

    public int Visibility { get; set; }
    public bool IsVisible => Visibility > 0 && Data.IsExplorable;
}