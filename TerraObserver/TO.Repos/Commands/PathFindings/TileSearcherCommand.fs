namespace TO.Repos.Commands.PathFindings

open Friflo.Engine.ECS
open TO.Domains.Components.HexSpheres.Tiles
open TO.Repos.Data.PathFindings

type InitSearchData = unit -> unit
type RefreshTileSearchData = TileCountId -> unit

[<Interface>]
type ITileSearcherCommand =
    abstract InitSearchData: InitSearchData
    abstract RefreshTileSearchData: RefreshTileSearchData

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-25 09:16:25
module TileSearcherCommand =
    let initSearchData (store: EntityStore) (this: TileSearcher) : InitSearchData =
        fun () ->
            this.SearchData <- Array.zeroCreate <| store.Query<TileUnitCentroid>().Count + 1
            this.SearchFrontier <- None

    let refreshTileSearchData (this: TileSearcher) : RefreshTileSearchData =
        fun tileCountId -> this.SearchData[tileCountId.CountId].SearchPhase <- 0
