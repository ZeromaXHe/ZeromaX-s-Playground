namespace TO.Domains.Types.Godots

open Godot

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-17 14:15:17
[<Interface>]
type IMeshInstance3D =
    inherit IGeometryInstance3D
    abstract Mesh: Mesh with get, set // 17
    abstract CreateTrimeshCollision: unit -> unit // 203
