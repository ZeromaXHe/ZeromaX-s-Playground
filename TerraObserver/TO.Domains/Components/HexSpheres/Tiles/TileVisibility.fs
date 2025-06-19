namespace TO.Domains.Components.HexSpheres.Tiles

open Friflo.Engine.ECS

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-17 13:08:17
[<Struct>]
type TileVisibility =
    interface IComponent
    val Visibility: int
    new(visibility: int) = { Visibility = visibility }

    member this.IsVisible(tileFlag: TileFlag) =
        this.Visibility > 0 && tileFlag.IsExplorable
