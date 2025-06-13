namespace TO.FSharp.Repos.Functions.HexSpheres

open Friflo.Engine.ECS
open Godot.Abstractions.Extensions.Chunks
open TO.FSharp.Commons.Utils
open TO.FSharp.Repos.Models.HexSpheres.Chunks
open TO.FSharp.Repos.Models.HexSpheres.Points
open TO.FSharp.Repos.Types.HexSpheres.ChunkRepoT

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-30 11:55:30
module ChunkRepo =
    let add (store: EntityStore) : AddChunk =
        fun centerId pos neighborCenterIds ->
            store
                .CreateEntity(
                    PointCenterId centerId, // 注意，此处对应的是中心点投射到单位球上的 Point id。
                    PointNeighborCenterIds neighborCenterIds, // 已确保顺序和 HexFaceIds 对应，每个邻居共边的顶点是 HexFaceIds[i] 和 HexFaceIds[(i + 1) % HexFaceIds.Count]
                    ChunkPos pos,
                    ChunkLod ChunkLodEnum.JustHex,
                    ChunkInsight false
                )
                .Id

    let truncate (store: EntityStore) : TruncateChunks =
        fun () -> FrifloEcsUtil.truncate <| store.Query<ChunkPos>()

    let getLodById (store: EntityStore) : GetChunkLodEnumById =
        fun chunkId -> store.GetEntityById(chunkId).GetComponent<ChunkLod>().Lod

    let getPosById (store: EntityStore) : GetChunkPosById =
        fun chunkId -> store.GetEntityById(chunkId).GetComponent<ChunkPos>().Pos

    let forEachPos (store: EntityStore) : ForEachChunkPos =
        fun forEachFace -> store.Query<ChunkPos>().ForEachEntity forEachFace

    /// 如果 inLoop，则需要通过 commitCommands 提交
    let updateChunkInsightAndLodById (store: EntityStore) : UpdateChunkInsightAndLodById =
        fun (id: int) (insight: bool) (lod: ChunkLodEnum) (inLoop: bool) ->
            let chunkInsight = ChunkInsight insight
            let chunkLod = ChunkLod lod
            // 更新组件
            if inLoop then
                let cb = store.GetCommandBuffer()
                cb.AddComponent<ChunkInsight>(id, &chunkInsight)
                cb.AddComponent<ChunkLod>(id, &chunkLod)
            else
                let chunk = store.GetEntityById(id)
                chunk.AddComponent<ChunkInsight>(&chunkInsight) |> ignore
                chunk.AddComponent<ChunkLod>(&chunkLod) |> ignore

    let commitCommands (store: EntityStore) : CommitChunkCommands =
        fun () ->
            let cb = store.GetCommandBuffer()
            cb.Playback()
            cb.Clear()

    let getDependency store : ChunkRepoDep =
        { Add = add store
          Truncate = truncate store
          GetLodEnumById = getLodById store
          GetPosById = getPosById store
          ForEachPos = forEachPos store
          UpdateInsightAndLodById = updateChunkInsightAndLodById store
          CommitCommands = commitCommands store }
