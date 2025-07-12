namespace TO.Domains.Types.Units

open Godot
open TO.Domains.Types.Godots

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-07-11 09:46:11
[<Interface>]
[<AllowNullLiteral>]
type IHexUnitPath =
    inherit IPath3D
    // =====【普通属性】=====
    abstract Working: bool with get, set
    abstract TileIds: int array with get, set
    abstract Progresses: float32 array with get, set
    // =====【on-ready】=====
    abstract PathFollow: PathFollow3D
    abstract RemoteTransform: RemoteTransform3D
    abstract View: CsgPolygon3D
