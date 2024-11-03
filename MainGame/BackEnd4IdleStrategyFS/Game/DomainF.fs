namespace BackEnd4IdleStrategyFS.Game

open System
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
        (terrainLayer: ITileMapLayer<'s>)
        (aStar2D: IAStar2D<'s>)
        (env: 's)
        cell
        =

        terrainLayer.GetSurroundingCells cell env
        |> Seq.fold
            (fun s surrounding ->
                let surTileOpt = tileQueryByCoord surrounding s
                let cellTileOpt = tileQueryByCoord cell s

                match surTileOpt, cellTileOpt with
                | Some surTile, Some cellTile ->
                    let (TileId cellTileId) = cellTile.Id
                    let (TileId surTileId) = surTile.Id
                    aStar2D.ConnectPoints cellTileId surTileId env
                | _, _ -> s)
            env

    /// 所有地块 AStar2D 连接初始化
    let initTilesConnections
        (tileQueryByCoord: TileQueryByCoord<'s>)
        (terrainLayer: ITileMapLayer<'s>)
        (aStar2D: IAStar2D<'s>)
        (env: 's)
        =

        terrainLayer.GetUsedCells env
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
    let randomConnectedTileId (aStar2D: IAStar2D<'s>) (random: Random) (tile: Tile) (env: 's) =

        let (TileId tileId) = tile.Id
        let connectedTileIds = aStar2D.GetPointConnections tileId env |> Seq.toList
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
        (aStar2D: IAStar2D<'s>)
        (random: Random)
        (player: Player)
        (env: 's)
        =

        let fromTile = randomPlayerTile tilesQueryByPlayer random player env
        let population = randomPopulationFromTile random fromTile
        let toTileId = randomConnectedTileId aStar2D random fromTile env
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
