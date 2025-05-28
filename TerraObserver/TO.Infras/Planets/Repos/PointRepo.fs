namespace TO.Infras.Planets.Repos

open Friflo.Engine.ECS
open Godot
open TO.Commons.DataStructures
open TO.Infras.Abstractions.Planets.Repos
open TO.Infras.Abstractions.Planets.Models.Faces
open TO.Infras.Abstractions.Planets.Models.Points
open TO.Infras.Planets.Utils

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

    let tryHeadByPosition chunky pos =
        // 我们默认只会最多存在一个结果
        FrifloEcsUtil.tryHeadEntity
        <| store
            .Query<PointComponent>()
            .HasValue<PointComponent, Vector3>(pos)
            .AllTags(if chunky then &tagChunk else &tagTile)

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
            tryHeadByPosition chunky <| face.Vertex((idx + 1) % 3)
            tryHeadByPosition chunky <| face.Vertex((idx + 2) % 3)
        }
    // 顺时针第一个顶点
    let getLeftOtherPoint (chunky, face, point: PointComponent inref) =
        let idx = getPointIdx (face, &point)
        tryHeadByPosition chunky <| face.Vertex((idx + 1) % 3)
    // 顺时针第二个顶点
    let getRightOtherPoint (chunky, face, point: PointComponent inref) =
        let idx = getPointIdx (face, &point)
        tryHeadByPosition chunky <| face.Vertex((idx + 2) % 3)

    let queryByChunky chunky =
        store.Query<PointComponent>().AllTags(if chunky then &tagChunk else &tagTile)


    interface IPointRepo with
        override this.ForEachByChunky chunky forEachPoint =
            FrifloEcsUtil.forEachEntity <| queryByChunky chunky <| forEachPoint

        override this.TryHeadByPosition chunky pos = tryHeadByPosition chunky pos

        override this.Add chunky position coords =
            let point =
                if chunky then
                    store.CreateEntity(PointComponent(position, coords), &tagChunk)
                else
                    store.CreateEntity(PointComponent(position, coords), &tagTile)

            point.Id

        // 使用 inref 时，无法使用函数柯里化形式
        override this.GetNeighborCenterPointIds(chunky, hexFaces: FaceComponent list, center: PointComponent inref) =
            // 使用 inref 时，无法使用闭包
            // hexFaces |> List.map (fun face -> getRightOtherPoints chunky face center)
            let result = ResizeArray<_>() // 对应就是 C# List 在 F# 的别名

            for face in hexFaces do
                let otherPoint = getRightOtherPoint (chunky, face, &center)
                otherPoint |> Option.iter (fun p -> result.Add p.Id)

            result

        override this.CreateVpTree(chunky: bool) =
            let pointQuery = queryByChunky chunky

            let items =
                FrifloEcsUtil.toComponentSeq pointQuery |> Seq.map _.Position |> Seq.toArray

            let tree = if chunky then chunkPointVpTree else tilePointVpTree
            tree.Create(items, fun p0 p1 -> p0.DistanceTo p1)

        override this.SearchNearestCenterPos(pos: Vector3, chunky: bool) =
            let mutable results: Vector3 array = null
            let mutable distances: float32 array = null
            let tree = if chunky then chunkPointVpTree else tilePointVpTree
            tree.Search(pos.Normalized(), 1, &results, &distances)
            results[0]

        override this.Truncate() =
            FrifloEcsUtil.truncate <| store.Query<PointComponent>()
