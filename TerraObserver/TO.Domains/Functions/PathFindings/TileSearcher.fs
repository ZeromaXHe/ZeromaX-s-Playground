namespace TO.Domains.Functions.PathFindings

open TO.Domains.Types.Friflos
open TO.Domains.Types.HexSpheres.Components.Tiles
open TO.Domains.Types.PathFindings

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-25 09:16:25
module TileSearcherCommand =
    let initSearchData (env: 'E when 'E :> IEntityStoreQuery and 'E :> ITileSearcherQuery) : InitSearchData =
        fun () ->
            let this = env.TileSearcher
            this.SearchData <- Array.zeroCreate <| env.Query<TileUnitCentroid>().Count + 1
            this.SearchFrontier <- None

    let refreshTileSearchData (env: #ITileSearcherQuery) : RefreshTileSearchData =
        fun tileCountId -> env.TileSearcher.SearchData[tileCountId.CountId].SearchPhase <- 0
