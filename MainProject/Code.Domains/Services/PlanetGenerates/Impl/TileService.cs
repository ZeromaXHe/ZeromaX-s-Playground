using Commons.Utils;
using Domains.Models.Entities.PlanetGenerates;
using Domains.Models.ValueObjects.PlanetGenerates;
using Domains.Repos.PlanetGenerates;
using Godot;

namespace Domains.Services.PlanetGenerates.Impl;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-02-24 13:35
public class TileService(
    IChunkService chunkService,
    IFaceRepo faceRepo,
    IPointService pointService,
    IPointRepo pointRepo,
    IPlanetSettingService planetSettingService,
    INoiseService noiseService,
    ITileRepo tileRepo) : ITileService
{
    private readonly VpTree<Vector3> _tilePointVpTree = new();

    public int? SearchNearestTileId(Vector3 pos)
    {
        // 存储的 Point 是单位球上，所以 pos 单位化减小误差
        _tilePointVpTree.Search(pos.Normalized(), 1, out var results, out _);
        var pointId = pointRepo.GetIdByPosition(false, results[0]);
        if (pointId == null) return null;
        return tileRepo.GetByCenterId((int)pointId)!.Id;
    }

    public float GetHeight(Tile tile) =>
        (tile.Data.Elevation + GetPerturbHeight(tile)) * planetSettingService.UnitHeight;

    public float GetOverrideHeight(Tile tile, HexTileDataOverrider tileDataOverrider) =>
        (tileDataOverrider.Elevation(tile) + GetPerturbHeight(tile) + 0.05f) * planetSettingService.UnitHeight;

    public float GetHeightById(int id) => GetHeight(tileRepo.GetById(id)!);

    private float GetPerturbHeight(Tile tile) =>
        (noiseService.SampleNoise(tile.GetCentroid(HexMetrics.StandardRadius)).Y * 2f - 1f)
        * noiseService.ElevationPerturbStrength * planetSettingService.UnitHeight;

    public void InitTiles()
    {
        var time = Time.GetTicksMsec();
        pointService.InitPointsAndFaces(false, planetSettingService.Divisions);
        foreach (var point in pointRepo.GetAllByChunky(false)) // 虽然没有排序，但好像默认也有顺序？不过不能依赖这一点
        {
            var hexFaces = faceRepo.GetOrderedFaces(point);
            var neighborCenters = pointRepo.GetNeighborCenterIds(hexFaces, point)
                .Select(c => c.Id)
                .ToList();
            var chunk = chunkService.SearchNearest(point.Position)!;
            var tile = Add(point.Id, chunk.Id, hexFaces.Select(f => f.Id).ToList(), neighborCenters);
            chunk.TileIds.Add(tile.Id);
        }

        var time2 = Time.GetTicksMsec();
        GD.Print($"InitTiles cost: {time2 - time} ms");
        time = time2;

        _tilePointVpTree.Create(pointRepo.GetAllByChunky(false)
                .Select(p => p.Position)
                .ToArray(),
            (p0, p1) => p0.DistanceTo(p1));
        time2 = Time.GetTicksMsec();
        GD.Print($"_tilePointVpTree Create cost: {time2 - time} ms");
    }

    private Tile Add(int centerId, int chunkId, List<int> hexFaceIds, List<int> neighborCenterIds) =>
        tileRepo.Add(centerId, chunkId, InitUnitCentroid(hexFaceIds), hexFaceIds, neighborCenterIds);

    // 初始计算单位重心（顶点坐标的算术平均）
    private Vector3 InitUnitCentroid(List<int> hexFaceIds) =>
        hexFaceIds
            .Select(id => faceRepo.GetById(id)!.Center.Normalized())
            .Aggregate((v1, v2) => v1 + v2) / hexFaceIds.Count;

    public IEnumerable<Vector3> GetCorners(Tile tile, float radius, float size = 1f) =>
        from faceId in tile.HexFaceIds
        select faceRepo.GetById(faceId)
        into face
        select tile.UnitCentroid.Lerp(face.Center, size)
        into pos
        select Math3dUtil.ProjectToSphere(pos, radius);

    public Vector3 GetCornerByFaceId(Tile tile, int id, float radius = 1f, float size = 1f) =>
        Math3dUtil.ProjectToSphere(tile.UnitCentroid.Lerp(faceRepo.GetById(id)!.Center, size), radius);

    public Vector3 GetFirstCorner(Tile tile, int idx, float radius = 1f, float size = 1f) =>
        GetCornerByFaceId(tile, tile.HexFaceIds[idx], radius, size);

    public Vector3 GetSecondCorner(Tile tile, int idx, float radius = 1f, float size = 1f) =>
        GetFirstCorner(tile, tile.NextIdx(idx), radius, size);

    public Vector3 GetFirstSolidCorner(Tile tile, int idx, float radius = 1f, float size = 1f) =>
        GetFirstCorner(tile, idx, radius, size * HexMetrics.SolidFactor);

    public Vector3 GetSecondSolidCorner(Tile tile, int idx, float radius = 1f, float size = 1f) =>
        GetSecondCorner(tile, idx, radius, size * HexMetrics.SolidFactor);

    public Vector3 GetEdgeMiddle(Tile tile, int idx, float radius = 1f, float size = 1f)
    {
        var corner1 = GetFirstCorner(tile, idx, radius, size);
        var corner2 = GetFirstCorner(tile, tile.NextIdx(idx), radius, size);
        return corner1.Lerp(corner2, 0.5f);
    }

    public Vector3 GetSolidEdgeMiddle(Tile tile, int idx, float radius = 1f, float size = 1f) =>
        GetEdgeMiddle(tile, idx, radius, size * HexMetrics.SolidFactor);

    private List<Vector3>? GetNeighborCommonCorners(Tile tile, Tile neighbor, float radius = 1f)
    {
        var idx = tile.NeighborCenterIds.FindIndex(ncId => ncId == neighbor.CenterId);
        if (idx == -1) return null;
        return
        [
            GetFirstCorner(tile, idx, radius),
            GetFirstCorner(tile, tile.NextIdx(idx), radius)
        ];
    }

    #region 水面

    public Vector3 GetFirstWaterCorner(Tile tile, int idx, float radius = 1f, float size = 1f) =>
        GetFirstCorner(tile, idx, radius, size * HexMetrics.WaterFactor);

    public Vector3 GetSecondWaterCorner(Tile tile, int idx, float radius = 1f, float size = 1f) =>
        GetSecondCorner(tile, idx, radius, size * HexMetrics.WaterFactor);

    #endregion
}