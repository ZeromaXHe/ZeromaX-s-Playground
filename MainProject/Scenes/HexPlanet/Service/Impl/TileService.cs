using System.Collections.Generic;
using System.Linq;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Repo;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service.Impl;

public class TileService(
    IChunkService chunkService,
    IFaceService faceService,
    ITileRepo tileRepo,
    IFaceRepo faceRepo,
    IPointRepo pointRepo) : ITileService
{
    #region 透传存储库方法

    public Tile GetById(int id) => tileRepo.GetById(id);
    public Tile GetByCenterId(int centerId) => tileRepo.GetByCenterId(centerId);
    public int GetCount() => tileRepo.GetCount();

    #endregion

    // 仿 setter 注入写法：
    // private readonly Lazy<ITileRepo> _tileRepo = new(() => (ITileRepo)Context.GetBean(Singleton.TileRepo));
    private readonly VpTree<Vector3> _tilePointVpTree = new();

    public int? SearchNearestTileId(Vector3 pos)
    {
        _tilePointVpTree.Search(pos, 1, out var results, out _);
        return pointRepo.GetIdByPosition(results[0]); // Tile id 就是对应 Point id
    }

    public float UnitHeight { get; set; } = 1f;

    public float GetHeight(Tile tile) => (tile.Elevation + GetPerturbHeight(tile)) * UnitHeight;
    public float GetHeightById(int id) => GetHeight(GetById(id));

    public float SetHeight(Tile tile, float height) =>
        tile.Elevation = Mathf.Clamp((int)((height - GetPerturbHeight(tile)) / UnitHeight),
            0, HexMetrics.ElevationStep);

    private float GetPerturbHeight(Tile tile)
    {
        var radius = UnitHeight / HexMetrics.MaxHeightRadiusRatio * HexMetrics.ElevationStep;
        return HexMetrics.SampleNoise(tile.GetCentroid(radius)).Y * 2f * HexMetrics.ElevationPerturbStrength;
    }

    public void ClearData() => tileRepo.Truncate();

    public void InitTiles()
    {
        var time = Time.GetTicksMsec();
        foreach (var point in pointRepo.GetAll())
        {
            var hexFaces = GetOrderedFaces(point);
            var neighborCenters = GetNeighbourCenterIds(hexFaces, point)
                .Select(c => c.Id)
                .ToList();
            var chunk = chunkService.SearchNearest(point.Position);
            var tile = Add(point.Id, chunk.Id, hexFaces.Select(f => f.Id).ToList(), neighborCenters);
            chunk.TileIds.Add(tile.Id);
        }

        var time2 = Time.GetTicksMsec();
        GD.Print($"InitTiles cost: {time2 - time} ms");
        time = time2;

        _tilePointVpTree.Create(tileRepo.GetAll().Select(p => pointRepo.GetById(p.CenterId).Position).ToArray(),
            (p0, p1) => p0.DistanceTo(p1));
        time2 = Time.GetTicksMsec();
        GD.Print($"_tilePointVpTree Create cost: {time2 - time} ms");

        return;

        List<Face> GetOrderedFaces(Point center)
        {
            var faces = center.FaceIds.Select(faceRepo.GetById).ToList();
            if (faces.Count == 0) return faces;
            // 第二个面必须保证和第一个面形成顺时针方向，从而保证所有都是顺时针
            var second =
                faces.First(face =>
                    face.Id != faces[0].Id
                    && face.IsAdjacentTo(faces[0])
                    && Math3dUtil.IsRightVSeq(Vector3.Zero, center.Position, faces[0].Center, face.Center));
            var orderedList = new List<Face> { faces[0], second };
            var currentFace = orderedList[1];
            while (orderedList.Count < faces.Count)
            {
                var existingIds = orderedList.Select(face => face.Id).ToList();
                var neighbour = faces.First(face =>
                    !existingIds.Contains(face.Id) && face.IsAdjacentTo(currentFace));
                currentFace = neighbour;
                orderedList.Add(currentFace);
            }

            return orderedList;
        }

        List<Point> GetNeighbourCenterIds(List<Face> hexFaces, Point center)
        {
            var neighbourCenters = new List<Point>();
            foreach (var p in
                     from face in hexFaces
                     from p in faceService.GetOtherPoints(face, center)
                     where !neighbourCenters.Contains(p)
                     select p)
                neighbourCenters.Add(p);
            return neighbourCenters;
        }
    }

    private Tile Add(int centerId, int chunkId, List<int> hexFaceIds, List<int> neighborCenterIds) =>
        tileRepo.Add(centerId, chunkId, InitUnitCentroid(hexFaceIds), hexFaceIds, neighborCenterIds);

    // 初始计算单位重心（顶点坐标的算术平均）
    private Vector3 InitUnitCentroid(List<int> hexFaceIds) =>
        hexFaceIds
            .Select(id => faceRepo.GetById(id).Center.Normalized())
            .Aggregate((v1, v2) => v1 + v2) / hexFaceIds.Count;

    public IEnumerable<Vector3> GetCorners(Tile tile, float radius, float size = 1f) =>
        from faceId in tile.HexFaceIds
        select faceRepo.GetById(faceId)
        into face
        select tile.UnitCentroid.Lerp(face.Center, size)
        into pos
        select Math3dUtil.ProjectToSphere(pos, radius);

    public Vector3 GetCorner(Tile tile, int idx, float radius = 1f, float size = 1f) =>
        Math3dUtil.ProjectToSphere(tile.UnitCentroid.Lerp(faceRepo.GetById(tile.HexFaceIds[idx]).Center, size), radius);

    public Vector3 GetCornerByFaceId(Tile tile, int id, float radius = 1f, float size = 1f) =>
        Math3dUtil.ProjectToSphere(tile.UnitCentroid.Lerp(faceRepo.GetById(id).Center, size), radius);

    public Vector3 GetCenter(Tile tile, float radius) =>
        Math3dUtil.ProjectToSphere(pointRepo.GetById(tile.CenterId).Position, radius);

    public IEnumerable<Tile> GetNeighbors(Tile tile) => tile.NeighborCenterIds.Select(tileRepo.GetByCenterId);

    public Tile GetNeighborByIdx(Tile tile, int idx) =>
        idx >= 0 && idx < tile.NeighborCenterIds.Count ? tileRepo.GetByCenterId(tile.NeighborCenterIds[idx]) : null;

    public IEnumerable<Tile> GetTilesInDistance(Tile tile, int dist)
    {
        if (dist == 0) return [tile];
        HashSet<Tile> resultSet = [tile];
        List<Tile> preRing = [tile];
        List<Tile> afterRing = [];
        for (var i = 0; i < dist; i++)
        {
            afterRing.AddRange(
                from t in preRing
                from neighbor in GetNeighbors(t)
                where resultSet.Add(neighbor)
                select neighbor);
            (preRing, afterRing) = (afterRing, preRing);
            afterRing.Clear();
        }

        return resultSet;
    }

    public List<Vector3> GetNeighborCommonCorners(Tile tile, Tile neighbor, float radius = 1f)
    {
        var idx = tile.NeighborCenterIds.FindIndex(ncId => ncId == neighbor.CenterId);
        if (idx == -1) return null;
        return
        [
            GetCorner(tile, idx, radius),
            GetCorner(tile, (idx + 1) % tile.HexFaceIds.Count, radius)
        ];
    }

    public Tile GetNeighborByDirection(Tile tile, int idx1, int idx2)
    {
        if (idx2 == (idx1 + 1) % tile.HexFaceIds.Count) return GetNeighborByIdx(tile, idx2);
        return idx1 == (idx2 + 1) % tile.HexFaceIds.Count ? GetNeighborByIdx(tile, idx1) : null;
    }

    public List<Tile> GetNeighborsByDirection(Tile tile, int idx, int filterNeighborId = -1)
    {
        var res = new List<Tile>();
        var neighbor1 = GetNeighborByIdx(tile, idx);
        var neighbor2 = GetNeighborByIdx(tile, idx == tile.HexFaceIds.Count - 1 ? 0 : idx + 1);
        if (neighbor1.Id != filterNeighborId)
            res.Add(neighbor1);
        if (neighbor2.Id != filterNeighborId)
            res.Add(neighbor2);
        return res;
    }
}