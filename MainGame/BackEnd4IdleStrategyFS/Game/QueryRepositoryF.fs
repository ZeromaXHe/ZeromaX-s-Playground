namespace BackEnd4IdleStrategyFS.Game

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
