namespace TO.FSharp.Repos.Models.Meshes

open System.Collections.Generic
open TO.FSharp.Commons.Utils
open TO.FSharp.Repos.Models.HexSpheres.Tiles

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-09 17:19:09
type OptionalToggle =
    | Ignore = 0
    | Yes = 1
    | No = 2

type HexTileDataOverrider() =
    [<DefaultValue>] val mutable EditMode: bool
    [<DefaultValue>] val mutable ApplyTerrain: bool
    [<DefaultValue>] val mutable ActiveTerrain: int
    [<DefaultValue>] val mutable ApplyElevation: bool
    [<DefaultValue>] val mutable ActiveElevation: int
    [<DefaultValue>] val mutable ApplyWaterLevel: bool
    [<DefaultValue>] val mutable ActiveWaterLevel: int
    [<DefaultValue>] val mutable BrushSize: int
    [<DefaultValue>] val mutable RiverMode: OptionalToggle
    [<DefaultValue>] val mutable RoadMode: OptionalToggle
    [<DefaultValue>] val mutable ApplyUrbanLevel: bool
    [<DefaultValue>] val mutable ActiveUrbanLevel: int
    [<DefaultValue>] val mutable ApplyFarmLevel: bool
    [<DefaultValue>] val mutable ActiveFarmLevel: int
    [<DefaultValue>] val mutable ApplyPlantLevel: bool
    [<DefaultValue>] val mutable ActivePlantLevel: int
    [<DefaultValue>] val mutable WalledMode: OptionalToggle
    [<DefaultValue>] val mutable ApplySpecialIndex: bool
    [<DefaultValue>] val mutable ActiveSpecialIndex: int
    member val OverrideTileIds = HashSet<int>()

    member this.IsOverridingTileConnection (tile: int) (neighbor: int) =
        this.EditMode
        && this.OverrideTileIds.Count > 0
        && this.OverrideTileIds.Contains tile
        && not <| this.OverrideTileIds.Contains neighbor

    member this.IsOverrideTile(tile: int) =
        this.EditMode && this.OverrideTileIds.Contains tile

    member this.IsOverrideNoRiver(tile: int) =
        this.IsOverrideTile tile && this.RiverMode = OptionalToggle.No

    member this.IsOverrideNoRoad(tile: int) =
        this.IsOverrideTile tile && this.RoadMode = OptionalToggle.No

    member this.Elevation (tile: int) (tileValue: TileValue) =
        if this.IsOverrideTile tile && this.ApplyElevation then
            this.ActiveElevation
        else
            tileValue.Elevation

    member this.GetEdgeType (tile1: int) (tileValue1: TileValue) (tile2: int) (tileValue2: TileValue) =
        HexMetrics.getEdgeType
        <| this.Elevation tile1 tileValue1
        <| this.Elevation tile2 tileValue2

    member this.WaterLevel (tile: int) (tileValue: TileValue) =
        if this.IsOverrideTile tile && this.ApplyWaterLevel then
            this.ActiveWaterLevel
        else
            tileValue.WaterLevel

    member this.IsUnderwater (tile: int) (tileValue: TileValue) =
        if this.IsOverrideTile tile then
            this.WaterLevel tile tileValue > this.Elevation tile tileValue
        else
            tileValue.IsUnderwater

    member this.StreamBedY (tile: int) (tileValue: TileValue) (unitHeight: float32) =
        if this.IsOverrideTile tile then
            (float32 (this.Elevation tile tileValue) + HexMetrics.streamBedElevationOffset)
            * unitHeight
        else
            tileValue.StreamBedY unitHeight

    member this.RiverSurfaceY (tile: int) (tileValue: TileValue) (unitHeight: float32) =
        if this.IsOverrideTile tile then
            (float32 (this.Elevation tile tileValue) + HexMetrics.waterElevationOffset)
            * unitHeight
        else
            tileValue.StreamBedY unitHeight

    member this.WaterSurfaceY (tile: int) (tileValue: TileValue) (unitHeight: float32) =
        if this.IsOverrideTile tile then
            (float32 (this.WaterLevel tile tileValue) + HexMetrics.waterElevationOffset)
            * unitHeight
        else
            tileValue.WaterSurfaceY unitHeight

    member this.HasRivers (tile: int) (tileFlag: TileFlag) =
        not <| this.IsOverrideNoRiver tile && tileFlag.HasRivers

    member this.HasIncomingRiver (tile: int) (tileFlag: TileFlag) =
        not <| this.IsOverrideNoRiver tile && tileFlag.HasIncomingRiver

    member this.HasOutgoingRiver (tile: int) (tileFlag: TileFlag) =
        not <| this.IsOverrideNoRiver tile && tileFlag.HasOutgoingRiver

    member this.HasRiverBeginOrEnd (tile: int) (tileFlag: TileFlag) =
        not <| this.IsOverrideNoRiver tile && tileFlag.HasRiverBeginOrEnd

    member this.HasRiverThroughEdge (tile: int) (tileFlag: TileFlag) (dir: int) =
        not <| this.IsOverrideNoRiver tile && tileFlag.HasRiverThroughEdge dir

    member this.HasIncomingRiverThroughEdge (tile: int) (tileFlag: TileFlag) (dir: int) =
        not <| this.IsOverrideNoRiver tile && tileFlag.HasIncomingRiverThoughEdge dir

    member this.HasRoads (tile: int) (tileFlag: TileFlag) =
        not <| this.IsOverrideNoRoad tile && tileFlag.HasRoads

    member this.HasRoadThroughEdge (tile: int) (tileFlag: TileFlag) (dir: int) =
        not <| this.IsOverrideNoRoad tile && tileFlag.HasRoadThroughEdge dir

    member this.Walled (tile: int) (tileFlag: TileFlag) =
        if this.IsOverrideTile tile && this.WalledMode <> OptionalToggle.Ignore then
            this.WalledMode = OptionalToggle.Yes
        else
            tileFlag.Walled

    member this.UrbanLevel (tile: int) (tileValue: TileValue) =
        if this.IsOverrideTile tile && this.ApplyUrbanLevel then
            this.ActiveUrbanLevel
        else
            tileValue.UrbanLevel

    member this.FarmLevel (tile: int) (tileValue: TileValue) =
        if this.IsOverrideTile tile && this.ApplyFarmLevel then
            this.ActiveFarmLevel
        else
            tileValue.FarmLevel

    member this.PlantLevel (tile: int) (tileValue: TileValue) =
        if this.IsOverrideTile tile && this.ApplyPlantLevel then
            this.ActivePlantLevel
        else
            tileValue.PlantLevel

    member this.SpecialIndex (tile: int) (tileValue: TileValue) =
        if this.IsOverrideTile tile && this.ApplySpecialIndex then
            this.ActiveSpecialIndex
        else
            tileValue.SpecialIndex

    member this.IsSpecial (tile: int) (tileValue: TileValue) = this.SpecialIndex tile tileValue <> 0
