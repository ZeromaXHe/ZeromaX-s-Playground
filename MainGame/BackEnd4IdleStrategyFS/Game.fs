namespace BackEnd4IdleStrategyFS.Game

open System

/// 领域层类型
module DomainT =

    /// 玩家 ID
    type PlayerId = PlayerId of int

    /// 玩家
    type Player = { id: PlayerId }

    /// 单位：人
    [<Measure>]
    type Pop

    /// 地块 ID
    type TileId = TileId of int

    /// 地块
    type Tile =
        { id: TileId
          coord: int * int
          population: int<Pop>
          playerId: PlayerId option }

    /// 地块被占领事件
    type TileConqueredEvent =
        { id: TileId
          conquerorId: PlayerId
          loserId: PlayerId option }

    /// 地块人口变化事件
    type TilePopulationChangedEvent =
        { id: TileId
          beforePopulation: int<Pop>
          afterPopulation: int<Pop> }

    /// 行军部队 ID
    type MarchingArmyId = MarchingArmyId of int

    /// 行军部队
    type MarchingArmy =
        { id: MarchingArmyId
          population: int<Pop>
          playerId: PlayerId
          fromTileId: TileId
          toTileId: TileId }

/// 领域层逻辑
module private DomainF =
    open System
    open DomainT

    // 增加人口
    let increaseTilePopulation increment (tile: Tile) =
        ({ tile with
            population = tile.population + increment },
         { id = tile.id
           beforePopulation = tile.population
           afterPopulation = tile.population + increment })

    // 增加玩家领土人口
    let addPopulationToPlayerTiles (tiles: seq<Tile>) incr =
        tiles
        |> Seq.filter (fun tile -> tile.playerId.IsSome && tile.population < 1000<Pop>)
        |> Seq.map (increaseTilePopulation incr)

    /// 占领地块
    let conquerTile (tile: Tile) (conqueror: Player) =
        ({ tile with
            playerId = Some conqueror.id },
         { id = tile.id
           conquerorId = conqueror.id
           loserId = tile.playerId })

    /// 出兵
    let marchArmy (fromTile: Tile) (toTile: Tile) =
        match fromTile.playerId with
        | Some playerId when fromTile.population > 0<Pop> ->
            // TODO：副作用
            let population = Random().Next(1, fromTile.population / 1<Pop>) * 1<Pop>
            let playerId = playerId
            let fromTileId = fromTile.id
            let toTileId = toTile.id
            playerId, population, fromTileId, toTileId
        | None -> failwith "按道理不应该有这种从不是自己领土出兵的情况"
        | Some value -> failwith $"出兵 fromFile {fromTile.id} 人口为零"

    /// 将玩家分配在地块上
    let playersFirstConquerTiles (tiles: seq<Tile>) (players: seq<Player>) =
        players
        |> Seq.zip tiles
        |> Seq.map (fun (tile, player) ->
            { tile with playerId = Some player.id },
            {id = tile.id
             conquerorId = player.id
             loserId = tile.playerId })

/// 数据存储库类型
module RepositoryT =
    open DomainT

    /// 游戏状态
    type GameState =
        { playerRepo: Map<PlayerId, Player>
          playerNextId: int
          tileRepo: Map<TileId, Tile>
          tileCoordIndex: Map<int * int, TileId>
          tilePlayerIndex: Map<PlayerId, TileId list>
          tileNextId: int
          marchingArmyRepo: Map<MarchingArmyId, MarchingArmy>
          marchingArmyNextId: int }

/// 查询数据存储库逻辑
module private QueryRepositoryF =
    open RepositoryT

    let getPlayer gameState playerId = gameState.playerRepo.TryFind playerId

    let getAllPlayers gameState = gameState.playerRepo.Values |> seq

    let getTile gameState tileId = gameState.tileRepo.TryFind tileId

    let getTileByCoord gameState coord =
        gameState.tileCoordIndex.TryFind coord |> Option.bind (getTile gameState)

    let getTileByCoords gameState coords =
        coords
        |> Seq.map (getTileByCoord gameState)
        |> Seq.filter Option.isSome
        |> Seq.map (fun x -> x.Value)

    let getTilesByPlayer gameState playerId =
        match gameState.tilePlayerIndex.TryFind playerId with
        | Some tileIds ->
            tileIds
            |> Seq.map (getTile gameState)
            |> Seq.filter Option.isSome
            |> Seq.map (fun x -> x.Value) // TODO: 这样实现是不是不太好？
        | None -> []

    let getAllTiles gameState = gameState.tileRepo.Values |> seq

    let getMarchingArmy gameState marchingArmyId =
        if gameState.marchingArmyRepo.ContainsKey marchingArmyId then
            Some <| gameState.marchingArmyRepo[marchingArmyId]
        else
            None

    let getAllMarchingArmies gameState =
        gameState.marchingArmyRepo.Values |> seq

/// 命令数据库逻辑
module private CommandRepositoryF =

    open DomainT
    open RepositoryT
    open QueryRepositoryF

    let insertPlayer gameState =
        let player = { id = PlayerId gameState.playerNextId }

        { gameState with
            playerNextId = gameState.playerNextId + 1
            playerRepo = gameState.playerRepo.Add(player.id, player) }
        
    let rec insertPlayers gameState count =
        match count with
        | 0 -> gameState
        | _ -> insertPlayers (insertPlayer gameState) (count - 1)

    let updateTilePlayerIndex (index: Map<PlayerId, TileId list>) tileId oldPlayerId newPlayerId =
        let index' =
            match oldPlayerId with
            | Some playerId when index.ContainsKey playerId ->
                let list = index[playerId] |> List.filter (fun x -> x <> tileId)

                if list.Length = 0 then
                    index.Remove playerId
                else
                    index.Add(playerId, list)
            | _ -> index

        match newPlayerId with
        | None -> index'
        | Some playerId when index'.ContainsKey playerId -> index'.Add(playerId, tileId :: index'[playerId])
        | Some playerId -> index'.Add(playerId, [ tileId ])

    let updateTile gameState (tile: Tile) =
        // 更新 Tile 逻辑
        let tileModelOption = getTile gameState tile.id

        match tileModelOption with
        | Some tileModel when tileModel.playerId <> tile.playerId ->
            { gameState with
                tileRepo = gameState.tileRepo.Add(tile.id, tile)
                tilePlayerIndex =
                    updateTilePlayerIndex gameState.tilePlayerIndex tile.id tileModel.playerId tile.playerId }
        | _ ->
            { gameState with
                tileRepo = gameState.tileRepo.Add(tile.id, tile) }

    let insertTile gameState coord =
        // 新建 Tile 逻辑
        let nextId = gameState.tileNextId |> TileId

        let tile =
            { id = nextId
              coord = coord
              population = 0<Pop>
              playerId = None }

        { gameState with
            tileNextId = gameState.tileNextId + 1
            tileRepo = gameState.tileRepo.Add(nextId, tile)
            tileCoordIndex = gameState.tileCoordIndex.Add(tile.coord, nextId) }

    let insertTiles gameState coords = coords |> Seq.fold insertTile gameState

    let insertMarchingArmy gameState playerId population fromTileId toTileId =
        let marchingArmy =
            { id = gameState.marchingArmyNextId |> MarchingArmyId
              population = population
              playerId = playerId
              fromTileId = fromTileId
              toTileId = toTileId }

        let gameState' =
            { gameState with
                marchingArmyNextId = gameState.marchingArmyNextId + 1
                marchingArmyRepo = gameState.marchingArmyRepo.Add(marchingArmy.id, marchingArmy) }

        gameState', marchingArmy

    let deleteMarchingArmy gameState marchingArmyId =
        { gameState with
            marchingArmyRepo = gameState.marchingArmyRepo.Remove marchingArmyId }

/// 对外主接口
module MainEntry =
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

    type NavServiceAddEvent = { tileId: TileId; coord: int * int }

    let initTiles gameState usedCells =
        let gameState' = CommandRepositoryF.insertTiles gameState usedCells

        let tiles =
            usedCells
            |> Seq.map (fun c ->
                let tileOpt = QueryRepositoryF.getTileByCoord gameState' c

                match tileOpt with
                | Some tile -> tile
                | None -> failwith $"init tile not found, coord: {c}")
        let idToCoords = tiles |> Seq.map (fun tile -> { tileId = tile.id; coord = tile.coord })

        gameState', tiles, idToCoords

    let addPopulationToPlayerTiles gameState incrInt =
        let incr = incrInt * 1<Pop>
        let tiles = QueryRepositoryF.getAllTiles gameState
        let resSeq = DomainF.addPopulationToPlayerTiles tiles incr
        let gameState' =
            resSeq
                |> Seq.map fst
                |> Seq.fold CommandRepositoryF.updateTile gameState
        let eventSeq = resSeq |> Seq.map snd
        gameState', eventSeq

    let initPlayerAndSpawnOnTile gameState tileCoords =
        let tiles = QueryRepositoryF.getTileByCoords gameState tileCoords
        let gameState' = CommandRepositoryF.insertPlayers gameState (Seq.length tiles)
        let players = QueryRepositoryF.getAllPlayers gameState'

        let tileEventSeq = DomainF.playersFirstConquerTiles tiles players

        let eventSeq = tileEventSeq |> Seq.map snd

        let gameState'' =
            tileEventSeq
            |> Seq.map fst
            |> Seq.fold CommandRepositoryF.updateTile gameState'

        gameState'', eventSeq

    let randomSendMarchingArmy gameState playerIdInt (navService: int -> int list) =
        let playerId = PlayerId playerIdInt
        let randomSendMarchingArmyFrom gameState playerId =
            let playerTiles = QueryRepositoryF.getTilesByPlayer gameState playerId |> Seq.toList

            if playerTiles.Length = 0 then
                None
            else
                let random = Random()
                Some playerTiles[random.Next playerTiles.Length].id

        let randomSendMarchingArmyTo gameState fromTileId (candidateToTileIds: int list) =
            let random = Random()
            let toTileId = TileId candidateToTileIds[random.Next candidateToTileIds.Length]
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
        | None -> failwith $"No tile to send army, playerId:{playerIdInt}"

    // TODO：现在这个 option 嵌套的解决逻辑写的就是一坨……
    /// 部队抵达目的地
    let marchingArmyArriveDestination gameState marchingArmyIdInt =
        let marchingArmyId = MarchingArmyId marchingArmyIdInt
        let armyOpt = QueryRepositoryF.getMarchingArmy gameState marchingArmyId 
        match armyOpt with
        | None -> failwith $"Invalid marching army id, id:{marchingArmyIdInt}"
        | Some marchingArmy ->
            let tileOpt = QueryRepositoryF.getTile gameState marchingArmy.toTileId
            match tileOpt with
            | None -> failwith $"Invalid tile id, armyId:{marchingArmyIdInt}, tileId:{marchingArmy.toTileId}"
            | Some tile ->
                    let playerOpt = QueryRepositoryF.getPlayer gameState marchingArmy.playerId
                    match playerOpt with
                    | None -> failwith $"Invalid player id, armyId:{marchingArmyIdInt}, playerId:{marchingArmy.playerId}"
                    | Some player ->
                        let tile', eOpt =
                            match tile.playerId with
                            | None ->
                                let tile2, e = DomainF.conquerTile tile player
                                { tile2 with population = tile2.population + marchingArmy.population}, Some e
                            | Some playerId->
                                if playerId = marchingArmy.playerId then
                                    { tile with population = tile.population + marchingArmy.population}, None
                                elif tile.population > marchingArmy.population then
                                    { tile with population = tile.population - marchingArmy.population}, None
                                else
                                    let tile2, e = DomainF.conquerTile tile player
                                    { tile with population = marchingArmy.population - tile2.population}, None
                        let gameState' = CommandRepositoryF.updateTile gameState tile'
                        let gameState'' = CommandRepositoryF.deleteMarchingArmy gameState' marchingArmyId
                        let (PlayerId marchingArmyPlayerIdInt) = marchingArmy.playerId 
                        gameState'', marchingArmyPlayerIdInt, eOpt

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

// TODO：为啥这里的 main 就跑不了？
module Program =

    // 定义 main 函数
    [<EntryPoint>]
    let main argv =
        printfn "Hello, World!"
        printfn $"Arguments: %A{argv}"

        // 返回退出状态码
        0