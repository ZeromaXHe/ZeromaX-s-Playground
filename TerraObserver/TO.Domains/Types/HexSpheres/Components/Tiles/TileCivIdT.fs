namespace TO.Domains.Types.HexSpheres.Components.Tiles

open Friflo.Engine.ECS

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-06 21:07:06
[<Struct>]
type TileCivId =
    interface IComponent
    val CivId: int
    new(civId: int) = { CivId = civId }
