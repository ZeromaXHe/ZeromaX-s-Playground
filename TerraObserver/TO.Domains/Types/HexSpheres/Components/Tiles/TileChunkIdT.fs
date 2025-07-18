namespace TO.Domains.Types.HexSpheres.Components.Tiles

open Friflo.Engine.ECS
open TO.Domains.Types.HexSpheres

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-06 11:12:06
[<Struct>]
type TileChunkId =
    interface ChunkId IIndexedComponent with
        override this.GetIndexedValue() = this.ChunkId

    val ChunkId: ChunkId
    new(chunkId: ChunkId) = { ChunkId = chunkId }
