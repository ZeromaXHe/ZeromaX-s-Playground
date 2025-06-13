namespace TO.FSharp.Repos.Types.HexSpheres.ChunkRepoT

open Friflo.Engine.ECS
open Godot
open Godot.Abstractions.Extensions.Chunks
open TO.FSharp.Repos.Models.HexSpheres.Chunks
open TO.FSharp.Repos.Models.HexSpheres.Points
open TO.FSharp.Repos.Models.HexSpheres.Tiles

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-30 10:38:30
type AddChunk = PointId -> Vector3 -> NeighborCenterIds -> ChunkId
type TruncateChunks = unit -> unit
type GetChunkLodEnumById = ChunkId -> ChunkLodEnum
type GetChunkPosById = ChunkId -> Vector3
type ForEachChunkPos = ChunkPos ForEachEntity -> unit
type UpdateChunkInsightAndLodById = ChunkId -> bool -> ChunkLodEnum -> bool -> unit
type CommitChunkCommands = unit -> unit

type ChunkRepoDep =
    { Add: AddChunk
      Truncate: TruncateChunks
      GetLodEnumById: GetChunkLodEnumById
      GetPosById: GetChunkPosById
      ForEachPos: ForEachChunkPos
      UpdateInsightAndLodById: UpdateChunkInsightAndLodById
      CommitCommands: CommitChunkCommands }
