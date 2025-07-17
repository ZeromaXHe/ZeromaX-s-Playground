namespace TO.Domains.Types.HexSpheres.Components.Tiles

open Friflo.Engine.ECS

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-09 16:37:09
[<Struct>]
type TileValue =
    interface IComponent
    val Values: int
    new(values) = { Values = values }
