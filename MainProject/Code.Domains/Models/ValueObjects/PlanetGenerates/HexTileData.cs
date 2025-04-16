using Commons.Enums;
using Commons.Frameworks;
using Commons.Utils;
using Domains.Models.Singletons.Planets;

namespace Domains.Models.ValueObjects.PlanetGenerates;

/// <summary>
/// Copyright 2022 Jasper Flick
/// 来源：Catlike Coding - Unity - Hex Map 教程
/// 源码地址：https://bitbucket.org/catlikecoding-projects/hex-map-project/src/2399393cdf64ad7d83eaff456f1207aa214356e2/Assets/Scripts/HexCellData.cs?at=release%2F3.4.0
/// 由 ZeromaXHe 进行针对 Godot 球面六边形地图的改造
/// </summary>
public struct HexTileData
{
    public HexTileData() => InitServices();

    #region 服务

    private static IPlanetConfig? _planetConfig;

    private static void InitServices()
    {
        _planetConfig ??= ContextHolder.BeanContext?.GetBean<IPlanetConfig>();
    }

    #endregion

    /// <summary>
    /// Cell flags.
    /// </summary>
    public HexFlags Flags = HexFlags.Explorable;

    /// <summary>
    /// Cell values.
    /// </summary>
    public HexValues Values = default;

    /// <summary>
    /// Surface elevation level.
    /// </summary>
    public readonly int Elevation => Values.Elevation;

    /// <summary>
    /// Water elevation level.
    /// </summary>
    public readonly int WaterLevel => Values.WaterLevel;

    /// <summary>
    /// Terrain type index.
    /// 地貌类型索引：
    /// 0 沙漠、1 草原、2 泥地、3 岩石、4 雪地
    /// </summary>
    public readonly int TerrainTypeIndex => Values.TerrainTypeIndex;

    /// <summary>
    /// Urban feature level.
    /// </summary>
    public readonly int UrbanLevel => Values.UrbanLevel;

    /// <summary>
    /// Farm feature level.
    /// </summary>
    public readonly int FarmLevel => Values.FarmLevel;

    /// <summary>
    /// Plant feature level.
    /// </summary>
    public readonly int PlantLevel => Values.PlantLevel;

    /// <summary>
    /// Special feature index.
    /// </summary>
    public readonly int SpecialIndex => Values.SpecialIndex;

    /// <summary>
    /// Whether the cell is considered inside a walled region.
    /// </summary>
    public readonly bool Walled => Flags.HasAny(HexFlags.Walled);

    /// <summary>
    /// Whether the cell contains roads.
    /// </summary>
    public readonly bool HasRoads => Flags.HasAny(HexFlags.Roads);

    /// <summary>
    /// Whether the cell counts as explored.
    /// </summary>
    public readonly bool IsExplored => Flags.HasAll(HexFlags.Explored | HexFlags.Explorable);

    public readonly bool IsExplorable => Flags.HasAll(HexFlags.Explorable);

    /// <summary>
    /// Whether the cell contains a special feature.
    /// </summary>
    public readonly bool IsSpecial => Values.SpecialIndex > 0;

    /// <summary>
    /// Whether the cell counts as underwater,
    /// which is when water is higher than surface.
    /// </summary>
    public readonly bool IsUnderwater => Values.WaterLevel > Values.Elevation;

    /// <summary>
    /// Whether there is an incoming river.
    /// </summary>
    public readonly bool HasIncomingRiver => Flags.HasAny(HexFlags.RiverIn);

    /// <summary>
    /// Whether there is an outgoing river.
    /// </summary>
    public readonly bool HasOutgoingRiver => Flags.HasAny(HexFlags.RiverOut);

    /// <summary>
    /// Whether there is a river, either incoming, outgoing, or both.
    /// </summary>
    public readonly bool HasRiver => Flags.HasAny(HexFlags.River);

    /// <summary>
    /// Whether a river begins or ends in the cell.
    /// </summary>
    public readonly bool HasRiverBeginOrEnd =>
        HasIncomingRiver != HasOutgoingRiver;

    /// <summary>
    /// Incoming river direction, if applicable.
    /// </summary>
    public readonly int IncomingRiver => Flags.RiverInDirection();

    /// <summary>
    /// Outgoing river direction, if applicable.
    /// </summary>
    /// 
    public readonly int OutgoingRiver => Flags.RiverOutDirection();

    /// <summary>
    /// Vertical positions the the stream bed, if applicable.
    /// </summary>
    public readonly float StreamBedY =>
        (Values.Elevation + HexMetrics.StreamBedElevationOffset) *
        _planetConfig!.UnitHeight;

    /// <summary>
    /// Vertical position of the river's surface, if applicable.
    /// </summary>
    public readonly float RiverSurfaceY =>
        (Values.Elevation + HexMetrics.WaterElevationOffset) *
        _planetConfig!.UnitHeight;

    /// <summary>
    /// Vertical position of the water surface, if applicable.
    /// </summary>
    public readonly float WaterSurfaceY =>
        (Values.WaterLevel + HexMetrics.WaterElevationOffset) *
        _planetConfig!.UnitHeight;

    /// <summary>
    /// Elevation at which the cell is visible.
    /// Highest of surface and water level.
    /// </summary>
    public readonly int ViewElevation =>
        Elevation >= WaterLevel ? Elevation : WaterLevel;

    /// <summary>
    /// Get the <see cref="HexEdgeType"/> based on this and another cell.
    /// </summary>
    /// <param name="otherTile">Other cell to consider as neighbor.</param>
    /// <returns><see cref="HexEdgeType"/> between cells.</returns>
    public readonly HexEdgeType GetEdgeType(HexTileData otherTile) =>
        HexMetrics.GetEdgeType(Values.Elevation, otherTile.Values.Elevation);

    /// <summary>
    /// Whether an incoming river goes through a specific cell edge.
    /// </summary>
    /// <param name="direction">Edge direction relative to the cell.</param>
    /// <returns>Whether an incoming river goes through the edge.</returns>
    public readonly bool HasIncomingRiverThroughEdge(int direction) =>
        Flags.HasRiverIn(direction);

    /// <summary>
    /// Whether a river goes through a specific cell edge.
    /// </summary>
    /// <param name="direction">Edge direction relative to the cell.</param>
    /// <returns>Whether a river goes through the edge.</returns>
    public readonly bool HasRiverThroughEdge(int direction) =>
        Flags.HasRiverIn(direction) || Flags.HasRiverOut(direction);

    /// <summary>
    /// Whether a road goes through a specific cell edge.
    /// </summary>
    /// <param name="direction">Edge direction relative to cell.</param>
    /// <returns>Whether a road goes through the edge.</returns>
    public readonly bool HasRoadThroughEdge(int direction) =>
        Flags.HasRoad(direction);
}