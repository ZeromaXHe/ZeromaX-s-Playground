namespace TO.Repos.Data.HexSpheres

open Godot
open TO.Repos.Data.Commons

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-19 15:08:19
type ChunkyVpTrees =
    { ChunkVpTree: Vector3 VpTree
      TileVpTree: Vector3 VpTree }

    member this.Choose chunky =
        if chunky then this.ChunkVpTree else this.TileVpTree

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-19 15:57:19
[<Interface>]
type IChunkyVpTrees =
    abstract ChunkyVpTrees: ChunkyVpTrees
