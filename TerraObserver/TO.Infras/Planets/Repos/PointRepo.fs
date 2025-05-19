namespace TO.Infras.Planets.Repos

open Friflo.Engine.ECS
open Godot
open TO.Infras.Planets.Models.Faces
open TO.Infras.Planets.Models.Points

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-18 09:37:18
type PointRepo(store: EntityStore) =
    let typePoint = ComponentTypes.Get<PointComponent>()
    let tagChunk = Tags.Get<PointTagChunk>()
    let tagTile = Tags.Get<PointTagTile>()

    let archetypeChunk = store.GetArchetype(&typePoint, &tagChunk)
    let archetypeTile = store.GetArchetype(&typePoint, &tagTile)
    let IndexByPosition = store.ComponentIndex<PointComponent, Vector3>()

    let queryByPosition chunky pos =
        let entityList =
            store
                .Query<PointComponent>()
                .HasValue<PointComponent, Vector3>(pos)
                .AllTags(if chunky then &tagChunk else &tagTile)
                .ToEntityList()

        if entityList.Count = 0 then None else Some entityList[0] // 我们默认只会存在一个点

    let getPointIdx (face: FaceComponent, point: inref<PointComponent>) =
        // Array.findIndex 自己会抛异常，没必要重复检测
        // if face.TriVertices |> Array.forall (fun v -> v <> point.Position) then
        //     failwith "Given point must be one of the points on the face!"
        // 采用 inref 的话，就只能用 for 循环了
        // face.TriVertices |> Array.findIndex (fun v -> v = point.Position)
        let mutable idx = -1

        for i = 0 to face.TriVertices.Length - 1 do
            if face.TriVertices[i] = point.Position then
                idx <- i

        idx
    // 按照顺时针方向返回三角形上的在指定顶点后的另外两个顶点
    let getOtherPoints (chunky, face, point: inref<PointComponent>) =
        // 注意：并没有对 face 和 point 的 Chunky 进行校验
        let idx = getPointIdx (face, &point)

        seq {
            queryByPosition chunky face.TriVertices[(idx + 1) % 3]
            queryByPosition chunky face.TriVertices[(idx + 2) % 3]
        }
    // 顺时针第一个顶点
    let getLeftOtherPoint (chunky, face, point: inref<PointComponent>) =
        let idx = getPointIdx (face, &point)
        queryByPosition chunky face.TriVertices[(idx + 1) % 3]
    // 顺时针第二个顶点
    let getRightOtherPoint (chunky, face, point: inref<PointComponent>) =
        let idx = getPointIdx (face, &point)
        queryByPosition chunky face.TriVertices[(idx + 2) % 3]

    member this.QueryAllByChunky chunky =
        store.Query<PointComponent>().AllTags(if chunky then &tagChunk else &tagTile)

    member this.QueryByPosition chunky pos = queryByPosition chunky pos

    member this.Add chunky position coords =
        let point =
            if chunky then
                archetypeChunk.CreateEntity()
            else
                archetypeTile.CreateEntity()

        let pointComponent = PointComponent(position, coords)
        point.AddComponent<PointComponent>(&pointComponent) |> ignore
        point.Id

    // 使用 inref 时，无法使用函数柯里化形式
    member this.GetNeighborCenterPoints(chunky, hexFaces: FaceComponent list, center: inref<PointComponent>) =
        // 使用 inref 时，无法使用闭包
        // hexFaces |> List.map (fun face -> getRightOtherPoints chunky face center)
        let result = ResizeArray<_>() // 对应就是 C# List 在 F# 的别名

        for face in hexFaces do
            let otherPoint = getRightOtherPoint (chunky, face, &center)
            otherPoint |> Option.iter result.Add

        result |> List.ofSeq
