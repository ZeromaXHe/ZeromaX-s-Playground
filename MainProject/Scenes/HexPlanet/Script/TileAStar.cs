using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Enum;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Script;

public partial class TileAStar(ITileService tileService) : AStar3D
{
    public TileAStar() : this(null)
    {
    }

    public override float _ComputeCost(long fromId, long toId)
    {
        var tile = tileService.GetById((int)fromId);
        var neighbor = tileService.GetById((int)toId);
        return Cost(tile, neighbor);
    }

    public static int Cost(Tile tile, Tile neighbor)
    {
        var hasRoad = tile.HasRoadThroughEdge(tile.GetNeighborIdx(neighbor));
        var isFlat = tile.GetEdgeType(neighbor) == HexEdgeType.Flat;
        return hasRoad
            ? 1
            : (isFlat ? 5 : 10)
              + neighbor.UrbanLevel + neighbor.FarmLevel + neighbor.PlantLevel;
    }

    public override float _EstimateCost(long fromId, long toId)
    {
        var from = tileService.GetById((int)fromId);
        var to = tileService.GetById((int)toId);
        var neighbor = tileService.GetNeighborByIdx(from, 0);
        var fromToAngle = from.UnitCentroid.AngleTo(to.UnitCentroid);
        var fromNeighborAngle = from.UnitCentroid.AngleTo(neighbor.UnitCentroid);
        return Mathf.Round(fromToAngle / fromNeighborAngle);
    }
}