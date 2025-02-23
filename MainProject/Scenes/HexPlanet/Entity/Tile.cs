using System.Collections.Generic;
using System.Linq;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;

public class Tile(
    int centerId,
    int chunkId,
    List<int> hexFaceIds,
    List<int> neighborCenterIds,
    int id = -1)
{
    public int Id { get; } = id;
    public int ChunkId { get; } = chunkId;
    public int CenterId { get; } = centerId; // 注意，此处对应的是中心点投射到单位球上的 Point id。
    public List<int> HexFaceIds { get; } = hexFaceIds; // 已确保顺序为顺时针方向

    // 单位重心（顶点坐标的算术平均）
    public Vector3 UnitCentroid { get; } = hexFaceIds
        .Select(id => Face.GetById(id).Center.Normalized())
        .Aggregate((v1, v2) => v1 + v2) / hexFaceIds.Count;

    // 已确保顺序和 HexFaceIds 对应，每个邻居共边的顶点是 HexFaceIds[i] 和 HexFaceIds[(i + 1) % HexFaceIds.Count]
    public List<int> NeighborCenterIds { get; } = neighborCenterIds;
    public int Elevation { get; set; } = GD.RandRange(0, 10);
    public static float UnitHeight { get; set; } = 1f;

    public float Height
    {
        get => (Elevation + GetPerturbHeight()) * UnitHeight;
        set => Elevation = Mathf.Clamp((int)((value - GetPerturbHeight()) / UnitHeight), 0,
            HexMetrics.ElevationStep);
    }

    public Color Color { get; set; } = Color.FromHsv(GD.Randf(), GD.Randf(), GD.Randf());

    private float GetPerturbHeight()
    {
        var radius = UnitHeight / HexMetrics.MaxHeightRadiusRatio * HexMetrics.ElevationStep;
        return HexMetrics.SampleNoise(GetCenter(radius)).Y * 2f * HexMetrics.ElevationPerturbStrength;
    }

    public Vector3 GetCentroid(float radius) => UnitCentroid * radius;

    // 获取地块的形状角落顶点（顺时针顺序）
    public IEnumerable<Vector3> GetCorners(float radius, float size = 1f) =>
        from faceId in HexFaceIds
        select Face.GetById(faceId)
        into face
        select UnitCentroid.Lerp(face.Center, size)
        into pos
        select Math3dUtil.ProjectToSphere(pos, radius);

    public Vector3 GetCorner(int idx, float radius = 1f, float size = 1f) =>
        Math3dUtil.ProjectToSphere(UnitCentroid.Lerp(Face.GetById(HexFaceIds[idx]).Center, size), radius);

    public Vector3 GetCornerByFaceId(int id, float radius = 1f, float size = 1f) =>
        Math3dUtil.ProjectToSphere(UnitCentroid.Lerp(Face.GetById(id).Center, size), radius);

    public Vector3 GetCenter(float radius) => Math3dUtil.ProjectToSphere(Point.GetById(CenterId).Position, radius);
    public IEnumerable<Tile> GetNeighbors() => NeighborCenterIds.Select(GetByCenterId);

    public Tile GetNeighborByIdx(int idx) =>
        idx >= 0 && idx < NeighborCenterIds.Count ? GetByCenterId(NeighborCenterIds[idx]) : null;

    public IEnumerable<Tile> GetTilesInDistance(int dist)
    {
        if (dist == 0) return [this];
        HashSet<Tile> resultSet = [this];
        List<Tile> preRing = [this];
        List<Tile> afterRing = [];
        for (var i = 0; i < dist; i++)
        {
            afterRing.AddRange(
                from tile in preRing
                from neighbor in tile.GetNeighbors()
                where resultSet.Add(neighbor)
                select neighbor);
            (preRing, afterRing) = (afterRing, preRing);
            afterRing.Clear();
        }

        return resultSet;
    }

    public List<Vector3> GetNeighborCommonCorners(Tile neighbor, float radius = 1f)
    {
        var idx = NeighborCenterIds.FindIndex(ncId => ncId == neighbor.CenterId);
        if (idx == -1) return null;
        return
        [
            GetCorner(idx, radius),
            GetCorner((idx + 1) % HexFaceIds.Count, radius)
        ];
    }

    /// <summary>
    /// 根据相邻的两个顶点 Face 索引，获取该方向上共边的邻居。
    /// </summary>
    /// <param name="idx1">顶点 Face 索引 1</param>
    /// <param name="idx2">顶点 Face 索引 2</param>
    /// <returns>一个共边的邻居</returns>
    public Tile GetNeighborByDirection(int idx1, int idx2)
    {
        if (idx2 == (idx1 + 1) % HexFaceIds.Count) return GetNeighborByIdx(idx2);
        return idx1 == (idx2 + 1) % HexFaceIds.Count ? GetNeighborByIdx(idx1) : null;
    }

    /// <summary>
    /// 根据顶点 Face 索引，获取该方向上的共角落的两个邻居。
    /// </summary>
    /// <param name="idx">顶点 Face 索引</param>
    /// <param name="filterNeighborId">需要过滤掉（不返回）的邻居 id</param>
    /// <returns>两个共角落的邻居（如果过滤，则为一个）</returns>
    public List<Tile> GetNeighborsByDirection(int idx, int filterNeighborId = -1)
    {
        var res = new List<Tile>();
        var neighbor1 = GetNeighborByIdx(idx);
        var neighbor2 = GetNeighborByIdx(idx == HexFaceIds.Count - 1 ? 0 : idx + 1);
        if (neighbor1.Id != filterNeighborId)
            res.Add(neighbor1);
        if (neighbor2.Id != filterNeighborId)
            res.Add(neighbor2);
        return res;
    }

    #region 数据查询

    private static readonly Dictionary<int, Tile> Repo = new();
    private static readonly Dictionary<int, int> CenterIdIndex = new();

    public static void Truncate()
    {
        Repo.Clear();
        CenterIdIndex.Clear();
    }

    public static Tile Add(int centerId, int chunkId, List<int> hexFaceIds, List<int> neighborCenterIds)
    {
        var tile = new Tile(centerId, chunkId, hexFaceIds, neighborCenterIds, Repo.Count);
        Repo.Add(tile.Id, tile);
        CenterIdIndex.Add(centerId, tile.Id);
        return tile;
    }

    public static Tile GetById(int id) => Repo.GetValueOrDefault(id);

    public static Tile GetByCenterId(int centerId) =>
        CenterIdIndex.TryGetValue(centerId, out var tileId)
            ? Repo.GetValueOrDefault(tileId)
            : null;

    public static IEnumerable<Tile> GetAll() => Repo.Values;
    public static int GetCount() => Repo.Count;

    #endregion
}