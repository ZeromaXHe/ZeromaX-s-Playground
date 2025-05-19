namespace TO.Infras.Planets.Repos

open Friflo.Engine.ECS
open Godot
open TO.Commons.DataStructures
open TO.Infras.Planets.Models.Faces
open TO.Infras.Planets.Models.Tiles

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-19 13:34:19
type TileRepo(store: EntityStore) =
    let typeTile = ComponentTypes.Get<TileComponent>()
    let archetypeTile = store.GetArchetype(&typeTile)
    // 单位球上的分块点 VP 树
    let tilePointVpTree = VpTree<Vector3>()

    let initUnitCentroid (hexFaces: FaceComponent array) =
        (hexFaces |> Array.map _.Center.Normalized() |> Array.sum)
        / (float32 hexFaces.Length)

    let initUnitCorners (hexFaces: FaceComponent array) = hexFaces |> Array.map _.Center

    member this.CreateVpTree(newItems: Vector3 array, distanceCalculator: CalculateDistance<Vector3>) =
        tilePointVpTree.Create(newItems, distanceCalculator)

    member this.SearchNearestTileCenterPos(pos: Vector3) =
        let mutable results: Vector3 array = null
        let mutable distances: float32 array = null
        tilePointVpTree.Search(pos.Normalized(), 1, &results, &distances)
        results[0]

    member this.QueryByCenterId(centerId: int) =
        let entityList =
            store
                .Query<TileComponent>()
                .HasValue<TileComponent, int>(centerId)
                .ToEntityList()

        if entityList.Count = 0 then None else Some entityList[0] // 我们默认只会存在一个点

    member this.Add
        (centerId: int, chunkId: int, hexFaces: FaceComponent array, hexFaceIds: int array, neighborCenterIds: int array) =
        let chunk = archetypeTile.CreateEntity()
        let unitCentroid = initUnitCentroid hexFaces
        let unitCorners = initUnitCorners hexFaces

        let chunkComponent =
            TileComponent(centerId, chunkId, unitCentroid, unitCorners, hexFaceIds, neighborCenterIds)

        chunk.AddComponent<TileComponent>(&chunkComponent) |> ignore
        chunk.Id
