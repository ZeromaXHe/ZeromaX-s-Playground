namespace TO.Domains.Types.Godots

open Godot

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-17 14:15:17
[<Interface>]
[<AllowNullLiteral>]
type IGeometryInstance3D =
    inherit IVisualInstance3D
    abstract MaterialOverride: Material with get, set // 104
