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

type Entry(aStar: IAStar2D, terrainLayer: ITileMapLayer, playerCount: int, logPrinter: string -> unit) =
    /// 游戏状态
    let mutable gameState = emptyGameState
    /// 游戏统计
    let mutable gameStat = emptyGameStat

    // TODO: 好像确定开局种子也不能保证后续现象一致。是 Random.Next 触发的顺序不一致导致的吗？
    let seed = 1662646297 // Random().Next Int32.MaxValue
    let random = Random(seed)

    let gameProcess = new Subject<float>()
    let playerAdded = new Subject<PlayerAddedEvent>()
    let tileAdded = new Subject<TileAddedEvent>()
    let tileConquered = new Subject<TileConqueredEvent>()
    let tilePopulationChanged = new Subject<TilePopulationChangedEvent>()
    let gameTicked = new Subject<int64>()
    let gameFirstArmyGenerated = new Subject<int64>()
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
          SpeedMultiplierQuery = QueryRepositoryF.getSpeedMultiplier
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
          MarchingArmyUpdater = CommandRepositoryF.updateMarchingArmy
          MarchingArmyDeleter = CommandRepositoryF.deleteMarchingArmy
          MarchingArmyQueryById = QueryRepositoryF.getMarchingArmy
          MarchingArmiesQueryAll = QueryRepositoryF.getAllMarchingArmies
          // 事件
          PlayerAdded = playerAdded.OnNext
          TileConquered = tileConquered.OnNext
          TilePopulationChanged = tilePopulationChanged.OnNext
          TileAdded = tileAdded.OnNext
          MarchingArmyAdded = marchingArmyAdded.OnNext
          MarchingArmyArrived = marchingArmyArrived.OnNext }

    /// 领域事件触发的 StateT Reader 游戏状态更新器
    let gameStateUpdater resultHandler updater =
        let res, gameState' = updater |> StateT.run <| gameState |> Reader.run <| injector
        gameState <- gameState'
        resultHandler res

    /// 领域事件触发的 OptionT StateT Reader 游戏状态更新器
    let gameStateOptionUpdater
        resultHandler
        (updater: OptionT<StateT<GameState, Reader<Injector<GameState>, 'a option * GameState>>>)
        =
        let opt, gameState' =
            updater |> OptionT.run |> StateT.run <| gameState |> Reader.run <| injector

        if opt.IsSome then
            gameState <- gameState'
            resultHandler opt.Value

    let gameProcessToTickSub =
        gameProcess
        |> Observable.scanInit (0.0, false) (fun (acc, _) delta ->
            let newAcc = acc + delta * gameState.SpeedMultiplier
            // 标准速度下，每满 0.5s 发送一个 Tick 事件
            if newAcc >= 0.5 then
                (newAcc - 0.5, true)
            else
                (newAcc, false))
        |> Observable.filter snd
        |> Observable.subscribe (fun _ -> gameTicked.OnNext DateTime.UtcNow.Ticks)

    let gameProcessToFirstArmyGeneratedSub =
        gameProcess
        |> Observable.scan (fun acc delta -> acc + delta * gameState.SpeedMultiplier)
        |> Observable.filter (fun t -> t > 3) // 标准速度下 3 秒后触发
        |> Observable.take 1
        |> Observable.subscribe (fun _ -> gameFirstArmyGenerated.OnNext DateTime.UtcNow.Ticks)

    // TODO: 仅仅满足现在游戏速度的功能需要，实现的非常丑……（后续考虑这种 Godot 调进来的是不是都得直接用成员方法，而不是响应式编程？）
    let gameProcessMarchArmiesSub =
        gameProcess
        // 确保游戏速度不为 0 且有行军部队存在
        |> Observable.filter (fun _ -> gameState.SpeedMultiplier <> 0 && gameState.MarchingArmyRepo.Count <> 0)
        |> Observable.subscribe (fun delta ->
            AppService.marchArmies delta
            |> gameStateUpdater (fun armies ->
                armies
                |> Seq.filter (fun army -> army.Progress >= 100.0)
                |> Seq.iter (fun army ->
                    AppService.armyArriveAndGenerateNew army.Id army.PlayerId
                    |> gameStateOptionUpdater ignore)))

    let tileAddedSub =
        tileAdded
        |> Observable.subscribe (fun e ->
            let (TileId tileId) = e.TileId
            aStar.AddPoint tileId e.Coord)

    let gameTickedSub =
        gameTicked
        |> Observable.subscribe (fun _ -> AppService.increaseAllPlayerTilesPopulation 1<Pop> |> gameStateUpdater ignore)

    let gameFirstArmyGeneratedSub =
        gameFirstArmyGenerated
        |> Observable.subscribe (fun _ -> AppService.generateFirstGroupArmy |> gameStateUpdater ignore)

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

    member this.GameProcess = gameProcess
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

    member this.ChangeGameSpeed speed =
        if gameState.SpeedMultiplier <> speed then
            logPrinter $"游戏速度修改为：{speed}"

            gameState <-
                { gameState with
                    SpeedMultiplier = speed }

    member this.GetSpeedMultiplier() = gameState.SpeedMultiplier

    member this.Init() =
        logPrinter $"随机种子：{seed}"
        AppService.init playerCount |> gameStateUpdater ignore
