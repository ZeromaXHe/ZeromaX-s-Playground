namespace TO.FSharp.Repos.Functions.HexSpheres

open Godot
open Friflo.Engine.ECS
open TO.FSharp.Commons.DataStructures
open TO.FSharp.Commons.Structs.HexSphereGrid
open TO.FSharp.Commons.Utils
open TO.FSharp.Repos.Models.HexSpheres.Chunks
open TO.FSharp.Repos.Models.HexSpheres.Faces
open TO.FSharp.Repos.Models.HexSpheres.Points
open TO.FSharp.Repos.Models.HexSpheres.Tiles

module private FacePointGetter =
    type TryHeadPointByPosition = Chunky -> Vector3 -> Entity option
    // 按照顺时针方向返回三角形上的在指定顶点后的另外两个顶点
    let getOtherPoints (tryHeadByPosition: TryHeadPointByPosition) =
        fun chunky (face: FaceComponent) (point: PointComponent inref) ->
            // 注意：并没有对 face 和 point 的 Chunky 进行校验
            let idx = face.GetPointIdx &point

            seq {
                tryHeadByPosition chunky <| face.Vertex((idx + 1) % 3)
                tryHeadByPosition chunky <| face.Vertex((idx + 2) % 3)
            }
    // 顺时针第一个顶点
    let getLeftOtherPoint
        (tryHeadByPosition: TryHeadPointByPosition)
        chunky
        (face: FaceComponent)
        (point: PointComponent inref)
        =
        let idx = face.GetPointIdx &point
        tryHeadByPosition chunky <| face.Vertex((idx + 1) % 3)
    // 顺时针第二个顶点
    let getRightOtherPoint
        (tryHeadByPosition: TryHeadPointByPosition)
        chunky
        (face: FaceComponent)
        (point: PointComponent inref)
        =
        let idx = face.GetPointIdx &point
        tryHeadByPosition chunky <| face.Vertex((idx + 2) % 3)

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-30 10:47:30
module PointRepo =
    let chunkyTag chunky =
        if chunky then Tags.Get<TagChunk>() else Tags.Get<TagTile>()

    let tryHeadByPosition (store: EntityStore) =
        fun (chunky: bool) (pos: Vector3) ->
            let tag = chunkyTag chunky
            // 我们默认只会最多存在一个结果
            FrifloEcsUtil.tryHeadEntity
            <| store
                .Query<PointComponent>()
                .HasValue<PointComponent, Vector3>(pos)
                .AllTags(&tag)

    let tryHeadEntityByCenterId (store: EntityStore) =
        fun (centerId: PointId) ->
            // 我们默认只会存在最多一个结果
            FrifloEcsUtil.tryHeadEntity
            <| store.Query<PointCenterId>().HasValue<PointCenterId, PointId>(centerId)

    let add (store: EntityStore) =
        fun (chunky: bool) (pos: Vector3) (coords: SphereAxial) ->
            let tag = chunkyTag chunky
            let point = store.CreateEntity(PointComponent(pos, coords), &tag)
            point.Id

    let getNeighborByIdAndIdx (store: EntityStore) =
        fun (id: int) (idx: int) ->
            let neighborCenterIds =
                store.GetEntityById(id).GetComponent<PointNeighborCenterIds>()

            if idx >= 0 && idx < neighborCenterIds.Length then
                tryHeadEntityByCenterId store neighborCenterIds[idx]
            else
                None

    let getNeighborIdx (store: EntityStore) =
        fun (id: int) (neighborId: int) ->
            let neighborCenterIds =
                store.GetEntityById(id).GetComponent<PointNeighborCenterIds>()

            let neighborCenterId = store.GetEntityById(neighborId).GetComponent<PointCenterId>()
            neighborCenterIds.GetNeighborIdx neighborCenterId.CenterId

    let getNeighborIdsById (store: EntityStore) =
        fun (chunkId: int) ->
            store.GetEntityById(chunkId).GetComponent<PointNeighborCenterIds>()
            |> Seq.collect (fun centerId ->
                let entities = store.ComponentIndex<PointCenterId, PointId>()[centerId]
                entities |> Seq.map _.Id)

    let getNeighborCenterPointIds (store: EntityStore) =
        fun (chunky: bool) (hexFaces: FaceComponent list) (center: PointComponent inref) ->
            // 使用 inref 时，无法使用闭包
            // hexFaces |> List.map (fun face -> getRightOtherPoints chunky face center)
            let result = ResizeArray<_>() // 对应就是 C# List 在 F# 的别名

            for face in hexFaces do
                let otherPoint =
                    FacePointGetter.getRightOtherPoint (tryHeadByPosition store) chunky face &center

                otherPoint |> Option.iter (fun p -> result.Add p.Id)

            result

    let createVpTree (store: EntityStore) (chunkVpTree: Vector3 VpTree) (tileVpTree: Vector3 VpTree) =
        fun chunky ->
            let tag = chunkyTag chunky
            let pointQuery = store.Query<PointComponent>().AllTags(&tag)

            let items =
                FrifloEcsUtil.toComponentSeq pointQuery |> Seq.map _.Position |> Seq.toArray

            let tree = if chunky then chunkVpTree else tileVpTree
            tree.Create(items, fun p0 p1 -> p0.DistanceTo p1)

    let searchNearestCenterPos (chunkVpTree: Vector3 VpTree) (tileVpTree: Vector3 VpTree) =
        fun (pos: Vector3) chunky ->
            let mutable results: Vector3 array = null
            let mutable distances: float32 array = null
            let tree = if chunky then chunkVpTree else tileVpTree
            tree.Search(pos.Normalized(), 1, &results, &distances)
            results[0]
