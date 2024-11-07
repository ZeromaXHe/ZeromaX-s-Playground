namespace BackEnd4IdleStrategyFS.Game

open BackEnd4IdleStrategyFS.Common
open DomainT
open RepositoryT

/// 命令数据库逻辑
module private CommandRepositoryF =
    open QueryRepositoryF

    let insertPlayer gameState =
        let player = { Id = PlayerId gameState.PlayerNextId }

        { gameState with
            PlayerNextId = gameState.PlayerNextId + 1
            PlayerRepo = gameState.PlayerRepo.Add(player.Id, player) },
        player

    let insertPlayers gameState count =
        [ 1..count ]
        |> List.fold
            (fun s _ ->
                let s', _ = insertPlayer s
                s')
            gameState

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
            TileCoordIndex = gameState.TileCoordIndex.Add(tile.Coord, nextId) },
        tile

    let insertTiles coords gameState =
        coords
        |> Seq.fold
            (fun s c ->
                let s', _ = insertTile c s
                s')
            gameState

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

/// 命令数据库逻辑 monad 版
module private CommandRepositoryFM =
    open FSharpPlus
    open FSharpPlus.Data
    open QueryRepositoryFM

    let insertPlayer =
        monad {
            let! gameState = State.get
            let player = { Id = PlayerId gameState.PlayerNextId }

            do!
                State.put
                    { gameState with
                        PlayerNextId = gameState.PlayerNextId + 1
                        PlayerRepo = gameState.PlayerRepo.Add(player.Id, player) }

            player
        }

    let insertPlayers count =
        [ 1..count ] |> Seq.map (fun _ -> insertPlayer) |> Seq.sequence

    let insertPlayers' count =
        monad {
            let! (gameState: GameState) = State.get

            let gameState', players =
                [ 1..count ]
                |> List.map (fun _ -> insertPlayer)
                |> List.fold
                    (fun tuple m ->
                        let state, list = tuple
                        let p, s = State.run m state
                        s, p :: list)
                    (gameState, [])

            do! State.put gameState'
            players
        }

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

    /// 更新 Tile 逻辑
    let updateTile (tile: Tile) =
        monad {
            let! gameState = State.get
            let tileModelOption = getTile tile.Id |> State.eval <| gameState

            let gameState' =
                match tileModelOption with
                | Some tileModel when tileModel.PlayerId <> tile.PlayerId ->
                    { gameState with
                        TileRepo = gameState.TileRepo.Add(tile.Id, tile)
                        TilePlayerIndex =
                            updateTilePlayerIndex gameState.TilePlayerIndex tile.Id tileModel.PlayerId tile.PlayerId }
                | _ ->
                    { gameState with
                        TileRepo = gameState.TileRepo.Add(tile.Id, tile) }

            do! State.put gameState'
            tile
        }

    /// 新建 Tile 逻辑
    let insertTile coord population playerIdOpt =
        monad {
            let! gameState = State.get
            let nextId = gameState.TileNextId |> TileId

            let tile =
                { Id = nextId
                  Coord = coord
                  Population = population
                  PlayerId = playerIdOpt }

            let gameState' =
                { gameState with
                    TileNextId = gameState.TileNextId + 1
                    TileRepo = gameState.TileRepo.Add(nextId, tile)
                    TileCoordIndex = gameState.TileCoordIndex.Add(tile.Coord, nextId) }

            do! State.put gameState'
            tile
        }

    let insertTiles coords population playerIdOpt =
        coords |> Seq.map (fun c -> insertTile c population playerIdOpt) |> Seq.sequence

    let insertTiles' coords population playerIdOpt =
        monad {
            let! (gameState: GameState) = State.get

            let gameState', tiles =
                coords
                |> Seq.map (fun c -> insertTile c population playerIdOpt)
                |> Seq.fold
                    (fun tuple m ->
                        let state, seq = tuple
                        let p, s = State.run m state
                        s, Seq.append seq [ p ])
                    (gameState, Seq.empty)

            do! State.put gameState'
            tiles
        }


    let insertMarchingArmy population playerId fromTileId toTileId =
        monad {
            let! gameState = State.get

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

            do! State.put gameState'
            marchingArmy
        }

    let deleteMarchingArmy marchingArmyId =
        monad {
            let! gameState = State.get
            let exist = gameState.MarchingArmyRepo.ContainsKey marchingArmyId

            let gameState' =
                { gameState with
                    MarchingArmyRepo = gameState.MarchingArmyRepo.Remove marchingArmyId }

            do! State.put gameState'
            exist
        }
