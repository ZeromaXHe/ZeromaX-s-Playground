namespace TO.FSharp.GodotAbstractions.Bases

open Godot

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-28 23:03:28
[<Interface>]
type IMeshInstance3D =
    inherit IGeometryInstance3D
    abstract Mesh: Mesh with get, set
