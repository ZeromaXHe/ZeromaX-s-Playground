namespace BackEnd4IdleStrategyFS.Game

module EventT =
    open DomainT

    /// 地块被占领事件
    type TileConqueredEvent =
        { id: TileId
          coord: int * int
          population: int<Pop>
          conquerorId: PlayerId
          loserId: PlayerId option }

    /// 地块人口变化事件
    type TilePopulationChangedEvent =
        { id: TileId
          beforePopulation: int<Pop>
          afterPopulation: int<Pop> }

    /// 地块添加事件
    type TileAddedEvent = { tileId: TileId; coord: int * int }
