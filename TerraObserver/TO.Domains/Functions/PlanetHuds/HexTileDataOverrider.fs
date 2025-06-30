namespace TO.Domains.Functions.PlanetHuds

open TO.Domains.Functions.HexMetrics
open TO.Domains.Functions.HexSpheres.Components.Tiles
open TO.Domains.Types.HexSpheres.Components.Tiles
open TO.Domains.Types.PlanetHuds

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-29 21:26:29
module HexTileDataOverrider =
    let isOverridingTileConnection (tile: int) (neighbor: int) (this: HexTileDataOverrider) =
        this.EditMode
        && this.OverrideTileIds.Count > 0
        && this.OverrideTileIds.Contains tile
        && not <| this.OverrideTileIds.Contains neighbor

    let isOverrideTile (tile: int) (this: HexTileDataOverrider) =
        this.EditMode && this.OverrideTileIds.Contains tile

    let isOverrideNoRiver (tile: int) (this: HexTileDataOverrider) =
        isOverrideTile tile this && this.RiverMode = OptionalToggle.No

    let isOverrideNoRoad (tile: int) (this: HexTileDataOverrider) =
        isOverrideTile tile this && this.RoadMode = OptionalToggle.No

    let elevation (tile: int) (tileValue: TileValue) (this: HexTileDataOverrider) =
        if isOverrideTile tile this && this.ApplyElevation then
            this.ActiveElevation
        else
            TileValue.elevation tileValue

    let getEdgeType
        (tile1: int)
        (tileValue1: TileValue)
        (tile2: int)
        (tileValue2: TileValue)
        (this: HexTileDataOverrider)
        =
        HexMetrics.getEdgeType
        <| elevation tile1 tileValue1 this
        <| elevation tile2 tileValue2 this

    let waterLevel (tile: int) (tileValue: TileValue) (this: HexTileDataOverrider) =
        if isOverrideTile tile this && this.ApplyWaterLevel then
            this.ActiveWaterLevel
        else
            TileValue.waterLevel tileValue

    let isUnderwater (tile: int) (tileValue: TileValue) (this: HexTileDataOverrider) =
        if isOverrideTile tile this then
            waterLevel tile tileValue this > elevation tile tileValue this
        else
            TileValue.isUnderwater tileValue

    let streamBedY (tile: int) (tileValue: TileValue) (unitHeight: float32) (this: HexTileDataOverrider) =
        if isOverrideTile tile this then
            (float32 (elevation tile tileValue this) + HexMetrics.streamBedElevationOffset)
            * unitHeight
        else
            TileValue.streamBedY unitHeight tileValue

    let riverSurfaceY (tile: int) (tileValue: TileValue) (unitHeight: float32) (this: HexTileDataOverrider) =
        if isOverrideTile tile this then
            (float32 (elevation tile tileValue this) + HexMetrics.waterElevationOffset)
            * unitHeight
        else
            TileValue.streamBedY unitHeight tileValue

    let waterSurfaceY (tile: int) (tileValue: TileValue) (unitHeight: float32) (this: HexTileDataOverrider) =
        if isOverrideTile tile this then
            (float32 (waterLevel tile tileValue this) + HexMetrics.waterElevationOffset)
            * unitHeight
        else
            TileValue.waterSurfaceY unitHeight tileValue

    let hasRivers (tile: int) (tileFlag: TileFlag) (this: HexTileDataOverrider) =
        not <| isOverrideNoRiver tile this && TileFlag.hasRivers tileFlag

    let hasIncomingRiver (tile: int) (tileFlag: TileFlag) (this: HexTileDataOverrider) =
        not <| isOverrideNoRiver tile this && TileFlag.hasIncomingRiver tileFlag

    let hasOutgoingRiver (tile: int) (tileFlag: TileFlag) (this: HexTileDataOverrider) =
        not <| isOverrideNoRiver tile this && TileFlag.hasOutgoingRiver tileFlag

    let hasRiverBeginOrEnd (tile: int) (tileFlag: TileFlag) (this: HexTileDataOverrider) =
        not <| isOverrideNoRiver tile this && TileFlag.hasRiverBeginOrEnd tileFlag

    let hasRiverThroughEdge (tile: int) (tileFlag: TileFlag) (dir: int) (this: HexTileDataOverrider) =
        not <| isOverrideNoRiver tile this && TileFlag.hasRiverThroughEdge dir tileFlag

    let hasIncomingRiverThroughEdge (tile: int) (tileFlag: TileFlag) (dir: int) (this: HexTileDataOverrider) =
        not <| isOverrideNoRiver tile this
        && TileFlag.hasIncomingRiverThoughEdge dir tileFlag

    let hasRoads (tile: int) (tileFlag: TileFlag) (this: HexTileDataOverrider) =
        not <| isOverrideNoRoad tile this && TileFlag.hasRoads tileFlag

    let hasRoadThroughEdge (tile: int) (tileFlag: TileFlag) (dir: int) (this: HexTileDataOverrider) =
        not <| isOverrideNoRoad tile this && TileFlag.hasRoadThroughEdge dir tileFlag

    let walled (tile: int) (tileFlag: TileFlag) (this: HexTileDataOverrider) =
        if isOverrideTile tile this && this.WalledMode <> OptionalToggle.Ignore then
            this.WalledMode = OptionalToggle.Yes
        else
            TileFlag.walled tileFlag

    let urbanLevel (tile: int) (tileValue: TileValue) (this: HexTileDataOverrider) =
        if isOverrideTile tile this && this.ApplyUrbanLevel then
            this.ActiveUrbanLevel
        else
            TileValue.urbanLevel tileValue

    let farmLevel (tile: int) (tileValue: TileValue) (this: HexTileDataOverrider) =
        if isOverrideTile tile this && this.ApplyFarmLevel then
            this.ActiveFarmLevel
        else
            TileValue.farmLevel tileValue

    let plantLevel (tile: int) (tileValue: TileValue) (this: HexTileDataOverrider) =
        if isOverrideTile tile this && this.ApplyPlantLevel then
            this.ActivePlantLevel
        else
            TileValue.plantLevel tileValue

    let specialIndex (tile: int) (tileValue: TileValue) (this: HexTileDataOverrider) =
        if isOverrideTile tile this && this.ApplySpecialIndex then
            this.ActiveSpecialIndex
        else
            TileValue.specialIndex tileValue

    let isSpecial (tile: int) (tileValue: TileValue) (this: HexTileDataOverrider) =
        specialIndex tile tileValue this <> 0
