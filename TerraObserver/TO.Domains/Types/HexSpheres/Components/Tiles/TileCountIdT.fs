namespace TO.Domains.Types.HexSpheres.Components.Tiles

open Friflo.Engine.ECS

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-17 07:04:17
[<Struct>]
type TileCountId =
    interface IComponent

    val CountId: int
    new(countId: int) = { CountId = countId }
