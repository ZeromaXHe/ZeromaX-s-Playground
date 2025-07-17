namespace TO.Domains.Types.Godots

open Godot

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-07-11 10:07:11
[<Interface>]
[<AllowNullLiteral>]
type IPath3D =
    inherit INode3D
    abstract Curve: Curve3D with get, set // 18
