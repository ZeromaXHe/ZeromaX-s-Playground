namespace BackEnd4IdleStrategyFS.Game

open FSharpPlus
open FSharpPlus.Data
open BackEnd4IdleStrategyFS.Common.MonadHelper
open DomainT
open Dependency

/// 领域层逻辑（Monad Transformer 实现）
module private DomainFM =

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

            return!
                di.TerrainLayer.GetUsedCells()
                |> Seq.map initTileConnections
                |> Seq.fold stateTReaderFolderM (stateTReaderReturn Seq.empty)
        }

    /// 玩家占领地块
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

    /// 玩家初始化出生地块
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

    /// 初始化 n 个玩家出生地块
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

    /// 随机一块玩家领土
    let randomPlayerTile (player: Player) =
        monad {
            let! di = Reader.ask |> StateT.lift
            let! (tileSeq: Tile seq) = di.TilesQueryByPlayer player.Id |> StateT.hoist
            let tiles = tileSeq |> List.ofSeq
            tiles[di.Random.Next tiles.Length]
        }

    /// 随机一部分地块人口（一层 monad，其实没用 Monad Transformer）
    let randomPopulationFromTile (tile: Tile) =
        monad {
            let! di = Reader.ask
            // BUG: 非常奇葩的情况下，敌方部队可能刚好把地块人口打到 0，这个时候要出兵的地块是当前地块会报错
            di.Random.Next(1, tile.Population / 1<Pop>) * 1<Pop>
        }

    /// 随机一个相邻地块 id（一层 monad，其实没用 Monad Transformer）
    let randomConnectedTileId (tile: Tile) =
        monad {
            let! di = Reader.ask
            let (TileId tileId) = tile.Id
            let connectedTileIds = di.AStar.GetPointConnections tileId |> Seq.toList
            TileId connectedTileIds[di.Random.Next connectedTileIds.Length]
        }

    /// 玩家随机行军目标
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

    /// 行军到达目的地
    let arriveArmy (marchingArmy: MarchingArmy) =
        monad {
            let! di = Reader.ask |> StateT.lift |> OptionT.lift
            let! (tile: Tile) = di.TileQueryById marchingArmy.ToTileId |> StateT.hoist |> OptionT
            let! (player: Player) = di.PlayerQueryById marchingArmy.PlayerId |> StateT.hoist |> OptionT

            let! _ =
                match tile.PlayerId with
                | None ->
                    let tile' =
                        { tile with
                            Population = tile.Population + marchingArmy.Population }

                    conquerTile tile' player |> OptionT.lift
                | Some playerId when playerId = marchingArmy.PlayerId ->
                    let tile' =
                        { tile with
                            Population = tile.Population + marchingArmy.Population }

                    di.TileUpdater tile' |> StateT.hoist |> OptionT.lift
                | Some _ when tile.Population > marchingArmy.Population ->
                    let tile' =
                        { tile with
                            Population = tile.Population - marchingArmy.Population }

                    di.TileUpdater tile' |> StateT.hoist |> OptionT.lift
                | Some _ ->
                    let tile' =
                        { tile with
                            Population = marchingArmy.Population - tile.Population }

                    conquerTile tile' player |> OptionT.lift

            let! resultBool = di.MarchingArmyDeleter marchingArmy.Id |> StateT.hoist |> OptionT.lift
            return resultBool
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
                  BeforePopulation = tile.Population
                  AfterPopulation = tile.Population + increment }

            tile'
        }

    // 增加所有玩家地块人口
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
    
