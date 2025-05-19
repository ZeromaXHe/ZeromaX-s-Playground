namespace TO.Infras.Planets.Repos

open Friflo.Engine.ECS
open Godot
open TO.Commons.DataStructures
open TO.Infras.Planets.Models.Chunks

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-18 09:44:18
type ChunkRepo(store: EntityStore) =
    let typeChunk = ComponentTypes.Get<ChunkComponent>()
    let archetypeChunk = store.GetArchetype(&typeChunk)
    // 单位球上的分块点 VP 树
    let chunkPointVpTree = VpTree<Vector3>()

    member this.CreateVpTree(newItems: Vector3 array, distanceCalculator: CalculateDistance<Vector3>) =
        chunkPointVpTree.Create(newItems, distanceCalculator)

    member this.SearchNearestChunk(pos: Vector3) =
        let mutable results: Vector3 array = null
        let mutable distances: float32 array = null
        chunkPointVpTree.Search(pos.Normalized(), 1, &results, &distances)
        results[0]

    member this.AddChunk(centerId: int, pos: Vector3, neighborCenterIds: int array) =
        let chunk = archetypeChunk.CreateEntity()
        let chunkComponent = ChunkComponent(centerId, pos, neighborCenterIds)
        chunk.AddComponent<ChunkComponent>(&chunkComponent) |> ignore
        chunk.Id
