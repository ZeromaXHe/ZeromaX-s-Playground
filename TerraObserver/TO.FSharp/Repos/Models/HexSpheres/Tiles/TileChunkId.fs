namespace TO.FSharp.Repos.Models.HexSpheres.Tiles

open Friflo.Engine.ECS

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-06 11:12:06
[<Struct>]
type TileChunkId =
    interface IComponent
    val ChunkId: ChunkId
    new(chunkId: ChunkId) = { ChunkId = chunkId }
