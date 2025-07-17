namespace TO.Domains.Types.HexSpheres

open Godot
open TO.Domains.Types.DataStructures

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-19 15:08:19
type ChunkyVpTrees =
    { ChunkVpTree: Vector3 VpTree
      TileVpTree: Vector3 VpTree }

type ChooseVpTreeByChunky = Chunky -> Vector3 VpTree

[<Interface>]
type IChunkyVpTreesQuery =
    abstract ChunkyVpTrees: ChunkyVpTrees
    abstract ChooseVpTreeByChunky: ChooseVpTreeByChunky
