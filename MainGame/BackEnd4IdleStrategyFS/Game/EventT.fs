namespace BackEnd4IdleStrategyFS.Game

module EventT =
    open DomainT

    /// 地块被占领事件
    type TileConqueredEvent =
        { TileId: TileId
          Coord: Coord
          Population: int<Pop>
          ConquerorId: PlayerId
          LoserId: PlayerId option }

    /// 地块被占领
    type TileConquered<'s> = TileConqueredEvent -> 's -> 's

    /// 地块人口变化事件
    type TilePopulationChangedEvent =
        { TileId: TileId
          BeforePopulation: int<Pop>
          AfterPopulation: int<Pop> }

    /// 地块人口变化
    type TilePopulationChanged<'s> = TilePopulationChangedEvent -> 's -> 's

    /// 地块添加事件
    type TileAddedEvent = { TileId: TileId; Coord: Coord }

    /// 地块添加
    type TileAdded<'s> = TileAddedEvent -> 's -> 's

    /// 部队新建事件
    type MarchingArmyAddedEvent =
        { MarchingArmyId: MarchingArmyId
          Population: int<Pop>
          FromTileId: TileId
          ToTileId: TileId
          PlayerId: PlayerId }

    /// 部队新建
    type MarchingArmyAdded<'s> = MarchingArmyAddedEvent -> 's -> 's

    /// 部队抵达事件
    type MarchingArmyArrivedEvent =
        { MarchingArmyId: MarchingArmyId
          Population: int<Pop>
          DestinationTileId: TileId
          PlayerId: PlayerId }

    /// 部队抵达
    type MarchingArmyArrived<'s> = MarchingArmyArrivedEvent -> 's -> 's
