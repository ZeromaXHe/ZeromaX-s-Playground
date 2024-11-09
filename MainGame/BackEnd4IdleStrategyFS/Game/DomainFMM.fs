namespace BackEnd4IdleStrategyFS.Game

open BackEnd4IdleStrategyFS.Common.MonadHelper
open DomainT
open Dependency
open FSharpPlus
open FSharpPlus.Data

/// 领域层逻辑（双层 Monad 实现）
module private DomainFMM =
    /// 创建单个地块
    let createTile coord =
        monad {
            let! di = Reader.ask

            monad {
                let! (tile: Tile) = di.TileFactory coord 0<Pop> None
                di.TileAdded { TileId = tile.Id; Coord = tile.Coord }
                return tile
            }
        }

    /// 创建多个地块
    let createTiles coords =
        // 必须断一下，不然类型推断推不出来 Tile 类型
        let r = coords |> Seq.traverse createTile
        r |> Reader.map Seq.sequence

    /// 地块 AStar2D 连接初始化
    let initTileConnections cell =
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

    /// 所有地块 AStar2D 连接初始化
    let initTilesConnections<'s> =
        monad {
            let! (di: Injector<'s>) = Reader.ask
            return!
                di.TerrainLayer.GetUsedCells()
                |> Seq.map initTileConnections
                |> Seq.fold readerFolderM (readerReturn Seq.empty)
        }

    /// 玩家占领地块
    let conquerTile (tile: Tile) (conqueror: Player) =
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

    /// 玩家初始化出生地块
    let rec spawnPlayer (tiles: Tile list) =
        monad {
            let! di = Reader.ask

            monad {
                let! player = di.PlayerFactory
                let tile = tiles[di.Random.Next tiles.Length]

                return!
                    match tile.PlayerId with
                    | None -> conquerTile tile player |> Reader.run <| di
                    | Some _ -> spawnPlayer tiles |> Reader.run <| di
            }
        }

    /// 初始化 n 个玩家出生地块
    let spawnPlayers playerCount =
        monad {
            let! di = Reader.ask

            monad {
                let! tileSeq = di.TilesQueryAll
                let tiles = tileSeq |> List.ofSeq

                return!
                    [ 1..playerCount ]
                    |> Seq.map (fun _ -> spawnPlayer tiles |> Reader.run <| di)
                    |> Seq.sequence
            }
        }

    /// 随机一块玩家领土
    let randomPlayerTile (player: Player) =
        monad {
            let! di = Reader.ask

            monad {
                let! (tileSeq: Tile seq) = di.TilesQueryByPlayer player.Id
                let tiles = tileSeq |> List.ofSeq
                tiles[di.Random.Next tiles.Length]
            }
        }

    /// 随机一部分地块人口（一层 monad）
    let randomPopulationFromTile (tile: Tile) =
        monad {
            let! di = Reader.ask
            di.Random.Next(1, tile.Population / 1<Pop>) * 1<Pop>
        }

    /// 随机一个相邻地块 id（一层 monad）
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
            let! di = Reader.ask

            monad {
                let! fromTile = randomPlayerTile player |> Reader.run <| di
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

    /// 行军到达目的地
    let arriveArmy (marchingArmy: MarchingArmy) =
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

                        conquerTile tile' player |> Reader.run <| di |> OptionT.lift
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

                        conquerTile tile' player |> Reader.run <| di |> OptionT.lift

                let! resultBool = di.MarchingArmyDeleter marchingArmy.Id |> OptionT.lift
                return resultBool
            }
            |> OptionT.run
        }

    /// 增加地块人口
    let increaseTilePopulation increment (tile: Tile) =
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

    // 增加所有玩家地块人口
    let increaseAllPlayerTilesPopulation increment =
        monad {
            let! di = Reader.ask

            monad {
                let! (tiles: Tile seq) = di.TilesQueryAll

                return!
                    tiles
                    |> Seq.filter (fun tile -> tile.PlayerId.IsSome && tile.Population < 1000<Pop>)
                    |> Seq.map (fun t -> increaseTilePopulation increment t |> Reader.run <| di)
                    |> Seq.sequence
            }
        }