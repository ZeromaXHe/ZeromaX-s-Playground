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
          MarchingArmyDeleter: MarchingArmyDeleter<'s>
          MarchingArmyQueryById: MarchingArmyQueryById<'s>
          // 事件
          TileConquered: TileConquered
          TilePopulationChanged: TilePopulationChanged
          TileAdded: TileAdded
          MarchingArmyAdded: MarchingArmyAdded
          MarchingArmyArrived: MarchingArmyArrived }
