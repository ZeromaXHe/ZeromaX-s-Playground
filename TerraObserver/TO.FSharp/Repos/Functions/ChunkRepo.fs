namespace TO.FSharp.Repos.Functions

open Friflo.Engine.ECS
open TO.FSharp.Commons.Utils
open TO.FSharp.Repos.Models.HexSpheres.Chunks
open TO.FSharp.Repos.Types.ChunkRepoT

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-30 11:55:30
module ChunkRepo =
    let tryHeadByCenterId (store: EntityStore) : TryHeadChunkByCenterId =
        fun centerId ->
            // 我们默认只会最多存在一个结果
            FrifloEcsUtil.tryHeadEntity
            <| store.Query<ChunkComponent>().HasValue<ChunkComponent, int>(centerId)

    let add (store: EntityStore) : AddChunk =
        fun centerId pos neighborCenterIds -> store.CreateEntity(ChunkComponent(centerId, pos, neighborCenterIds)).Id

    let truncate (store: EntityStore) : TruncateChunks =
        fun () -> FrifloEcsUtil.truncate <| store.Query<ChunkComponent>()

    let getDependency store : ChunkRepoDep =
        { TryHeadByCenterId = tryHeadByCenterId store
          Add = add store
          Truncate = truncate store }
