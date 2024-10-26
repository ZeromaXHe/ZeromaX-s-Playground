namespace FrontEnd4IdleStrategyFS.Display

open BackEnd4IdleStrategyFS.Game
open FrontEnd4IdleStrategyFS.Common

module MapBoardFS =
    open System
    open FSharp.Control.Reactive
    open Godot
    open DomainT
    open EventT

    let private territorySrcId = 0
    let private territoryAtlasCoords =
        [| Vector2I(0, 2); Vector2I(1, 2); Vector2I(2, 2); Vector2I(3, 2)
           Vector2I(0, 3); Vector2I(1, 3); Vector2I(2, 3); Vector2I(3, 3) |]
    
    let subscribeEventSubject
        (eventSubject: EventSubject)
        gameState
        (territory: TileMapLayer)
        (tileGuiFactory: Action<int, int * int, int>)
        (tileGuiPopulationChanger: Action<int, int>)
        =

        eventSubject.tileConquered
        |> Observable.subscribe (fun e ->
            GD.Print($"地块 {e.id} 被玩家 {e.conquerorId} 占领！")
            let (TileId tileId) = e.id
            let (PlayerId conquerorId) = e.conquerorId
            territory.SetCell(BackEndUtil.fromBackEndI(e.coord),
                              territorySrcId, territoryAtlasCoords[conquerorId - 1])
            if e.loserId.IsNone then
                tileGuiFactory.Invoke(tileId, e.coord, e.population / 1<Pop>)
            )
        |> ignore
        
        eventSubject.tilePopulationChanged
        |> Observable.subscribe (fun e ->
            let (TileId tileId) = e.id
            tileGuiPopulationChanger.Invoke(tileId, e.afterPopulation / 1<Pop>)
            )
        |> ignore