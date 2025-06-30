namespace TO.Domains.Types.HexSpheres.Components.Tiles

open Friflo.Engine.ECS
open TO.Domains.Types.HexSpheres

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-09 16:47:09
[<Struct>]
type TileFlag =
    interface IComponent
    val Flag: TileFlagEnum
    new(flag) = { Flag = flag }
