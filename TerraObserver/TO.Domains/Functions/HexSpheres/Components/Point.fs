namespace TO.Domains.Functions.HexSpheres.Components

open Friflo.Engine.ECS
open Godot
open TO.Domains.Functions.DataStructures
open TO.Domains.Functions.Friflos
open TO.Domains.Functions.HexSpheres.Components.Chunks
open TO.Domains.Functions.HexSpheres.Components.Faces
open TO.Domains.Functions.HexSpheres.Components.Points
open TO.Domains.Types.DataStructures
open TO.Domains.Types.Friflos
open TO.Domains.Types.HexGridCoords
open TO.Domains.Types.HexSpheres
open TO.Domains.Types.HexSpheres.Components
open TO.Domains.Types.HexSpheres.Components.Faces
open TO.Domains.Types.HexSpheres.Components.Points


module private FacePointGetter =
    type TryHeadPointByPosition = Chunky -> Vector3 -> Entity option
    // 按照顺时针方向返回三角形上的在指定顶点后的另外两个顶点
    let getOtherPoints (tryHeadByPosition: TryHeadPointByPosition) =
        fun chunky (face: FaceComponent) (point: PointComponent inref) ->
            // 注意：并没有对 face 和 point 的 Chunky 进行校验
            let idx = face |> FaceComponent.getPointIdx point

            seq {
                tryHeadByPosition chunky <| (FaceComponent.item <| (idx + 1) % 3 <| face)
                tryHeadByPosition chunky <| (FaceComponent.item <| (idx + 2) % 3 <| face)
            }
    // 顺时针第一个顶点
    let getLeftOtherPoint
        (tryHeadByPosition: TryHeadPointByPosition)
        chunky
        (face: FaceComponent)
        (point: PointComponent inref)
        =
        let idx = face |> FaceComponent.getPointIdx point
        tryHeadByPosition chunky <| (FaceComponent.item <| (idx + 1) % 3 <| face)
    // 顺时针第二个顶点
    let getRightOtherPoint
        (tryHeadByPosition: TryHeadPointByPosition)
        chunky
        (face: FaceComponent)
        (point: PointComponent inref)
        =
        let idx = face |> FaceComponent.getPointIdx point
        tryHeadByPosition chunky <| (FaceComponent.item <| (idx + 2) % 3 <| face)

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-19 15:36:19
module PointQuery =
    let tryHeadByPosition (env: #IEntityStoreQuery) : TryHeadPointByPosition =
        fun (chunky: Chunky) (pos: Vector3) ->
            let tag = ChunkFunction.chunkyTag chunky
            // 我们默认只会最多存在一个结果
            env.Query<PointComponent>().HasValue<PointComponent, Vector3>(pos).AllTags(&tag)
            |> ArchetypeQueryQuery.tryHeadEntity

    let tryHeadEntityByCenterId (env: #IEntityStoreQuery) : TryHeadEntityByPointCenterId =
        fun (centerId: PointId) ->
            // 我们默认只会存在最多一个结果
            env.EntityStore
                .Query<PointCenterId>()
                .HasValue<PointCenterId, PointId>(centerId)
            |> ArchetypeQueryQuery.tryHeadEntity

    let getNeighborByIdAndIdx
        (env: 'E when 'E :> IEntityStoreQuery and 'E :> IPointQuery)
        : GetPointNeighborByIdAndIdx =
        fun (id: int) (idx: int) ->
            let neighborCenterIds =
                env.EntityStore.GetEntityById(id).GetComponent<PointNeighborCenterIds>()

            if idx >= 0 && idx < neighborCenterIds.Length then
                neighborCenterIds
                |> PointNeighborCenterIds.item idx
                |> env.TryHeadEntityByCenterId
            else
                None

    let getNeighborIdx (env: #IEntityStoreQuery) : GetPointNeighborIdx =
        fun (id: int) (neighborId: int) ->
            let store = env.EntityStore

            let neighborCenterIds =
                store.GetEntityById(id).GetComponent<PointNeighborCenterIds>()

            let neighborCenterId = store.GetEntityById(neighborId).GetComponent<PointCenterId>()
            PointNeighborCenterIds.getNeighborIdx neighborCenterIds neighborCenterId.CenterId

    let getNeighborIdsById (env: #IEntityStoreQuery) : GetPointNeighborIdsById =
        fun (id: int) ->
            let store = env.EntityStore

            let neighborCenterIds =
                store.GetEntityById(id).GetComponent<PointNeighborCenterIds>()

            { 0 .. neighborCenterIds.Length - 1 }
            |> Seq.collect (fun i ->
                let centerId = neighborCenterIds |> PointNeighborCenterIds.item i
                let entities = store.ComponentIndex<PointCenterId, PointId>()[centerId]
                entities |> Seq.map _.Id)

    let getNeighborCenterPointIds
        (env: 'E when 'E :> IEntityStoreQuery and 'E :> IPointQuery)
        : GetNeighborCenterPointIds =
        fun (chunky: bool) (hexFaces: FaceComponent list) (center: PointComponent) ->
            // 使用 inref 时，无法使用闭包
            // hexFaces |> List.map (fun face -> FacePointGetter.getRightOtherPoint (tryHeadByPosition store) chunky face &center)
            let result = ResizeArray<_>() // 对应就是 C# List 在 F# 的别名

            for face in hexFaces do
                let otherPoint =
                    FacePointGetter.getRightOtherPoint env.TryHeadByPosition chunky face &center

                otherPoint |> Option.iter (fun p -> result.Add p.Id)

            result

    let searchNearestCenterPos (env: #IChunkyVpTreesQuery) : SearchNearestPointCenterPos =
        fun (pos: Vector3) chunky ->
            let tree = env.ChooseVpTreeByChunky chunky
            let results, _ = tree |> (VpTreeQuery.search <| pos.Normalized() <| 1)
            results[0]

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-19 15:50:19
module PointCommand =
    let add (env: #IEntityStoreQuery) : AddPoint =
        fun (chunky: Chunky) (pos: Vector3) (coords: SphereAxial) ->
            let tag = ChunkFunction.chunkyTag chunky
            let point = env.EntityStore.CreateEntity(PointComponent(pos, coords), &tag)
            point.Id

    let createVpTree  (env: 'E when 'E :> IEntityStoreQuery and 'E :> IChunkyVpTreesQuery) : CreateVpTree =
        fun chunky ->
            let tag = ChunkFunction.chunkyTag chunky
            let pointQuery = env.Query<PointComponent>().AllTags(&tag)

            let items =
                pointQuery |> ArchetypeQueryQuery.toComponentSeq |> Seq.map _.Position |> Seq.toArray

            let tree = env.ChooseVpTreeByChunky chunky
            tree |> VpTreeCommand.create items (CalculateDistance(_.DistanceTo))
