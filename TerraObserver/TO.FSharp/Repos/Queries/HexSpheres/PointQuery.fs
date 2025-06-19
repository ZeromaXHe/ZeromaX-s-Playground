namespace TO.FSharp.Repos.Queries.HexSpheres

open Friflo.Engine.ECS
open Godot
open TO.FSharp.Domains.Components.HexSpheres.Faces
open TO.FSharp.Domains.Components.HexSpheres.Points
open TO.FSharp.Domains.Functions.HexSpheres
open TO.FSharp.Domains.Alias.HexSpheres.Chunks
open TO.FSharp.Domains.Alias.HexSpheres.Points
open TO.FSharp.Domains.Utils.Commons
open TO.FSharp.Repos.Data.Commons
open TO.FSharp.Repos.Data.HexSpheres

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
/// Date: 2025-06-19 15:36:19
module PointQuery =
    let tryHeadByPosition (env: #IEntityStore) =
        fun (chunky: bool) (pos: Vector3) ->
            let tag = ChunkFunction.chunkyTag chunky
            // 我们默认只会最多存在一个结果
            FrifloEcsUtil.tryHeadEntity
            <| env.EntityStore
                .Query<PointComponent>()
                .HasValue<PointComponent, Vector3>(pos)
                .AllTags(&tag)

    let tryHeadEntityByCenterId (env: #IEntityStore) =
        fun (centerId: PointId) ->
            // 我们默认只会存在最多一个结果
            FrifloEcsUtil.tryHeadEntity
            <| env.EntityStore.Query<PointCenterId>().HasValue<PointCenterId, PointId>(centerId)

    let getNeighborByIdAndIdx (env: #IEntityStore) =
        fun (id: int) (idx: int) ->
            let neighborCenterIds =
                env.EntityStore.GetEntityById(id).GetComponent<PointNeighborCenterIds>()

            if idx >= 0 && idx < neighborCenterIds.Length then
                tryHeadEntityByCenterId env neighborCenterIds[idx]
            else
                None

    let getNeighborIdx (env: #IEntityStore) =
        fun (id: int) (neighborId: int) ->
            let neighborCenterIds =
                env.EntityStore.GetEntityById(id).GetComponent<PointNeighborCenterIds>()

            let neighborCenterId =
                env.EntityStore.GetEntityById(neighborId).GetComponent<PointCenterId>()

            neighborCenterIds.GetNeighborIdx neighborCenterId.CenterId

    let getNeighborIdsById (env: #IEntityStore) =
        fun (chunkId: int) ->
            env.EntityStore.GetEntityById(chunkId).GetComponent<PointNeighborCenterIds>()
            |> Seq.collect (fun centerId ->
                let entities = env.EntityStore.ComponentIndex<PointCenterId, PointId>()[centerId]
                entities |> Seq.map _.Id)

    let getNeighborCenterPointIds (env: #IEntityStore) =
        fun (chunky: bool) (hexFaces: FaceComponent list) (center: PointComponent inref) ->
            // 使用 inref 时，无法使用闭包
            // hexFaces |> List.map (fun face -> getRightOtherPoints chunky face center)
            let result = ResizeArray<_>() // 对应就是 C# List 在 F# 的别名

            for face in hexFaces do
                let otherPoint =
                    FacePointGetter.getRightOtherPoint (tryHeadByPosition env) chunky face &center

                otherPoint |> Option.iter (fun p -> result.Add p.Id)

            result

    let searchNearestCenterPos (env: #IChunkyVpTrees) =
        fun (pos: Vector3) chunky ->
            let mutable results: Vector3 array = null
            let mutable distances: float32 array = null
            let tree = env.ChunkyVpTrees.Choose chunky
            tree.Search(pos.Normalized(), 1, &results, &distances)
            results[0]
