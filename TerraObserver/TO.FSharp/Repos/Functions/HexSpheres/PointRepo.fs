namespace TO.FSharp.Repos.Functions.HexSpheres

open Godot
open Friflo.Engine.ECS
open TO.FSharp.Commons.DataStructures
open TO.FSharp.Commons.Utils
open TO.FSharp.Repos.Models.HexSpheres.Chunks
open TO.FSharp.Repos.Models.HexSpheres.Faces
open TO.FSharp.Repos.Models.HexSpheres.Points
open TO.FSharp.Repos.Types.HexSpheres.PointRepoT

module private FacePointGetter =
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

module private PointQuery =
    let queryByChunky (dep: ChunkyDep) chunky =
        dep.Store
            .Query<PointComponent>()
            .AllTags(if chunky then &dep.TagChunk else &dep.TagTile)

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-30 10:47:30
module PointRepo =
    let tryHeadByPosition (dep: ChunkyDep) : TryHeadPointByPosition =
        fun chunky pos ->
            // 我们默认只会最多存在一个结果
            FrifloEcsUtil.tryHeadEntity
            <| dep.Store
                .Query<PointComponent>()
                .HasValue<PointComponent, Vector3>(pos)
                .AllTags(if chunky then &dep.TagChunk else &dep.TagTile)

    let tryHeadEntityByPointCenterId (store: EntityStore) : TryHeadEntityByPointId =
        fun centerId ->
            // 我们默认只会存在最多一个结果
            FrifloEcsUtil.tryHeadEntity
            <| store.Query<PointCenterId>().HasValue<PointCenterId, PointId>(centerId)

    let forEachByChunky (dep: ChunkyDep) : ForEachPointByChunky =
        fun chunky forEachPoint -> (PointQuery.queryByChunky dep chunky).ForEachEntity forEachPoint

    let add (dep: ChunkyDep) : AddPoint =
        fun chunky pos coords ->
            let point =
                if chunky then
                    dep.Store.CreateEntity(PointComponent(pos, coords), &dep.TagChunk)
                else
                    dep.Store.CreateEntity(PointComponent(pos, coords), &dep.TagTile)

            point.Id

    let private getNeighborCenterIdsById (store: EntityStore) =
        fun id -> store.GetEntityById(id).GetComponent<PointNeighborCenterIds>()

    let private getCenterIdById (store: EntityStore) =
        fun id -> store.GetEntityById(id).GetComponent<PointCenterId>()

    let getNeighborByIdxAndId (store: EntityStore) : GetNeighborByIdAndIdx =
        fun id idx ->
            let neighborCenterIds = getNeighborCenterIdsById store id

            if idx >= 0 && idx < neighborCenterIds.Length then
                tryHeadEntityByPointCenterId store neighborCenterIds[idx]
            else
                None

    let getNeighborIdx (store: EntityStore) : GetNeighborIdx =
        fun tileId neighborId ->
            let tileNeighborCenterIds = getNeighborCenterIdsById store tileId
            let neighborCenterId = getCenterIdById store neighborId
            tileNeighborCenterIds.GetNeighborIdx neighborCenterId.CenterId

    let getNeighborIdsById (store: EntityStore) : GetNeighborIdsById =
        fun chunkId ->
            store.GetEntityById(chunkId).GetComponent<PointNeighborCenterIds>()
            |> Seq.collect (fun centerId ->
                let entities = store.ComponentIndex<PointCenterId, PointId>()[centerId]
                entities |> Seq.map _.Id)

    // 使用 inref 时，无法使用函数柯里化形式
    let getNeighborCenterPointIds (dep: ChunkyDep) =
        GetNeighborPointIds(fun chunky hexFaces center ->
            // 使用 inref 时，无法使用闭包
            // hexFaces |> List.map (fun face -> getRightOtherPoints chunky face center)
            let result = ResizeArray<_>() // 对应就是 C# List 在 F# 的别名

            for face in hexFaces do
                let otherPoint =
                    FacePointGetter.getRightOtherPoint (tryHeadByPosition dep) chunky face &center

                otherPoint |> Option.iter (fun p -> result.Add p.Id)

            result)

    let createVpTree (dependency: ChunkyDep) (chunkVpTree: Vector3 VpTree) (tileVpTree: Vector3 VpTree) : CreateVpTree =
        fun chunky ->
            let pointQuery = PointQuery.queryByChunky dependency chunky

            let items =
                FrifloEcsUtil.toComponentSeq pointQuery |> Seq.map _.Position |> Seq.toArray

            let tree = if chunky then chunkVpTree else tileVpTree
            tree.Create(items, fun p0 p1 -> p0.DistanceTo p1)

    let searchNearestCenterPos (chunkVpTree: Vector3 VpTree) (tileVpTree: Vector3 VpTree) : SearchNearestCenterPos =
        fun pos chunky ->
            let mutable results: Vector3 array = null
            let mutable distances: float32 array = null
            let tree = if chunky then chunkVpTree else tileVpTree
            tree.Search(pos.Normalized(), 1, &results, &distances)
            results[0]

    let truncate (store: EntityStore) : TruncatePoints =
        fun () -> FrifloEcsUtil.truncate <| store.Query<PointComponent>()

    let getDependency chunkDep chunkVpTree tileVpTree : PointRepoDep =
        { ForEachByChunky = forEachByChunky chunkDep
          TryHeadByPosition = tryHeadByPosition chunkDep
          TryHeadEntityByPointId = tryHeadEntityByPointCenterId chunkDep.Store
          Add = add chunkDep
          GetNeighborByIdAndIdx = getNeighborByIdxAndId chunkDep.Store
          GetNeighborIdx = getNeighborIdx chunkDep.Store
          GetNeighborIdsById = getNeighborIdsById chunkDep.Store
          GetNeighborCenterPointIds = getNeighborCenterPointIds chunkDep
          CreateVpTree = createVpTree chunkDep chunkVpTree tileVpTree
          SearchNearestCenterPos = searchNearestCenterPos chunkVpTree tileVpTree
          Truncate = truncate chunkDep.Store }
