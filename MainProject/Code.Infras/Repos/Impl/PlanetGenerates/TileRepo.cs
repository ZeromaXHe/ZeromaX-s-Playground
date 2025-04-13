using Domains.Models.Entities.PlanetGenerates;
using Domains.Models.ValueObjects.PlanetGenerates;
using Domains.Repos.PlanetGenerates;
using Godot;
using Infras.Base;

namespace Infras.Repos.Impl.PlanetGenerates;

public class TileRepo : Repository<Tile>, ITileRepo
{
    #region 事件

    public event ITileRepo.RefreshChunkEvent? RefreshChunk;
    public event ITileRepo.UnitValidateLocationEvent? UnitValidateLocation;
    public event ITileRepo.RefreshTerrainShaderEvent? RefreshTerrainShader;
    public event ITileRepo.ViewElevationChangedEvent? ViewElevationChanged;
    private void RefreshSelfOnly(Tile tile) => RefreshChunk?.Invoke(tile.ChunkId);

    private void Refresh(Tile tile)
    {
        RefreshChunk?.Invoke(tile.ChunkId);
        foreach (var neighbor in GetNeighbors(tile))
            if (neighbor.ChunkId != tile.ChunkId)
                RefreshChunk?.Invoke(neighbor.ChunkId);
    }

    #endregion

    private readonly Dictionary<int, int> _centerIdIndex = new();

    public Tile Add(int centerId, int chunkId, Vector3 unitCentroid,
        List<int> hexFaceIds, List<int> neighborCenterIds) =>
        Add(id => new Tile(centerId, chunkId, unitCentroid, hexFaceIds, neighborCenterIds, id));

    protected override void AddHook(Tile tile) => _centerIdIndex.Add(tile.CenterId, tile.Id);
    protected override void DeleteHook(Tile entity) => _centerIdIndex.Remove(entity.CenterId);
    protected override void TruncateHook() => _centerIdIndex.Clear();

    public Tile? GetByCenterId(int centerId) =>
        _centerIdIndex.TryGetValue(centerId, out var tileId) ? GetById(tileId) : null;

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

    private int GetElevationDifference(Tile tile, int idx)
    {
        var diff = tile.Data.Elevation - GetNeighborByIdx(tile, idx)!.Data.Elevation;
        return diff >= 0 ? diff : -diff;
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

    #region 邻居

    public IEnumerable<Tile> GetNeighbors(Tile tile) => tile.NeighborCenterIds.Select(cid => GetByCenterId(cid)!);

    public Tile? GetNeighborByIdx(Tile tile, int idx) =>
        idx >= 0 && idx < tile.NeighborCenterIds.Count ? GetByCenterId(tile.NeighborCenterIds[idx]) : null;

    public int GetNeighborIdIdx(Tile tile, int neighborId) => tile.GetNeighborIdx(GetById(neighborId)!);

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

    public List<Tile> GetCornerNeighborsByIdx(Tile tile, int idx)
    {
        var neighbor1 = GetNeighborByIdx(tile, tile.PreviousIdx(idx))!;
        var neighbor2 = GetNeighborByIdx(tile, idx)!;
        return [neighbor1, neighbor2];
    }

    #endregion

    #region 河流

    private bool IsValidRiverDestination(Tile tile, Tile neighbor) =>
        tile.Data.Elevation >= neighbor.Data.Elevation || tile.Data.WaterLevel == neighbor.Data.Elevation;

    private void ValidateRivers(Tile tile)
    {
        if (tile.Data.HasOutgoingRiver
            && !IsValidRiverDestination(tile, GetNeighborByIdx(tile, tile.Data.OutgoingRiver)!))
            RemoveOutgoingRiver(tile);
        if (tile.Data.HasIncomingRiver
            && !IsValidRiverDestination(GetNeighborByIdx(tile, tile.Data.IncomingRiver)!, tile))
            RemoveIncomingRiver(tile);
    }

    private void RemoveOutgoingRiver(Tile tile)
    {
        if (!tile.Data.HasOutgoingRiver) return;
        var neighbor = GetNeighborByIdx(tile, tile.Data.Flags.RiverOutDirection())!;
        tile.Data = tile.Data with { Flags = tile.Data.Flags.Without(HexFlags.RiverOut) };
        neighbor.Data = neighbor.Data with { Flags = neighbor.Data.Flags.Without(HexFlags.RiverIn) };
        RefreshSelfOnly(neighbor);
        RefreshSelfOnly(tile);
    }

    private void RemoveIncomingRiver(Tile tile)
    {
        if (!tile.Data.HasIncomingRiver) return;
        var neighbor = GetNeighborByIdx(tile, tile.Data.Flags.RiverInDirection())!;
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
            GetNeighborByIdx(tile, tile.Data.Flags.RiverOutDirection())!.Id == riverToTile.Id) return;
        if (!IsValidRiverDestination(tile, riverToTile))
        {
            GD.Print($"SetOutgoingRiver tile {tile.Id} to {riverToTile.Id} failed because neighbor higher");
            return;
        }

        // GD.Print($"Setting Outgoing River from {tile.Id} to {riverToTile.Id}");
        RemoveOutgoingRiver(tile);
        if (tile.Data.HasIncomingRiver &&
            GetNeighborByIdx(tile, tile.Data.Flags.RiverInDirection())!.Id == riverToTile.Id)
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
        RefreshSelfOnly(tile);
        RefreshSelfOnly(riverToTile);
    }

    #endregion

    #region 道路

    public void AddRoad(Tile tile, Tile neighbor) => AddRoad(tile, tile.GetNeighborIdx(neighbor));

    private void AddRoad(Tile tile, int idx)
    {
        var neighbor = GetNeighborByIdx(tile, idx)!;
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

        var neighbor = GetNeighborByIdx(tile, idx)!;
        if (neighbor.Data.HasRoadThroughEdge(neighbor.GetNeighborIdx(tile)) != state)
        {
            var neighborIdx = neighbor.GetNeighborIdx(tile);
            var flags = state
                ? neighbor.Data.Flags.WithRoad(neighborIdx)
                : neighbor.Data.Flags.WithoutRoad(neighborIdx);
            neighbor.Data = neighbor.Data with { Flags = flags };
            RefreshSelfOnly(neighbor);
        }
    }

    #endregion
}