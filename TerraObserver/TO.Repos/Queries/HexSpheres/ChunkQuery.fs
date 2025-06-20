namespace TO.Repos.Queries.HexSpheres

open TO.Domains.Alias.HexSpheres.Chunks
open TO.Domains.Components.HexSpheres.Chunks
open TO.Domains.Enums.HexSpheres.Chunks

type IsHandlingChunkLodGaps = ChunkLodEnum -> ChunkId -> bool

[<Interface>]
type IChunkQuery =
    abstract IsHandlingLodGaps: IsHandlingChunkLodGaps

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-19 18:43:19
module ChunkQuery =
    let isHandlingLodGaps store : IsHandlingChunkLodGaps =
        fun (lod: ChunkLodEnum) (chunkId: ChunkId) ->
            (lod = ChunkLodEnum.PlaneHex
             && PointQuery.getNeighborIdsById store chunkId
                |> Seq.exists (fun id -> store.GetEntityById(id).GetComponent<ChunkLod>().Lod >= ChunkLodEnum.SimpleHex))
            || (lod = ChunkLodEnum.TerracesHex
                && PointQuery.getNeighborIdsById store chunkId
                   |> Seq.exists (fun id -> store.GetEntityById(id).GetComponent<ChunkLod>().Lod = ChunkLodEnum.Full))
