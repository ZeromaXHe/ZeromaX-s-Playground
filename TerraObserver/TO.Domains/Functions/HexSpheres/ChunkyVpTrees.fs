namespace TO.Domains.Functions.HexSpheres

open TO.Domains.Types.HexSpheres

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-29 19:46:29
module ChunkyVpTreesQuery =
    let chooseByChunky (env: #IChunkyVpTreesQuery) : ChooseVpTreeByChunky =
        fun chunky ->
            if chunky then
                env.ChunkyVpTrees.ChunkVpTree
            else
                env.ChunkyVpTrees.TileVpTree
