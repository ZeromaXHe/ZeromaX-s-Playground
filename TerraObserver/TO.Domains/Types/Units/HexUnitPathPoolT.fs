namespace TO.Domains.Types.Units

open Godot
open TO.Domains.Types.Godots
open TO.Domains.Types.HexSpheres

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-07-11 09:47:11
[<Interface>]
[<AllowNullLiteral>]
type IHexUnitPathPool =
    inherit INode3D
    // =====【Export】=====
    abstract PathScene: PackedScene
    abstract InstantiatePath: unit -> IHexUnitPath
    // =====【普通属性】=====
    abstract Paths: IHexUnitPath ResizeArray

[<Interface>]
type IHexUnitPathPoolQuery =
    abstract HexUnitPathPoolOpt: IHexUnitPathPool option

type NewUnitPathTask = TileId array -> IHexUnitPath option

[<Interface>]
type IHexUnitPathPoolCommand =
    abstract NewUnitPathTask: NewUnitPathTask
