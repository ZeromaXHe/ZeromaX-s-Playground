namespace TO.Domains.Types.Chunks

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-30 07:56:30
type Triangulate = IChunk -> int -> unit

[<Interface>]
type IChunkTriangulationCommand =
    abstract Triangulate: Triangulate
