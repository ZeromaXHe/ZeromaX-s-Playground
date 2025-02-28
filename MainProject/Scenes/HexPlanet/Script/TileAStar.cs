using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Enum;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Script;

public partial class TileAStar(ITileService tileService) : AStar3D
{
    public override float _ComputeCost(long fromId, long toId)
    {
        var path = GetIdPath(fromId, toId);
        if (path == null || path.Length < 2) return float.MaxValue;
        var costTotal = 0f;
        for (var i = 0; i < path.Length - 1; i++)
        {
            var tile = tileService.GetById((int)path[i]);
            var neighbor = tileService.GetById((int)path[i + 1]);
            var hasRoad = tile.HasRoadThroughEdge(tile.GetNeighborIdx(neighbor));
            var isFlat = tile.GetEdgeType(neighbor) == HexEdgeType.Flat;
            var cost = hasRoad
                ? 1f
                : (isFlat ? 5f : 10f)
                  + neighbor.UrbanLevel + neighbor.FarmLevel + neighbor.PlantLevel;
            costTotal += cost;
        }

        return costTotal;
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