namespace BackEnd4IdleStrategyFS.Game

open BackEnd4IdleStrategyFS.Game.RepositoryT
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
    /// 游戏状态
    let mutable gameState = emptyGameState
    /// 游戏统计
    let mutable gameStat = emptyGameStat

    // TODO: 好像确定开局种子也不能保证后续现象一致。是 Random.Next 触发的顺序不一致导致的吗？
    let seed = 1662646297 // Random().Next Int32.MaxValue
    let random = Random(seed)
    let playerAdded = new Subject<PlayerAddedEvent>()
    let tileAdded = new Subject<TileAddedEvent>()
    let tileConquered = new Subject<TileConqueredEvent>()
    let tilePopulationChanged = new Subject<TilePopulationChangedEvent>()
    let gameTicked = TimeSpan.FromSeconds 0.5 |> Observable.interval
    let gameFirstArmyGenerated = TimeSpan.FromSeconds 3 |> Observable.timerSpan
    let marchingArmyAdded = new Subject<MarchingArmyAddedEvent>()
    let marchingArmyArrived = new Subject<MarchingArmyArrivedEvent>()
    let playerStatUpdated = new Subject<Map<PlayerId, PlayerStat>>()

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
          PlayerAdded = playerAdded.OnNext
          TileConquered = tileConquered.OnNext
          TilePopulationChanged = tilePopulationChanged.OnNext
          TileAdded = tileAdded.OnNext
          MarchingArmyAdded = marchingArmyAdded.OnNext
          MarchingArmyArrived = marchingArmyArrived.OnNext }

    /// 领域事件触发的 StateT Reader 游戏状态更新器
    let gameStateUpdater updater =
        let _, gameState' = updater |> StateT.run <| gameState |> Reader.run <| injector
        gameState <- gameState'

    /// 领域事件触发的 OptionT StateT Reader 游戏状态更新器
    let gameStateOptionUpdater
        (updater: OptionT<StateT<GameState, Reader<Injector<GameState>, 'a option * GameState>>>)
        =
        let opt, gameState' =
            updater |> OptionT.run |> StateT.run <| gameState |> Reader.run <| injector

        if opt.IsSome then
            gameState <- gameState'

    let tileAddedSub =
        tileAdded
        |> Observable.subscribe (fun e ->
            let (TileId tileId) = e.TileId
            aStar.AddPoint tileId e.Coord)

    let marchingArmyAddedSub =
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

    let marchingArmyArrivedSub =
        marchingArmyArrived
        |> Observable.subscribe (fun e ->
            AppService.armyArriveAndGenerateNew e.MarchingArmyId e.PlayerId
            |> gameStateOptionUpdater)

    let gameTickedSub =
        gameTicked
        |> Observable.subscribe (fun _ -> AppService.increaseAllPlayerTilesPopulation 1<Pop> |> gameStateUpdater)

    let gameFirstArmyGeneratedSub =
        gameFirstArmyGenerated
        |> Observable.subscribe (fun _ -> AppService.generateFirstGroupArmy |> gameStateUpdater)

    /// 领域事件触发的 State 游戏统计更新器
    let gameStatUpdater updater e =
        let _, gameStat' = updater e |> State.run <| gameStat
        gameStat <- gameStat'
        // TODO：暂时所有的游戏统计都会修改玩家统计（因为只有玩家统计……）
        playerStatUpdated.OnNext gameStat.PlayerStat

    let playerAddedUpdatePlayerStatSub =
        playerAdded
        |> Observable.subscribe (StatRepositoryF.updatePlayerStatByPlayerAddedEvent |> gameStatUpdater)

    let tileConqueredUpdatePlayerStatSub =
        tileConquered
        |> Observable.subscribe (StatRepositoryF.updatePlayerStatByTileConqueredEvent |> gameStatUpdater)

    let tilePopulationChangedUpdatePlayerStatSub =
        tilePopulationChanged
        |> Observable.subscribe (StatRepositoryF.updatePlayerStatByTilePopulationChangedEvent |> gameStatUpdater)

    let marchingArmyAddedUpdatePlayerStatSub =
        marchingArmyAdded
        |> Observable.subscribe (StatRepositoryF.updatePlayerStatByMarchingArmyAddedEvent |> gameStatUpdater)

    let marchingArmyArrivedUpdatePlayerStatSub =
        marchingArmyArrived
        |> Observable.subscribe (StatRepositoryF.updatePlayerStatByMarchingArmyArrivedEvent |> gameStatUpdater)

    member this.GameTicked = gameTicked |> Observable.asObservable

    member this.GameFirstArmyGenerated = gameFirstArmyGenerated |> Observable.asObservable

    member this.TileConquered = tileConquered |> Observable.asObservable

    member this.TilePopulationChanged = tilePopulationChanged |> Observable.asObservable

    member this.MarchingArmyAdded = marchingArmyAdded |> Observable.asObservable

    member this.MarchingArmyArrived = marchingArmyArrived |> Observable.asObservable

    member this.PlayerStatUpdated = playerStatUpdated |> Observable.asObservable

    member this.QueryTileById tileId =
        AppService.queryTileById gameState tileId

    member this.MarchingSpeed population = DomainF.marchSpeed population

    member this.Init() =
        logPrinter $"随机种子：{seed}"
        AppService.init playerCount |> gameStateUpdater
