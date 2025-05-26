namespace TO.Infras.Planets.Repos

open Friflo.Engine.ECS
open Godot
open TO.Infras.Planets.Models.Chunks

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-18 09:44:18
type ChunkRepo(store: EntityStore) =
    member this.QueryByCenterId(centerId: int) =
        let chunkChunks =
            store.Query<ChunkComponent>().HasValue<ChunkComponent, int>(centerId).Chunks

        if chunkChunks.Count = 0 then
            None
        else
            chunkChunks
            |> Seq.tryHead
            |> Option.map (fun chunk ->
                let _, chunkEntities = chunk.Deconstruct()
                chunkEntities.EntityAt(0)) // 我们默认只会存在一个点

    member this.Add(centerId: int, pos: Vector3, neighborCenterIds: int array) =
        store.CreateEntity(ChunkComponent(centerId, pos, neighborCenterIds)).Id
