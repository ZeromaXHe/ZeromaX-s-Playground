namespace TO.Domains.Types.HexSpheres.Components.Points

open Friflo.Engine.ECS
open Godot
open TO.Domains.Types.HexGridCoords

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
