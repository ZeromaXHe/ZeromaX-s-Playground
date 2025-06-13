namespace TO.FSharp.Repos.Models.PathFindings

open TO.FSharp.Repos.Models.HexSpheres.Tiles

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-07 09:10:07
type TileSearcher() =
    let mutable searchData: TileSearchData array = null
    let mutable searchFrontier: TilePriorityQueue option = None
    let mutable searchFrontierPhase = 0
    let mutable currentPathFromId = -1
    let mutable currentPathToId = -1
    let mutable hasPath = false

    member this.InitSearchData(tileCount: int) =
        searchData <- Array.zeroCreate <| tileCount + 1
        searchFrontier <- None

    member this.RefreshTileSearchData(tileId: TileId) = searchData[tileId].SearchPhase <- 0
