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
    public event ITileService.UpdateTileAStarEvent UpdateTileAStar;
    public event ITileService.UnitValidateLocationEvent UnitValidateLocation;

    private void Refresh(Tile tile)
    {
        chunkService.Refresh(chunkService.GetById(tile.ChunkId));
        foreach (var neighbor in GetNeighbors(tile))
            if (neighbor.ChunkId != tile.ChunkId)
                chunkService.Refresh(chunkService.GetById(neighbor.ChunkId));
    }

    private void RefreshSelfOnly(Tile tile)
    {
        chunkService.Refresh(chunkService.GetById(tile.ChunkId));
    }

    #region 透传存储库方法

    public Tile GetById(int id) => tileRepo.GetById(id);
    public Tile GetByCenterId(int centerId) => tileRepo.GetByCenterId(centerId);
    public int GetCount() => tileRepo.GetCount();
    public IEnumerable<Tile> GetAll() => tileRepo.GetAll();
    public void Truncate() => tileRepo.Truncate();

    #endregion

    #region 修改 Tile 属性的方法（相当于 Update）

    public void SetHeight(Tile tile, float height) =>
        SetElevation(tile,
            Mathf.Clamp((int)((height - GetPerturbHeight(tile)) / UnitHeight),
                0, HexMetrics.ElevationStep));

    public void SetElevation(Tile tile, int elevation)
    {
        if (tile.Elevation == elevation) return;
        tile.Elevation = elevation;
        ValidateRivers(tile);
        for (var i = 0; i < tile.Roads.Length; i++)
            if (tile.Roads[i] && GetElevationDifference(tile, i) > 1)
                SetRoad(tile, i, false);
        Refresh(tile);
        UpdateTileAStar?.Invoke(tile.Id);
        if (tile.UnitId != 0)
            UnitValidateLocation?.Invoke(tile.UnitId);
    }

    public void SetTerrainTypeIndex(Tile tile, int idx)
    {
        if (tile.TerrainTypeIndex == idx) return;
        tile.TerrainTypeIndex = idx;
        Refresh(tile);
    }

    public void SetWaterLevel(Tile tile, int waterLevel)
    {
        if (tile.WaterLevel == waterLevel) return;
        tile.WaterLevel = waterLevel;
        ValidateRivers(tile);
        Refresh(tile);
        UpdateTileAStar?.Invoke(tile.Id);
    }

    public void SetUrbanLevel(Tile tile, int urbanLevel)
    {
        if (tile.UrbanLevel == urbanLevel) return;
        tile.UrbanLevel = urbanLevel;
        RefreshSelfOnly(tile);
    }

    public void SetFarmLevel(Tile tile, int farmLevel)
    {
        if (tile.FarmLevel == farmLevel) return;
        tile.FarmLevel = farmLevel;
        RefreshSelfOnly(tile);
    }

    public void SetPlantLevel(Tile tile, int plantLevel)
    {
        if (tile.PlantLevel == plantLevel) return;
        tile.PlantLevel = plantLevel;
        RefreshSelfOnly(tile);
    }

    public void SetWalled(Tile tile, bool walled)
    {
        if (tile.Walled == walled) return;
        tile.Walled = walled;
        Refresh(tile);
        UpdateTileAStar?.Invoke(tile.Id);
    }

    public void SetSpecialIndex(Tile tile, int specialIndex)
    {
        if (tile.SpecialIndex == specialIndex && !tile.HasRiver) return;
        tile.SpecialIndex = specialIndex;
        RemoveRoads(tile);
        RefreshSelfOnly(tile);
    }

    public void SetUnitId(Tile tile, int unitId)
    {
        if (tile.UnitId == unitId) return;
        tile.UnitId = unitId;
        UpdateTileAStar?.Invoke(tile.Id);
    }

    #endregion

    private readonly VpTree<Vector3> _tilePointVpTree = new();

    public int? SearchNearestTileId(Vector3 pos)
    {
        _tilePointVpTree.Search(pos, 1, out var results, out _);
        return pointRepo.GetIdByPosition(results[0]); // Tile id 就是对应 Point id
    }

    public float UnitHeight { get; set; } = 1f;

    public float GetHeight(Tile tile) => (tile.Elevation + GetPerturbHeight(tile)) * UnitHeight;
    public float GetHeightById(int id) => GetHeight(GetById(id));

    private float GetPerturbHeight(Tile tile)
    {
        var radius = UnitHeight / HexMetrics.MaxHeightRadiusRatio * HexMetrics.ElevationStep;
        return (HexMetrics.SampleNoise(tile.GetCentroid(radius)).Y * 2f - 1f)
               * HexMetrics.ElevationPerturbStrength * UnitHeight;
    }

    public int GetElevationDifference(Tile tile, int idx)
    {
        var diff = tile.Elevation - GetNeighborByIdx(tile, idx).Elevation;
        return diff >= 0 ? diff : -diff;
    }

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
            return (
                from face in hexFaces
                select faceService.GetRightOtherPoints(face, center)
            ).ToList();
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

    public Vector3 GetCornerByFaceId(Tile tile, int id, float radius = 1f, float size = 1f) =>
        Math3dUtil.ProjectToSphere(tile.UnitCentroid.Lerp(faceRepo.GetById(id).Center, size), radius);

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

    public Vector3 GetCenter(Tile tile, float radius) =>
        Math3dUtil.ProjectToSphere(pointRepo.GetById(tile.CenterId).Position, radius);

    #region 邻居

    public IEnumerable<Tile> GetNeighbors(Tile tile) => tile.NeighborCenterIds.Select(tileRepo.GetByCenterId);

    public Tile GetNeighborByIdx(Tile tile, int idx) =>
        idx >= 0 && idx < tile.NeighborCenterIds.Count ? tileRepo.GetByCenterId(tile.NeighborCenterIds[idx]) : null;

    public int GetNeighborIdIdx(Tile tile, int neighborId) => tile.GetNeighborIdx(GetById(neighborId));

    public bool IsNeighbor(Tile tile1, Tile tile2) =>
        tile1.NeighborCenterIds.Contains(tile2.CenterId);

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
            GetFirstCorner(tile, idx, radius),
            GetFirstCorner(tile, tile.NextIdx(idx), radius)
        ];
    }

    public List<Tile> GetCornerNeighborsByIdx(Tile tile, int idx)
    {
        var neighbor1 = GetNeighborByIdx(tile, tile.PreviousIdx(idx));
        var neighbor2 = GetNeighborByIdx(tile, idx);
        return [neighbor1, neighbor2];
    }

    #endregion

    #region 河流

    private void ValidateRivers(Tile tile)
    {
        if (tile.HasOutgoingRiver && !tile.IsValidRiverDestination(GetById(tile.OutgoingRiverNId)))
            RemoveOutgoingRiver(tile);
        if (tile.HasIncomingRiver && !GetById(tile.IncomingRiverNId).IsValidRiverDestination(tile))
            RemoveIncomingRiver(tile);
    }

    private void RemoveOutgoingRiver(Tile tile)
    {
        if (!tile.HasOutgoingRiver) return;
        tile.HasOutgoingRiver = false;
        RefreshSelfOnly(tile);
        var neighbor = GetById(tile.OutgoingRiverNId);
        neighbor.HasIncomingRiver = false;
        RefreshSelfOnly(neighbor);
    }

    private void RemoveIncomingRiver(Tile tile)
    {
        if (!tile.HasIncomingRiver) return;
        tile.HasIncomingRiver = false;
        RefreshSelfOnly(tile);
        var neighbor = GetById(tile.IncomingRiverNId);
        neighbor.HasOutgoingRiver = false;
        RefreshSelfOnly(neighbor);
    }

    public void RemoveRiver(Tile tile)
    {
        RemoveOutgoingRiver(tile);
        RemoveIncomingRiver(tile);
    }

    public void SetOutgoingRiver(Tile tile, Tile riverToTile)
    {
        if (tile.HasOutgoingRiver && tile.OutgoingRiverNId == riverToTile.Id) return;
        if (!tile.IsValidRiverDestination(riverToTile))
        {
            GD.Print($"SetOutgoingRiver tile {tile.Id} to {riverToTile.Id} failed because neighbor higher");
            return;
        }

        GD.Print($"Setting Outgoing River from {tile.Id} to {riverToTile.Id}");
        RemoveOutgoingRiver(tile);
        if (tile.HasIncomingRiver && tile.IncomingRiverNId == riverToTile.Id)
            RemoveIncomingRiver(tile);
        tile.HasOutgoingRiver = true;
        tile.OutgoingRiverNId = riverToTile.Id;
        tile.SpecialIndex = 0;
        RemoveIncomingRiver(riverToTile);
        riverToTile.HasIncomingRiver = true;
        riverToTile.IncomingRiverNId = tile.Id;
        riverToTile.SpecialIndex = 0;
        SetRoad(tile, tile.GetNeighborIdx(riverToTile), false);
    }

    public float GetStreamBedHeight(Tile tile) => (tile.Elevation + HexMetrics.StreamBedElevationOffset) * UnitHeight;

    public bool HasRiverThroughEdge(Tile tile, int idx) =>
        tile.HasRiverToNeighbor(GetNeighborByIdx(tile, idx).Id);

    public float GetRiverSurfaceHeight(Tile tile) =>
        (tile.Elevation + HexMetrics.WaterElevationOffset) * UnitHeight;

    public int GetRiverBeginOrEndIdx(Tile tile) =>
        tile.HasRiverBeginOrEnd ? tile.GetNeighborIdx(GetById(tile.RiverBeginOrEndNId)) : -1;

    #endregion

    #region 道路

    public void AddRoad(Tile tile, Tile neighbor) => AddRoad(tile, tile.GetNeighborIdx(neighbor));

    private void AddRoad(Tile tile, int idx)
    {
        if (!tile.Roads[idx]
            && !HasRiverThroughEdge(tile, idx)
            && !tile.IsSpecial && !GetNeighborByIdx(tile, idx).IsSpecial
            && GetElevationDifference(tile, idx) <= 1)
            SetRoad(tile, idx, true);
    }

    public void RemoveRoads(Tile tile)
    {
        for (var i = 0; i < tile.Roads.Length; i++)
        {
            if (tile.Roads[i])
                SetRoad(tile, i, false);
        }
    }

    private void SetRoad(Tile tile, int idx, bool state)
    {
        tile.Roads[idx] = state;
        var neighbor = GetNeighborByIdx(tile, idx);
        neighbor.Roads[neighbor.GetNeighborIdx(tile)] = state;
        RefreshSelfOnly(neighbor);
        RefreshSelfOnly(tile);
    }

    #endregion

    #region 水面

    public float GetWaterSurfaceHeight(Tile tile) =>
        (tile.WaterLevel + HexMetrics.WaterElevationOffset) * UnitHeight;

    public Vector3 GetFirstWaterCorner(Tile tile, int idx, float radius = 1f, float size = 1f) =>
        GetFirstCorner(tile, idx, radius, size * HexMetrics.WaterFactor);

    public Vector3 GetSecondWaterCorner(Tile tile, int idx, float radius = 1f, float size = 1f) =>
        GetSecondCorner(tile, idx, radius, size * HexMetrics.WaterFactor);

    #endregion

    public float GetWallHeight() => UnitHeight * HexMetrics.WallHeight;
    public float GetWallThickness() => UnitHeight * HexMetrics.WallThickness;

    public void UpdateTileLabel(Tile tile, string text)
    {
        var chunk = chunkService.GetById(tile.ChunkId);
        chunkService.RefreshTileLabel(chunk, tile.Id, text);
    }
}