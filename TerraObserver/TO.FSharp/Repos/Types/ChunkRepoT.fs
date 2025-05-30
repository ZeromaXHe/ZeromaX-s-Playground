namespace TO.FSharp.Repos.Types.ChunkRepoT

open Friflo.Engine.ECS
open Godot
open TO.FSharp.Repos.Models.HexSpheres.Tiles

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-30 10:38:30
type TryHeadChunkByCenterId = CenterId -> Entity option
type AddChunk = CenterId -> Vector3 -> NeighborCenterIds -> int
type TruncateChunks = unit -> unit

type ChunkRepoDep =
    { TryHeadByCenterId: TryHeadChunkByCenterId
      Add: AddChunk
      Truncate: TruncateChunks }
