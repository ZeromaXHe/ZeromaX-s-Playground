namespace TO.Domains.Functions.Chunks

open Friflo.Engine.ECS
open TO.Domains.Functions.HexMetrics
open TO.Domains.Functions.HexSpheres.Components
open TO.Domains.Functions.HexSpheres.Components.Tiles
open TO.Domains.Types.Chunks
open TO.Domains.Types.Configs
open TO.Domains.Types.HexSpheres
open TO.Domains.Types.PlanetHuds

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-07-07 20:35:07
module TileOverriderQuery =
    let isOverridingTileConnection (env: #IPlanetHudQuery) : IsOverridingTileConnection =
        fun (chunk: IChunk) (tileId: TileId) (neighborId: TileId) ->
            chunk :? IEditPreviewChunk
            && env.PlanetHudOpt |> Option.map _.EditMode |> Option.defaultValue false
            && chunk.EditingTileIds.Count > 0
            && chunk.EditingTileIds.Contains tileId
            && not <| chunk.EditingTileIds.Contains neighborId

    let private isOverrideTile (env: #IPlanetHudQuery) =
        fun (chunk: IChunk) (tileId: TileId) ->
            chunk :? IEditPreviewChunk
            && env.PlanetHudOpt |> Option.map _.EditMode |> Option.defaultValue false
            && chunk.EditingTileIds.Contains tileId

    let private isOverrideNoRiver (env: #IPlanetHudQuery) =
        fun (chunk: IChunk) (tileId: TileId) ->
            isOverrideTile env chunk tileId
            && env.PlanetHudOpt.Value.RiverMode = OptionalToggle.No

    let private isOverrideNoRoad (env: #IPlanetHudQuery) =
        fun (chunk: IChunk) (tileId: TileId) ->
            isOverrideTile env chunk tileId
            && env.PlanetHudOpt.Value.RoadMode = OptionalToggle.No

    let getOverrideElevation (env: #IPlanetHudQuery) : GetOverrideElevation =
        fun (chunk: IChunk) (tile: Entity) ->
            if isOverrideTile env chunk tile.Id && env.PlanetHudOpt.Value.ApplyElevation then
                env.PlanetHudOpt.Value.ActiveElevation
            else
                tile |> Tile.value |> TileValue.elevation

    let getOverrideHeight
        (env: 'E when 'E :> ITileOverriderQuery and 'E :> ICatlikeCodingNoiseQuery and 'E :> IPlanetConfigQuery)
        : GetOverrideHeight =
        fun (chunk: IChunk) (tile: Entity) ->
            if isOverrideTile env chunk tile.Id then
                (float32 (env.GetOverrideElevation chunk tile)
                 + env.GetPerturbHeight tile
                 + 0.05f)
                * env.PlanetConfig.UnitHeight
            else
                env.GetHeight tile

    let getOverrideEdgeType (env: #ITileOverriderQuery) : GetOverrideEdgeType =
        fun (chunk: IChunk) (tile1: Entity) (tile2: Entity) ->
            HexMetrics.getEdgeType
            <| env.GetOverrideElevation chunk tile1
            <| env.GetOverrideElevation chunk tile2

    let getOverrideWaterLevel (env: #IPlanetHudQuery) : GetOverrideWaterLevel =
        fun (chunk: IChunk) (tile: Entity) ->
            if isOverrideTile env chunk tile.Id && env.PlanetHudOpt.Value.ApplyWaterLevel then
                env.PlanetHudOpt.Value.ActiveWaterLevel
            else
                tile |> Tile.value |> TileValue.waterLevel

    let isOverrideUnderwater (env: #ITileOverriderQuery) : IsOverrideUnderwater =
        fun (chunk: IChunk) (tile: Entity) ->
            if isOverrideTile env chunk tile.Id then
                env.GetOverrideWaterLevel chunk tile > env.GetOverrideElevation chunk tile
            else
                tile |> Tile.value |> TileValue.isUnderwater

    let getOverrideStreamBedY
        (env: 'E when 'E :> ITileOverriderQuery and 'E :> IPlanetConfigQuery)
        : GetOverrideStreamBedY =
        fun (chunk: IChunk) (tile: Entity) ->
            if isOverrideTile env chunk tile.Id then
                (float32 (env.GetOverrideElevation chunk tile)
                 + HexMetrics.streamBedElevationOffset)
                * env.PlanetConfig.UnitHeight
            else
                tile |> Tile.value |> TileValue.streamBedY env.PlanetConfig.UnitHeight

    let getOverrideRiverSurfaceY
        (env: 'E when 'E :> ITileOverriderQuery and 'E :> IPlanetConfigQuery)
        : GetOverrideRiverSurfaceY =
        fun (chunk: IChunk) (tile: Entity) ->
            if isOverrideTile env chunk tile.Id then
                (float32 (env.GetOverrideElevation chunk tile) + HexMetrics.waterElevationOffset)
                * env.PlanetConfig.UnitHeight
            else
                tile |> Tile.value |> TileValue.riverSurfaceY env.PlanetConfig.UnitHeight

    let getOverrideWaterSurfaceY
        (env: 'E when 'E :> ITileOverriderQuery and 'E :> IPlanetConfigQuery)
        : GetOverrideWaterSurfaceY =
        fun (chunk: IChunk) (tile: Entity) ->
            if isOverrideTile env chunk tile.Id then
                (float32 (env.GetOverrideElevation chunk tile) + HexMetrics.waterElevationOffset)
                * env.PlanetConfig.UnitHeight
            else
                tile |> Tile.value |> TileValue.waterSurfaceY env.PlanetConfig.UnitHeight

    let hasOverrideRivers env : HasOverrideRivers =
        fun (chunk: IChunk) (tile: Entity) ->
            not <| isOverrideNoRiver env chunk tile.Id
            && tile |> Tile.flag |> TileFlag.hasRivers

    let hasOverrideIncomingRiver env : HasOverrideIncomingRiver =
        fun (chunk: IChunk) (tile: Entity) ->
            not <| isOverrideNoRiver env chunk tile.Id
            && tile |> Tile.flag |> TileFlag.hasIncomingRiver

    let hasOverrideOutgoingRiver env : HasOverrideOutgoingRiver =
        fun (chunk: IChunk) (tile: Entity) ->
            not <| isOverrideNoRiver env chunk tile.Id
            && tile |> Tile.flag |> TileFlag.hasOutgoingRiver

    let hasOverrideRiverBeginOrEnd env : HasOverrideRiverBeginOrEnd =
        fun (chunk: IChunk) (tile: Entity) ->
            not <| isOverrideNoRiver env chunk tile.Id
            && tile |> Tile.flag |> TileFlag.hasRiverBeginOrEnd

    let hasOverrideRiverThroughEdge env : HasOverrideRiverThroughEdge =
        fun (chunk: IChunk) (tile: Entity) (dir: int) ->
            not <| isOverrideNoRiver env chunk tile.Id
            && tile |> Tile.flag |> TileFlag.hasRiver dir

    let hasOverrideIncomingRiverThroughEdge env : HasOverrideIncomingRiverThroughEdge =
        fun (chunk: IChunk) (tile: Entity) (dir: int) ->
            not <| isOverrideNoRiver env chunk tile.Id
            && tile |> Tile.flag |> TileFlag.hasRiverIn dir

    let hasOverrideRoads env : HasOverrideRoads =
        fun (chunk: IChunk) (tile: Entity) ->
            not <| isOverrideNoRoad env chunk tile.Id
            && tile |> Tile.flag |> TileFlag.hasRoads

    let hasOverrideRoadThroughEdge env : HasOverrideRoadThroughEdge =
        fun (chunk: IChunk) (tile: Entity) (dir: int) ->
            not <| isOverrideNoRoad env chunk tile.Id
            && tile |> Tile.flag |> TileFlag.hasRoad dir

    let getOverrideWalled env : GetOverrideWalled =
        fun (chunk: IChunk) (tile: Entity) ->
            if
                isOverrideTile env chunk tile.Id
                && env.PlanetHudOpt.Value.WalledMode <> OptionalToggle.Ignore
            then
                env.PlanetHudOpt.Value.WalledMode = OptionalToggle.Yes
            else
                tile |> Tile.flag |> TileFlag.walled

    let getOverrideUrbanLevel env : GetOverrideUrbanLevel =
        fun (chunk: IChunk) (tile: Entity) ->
            if isOverrideTile env chunk tile.Id && env.PlanetHudOpt.Value.ApplyUrbanLevel then
                env.PlanetHudOpt.Value.ActiveUrbanLevel
            else
                tile |> Tile.value |> TileValue.urbanLevel

    let getOverrideFarmLevel env : GetOverrideFarmLevel =
        fun (chunk: IChunk) (tile: Entity) ->
            if isOverrideTile env chunk tile.Id && env.PlanetHudOpt.Value.ApplyFarmLevel then
                env.PlanetHudOpt.Value.ActiveFarmLevel
            else
                tile |> Tile.value |> TileValue.farmLevel

    let getOverridePlantLevel env : GetOverridePlantLevel =
        fun (chunk: IChunk) (tile: Entity) ->
            if isOverrideTile env chunk tile.Id && env.PlanetHudOpt.Value.ApplyPlantLevel then
                env.PlanetHudOpt.Value.ActivePlantLevel
            else
                tile |> Tile.value |> TileValue.plantLevel

    let getOverrideSpecialIndex env : GetOverrideSpecialIndex =
        fun (chunk: IChunk) (tile: Entity) ->
            if isOverrideTile env chunk tile.Id && env.PlanetHudOpt.Value.ApplySpecialIndex then
                env.PlanetHudOpt.Value.ActiveSpecialIndex
            else
                tile |> Tile.value |> TileValue.specialIndex

    let isOverrideSpecial (env: #ITileOverriderQuery) : IsOverrideSpecial =
        fun (chunk: IChunk) (tile: Entity) -> env.GetOverrideSpecialIndex chunk tile <> 0
