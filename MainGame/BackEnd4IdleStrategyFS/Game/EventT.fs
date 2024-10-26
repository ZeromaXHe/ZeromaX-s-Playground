namespace BackEnd4IdleStrategyFS.Game

module EventT =
    open System.Reactive.Subjects
    open DomainT

    /// 地块被占领事件
    type TileConqueredEvent =
        { id: TileId
          conquerorId: PlayerId
          loserId: PlayerId option }

    /// 地块人口变化事件
    type TilePopulationChangedEvent =
        { id: TileId
          beforePopulation: int<Pop>
          afterPopulation: int<Pop> }

    /// 事件主题
    type EventSubject = {
        tileConquered: Subject<TileConqueredEvent>
        tilePopulationChanged: Subject<TilePopulationChangedEvent>
    }