using System.Collections.Generic;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Base;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;

public class Tile(
    int centerId,
    int chunkId,
    Vector3 unitCentroid,
    List<int> hexFaceIds,
    List<int> neighborCenterIds,
    int id = -1): AEntity(id)
{
    public int ChunkId { get; } = chunkId;
    public int CenterId { get; } = centerId; // 注意，此处对应的是中心点投射到单位球上的 Point id。
    public List<int> HexFaceIds { get; } = hexFaceIds; // 已确保顺序为顺时针方向

    // 单位重心（顶点坐标的算术平均）
    public Vector3 UnitCentroid { get; } = unitCentroid;

    // 已确保顺序和 HexFaceIds 对应，每个邻居共边的顶点是 HexFaceIds[i] 和 HexFaceIds[(i + 1) % HexFaceIds.Count]
    public List<int> NeighborCenterIds { get; } = neighborCenterIds;
    public int Elevation { get; set; } = GD.RandRange(0, 10);

    public Color Color { get; set; } = Color.FromHsv(GD.Randf(), GD.Randf(), GD.Randf());

    public Vector3 GetCentroid(float radius) => UnitCentroid * radius;
}