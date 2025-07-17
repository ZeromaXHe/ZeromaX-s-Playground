namespace TO.Domains.Types.Units

open System
open System.Collections.Generic
open Godot
open TO.Domains.Types.Godots
open TO.Domains.Types.HexSpheres

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-07-11 18:55:11
[<Interface>]
[<AllowNullLiteral>]
type IUnitManager =
    inherit INode3D
    // =====【Export】=====
    abstract UnitScene: PackedScene
    abstract InstantiateUnit: unit -> IHexUnit
    // =====【普通属性】=====
    abstract Units: Dictionary<int, IHexUnit>
    abstract PathFromTileId: int with get, set

[<Interface>]
type IUnitManagerQuery =
    abstract UnitManagerOpt: IUnitManager option

type AddUnit = TileId -> float32 -> unit
type RemoveUnit = int -> unit
type ValidateUnitLocationById = int -> unit
type FindUnitPath = TileId Nullable -> unit
type ClearAllUnits = unit -> unit

[<Interface>]
type IUnitManagerCommand =
    abstract AddUnit: AddUnit
    abstract RemoveUnit: RemoveUnit
    abstract ValidateUnitLocationById: ValidateUnitLocationById
    abstract FindUnitPath: FindUnitPath
    abstract ClearAllUnits: ClearAllUnits
