namespace BackEnd4IdleStrategyFS.Game

/// 应用服务层
module AppService =
    open System
    open DomainT
    open RepositoryT

    let emptyGameState =
        { playerRepo = Map.empty
          playerNextId = 1
          tileRepo = Map.empty
          tileCoordIndex = Map.empty
          tilePlayerIndex = Map.empty
          tileNextId = 1
          marchingArmyRepo = Map.empty
          marchingArmyNextId = 1 }

    let initTiles tileAdded gameState usedCells =
        let gameState' = CommandRepositoryF.insertTiles tileAdded gameState usedCells

        let tiles =
            usedCells
            |> Seq.map (fun c ->
                let tileOpt = QueryRepositoryF.getTileByCoord gameState' c

                match tileOpt with
                | Some tile -> tile
                | None -> failwith $"init tile not found, coord: {c}")

        gameState', tiles

    let addPopulationToPlayerTiles tilePopulationChanged gameState incrInt =
        let incr = incrInt * 1<Pop>
        let tiles = QueryRepositoryF.getAllTiles gameState

        DomainF.addPopulationToPlayerTiles tilePopulationChanged tiles incr
        |> Seq.fold CommandRepositoryF.updateTile gameState

    let initPlayerAndSpawnOnTile tileConquered gameState tileCoords =
        let tiles = QueryRepositoryF.getTileByCoords gameState tileCoords
        let gameState' = CommandRepositoryF.insertPlayers gameState (Seq.length tiles)
        let players = QueryRepositoryF.getAllPlayers gameState'

        DomainF.playersFirstConquerTiles tileConquered tiles players
        |> Seq.fold CommandRepositoryF.updateTile gameState'

    let randomSendMarchingArmy gameState playerIdInt (navService: int -> int seq) =
        let playerId = PlayerId playerIdInt

        let randomSendMarchingArmyFrom gameState playerId =
            let playerTiles = QueryRepositoryF.getTilesByPlayer gameState playerId |> Seq.toList

            if playerTiles.Length = 0 then
                None
            else
                let random = Random()
                Some playerTiles[random.Next playerTiles.Length].id

        let randomSendMarchingArmyTo gameState fromTileId (candidateToTileIds: int seq) =
            let random = Random()
            let candidateToTileIdsList = candidateToTileIds |> Seq.toList

            let toTileId =
                TileId candidateToTileIdsList[random.Next candidateToTileIdsList.Length]

            let fromTileOpt = QueryRepositoryF.getTile gameState fromTileId

            match fromTileOpt with
            | Some fromTile ->
                let armyPop = random.Next(1, fromTile.population / 1<Pop> + 1) * 1<Pop>

                let fromTile' =
                    { fromTile with
                        population = fromTile.population - armyPop }

                let gameState' = CommandRepositoryF.updateTile gameState fromTile'
                CommandRepositoryF.insertMarchingArmy gameState' fromTile'.playerId.Value armyPop fromTileId toTileId
            | None -> failwith $"Invalid tile id, playerId:{playerIdInt}"

        let marchingArmyOpt =
            randomSendMarchingArmyFrom gameState playerId
            |> Option.map (fun fromTileId ->
                let (TileId tileId) = fromTileId
                let candidateToTileIds = navService tileId
                randomSendMarchingArmyTo gameState fromTileId candidateToTileIds)

        match marchingArmyOpt with
        | Some ma -> ma
        // TODO: 当国家灭亡时可能这里报错，不过 Godot 倒是会直接吞掉……
        | None -> failwith $"No tile to send army, playerId:{playerIdInt}"

    type Maybe() =
        member this.Bind(opt, func) = opt |> Option.bind func
        member this.Return v = Some v
        member this.Zero() = None
        member this.ReturnFrom opt = opt

    /// 部队抵达目的地
    let marchingArmyArriveDestination tileConquered gameState marchingArmyIdInt =
        let marchingArmyId = MarchingArmyId marchingArmyIdInt

        let maybe = Maybe()

        let resultOpt =
            maybe {
                let! marchingArmy = QueryRepositoryF.getMarchingArmy gameState marchingArmyId
                let! tile = QueryRepositoryF.getTile gameState marchingArmy.toTileId
                let! player = QueryRepositoryF.getPlayer gameState marchingArmy.playerId

                let tile' =
                    match tile.playerId with
                    | None ->
                        let tile2 =
                            { tile with
                                population = tile.population + marchingArmy.population }

                        DomainF.conquerTile tileConquered tile2 player
                    | Some playerId when playerId = marchingArmy.playerId ->
                        { tile with
                            population = tile.population + marchingArmy.population }
                    | Some _ when tile.population > marchingArmy.population ->
                        { tile with
                            population = tile.population - marchingArmy.population }
                    | Some _ ->
                        let tile2 =
                            { tile with
                                population = marchingArmy.population - tile.population }

                        DomainF.conquerTile tileConquered tile2 player

                // BUG: 现在有个问题就是这里比 tileConquered 事件慢，所以占领空地时 TileGui 第一次查出来的地块人口还是 0，会延迟显示
                let gameState' = CommandRepositoryF.updateTile gameState tile'
                let gameState'' = CommandRepositoryF.deleteMarchingArmy gameState' marchingArmyId
                let (PlayerId marchingArmyPlayerIdInt) = marchingArmy.playerId
                return gameState'', marchingArmyPlayerIdInt
            }

        match resultOpt with
        | None -> failwith $"Invalid marching army id, id:{marchingArmyIdInt}"
        | Some(gameState', marchingArmyPlayerIdInt) -> gameState', marchingArmyPlayerIdInt

    let queryTileById gameState tileIdInt =
        let tileId = TileId tileIdInt

        match QueryRepositoryF.getTile gameState tileId with
        | Some tile -> tile
        | None -> failwith $"Invalid tile id, id:{tileIdInt}"

    let queryTilesByPlayerId gameState playerIdInt =
        let playerId = PlayerId playerIdInt
        QueryRepositoryF.getTilesByPlayer gameState playerId

    let queryAllPlayers gameState =
        QueryRepositoryF.getAllPlayers gameState

    let queryPlayerById gameState playerIdInt =
        match QueryRepositoryF.getPlayer gameState (PlayerId playerIdInt) with
        | Some player -> player
        | None -> failwith $"Invalid player id, id:{playerIdInt}"

// TODO：为啥这里 [<EntryPoint>] let main argv = 0 就跑不了？main() = () 就可以？但运行后报错
// （感觉和 Rider 识别出来的运行类型有关：单测那边 EntryPoint 识别出来是 .NET Project，而这里 main() 识别出来是 .NET Static Method
// 不知道是不是当成 C# 跑了所以跑不起来）
module Program =

    // 定义 main 函数
    let main () =
        printfn "Hello, World!"

        // 返回退出状态码
        ()
