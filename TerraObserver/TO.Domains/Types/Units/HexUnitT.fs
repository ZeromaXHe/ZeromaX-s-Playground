namespace TO.Domains.Types.Units

open Godot
open TO.Domains.Types.Godots
open TO.Domains.Types.HexSpheres

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-07-11 09:46:11
[<Interface>]
type IHexUnit =
    inherit ICsgBox3D
    // =====【普通属性】=====
    abstract Id: int with get, set
    abstract BeginRotation: Vector3 with get, set
    abstract Orientation: float32 with get, set
    abstract Path: IHexUnitPath with get, set
    abstract PathTileIdx: int with get, set
    abstract PathOriented: bool with get, set

type OnHexUnitProcessed = float32 -> IHexUnit -> unit
type ValidateUnitLocation = IHexUnit -> unit
type ChangeUnitTileId = TileId -> IHexUnit -> unit
type KillUnit = IHexUnit -> unit
type TravelUnit = IHexUnit -> IHexUnitPath -> unit

[<Interface>]
type IHexUnitCommand =
    abstract OnHexUnitProcessed: OnHexUnitProcessed
    abstract ValidateUnitLocation: ValidateUnitLocation
    abstract ChangeUnitTileId: ChangeUnitTileId
    abstract KillUnit: KillUnit
    abstract TravelUnit: TravelUnit
