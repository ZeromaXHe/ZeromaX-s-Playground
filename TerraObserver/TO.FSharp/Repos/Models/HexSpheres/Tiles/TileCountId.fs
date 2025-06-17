namespace TO.FSharp.Repos.Models.HexSpheres.Tiles

open Friflo.Engine.ECS

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-17 07:04:17
[<Struct>]
type TileCountId =
    interface int IIndexedComponent with
        override this.GetIndexedValue() = this.CountId

    val CountId: int
    new(countId: int) = { CountId = countId }
