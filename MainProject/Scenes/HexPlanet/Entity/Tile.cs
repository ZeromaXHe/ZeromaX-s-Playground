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
    public List<int> NeighborCenterIds { get; } = neighborCenterIds;
    public float Height { get; set; } = GD.Randf() * 100f * 0.1f;
    public Color Color { get; set; } = Colors.White;

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
        var points = GetCorners(radius, 1f);
        foreach (var lowerP in neighborPoints)
        {
            foreach (var p in points.Where(p => p.IsEqualApprox(lowerP)))
            {
                commonPoints.Add(p);
                break;
            }
        }

        if (commonPoints.Count == 2) return commonPoints;
        GD.PrintErr($"Error: tile {Id} has no 2 common points with neighbor {neighbor.Id}");
        return null;
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