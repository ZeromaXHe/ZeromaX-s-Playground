namespace BackEnd4IdleStrategyFS.Game

/// 命令数据库逻辑
module private CommandRepositoryF =
    open DomainT
    open RepositoryT
    open QueryRepositoryF

    let insertPlayer gameState =
        let player = { Id = PlayerId gameState.PlayerNextId }

        { gameState with
            PlayerNextId = gameState.PlayerNextId + 1
            PlayerRepo = gameState.PlayerRepo.Add(player.Id, player) }, player

    let insertPlayers gameState count =
        [1..count]
        |> List.fold (fun s _ ->
            let s', _ = insertPlayer s
            s') gameState

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

    let updateTile (tile: Tile) gameState =
        // 更新 Tile 逻辑
        let tileModelOption = getTile tile.Id gameState

        match tileModelOption with
        | Some tileModel when tileModel.PlayerId <> tile.PlayerId ->
            { gameState with
                TileRepo = gameState.TileRepo.Add(tile.Id, tile)
                TilePlayerIndex =
                    updateTilePlayerIndex gameState.TilePlayerIndex tile.Id tileModel.PlayerId tile.PlayerId }
        | _ ->
            { gameState with
                TileRepo = gameState.TileRepo.Add(tile.Id, tile) }

    let insertTile coord gameState =
        // 新建 Tile 逻辑
        let nextId = gameState.TileNextId |> TileId

        let tile =
            { Id = nextId
              Coord = coord
              Population = 0<Pop>
              PlayerId = None }

        { gameState with
            TileNextId = gameState.TileNextId + 1
            TileRepo = gameState.TileRepo.Add(nextId, tile)
            TileCoordIndex = gameState.TileCoordIndex.Add(tile.Coord, nextId) }, tile

    let insertTiles coords gameState =
        coords
        |> Seq.fold (fun s c ->
            let s', _ = insertTile c s
            s') gameState

    let insertMarchingArmy population playerId fromTileId toTileId gameState =
        let marchingArmy =
            { Id = gameState.MarchingArmyNextId |> MarchingArmyId
              Population = population
              PlayerId = playerId
              FromTileId = fromTileId
              ToTileId = toTileId }

        let gameState' =
            { gameState with
                MarchingArmyNextId = gameState.MarchingArmyNextId + 1
                MarchingArmyRepo = gameState.MarchingArmyRepo.Add(marchingArmy.Id, marchingArmy) }

        gameState', marchingArmy

    let deleteMarchingArmy marchingArmyId gameState =
        { gameState with
            MarchingArmyRepo = gameState.MarchingArmyRepo.Remove marchingArmyId }
