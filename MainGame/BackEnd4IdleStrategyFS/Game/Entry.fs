namespace BackEnd4IdleStrategyFS.Game

open BackEnd4IdleStrategyFS.Godot.Adapter

module Entry =
    open FSharp.Control.Reactive
    open System.Reactive.Subjects
    open DomainT
    open EventT

    type Container(aStar: IAStar2D) =
        let mutable gameState = AppService.emptyGameState
        let tileAdded = new Subject<TileAddedEvent>()
        let tileConquered = new Subject<TileConqueredEvent>()
        let tilePopulationChanged = new Subject<TilePopulationChangedEvent>()

        let tileAddedSubscription =
            tileAdded
            |> Observable.subscribe (fun e ->
                let (TileId tileId) = e.tileId
                aStar.AddPoint tileId e.coord)

        member this.TileConquered = tileConquered |> Observable.asObservable

        member this.TilePopulationChanged = tilePopulationChanged |> Observable.asObservable

        member this.InitTiles usedCells =
            let gameState', tiles = AppService.initTiles tileAdded gameState usedCells
            gameState <- gameState'
            tiles

        member this.InitPlayerAndSpawnOnTile tileCoords =
            gameState <- AppService.initPlayerAndSpawnOnTile tileConquered gameState tileCoords

        member this.RandomSendMarchingArmy playerIdInt =
            // 必须这样转一道……
            let navService = aStar.GetPointConnections

            let gameState', marchingArmy =
                AppService.randomSendMarchingArmy gameState playerIdInt navService

            gameState <- gameState'
            marchingArmy

        member this.MarchingArmyArriveDestination marchingArmyIdInt =
            let gameState', playerIdInt =
                AppService.marchingArmyArriveDestination tileConquered gameState marchingArmyIdInt

            gameState <- gameState'
            playerIdInt

        member this.AddPopulationToPlayerTiles incrInt =
            gameState <- AppService.addPopulationToPlayerTiles tilePopulationChanged gameState incrInt

        member this.QueryAllPlayers() = AppService.queryAllPlayers gameState

        member this.QueryPlayerById playerIdInt =
            AppService.queryPlayerById gameState playerIdInt

        member this.QueryTileById tileIdInt =
            AppService.queryTileById gameState tileIdInt

        member this.QueryTilesByPlayerId playerIdInt =
            AppService.queryTilesByPlayerId gameState playerIdInt
