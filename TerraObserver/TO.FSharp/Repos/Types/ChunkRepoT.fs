namespace TO.FSharp.Repos.Types.ChunkRepoT

open Godot
open TO.FSharp.Repos.Models.HexSpheres.Points

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-30 10:38:30
type AddChunk = CenterId -> Vector3 -> NeighborCenterIds -> int
type TruncateChunks = unit -> unit

type ChunkRepoDep =
    { Add: AddChunk
      Truncate: TruncateChunks }
