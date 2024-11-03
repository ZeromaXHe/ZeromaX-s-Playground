namespace BackEnd4IdleStrategyFS.Game

/// 查询数据存储库逻辑
module private QueryRepositoryF =
    open RepositoryT

    let getPlayer playerId gameState = gameState.PlayerRepo.TryFind playerId

    let getAllPlayers gameState = gameState.PlayerRepo.Values |> seq

    let getTile tileId gameState = gameState.TileRepo.TryFind tileId

    let getTileByCoord coord gameState =
        gameState.TileCoordIndex.TryFind coord |> Option.bind (fun c -> getTile c gameState)

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
