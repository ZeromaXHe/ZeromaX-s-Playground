namespace BackEnd4IdleStrategyFS.Game

open System
open BackEnd4IdleStrategyFS.Godot.IAdapter
open DomainT
open EventT

module Dependency =
    type Injector<'s> =
        {
          // Godot
          AStar: IAStar2D
          TerrainLayer: ITileMapLayer
          // 随机
          Random: Random
          // 日志
          LogPrint: string -> unit
          // 仓储
          SpeedMultiplierQuery: SpeedMultiplierQuery<'s>
          PlayerFactory: PlayerFactory<'s>
          PlayerQueryById: PlayerQueryById<'s>
          PlayersQueryAll: PlayersQueryAll<'s>
          TileFactory: TileFactory<'s>
          TileUpdater: TileUpdater<'s>
          TileQueryById: TileQueryById<'s>
          TileQueryByCoord: TileQueryByCoord<'s>
          TilesQueryByPlayer: TilesQueryByPlayer<'s>
          TilesQueryAll: TilesQueryAll<'s>
          MarchingArmyFactory: MarchingArmyFactory<'s>
          MarchingArmyUpdater: MarchingArmyUpdater<'s>
          MarchingArmyDeleter: MarchingArmyDeleter<'s>
          MarchingArmyQueryById: MarchingArmyQueryById<'s>
          MarchingArmiesQueryAll: MarchingArmiesQueryAll<'s>
          // 事件
          PlayerAdded: PlayerAdded
          TileConquered: TileConquered
          TilePopulationChanged: TilePopulationChanged
          TileAdded: TileAdded
          MarchingArmyAdded: MarchingArmyAdded
          MarchingArmyArrived: MarchingArmyArrived }
