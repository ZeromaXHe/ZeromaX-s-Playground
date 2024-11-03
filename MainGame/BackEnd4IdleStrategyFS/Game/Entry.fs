namespace BackEnd4IdleStrategyFS.Game

open BackEnd4IdleStrategyFS.Game.RepositoryT
open FSharp.Control.Reactive
open System
open System.Reactive.Subjects
open BackEnd4IdleStrategyFS.Game.EventT
open BackEnd4IdleStrategyFS.Game.DomainT
open BackEnd4IdleStrategyFS.Godot.IAdapter

module Entry =

    type Container(aStar: IAStar2D<GameState>, terrainLayer: ITileMapLayer<GameState>, playerCount: int) =
        let mutable gameState = emptyGameState

        let random = Random()
        let tileAdded = new Subject<TileAddedEvent>()
        let tileConquered = new Subject<TileConqueredEvent>()
        let tilePopulationChanged = new Subject<TilePopulationChangedEvent>()
        let gameTicked = Observable.interval (TimeSpan.FromSeconds(0.5))
        let gameFirstArmyGenerated = Observable.timerSpan (TimeSpan.FromSeconds(3))
        let marchingArmyAdded = new Subject<MarchingArmyAddedEvent>()
        let marchingArmyArrived = new Subject<MarchingArmyArrivedEvent>()

        let tileAddedSubscription =
            tileAdded
            |> Observable.subscribe
                (fun e ->
                    let (TileId tileId) = e.TileId
                    aStar.AddPoint tileId e.Coord gameState |> ignore)

        let marchingArmyAddedSubscription =
            marchingArmyAdded
            |> Observable.delayMap (fun e ->
                // 计算延迟到达时间
                let speed = DomainF.marchSpeed e.Population
                let delay = 100.0 / float speed
                Observable.timerSpan (TimeSpan.FromSeconds(delay)))
            |> Observable.subscribe
                (fun e ->
                    // 转发抵达事件
                    marchingArmyArrived.OnNext
                        { MarchingArmyId = e.MarchingArmyId
                          Population = e.Population
                          DestinationTileId = e.ToTileId
                          PlayerId = e.PlayerId })

        let onNextWrapper onNext e s =
            onNext e
            s

        let marchingArmyArrivedSubscription =
            marchingArmyArrived
            |> Observable.subscribe
                (fun e ->
                    let marchingArmy = QueryRepositoryF.getMarchingArmy e.MarchingArmyId gameState
                    // 处理抵达部队
                    if marchingArmy.IsSome then
                        gameState <-
                            DomainF.arriveArmy
                                QueryRepositoryF.getTile
                                QueryRepositoryF.getPlayer
                                CommandRepositoryF.deleteMarchingArmy
                                CommandRepositoryF.updateTile
                                (onNextWrapper tileConquered.OnNext)
                                marchingArmy.Value
                                gameState
                    let player = QueryRepositoryF.getPlayer e.PlayerId gameState
                    // 派出新的部队
                    if player.IsSome then
                        let gameState', _ =
                            DomainF.marchArmy
                                CommandRepositoryF.insertMarchingArmy
                                (onNextWrapper marchingArmyAdded.OnNext)
                                QueryRepositoryF.getTilesByPlayer
                                CommandRepositoryF.updateTile
                                aStar
                                random
                                player.Value
                                gameState
                        gameState <- gameState'
                )

        let gameTickedSubscription =
            gameTicked
            |> Observable.subscribe
                (fun _ ->
                    gameState <-
                        DomainF.increaseAllPlayerTilesPopulation
                            QueryRepositoryF.getAllTiles
                            CommandRepositoryF.updateTile
                            (onNextWrapper tilePopulationChanged.OnNext)
                            1<Pop>
                            gameState)

        let gameFirstArmyGeneratedSubscription =
            gameFirstArmyGenerated
            |> Observable.subscribe
                (fun _ ->
                    gameState <-
                        QueryRepositoryF.getAllPlayers gameState
                        |> Seq.fold
                            (fun s p ->
                                let s', _ =
                                    DomainF.marchArmy
                                        CommandRepositoryF.insertMarchingArmy
                                        (onNextWrapper marchingArmyAdded.OnNext)
                                        QueryRepositoryF.getTilesByPlayer
                                        CommandRepositoryF.updateTile
                                        aStar
                                        random
                                        p
                                        s

                                s')
                            gameState)

        member this.GameTicked = gameTicked |> Observable.asObservable

        member this.GameFirstArmyGenerated = gameFirstArmyGenerated |> Observable.asObservable

        member this.TileAdded = tileAdded |> Observable.asObservable

        member this.TileConquered = tileConquered |> Observable.asObservable

        member this.TilePopulationChanged = tilePopulationChanged |> Observable.asObservable

        member this.MarchingArmyAdded = marchingArmyAdded |> Observable.asObservable

        member this.MarchingArmyArrived = marchingArmyArrived |> Observable.asObservable

        member this.QueryAllPlayers() = AppService.queryAllPlayers gameState

        member this.QueryTileById tileIdInt =
            AppService.queryTileById gameState tileIdInt

        member this.QueryTilesByPlayerId playerIdInt =
            AppService.queryTilesByPlayerId gameState playerIdInt

        member this.Init() =
            let usedCells() = terrainLayer.GetUsedCells gameState

            let combineF =
                DomainF.createTiles
                    (fun c _ _ -> CommandRepositoryF.insertTile c)
                    (onNextWrapper tileAdded.OnNext)
                    (usedCells())
                >> (DomainF.initTilesConnections QueryRepositoryF.getTileByCoord terrainLayer aStar)
                >> (DomainF.spawnPlayers
                        QueryRepositoryF.getAllTiles
                        CommandRepositoryF.updateTile
                        CommandRepositoryF.insertPlayer
                        (onNextWrapper tileConquered.OnNext)
                        random
                        playerCount)

            do gameState <- combineF gameState
