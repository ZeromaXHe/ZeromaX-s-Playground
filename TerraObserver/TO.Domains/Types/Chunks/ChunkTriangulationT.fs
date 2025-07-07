namespace TO.Domains.Types.Chunks

open Friflo.Engine.ECS

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-30 07:56:30
type Triangulate = IChunk -> Entity -> unit

[<Interface>]
type IChunkTriangulationCommand =
    abstract Triangulate: Triangulate
