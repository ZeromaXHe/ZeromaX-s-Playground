namespace TO.Repos.Queries.HexSpheres

open Friflo.Engine.ECS
open TO.Domains.Alias.HexSpheres.Chunks
open TO.Domains.Components.HexSpheres.Chunks
open TO.Domains.Enums.HexSpheres.Chunks

type IsHandlingChunkLodGaps = ChunkLodEnum -> ChunkId -> bool
type GetChunkLod = ChunkId -> ChunkLodEnum

[<Interface>]
type IChunkQuery =
    abstract IsHandlingLodGaps: IsHandlingChunkLodGaps
    abstract GetLod: GetChunkLod

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

    let getLod (store: EntityStore): GetChunkLod =
        fun (chunkId: ChunkId) ->
            store.GetEntityById(chunkId).GetComponent<ChunkLod>().Lod