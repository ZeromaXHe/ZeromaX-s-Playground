namespace TO.Domains.Types.HexSpheres.Components.Tiles

open Friflo.Engine.ECS

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-07-11 21:54:11
[<Struct>]
type TileUnitId =
    interface IComponent
    val UnitId: int
    new(unitId: int) = { UnitId = unitId }
