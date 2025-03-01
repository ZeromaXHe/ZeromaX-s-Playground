using System.Collections.Generic;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service;

public interface IAStarService
{
    void Init();
    void ClearOldData();

    // 当 fromTile（出发地块）和 toTile（目标地块）之间存在路径时，返回路径上的所有地块（包括出发和目标地块）
    List<Tile> FindPath(Tile fromTile, Tile toTile);
    bool ExistPath(Tile fromTile, Tile toTile);
    int Cost(Tile fromTile, Tile toTile);
}