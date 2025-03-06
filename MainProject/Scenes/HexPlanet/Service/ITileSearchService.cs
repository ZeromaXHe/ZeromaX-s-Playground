using System.Collections.Generic;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service;

public interface ITileSearchService
{
    List<Tile> FindPath(Tile fromTile, Tile toTile, bool useCache = false);
    void ClearPath();
    public void InitSearchData();
    public void RefreshTileSearchData(int tileId);
    List<Tile> GetVisibleTiles(Tile fromTile, int range);
    int GetMoveCost(Tile fromTile, Tile toTile);

    int RaiseTerrain(int chunkSize, int budget, int firstTileId, int rise,
        RandomNumberGenerator random, int elevationMaximum, int waterLevel, float jitterProbability);

    int SinkTerrain(int chunkSize, int budget, int firstTileId, int sink,
        RandomNumberGenerator random, int elevationMinimum, int waterLevel, float jitterProbability);
}