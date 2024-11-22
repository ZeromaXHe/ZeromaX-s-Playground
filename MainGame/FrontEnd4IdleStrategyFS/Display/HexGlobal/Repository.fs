namespace FrontEnd4IdleStrategyFS.Display.HexGlobal

open Domain
open Godot
open FSharpPlus
open FSharpPlus.Data

module Repository =
    type Planet =
        { ChunkRepo: Map<int, HexChunk>
          ChunkNextId: int
          TileRepo: Map<int, HexTile>
          TileNextId: int }

    let emptyPlanet =
        { ChunkRepo = Map.empty
          ChunkNextId = 1
          TileRepo = Map.empty
          TileNextId = 1 }

    let getHexTile tileId =
        monad {
            let! planet = State.get
            planet.TileRepo.TryFind tileId
        }

    let insertHexTile center verts =
        monad {
            let! planet = State.get

            let tile =
                { Id = planet.TileNextId
                  NeighborIDs = List.empty
                  ChunkId = None
                  Center = center
                  Vertices = verts
                  Height = 0.0f
                  Color = Colors.White }

            let planet' =
                { planet with
                    TileNextId = planet.TileNextId + 1
                    TileRepo = planet.TileRepo.Add(tile.Id, tile) }

            do! State.put planet'
            tile
        }

    let updateHexTile (tile: HexTile) =
        monad {
            let! planet = State.get
            
            let planet' =
                { planet with
                    TileRepo = planet.TileRepo.Add(tile.Id, tile) }
            do! State.put planet'
            tile
        }

    let getHexChunk chunkId =
        monad {
            let! planet = State.get
            planet.ChunkRepo.TryFind chunkId
        }

    let insertHexChunk origin =
        monad {
            let! planet = State.get

            let chunk =
                { Id = planet.ChunkNextId
                  Origin = origin
                  TileIds = List.empty }

            let planet' =
                { planet with
                    ChunkNextId = planet.ChunkNextId + 1
                    ChunkRepo = planet.ChunkRepo.Add(chunk.Id, chunk) }

            do! State.put planet'
            chunk
        }

    let updateHexChunk (chunk: HexChunk) =
        monad {
            let! planet = State.get
            
            let planet' =
                { planet with
                    ChunkRepo = planet.ChunkRepo.Add(chunk.Id, chunk) }
            do! State.put planet'
            chunk
        }

    