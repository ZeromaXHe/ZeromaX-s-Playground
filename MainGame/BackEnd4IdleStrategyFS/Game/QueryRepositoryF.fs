namespace BackEnd4IdleStrategyFS.Game

open FSharpPlus
open FSharpPlus.Data
open RepositoryT

/// 查询数据存储库逻辑
module private QueryRepositoryF =

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
            |> Option.bind (fun r -> getTile r |> State.eval <| gameState)
        }

    let getTileByCoords coords =
        monad {
            let! gameState = State.get

            coords
            |> Seq.map (fun r -> getTileByCoord r |> State.eval <| gameState)
            |> Seq.filter Option.isSome
            |> Seq.map _.Value
        }

    let getTilesByPlayer playerId =
        monad {
            let! gameState = State.get

            match gameState.TilePlayerIndex.TryFind playerId with
            | Some tileIds ->
                tileIds
                |> Seq.map (fun r -> getTile r |> State.eval <| gameState)
                |> Seq.filter Option.isSome
                |> Seq.map _.Value
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
