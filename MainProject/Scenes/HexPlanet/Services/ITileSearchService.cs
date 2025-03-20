using System.Collections.Generic;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entities;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Services;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-03-03 09:14
public interface ITileSearchService
{
    List<Tile> FindPath(Tile fromTile, Tile toTile, bool useCache = false);
    void ClearPath();
    public void InitSearchData();
    public void RefreshTileSearchData(int tileId);
    List<Tile> GetVisibleTiles(Tile fromTile, int range);
    int GetMoveCost(Tile fromTile, Tile toTile);

    int RaiseTerrain(int chunkSize, int budget, int firstTileId, int rise,
        RandomNumberGenerator random, float jitterProbability);

    int SinkTerrain(int chunkSize, int budget, int firstTileId, int sink,
        RandomNumberGenerator random, float jitterProbability);
}