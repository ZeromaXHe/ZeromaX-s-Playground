namespace BackEnd4IdleStrategyFS.Game

open FSharpPlus.Control
open FSharpPlus.Data
open FSharp.Control.Reactive
open System
open System.Reactive.Subjects
open BackEnd4IdleStrategyFS.Godot.IAdapter
open EventT
open DomainT
open Dependency
open RepositoryT

type Entry(aStar: IAStar2D, terrainLayer: ITileMapLayer, playerCount: int, logPrinter: string -> unit) =
    let mutable gameState = emptyGameState

    // TODO: 好像确定开局种子也不能保证后续现象一致。是 Random.Next 触发的顺序不一致导致的吗？
    let seed = 1662646297 // Random().Next Int32.MaxValue
    let random = Random(seed)
    let tileAdded = new Subject<TileAddedEvent>()
    let tileConquered = new Subject<TileConqueredEvent>()
    let tilePopulationChanged = new Subject<TilePopulationChangedEvent>()
    let gameTicked = TimeSpan.FromSeconds 0.5 |> Observable.interval
    let gameFirstArmyGenerated = TimeSpan.FromSeconds 3 |> Observable.timerSpan
    let marchingArmyAdded = new Subject<MarchingArmyAddedEvent>()
    let marchingArmyArrived = new Subject<MarchingArmyArrivedEvent>()

    let injector =
        {
          // Godot
          AStar = aStar
          TerrainLayer = terrainLayer
          // 随机
          Random = random
          // 日志
          LogPrint = logPrinter
          // 仓储
          PlayerFactory = CommandRepositoryF.insertPlayer
          PlayerQueryById = QueryRepositoryF.getPlayer
          PlayersQueryAll = QueryRepositoryF.getAllPlayers
          TileFactory = CommandRepositoryF.insertTile
          TileUpdater = CommandRepositoryF.updateTile
          TileQueryById = QueryRepositoryF.getTile
          TileQueryByCoord = QueryRepositoryF.getTileByCoord
          TilesQueryByPlayer = QueryRepositoryF.getTilesByPlayer
          TilesQueryAll = QueryRepositoryF.getAllTiles
          MarchingArmyFactory = CommandRepositoryF.insertMarchingArmy
          MarchingArmyDeleter = CommandRepositoryF.deleteMarchingArmy
          MarchingArmyQueryById = QueryRepositoryF.getMarchingArmy
          // 事件
          TileConquered = tileConquered.OnNext
          TilePopulationChanged = tilePopulationChanged.OnNext
          TileAdded = tileAdded.OnNext
          MarchingArmyAdded = marchingArmyAdded.OnNext
          MarchingArmyArrived = marchingArmyArrived.OnNext }

    let tileAddedSubscription =
        tileAdded
        |> Observable.subscribe (fun e ->
            let (TileId tileId) = e.TileId
            aStar.AddPoint tileId e.Coord)

    let marchingArmyAddedSubscription =
        marchingArmyAdded
        |> Observable.delayMap (fun e ->
            // 计算延迟到达时间
            let speed = DomainF.marchSpeed e.Population
            let delay = 100.0 / float speed
            TimeSpan.FromSeconds delay |> Observable.timerSpan)
        |> Observable.subscribe (fun e ->
            // 转发抵达事件
            marchingArmyArrived.OnNext
                { MarchingArmyId = e.MarchingArmyId
                  Population = e.Population
                  DestinationTileId = e.ToTileId
                  PlayerId = e.PlayerId })

    let marchingArmyArrivedSubscription =
        let marchingArmyArrivedOnNext e =
            let armyOpt, gameState' =
                AppService.armyArriveAndGenerateNew e.MarchingArmyId e.PlayerId
                |> OptionT.run
                |> StateT.run
                <| gameState
                |> Reader.run
                <| injector

            if armyOpt.IsSome then
                gameState <- gameState'

        marchingArmyArrived |> Observable.subscribe marchingArmyArrivedOnNext

    let gameTickedSubscription =
        let gameTickedOnNext _ =
            let _, gameState' =
                AppService.increaseAllPlayerTilesPopulation 1<Pop> |> StateT.run <| gameState
                |> Reader.run
                <| injector

            gameState <- gameState'

        gameTicked |> Observable.subscribe gameTickedOnNext

    let gameFirstArmyGeneratedSubscription =
        let gameFirstArmyGeneratedOnNext _ =
            let _, s =
                AppService.generateFirstGroupArmy |> StateT.run <| gameState |> Reader.run
                <| injector

            gameState <- s

        gameFirstArmyGenerated |> Observable.subscribe gameFirstArmyGeneratedOnNext

    member this.GameTicked = gameTicked |> Observable.asObservable

    member this.GameFirstArmyGenerated = gameFirstArmyGenerated |> Observable.asObservable

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
        logPrinter $"随机种子：{seed}"

        let _, gameState' =
            AppService.init playerCount |> StateT.run <| gameState |> Reader.run <| injector

        gameState <- gameState'
