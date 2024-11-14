namespace BackEnd4IdleStrategyFS.Game

open FSharpPlus
open FSharpPlus.Data
open DomainT
open RepositoryT
open QueryRepositoryF

/// 命令数据库逻辑
module private CommandRepositoryF =

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
        { 1..count } |> Seq.traverse (fun _ -> insertPlayer)

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
            let! tileModelOption = getTile tile.Id

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
        coords |> Seq.traverse (fun c -> insertTile c population playerIdOpt)

    let insertMarchingArmy population playerId fromTileId toTileId =
        monad {
            let! gameState = State.get

            let marchingArmy =
                { Id = gameState.MarchingArmyNextId |> MarchingArmyId
                  Population = population
                  PlayerId = playerId
                  FromTileId = fromTileId
                  ToTileId = toTileId
                  Progress = 0.0 }

            let gameState' =
                { gameState with
                    MarchingArmyNextId = gameState.MarchingArmyNextId + 1
                    MarchingArmyRepo = gameState.MarchingArmyRepo.Add(marchingArmy.Id, marchingArmy) }

            do! State.put gameState'
            marchingArmy
        }

    let updateMarchingArmy marchingArmy =
        monad {
            let! gameState = State.get

            let gameState' =
                { gameState with
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
