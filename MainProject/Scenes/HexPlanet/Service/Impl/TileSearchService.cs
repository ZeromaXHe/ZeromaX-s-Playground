using System.Collections.Generic;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Enum;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Struct;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service.Impl;

public class TileSearchService(ITileService tileService) : ITileSearchService
{
    private TileSearchData[] _searchData;
    private TilePriorityQueue _searchFrontier;
    private int _searchFrontierPhase = 0;
    private int _currentPathFromId = -1;
    private int _currentPathToId = -1;

    public bool HasPath { get; private set; }

    public void InitSearchData() => _searchData = new TileSearchData[tileService.GetCount() + 1];
    public void RefreshTileSearchData(int tileId) => _searchData[tileId].SearchPhase = 0;

    private const int UnitSpeed = 24;

    public List<Tile> FindPath(Tile fromTile, Tile toTile, bool useCache = false)
    {
        if (!useCache || fromTile.Id != _currentPathFromId || toTile.Id != _currentPathToId)
        {
            HasPath = IsValidDestination(toTile) && SearchPath(fromTile, toTile);
            // GD.Print($"SearchPath from {fromTile.Id} to {toTile.Id} result: {HasPath}");
        }
        _currentPathFromId = fromTile.Id;
        _currentPathToId = toTile.Id;
        if (!HasPath) return [];
        var currentId = _currentPathToId;
        var res = new List<Tile>();
        while (currentId != _currentPathFromId)
        {
            res.Add(tileService.GetById(currentId));
            currentId = _searchData[currentId].PathFrom;
        }
        res.Add(fromTile);
        res.Reverse();
        return res;
    }

    public void ClearPath()
    {
        if (HasPath)
        {
            var currentId = _currentPathToId;
            while (currentId != _currentPathFromId)
            {
                tileService.UpdateTileLabel(tileService.GetById(currentId), "");
                currentId = _searchData[currentId].PathFrom;
            }

            HasPath = false;
        }

        _currentPathFromId = -1;
        _currentPathToId = -1;
    }

    public bool SearchPath(Tile fromTile, Tile toTile)
    {
        _searchFrontierPhase += 2;
        _searchFrontier ??= new TilePriorityQueue(_searchData);
        _searchFrontier.Clear();
        _searchData[fromTile.Id] = new TileSearchData { SearchPhase = _searchFrontierPhase };
        _searchFrontier.Enqueue(fromTile.Id);
        while (_searchFrontier.TryDequeue(out var currentId))
        {
            var current = tileService.GetById(currentId);
            var currentDistance = _searchData[currentId].Distance;
            _searchData[currentId].SearchPhase++;
            if (current == toTile)
                return true;
            foreach (var neighbor in tileService.GetNeighbors(current))
            {
                var neighborData = _searchData[neighbor.Id];
                if (neighborData.SearchPhase > _searchFrontierPhase || !IsValidDestination(neighbor))
                    continue;
                var moveCost = GetMoveCost(current, neighbor);
                if (moveCost < 0)
                    continue;
                var distance = currentDistance + moveCost;
                if (neighborData.SearchPhase < _searchFrontierPhase)
                {
                    _searchData[neighbor.Id] = new TileSearchData
                    {
                        Distance = distance,
                        PathFrom = currentId,
                        Heuristic = (int)HeuristicCost(neighbor, toTile),
                        SearchPhase = _searchFrontierPhase
                    };
                    _searchFrontier.Enqueue(neighbor.Id);
                }
                else if (distance < neighborData.Distance)
                {
                    _searchData[neighbor.Id].Distance = distance;
                    _searchData[neighbor.Id].PathFrom = currentId;
                    _searchFrontier.Change(neighbor.Id, neighborData.SearchPriority);
                }
            }
        }

        return false;
    }

    public List<Tile> GetVisibleTiles(Tile fromTile, int range)
    {
        var visibleTiles = new List<Tile>();
        _searchFrontierPhase += 2;
        _searchFrontier ??= new TilePriorityQueue(_searchData);
        _searchFrontier.Clear();
        range += fromTile.ViewElevation;
        _searchData[fromTile.Id] = new TileSearchData
        {
            SearchPhase = _searchFrontierPhase,
            PathFrom = _searchData[fromTile.Id].PathFrom
        };
        _searchFrontier.Enqueue(fromTile.Id);
        while (_searchFrontier.TryDequeue(out var currentId))
        {
            var current = tileService.GetById(currentId);
            _searchData[currentId].SearchPhase++;
            visibleTiles.Add(current);
            foreach (var neighbor in tileService.GetNeighbors(current))
            {
                var neighborData = _searchData[neighbor.Id];
                if (neighborData.SearchPhase > _searchFrontierPhase || !neighbor.Explorable)
                    continue;
                var distance = _searchData[currentId].Distance + 1;
                if (distance + neighbor.ViewElevation > range
                    || distance > HeuristicCost(fromTile, neighbor) * HexMetrics.InnerToOuter)
                    // 没法直接拿到两地块间的最短间距，使用启发式值（估算球面距离）/ √3 * 2 作为最短间距
                    continue;
                if (neighborData.SearchPhase < _searchFrontierPhase)
                {
                    _searchData[neighbor.Id] = new TileSearchData
                    {
                        Distance = distance,
                        PathFrom = neighborData.PathFrom,
                        SearchPhase = _searchFrontierPhase
                    };
                    _searchFrontier.Enqueue(neighbor.Id);
                }
                else if (distance < neighborData.Distance)
                {
                    _searchData[neighbor.Id].Distance = distance;
                    _searchFrontier.Change(neighbor.Id, neighborData.SearchPriority);
                }
            }
        }

        return visibleTiles;
    }

    private float HeuristicCost(Tile from, Tile to)
    {
        var neighbor = tileService.GetNeighborByIdx(from, 0);
        var fromToAngle = from.UnitCentroid.AngleTo(to.UnitCentroid);
        var fromNeighborAngle = from.UnitCentroid.AngleTo(neighbor.UnitCentroid);
        return Mathf.Round(fromToAngle / fromNeighborAngle);
    }

    private static bool IsValidDestination(Tile tile)
    {
        return tile.Explored && tile.Explorable && !tile.IsUnderwater && !tile.HasUnit;
    }

    public int GetMoveCost(Tile fromTile, Tile toTile)
    {
        var edgeType = fromTile.GetEdgeType(toTile);
        if (edgeType == HexEdgeType.Cliff)
            return -1;
        if (fromTile.HasRoadThroughEdge(fromTile.GetNeighborIdx(toTile)))
            return 1;
        if (fromTile.Walled != toTile.Walled)
            return -1;
        return (edgeType == HexEdgeType.Flat ? 5 : 10) + toTile.UrbanLevel + toTile.FarmLevel + toTile.PlantLevel;
    }
}