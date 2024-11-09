namespace BackEnd4IdleStrategyFS.Game

open BackEnd4IdleStrategyFS.Common.MonadHelper
open DomainT
open Dependency
open FSharpPlus
open FSharpPlus.Data

/// 应用服务层
module AppService =

    /// 初始化 n 个玩家出生地块
    let spawnPlayers playerCount =
        monad {
            let! di = Reader.ask |> StateT.lift
            let! tileSeq = di.TilesQueryAll |> StateT.hoist
            let tiles = tileSeq |> List.ofSeq

            return!
                { 1..playerCount }
                |> Seq.map (fun _ -> DomainF.spawnPlayer tiles)
                // |> Seq.sequence // 这样结果里的 Tile 就推断不出来了……
                |> Seq.fold stateTReaderFolder (stateTReaderReturn Seq.empty)
                // |> Seq.traverse (fun _ -> DomainF.spawnPlayer tiles) // 把 map f |> sequence 简化，一样推断不出 Tile
        }

    /// 初始化游戏
    let init playerCount =
        monad {
            let! di = Reader.ask |> StateT.lift
            let usedCells = di.TerrainLayer.GetUsedCells()
            let! tiles = DomainF.createTiles usedCells
            let! _ = DomainF.initTilesConnections
            let! playerTiles = spawnPlayers playerCount
            tiles, playerTiles
        }

    /// 生成第一波部队
    let generateFirstGroupArmy<'s> =
        monad {
            let! (di: Injector<'s>) = Reader.ask |> StateT.lift
            let! playerSeq = di.PlayersQueryAll |> StateT.hoist
            return! playerSeq |> Seq.map DomainF.marchArmy |> Seq.sequence
        }

    /// 部队抵达，生成新的部队
    let armyArriveAndGenerateNew marchingArmyId playerId =
        monad {
            // 处理抵达部队
            let! di = Reader.ask |> StateT.lift |> OptionT.lift
            let! marchingArmy = di.MarchingArmyQueryById marchingArmyId |> StateT.hoist |> OptionT
            let! _ = DomainF.arriveArmy marchingArmy
            // 派出新的部队
            let! player = di.PlayerQueryById playerId |> StateT.hoist |> OptionT
            return! DomainF.marchArmy player |> OptionT.lift
        }

    /// 增加所有玩家地块人口
    let increaseAllPlayerTilesPopulation increment =
        monad {
            let! di = Reader.ask |> StateT.lift
            let! (tiles: Tile seq) = di.TilesQueryAll |> StateT.hoist

            return!
                tiles
                |> Seq.filter (fun tile -> tile.PlayerId.IsSome && tile.Population < 1000<Pop>)
                |> Seq.map (DomainF.increaseTilePopulation increment)
                // |> Seq.sequence //（类型完全推断不出来，这样会报红。必须使用自己写的辅助类）
                |> Seq.fold stateTReaderFolder (stateTReaderReturn Seq.empty)       
                // |> Seq.traverse (DomainF.increaseTilePopulation increment) 一样推断不出来，报红
        }

    let queryTileById gameState tileIdInt =
        let tileId = TileId tileIdInt

        let tileOpt, _ = QueryRepositoryF.getTile tileId |> State.run <| gameState

        match tileOpt with
        | Some tile -> tile
        | None -> failwith $"Invalid tile id, id:{tileIdInt}"

    let queryTilesByPlayerId gameState playerIdInt =
        let playerId = PlayerId playerIdInt
        let tiles, _ = QueryRepositoryF.getTilesByPlayer playerId |> State.run <| gameState
        tiles

    let queryAllPlayers gameState =
        let players, _ = QueryRepositoryF.getAllPlayers |> State.run <| gameState
        players

// TODO：为啥这里 [<EntryPoint>] let main argv = 0 就跑不了？main() = () 就可以？但运行后报错
// （感觉和 Rider 识别出来的运行类型有关：单测那边 EntryPoint 识别出来是 .NET Project，而这里 main() 识别出来是 .NET Static Method
// 不知道是不是当成 C# 跑了所以跑不起来）
module Program =

    // 定义 main 函数
    let main () =
        printfn "Hello, World!"

        // 返回退出状态码
        ()
