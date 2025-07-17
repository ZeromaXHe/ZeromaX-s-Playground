namespace TO.Domains.Types.HexSpheres.Components.Faces

open Friflo.Engine.ECS
open Godot

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
