namespace TO.Repos.Data.PathFindings

open TO.Domains.Alias.HexSpheres.Tiles
open TO.Domains.Structs.PathFindings

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-07 09:10:07
type TileSearcher() =
    member val SearchData: TileSearchData array = null with get, set
    member val SearchFrontier: TilePriorityQueue option = None with get, set
    member val SearchFrontierPhase = 0 with get, set
    member val CurrentPathFromId = -1 with get, set
    member val CurrentPathToId = -1 with get, set
    member val HasPath = false with get, set
