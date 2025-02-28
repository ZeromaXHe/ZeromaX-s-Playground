using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Enum;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Script;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service.Impl;

public class AStarService(ITileService tileService)
{
    private TileAStar _aStar;

    public void Init()
    {
        _aStar = new TileAStar(tileService);
        // 初始化所有可达地块
        foreach (var tile in tileService.GetAll())
        {
            if (tile.IsUnderwater) continue;
            _aStar.AddPoint(tile.Id, tile.UnitCentroid);
        }

        // 初始化地块之间的连接
        foreach (var tile in tileService.GetAll())
        {
            if (tile.IsUnderwater) continue;
            foreach (var neighbor in tileService.GetNeighbors(tile))
            {
                if (neighbor.IsUnderwater
                    || tile.GetEdgeType(neighbor) == HexEdgeType.Cliff)
                    continue; // 水下或者悬崖阻挡
                if (tile.Walled != neighbor.Walled && !tile.HasRoadThroughEdge(tile.GetNeighborIdx(neighbor)))
                    continue; // 城墙阻挡
                _aStar.ConnectPoints(tile.Id, neighbor.Id);
            }
        }
    }
}