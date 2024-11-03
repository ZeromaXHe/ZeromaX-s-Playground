namespace BackEnd4IdleStrategyFS.Game

open DomainT

/// 应用服务层
module AppService =

     let queryTileById gameState tileIdInt =
         let tileId = TileId tileIdInt

         match QueryRepositoryF.getTile tileId gameState with
         | Some tile -> tile
         | None -> failwith $"Invalid tile id, id:{tileIdInt}"

     let queryTilesByPlayerId gameState playerIdInt =
         let playerId = PlayerId playerIdInt
         QueryRepositoryF.getTilesByPlayer playerId gameState

     let queryAllPlayers gameState =
         QueryRepositoryF.getAllPlayers gameState

// TODO：为啥这里 [<EntryPoint>] let main argv = 0 就跑不了？main() = () 就可以？但运行后报错
// （感觉和 Rider 识别出来的运行类型有关：单测那边 EntryPoint 识别出来是 .NET Project，而这里 main() 识别出来是 .NET Static Method
// 不知道是不是当成 C# 跑了所以跑不起来）
module Program =

    // 定义 main 函数
    let main () =
        printfn "Hello, World!"

        // 返回退出状态码
        ()
