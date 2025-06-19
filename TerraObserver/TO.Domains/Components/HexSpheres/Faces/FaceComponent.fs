namespace TO.Domains.Components.HexSpheres.Faces

open Friflo.Engine.ECS
open Godot
open TO.Domains.Components.HexSpheres.Points

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-17 16:10:17
[<Struct>]
type FaceComponent =
    interface IComponent
    val Center: Vector3 // 三角形重心 median point
    // 第一个顶点是非水平边的顶点，后续水平边的两点按照顺时针方向排列
    // ECS 结构中不建议使用 array，更不推荐使用 List、Dictionary 之类的（否则没法有默认值，从而无法生成默认的无参构造函数）
    val Vertex1: Vector3
    val Vertex2: Vector3
    val Vertex3: Vector3

    new(center: Vector3, vertex1: Vector3, vertex2: Vector3, vertex3: Vector3) =
        { Center = center
          Vertex1 = vertex1
          Vertex2 = vertex2
          Vertex3 = vertex3 }

    member this.Vertex i =
        match i with
        | 0 -> this.Vertex1
        | 1 -> this.Vertex2
        | 2 -> this.Vertex3
        | _ -> failwith "Invalid index"

    member this.IsAdjacentTo(face: FaceComponent) =
        // F# 没有 Enumerable.Intersect，因为我们只有三个顶点，所以直接数吧
        let mutable count = 0

        for v1 in [| this.Vertex1; this.Vertex2; this.Vertex3 |] do
            for v2 in [| face.Vertex1; face.Vertex2; face.Vertex3 |] do
                if v1 = v2 then
                    count <- count + 1

        count = 2

    member this.GetPointIdx(point: PointComponent inref) =
        if this.Vertex1 = point.Position then 0
        elif this.Vertex2 = point.Position then 1
        elif this.Vertex3 = point.Position then 2
        else -1
