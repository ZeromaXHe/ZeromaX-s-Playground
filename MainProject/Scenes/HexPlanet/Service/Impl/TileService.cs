using System.Collections.Generic;
using System.Linq;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Enum;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Repo;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Struct;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util.HexSphereGrid;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service.Impl;

public class TileService(
    IChunkService chunkService,
    IFaceService faceService,
    ITileRepo tileRepo,
    IFaceRepo faceRepo,
    IPointRepo pointRepo) : ITileService
{
    public event ITileService.UnitValidateLocationEvent UnitValidateLocation;
    public event ITileService.RefreshTerrainShaderEvent RefreshTerrainShader;
    public event ITileService.ViewElevationChangedEvent ViewElevationChanged;

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

    public void SetElevation(Tile tile, int elevation)
    {
        if (tile.Data.Elevation == elevation) return;
        var originalViewElevation = tile.Data.ViewElevation;
        tile.Data = tile.Data with { Values = tile.Data.Values.WithElevation(elevation) };
        ValidateRivers(tile);
        if (!ValidateRoadsWater(tile))
            for (var i = 0; i < (tile.IsPentagon() ? 5 : 6); i++)
                if (tile.Data.Flags.HasRoad(i) && GetElevationDifference(tile, i) > 1)
                    SetRoad(tile, i, false);
        Refresh(tile);
        RefreshTerrainShader?.Invoke(tile.Id);
        if (tile.Data.ViewElevation != originalViewElevation)
            ViewElevationChanged?.Invoke(tile.Id);
        if (tile.UnitId != 0)
            UnitValidateLocation?.Invoke(tile.UnitId);
    }

    public void SetTerrainTypeIndex(Tile tile, int idx)
    {
        if (tile.Data.TerrainTypeIndex == idx) return;
        tile.Data = tile.Data with { Values = tile.Data.Values.WithTerrainTypeIndex(idx) };
        Refresh(tile);
        RefreshTerrainShader?.Invoke(tile.Id);
    }

    public void SetWaterLevel(Tile tile, int waterLevel)
    {
        if (tile.Data.WaterLevel == waterLevel) return;
        var originalViewElevation = tile.Data.ViewElevation;
        tile.Data = tile.Data with { Values = tile.Data.Values.WithWaterLevel(waterLevel) };
        ValidateRivers(tile);
        ValidateRoadsWater(tile);
        Refresh(tile);
        RefreshTerrainShader?.Invoke(tile.Id);
        if (tile.Data.ViewElevation != originalViewElevation)
            ViewElevationChanged?.Invoke(tile.Id);
    }

    public void SetUrbanLevel(Tile tile, int urbanLevel)
    {
        if (tile.Data.UrbanLevel == urbanLevel) return;
        tile.Data = tile.Data with { Values = tile.Data.Values.WithUrbanLevel(urbanLevel) };
        RefreshSelfOnly(tile);
    }

    public void SetFarmLevel(Tile tile, int farmLevel)
    {
        if (tile.Data.FarmLevel == farmLevel) return;
        tile.Data = tile.Data with { Values = tile.Data.Values.WithFarmLevel(farmLevel) };
        RefreshSelfOnly(tile);
    }

    public void SetPlantLevel(Tile tile, int plantLevel)
    {
        if (tile.Data.PlantLevel == plantLevel) return;
        tile.Data = tile.Data with { Values = tile.Data.Values.WithPlantLevel(plantLevel) };
        RefreshSelfOnly(tile);
    }

    public void SetWalled(Tile tile, bool walled)
    {
        if (tile.Data.Walled == walled) return;
        if (walled)
            tile.Data = tile.Data with { Flags = tile.Data.Flags.With(HexFlags.Walled) };
        else
            tile.Data = tile.Data with { Flags = tile.Data.Flags.Without(HexFlags.Walled) };
        Refresh(tile);
    }

    public void SetSpecialIndex(Tile tile, int specialIndex)
    {
        if (tile.Data.SpecialIndex == specialIndex && !tile.Data.HasRiver) return;
        tile.Data = tile.Data with { Values = tile.Data.Values.WithSpecialIndex(specialIndex) };
        RemoveRoads(tile);
        RefreshSelfOnly(tile);
    }

    public void SetUnitId(Tile tile, int unitId)
    {
        if (tile.UnitId == unitId) return;
        tile.UnitId = unitId;
    }

    #endregion

    private readonly VpTree<Vector3> _tilePointVpTree = new();

    public int? SearchNearestTileId(Vector3 pos)
    {
        _tilePointVpTree.Search(pos, 1, out var results, out _);
        return pointRepo.GetIdByPosition(results[0]); // Tile id 就是对应 Point id
    }

    public SphereAxial GetSphereAxial(Tile tile) => pointRepo.GetById(tile.CenterId).Coords;

    public float GetHeight(Tile tile) => (tile.Data.Elevation + GetPerturbHeight(tile)) * HexMetrics.UnitHeight;
    public float GetHeightById(int id) => GetHeight(GetById(id));

    private static float GetPerturbHeight(Tile tile) =>
        (HexMetrics.SampleNoise(tile.GetCentroid(HexMetrics.StandardRadius)).Y * 2f - 1f)
        * HexMetrics.ElevationPerturbStrength * HexMetrics.UnitHeight;

    public int GetElevationDifference(Tile tile, int idx)
    {
        var diff = tile.Data.Elevation - GetNeighborByIdx(tile, idx).Data.Elevation;
        return diff >= 0 ? diff : -diff;
    }

    public void InitTiles()
    {
        var time = Time.GetTicksMsec();
        foreach (var point in pointRepo.GetAll()) // 虽然没有排序，但好像默认也有顺序？不过不能依赖这一点
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
            // 将第一个面设置为最接近北方顺时针方向第一个的面
            var first = faces[0];
            var minAngle = Mathf.Tau;
            foreach (var face in faces)
            {
                var angle = center.Position.DirectionTo(face.Center).AngleTo(Vector3.Up);
                if (angle < minAngle)
                {
                    minAngle = angle;
                    first = face;
                }
            }

            // 第二个面必须保证和第一个面形成顺时针方向，从而保证所有都是顺时针
            var second =
                faces.First(face =>
                    face.Id != first.Id
                    && face.IsAdjacentTo(first)
                    && Math3dUtil.IsRightVSeq(Vector3.Zero, center.Position, first.Center, face.Center));
            var orderedList = new List<Face> { first, second };
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

    private bool IsValidRiverDestination(Tile tile, Tile neighbor) =>
        tile.Data.Elevation >= neighbor.Data.Elevation || tile.Data.WaterLevel == neighbor.Data.Elevation;

    private void ValidateRivers(Tile tile)
    {
        if (tile.Data.HasOutgoingRiver
            && !IsValidRiverDestination(tile, GetNeighborByIdx(tile, tile.Data.OutgoingRiver)))
            RemoveOutgoingRiver(tile);
        if (tile.Data.HasIncomingRiver
            && !IsValidRiverDestination(GetNeighborByIdx(tile, tile.Data.IncomingRiver), tile))
            RemoveIncomingRiver(tile);
    }

    private void RemoveOutgoingRiver(Tile tile)
    {
        if (!tile.Data.HasOutgoingRiver) return;
        var neighbor = GetNeighborByIdx(tile, tile.Data.Flags.RiverOutDirection());
        tile.Data = tile.Data with { Flags = tile.Data.Flags.Without(HexFlags.RiverOut) };
        neighbor.Data = neighbor.Data with { Flags = neighbor.Data.Flags.Without(HexFlags.RiverIn) };
        RefreshSelfOnly(neighbor);
        RefreshSelfOnly(tile);
    }

    private void RemoveIncomingRiver(Tile tile)
    {
        if (!tile.Data.HasIncomingRiver) return;
        var neighbor = GetNeighborByIdx(tile, tile.Data.Flags.RiverInDirection());
        tile.Data = tile.Data with { Flags = tile.Data.Flags.Without(HexFlags.RiverIn) };
        neighbor.Data = neighbor.Data with { Flags = neighbor.Data.Flags.Without(HexFlags.RiverOut) };
        RefreshSelfOnly(neighbor);
        RefreshSelfOnly(tile);
    }

    public void RemoveRiver(Tile tile)
    {
        RemoveOutgoingRiver(tile);
        RemoveIncomingRiver(tile);
    }

    public void SetOutgoingRiver(Tile tile, Tile riverToTile)
    {
        if (tile.Data.HasOutgoingRiver &&
            GetNeighborByIdx(tile, tile.Data.Flags.RiverOutDirection()).Id == riverToTile.Id) return;
        if (!IsValidRiverDestination(tile, riverToTile))
        {
            GD.Print($"SetOutgoingRiver tile {tile.Id} to {riverToTile.Id} failed because neighbor higher");
            return;
        }

        GD.Print($"Setting Outgoing River from {tile.Id} to {riverToTile.Id}");
        RemoveOutgoingRiver(tile);
        if (tile.Data.HasIncomingRiver &&
            GetNeighborByIdx(tile, tile.Data.Flags.RiverInDirection()).Id == riverToTile.Id)
            RemoveIncomingRiver(tile);
        tile.Data = new HexTileData
        {
            Flags = tile.Data.Flags.WithRiverOut(tile.GetNeighborIdx(riverToTile)),
            Values = tile.Data.Values.WithSpecialIndex(0)
        };
        RemoveIncomingRiver(riverToTile);
        riverToTile.Data = new HexTileData
        {
            Flags = riverToTile.Data.Flags.WithRiverIn(riverToTile.GetNeighborIdx(tile)),
            Values = riverToTile.Data.Values.WithSpecialIndex(0)
        };
        SetRoad(tile, tile.GetNeighborIdx(riverToTile), false);
    }

    #endregion

    #region 道路

    public void AddRoad(Tile tile, Tile neighbor) => AddRoad(tile, tile.GetNeighborIdx(neighbor));

    private void AddRoad(Tile tile, int idx)
    {
        var neighbor = GetNeighborByIdx(tile, idx);
        if (!tile.Data.HasRoadThroughEdge(idx)
            && !tile.Data.HasRiverThroughEdge(idx)
            && !tile.Data.IsSpecial && !neighbor.Data.IsSpecial
            && !tile.Data.IsUnderwater && !neighbor.Data.IsUnderwater // 不在水下生成道路
            && GetElevationDifference(tile, idx) <= 1)
            SetRoad(tile, idx, true);
    }

    private bool ValidateRoadsWater(Tile tile)
    {
        if (tile.Data.IsUnderwater)
            RemoveRoads(tile);
        return tile.Data.IsUnderwater;
    }

    public void RemoveRoads(Tile tile)
    {
        for (var i = 0; i < (tile.IsPentagon() ? 5 : 6); i++)
        {
            if (tile.Data.HasRoadThroughEdge(i))
                SetRoad(tile, i, false);
        }
    }

    private void SetRoad(Tile tile, int idx, bool state)
    {
        if (tile.Data.HasRoadThroughEdge(idx) != state)
        {
            var flags = state ? tile.Data.Flags.WithRoad(idx) : tile.Data.Flags.WithoutRoad(idx);
            tile.Data = tile.Data with { Flags = flags };
            RefreshSelfOnly(tile);
        }

        var neighbor = GetNeighborByIdx(tile, idx);
        if (neighbor.Data.HasRoadThroughEdge(neighbor.GetNeighborIdx(tile)) != state)
        {
            var neighborIdx = neighbor.GetNeighborIdx(tile);
            var flags = state ? neighbor.Data.Flags.WithRoad(neighborIdx) : neighbor.Data.Flags.WithoutRoad(neighborIdx);
            neighbor.Data = neighbor.Data with { Flags = flags };
            RefreshSelfOnly(neighbor);
        }
    }

    #endregion

    #region 水面

    public Vector3 GetFirstWaterCorner(Tile tile, int idx, float radius = 1f, float size = 1f) =>
        GetFirstCorner(tile, idx, radius, size * HexMetrics.WaterFactor);

    public Vector3 GetSecondWaterCorner(Tile tile, int idx, float radius = 1f, float size = 1f) =>
        GetSecondCorner(tile, idx, radius, size * HexMetrics.WaterFactor);

    #endregion

    public void UpdateTileLabel(Tile tile, string text)
    {
        var chunk = chunkService.GetById(tile.ChunkId);
        chunkService.RefreshTileLabel(chunk, tile.Id, text);
    }
}