using System.Collections.Generic;
using System.Linq;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;

public class Tile(
    int centerId,
    List<int> hexFaceIds,
    List<int> neighborCenterIds,
    int id = -1)
{
    public int Id { get; } = id;
    public int CenterId { get; } = centerId; // 注意，此处对应的是中心点投射到单位球上的 Point id。
    public List<int> HexFaceIds { get; } = hexFaceIds;

    // 单位重心（顶点坐标的算术平均）
    public Vector3 UnitCentroid { get; } = hexFaceIds
        .Select(id => Face.GetById(id).Center.Normalized())
        .Aggregate((v1, v2) => v1 + v2) / hexFaceIds.Count;

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

    // 获取地块的形状角落顶点
    public IEnumerable<Vector3> GetCorners(float radius, float size) =>
        from faceId in HexFaceIds
        select Face.GetById(faceId)
        into face
        let center = Point.GetById(CenterId)
        select center.Position.Lerp(face.Center, size)
        into pos
        select Math3dUtil.ProjectToSphere(pos, radius);

    public Vector3 GetCenter(float radius) => Math3dUtil.ProjectToSphere(Point.GetById(CenterId).Position, radius);
    public IEnumerable<Tile> GetNeighbors() => NeighborCenterIds.Select(GetByCenterId);

    public List<Vector3> GetNeighborCommonCorners(Tile neighbor, float radius)
    {
        var commonPoints = new List<Vector3>();
        var neighborPoints = neighbor.GetCorners(radius, 1f);
        var points = GetCorners(radius, 1f).ToList();
        foreach (var np in neighborPoints)
        {
            foreach (var p in points.Where(p => p.IsEqualApprox(np)))
            {
                commonPoints.Add(p);
                break;
            }
        }

        if (commonPoints.Count == 2) return commonPoints;
        GD.PrintErr($"Error: tile {Id} has no 2 common points with neighbor {neighbor.Id}");
        return null;
    }

    /// <summary>
    /// 根据重心到两个角落的向量，获取该方向上的邻居。
    /// </summary>
    /// <param name="dir1">重心到角落方向向量 1</param>
    /// <param name="dir2">重心到角落方向向量 2</param>
    /// <returns>一个共边的邻居</returns>
    public Tile GetNeighborByDirection(Vector3 dir1, Vector3 dir2)
    {
        // return (from neighbor in GetNeighbors()
        //         let commonPoints = GetNeighborCommonCorners(neighbor, 1f)
        //         let v1 = (commonPoints[0] - UnitCentroid).Normalized()
        //         let v2 = (commonPoints[1] - UnitCentroid).Normalized()
        //         // 这里误差有点大，不能用 IsEqualApprox() 判断。打断点看有的误差为 0.03 多，暂时先用 0.05 判定
        //         where (v1.DistanceTo(dir1) < 0.05f || v1.DistanceTo(dir2) < 0.05f)
        //               && (v2.DistanceTo(dir1) < 0.05f || v2.DistanceTo(dir2) < 0.05f)
        //         select neighbor)
        //     .FirstOrDefault();
        // LINQ 暂时不知道怎么调试
        foreach (var neighbor in GetNeighbors())
        {
            var commonPoints = GetNeighborCommonCorners(neighbor, 1f);
            var v1 = (commonPoints[0] - UnitCentroid).Normalized();
            var v2 = (commonPoints[1] - UnitCentroid).Normalized();
            if ((v1.DistanceTo(dir1) < 0.05f || v1.DistanceTo(dir2) < 0.05f)
                && (v2.DistanceTo(dir1) < 0.05f || v2.DistanceTo(dir2) < 0.05f))
                return neighbor;
        }
        
        return null;
    }

    /// <summary>
    /// 根据中心到角落的向量，获取该方向上的两个邻居。
    /// </summary>
    /// <param name="dir">中心到角落向量</param>
    /// <param name="filterNeighborId">需要过滤掉（不返回）的邻居 id</param>
    /// <returns>两个共角落的邻居（如果过滤，则为一个）</returns>
    public List<Tile> GetNeighborsByDirection(Vector3 dir, int filterNeighborId = -1)
    {
        var v = dir.Normalized();
        var center = GetCenter(1f);
        // var res = (
        //     from neighbor in GetNeighbors()
        //     where neighbor.Id != filterNeighborId
        //     let commonPoints = GetNeighborCommonCorners(neighbor, 1f)
        //     let v1 = (commonPoints[0] - center).Normalized()
        //     let v2 = (commonPoints[1] - center).Normalized()
        //     where v.DistanceTo(v1) < 0.2f || v.DistanceTo(v2) < 0.2f
        //     select neighbor
        // ).ToList();
        // LINQ 暂时不知道怎么调试
        var res = new List<Tile>();
        foreach (var neighbor in GetNeighbors())
        {
            if (neighbor.Id == filterNeighborId) continue;
            var commonPoints = GetNeighborCommonCorners(neighbor, 1f);
            var v1 = (commonPoints[0] - center).Normalized();
            var v2 = (commonPoints[1] - center).Normalized();
            if (v.DistanceTo(v1) < 0.2f || v.DistanceTo(v2) < 0.2f)
                res.Add(neighbor);
        }
        if (res.Count != (filterNeighborId == -1 ? 2 : 1))
            GD.PrintErr($"Error: tile {Id} has {res.Count} neighbors with direction {v} and filter {filterNeighborId}");
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

    public static Tile Add(int centerId, List<int> hexFaceIds, List<int> neighborCenterIds)
    {
        var tile = new Tile(centerId, hexFaceIds, neighborCenterIds, Repo.Count);
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