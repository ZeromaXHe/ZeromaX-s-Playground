namespace TO.FSharp.Repos.Models.HexSpheres.Points

open Friflo.Engine.ECS
open Godot
open TO.FSharp.Commons.Structs.HexSphereGrid

// ILinkRelation 要求每个目标实体只有一个链接关系！试验中发现多个的情况下，LinkRelations.[] 会混乱获取到外面的实体！
// 需要注意 struct 的成员必须是 array 而不是 list，否则没法有默认值，从而无法生成默认的无参构造函数
// 进而索引 IIndexedComponent 无法正常使用（所以要用的话，按这样使用：val FaceIds: int array）
// 这里直接用 ILinkRelation 连接面实体，是因为我们不在乎面的顺序
[<Struct>]
type PointToFaceId =
    interface int IRelation with
        member this.GetRelationKey() = this.FaceId

    val FaceId: int
    new(faceId: int) = { FaceId = faceId }

/// 点组件<br/>
/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-17 10:44:17
[<Struct>]
type PointComponent =
    interface Vector3 IIndexedComponent with
        member this.GetIndexedValue() = this.Position

    val Position: Vector3
    val Coords: SphereAxial
    new(position: Vector3, coords: SphereAxial) = { Position = position; Coords = coords }

type Chunky = bool
