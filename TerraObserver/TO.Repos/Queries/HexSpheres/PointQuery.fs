namespace TO.Repos.Queries.HexSpheres

open Friflo.Engine.ECS
open Godot
open TO.Domains.Components.HexSpheres.Faces
open TO.Domains.Components.HexSpheres.Points
open TO.Domains.Functions.HexSpheres
open TO.Domains.Alias.HexSpheres.Chunks
open TO.Domains.Alias.HexSpheres.Points
open TO.Domains.Utils.Commons
open TO.Repos.Data.HexSpheres

module private FacePointGetter =
    type TryHeadPointByPosition = Chunky -> Vector3 -> Entity option
    // 按照顺时针方向返回三角形上的在指定顶点后的另外两个顶点
    let getOtherPoints (tryHeadByPosition: TryHeadPointByPosition) =
        fun chunky (face: FaceComponent) (point: PointComponent inref) ->
            // 注意：并没有对 face 和 point 的 Chunky 进行校验
            let idx = face.GetPointIdx &point

            seq {
                tryHeadByPosition chunky <| face[(idx + 1) % 3]
                tryHeadByPosition chunky <| face[(idx + 2) % 3]
            }
    // 顺时针第一个顶点
    let getLeftOtherPoint
        (tryHeadByPosition: TryHeadPointByPosition)
        chunky
        (face: FaceComponent)
        (point: PointComponent inref)
        =
        let idx = face.GetPointIdx &point
        tryHeadByPosition chunky <| face[(idx + 1) % 3]
    // 顺时针第二个顶点
    let getRightOtherPoint
        (tryHeadByPosition: TryHeadPointByPosition)
        chunky
        (face: FaceComponent)
        (point: PointComponent inref)
        =
        let idx = face.GetPointIdx &point
        tryHeadByPosition chunky <| face[(idx + 2) % 3]

type TryHeadPointByPosition = Chunky -> Vector3 -> Entity option
type TryHeadEntityByPointCenterId = PointId -> Entity option
type GetPointNeighborByIdAndIdx = int -> int -> Entity option
type GetPointNeighborIdx = int -> int -> int
type GetPointNeighborIdsById = int -> int seq
type GetNeighborCenterPointIds = Chunky -> FaceComponent list -> PointComponent -> int ResizeArray
type SearchNearestPointCenterPos = Vector3 -> Chunky -> Vector3

[<Interface>]
type IPointQuery =
    abstract TryHeadByPosition: TryHeadPointByPosition
    abstract TryHeadEntityByCenterId: TryHeadEntityByPointCenterId
    abstract GetNeighborByIdAndIdx: GetPointNeighborByIdAndIdx
    abstract GetNeighborIdx: GetPointNeighborIdx
    abstract GetNeighborIdsById: GetPointNeighborIdsById
    abstract GetNeighborCenterPointIds: GetNeighborCenterPointIds
    abstract SearchNearestCenterPos: SearchNearestPointCenterPos

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-19 15:36:19
module PointQuery =
    let tryHeadByPosition (store: EntityStore) : TryHeadPointByPosition =
        fun (chunky: Chunky) (pos: Vector3) ->
            let tag = ChunkFunction.chunkyTag chunky
            // 我们默认只会最多存在一个结果
            FrifloEcsUtil.tryHeadEntity
            <| store
                .Query<PointComponent>()
                .HasValue<PointComponent, Vector3>(pos)
                .AllTags(&tag)

    let tryHeadEntityByCenterId (store: EntityStore) : TryHeadEntityByPointCenterId =
        fun (centerId: PointId) ->
            // 我们默认只会存在最多一个结果
            FrifloEcsUtil.tryHeadEntity
            <| store.Query<PointCenterId>().HasValue<PointCenterId, PointId>(centerId)

    let getNeighborByIdAndIdx (store: EntityStore) : GetPointNeighborByIdAndIdx =
        fun (id: int) (idx: int) ->
            let neighborCenterIds =
                store.GetEntityById(id).GetComponent<PointNeighborCenterIds>()

            if idx >= 0 && idx < neighborCenterIds.Length then
                tryHeadEntityByCenterId store neighborCenterIds[idx]
            else
                None

    let getNeighborIdx (store: EntityStore) : GetPointNeighborIdx =
        fun (id: int) (neighborId: int) ->
            let neighborCenterIds =
                store.GetEntityById(id).GetComponent<PointNeighborCenterIds>()

            let neighborCenterId = store.GetEntityById(neighborId).GetComponent<PointCenterId>()
            PointNeighborCenterIds.getNeighborIdx neighborCenterIds neighborCenterId.CenterId

    let getNeighborIdsById (store: EntityStore) : GetPointNeighborIdsById =
        fun (id: int) ->
            let neighborCenterIds = store.GetEntityById(id).GetComponent<PointNeighborCenterIds>()
            {0..neighborCenterIds.Length - 1}
            |> Seq.collect (fun i ->
                let centerId = neighborCenterIds[i]
                let entities = store.ComponentIndex<PointCenterId, PointId>()[centerId]
                entities |> Seq.map _.Id)

    let getNeighborCenterPointIds (store: EntityStore) : GetNeighborCenterPointIds =
        fun (chunky: bool) (hexFaces: FaceComponent list) (center: PointComponent) ->
            // 使用 inref 时，无法使用闭包
            // hexFaces |> List.map (fun face -> FacePointGetter.getRightOtherPoint (tryHeadByPosition store) chunky face &center)
            let result = ResizeArray<_>() // 对应就是 C# List 在 F# 的别名

            for face in hexFaces do
                let otherPoint =
                    FacePointGetter.getRightOtherPoint (tryHeadByPosition store) chunky face &center

                otherPoint |> Option.iter (fun p -> result.Add p.Id)

            result

    let searchNearestCenterPos (chunkyVpTrees: ChunkyVpTrees) : SearchNearestPointCenterPos =
        fun (pos: Vector3) chunky ->
            let mutable results: Vector3 array = null
            let mutable distances: float32 array = null
            let tree = chunkyVpTrees.Choose chunky
            tree.Search(pos.Normalized(), 1, &results, &distances)
            results[0]
