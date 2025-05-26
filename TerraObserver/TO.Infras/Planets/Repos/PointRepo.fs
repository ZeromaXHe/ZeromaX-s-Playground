namespace TO.Infras.Planets.Repos

open Friflo.Engine.ECS
open Godot
open TO.Commons.DataStructures
open TO.Infras.Planets.Models.Faces
open TO.Infras.Planets.Models.Points

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-18 09:37:18
type PointRepo(store: EntityStore) =
    let tagChunk = Tags.Get<PointTagChunk>()
    let tagTile = Tags.Get<PointTagTile>()
    // 单位球上的分块点 VP 树
    let chunkPointVpTree = VpTree<Vector3>()
    // 单位球上的 Tile 点 VP 树
    let tilePointVpTree = VpTree<Vector3>()

    let queryByPosition chunky pos =
        let pointChunks =
            store
                .Query<PointComponent>()
                .HasValue<PointComponent, Vector3>(pos)
                .AllTags(if chunky then &tagChunk else &tagTile)
                .Chunks

        if pointChunks.Count = 0 then
            None
        else
            pointChunks
            |> Seq.tryHead
            |> Option.map (fun chunk ->
                let _, pointEntities = chunk.Deconstruct()
                pointEntities.EntityAt(0)) // 我们默认只会存在一个点

    let getPointIdx (face: FaceComponent, point: PointComponent inref) =
        if face.Vertex1 = point.Position then 0
        elif face.Vertex2 = point.Position then 1
        elif face.Vertex3 = point.Position then 2
        else -1
    // 按照顺时针方向返回三角形上的在指定顶点后的另外两个顶点
    let getOtherPoints (chunky, face, point: PointComponent inref) =
        // 注意：并没有对 face 和 point 的 Chunky 进行校验
        let idx = getPointIdx (face, &point)

        seq {
            queryByPosition chunky <| face.Vertex((idx + 1) % 3)
            queryByPosition chunky <| face.Vertex((idx + 2) % 3)
        }
    // 顺时针第一个顶点
    let getLeftOtherPoint (chunky, face, point: PointComponent inref) =
        let idx = getPointIdx (face, &point)
        queryByPosition chunky <| face.Vertex((idx + 1) % 3)
    // 顺时针第二个顶点
    let getRightOtherPoint (chunky, face, point: PointComponent inref) =
        let idx = getPointIdx (face, &point)
        queryByPosition chunky <| face.Vertex((idx + 2) % 3)

    member this.QueryAllByChunky chunky =
        store.Query<PointComponent>().AllTags(if chunky then &tagChunk else &tagTile)

    member this.QueryByPosition chunky pos = queryByPosition chunky pos

    member this.Add chunky position coords =
        let point =
            if chunky then
                store.CreateEntity(PointComponent(position, coords), &tagChunk)
            else
                store.CreateEntity(PointComponent(position, coords), &tagTile)

        point.Id

    // 使用 inref 时，无法使用函数柯里化形式
    member this.GetNeighborCenterPoints(chunky, hexFaces: FaceComponent list, center: PointComponent inref) =
        // 使用 inref 时，无法使用闭包
        // hexFaces |> List.map (fun face -> getRightOtherPoints chunky face center)
        let result = ResizeArray<_>() // 对应就是 C# List 在 F# 的别名

        for face in hexFaces do
            let otherPoint = getRightOtherPoint (chunky, face, &center)
            otherPoint |> Option.iter result.Add

        result

    member this.CreateVpTree(chunky: bool) =
        let pointQuery = this.QueryAllByChunky chunky
        let items = Array.zeroCreate<Vector3> pointQuery.Count
        let mutable i = 0

        pointQuery.ForEachEntity(fun pComp pEntity ->
            items[i] <- pComp.Position
            i <- i + 1)

        let tree = if chunky then chunkPointVpTree else tilePointVpTree
        tree.Create(items, fun p0 p1 -> p0.DistanceTo p1)

    member this.SearchNearestCenterPos(pos: Vector3, chunky: bool) =
        let mutable results: Vector3 array = null
        let mutable distances: float32 array = null
        let tree = if chunky then chunkPointVpTree else tilePointVpTree
        tree.Search(pos.Normalized(), 1, &results, &distances)
        results[0]
