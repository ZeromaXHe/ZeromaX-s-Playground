using System.Collections.Generic;
using System.Linq;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;

public class Tile(
    int centerId,
    List<int> hexFaceIds,
    List<int> neighborCenterIds,
    float height = 0f,
    int id = -1)
{
    public int Id { get; } = id;
    public int CenterId { get; } = centerId;
    public List<int> HexFaceIds { get; } = hexFaceIds;
    public List<int> NeighborCenterIds { get; } = neighborCenterIds;
    public float Height { get; set; } = height;

    public List<Vector3> GetPoints(float radius, float size)
    {
        return (
            from faceId in HexFaceIds
            select Face.GetById(faceId)
            into face
            let center = Point.GetById(CenterId)
            select center.Position.Lerp(face.Center, size)
            into pos
            select Math3dUtil.ProjectToSphere(pos, radius)
        ).ToList();
    }

    #region 数据查询

    private static readonly Dictionary<int, Tile> Repo = new();
    private static readonly Dictionary<int, int> CenterIdIndex = new();

    public static void Truncate()
    {
        Repo.Clear();
        CenterIdIndex.Clear();
    }

    public static Tile Add(int centerId, List<int> hexFaceIds, List<int> neighborCenterIds, float height = 0f)
    {
        var tile = new Tile(centerId, hexFaceIds, neighborCenterIds, height, Repo.Count);
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