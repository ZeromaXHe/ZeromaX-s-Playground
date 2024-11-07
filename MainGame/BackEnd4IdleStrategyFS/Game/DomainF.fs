namespace BackEnd4IdleStrategyFS.Game

open System
open BackEnd4IdleStrategyFS.Common
open FSharpPlus
open FSharpPlus.Data
open BackEnd4IdleStrategyFS.Game.DomainT
open BackEnd4IdleStrategyFS.Game.EventT
open BackEnd4IdleStrategyFS.Godot.IAdapter

/// 领域层逻辑
module private DomainF =

    /// 创建单个地块
    let createTile (tileFactory: TileFactory<'s>) (tileAdded: TileAdded<'s>) env coord =
        let env', tile = tileFactory coord 0<Pop> None env
        tileAdded { TileId = tile.Id; Coord = tile.Coord } env'

    /// 创建多个地块
    let createTiles (tileFactory: TileFactory<'s>) (tileAdded: TileAdded<'s>) coords (env: 's) =
        coords |> Seq.fold (createTile tileFactory tileAdded) env

    /// 地块 AStar2D 连接初始化
    let initTileConnections
        (tileQueryByCoord: TileQueryByCoord<'s>)
        (terrainLayer: ITileMapLayer)
        (aStar2D: IAStar2D)
        (env: 's)
        cell
        =

        terrainLayer.GetSurroundingCells cell
        |> Seq.fold
            (fun s surrounding ->
                let surTileOpt = tileQueryByCoord surrounding s
                let cellTileOpt = tileQueryByCoord cell s

                match surTileOpt, cellTileOpt with
                | Some surTile, Some cellTile ->
                    let (TileId cellTileId) = cellTile.Id
                    let (TileId surTileId) = surTile.Id
                    aStar2D.ConnectPoints cellTileId surTileId
                    s
                | _, _ -> s)
            env

    /// 所有地块 AStar2D 连接初始化
    let initTilesConnections
        (tileQueryByCoord: TileQueryByCoord<'s>)
        (terrainLayer: ITileMapLayer)
        (aStar2D: IAStar2D)
        (env: 's)
        =

        terrainLayer.GetUsedCells()
        |> Seq.fold (initTileConnections tileQueryByCoord terrainLayer aStar2D) env

    /// 玩家占领地块
    let conquerTile
        (tileUpdater: TileUpdater<'s>)
        (tileConquered: TileConquered<'s>)
        (tile: Tile)
        (conqueror: Player)
        (env: 's)
        =

        let env' =
            tileUpdater
                { tile with
                    PlayerId = Some conqueror.Id }
                env

        tileConquered
            { TileId = tile.Id
              Coord = tile.Coord
              Population = tile.Population
              ConquerorId = conqueror.Id
              LoserId = tile.PlayerId }
            env'

    /// 玩家初始化出生地块
    let rec spawnPlayer
        (tileUpdater: TileUpdater<'s>)
        (playerFactory: PlayerFactory<'s>)
        (tileConquered: TileConquered<'s>)
        (random: Random)
        (tiles: Tile list)
        (env: 's)
        =

        let env', player = playerFactory env
        let tile = tiles[random.Next tiles.Length]

        match tile.PlayerId with
        | None -> conquerTile tileUpdater tileConquered tile player env'
        | Some _ -> spawnPlayer tileUpdater playerFactory tileConquered random tiles env'

    /// 初始化 n 个玩家出生地块
    let spawnPlayers
        (tilesQueryAll: TilesQueryAll<'s>)
        (tileUpdater: TileUpdater<'s>)
        (playerFactory: PlayerFactory<'s>)
        (tileConquered: TileConquered<'s>)
        (random: Random)
        playerCount
        (env: 's)
        =

        let tiles = tilesQueryAll env |> List.ofSeq

        [ 1..playerCount ]
        |> List.fold (fun s _ -> spawnPlayer tileUpdater playerFactory tileConquered random tiles s) env


    /// 随机一块玩家领土
    let randomPlayerTile (tilesQueryByPlayer: TilesQueryByPlayer<'s>) (random: Random) (player: Player) (env: 's) =

        let tiles = tilesQueryByPlayer player.Id env |> List.ofSeq
        tiles[random.Next tiles.Length]

    /// 随机一部分地块人口
    let randomPopulationFromTile (random: Random) (tile: Tile) =

        random.Next(1, tile.Population / 1<Pop>) * 1<Pop>

    /// 随机连接地块
    let randomConnectedTileId (aStar2D: IAStar2D) (random: Random) (tile: Tile) =

        let (TileId tileId) = tile.Id
        let connectedTileIds = aStar2D.GetPointConnections tileId |> Seq.toList
        TileId connectedTileIds[random.Next connectedTileIds.Length]

    /// 行军速度（单位：%/s）
    let marchSpeed =
        function
        | p when p < 10<Pop> -> 50 // 人数小于 10 人，2 秒后到达目的地
        | p when p < 50<Pop> -> 25 // 小于 50 人，4 秒后
        | p when p < 200<Pop> -> 15 // 小于 200 人，7 秒左右后
        | p when p < 1000<Pop> -> 10 // 小于 1000 人，10 秒后
        | _ -> 5 // 大于 1000 人，20 秒后


    /// 玩家随机行军目标
    let marchArmy
        (marchingArmyFactory: MarchingArmyFactory<'s>)
        (marchingArmyAdded: MarchingArmyAdded<'s>)
        (tilesQueryByPlayer: TilesQueryByPlayer<'s>)
        (tileUpdater: TileUpdater<'s>)
        (aStar2D: IAStar2D)
        (random: Random)
        (player: Player)
        (env: 's)
        =

        let fromTile = randomPlayerTile tilesQueryByPlayer random player env
        let population = randomPopulationFromTile random fromTile
        let toTileId = randomConnectedTileId aStar2D random fromTile
        let env', army = marchingArmyFactory population player.Id fromTile.Id toTileId env

        let env'' =
            tileUpdater
                { fromTile with
                    Population = fromTile.Population - army.Population }
                env'

        let env''' =
            marchingArmyAdded
                { MarchingArmyId = army.Id
                  Population = army.Population
                  FromTileId = fromTile.Id
                  ToTileId = toTileId
                  PlayerId = player.Id }
                env''

        env''', army

    /// 行军到达目的地
    let arriveArmy
        (tileQueryById: TileQueryById<'s>)
        (playerQueryById: PlayerQueryById<'s>)
        (marchingArmyDeleter: MarchingArmyDeleter<'s>)
        (tileUpdater: TileUpdater<'s>)
        (tileConquered: TileConquered<'s>)
        (marchingArmy: MarchingArmy)
        (env: 's)
        =

        let resultOpt =
            monad {
                let! tile = tileQueryById marchingArmy.ToTileId env
                let! player = playerQueryById marchingArmy.PlayerId env

                let env' =
                    match tile.PlayerId with
                    | None ->
                        let tile' =
                            { tile with
                                Population = tile.Population + marchingArmy.Population }

                        conquerTile tileUpdater tileConquered tile' player env
                    | Some playerId when playerId = marchingArmy.PlayerId ->
                        let tile' =
                            { tile with
                                Population = tile.Population + marchingArmy.Population }

                        tileUpdater tile' env
                    | Some _ when tile.Population > marchingArmy.Population ->
                        let tile' =
                            { tile with
                                Population = tile.Population - marchingArmy.Population }

                        tileUpdater tile' env
                    | Some _ ->
                        let tile' =
                            { tile with
                                Population = marchingArmy.Population - tile.Population }

                        conquerTile tileUpdater tileConquered tile' player env

                let env'' = marchingArmyDeleter marchingArmy.Id env'
                return env''
            }

        match resultOpt with
        | None -> failwith $"Invalid marching army id, id:{marchingArmy.Id}"
        | Some e -> e

    /// 增加地块人口
    let increaseTilePopulation
        (tileUpdater: TileUpdater<'s>)
        (tilePopulationChanged: TilePopulationChanged<'s>)
        increment
        (env: 's)
        (tile: Tile)
        =

        let env' =
            tileUpdater
                { tile with
                    Population = tile.Population + increment }
                env

        tilePopulationChanged
            { TileId = tile.Id
              BeforePopulation = tile.Population
              AfterPopulation = tile.Population + increment }
            env'

    // 增加所有玩家地块人口
    let increaseAllPlayerTilesPopulation
        (tilesQueryAll: TilesQueryAll<'s>)
        (tileUpdater: TileUpdater<'s>)
        (tilePopulationChanged: TilePopulationChanged<'s>)
        increment
        (env: 's)
        =

        tilesQueryAll env
        |> Seq.filter (fun tile -> tile.PlayerId.IsSome && tile.Population < 1000<Pop>)
        |> Seq.fold (increaseTilePopulation tileUpdater tilePopulationChanged increment) env


module private Dependency =
    type Injector<'s> =
        {
          // Godot
          AStar: IAStar2D
          TerrainLayer: ITileMapLayer
          // 随机
          Random: Random
          // 仓储
          PlayerFactory: PlayerFactoryM<'s>
          PlayerQueryById: PlayerQueryByIdM<'s>
          TileFactory: TileFactoryM<'s>
          TileUpdater: TileUpdaterM<'s>
          TileQueryById: TileQueryByIdM<'s>
          TileQueryByCoord: TileQueryByCoordM<'s>
          TilesQueryByPlayer: TilesQueryByPlayerM<'s>
          TilesQueryAll: TilesQueryAllM<'s>
          MarchingArmyFactory: MarchingArmyFactoryM<'s>
          MarchingArmyDeleter: MarchingArmyDeleterM<'s>
          // 事件
          TileConquered: TileConqueredM
          TilePopulationChanged: TilePopulationChangedM
          TileAdded: TileAddedM
          MarchingArmyAdded: MarchingArmyAddedM
          MarchingArmyArrived: MarchingArmyArrivedM }

/// 领域层逻辑 monad
module private DomainFM =
    open MonadHelper
    open Dependency

    /// 创建单个地块（双层 Monad 实现）
    let createTileMM coord =
        monad {
            let! di = Reader.ask

            monad {
                let! (tile: Tile) = di.TileFactory coord 0<Pop> None
                di.TileAdded { TileId = tile.Id; Coord = tile.Coord }
                return tile
            }
        }

    /// 创建单个地块（Monad Transformer 实现）
    let createTile coord =
        monad {
            let! di = Reader.ask |> StateT.lift
            let! (tile: Tile) = di.TileFactory coord 0<Pop> None |> StateT.hoist
            di.TileAdded { TileId = tile.Id; Coord = tile.Coord }
            tile
        }

    /// 创建多个地块（双层 Monad 实现）
    let createTilesMM coords =
        // 必须断一下，不然类型推断推不出来 Tile 类型
        let r = coords |> Seq.traverse createTileMM
        r |> Reader.map Seq.sequence

    /// 创建多个地块（Monad Transformer 实现）
    let createTiles coords = coords |> Seq.traverse createTile

    /// 地块 AStar2D 连接初始化（双层 Monad 实现）
    let initTileConnectionsMM cell =
        monad {
            let! di = Reader.ask

            di.TerrainLayer.GetSurroundingCells cell
            |> Seq.fold
                (fun _ surrounding ->
                    monad {
                        let! s = State.get
                        let surTileOpt, _ = di.TileQueryByCoord surrounding |> State.run <| s
                        let cellTileOpt, _ = di.TileQueryByCoord cell |> State.run <| s

                        match surTileOpt, cellTileOpt with
                        | Some surTile, Some cellTile ->
                            let (TileId cellTileId) = cellTile.Id
                            let (TileId surTileId) = surTile.Id
                            di.AStar.ConnectPoints cellTileId surTileId
                        | _, _ -> ()
                    })
                stateReturnUnit
        }

    /// 地块 AStar2D 连接初始化（Monad Transformer 实现）
    let initTileConnections cell =
        monad {
            let! di = Reader.ask |> StateT.lift
            let! s = State.get |> StateT.hoist

            di.TerrainLayer.GetSurroundingCells cell
            |> Seq.fold
                (fun _ surrounding ->
                    let surTileOpt, _ = di.TileQueryByCoord surrounding |> State.run <| s
                    let cellTileOpt, _ = di.TileQueryByCoord cell |> State.run <| s

                    match surTileOpt, cellTileOpt with
                    | Some surTile, Some cellTile ->
                        let (TileId cellTileId) = cellTile.Id
                        let (TileId surTileId) = surTile.Id
                        di.AStar.ConnectPoints cellTileId surTileId
                    | _, _ -> ())
                ()
        }

    /// 所有地块 AStar2D 连接初始化（双层 Monad 实现）
    let initTilesConnectionsMM<'s> =
        monad {
            let! (di: Injector<'s>) = Reader.ask
            return!
                di.TerrainLayer.GetUsedCells()
                |> Seq.map initTileConnectionsMM
                |> Seq.fold readerFolderM (readerReturn Seq.empty)
        }

    /// 所有地块 AStar2D 连接初始化（Monad Transformer 实现）
    let initTilesConnections<'s> =
        monad {
            let! (di: Injector<'s>) = Reader.ask |> StateT.lift

            return!
                di.TerrainLayer.GetUsedCells()
                |> Seq.map initTileConnections
                |> Seq.fold stateTReaderFolderM (stateTReaderReturn Seq.empty)
        }

    /// 玩家占领地块（双层 Monad 实现）
    let conquerTileMM (tile: Tile) (conqueror: Player) =
        monad {
            let! di = Reader.ask

            monad {
                let! (tile': Tile) =
                    di.TileUpdater
                        { tile with
                            PlayerId = Some conqueror.Id }

                di.TileConquered
                    { TileId = tile.Id
                      Coord = tile.Coord
                      Population = tile.Population
                      ConquerorId = conqueror.Id
                      LoserId = tile.PlayerId }

                tile'
            }
        }

    /// 玩家占领地块（Monad Transformer 实现）
    let conquerTile (tile: Tile) (conqueror: Player) =
        monad {
            let! di = Reader.ask |> StateT.lift

            let! (tile': Tile) =
                di.TileUpdater
                    { tile with
                        PlayerId = Some conqueror.Id }
                |> StateT.hoist

            di.TileConquered
                { TileId = tile.Id
                  Coord = tile.Coord
                  Population = tile.Population
                  ConquerorId = conqueror.Id
                  LoserId = tile.PlayerId }

            tile'
        }

    /// 玩家初始化出生地块（双层 Monad 实现）
    let rec spawnPlayerMM (tiles: Tile list) =
        monad {
            let! di = Reader.ask

            monad {
                let! player = di.PlayerFactory
                let tile = tiles[di.Random.Next tiles.Length]

                return!
                    match tile.PlayerId with
                    | None -> conquerTileMM tile player |> Reader.run <| di
                    | Some _ -> spawnPlayerMM tiles |> Reader.run <| di
            }
        }

    /// 玩家初始化出生地块（Monad Transformer 实现）
    let rec spawnPlayer (tiles: Tile list) =
        monad {
            let! di = Reader.ask |> StateT.lift
            let! player = di.PlayerFactory |> StateT.hoist
            let tile = tiles[di.Random.Next tiles.Length]
            // BUG: 当前实现在 tiles 接近全部有玩家时容易导致死循环
            return!
                match tile.PlayerId with
                | None -> conquerTile tile player
                | Some _ -> spawnPlayer tiles
        }

    /// 初始化 n 个玩家出生地块（双层 Monad 实现）
    let spawnPlayersMM playerCount =
        monad {
            let! di = Reader.ask

            monad {
                let! tileSeq = di.TilesQueryAll
                let tiles = tileSeq |> List.ofSeq

                return!
                    [ 1..playerCount ]
                    |> Seq.map (fun _ -> spawnPlayerMM tiles |> Reader.run <| di)
                    |> Seq.sequence
            }
        }

    /// 初始化 n 个玩家出生地块（Monad Transformer 实现）
    let spawnPlayers playerCount =
        monad {
            let! di = Reader.ask |> StateT.lift
            let! tileSeq = di.TilesQueryAll |> StateT.hoist
            let tiles = tileSeq |> List.ofSeq

            return!
                [ 1..playerCount ]
                |> Seq.map (fun _ -> spawnPlayer tiles)
                |> Seq.fold stateTReaderFolderM (stateTReaderReturn Seq.empty)
        }

    /// 随机一块玩家领土（双层 Monad 实现）
    let randomPlayerTileMM (player: Player) =
        monad {
            let! di = Reader.ask

            monad {
                let! (tileSeq: Tile seq) = di.TilesQueryByPlayer player.Id
                let tiles = tileSeq |> List.ofSeq
                tiles[di.Random.Next tiles.Length]
            }
        }

    /// 随机一块玩家领土（Monad Transformer 实现）
    let randomPlayerTile (player: Player) =
        monad {
            let! di = Reader.ask |> StateT.lift
            let! (tileSeq: Tile seq) = di.TilesQueryByPlayer player.Id |> StateT.hoist
            let tiles = tileSeq |> List.ofSeq
            tiles[di.Random.Next tiles.Length]
        }

    /// 随机一部分地块人口
    let randomPopulationFromTile (tile: Tile) =
        monad {
            let! di = Reader.ask
            di.Random.Next(1, tile.Population / 1<Pop>) * 1<Pop>
        }

    /// 随机一个相邻地块 id
    let randomConnectedTileId (tile: Tile) =
        monad {
            let! di = Reader.ask
            let (TileId tileId) = tile.Id
            let connectedTileIds = di.AStar.GetPointConnections tileId |> Seq.toList
            TileId connectedTileIds[di.Random.Next connectedTileIds.Length]
        }

    /// 玩家随机行军目标（双层 Monad 实现）
    let marchArmyMM (player: Player) =
        monad {
            let! di = Reader.ask

            monad {
                let! fromTile = randomPlayerTileMM player |> Reader.run <| di
                let population = randomPopulationFromTile fromTile |> Reader.run <| di
                let toTileId = randomConnectedTileId fromTile |> Reader.run <| di
                let! army = di.MarchingArmyFactory population player.Id fromTile.Id toTileId

                let! _ =
                    di.TileUpdater
                        { fromTile with
                            Population = fromTile.Population - army.Population }

                di.MarchingArmyAdded
                    { MarchingArmyId = army.Id
                      Population = army.Population
                      FromTileId = fromTile.Id
                      ToTileId = toTileId
                      PlayerId = player.Id }

                army
            }
        }

    /// 玩家随机行军目标（Monad Transformer 实现）
    let marchArmy (player: Player) =
        monad {
            let! di = Reader.ask |> StateT.lift
            let! fromTile = randomPlayerTile player
            let! population = randomPopulationFromTile fromTile |> StateT.lift
            let! toTileId = randomConnectedTileId fromTile |> StateT.lift
            let! (army: MarchingArmy) = di.MarchingArmyFactory population player.Id fromTile.Id toTileId |> StateT.hoist

            let! _ =
                di.TileUpdater
                    { fromTile with
                        Population = fromTile.Population - army.Population }
                |> StateT.hoist

            di.MarchingArmyAdded
                { MarchingArmyId = army.Id
                  Population = army.Population
                  FromTileId = fromTile.Id
                  ToTileId = toTileId
                  PlayerId = player.Id }

            army
        }

    /// 行军速度（单位：%/s）
    let marchSpeed =
        function
        | p when p < 10<Pop> -> 50 // 人数小于 10 人，2 秒后到达目的地
        | p when p < 50<Pop> -> 25 // 小于 50 人，4 秒后
        | p when p < 200<Pop> -> 15 // 小于 200 人，7 秒左右后
        | p when p < 1000<Pop> -> 10 // 小于 1000 人，10 秒后
        | _ -> 5 // 大于 1000 人，20 秒后

    /// 行军到达目的地（双层 Monad 实现）
    let arriveArmyMM (marchingArmy: MarchingArmy) =
        monad {
            let! di = Reader.ask

            monad {
                let! tile = di.TileQueryById marchingArmy.ToTileId |> OptionT
                let! player = di.PlayerQueryById marchingArmy.PlayerId |> OptionT

                let! _ =
                    match tile.PlayerId with
                    | None ->
                        let tile' =
                            { tile with
                                Population = tile.Population + marchingArmy.Population }

                        conquerTileMM tile' player |> Reader.run <| di |> OptionT.lift
                    | Some playerId when playerId = marchingArmy.PlayerId ->
                        let tile' =
                            { tile with
                                Population = tile.Population + marchingArmy.Population }

                        di.TileUpdater tile' |> OptionT.lift
                    | Some _ when tile.Population > marchingArmy.Population ->
                        let tile' =
                            { tile with
                                Population = tile.Population - marchingArmy.Population }

                        di.TileUpdater tile' |> OptionT.lift
                    | Some _ ->
                        let tile' =
                            { tile with
                                Population = marchingArmy.Population - tile.Population }

                        conquerTileMM tile' player |> Reader.run <| di |> OptionT.lift

                let! resultBool = di.MarchingArmyDeleter marchingArmy.Id |> OptionT.lift
                return resultBool
            }
            |> OptionT.run
        }

    /// 行军到达目的地（双层 Monad 过渡到 Monad Transformer 实现）
    let arriveArmy' (marchingArmy: MarchingArmy) =
        monad {
            let! di = Reader.ask

            monad {
                let! (tile: Tile) = di.TileQueryById marchingArmy.ToTileId |> OptionT
                let! (player: Player) = di.PlayerQueryById marchingArmy.PlayerId |> OptionT
                let! s = State.get |> OptionT.lift

                let! _ =
                    match tile.PlayerId with
                    | None ->
                        let tile' =
                            { tile with
                                Population = tile.Population + marchingArmy.Population }

                        monad {
                            let r = conquerTile tile' player |> StateT.run <| s
                            let t, s' = Reader.run r di
                            do! State.put s'
                            t
                        }
                        |> OptionT.lift
                    | Some playerId when playerId = marchingArmy.PlayerId ->
                        let tile' =
                            { tile with
                                Population = tile.Population + marchingArmy.Population }

                        di.TileUpdater tile' |> OptionT.lift
                    | Some _ when tile.Population > marchingArmy.Population ->
                        let tile' =
                            { tile with
                                Population = tile.Population - marchingArmy.Population }

                        di.TileUpdater tile' |> OptionT.lift
                    | Some _ ->
                        let tile' =
                            { tile with
                                Population = marchingArmy.Population - tile.Population }

                        monad {
                            let r = conquerTile tile' player |> StateT.run <| s
                            let t, s' = Reader.run r di
                            do! State.put s'
                            t
                        }
                        |> OptionT.lift

                let! resultBool = di.MarchingArmyDeleter marchingArmy.Id |> OptionT.lift
                return resultBool
            }
            |> OptionT.run
        }

    /// 行军到达目的地（Monad Transformer 实现）
    let arriveArmy (marchingArmy: MarchingArmy) =
        monad {
            let! di = Reader.ask |> StateT.lift
            let! (tileOpt: Tile option) = di.TileQueryById marchingArmy.ToTileId |> StateT.hoist
            let! (playerOpt: Player option) = di.PlayerQueryById marchingArmy.PlayerId |> StateT.hoist
            // BUG: 临时实现，实在不知道怎么解这种三层 Monad 了……
            let tile = tileOpt.Value
            let player = playerOpt.Value
            let! s = State.get |> StateT.hoist

            let! _ =
                match tile.PlayerId with
                | None ->
                    let tile' =
                        { tile with
                            Population = tile.Population + marchingArmy.Population }

                    monad {
                        let r = conquerTile tile' player |> StateT.run <| s
                        let t, s' = Reader.run r di
                        do! State.put s'
                        t
                    }
                    |> StateT.hoist
                | Some playerId when playerId = marchingArmy.PlayerId ->
                    let tile' =
                        { tile with
                            Population = tile.Population + marchingArmy.Population }

                    di.TileUpdater tile' |> StateT.hoist
                | Some _ when tile.Population > marchingArmy.Population ->
                    let tile' =
                        { tile with
                            Population = tile.Population - marchingArmy.Population }

                    di.TileUpdater tile' |> StateT.hoist
                | Some _ ->
                    let tile' =
                        { tile with
                            Population = marchingArmy.Population - tile.Population }

                    monad {
                        let r = conquerTile tile' player |> StateT.run <| s
                        let t, s' = Reader.run r di
                        do! State.put s'
                        t
                    }
                    |> StateT.hoist

            let! resultBool = di.MarchingArmyDeleter marchingArmy.Id |> StateT.hoist
            return resultBool
        }

    /// 增加地块人口（双层 Monad 实现）
    let increaseTilePopulationMM increment (tile: Tile) =
        monad {
            let! di = Reader.ask

            monad {
                let! (tile': Tile) =
                    di.TileUpdater
                        { tile with
                            Population = tile.Population + increment }

                di.TilePopulationChanged
                    { TileId = tile.Id
                      BeforePopulation = tile.Population
                      AfterPopulation = tile.Population + increment }

                tile'
            }
        }

    /// 增加地块人口（Monad Transformer 实现）
    let increaseTilePopulation increment (tile: Tile) =
        monad {
            let! di = Reader.ask |> StateT.lift

            let! (tile': Tile) =
                di.TileUpdater
                    { tile with
                        Population = tile.Population + increment }
                |> StateT.hoist

            di.TilePopulationChanged
                { TileId = tile.Id
                  BeforePopulation = tile.Population
                  AfterPopulation = tile.Population + increment }

            tile'
        }

    // 增加所有玩家地块人口（双层 Monad 实现）
    let increaseAllPlayerTilesPopulationMM increment =
        monad {
            let! di = Reader.ask

            monad {
                let! (tiles: Tile seq) = di.TilesQueryAll

                return!
                    tiles
                    |> Seq.filter (fun tile -> tile.PlayerId.IsSome && tile.Population < 1000<Pop>)
                    |> Seq.map (fun t -> increaseTilePopulationMM increment t |> Reader.run <| di)
                    |> Seq.sequence
            }
        }

    // 增加所有玩家地块人口（Monad Transformer 实现）
    let increaseAllPlayerTilesPopulation increment =
        monad {
            let! di = Reader.ask |> StateT.lift
            let! (tiles: Tile seq) = di.TilesQueryAll |> StateT.hoist

            return!
                tiles
                |> Seq.filter (fun tile -> tile.PlayerId.IsSome && tile.Population < 1000<Pop>)
                |> Seq.map (increaseTilePopulation increment)
                // |> Seq.sequence //（类型推断不出来，必须使用自己写的辅助类）
                // |> Seq.fold stateTReaderFolderM (StateT.Return Seq.empty)
                |> Seq.fold stateTReaderFolderM (stateTReaderReturn Seq.empty)
        }
