namespace TO.Domains.Functions.Units

open System
open Friflo.Engine.ECS
open Godot
open TO.Domains.Functions.HexSpheres.Components
open TO.Domains.Functions.HexSpheres.Components.Tiles
open TO.Domains.Functions.Maths
open TO.Domains.Types.Configs
open TO.Domains.Types.Friflos
open TO.Domains.Types.HexSpheres
open TO.Domains.Types.HexSpheres.Components
open TO.Domains.Types.HexSpheres.Components.Tiles
open TO.Domains.Types.PathFindings
open TO.Domains.Types.Shaders
open TO.Domains.Types.Units

module HexUnit =
    let visionRange = 3

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-07-11 09:47:11
module HexUnitCommand =
    let private increaseVisibility
        (env: 'E when 'E :> ITileQuery and 'E :> ITileSearcherCommand and 'E :> ITileShaderDataCommand)
        (fromTileId: TileId)
        (range: int)
        =
        let fromTile = env.GetTile fromTileId
        let tileIds = env.GetVisibleTiles fromTile range

        for tileId in tileIds do
            let tile = env.GetTile tileId
            let tileVis = tile |> Tile.visibility
            let newTileVis = tileVis.Visibility + 1 |> TileVisibility
            tile.AddComponent(&newTileVis) |> ignore

            if newTileVis.Visibility = 1 then
                let newTileFlag =
                    tile |> Tile.flag |> TileFlag.withMask TileFlagEnum.Explored |> TileFlag

                tile.AddComponent(&newTileFlag) |> ignore

                env.RefreshTileShaderDataVisibility tile.Id
                <| Tile.countId tile
                <| Tile.flag tile
                <| Tile.visibility tile

    let private decreaseVisibility
        (env: 'E when 'E :> ITileQuery and 'E :> ITileSearcherCommand and 'E :> ITileShaderDataCommand)
        (fromTileId: TileId)
        (range: int)
        =
        let fromTile = env.GetTile fromTileId
        let tileIds = env.GetVisibleTiles fromTile range

        for tileId in tileIds do
            let tile = env.GetTile tileId
            let tileVis = tile |> Tile.visibility
            let newTileVis = tileVis.Visibility - 1 |> TileVisibility
            tile.AddComponent(&newTileVis) |> ignore

            if newTileVis.Visibility = 0 then
                env.RefreshTileShaderDataVisibility tile.Id
                <| Tile.countId tile
                <| Tile.flag tile
                <| Tile.visibility tile

    let pathRotationSpeed = Mathf.Pi
    let pathMoveSpeed = 30f // 每秒走 30f 标准距离

    let onHexUnitProcessed (env: #IPlanetConfigQuery) : OnHexUnitProcessed =
        fun (delta: float32) (unit: IHexUnit) ->
            if unit.Path <> null then
                let deltaProgress = delta * env.PlanetConfig.StandardScale * pathMoveSpeed

                if unit.PathOriented then
                    let prePathTileIdx = unit.PathTileIdx
                    let progress = unit.Path |> HexUnitPathQuery.getUnitPathProcess

                    while unit.PathTileIdx < unit.Path.Progresses.Length
                          && unit.Path.Progresses[unit.PathTileIdx] < progress do
                        unit.PathTileIdx <- unit.PathTileIdx + 1

                    if prePathTileIdx <> unit.PathTileIdx then
                        decreaseVisibility env unit.Path.TileIds[prePathTileIdx] HexUnit.visionRange
                        increaseVisibility env unit.Path.TileIds[unit.PathTileIdx] HexUnit.visionRange

                    let before = unit.Path.Curve.SampleBaked(progress - deltaProgress, true)

                    Node3dUtil.AlignYAxisToDirection(
                        unit,
                        unit.Position,
                        alignForward = before.DirectionTo unit.Position
                    )
                else
                    let forward =
                        unit.Position.DirectionTo <| unit.Path.Curve.SampleBaked(deltaProgress, true)

                    let angle = Math3dUtil.GetPlanarAngle(-unit.Basis.Z, forward, unit.Position, true)
                    let deltaAngle = float32 (Single.Sign angle) * pathRotationSpeed * delta

                    if Mathf.Abs deltaAngle >= Mathf.Abs angle then
                        unit.Rotate(unit.Position.Normalized(), angle)
                        unit.PathOriented <- true
                        unit.Path |> HexUnitPathCommand.unitStartMove unit
                    else
                        unit.Rotate(unit.Position.Normalized(), deltaAngle)

    let validateUnitLocation
        (env:
            'E
                when 'E :> IEntityStoreQuery
                and 'E :> ITileQuery
                and 'E :> IPlanetConfigQuery
                and 'E :> ICatlikeCodingNoiseQuery)
        : ValidateUnitLocation =
        fun (hexUnit: IHexUnit) ->
            let unitId = hexUnit.Id
            let unit = env.GetEntityById unitId
            let unitTileId = unit.GetComponent<UnitComponent>().TileId
            let tile = env.GetTile unitTileId

            let position =
                tile
                |> Tile.unitCentroid
                |> TileUnitCentroid.scaled (env.PlanetConfig.Radius + env.GetHeight tile)

            Node3dUtil.PlaceOnSphere(hexUnit, position, env.PlanetConfig.StandardScale, alignForward = Vector3.Up)
            hexUnit.BeginRotation <- hexUnit.Rotation

    let changeUnitTileId
        (env: 'E when 'E :> IEntityStoreQuery and 'E :> ITileQuery and 'E :> IHexUnitCommand)
        : ChangeUnitTileId =
        fun (tileId: TileId) (hexUnit: IHexUnit) ->
            let unitId = hexUnit.Id
            let unit = env.GetEntityById unitId
            let pre = unit.GetComponent<UnitComponent>().TileId

            if pre > 0 then
                decreaseVisibility env pre HexUnit.visionRange
                let emptyUnitId = TileUnitId 0
                (env.GetTile pre).AddComponent(&emptyUnitId) |> ignore

            let newUnitTileId = UnitComponent tileId
            unit.AddComponent(&newUnitTileId) |> ignore
            env.ValidateUnitLocation hexUnit
            increaseVisibility env tileId HexUnit.visionRange
            let newTile = env.GetTile tileId
            let newUnitId = TileUnitId unitId
            newTile.AddComponent(&newUnitId) |> ignore

    let killUnit (env: 'E when 'E :> ITileQuery and 'E :> IEntityStoreQuery) : KillUnit =
        fun (hexUnit: IHexUnit) ->
            let unit = env.GetEntityById hexUnit.Id
            let tileId = unit.GetComponent<UnitComponent>().TileId
            decreaseVisibility env tileId HexUnit.visionRange
            let tile = env.GetTile tileId
            let newTileUnitId = TileUnitId 0
            tile.AddComponent(&newTileUnitId) |> ignore
            unit.DeleteEntity()
            hexUnit.QueueFree()

    let travelUnit (env: 'E when 'E :> ITileQuery and 'E :> IEntityStoreQuery) : TravelUnit =
        fun (hexUnit: IHexUnit) (path: IHexUnitPath) ->
            hexUnit.Path <- path
            hexUnit.PathOriented <- false
            hexUnit.PathTileIdx <- 0
            // 提前把实际单位数据设置到目标 Tile 中
            let unit = env.GetEntityById hexUnit.Id
            let fromTile = env.GetTile <| unit.GetComponent<UnitComponent>().TileId
            let emptyTileUnitId = TileUnitId 0
            fromTile.AddComponent(&emptyTileUnitId) |> ignore
            let toTile = env.GetTile hexUnit.Path.TileIds[hexUnit.Path.TileIds.Length - 1]
            let newTileUnitId = TileUnitId hexUnit.Id
            toTile.AddComponent(&newTileUnitId) |> ignore
            let newUnitTileId = UnitComponent toTile.Id
            unit.AddComponent(&newUnitTileId) |> ignore
