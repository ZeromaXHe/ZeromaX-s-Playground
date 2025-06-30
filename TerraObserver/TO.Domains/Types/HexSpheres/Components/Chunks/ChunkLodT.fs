namespace TO.Domains.Types.HexSpheres.Components.Chunks

open Friflo.Engine.ECS
open TO.Domains.Types.HexSpheres

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-08 13:30:08
[<Struct>]
type ChunkLod =
    interface IComponent
    val Lod: ChunkLodEnum
    new(lod) = { Lod = lod }
