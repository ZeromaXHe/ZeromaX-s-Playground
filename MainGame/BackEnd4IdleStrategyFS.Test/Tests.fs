module Tests

open System
open BackEnd4IdleStrategyFS.Game
open BackEnd4IdleStrategyFS.Game.DomainT
open Xunit

[<Fact>]
let ``initTiles test`` () =
    let gameState = MainEntry.emptyGameState
    let usedCells = [ (0, 0); (1, 0) ]

    let gameState', tileSeq, eventSeq = MainEntry.initTiles gameState usedCells

    Assert.Equal(gameState'.tileRepo.Count, 2)

    Assert.Equal(
        gameState'.tileRepo.TryFind(TileId 1),
        { id = TileId 1
          coord = (0, 0)
          population = 0<Pop>
          playerId = None }
        |> Some
    )

    Assert.Equal(
        gameState'.tileRepo.TryFind(TileId 2),
        { id = TileId 2
          coord = (1, 0)
          population = 0<Pop>
          playerId = None }
        |> Some
    )

    Assert.Equal(gameState'.tileRepo.TryFind(TileId 3), None)
    Assert.Equal(gameState'.tileCoordIndex.Count, 2)
    Assert.Equal(gameState'.tileCoordIndex.TryFind(0, 0), TileId 1 |> Some)
    Assert.Equal(gameState'.tileCoordIndex.TryFind(1, 0), TileId 2 |> Some)
    Assert.Equal(gameState'.tileCoordIndex.TryFind(1, 1), None)
    Assert.Equal(gameState'.tilePlayerIndex.Count, 0)
    Assert.Equal(gameState'.tileNextId, 3)
    Assert.Equal(Seq.length tileSeq, 2)
    Assert.Equal(Seq.length eventSeq, 2)

[<Fact>]
let ``initPlayerAndSpawnOnTile test`` () =
    let gameState = MainEntry.emptyGameState
    let usedCells = [ (0, 0); (1, 0) ]
    let gameState', _, _ = MainEntry.initTiles gameState usedCells
    let gameState'', eventSeq = MainEntry.initPlayerAndSpawnOnTile gameState' usedCells
    Assert.Equal(gameState''.playerRepo.Count, 2)
    Assert.Equal(gameState''.playerRepo.TryFind(PlayerId 1), { id = PlayerId 1 } |> Some)
    Assert.Equal(gameState''.playerRepo.TryFind(PlayerId 2), { id = PlayerId 2 } |> Some)
    Assert.Equal(gameState''.playerRepo.TryFind(PlayerId 3), None)
    Assert.Equal(gameState''.tilePlayerIndex.Count, 2)
    Assert.Equal(Seq.length eventSeq, 2)
