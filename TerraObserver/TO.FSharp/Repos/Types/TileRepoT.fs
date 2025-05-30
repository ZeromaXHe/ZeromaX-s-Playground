namespace TO.FSharp.Repos.Types.TileRepoT

open Friflo.Engine.ECS
open TO.FSharp.Repos.Models.HexSpheres.Tiles

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-30 10:35:30

type TryHeadTileByCenterId = CenterId -> Entity option
type AddTile = CenterId -> ChunkId -> HexFaces -> HexFaceIds -> NeighborCenterIds -> int
type AllTilesSeq = unit -> TileComponent seq
type TruncateTiles = unit -> unit

type TileRepoDep =
    { TryHeadByCenterId: TryHeadTileByCenterId
      Add: AddTile
      AllSeq: AllTilesSeq
      Truncate: TruncateTiles }
