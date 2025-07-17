namespace TO.Domains.Types.Units

open Friflo.Engine.ECS
open TO.Domains.Types.HexSpheres

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-07-12 10:47:12
[<Struct>]
type UnitComponent =
    interface IComponent
    val TileId: TileId
    new(tileId: TileId) = { TileId = tileId }