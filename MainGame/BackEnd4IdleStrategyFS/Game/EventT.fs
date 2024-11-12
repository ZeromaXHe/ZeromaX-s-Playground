namespace BackEnd4IdleStrategyFS.Game

open DomainT

module EventT =

    /// 地块被占领事件
    type TileConqueredEvent =
        { TileId: TileId
          Coord: Coord
          Population: int<Pop>
          ConquerorId: PlayerId option
          LoserId: PlayerId option }

    /// 地块被占领
    type TileConquered = TileConqueredEvent -> unit

    /// 地块人口变化事件
    type TilePopulationChangedEvent =
        { TileId: TileId
          BeforePopulation: int<Pop>
          AfterPopulation: int<Pop> }

    /// 地块人口变化
    type TilePopulationChanged = TilePopulationChangedEvent -> unit

    /// 地块添加事件
    type TileAddedEvent = { TileId: TileId; Coord: Coord }

    /// 地块添加
    type TileAdded = TileAddedEvent -> unit

    /// 部队新建事件
    type MarchingArmyAddedEvent =
        { MarchingArmyId: MarchingArmyId
          Population: int<Pop>
          FromTileId: TileId
          ToTileId: TileId
          PlayerId: PlayerId }

    /// 部队新建
    type MarchingArmyAdded = MarchingArmyAddedEvent -> unit

    /// 部队抵达事件
    type MarchingArmyArrivedEvent =
        { MarchingArmyId: MarchingArmyId
          Population: int<Pop>
          DestinationTileId: TileId
          PlayerId: PlayerId }

    /// 部队抵达
    type MarchingArmyArrived = MarchingArmyArrivedEvent -> unit
