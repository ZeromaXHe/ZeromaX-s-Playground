namespace TO.Infras.Planets.Models

open Friflo.Engine.ECS
open Godot

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-18 13:43:18
module Faces =
    [<Struct>]
    type FaceTagChunk =
        interface ITag

    [<Struct>]
    type FaceTagTile =
        interface ITag

    /// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
    /// Author: Zhu XH (ZeromaXHe)
    /// Date: 2025-05-17 16:10:17
    [<Struct>]
    type FaceComponent =
        interface IComponent
        val Center: Vector3 // 三角形重心 median point
        // 需要注意 struct 的成员必须是 array 而不是 list，否则没法有默认值，从而无法生成默认的无参构造函数
        // （之前索引 IIndexedComponent 将因此无法正常使用，不确定 IComponent 的情况）
        // 之所以不用 ILinkRelation 功能，是因为我们的顶点 id 需要保证顺序，并且
        val TriVertices: Vector3 array // 第一个顶点是非水平边的顶点，后续水平边的两点按照顺时针方向排列

        new(center: Vector3, triVertices: Vector3 array) =
            { Center = center
              TriVertices = triVertices }

        member this.IsAdjacentTo(face: FaceComponent) =
            // F# 没有 Enumerable.Intersect，因为我们 TriVertices 只有三个顶点，所以直接数吧
            let mutable count = 0

            for v1 in this.TriVertices do
                for v2 in face.TriVertices do
                    if v1 = v2 then
                        count <- count + 1

            count = 2
