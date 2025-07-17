namespace TO.Domains.Functions.Units

open System
open Friflo.Engine.ECS
open TO.Domains.Functions.HexSpheres.Components
open TO.Domains.Types.Friflos
open TO.Domains.Types.HexSpheres
open TO.Domains.Types.HexSpheres.Components
open TO.Domains.Types.PathFindings
open TO.Domains.Types.Units

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-07-11 19:05:11
module UnitManagerCommand =
    let addUnit (env: 'E when 'E :> IUnitManagerQuery and 'E :> IEntityStoreQuery and 'E :> IHexUnitCommand) : AddUnit =
        fun (tileId: TileId) (orientation: float32) ->
            match env.UnitManagerOpt with
            | None -> ()
            | Some manager ->
                let unit = manager.InstantiateUnit()
                let unitId = env.EntityStore.CreateEntity(UnitComponent 0).Id
                unit.Id <- unitId
                manager.Units.Add(unitId, unit)
                env.ChangeUnitTileId tileId unit
                unit.Orientation <- orientation

    let removeUnit (env: 'E when 'E :> IUnitManagerQuery and 'E :> IHexUnitCommand) : RemoveUnit =
        fun (unitId: int) ->
            match env.UnitManagerOpt with
            | None -> ()
            | Some manager ->
                env.KillUnit manager.Units[unitId]
                manager.Units.Remove unitId |> ignore

    let validateUnitLocationById
        (env: 'E when 'E :> IUnitManagerQuery and 'E :> IHexUnitCommand)
        : ValidateUnitLocationById =
        fun (unitId: int) ->
            match env.UnitManagerOpt with
            | None -> ()
            | Some manager -> env.ValidateUnitLocation manager.Units[unitId]

    let findUnitPath
        (env:
            'E
                when 'E :> IUnitManagerQuery
                and 'E :> ITileSearcherCommand
                and 'E :> ITileQuery
                and 'E :> IHexUnitPathPoolCommand
                and 'E :> IHexUnitCommand)
        : FindUnitPath =
        fun (tileId: TileId Nullable) ->
            match env.UnitManagerOpt with
            | None -> ()
            | Some manager ->
                if manager.PathFromTileId <> 0 then
                    if not tileId.HasValue || tileId.Value = manager.PathFromTileId then
                        // 重复点选同一地块，则取消选择
                        env.ClearTileSearchPath()
                        manager.PathFromTileId <- 0
                    else
                        let fromTile = env.GetTile manager.PathFromTileId
                        let toTile = env.GetTile tileId.Value
                        let path = env.FindTileSearchPath fromTile toTile true

                        if path.Count > 1 then
                            // 确实有找到从出发点到 tile 的路径
                            env.NewUnitPathTask(path |> Seq.map _.Id |> Seq.toArray)
                            |> Option.iter (env.TravelUnit manager.Units[fromTile |> Tile.unitId |> _.UnitId])

                        env.ClearTileSearchPath()
                        manager.PathFromTileId <- 0
                else if
                    // 当前没有选择地块（即没有选中单位）的话，则在有单位时选择该地块
                    tileId.HasValue && (env.GetTile tileId.Value) |> Tile.unitId |> _.UnitId <> 0
                then
                    manager.PathFromTileId <- tileId.Value
                else
                    env.ClearTileSearchPath()
                    manager.PathFromTileId <- 0

    let clearAllUnits
        (env:
            'E
                when 'E :> IUnitManagerQuery
                and 'E :> IHexUnitCommand
                and 'E :> IEntityStoreQuery
                and 'E :> IEntityStoreCommand)
        : ClearAllUnits =
        fun () ->
            match env.UnitManagerOpt with
            | None -> ()
            | Some manager ->
                for unit in manager.Units.Values do
                    env.KillUnit unit

                manager.Units.Clear()

                env.ExecuteInCommandBuffer(fun cb ->
                    env.Query<UnitComponent>().ForEachEntity(fun _ e -> cb.DeleteEntity(e.Id)))
