namespace TO.Repos.Commands.PathFindings

open Friflo.Engine.ECS
open TO.Domains.Components.HexSpheres.Tiles
open TO.Repos.Data.PathFindings

type InitSearchData = unit -> unit

[<Interface>]
type ITileSearcherCommand =
    abstract InitSearchData: InitSearchData

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-25 09:16:25
module TileSearcherCommand =
    let initSearchData (store: EntityStore) (tileSearcher: TileSearcher) : InitSearchData =
        fun () -> tileSearcher.InitSearchData <| store.Query<TileUnitCentroid>().Count
