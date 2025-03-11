using System.Collections.Generic;
using ZeromaXsPlaygroundProject.Scenes.Framework.Dependency;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Enum;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Struct;

public enum OptionalToggle
{
    Ignore,
    Yes,
    No
}

public struct HexTileDataOverrider
{
    public HexTileDataOverrider() => InitServices();

    #region 服务

    private static IPlanetSettingService _planetSettingService;

    private static void InitServices()
    {
        _planetSettingService ??= Context.GetBean<IPlanetSettingService>();
    }

    #endregion

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

    public bool IsOverrideTile(Tile tile) => EditMode && OverrideTiles.Contains(tile);
    public bool IsOverrideNoRiver(Tile tile) => IsOverrideTile(tile) && RiverMode == OptionalToggle.No;
    public bool IsOverrideNoRoad(Tile tile) => IsOverrideTile(tile) && RoadMode == OptionalToggle.No;

    public int Elevation(Tile tile) =>
        // 现在低于陆地高度的不再绘制低于高度的部分
        IsOverrideTile(tile) && ApplyElevation && ActiveElevation > tile.Data.Elevation
            ? ActiveElevation
            : tile.Data.Elevation;

    public int WaterLevel(Tile tile) =>
        IsOverrideTile(tile) && ApplyWaterLevel ? ActiveWaterLevel : tile.Data.WaterLevel;

    public bool IsUnderwater(Tile tile) =>
        IsOverrideTile(tile) ? WaterLevel(tile) > Elevation(tile) : tile.Data.IsUnderwater;

    public float StreamBedY(Tile tile) =>
        IsOverrideTile(tile)
            ? (Elevation(tile) + HexMetrics.StreamBedElevationOffset) * _planetSettingService.UnitHeight
            : tile.Data.StreamBedY;

    public float RiverSurfaceY(Tile tile) =>
        IsOverrideTile(tile)
            ? (Elevation(tile) + HexMetrics.WaterElevationOffset) * _planetSettingService.UnitHeight
            : tile.Data.RiverSurfaceY;

    public float WaterSurfaceY(Tile tile) =>
        IsOverrideTile(tile)
            ? (WaterLevel(tile) + HexMetrics.WaterElevationOffset) * _planetSettingService.UnitHeight
            : tile.Data.WaterSurfaceY;

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