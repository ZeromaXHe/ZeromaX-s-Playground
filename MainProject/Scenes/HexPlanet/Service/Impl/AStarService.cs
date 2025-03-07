using System.Collections.Generic;
using System.Linq;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Enum;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Script;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service.Impl;

public class AStarService(ITileService tileService) : IAStarService
{
    private TileAStar _aStar;

    public void Init()
    {
        _aStar ??= new TileAStar(tileService);
        _aStar.Clear();
        // 初始化所有可达地块
        foreach (var tile in tileService.GetAll())
        {
            _aStar.AddPoint(tile.Id, tile.UnitCentroid);
            if (!IsPathValidTile(tile)) _aStar.SetPointDisabled(tile.Id);
        }

        // 初始化地块之间的连接
        foreach (var tile in tileService.GetAll())
        {
            if (!IsPathValidTile(tile)) continue;
            foreach (var neighbor in tileService.GetNeighbors(tile))
            {
                if (IsTileConnected(tile, neighbor))
                    _aStar.ConnectPoints(tile.Id, neighbor.Id);
            }
        }

        tileService.UpdateTileAStar += tileId => UpdateAStar(tileId);
    }

    private void UpdateAStar(int tileId, bool unitBlock = true)
    {
        var tile = tileService.GetById(tileId);
        _aStar.SetPointDisabled(tile.Id, !IsPathValidTile(tile, unitBlock));
        foreach (var neighbor in tileService.GetNeighbors(tile))
        {
            if (IsTileConnected(tile, neighbor, unitBlock))
                _aStar.ConnectPoints(tile.Id, neighbor.Id);
            else
                _aStar.DisconnectPoints(tile.Id, neighbor.Id);
        }
    }

    public void ClearOldData() => _aStar?.Clear();

    public List<Tile> FindPath(Tile fromTile, Tile toTile)
    {
        if (fromTile == toTile)
            return [];
        if (!IsPathValidTile(fromTile, false) || !IsPathValidTile(toTile))
            return [];
        UpdateAStar(fromTile.Id, false); // 避免单位被自己阻挡
        var path = _aStar.GetIdPath(fromTile.Id, toTile.Id);
        UpdateAStar(fromTile.Id); // 还原 A* 地图
        return path == null ? [] : path.Select(l => tileService.GetById((int)l)).ToList();
    }

    public bool ExistPath(Tile fromTile, Tile toTile)
    {
        if (fromTile == toTile)
            return true;
        if (!IsPathValidTile(fromTile, false) || !IsPathValidTile(toTile))
            return false;
        UpdateAStar(fromTile.Id, false); // 避免单位被自己阻挡
        var path = _aStar.GetIdPath(fromTile.Id, toTile.Id);
        UpdateAStar(fromTile.Id); // 还原 A* 地图
        return path != null;
    }

    public float TotalCost(long fromId, long toId)
    {
        if (fromId == toId) return 0f;
        var path = _aStar.GetIdPath(fromId, toId);
        if (path == null) return float.MaxValue;
        var costTotal = 0f;
        for (var i = 0; i < path.Length - 1; i++)
        {
            var tile = tileService.GetById((int)path[i]);
            var neighbor = tileService.GetById((int)path[i + 1]);
            costTotal += Cost(tile, neighbor);
        }

        return costTotal;
    }

    public int Cost(Tile fromTile, Tile toTile) => TileAStar.Cost(fromTile, toTile);

    private static bool IsPathValidTile(Tile tile, bool unitBlock = true) =>
        // 水下不可寻路，unitBlock 为 true 时校验单位阻挡
        !tile.IsUnderwater && (!unitBlock || tile.UnitId == 0);

    private static bool IsTileConnected(Tile tile, Tile neighbor, bool unitBlock = true)
    {
        if (!IsPathValidTile(tile, unitBlock) || !IsPathValidTile(neighbor)
                                              || tile.GetEdgeType(neighbor) == HexEdgeType.Cliff)
            return false; // 寻路无效地块，或者悬崖阻挡
        // 是否被城墙阻挡
        return tile.Walled == neighbor.Walled || tile.HasRoadThroughEdge(tile.GetNeighborIdx(neighbor));
    }
}