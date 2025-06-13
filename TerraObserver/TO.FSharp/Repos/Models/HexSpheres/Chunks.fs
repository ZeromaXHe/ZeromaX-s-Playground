namespace TO.FSharp.Repos.Models.HexSpheres.Chunks

open Friflo.Engine.ECS
open TO.FSharp.Repos.Models.HexSpheres.Tiles

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-16 19:30:16
[<Struct>]
type ChunkToTileId =
    interface TileId IRelation with
        member this.GetRelationKey() = this.TileId

    val TileId: TileId
    new(tileId: TileId) = { TileId = tileId }

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-18 13:43:18
[<Struct>]
type TagChunk =
    interface ITag

type ChunkyDep =
    { Store: EntityStore
      TagChunk: Tags
      TagTile: Tags }
