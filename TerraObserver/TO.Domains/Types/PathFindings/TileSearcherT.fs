namespace TO.Domains.Types.PathFindings

open Friflo.Engine.ECS
open TO.Domains.Types.HexSpheres
open TO.Domains.Types.HexSpheres.Components.Tiles

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

[<Interface>]
type ITileSearcherQuery =
    abstract TileSearcher: TileSearcher

type InitSearchData = unit -> unit
type RefreshTileSearchData = TileCountId -> unit
type GetVisibleTiles = Entity -> int -> TileId ResizeArray
type FindTileSearchPath = Entity -> Entity -> bool -> Entity ResizeArray
type ClearTileSearchPath = unit -> unit

[<Interface>]
type ITileSearcherCommand =
    abstract InitSearchData: InitSearchData
    abstract RefreshTileSearchData: RefreshTileSearchData
    abstract GetVisibleTiles: GetVisibleTiles
    abstract FindTileSearchPath: FindTileSearchPath
    abstract ClearTileSearchPath: ClearTileSearchPath
