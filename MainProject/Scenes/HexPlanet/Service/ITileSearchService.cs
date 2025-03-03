using System.Collections.Generic;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service;

public interface ITileSearchService
{
    public void InitSearchData(int tileCount);
    public void RefreshTileSearchData(int tileId);
    List<Tile> GetVisibleTiles(Tile fromTile, int range);
}