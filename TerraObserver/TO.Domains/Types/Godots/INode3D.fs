namespace TO.Domains.Types.Godots

open System
open System.Runtime.InteropServices
open Godot

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-16 16:51:16
[<Interface>]
type INode3D =
    inherit INode
    abstract Transform: Transform3D with get, set // 59
    abstract GlobalTransform: Transform3D with get, set // 74
    abstract Position: Vector3 with get, set // 89
    abstract Rotation: Vector3 with get, set // 106
    abstract RotationDegrees: Vector3 with get, set // 121
    abstract Quaternion: Quaternion with get, set // 136
    abstract Basis: Basis with get, set // 151
    abstract Scale: Vector3 with get, set // 168
    abstract GlobalPosition: Vector3 with get, set // 228
    abstract GlobalBasis: Basis with get, set // 243
    abstract Visible: bool with get, set // 289
    abstract GetWorld3D: unit -> World3D // 676
    abstract Show: unit -> unit // 815
    abstract Hide: unit -> unit // 826
    abstract Rotate: axis: Vector3 * angle: float32 -> unit // 881
    abstract GlobalRotate: axis: Vector3 * angle: float32 -> unit // 892
    abstract RotateX: angle: float32 -> unit // 958

    abstract LookAt:
        target: Vector3 *
        [<Optional; DefaultParameterValue(Nullable<Vector3>())>] up: Vector3 Nullable *
        [<Optional; DefaultParameterValue(false)>] useModelFront: bool ->
            unit // 1031

    abstract ToLocal: globalPoint: Vector3 -> Vector3 // 1056
    abstract ToGlobal: localPoint: Vector3 -> Vector3 // 1067
