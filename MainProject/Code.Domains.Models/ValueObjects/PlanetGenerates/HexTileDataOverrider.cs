using Commons.Enums;
using Commons.Utils;
using Domains.Models.Entities.PlanetGenerates;

namespace Domains.Models.ValueObjects.PlanetGenerates;

public enum OptionalToggle
{
    Ignore,
    Yes,
    No
}

public struct HexTileDataOverrider
{
    public HexTileDataOverrider()
    {
    }

    public bool EditMode;
    public bool ApplyTerrain;
    public int ActiveTerrain;
    public bool ApplyElevation;
    public int ActiveElevation;
    public bool ApplyWaterLevel;
    public int ActiveWaterLevel;
    public int BrushSize;
    public OptionalToggle RiverMode;
    public OptionalToggle RoadMode;
    public bool ApplyUrbanLevel;
    public int ActiveUrbanLevel;
    public bool ApplyFarmLevel;
    public int ActiveFarmLevel;
    public bool ApplyPlantLevel;
    public int ActivePlantLevel;
    public OptionalToggle WalledMode;
    public bool ApplySpecialIndex;
    public int ActiveSpecialIndex;
    public HashSet<Tile> OverrideTiles = [];

    public bool IsOverridingTileConnection(Tile tile, Tile neighbor) =>
        EditMode && OverrideTiles.Count > 0
                 && OverrideTiles.Contains(tile) && !OverrideTiles.Contains(neighbor);

    public bool IsOverrideTile(Tile tile) => EditMode && OverrideTiles.Contains(tile);
    public bool IsOverrideNoRiver(Tile tile) => IsOverrideTile(tile) && RiverMode == OptionalToggle.No;
    public bool IsOverrideNoRoad(Tile tile) => IsOverrideTile(tile) && RoadMode == OptionalToggle.No;

    public int Elevation(Tile tile) =>
        IsOverrideTile(tile) && ApplyElevation ? ActiveElevation : tile.Data.Elevation;

    public int WaterLevel(Tile tile) =>
        IsOverrideTile(tile) && ApplyWaterLevel ? ActiveWaterLevel : tile.Data.WaterLevel;

    public bool IsUnderwater(Tile tile) =>
        IsOverrideTile(tile) ? WaterLevel(tile) > Elevation(tile) : tile.Data.IsUnderwater;

    public float StreamBedY(Tile tile, float unitHeight) =>
        IsOverrideTile(tile)
            ? (Elevation(tile) + HexMetrics.StreamBedElevationOffset) * unitHeight
            : tile.Data.StreamBedY(unitHeight);

    public float RiverSurfaceY(Tile tile, float unitHeight) =>
        IsOverrideTile(tile)
            ? (Elevation(tile) + HexMetrics.WaterElevationOffset) * unitHeight
            : tile.Data.RiverSurfaceY(unitHeight);

    public float WaterSurfaceY(Tile tile, float unitHeight) =>
        IsOverrideTile(tile)
            ? (WaterLevel(tile) + HexMetrics.WaterElevationOffset) * unitHeight
            : tile.Data.WaterSurfaceY(unitHeight);

    public bool HasRiver(Tile tile) => !IsOverrideNoRiver(tile) && tile.Data.HasRiver;
    public bool HasIncomingRiver(Tile tile) => !IsOverrideNoRiver(tile) && tile.Data.HasIncomingRiver;
    public bool HasOutgoingRiver(Tile tile) => !IsOverrideNoRiver(tile) && tile.Data.HasOutgoingRiver;
    public bool HasRiverBeginOrEnd(Tile tile) => !IsOverrideNoRiver(tile) && tile.Data.HasRiverBeginOrEnd;

    public bool HasRiverThroughEdge(Tile tile, int idx) =>
        !IsOverrideNoRiver(tile) && tile.Data.HasRiverThroughEdge(idx);

    public bool HasIncomingRiverThroughEdge(Tile tile, int idx) =>
        !IsOverrideNoRiver(tile) && tile.Data.HasIncomingRiverThroughEdge(idx);

    public bool HasRoads(Tile tile) => !IsOverrideNoRoad(tile) && tile.Data.HasRoads;

    public bool HasRoadThroughEdge(Tile tile, int idx) =>
        !IsOverrideNoRoad(tile) && tile.Data.HasRoadThroughEdge(idx);

    public bool Walled(Tile tile) =>
        IsOverrideTile(tile) && WalledMode != OptionalToggle.Ignore
            ? WalledMode == OptionalToggle.Yes
            : tile.Data.Walled;

    public HexEdgeType GetEdgeType(Tile tile1, Tile tile2) =>
        HexMetrics.GetEdgeType(Elevation(tile1), Elevation(tile2));

    public int UrbanLevel(Tile tile) =>
        IsOverrideTile(tile) && ApplyUrbanLevel ? ActiveUrbanLevel : tile.Data.UrbanLevel;

    public int FarmLevel(Tile tile) =>
        IsOverrideTile(tile) && ApplyFarmLevel ? ActiveFarmLevel : tile.Data.FarmLevel;

    public int PlantLevel(Tile tile) =>
        IsOverrideTile(tile) && ApplyPlantLevel ? ActivePlantLevel : tile.Data.PlantLevel;

    public int SpecialIndex(Tile tile) =>
        IsOverrideTile(tile) && ApplySpecialIndex ? ActiveSpecialIndex : tile.Data.SpecialIndex;

    public bool IsSpecial(Tile tile) => SpecialIndex(tile) > 0;
}