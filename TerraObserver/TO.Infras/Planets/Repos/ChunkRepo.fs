namespace TO.Infras.Planets.Repos

open Friflo.Engine.ECS
open Godot
open TO.Infras.Planets.Models.Chunks
open TO.Infras.Planets.Utils

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-18 09:44:18
type ChunkRepo(store: EntityStore) =
    member this.TryHeadByCenterId(centerId: int) =
        // 我们默认只会最多存在一个结果
        FrifloEcsUtil.tryHeadEntity
        <| store.Query<ChunkComponent>().HasValue<ChunkComponent, int>(centerId)

    member this.Add(centerId: int, pos: Vector3, neighborCenterIds: int array) =
        store.CreateEntity(ChunkComponent(centerId, pos, neighborCenterIds)).Id
