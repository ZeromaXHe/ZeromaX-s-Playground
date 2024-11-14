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
    let gameStateSubject = Subject.behavior emptyGameState
    /// 游戏统计
    let gameStatSubject = Subject.behavior emptyGameStat

    // TODO: 好像确定开局种子也不能保证后续现象一致。是 Random.Next 触发的顺序不一致导致的吗？
    let seed = 1662646297 // Random().Next Int32.MaxValue
    let random = Random(seed)

    let gameProcess = new Subject<float>()
    let playerAdded = new Subject<PlayerAddedEvent>()
    let tileAdded = new Subject<TileAddedEvent>()
    let tileConquered = new Subject<TileConqueredEvent>()
    let tilePopulationChanged = new Subject<TilePopulationChangedEvent>()
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
        let res, gameState' =
            updater |> StateT.run <| gameStateSubject.Value |> Reader.run <| injector

        gameStateSubject.OnNext gameState'
        resultHandler res

    /// 领域事件触发的 OptionT StateT Reader 游戏状态更新器
    let gameStateOptionUpdater
        resultHandler
        (updater: OptionT<StateT<GameState, Reader<Injector<GameState>, 'a option * GameState>>>)
        =
        let opt, gameState' =
            updater |> OptionT.run |> StateT.run <| gameStateSubject.Value |> Reader.run
            <| injector

        if opt.IsSome then
            gameStateSubject.OnNext gameState'
            resultHandler opt.Value

    let marchArmiesSub =
        gameProcess
        // 确保游戏速度不为 0 且有行军部队存在
        |> Observable.filter (fun _ ->
            gameStateSubject.Value.SpeedMultiplier <> 0
            && gameStateSubject.Value.MarchingArmyRepo.Count <> 0)
        |> Observable.subscribe (fun delta -> AppService.marchArmies delta |> gameStateOptionUpdater ignore)

    let tileAddedSub =
        tileAdded
        |> Observable.subscribe (fun e ->
            let (TileId tileId) = e.TileId
            aStar.AddPoint tileId e.Coord)

    /// 游戏 tick：增加所有玩家地块人口
    let gameTickedSub =
        gameProcess
        |> Observable.scanInit (0.0, false) (fun (acc, _) delta ->
            let newAcc = acc + delta * gameStateSubject.Value.SpeedMultiplier
            // 标准速度下，每满 0.5s 发送一个 Tick 事件
            if newAcc >= 0.5 then
                (newAcc - 0.5, true)
            else
                (newAcc, false))
        |> Observable.filter snd
        |> Observable.map (fun _ -> DateTime.UtcNow.Ticks)
        |> Observable.subscribe (fun t ->
            AppService.increaseAllPlayerTilesPopulation 1<Pop> |> gameStateUpdater ignore
            logPrinter $"tick ${t}")

    /// 第一次出兵
    let gameFirstArmyGeneratedSub =
        gameProcess
        |> Observable.scan (fun acc delta -> acc + delta * gameStateSubject.Value.SpeedMultiplier)
        |> Observable.filter (fun t -> t > 3) // 标准速度下 3 秒后触发
        |> Observable.take 1
        |> Observable.subscribe (fun _ ->
            AppService.generateFirstGroupArmy |> gameStateUpdater ignore
            logPrinter "第一次出兵！！！")

    /// 领域事件触发的 State 游戏统计更新器
    let gameStatUpdater updater e =
        let _, gameStat' = updater e |> State.run <| gameStatSubject.Value
        gameStatSubject.OnNext gameStat'
        // TODO：暂时所有的游戏统计都会修改玩家统计（因为只有玩家统计……）
        playerStatUpdated.OnNext gameStatSubject.Value.PlayerStat

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
    member this.TileConquered = tileConquered |> Observable.asObservable
    member this.TilePopulationChanged = tilePopulationChanged |> Observable.asObservable
    member this.MarchingArmyAdded = marchingArmyAdded |> Observable.asObservable
    member this.MarchingArmyArrived = marchingArmyArrived |> Observable.asObservable
    member this.PlayerStatUpdated = playerStatUpdated |> Observable.asObservable

    member this.QueryTileById tileId =
        AppService.queryTileById gameStateSubject.Value tileId

    member this.MarchingSpeed population = DomainF.marchSpeed population

    member this.ChangeGameSpeed speed =
        if gameStateSubject.Value.SpeedMultiplier <> speed then
            logPrinter $"游戏速度修改为：{speed}"

            gameStateSubject.OnNext
                { gameStateSubject.Value with
                    SpeedMultiplier = speed }

    member this.GetSpeedMultiplier() = gameStateSubject.Value.SpeedMultiplier

    member this.Init() =
        logPrinter $"随机种子：{seed}"
        AppService.init playerCount |> gameStateUpdater ignore
