namespace TO.FSharp.Repos.Queries.HexSpheres

open TO.Domains.Alias.HexSpheres.Chunks
open TO.Domains.Components.HexSpheres.Chunks
open TO.Domains.Enums.HexSpheres.Chunks

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-19 18:43:19
module ChunkQuery =
    let isHandlingLodGaps env (lod: ChunkLodEnum) (chunkId: ChunkId) =
        (lod = ChunkLodEnum.PlaneHex
         && PointQuery.getNeighborIdsById env chunkId
            |> Seq.exists (fun id ->
                env.EntityStore.GetEntityById(id).GetComponent<ChunkLod>().Lod
                >= ChunkLodEnum.SimpleHex))
        || (lod = ChunkLodEnum.TerracesHex
            && PointQuery.getNeighborIdsById env chunkId
               |> Seq.exists (fun id ->
                   env.EntityStore.GetEntityById(id).GetComponent<ChunkLod>().Lod = ChunkLodEnum.Full))
