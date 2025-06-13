namespace TO.FSharp.Repos.Types.HexSpheres.TileRepoT

open Friflo.Engine.ECS
open TO.FSharp.Repos.Models.HexSpheres.Faces
open TO.FSharp.Repos.Models.HexSpheres.Points
open TO.FSharp.Repos.Models.HexSpheres.Tiles

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-30 10:35:30
type HexFaces = FaceComponent array
type AddTile = PointId -> ChunkId -> HexFaces -> HexFaceIds -> NeighborCenterIds -> TileId
type CountTile = unit -> int
type CentroidAndCornersSeq = unit -> (TileUnitCentroid * TileUnitCorners) seq
type ForEachTileByChunkId = ChunkId -> TileChunkId ForEachEntity -> unit
type GetTileChunkIdById = TileId -> TileChunkId
type GetTileUnitCentroidById = TileId -> TileUnitCentroid
type GetTileUnitCornersById = TileId -> TileUnitCorners
type GetTileHexFaceIdsById = TileId -> TileHexFaceIds
type GetTileValueById = TileId -> TileValue
type GetTileFlagById = TileId -> TileFlag
type TruncateTiles = unit -> unit

type TileRepoDep =
    { Add: AddTile
      Count: CountTile
      CentroidAndCornersSeq: CentroidAndCornersSeq
      ForEachByChunkId: ForEachTileByChunkId
      GetChunkIdById: GetTileChunkIdById
      GetUnitCentroidById: GetTileUnitCentroidById
      GetUnitCornersById: GetTileUnitCornersById
      GetHexFaceIdsById: GetTileHexFaceIdsById
      GetValueById: GetTileValueById
      GetFlagById: GetTileFlagById
      Truncate: TruncateTiles }
