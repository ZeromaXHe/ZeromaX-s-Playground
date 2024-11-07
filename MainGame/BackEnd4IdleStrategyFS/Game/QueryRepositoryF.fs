namespace BackEnd4IdleStrategyFS.Game

open RepositoryT

/// 查询数据存储库逻辑
module private QueryRepositoryF =

    let getPlayer playerId gameState = gameState.PlayerRepo.TryFind playerId

    let getAllPlayers gameState = gameState.PlayerRepo.Values |> seq

    let getTile tileId gameState = gameState.TileRepo.TryFind tileId

    let getTileByCoord coord gameState =
        gameState.TileCoordIndex.TryFind coord
        |> Option.bind (fun c -> getTile c gameState)

    let getTileByCoords coords gameState =
        coords
        |> Seq.map (fun c -> getTileByCoord c gameState)
        |> Seq.filter Option.isSome
        |> Seq.map (fun x -> x.Value)

    let getTilesByPlayer playerId gameState =
        match gameState.TilePlayerIndex.TryFind playerId with
        | Some tileIds ->
            tileIds
            |> Seq.map (fun t -> getTile t gameState)
            |> Seq.filter Option.isSome
            |> Seq.map (fun x -> x.Value) // TODO: 这样实现是不是不太好？
        | None -> []

    let getAllTiles gameState = gameState.TileRepo.Values |> seq

    let getMarchingArmy marchingArmyId gameState =
        if gameState.MarchingArmyRepo.ContainsKey marchingArmyId then
            Some <| gameState.MarchingArmyRepo[marchingArmyId]
        else
            None

    let getAllMarchingArmies gameState =
        gameState.MarchingArmyRepo.Values |> seq

/// 查询数据存储库逻辑 monad
module private QueryRepositoryFM =
    open FSharpPlus
    open FSharpPlus.Data

    let getPlayer playerId =
        monad {
            let! gameState = State.get
            gameState.PlayerRepo.TryFind playerId
        }

    let getAllPlayers =
        monad {
            let! gameState = State.get
            seq gameState.PlayerRepo.Values
        }

    let getTile tileId =
        monad {
            let! gameState = State.get
            gameState.TileRepo.TryFind tileId
        }

    let getTileByCoord coord =
        monad {
            let! gameState = State.get

            gameState.TileCoordIndex.TryFind coord
            |> Option.map getTile
            |> Option.bind (fun r -> State.eval r gameState)
        }

    let getTileByCoords coords =
        monad {
            let! gameState = State.get

            coords
            |> Seq.map getTileByCoord
            |> Seq.map (fun r -> State.eval r gameState)
            |> Seq.filter Option.isSome
            |> Seq.map (fun x -> x.Value)
        }

    let getTilesByPlayer playerId =
        monad {
            let! gameState = State.get

            match gameState.TilePlayerIndex.TryFind playerId with
            | Some tileIds ->
                tileIds
                |> Seq.map getTile
                |> Seq.map (fun r -> State.eval r gameState)
                |> Seq.filter Option.isSome
                |> Seq.map (fun x -> x.Value)
            | None -> []
        }

    let getAllTiles =
        monad {
            let! gameState = State.get
            seq gameState.TileRepo.Values
        }

    let getMarchingArmy marchingArmyId =
        monad {
            let! gameState = State.get

            if gameState.MarchingArmyRepo.ContainsKey marchingArmyId then
                Some gameState.MarchingArmyRepo[marchingArmyId]
            else
                None
        }

    let getAllMarchingArmies =
        monad {
            let! gameState = State.get
            seq gameState.MarchingArmyRepo.Values
        }
