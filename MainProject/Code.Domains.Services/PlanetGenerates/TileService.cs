using Commons.Utils;
using Domains.Models.Entities.PlanetGenerates;
using Domains.Models.ValueObjects.PlanetGenerates;
using Domains.Services.Abstractions.PlanetGenerates;
using Godot;
using Infras.Readers.Abstractions.Nodes.Singletons;
using Infras.Writers.Abstractions.PlanetGenerates;

namespace Domains.Services.PlanetGenerates;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-02-24 13:35
public class TileService(
    IChunkService chunkService,
    IFaceRepo faceRepo,
    IPointService pointService,
    IPointRepo pointRepo,
    IHexPlanetManagerRepo hexPlanetManagerRepo,
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

    public void InitTiles()
    {
        var time = Time.GetTicksMsec();
        pointService.InitPointsAndFaces(false, hexPlanetManagerRepo.Divisions);
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
        tileRepo.Add(centerId, chunkId, InitUnitCentroid(hexFaceIds), InitUnitCorners(hexFaceIds),
            hexFaceIds, neighborCenterIds);

    // 初始计算单位重心（顶点坐标的算术平均）
    private Vector3 InitUnitCentroid(List<int> hexFaceIds) =>
        hexFaceIds
            .Select(id => faceRepo.GetById(id)!.Center.Normalized())
            .Aggregate((v1, v2) => v1 + v2) / hexFaceIds.Count;

    // 初始计算单位顶点坐标
    private List<Vector3> InitUnitCorners(List<int> hexFaceIds) =>
        hexFaceIds
            .Select(faceId => faceRepo.GetById(faceId)!.Center)
            .ToList();
    
    public void EditTiles(Tile tile, HexTileDataOverrider tileOverrider, bool isDrag, Tile? previousTile, Tile? dragTile)
    {
        foreach (var t in tileRepo.GetTilesInDistance(tile, tileOverrider.BrushSize))
            EditTile(t, tileOverrider, isDrag, previousTile, dragTile);
    }

    private void EditTile(Tile tile, HexTileDataOverrider tileOverrider, bool isDrag, Tile? previousTile, Tile? dragTile)
    {
        if (tileOverrider.ApplyTerrain)
            tileRepo.SetTerrainTypeIndex(tile, tileOverrider.ActiveTerrain);
        if (tileOverrider.ApplyElevation)
            tileRepo.SetElevation(tile, tileOverrider.ActiveElevation);
        if (tileOverrider.ApplyWaterLevel)
            tileRepo.SetWaterLevel(tile, tileOverrider.ActiveWaterLevel);
        if (tileOverrider.ApplySpecialIndex)
            tileRepo.SetSpecialIndex(tile, tileOverrider.ActiveSpecialIndex);
        if (tileOverrider.ApplyUrbanLevel)
            tileRepo.SetUrbanLevel(tile, tileOverrider.ActiveUrbanLevel);
        if (tileOverrider.ApplyFarmLevel)
            tileRepo.SetFarmLevel(tile, tileOverrider.ActiveFarmLevel);
        if (tileOverrider.ApplyPlantLevel)
            tileRepo.SetPlantLevel(tile, tileOverrider.ActivePlantLevel);
        if (tileOverrider.RiverMode == OptionalToggle.No)
            tileRepo.RemoveRiver(tile);
        if (tileOverrider.RoadMode == OptionalToggle.No)
            tileRepo.RemoveRoads(tile);
        if (tileOverrider.WalledMode != OptionalToggle.Ignore)
            tileRepo.SetWalled(tile, tileOverrider.WalledMode == OptionalToggle.Yes);
        if (isDrag)
        {
            if (tileOverrider.RiverMode == OptionalToggle.Yes)
                tileRepo.SetOutgoingRiver(previousTile!, dragTile!);
            if (tileOverrider.RoadMode == OptionalToggle.Yes)
                tileRepo.AddRoad(previousTile!, dragTile!);
        }
    }
}