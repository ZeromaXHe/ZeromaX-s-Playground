namespace BackEnd4IdleStrategyFS.Game

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
