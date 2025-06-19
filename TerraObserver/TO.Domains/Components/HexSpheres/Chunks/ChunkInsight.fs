namespace TO.Domains.Components.HexSpheres.Chunks

open Friflo.Engine.ECS

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-09 09:43:09
[<Struct>]
type ChunkInsight =
    interface IComponent
    val Insight: bool
    new(insight) = { Insight = insight }
