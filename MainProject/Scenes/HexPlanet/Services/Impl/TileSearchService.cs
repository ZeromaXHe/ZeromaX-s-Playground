using System.Collections.Generic;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entities;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Enums;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Repos;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Structs;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Utils;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Services.Impl;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-03-03 09:14
public class TileSearchService(
    IPointRepo pointRepo,
    IChunkRepo chunkRepo,
    ITileRepo tileRepo,
    IPlanetSettingService planetSettingService)
    : ITileSearchService
{
    private TileSearchData[] _searchData;
    private TilePriorityQueue _searchFrontier;
    private int _searchFrontierPhase = 0;
    private int _currentPathFromId = -1;
    private int _currentPathToId = -1;

    public bool HasPath { get; private set; }

    public void InitSearchData()
    {
        _searchData = new TileSearchData[tileRepo.GetCount() + 1];
        _searchFrontier = null;
    }

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
            res.Add(tileRepo.GetById(currentId));
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
                chunkRepo.RefreshTileLabel(tileRepo.GetById(currentId), "");
                currentId = _searchData[currentId].PathFrom;
            }

            HasPath = false;
        }

        _currentPathFromId = -1;
        _currentPathToId = -1;
    }

    // 寻路
    public bool SearchPath(Tile fromTile, Tile toTile)
    {
        _searchFrontierPhase += 2;
        _searchFrontier ??= new TilePriorityQueue(_searchData);
        _searchFrontier.Clear();
        _searchData[fromTile.Id] = new TileSearchData { SearchPhase = _searchFrontierPhase };
        _searchFrontier.Enqueue(fromTile.Id);
        while (_searchFrontier.TryDequeue(out var currentId))
        {
            var current = tileRepo.GetById(currentId);
            var currentDistance = _searchData[currentId].Distance;
            _searchData[currentId].SearchPhase++;
            if (current == toTile)
                return true;
            foreach (var neighbor in tileRepo.GetNeighbors(current))
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
                        Heuristic = HeuristicCost(neighbor, toTile),
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

    // 可视范围
    public List<Tile> GetVisibleTiles(Tile fromTile, int range)
    {
        var visibleTiles = new List<Tile>();
        _searchFrontierPhase += 2;
        _searchFrontier ??= new TilePriorityQueue(_searchData);
        _searchFrontier.Clear();
        range += fromTile.Data.ViewElevation;
        _searchData[fromTile.Id] = new TileSearchData
        {
            SearchPhase = _searchFrontierPhase,
            PathFrom = _searchData[fromTile.Id].PathFrom
        };
        _searchFrontier.Enqueue(fromTile.Id);
        var fromCoords = pointRepo.GetSphereAxial(fromTile);
        while (_searchFrontier.TryDequeue(out var currentId))
        {
            var current = tileRepo.GetById(currentId);
            _searchData[currentId].SearchPhase++;
            visibleTiles.Add(current);
            foreach (var neighbor in tileRepo.GetNeighbors(current))
            {
                var neighborData = _searchData[neighbor.Id];
                if (neighborData.SearchPhase > _searchFrontierPhase || !neighbor.Data.IsExplorable)
                    continue;
                var distance = _searchData[currentId].Distance + 1;
                if (distance + neighbor.Data.ViewElevation > range
                    || distance > fromCoords.DistanceTo(pointRepo.GetSphereAxial(neighbor)))
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
                else if (distance < _searchData[neighbor.Id].Distance)
                {
                    _searchData[neighbor.Id].Distance = distance;
                    _searchFrontier.Change(neighbor.Id, neighborData.SearchPriority);
                }
            }
        }

        return visibleTiles;
    }

    private int HeuristicCost(Tile from, Tile to) =>
        pointRepo.GetSphereAxial(from).DistanceTo(pointRepo.GetSphereAxial(to));
    // {
    //     var fromSa = tileService.GetSphereAxial(from);
    //     var toSa = tileService.GetSphereAxial(to);
    //     var dist = fromSa.DistanceTo(toSa);
    //     GD.Print($"Heuristic: Tile {from.Id} {fromSa} to Tile {to.Id} {toSa} distance {dist}");
    //     return dist;
    // }

    private static bool IsValidDestination(Tile tile)
    {
        return tile.Data is { IsExplored: true, IsExplorable: true, IsUnderwater: false } && !tile.HasUnit;
    }

    public int GetMoveCost(Tile fromTile, Tile toTile)
    {
        var edgeType = fromTile.Data.GetEdgeType(toTile.Data);
        if (edgeType == HexEdgeType.Cliff)
            return -1;
        if (fromTile.Data.HasRoadThroughEdge(fromTile.GetNeighborIdx(toTile)))
            return 1;
        if (fromTile.Data.Walled != toTile.Data.Walled)
            return -1;
        return (edgeType == HexEdgeType.Flat ? 5 : 10)
               + toTile.Data.UrbanLevel + toTile.Data.FarmLevel + toTile.Data.PlantLevel;
    }

    // 抬升土地
    public int RaiseTerrain(int chunkSize, int budget, int firstTileId, int rise,
        RandomNumberGenerator random, float jitterProbability)
    {
        _searchFrontierPhase++;
        _searchFrontier ??= new TilePriorityQueue(_searchData);
        _searchFrontier.Clear();
        _searchData[firstTileId] = new TileSearchData { SearchPhase = _searchFrontierPhase };
        _searchFrontier.Enqueue(firstTileId);
        var firstTile = tileRepo.GetById(firstTileId);
        var center = pointRepo.GetSphereAxial(firstTile);
        var size = 0;
        while (size < chunkSize && _searchFrontier.TryDequeue(out var id))
        {
            var current = tileRepo.GetById(id);
            var originalElevation = current.Data.Elevation;
            var newElevation = originalElevation + rise;
            if (newElevation > planetSettingService.ElevationStep)
                continue;
            current.Data = current.Data with { Values = current.Data.Values.WithElevation(newElevation) };
            if (originalElevation < planetSettingService.DefaultWaterLevel
                && newElevation >= planetSettingService.DefaultWaterLevel && --budget == 0)
                break;
            size++;
            foreach (var neighbor in tileRepo.GetNeighbors(current))
            {
                if (_searchData[neighbor.Id].SearchPhase >= _searchFrontierPhase) continue;
                _searchData[neighbor.Id] = new TileSearchData
                {
                    SearchPhase = _searchFrontierPhase,
                    Distance = pointRepo.GetSphereAxial(neighbor).DistanceTo(center),
                    Heuristic = random.Randf() < jitterProbability ? 1 : 0
                };
                _searchFrontier.Enqueue(neighbor.Id);
            }
        }

        _searchFrontier.Clear();
        return budget;
    }

    // 下沉土地
    public int SinkTerrain(int chunkSize, int budget, int firstTileId, int sink,
        RandomNumberGenerator random, float jitterProbability)
    {
        _searchFrontierPhase++;
        _searchFrontier ??= new TilePriorityQueue(_searchData);
        _searchFrontier.Clear();
        _searchData[firstTileId] = new TileSearchData { SearchPhase = _searchFrontierPhase };
        _searchFrontier.Enqueue(firstTileId);
        var firstTile = tileRepo.GetById(firstTileId);
        var center = pointRepo.GetSphereAxial(firstTile);
        var size = 0;
        while (size < chunkSize && _searchFrontier.TryDequeue(out var id))
        {
            var current = tileRepo.GetById(id);
            var originalElevation = current.Data.Elevation;
            var newElevation = originalElevation - sink;
            if (newElevation < planetSettingService.ElevationStep)
                continue;
            current.Data = current.Data with { Values = current.Data.Values.WithElevation(newElevation) };
            if (originalElevation >= planetSettingService.DefaultWaterLevel
                && newElevation < planetSettingService.DefaultWaterLevel)
                budget++;
            size++;
            foreach (var neighbor in tileRepo.GetNeighbors(current))
            {
                if (_searchData[neighbor.Id].SearchPhase < _searchFrontierPhase)
                {
                    _searchData[neighbor.Id] = new TileSearchData
                    {
                        SearchPhase = _searchFrontierPhase,
                        Distance = pointRepo.GetSphereAxial(neighbor).DistanceTo(center),
                        Heuristic = random.Randf() < jitterProbability ? 1 : 0
                    };
                    _searchFrontier.Enqueue(neighbor.Id);
                }
            }
        }

        _searchFrontier.Clear();
        return budget;
    }
}