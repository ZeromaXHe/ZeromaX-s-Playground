namespace TO.FSharp.Repos.Models.HexSpheres.Chunks

open Friflo.Engine.ECS
open Godot

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-16 19:30:16
[<Struct>]
type ChunkComponent =
    interface IComponent
    val Pos: Vector3 // 这里存储是实际分块中心位置（带有星球半径）
    new(pos: Vector3) = { Pos = pos }

[<Struct>]
type ChunkToTileId =
    interface IRelation<int> with
        member this.GetRelationKey() = this.TileId

    val TileId: int
    new(tileId: int) = { TileId = tileId }

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
