namespace BackEnd4IdleStrategyFS.Game

open FSharpPlus
open FSharpPlus.Data
open DomainT
open Dependency

/// 领域层逻辑
module private DomainF =

    /// 创建单个地块
    let createTile coord =
        monad {
            let! di = Reader.ask |> StateT.lift
            let! (tile: Tile) = di.TileFactory coord 0<Pop> None |> StateT.hoist
            di.TileAdded { TileId = tile.Id; Coord = tile.Coord }
            tile
        }

    /// 创建多个地块
    let createTiles coords = coords |> Seq.traverse createTile

    /// 地块 AStar2D 连接初始化
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

    /// 所有地块 AStar2D 连接初始化
    let initTilesConnections<'s> =
        monad {
            let! (di: Injector<'s>) = Reader.ask |> StateT.lift

            return! di.TerrainLayer.GetUsedCells() |> Seq.traverse initTileConnections // traverse f = map f |> sequence
        }

    /// 玩家是否已经没有任何地块
    let isNoLandPlayer (player: Player) =
        monad {
            let! di = Reader.ask |> StateT.lift
            let! (tileSeq: Tile seq) = di.TilesQueryByPlayer player.Id |> StateT.hoist
            return tileSeq |> Seq.length = 0
        }

    /// 玩家占领地块
    let conquerTile (tile: Tile) afterPopulation (conqueror: Player option) =
        monad {
            let! di = Reader.ask |> StateT.lift
            let conquerorId = if conqueror.IsSome then Some conqueror.Value.Id else None

            let! (tile': Tile) =
                di.TileUpdater
                    { tile with
                        PlayerId = conquerorId
                        Population = afterPopulation }
                |> StateT.hoist

            di.TileConquered
                { TileId = tile.Id
                  Coord = tile.Coord
                  BeforePopulation = tile.Population
                  AfterPopulation = afterPopulation
                  ConquerorId = conquerorId
                  LoserId = tile.PlayerId }

            tile'
        }

    /// 玩家初始化出生地块
    let rec spawnPlayer (tiles: Tile list) =
        monad {
            let! di = Reader.ask |> StateT.lift
            let! (player: Player) = di.PlayerFactory |> StateT.hoist
            di.PlayerAdded { PlayerId = player.Id }

            let tile = tiles[di.Random.Next tiles.Length]
            // BUG: 当前实现在 tiles 接近全部有玩家时容易导致死循环
            return!
                match tile.PlayerId with
                | None -> Some player |> conquerTile tile 1<Pop>
                | Some _ -> spawnPlayer tiles
        }

    /// 随机一块玩家领土
    let randomPlayerTile (player: Player) =
        monad {
            let! di = Reader.ask |> StateT.lift
            let! (tileSeq: Tile seq) = di.TilesQueryByPlayer player.Id |> StateT.hoist
            let tiles = tileSeq |> List.ofSeq
            tiles[di.Random.Next tiles.Length]
        }

    /// 随机一部分地块人口
    /// 需要保证玩家控制地块人口均 > 0
    /// （一层 monad，其实没用 Monad Transformer）
    let randomPopulationFromTile (tile: Tile) =
        monad {
            let! di = Reader.ask
            di.Random.Next(1, tile.Population / 1<Pop>) * 1<Pop>
        }

    /// 随机一个相邻地块 id
    /// （一层 monad，其实没用 Monad Transformer）
    let randomConnectedTileId (tile: Tile) =
        monad {
            let! di = Reader.ask
            let (TileId tileId) = tile.Id
            let connectedTileIds = di.AStar.GetPointConnections tileId |> Seq.toList
            TileId connectedTileIds[di.Random.Next connectedTileIds.Length]
        }

    /// 玩家向随机行军目标发出部队
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

    /// 增加地块人口
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
                  PlayerId = tile.PlayerId
                  BeforePopulation = tile.Population
                  AfterPopulation = tile.Population + increment }

            tile'
        }

    /// 行军部队到达目的地
    let arriveArmy (marchingArmy: MarchingArmy) =
        monad {
            let! di = Reader.ask |> StateT.lift |> OptionT.lift
            let! (tile: Tile) = di.TileQueryById marchingArmy.ToTileId |> StateT.hoist |> OptionT
            let! (player: Player) = di.PlayerQueryById marchingArmy.PlayerId |> StateT.hoist |> OptionT

            let! _ =
                match tile.PlayerId with
                | None ->
                    Some player
                    |> conquerTile tile (tile.Population + marchingArmy.Population)
                    |> OptionT.lift
                | Some playerId when playerId = marchingArmy.PlayerId ->
                    increaseTilePopulation marchingArmy.Population tile |> OptionT.lift
                | Some _ when tile.Population > marchingArmy.Population ->
                    increaseTilePopulation -marchingArmy.Population tile |> OptionT.lift
                | Some _ when tile.Population = marchingArmy.Population ->
                    None |> conquerTile tile 0<Pop> |> OptionT.lift
                | Some _ ->
                    Some player
                    |> conquerTile tile (marchingArmy.Population - tile.Population)
                    |> OptionT.lift

            let! resultBool = di.MarchingArmyDeleter marchingArmy.Id |> StateT.hoist |> OptionT.lift
            return resultBool
        }

    /// 行军速度（单位：%/s）
    let marchSpeed =
        function
        | p when p < 10<Pop> -> 50 // 人数小于 10 人，2 秒后到达目的地
        | p when p < 50<Pop> -> 25 // 小于 50 人，4 秒后
        | p when p < 200<Pop> -> 15 // 小于 200 人，7 秒左右后
        | p when p < 1000<Pop> -> 10 // 小于 1000 人，10 秒后
        | _ -> 5 // 大于 1000 人，20 秒后
