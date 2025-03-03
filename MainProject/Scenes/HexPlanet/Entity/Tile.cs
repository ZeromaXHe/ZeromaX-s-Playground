using System.Collections.Generic;
using System.Linq;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Base;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Enum;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;

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

    public int Elevation { get; set; } = GD.RandRange(0, 10);
    public HexEdgeType GetEdgeType(Tile neighbor) => HexMetrics.GetEdgeType(Elevation, neighbor.Elevation);
    public int TerrainTypeIndex { get; set; } = GD.RandRange(0, 5);

    #region 河流

    public bool HasIncomingRiver { get; set; }
    public bool HasOutgoingRiver { get; set; }

    // 流入河流的邻居 Tile Id
    public int IncomingRiverNId { get; set; }

    // 流出河流的邻居 Tile Id
    public int OutgoingRiverNId { get; set; }
    public bool HasRiver => HasIncomingRiver || HasOutgoingRiver;
    public bool HasRiverBeginOrEnd => HasIncomingRiver != HasOutgoingRiver;
    public int RiverBeginOrEndNId => HasIncomingRiver ? IncomingRiverNId : OutgoingRiverNId;

    public bool HasRiverToNeighbor(int neighborId) =>
        (HasIncomingRiver && IncomingRiverNId == neighborId)
        || (HasOutgoingRiver && OutgoingRiverNId == neighborId);

    public bool IsValidRiverDestination(Tile neighbor) =>
        Elevation >= neighbor.Elevation || WaterLevel == neighbor.Elevation;

    #endregion

    #region 道路

    public bool[] Roads { get; } = new bool[hexFaceIds.Count];
    public bool HasRoadThroughEdge(int idx) => Roads[idx];
    public bool HasRoads => Roads.Any(r => r); // C# 貌似没有类似 Java Function.identity() 方法，或者 F# id 的这种默认实现

    #endregion

    #region 水面

    public int WaterLevel { get; set; } = HexMetrics.ElevationStep / 2;
    public bool IsUnderwater => WaterLevel > Elevation;

    #endregion

    #region 特征

    public int UrbanLevel { get; set; } = GD.RandRange(0, 3);
    public int FarmLevel { get; set; } = GD.RandRange(0, 3);
    public int PlantLevel { get; set; } = GD.RandRange(0, 3);
    public bool Walled { get; set; } = GD.Randf() < 0.1f;
    public int SpecialIndex { get; set; }
    public bool IsSpecial => SpecialIndex > 0;

    #endregion

    public int UnitId { get; set; }
    public bool HasUnit => UnitId > 0;

    public int Visibility { get; set; }
    public bool IsVisible => Visibility > 0 && Explorable;

    public int ViewElevation => Mathf.Max(Elevation, WaterLevel);

    private bool _explored;

    public bool Explored
    {
        get => _explored && Explorable;
        set => _explored = value;
    }

    public bool Explorable { get; set; } = true;
}