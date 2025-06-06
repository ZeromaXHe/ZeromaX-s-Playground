namespace TO.FSharp.Repos.Types.TileRepoT

open Friflo.Engine.ECS
open TO.FSharp.Repos.Models.HexSpheres.Faces
open TO.FSharp.Repos.Models.HexSpheres.Points
open TO.FSharp.Repos.Models.HexSpheres.Tiles

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-30 10:35:30
type HexFaces = FaceComponent array
type AddTile = CenterId -> ChunkId -> HexFaces -> HexFaceIds -> NeighborCenterIds -> int
type CentroidAndCornersSeq = unit -> (TileUnitCentroid * TileUnitCorners) seq
type TruncateTiles = unit -> unit

type TileRepoDep =
    { Add: AddTile
      CentroidAndCornersSeq: CentroidAndCornersSeq
      Truncate: TruncateTiles }
