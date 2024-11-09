namespace BackEnd4IdleStrategyFS.Game

open System
open BackEnd4IdleStrategyFS.Godot.IAdapter
open DomainT
open EventT

module private Dependency =
    type Injector<'s> =
        {
          // Godot
          AStar: IAStar2D
          TerrainLayer: ITileMapLayer
          // 随机
          Random: Random
          // 仓储
          PlayerFactory: PlayerFactoryM<'s>
          PlayerQueryById: PlayerQueryByIdM<'s>
          TileFactory: TileFactoryM<'s>
          TileUpdater: TileUpdaterM<'s>
          TileQueryById: TileQueryByIdM<'s>
          TileQueryByCoord: TileQueryByCoordM<'s>
          TilesQueryByPlayer: TilesQueryByPlayerM<'s>
          TilesQueryAll: TilesQueryAllM<'s>
          MarchingArmyFactory: MarchingArmyFactoryM<'s>
          MarchingArmyDeleter: MarchingArmyDeleterM<'s>
          // 事件
          TileConquered: TileConqueredM
          TilePopulationChanged: TilePopulationChangedM
          TileAdded: TileAddedM
          MarchingArmyAdded: MarchingArmyAddedM
          MarchingArmyArrived: MarchingArmyArrivedM }
